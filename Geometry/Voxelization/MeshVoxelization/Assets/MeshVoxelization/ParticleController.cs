using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    public class ParticleController : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetRadius(float radius)
        {
            Vector3 scale = new Vector3(radius, radius, radius);
            this.transform.localScale = scale;
        }

        public void SetLocalPos(Vector3 pos)
        {
            this.transform.localPosition = pos;
        }
    }
}
