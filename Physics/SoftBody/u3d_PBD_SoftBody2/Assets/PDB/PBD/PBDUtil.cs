using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace bluebean.UGFramework.Physics
{
    public static class PBDUtil
    {
        public static bool IsParticleFixed(float4 property)
        {
            return property.x >= 1;
        }
    }
}
