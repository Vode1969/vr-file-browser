using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothBeam : MonoBehaviour
{
    public Transform parent;  // The controller transform
    public Transform goal;    // The target transform for the beam
    public int smoothingFrames = 10; // Number of frames for smoothing
    public float positionSmoothTime = 0.1f; // Smoothing time for position
    public float rotationSmoothTime = 0.1f; // Smoothing time for rotation

    private Queue<Vector3> positionQueue;
    private Vector3 positionVelocity; // Velocity parameter for position smoothing
    private Quaternion rotationVelocity; // Velocity parameter for rotation smoothing

    void Start()
    {
        positionQueue = new Queue<Vector3>(new Vector3[smoothingFrames]);
    }

    void LateUpdate()
    {
        // Update position queue with the current parent position
        if (positionQueue.Count >= smoothingFrames)
        {
            positionQueue.Dequeue();
        }
        positionQueue.Enqueue(parent.position);

        // Calculate smoothed position
        Vector3 averagePosition = Vector3.zero;
        foreach (Vector3 pos in positionQueue)
        {
            averagePosition += pos;
        }
        averagePosition /= positionQueue.Count;

        // Smooth the position and rotation
        goal.position = Vector3.SmoothDamp(goal.position, averagePosition, ref positionVelocity, positionSmoothTime);
        goal.rotation = SmoothDampQuaternion(goal.rotation, parent.rotation, ref rotationVelocity, rotationSmoothTime);
    }

    private Quaternion SmoothDampQuaternion(Quaternion current, Quaternion target, ref Quaternion velocity, float smoothTime)
    {
        // Smooth rotation using Quaternion.Lerp
        float t = 1 - Mathf.Exp(-smoothTime * Time.deltaTime);
        return Quaternion.Lerp(current, target, t);
    }
}
