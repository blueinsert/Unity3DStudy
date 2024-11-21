using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class CurvePathProvider : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual List<CurvePathPoint> GetPath()
        {
            return null;
        }

        public virtual float RestLength { get { return 0f; } }
    }
}
