using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


namespace Obi
{
    public class ObiFluidBuiltInRendererFeature : MonoBehaviour
	{

        public ObiFluidRenderingPass[] passes;

        [Range(1, 4)]
        public int refractionDownsample = 1;

        [Range(1, 4)]
        public int thicknessDownsample = 1;

        [Min(0)]
        public float foamFadeDepth = 1.0f;

        protected Dictionary<Camera, CommandBuffer> cmdBuffers = new Dictionary<Camera, CommandBuffer>();

        private FluidRenderingUtils.FluidRenderTargets renderTargets;
        private Material m_TransmissionMaterial;
        private Material m_FoamMaterial;

        protected Material CreateMaterial(Shader shader)
        {
            if (!shader || !shader.isSupported)
                return null;
            Material m = new Material(shader);
            m.hideFlags = HideFlags.HideAndDontSave;
            return m;
        }

        public void OnEnable()
        {
            Setup();
            Camera.onPreRender += SetupFluidRendering;
        }

        public void OnDisable()
        {
            Camera.onPreRender -= SetupFluidRendering;
            Cleanup();
        }

        protected void Setup()
        {

            if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
            {
                enabled = false;
                Debug.LogWarning("Obi Fluid Renderer not supported in this platform.");
                return;
            }

            m_TransmissionMaterial = CreateMaterial(Shader.Find("Hidden/AccumulateTransmission"));
            m_FoamMaterial = CreateMaterial(Shader.Find("Obi/Foam"));

            renderTargets = new FluidRenderingUtils.FluidRenderTargets();
            renderTargets.refraction = Shader.PropertyToID("_CameraOpaqueTexture");
            renderTargets.transmission = Shader.PropertyToID("_FluidThickness");  // used to store total transmitted color (RGB) and total thickness (A) for all renderers.
            renderTargets.foam = Shader.PropertyToID("_Foam");                    // used to store underwater foam.
            renderTargets.surfaceDepth = Shader.PropertyToID("_TemporaryBuffer"); // buffer used to store fluid's front and back surface depth.
            renderTargets.surfaceColor = Shader.PropertyToID("_SurfaceColor");    // intermediate buffer used to store surface color and thickness for each renderer.
        }

		protected void Cleanup()
		{
            if (m_TransmissionMaterial != null)
                DestroyImmediate(m_TransmissionMaterial);

            if (m_FoamMaterial != null)
                DestroyImmediate(m_FoamMaterial);

            foreach (var entry in cmdBuffers)
                if (entry.Key != null)
                    entry.Key.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, entry.Value);

            cmdBuffers.Clear();
        }

