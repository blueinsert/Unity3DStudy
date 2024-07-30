using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public partial class ParticleTopology : MonoBehaviour, IParticleCollection
    {
        
        [HideInInspector][SerializeField] protected Bounds _bounds = new Bounds();
        [HideInInspector] public Vector3[] positions = null;           /**< Particle positions.*/
        [HideInInspector] public float[] invMasses = null;             /**< Particle inverse masses*/
        [HideInInspector] public float[] radius = null;      /**< Particle ellipsoid principal radii. These are the ellipsoid radius in each axis.*/
        [HideInInspector] public Color[] colors = null;                /**< Particle colors (not used by all actors, can be null)*/
        /** Simplices **/
        [HideInInspector] public int[] points = null;
        [HideInInspector] public int[] edges = null;
        [HideInInspector] public int[] triangles = null;

        public int ParticleCount => positions == null ? 0 : positions.Length;

        public Color GetParticleColor(int index)
        {
            return colors[index];
        }

        public Vector3 GetParticlePosition(int index)
        {
            return positions[index];
        }

        public float GetParticleRadius(int index)
        {
            return radius[index];
        }

        public int GetParticleRuntimeIndex(int index)
        {
            return index;
        }
    }
}
