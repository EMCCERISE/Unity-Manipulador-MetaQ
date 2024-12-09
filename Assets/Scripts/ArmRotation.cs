using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Unity.Robotics;
using TMPro;
using UnityEngine.Rendering;

public class ArmRotation : MonoBehaviour
{
    public Transform armBase;   // Reference to the base of the robot arm
    [SerializeField] private Slider _sliderA;
    [SerializeField] private Slider _sliderB;
    [SerializeField] private Slider _sliderC;
    [SerializeField] private Slider _sliderD;
    [SerializeField] public ArticulationBody _jointA;
    [SerializeField] public ArticulationBody _jointB;
    [SerializeField] public ArticulationBody _jointC;
    [SerializeField] public ArticulationBody _jointD;

    public Transform[] joints;  // Array of joints (base to end effector)
    public Transform endEffector;  // The end effector of the robot arm
    public Transform target;  // The target position (the cube)

    // Joint angle restrictions
    private float[] minAngles = { -125f, -110f, -60f };
    private float[] maxAngles = { 125f, 90f, 60f };
    private float threshold = 0.1f;  // Distance threshold to consider the target reached
    private int maxIterations = 10;  // Max number of iterations to avoid infinite loops
    public float previousApply = 0f;
    public float updateInterval = 10f;
    public float angle = 0f;

    void Update()
    {
        if ((Time.time - previousApply) < updateInterval)
        {
            return;
        }
        previousApply = Time.time;

        // Define the forward direction of the arm's base
        Vector3 armForward = armBase.forward;

        // Calculate the direction vector from the arm base to the cube
        Vector3 directionToCube = (new Vector3(target.position.x, armBase.position.y, target.position.z) - armBase.position).normalized;

        // Calculate the angle between the forward direction of the arm and the direction to the cube
        angle = Vector3.SignedAngle(armForward, directionToCube, Vector3.up);

        // Debug.Log("EE/Target at " + endEffector.position.x.ToString("#.##") + "/" + target.position.x.ToString("#.##") + ", " + endEffector.position.y.ToString("#.##") + "/" + target.position.y.ToString("#.##") + ", " + endEffector.position.z.ToString("#.##") + "/" + target.position.z.ToString("#.##"));

        // Print the angle
        //Debug.Log("Angle to turn the base joint: " + (-1f * _jointA.xDrive.target));

        _sliderA.value = angle;
        //Debug.Log("Distance :" + (target.position - endEffector.position).magnitude.ToString("#.####"));

        if (Mathf.Abs((-1f * _jointA.xDrive.target) - _sliderA.value) < 1f && Mathf.Abs((-1f * _jointB.xDrive.target) - _sliderB.value) < 1f && Mathf.Abs((-1f * _jointC.xDrive.target) - _sliderC.value) < 1f && Mathf.Abs((-1f * _jointD.xDrive.target) - _sliderD.value) < 1f)
        {
            SimulateCCD();
        }
    }