        private void SetupFluidRendering(Camera cam)
        {
            if (cmdBuffers.TryGetValue(cam, out CommandBuffer cmdBuffer))
            {
                UpdateFluidRenderingCommandBuffer(cam, cmdBuffer);
            }
            else
            {
                cmdBuffer = new CommandBuffer();
                cmdBuffer.name = "Render fluid";
                cmdBuffers[cam] = cmdBuffer;

                cam.forceIntoRenderTexture = true;
                cam.depthTextureMode |= DepthTextureMode.Depth;
                UpdateFluidRenderingCommandBuffer(cam, cmdBuffer);
                cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, cmdBuffer);
            }
        }

        public void UpdateFluidRenderingCommandBuffer(Camera cam, CommandBuffer cmd)
		{
			cmd.Clear();
	
			if (passes == null)
				return;

            // grab opaque contents:
            cmd.GetTemporaryRT(renderTargets.refraction, -refractionDownsample, -refractionDownsample, 0, FilterMode.Bilinear);
            cmd.Blit(BuiltinRenderTextureType.CameraTarget, renderTargets.refraction);

            // TODO: get temporary buffer (needed in order to store intermediate color/thickness for each pass, without overwriting surface depth.)
            cmd.GetTemporaryRT(renderTargets.surfaceColor, -thicknessDownsample, -thicknessDownsample, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);

            // get temporary buffer with depth support:
            cmd.GetTemporaryRT(renderTargets.surfaceDepth, -thicknessDownsample, -thicknessDownsample, 16, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);

            // get transmission buffer, color only:
            cmd.GetTemporaryRT(renderTargets.transmission, -thicknessDownsample, -thicknessDownsample, 0, FilterMode.Bilinear, RenderTextureFormat.ARGBHalf);

            // get foam RT:
            cmd.GetTemporaryRT(renderTargets.foam, -refractionDownsample, -refractionDownsample, 0, FilterMode.Bilinear);

            // render fluid surface front/back depth:
            cmd.SetRenderTarget(renderTargets.surfaceDepth);
            cmd.ClearRenderTarget(true, true, Color.clear);
            for (int i = 0; i < passes.Length; ++i)
            {
                if (passes[i] != null && passes[i].renderers.Count > 0)
                {
                    var fluidMesher = passes[i].renderers[0];
                    if (fluidMesher.actor.isLoaded)
                    {
                        // fluid mesh renders surface onto surface buffer
                        var renderSystem = fluidMesher.actor.solver.GetRenderSystem<ObiFluidSurfaceMesher>() as IFluidRenderSystem;
                        if (renderSystem != null)
                            renderSystem.RenderSurface(cmd, passes[i], fluidMesher);
                    }
                }
            }

            // prepare foam RT:
            cmd.SetRenderTarget(renderTargets.foam);
            cmd.ClearRenderTarget(false, true, Color.clear);

            // prepare transmission RT:
            cmd.SetRenderTarget(renderTargets.transmission);
            cmd.ClearRenderTarget(false, true, FluidRenderingUtils.transmissionBufferClear);

            // render each pass (there's only one mesh per pass) onto temp buffer to calculate its color and thickness.
            for (int i = 0; i < passes.Length; ++i)
            {
                if (passes[i] != null && passes[i].renderers.Count > 0)
                {
                    var fluidMesher = passes[i].renderers[0];
                    if (fluidMesher.actor.isLoaded)
                    {
                        cmd.SetRenderTarget(renderTargets.surfaceColor);
                        cmd.ClearRenderTarget(false, true, FluidRenderingUtils.thicknessBufferClear);

                        // fluid mesh renders absorption color and thickness onto temp buffer:
                        var renderSystem = fluidMesher.actor.solver.GetRenderSystem<ObiFluidSurfaceMesher>() as IFluidRenderSystem;
                        if (renderSystem != null)
                            renderSystem.RenderVolume(cmd, passes[i], fluidMesher);

                        // render foam here, reading color and thickness of fluid surface.
                        cmd.SetRenderTarget(renderTargets.foam);
                        for (int j = 0; j < passes[i].renderers.Count; ++j)
                        {
                            if (passes[i].renderers[j].TryGetComponent(out ObiFoamGenerator _))
                            {
                                var solver = passes[i].renderers[j].actor.solver;
                                if (solver != null)
                                {
                                    var rend = solver.GetRenderSystem<ObiFoamGenerator>() as ObiFoamRenderSystem;

                                    if (rend != null)
                                    {
                                        m_FoamMaterial.SetFloat("_FadeDepth", foamFadeDepth);
                                        m_FoamMaterial.SetFloat("_VelocityStretching", solver.maxFoamVelocityStretch);
                                        m_FoamMaterial.SetFloat("_FadeIn", solver.foamFade.x);
                                        m_FoamMaterial.SetFloat("_FadeOut", solver.foamFade.y);
                                        m_FoamMaterial.SetFloat("_Thickness", passes[i].renderers[j].pass.thickness);
                                        m_FoamMaterial.SetVector("_Turbidity", passes[i].renderers[j].pass.turbidity);
                                        cmd.DrawMesh(rend.renderBatch.mesh, solver.transform.localToWorldMatrix, m_FoamMaterial);
                                    }
                                }
                            }
                        }

                        // calculate transmission from thickness & absorption and accumulate onto transmission buffer.
                        cmd.SetGlobalFloat("_Thickness", passes[i].thickness);
                        cmd.Blit(renderTargets.surfaceColor, renderTargets.transmission, m_TransmissionMaterial, 0);
                    }
                }
            }
        }
    }
}

