using System.Collections.Generic;
using UnityEngine;

public class NeoIK : MonoBehaviour
{
    public Transform target; // The target point the end effector should reach
    public List<Transform> joints; // List of joint transforms
    public List<float> lowerLimits; // Lower limits for each joint
    public List<float> upperLimits; // Upper limits for each joint
    public float learningRate = 0.1f; // Learning rate for the gradient descent
    public float threshold = 0.01f; // Threshold for stopping the iteration

    void Start()
    {
        // Ensure the lists are populated correctly
        if (joints.Count != lowerLimits.Count || joints.Count != upperLimits.Count)
        {
            Debug.LogError("Joints, lowerLimits, and upperLimits lists must have the same length.");
            return;
        }
    }

    void Update()
    {
        InverseKinematicsSolver();
    }

    void InverseKinematicsSolver()
    {
        for (int i = 0; i < 1000; i++) // Limit iterations to prevent infinite loops
        {
            // Calculate the current end effector position
            Vector3 currentEndEffectorPos = joints[joints.Count - 1].position;

            // Calculate the distance to the target
            float distance = Vector3.Distance(currentEndEffectorPos, target.position);

            // Stop if the distance is less than the threshold
            if (distance < threshold)
            {
                break;
            }

            // Perform gradient descent
            for (int j = joints.Count - 2; j >= 0; j--) // Iterate from the second last joint to the first
            {
                // Save the current joint rotation
                Quaternion originalRotation = joints[j].localRotation;

                // Rotate the joint a small amount around its local axes and calculate the new end effector position
                joints[j].localRotation = originalRotation * Quaternion.Euler(learningRate, 0, 0);
                float dx = Vector3.Distance(joints[joints.Count - 1].position, target.position);
                joints[j].localRotation = originalRotation * Quaternion.Euler(0, learningRate, 0);
                float dy = Vector3.Distance(joints[joints.Count - 1].position, target.position);
                joints[j].localRotation = originalRotation * Quaternion.Euler(0, 0, learningRate);
                float dz = Vector3.Distance(joints[joints.Count - 1].position, target.position);

                // Compute the gradient
                Vector3 gradient = new Vector3(dx, dy, dz);

                // Update the joint rotation in the direction of the negative gradient
                joints[j].localRotation = originalRotation * Quaternion.Euler(-learningRate * gradient.x, -learningRate * gradient.y, -learningRate * gradient.z);

                // Clamp the joint rotation to its limits
                Vector3 currentAngles = joints[j].localEulerAngles;
                currentAngles.x = Mathf.Clamp(currentAngles.x, lowerLimits[j], upperLimits[j]);
                currentAngles.y = Mathf.Clamp(currentAngles.y, lowerLimits[j], upperLimits[j]);
                currentAngles.z = Mathf.Clamp(currentAngles.z, lowerLimits[j], upperLimits[j]);
                joints[j].localEulerAngles = currentAngles;
            }
        }
    }
}
