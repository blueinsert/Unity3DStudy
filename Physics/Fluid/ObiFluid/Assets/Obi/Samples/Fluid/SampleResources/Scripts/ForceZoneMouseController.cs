using UnityEngine;
using Obi;

namespace Obi.Samples
{
    [RequireComponent(typeof(ObiForceZone))]
    public class ForceZoneMouseController : MonoBehaviour
    {
        ObiForceZone zone;
        Vector3 prevPos;

        public float intensity = 10;

        [Range(0, 1)]
        public float driveIntensityWithSpeed = 1;

        [Range(0,1)]
        public float colorChange = 0.998f;

        private float startDamping;

        // Start is called before the first frame update
        void Start()
        {
            zone = GetComponent<ObiForceZone>();
            startDamping = zone.damping;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

            if (Input.GetMouseButtonDown(0))
            {
                prevPos = pos;
                zone.color = Random.ColorHSV(0, 1, 0.98f, 0.98f, 0.98f, 0.98f, colorChange, colorChange);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 dir = (pos - prevPos) / Time.deltaTime;

                zone.transform.position = pos;
                zone.damping = startDamping;
                zone.intensity = Mathf.Lerp(intensity, Vector3.Magnitude(dir) * intensity, driveIntensityWithSpeed);
                if (zone.intensity > 0.0001)
                    zone.transform.rotation = Quaternion.LookRotation(Vector3.Normalize(dir), Vector3.forward);

                prevPos = pos;
            }
            else
            {
                zone.intensity = 0;
                zone.damping = 0;
                zone.color = Color.clear;
            }
        }
    }
}
