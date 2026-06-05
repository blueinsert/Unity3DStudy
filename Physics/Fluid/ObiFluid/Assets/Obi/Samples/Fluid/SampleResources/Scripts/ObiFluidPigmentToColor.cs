using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Obi.Samples
{
	/**
	 * Sample script that colors fluid particles based on their vorticity (2D only)
	 */
	[RequireComponent(typeof(ObiEmitter))]
	public class ObiFluidPigmentToColor : MonoBehaviour
	{
		ObiEmitter em;

		void OnEnable(){
			em = GetComponent<ObiEmitter>();
            em.OnEmitParticle += Emitter_OnEmitParticle;
		}

        void OnDisable()
        {
            em.OnEmitParticle -= Emitter_OnEmitParticle;
        }

        private void Emitter_OnEmitParticle(ObiEmitter emitter, int particleIndex)
        {
            // upon emission, convert particle color to spectral representation and store it as user data.
            int index = emitter.solverIndices[particleIndex];
            emitter.solver.userData[index] = ObiUtils.RGBToAbsorption(emitter.solver.colors[index]);
        }

		void LateUpdate()
		{
			if (!isActiveAndEnabled || !em.isLoaded)
				return;

			for (int i = 0; i < em.solverIndices.count; ++i)
            {
                // convert user data back to RGB and set it as the color.
                int k = em.solverIndices[i];
                em.solver.colors[k] = ObiUtils.AbsorptionToRGB(em.solver.userData[k]);
			}
		}
	
	}
}

