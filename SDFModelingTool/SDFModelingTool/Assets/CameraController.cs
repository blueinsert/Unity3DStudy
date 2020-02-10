using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bluebean
{
    /// <summary>
    /// 用于旋转观察物体的摄像机控制
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera Camera;
        public float MinDistance = 1f;
        public float MaxDistance = 40f;
        public Vector3 ObservePosition = new Vector3(0, 0, 0);
        public float m_speed = 5f;
        public float m_rotateSpeed = 15f;

        private float m_distance = 10f;
        private Vector3 m_cameraR;
        
        void Start()
        {
            Camera = this.GetComponent<Camera>();
            Camera.transform.LookAt(ObservePosition);
            m_cameraR = new Vector3(180+Camera.transform.rotation.eulerAngles.x, 180+Camera.transform.rotation.eulerAngles.y, 0);
            m_distance = (transform.position - ObservePosition).magnitude;
        }

        void Update()
        {
            Vector3 Face = transform.rotation * Vector3.forward;
            Face = Face.normalized;

            if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftAlt))
            {
                float yRot = Input.GetAxis("Mouse X") * m_rotateSpeed;
                float xRot = -Input.GetAxis("Mouse Y") * m_rotateSpeed;

                Vector3 R = m_cameraR + new Vector3(xRot, yRot, 0f);

                m_cameraR = Vector3.Slerp(m_cameraR, R, m_speed * Time.deltaTime);

                var front = Quaternion.Euler(m_cameraR) * Vector3.forward;
                front.Normalize();
                transform.position = ObservePosition + front * m_distance;

                transform.LookAt(ObservePosition);
            }

            if (Input.GetKey("w"))
            {
                transform.position += Face * m_speed * Time.deltaTime;
                m_distance = (transform.position - ObservePosition).magnitude;
            }

            if (Input.GetKey("s"))
            {
                transform.position -= Face * m_speed * Time.deltaTime;
                m_distance = (transform.position - ObservePosition).magnitude;
            }

            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                transform.position -= Face * m_speed * Time.deltaTime;
                m_distance = (transform.position - ObservePosition).magnitude;

            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                transform.position += Face * m_speed * Time.deltaTime;
                m_distance = (transform.position - ObservePosition).magnitude;
            }

        }


    }
}
