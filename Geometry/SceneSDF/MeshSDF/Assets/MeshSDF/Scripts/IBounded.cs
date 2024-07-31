using System;
using UnityEngine;

namespace bluebean
{
    public interface IBounded
    {
        Aabb GetBounds();
    }
}