    void SimulateCCD()
    {
        // Create a copy of joint rotations to simulate without moving the actual joints
        Vector3[] simulatedJointAngles = new Vector3[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            simulatedJointAngles[i] = joints[i].localEulerAngles;
        }

        // Create a copy of the end effector position
        Vector3 simulatedEndEffectorPosition = endEffector.position;

        for (int iteration = 0; iteration < maxIterations; iteration++)
        {
            // Start from the last joint and move towards the base
            for (int i = joints.Length - 1; i >= 0; i--)
            {
                // Direction vector from the current joint to the end effector
                Vector3 toEndEffector = simulatedEndEffectorPosition - joints[i].position;

                // Direction vector from the current joint to the target
                Vector3 toTarget = target.position - joints[i].position;

                // Calculate the rotation needed to align the end effector with the target
                Quaternion rotationToTarget = Quaternion.FromToRotation(toEndEffector, toTarget);

                // Apply the rotation to the simulated joint angles (only Y axis)
                if (i.Equals(0))
                {
                    simulatedJointAngles[i].x = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.x;
                    //simulatedJointAngles[i].y = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.y;
                    //simulatedJointAngles[i].z = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.z;

                    // Clamp the angle within the joint's restrictions
                    simulatedJointAngles[i].x = Mathf.Clamp(simulatedJointAngles[i].x, minAngles[i], maxAngles[i]);
                    //simulatedJointAngles[i].y = Mathf.Clamp(simulatedJointAngles[i].y, 0, 0);
                    //simulatedJointAngles[i].z = Mathf.Clamp(simulatedJointAngles[i].z, 0, 0);
                }
                else if (i.Equals(1))
                {
                    simulatedJointAngles[i].x = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.x;
                    //simulatedJointAngles[i].y = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.y;
                    //simulatedJointAngles[i].z = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.z;

                    // Clamp the angle within the joint's restrictions
                    simulatedJointAngles[i].x = Mathf.Clamp(simulatedJointAngles[i].x, minAngles[i], maxAngles[i]);
                    //simulatedJointAngles[i].y = Mathf.Clamp(simulatedJointAngles[i].y, 0, 0);
                    //simulatedJointAngles[i].z = Mathf.Clamp(simulatedJointAngles[i].z, 0, 0);
                }
                else if (i.Equals(2))
                {
                    simulatedJointAngles[i].x = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.x;
                    //simulatedJointAngles[i].y = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.y;
                    //simulatedJointAngles[i].z = (rotationToTarget * Quaternion.Euler(simulatedJointAngles[i])).eulerAngles.z;

                    // Clamp the angle within the joint's restrictions
                    simulatedJointAngles[i].x = Mathf.Clamp(simulatedJointAngles[i].x, minAngles[i], maxAngles[i]);
                    //simulatedJointAngles[i].y = Mathf.Clamp(simulatedJointAngles[i].y, 0, 0);
                    //simulatedJointAngles[i].z = Mathf.Clamp(simulatedJointAngles[i].z, 0, 0);
                }

                // Recalculate the simulated end effector position
                simulatedEndEffectorPosition = CalculateSimulatedEndEffectorPosition(simulatedJointAngles);


                // Break if the end effector is close enough to the target
                if (Vector3.Distance(simulatedEndEffectorPosition, target.position) < threshold)
                {
                    // Draw arrows to visualize the target positions
                    Debug.Log("Ax: " + 0.ToString("#.##") + ", Bx: " + simulatedJointAngles[0].x.ToString("#.##") + ", Cx: " + simulatedJointAngles[1].x.ToString("#.##") + ", Dx: " + simulatedJointAngles[2].x.ToString("#.##") );
                    Debug.Log("Ay: " + angle.ToString("#.##") + ", By: " + simulatedJointAngles[0].y.ToString("#.##") + ", Cy: " + simulatedJointAngles[1].y.ToString("#.##") + ", Dy: " + simulatedJointAngles[2].y.ToString("#.##"));
                    Debug.Log("Az: " + 0.ToString("#.##") + ", Bz: " + simulatedJointAngles[0].z.ToString("#.##") + ", Cz: " + simulatedJointAngles[1].z.ToString("#.##") + ", Dz: " + simulatedJointAngles[2].z.ToString("#.##"));
                    Debug.Log("As: " + _sliderA.value.ToString("#.##") + ", Bs: " + _sliderB.value.ToString("#.##") + ", Cs: " + _sliderC.value.ToString("#.##") + ", Ds: " + _sliderD.value.ToString("#.##"));
                    DrawArrows(simulatedJointAngles);
                    // Apply the simulated rotations to the actual joints
                    ApplySimulatedRotations(simulatedJointAngles);
                    return;
                }
            }
        }

        // Draw arrows to visualize the target positions
        Debug.Log("Ax: " + 0.ToString("#.##") + ", Bx: " + simulatedJointAngles[0].x.ToString("#.##") + ", Cx: " + simulatedJointAngles[1].x.ToString("#.##") + ", Dx: " + simulatedJointAngles[2].x.ToString("#.##"));
        Debug.Log("Ay: " + angle.ToString("#.##") + ", By: " + simulatedJointAngles[0].y.ToString("#.##") + ", Cy: " + simulatedJointAngles[1].y.ToString("#.##") + ", Dy: " + simulatedJointAngles[2].y.ToString("#.##"));
        Debug.Log("Az: " + 0.ToString("#.##") + ", Bz: " + simulatedJointAngles[0].z.ToString("#.##") + ", Cz: " + simulatedJointAngles[1].z.ToString("#.##") + ", Dz: " + simulatedJointAngles[2].z.ToString("#.##"));
        Debug.Log("As: " + _sliderA.value.ToString("#.##") + ", Bs: " + _sliderB.value.ToString("#.##") + ", Cs: " + _sliderC.value.ToString("#.##") + ", Ds: " + _sliderD.value.ToString("#.##"));
        DrawArrows(simulatedJointAngles);
        // Apply the simulated rotations to the actual joints if max iterations reached
        Debug.Log("Not close enough: " + Vector3.Distance(simulatedEndEffectorPosition, target.position));
        //ApplySimulatedRotations(simulatedJointAngles);
    }

