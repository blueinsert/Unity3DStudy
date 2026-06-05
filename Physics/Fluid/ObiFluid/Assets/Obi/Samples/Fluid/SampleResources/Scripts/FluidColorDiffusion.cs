using UnityEngine;

namespace Obi.Samples
{
    [RequireComponent(typeof(ObiSolver))]
    public class FluidColorDiffusion : MonoBehaviour
    {
        public ComputeShader diffuseColorsShader;
        ObiSolver solver;

        void OnEnable()
        {
            solver = GetComponent<ObiSolver>();
            solver.OnSubstepsStart += Solver_OnSubstepsStart; 
            solver.OnRequestReadback += Solver_OnRequestReadback;
        }

        void OnDisable()
        {
            solver.OnSubstepsStart -= Solver_OnSubstepsStart;
            solver.OnRequestReadback -= Solver_OnRequestReadback;
        }

        // color to user data.
        private void Solver_OnSubstepsStart(ObiSolver solv, float timeToSimulate, float substepTime)
        {
            if (solver.backendType == ObiSolver.BackendType.Compute)
            {
                int threadGroups = ComputeMath.ThreadGroupCount(solver.allocParticleCount, 128);
                diffuseColorsShader.SetInt("particleCount", solver.allocParticleCount);
                diffuseColorsShader.SetFloat("deltaTime", timeToSimulate);
                diffuseColorsShader.SetBuffer(0, "userData", solver.userData.computeBuffer);
                diffuseColorsShader.SetBuffer(0, "colors", solver.colors.computeBuffer);
                diffuseColorsShader.SetBuffer(0, "velocities", solver.velocities.computeBuffer);
                diffuseColorsShader.Dispatch(0, threadGroups, 1, 1);
            }
        }

        // user data to color.
        private void Solver_OnRequestReadback(ObiSolver solv)
        {
            if (solver.backendType == ObiSolver.BackendType.Compute)
            {
                int threadGroups = ComputeMath.ThreadGroupCount(solver.allocParticleCount, 128);
                diffuseColorsShader.SetInt("particleCount", solver.allocParticleCount);
                diffuseColorsShader.SetBuffer(1, "userData", solver.userData.computeBuffer);
                diffuseColorsShader.SetBuffer(1, "colors", solver.colors.computeBuffer);
                diffuseColorsShader.Dispatch(1, threadGroups, 1, 1);
            }
        }
    }
}