using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Robotics.UrdfImporter;
using UnityEngine.UIElements;

public class SliderGripperScript : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    [SerializeField] private ArticulationBody[] _grippers; // Array de grippers
    [SerializeField] private float[] _gripperMultipliers; // Array de multiplicadores para cada gripper
    [SerializeField] private float maxChangeOnUpdate;

    // Start is called before the first frame update
    void Start()
    {
        if (_grippers.Length != _gripperMultipliers.Length)
        {
            Debug.LogError("Número de grippers e multiplicadores não corresponde!");
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < _grippers.Length; i++)
        {
            ArticulationBody joint = _grippers[i];
            float multiplier = _gripperMultipliers[i];
            float newTargetDelta = -1f * _slider.value * multiplier;

            if (joint.jointType != ArticulationJointType.FixedJoint)
            {
                ArticulationDrive currentDrive = joint.xDrive;

                if (joint.jointType == ArticulationJointType.RevoluteJoint)
                {
                    if (joint.twistLock == ArticulationDofLock.LimitedMotion)
                    {
                        currentDrive.target = Mathf.Clamp(newTargetDelta, currentDrive.lowerLimit, currentDrive.upperLimit);
                    }
                    else
                    {
                        currentDrive.target = UpdateTargetWithLimit(currentDrive.target, newTargetDelta);
                    }
                }
                else if (joint.jointType == ArticulationJointType.PrismaticJoint)
                {
                    if (joint.linearLockX == ArticulationDofLock.LimitedMotion)
                    {
                        currentDrive.target = Mathf.Clamp(newTargetDelta, currentDrive.lowerLimit, currentDrive.upperLimit);
                    }
                    else
                    {
                        currentDrive.target = newTargetDelta;
                    }
                }
                joint.xDrive = currentDrive;
            }
        }

        UpdateSliderText();
    }

    private float UpdateTargetWithLimit(float currentTarget, float newTargetDelta)
    {
        if (currentTarget < newTargetDelta)
        {
            return Mathf.Min(newTargetDelta, currentTarget + maxChangeOnUpdate);
        }
        else
        {
            return Mathf.Max(newTargetDelta, currentTarget - maxChangeOnUpdate);
        }
    }

    private void UpdateSliderText()
    {
        _sliderText.text = Mathf.RoundToInt(_slider.value).ToString() + "°";
    }
}