    Vector3 CalculateSimulatedEndEffectorPosition(Vector3[] simulatedJointAngles)
    {
        // Start from the base position
        Vector3 position = joints[0].position;
        Quaternion rotation = Quaternion.identity;

        // Iterate through each joint and calculate the position of the end effector
        for (int i = 0; i < joints.Length; i++)
        {
            if (i.Equals(0))
            {
                //rotation *= Quaternion.Euler(simulatedJointAngles[i].x, simulatedJointAngles[i].y, simulatedJointAngles[i].z);  // Only consider the X rotation for simulation
                rotation *= Quaternion.Euler(simulatedJointAngles[i].x, 0, 0);  // Only consider the X rotation for simulation
                //rotation *= Quaternion.Euler(0, simulatedJointAngles[i].y, 0);  // Only consider the Y rotation for simulation
                //rotation *= Quaternion.Euler(0, 0, simulatedJointAngles[i].z);  // Only consider the Z rotation for simulation
            }
            else if (i.Equals(1))
            {
                //rotation *= Quaternion.Euler(simulatedJointAngles[i].x, simulatedJointAngles[i].y, simulatedJointAngles[i].z);  // Only consider the X rotation for simulation
                rotation *= Quaternion.Euler(simulatedJointAngles[i].x, 0, 0);  // Only consider the X rotation for simulation
                //rotation *= Quaternion.Euler(0, simulatedJointAngles[i].y, 0);  // Only consider the Y rotation for simulation
                //rotation *= Quaternion.Euler(0, 0, simulatedJointAngles[i].z);  // Only consider the Z rotation for simulation
            }
            else if (i.Equals(2))
            {
                //rotation *= Quaternion.Euler(simulatedJointAngles[i].x, simulatedJointAngles[i].y, simulatedJointAngles[i].z);  // Only consider the X rotation for simulation
                rotation *= Quaternion.Euler(simulatedJointAngles[i].x, 0, 0);  // Only consider the X rotation for simulation
                //rotation *= Quaternion.Euler(0, simulatedJointAngles[i].y, 0);  // Only consider the Y rotation for simulation
                //rotation *= Quaternion.Euler(0, 0, simulatedJointAngles[i].z);  // Only consider the Z rotation for simulation
            }
            if (i < joints.Length - 1)
            {
                position += rotation * (joints[i + 1].position - joints[i].position);
            }
            else
            {
                position += rotation * (endEffector.position - joints[i].position);
            }
        }

        return position;
    }

