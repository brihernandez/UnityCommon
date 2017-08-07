// FloatingOrigin.cs
// Written by Peter Stirling
// 11 November 2010
// Uploaded to Unify Community Wiki on 11 November 2010
// Updated to Unity 5.x particle system by Tony Lovell 14 January, 2016
// fix to ensure ALL particles get moved by Tony Lovell 8 September, 2016
// URL: http://wiki.unity3d.com/index.php/Floating_Origin
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class FloatingOrigin : MonoBehaviour
{
    [Tooltip("Max distance from origin allowed.")]
    public float threshold = 1000.0f;
    [Tooltip("Distance at which physics stop being simulated.")]
    public float physicsThreshold = 10000.0f; // Set to zero to disable

    public float defaultSleepThreshold = 0.14f;

    ParticleSystem.Particle[] parts = null;

    void LateUpdate()
    {
        Vector3 cameraPosition = gameObject.transform.position;
        if (cameraPosition.magnitude > threshold)
        {
            Object[] objects = FindObjectsOfType<Transform>();
            foreach (Object o in objects)
            {
                Transform t = (Transform)o;
                if (t.parent == null)
                {
                    t.position -= cameraPosition;
                }
            }

            // new particles... very similar to old version above
            objects = FindObjectsOfType<ParticleSystem>();
            foreach (Object o in objects)
            {
                ParticleSystem sys = (ParticleSystem)o;

                if (sys.main.simulationSpace != ParticleSystemSimulationSpace.World)
                    continue;

                int particlesNeeded = sys.main.maxParticles;

                if (particlesNeeded <= 0)
                    continue;

                bool wasPaused = sys.isPaused;
                bool wasPlaying = sys.isPlaying;

                if (!wasPaused)
                    sys.Pause();

                // ensure a sufficiently large array in which to store the particles
                if (parts == null || parts.Length < particlesNeeded)
                {
                    parts = new ParticleSystem.Particle[particlesNeeded];
                }

                // now get the particles
                int num = sys.GetParticles(parts);

                for (int i = 0; i < num; i++)
                {
                    parts[i].position -= cameraPosition;
                }

                sys.SetParticles(parts, num);

                if (wasPlaying)
                    sys.Play();
            }

            // Clear trail renderers. Very inelegant, but better than the alternative.
            objects = FindObjectsOfType<TrailRenderer>();
            foreach (Object o in objects)
            {
                TrailRenderer trail = (TrailRenderer)o;
                trail.Clear();
            }

            // Move the segments of a line renderer
            objects = FindObjectsOfType(typeof(LineRenderer));
            foreach (Object o in objects)
            {
                LineRenderer line = (LineRenderer)o;

                Vector3[] positions = new Vector3[line.positionCount];
                line.GetPositions(positions);

                for (int i = 0; i < positions.Length; ++i)
                    positions[i] -= cameraPosition;

                line.SetPositions(positions);
            }

            if (physicsThreshold > 0f)
            {
                float physicsThreshold2 = physicsThreshold * physicsThreshold; // simplify check on threshold
                objects = FindObjectsOfType(typeof(Rigidbody));
                foreach (Object o in objects)
                {
                    Rigidbody r = (Rigidbody)o;
                    if (r.gameObject.transform.position.sqrMagnitude > physicsThreshold2)
                    {
                        r.sleepThreshold = float.MaxValue;
                    }
                    else
                    {
                        r.sleepThreshold = defaultSleepThreshold;
                    }
                }
            }

            print("Moved origin");
        }
    }
}