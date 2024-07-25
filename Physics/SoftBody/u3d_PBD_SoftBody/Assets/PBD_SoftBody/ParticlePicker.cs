using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ParticlePicker : MonoBehaviour
{

    public class ParticlePickEventArgs : EventArgs
    {
        public int actorId;
        public int particleIndex;
        public Vector3 worldPosition;

        public ParticlePickEventArgs(int actorId, int particleIndex, Vector3 worldPosition)
        {
            this.actorId = actorId;
            this.particleIndex = particleIndex;
            this.worldPosition = worldPosition;
        }
    }

    [Serializable]
    public class ParticlePickUnityEvent : UnityEvent<ParticlePickEventArgs> { }

    public PBDSolver solver;
    public float radiusScale = 1;

    public ParticlePickUnityEvent OnParticlePicked;
    public ParticlePickUnityEvent OnParticleHeld;
    public ParticlePickUnityEvent OnParticleDragged;
    public ParticlePickUnityEvent OnParticleReleased;

    private Vector3 lastMousePos = Vector3.zero;
    private int pickedParticleIndex = -1;
    private int pickedActorId = -1;
    private float pickedParticleDepth = 0;

    void Awake()
    {
        lastMousePos = Input.mousePosition;
    }

    void LateUpdate()
    {

        if (solver != null)
        {

            // Click:
            if (Input.GetMouseButtonDown(0))
            {

                pickedParticleIndex = -1;
                pickedActorId = -1;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float closestMu = float.MaxValue;
                float closestDistance = float.MaxValue;

                Matrix4x4 solver2World = solver.transform.localToWorldMatrix;
                foreach(var actorId in solver.GetActorIds())
                {
                    for(int i = 0; i < solver.GetActorParticleCount(actorId); i++)
                    {
                        var pos = solver.GetParticlePosition(actorId, i);
                        Vector3 worldPos = solver2World.MultiplyPoint3x4(pos);

                        float mu;
                        Vector3 projected = GeometryUtil.ProjectPointLine(worldPos, ray.origin, ray.origin + ray.direction, out mu, false);
                        float distanceToRay = Vector3.SqrMagnitude(worldPos - projected);

                        // Disregard particles behind the camera:
                        mu = Mathf.Max(0, mu);

                        float radius = 0.2f * radiusScale;

                        if (distanceToRay <= radius * radius && distanceToRay < closestDistance && mu < closestMu)
                        {
                            closestMu = mu;
                            closestDistance = distanceToRay;
                            pickedParticleIndex = i;
                            pickedActorId = actorId;
                        }
                    }
                }


                if (pickedParticleIndex >= 0)
                {

                    pickedParticleDepth = Camera.main.transform.InverseTransformVector(solver2World.MultiplyPoint3x4(solver.GetParticlePosition(pickedActorId, pickedParticleIndex)) - Camera.main.transform.position).z;

                    if (OnParticlePicked != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticlePicked.Invoke(new ParticlePickEventArgs(pickedActorId, pickedParticleIndex, worldPosition));
                    }
                }

            }
            else if (pickedParticleIndex >= 0)
            {

                // Drag:
                Vector3 mouseDelta = Input.mousePosition - lastMousePos;
                if (mouseDelta.magnitude > 0.01f && OnParticleDragged != null)
                {

                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                    OnParticleDragged.Invoke(new ParticlePickEventArgs(pickedActorId, pickedParticleIndex, worldPosition));

                }
                else if (OnParticleHeld != null)
                {

                    Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                    OnParticleHeld.Invoke(new ParticlePickEventArgs(pickedActorId, pickedParticleIndex, worldPosition));

                }

                // Release:				
                if (Input.GetMouseButtonUp(0))
                {

                    if (OnParticleReleased != null)
                    {
                        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, pickedParticleDepth));
                        OnParticleReleased.Invoke(new ParticlePickEventArgs(pickedActorId, pickedParticleIndex, worldPosition));
                    }

                    pickedParticleIndex = -1;

                }
            }
        }

        lastMousePos = Input.mousePosition;
    }
}

