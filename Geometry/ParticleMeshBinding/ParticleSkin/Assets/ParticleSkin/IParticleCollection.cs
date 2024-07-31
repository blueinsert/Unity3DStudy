using UnityEngine;
using System.Collections;

namespace bluebean
{
    // Interface for classes that hold a collection of particles. Contains method to get common particle properties.
    public interface IParticleCollection 
    {
        int ParticleCount { get; }

        int GetParticleRuntimeIndex(int index); // returns solver index, depending on implementation.
        Vector3 GetParticlePosition(int index);
        float GetParticleRadius(int index);
        Color GetParticleColor(int index);
    }
}