    void ApplySimulatedRotations(Vector3[] simulatedJointAngles)
    {
/*        Debug.Log("A :" + Mathf.Abs((-1f * _jointA.xDrive.target) - _sliderA.value));
        Debug.Log("B :" + Mathf.Abs((-1f * _jointB.xDrive.target) - _sliderB.value));
        Debug.Log("C :" + Mathf.Abs((-1f * _jointC.xDrive.target) - _sliderC.value));
        Debug.Log("D :" + Mathf.Abs((-1f * _jointD.xDrive.target) - _sliderD.value));
*/        for (int i = 0; i < joints.Length; i++)
        {
            if (i.Equals(0))
            {
                if (Mathf.Abs((-1f * _jointA.xDrive.target) - _sliderA.value) < 1f)
                { 
                    _sliderB.value = simulatedJointAngles[i].x + 10;
                }
            }
            else if (i.Equals(1))
            {
                if (Mathf.Abs((-1f * _jointA.xDrive.target) - _sliderA.value) < 1f && Mathf.Abs((_jointB.xDrive.target) - _sliderB.value) < 1f)
                {
                    _sliderC.value = -simulatedJointAngles[i].x;
                }
            }
            else if (i.Equals (2))
            {
                if (Mathf.Abs((-1f * _jointA.xDrive.target) - _sliderA.value) < 1f && Mathf.Abs((_jointB.xDrive.target) - _sliderB.value) < 1f && Mathf.Abs((-1f * _jointC.xDrive.target) - _sliderC.value) < 1f)
                {
                    _sliderD.value = simulatedJointAngles[i].x;
                }
            }
            // joints[i].localEulerAngles = simulatedJointAngles[i];
        }
    }

    void DrawArrows(Vector3[] simulatedJointAngles, float duration = 10f)
    {
        Vector3 position = joints[0].position;
        Quaternion rotation = Quaternion.identity;

        for (int i = 0; i < joints.Length; i++)
        {
            if (i.Equals(0))
            {
                //rotation *= Quaternion.Euler(simulatedJointAngles[i].x, simulatedJointAngles[i].y, simulatedJointAngles[i].z);  // Only consider the X rotation for simulation
                rotation *= Quaternion.Euler(simulatedJointAngles[i].x, 0, 0);  // Only consider the X rotation for simulation
                //rotation *= Quaternion.Euler(0, simulatedJointAngles[i].y, 0);  // Only consider the Y rotation for simulation
                //rotation *= Quaternion.Euler(0, 0, simulatedJointAngles[i].z);  // Only consider the Z rotation for simulation
            }
            else if (i.Equals(1))
            {
                //rotation *= Quaternion.Euler(simulatedJointAngles[i].x, simulatedJointAngles[i].y, simulatedJointAngles[i].z);  // Only consider the X rotation for simulation
                rotation *= Quaternion.Euler(simulatedJointAngles[i].x, 0, 0);  // Only consider the X rotation for simulation
                //rotation *= Quaternion.Euler(0, simulatedJointAngles[i].y, 0);  // Only consider the Y rotation for simulation
                //rotation *= Quaternion.Euler(0, 0, simulatedJointAngles[i].z);  // Only consider the Z rotation for simulation
            }
            else if (i.Equals(2))
            {
                //rotation *= Quaternion.Euler(simulatedJointAngles[i].x, simulatedJointAngles[i].y, simulatedJointAngles[i].z);  // Only consider the X rotation for simulation
                rotation *= Quaternion.Euler(simulatedJointAngles[i].x, 0, 0);  // Only consider the X rotation for simulation
                //rotation *= Quaternion.Euler(0, simulatedJointAngles[i].y, 0);  // Only consider the Y rotation for simulation
                //rotation *= Quaternion.Euler(0, 0, simulatedJointAngles[i].z);  // Only consider the Z rotation for simulation
            }
            if (i < joints.Length - 1)
            {
                Vector3 nextPosition = position + rotation * (joints[i + 1].position - joints[i].position);
                Debug.DrawRay(position, nextPosition - position, Color.red, duration);
                position = nextPosition;
            }
            else
            {
                Vector3 nextPosition = position + rotation * (endEffector.position - joints[i].position);
                Debug.DrawRay(position, nextPosition - position, Color.red, duration);
                position = nextPosition;
            }
        }
    }

}