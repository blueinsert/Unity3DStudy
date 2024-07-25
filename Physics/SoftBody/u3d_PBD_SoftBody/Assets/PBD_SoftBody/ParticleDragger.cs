using System;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ParticlePicker))]
public class ObiParticleDragger : MonoBehaviour
{
    public float springStiffness = 500;
    public float springDamping = 50;
    public bool drawSpring = true;

    private LineRenderer lineRenderer;
    private ParticlePicker picker;
    private ParticlePicker.ParticlePickEventArgs pickArgs;

    void OnEnable()
    {
        lineRenderer = GetComponent<LineRenderer>();
        picker = GetComponent<ParticlePicker>();
        picker.OnParticlePicked.AddListener(Picker_OnParticleDragged);
        picker.OnParticleDragged.AddListener(Picker_OnParticleDragged);
        picker.OnParticleReleased.AddListener(Picker_OnParticleReleased);
    }

    void OnDisable()
    {
        picker.OnParticlePicked.RemoveListener(Picker_OnParticleDragged);
        picker.OnParticleDragged.RemoveListener(Picker_OnParticleDragged);
        picker.OnParticleReleased.RemoveListener(Picker_OnParticleReleased);
        lineRenderer.positionCount = 0;
    }

    void FixedUpdate()
    {
        PBDSolver solver = picker.solver;

        if (solver != null && pickArgs != null)
        {

            // Calculate picking position in solver space:
            Vector4 targetPosition = solver.transform.InverseTransformPoint(pickArgs.worldPosition);

            // Calculate effective inverse mass:
            float invMass = solver.GetParticleInvMass(pickArgs.actorId, pickArgs.particleIndex);

            if (invMass > 0)
            {
                // Calculate and apply spring force:
                Vector4 position = solver.GetParticlePosition(pickArgs.actorId, pickArgs.particleIndex);
                Vector4 velocity = solver.GetParticleVel(pickArgs.actorId, pickArgs.particleIndex);
                var force = ((targetPosition - position) * springStiffness - velocity * springDamping) / invMass;
                solver.SetParticleExternalForce(pickArgs.actorId, pickArgs.particleIndex, force);


                if (drawSpring)
                {
                    lineRenderer.positionCount = 2;
                    lineRenderer.SetPosition(0, pickArgs.worldPosition);
                    lineRenderer.SetPosition(1, solver.transform.TransformPoint(position));
                }
                else
                {
                    lineRenderer.positionCount = 0;
                }
            }

        }
    }

    void Picker_OnParticleDragged(ParticlePicker.ParticlePickEventArgs e)
    {
        pickArgs = e;
    }

    void Picker_OnParticleReleased(ParticlePicker.ParticlePickEventArgs e)
    {
        pickArgs = null;
        lineRenderer.positionCount = 0;
    }

}


