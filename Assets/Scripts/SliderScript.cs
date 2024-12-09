using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Robotics.UrdfImporter;
using UnityEngine.UIElements;

public class SliderScript : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Slider _slider;
    [SerializeField] private TextMeshProUGUI _sliderText;
    [SerializeField] private ArticulationBody _body;
    [SerializeField] private float maxChangeOnUpdate;

    // Start is called before the first frame update
    void Start()
    {
        //_slider.onValueChanged.AddListener((v) =>
        //{
        //  _sliderText.text = _body.xDrive.target.ToString() + "°";
        //});
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ArticulationBody joint = _body;
        //Debug.Log("Slider value: " + (_slider.value.ToString("#.#0")) + "/" + Mathf.DeltaAngle(0, (-_slider.value)).ToString("#.#0"));


        if (joint.jointType != ArticulationJointType.FixedJoint)
        {
            //if (controltype == Unity.Robotics.UrdfImporter.Control.ControlType.PositionControl)
            if (true)
                {
                ArticulationDrive currentDrive = joint.xDrive;
                float newTargetDelta = -1f * _slider.value;

                if (joint.jointType == ArticulationJointType.RevoluteJoint)
                {
                    if (joint.twistLock == ArticulationDofLock.LimitedMotion)
                    {
                        if (newTargetDelta > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (newTargetDelta < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            // Verifica direção
                            if (currentDrive.target < newTargetDelta)
                            {
                                // Aplica limite de update por quadro
                                currentDrive.target = Mathf.Min(newTargetDelta, currentDrive.target + maxChangeOnUpdate);
                            }
                            else
                            {
                                // Aplica limite de update por quadro
                                currentDrive.target = Mathf.Max(newTargetDelta, currentDrive.target - maxChangeOnUpdate);
                            }
                        }
                    }
                    else
                    {
                        // Verifica direção
                        if (currentDrive.target < newTargetDelta)
                        {
                            // Aplica limite de update por quadro
                            currentDrive.target = Mathf.Min(newTargetDelta, currentDrive.target + maxChangeOnUpdate);
                        }
                        else
                        {
                            // Aplica limite de update por quadro
                            currentDrive.target = Mathf.Max(newTargetDelta, currentDrive.target - maxChangeOnUpdate);
                        }
                    }
                }

                else if (joint.jointType == ArticulationJointType.PrismaticJoint)
                {
                    if (joint.linearLockX == ArticulationDofLock.LimitedMotion)
                    {
                        if (newTargetDelta > currentDrive.upperLimit)
                        {
                            currentDrive.target = currentDrive.upperLimit;
                        }
                        else if (newTargetDelta < currentDrive.lowerLimit)
                        {
                            currentDrive.target = currentDrive.lowerLimit;
                        }
                        else
                        {
                            currentDrive.target = newTargetDelta;
                        }
                    }
                    else
                    {
                        currentDrive.target = newTargetDelta;

                    }
                }
                joint.xDrive = currentDrive;

                _sliderText.text = (Mathf.RoundToInt(_slider.value)) == (Mathf.RoundToInt(-1f * currentDrive.target)) ? (Mathf.RoundToInt(-1f * currentDrive.target)).ToString() + "°" : (Mathf.RoundToInt(-1f * currentDrive.target)).ToString() + "° ► " + (Mathf.RoundToInt(_slider.value)).ToString();
                //_slider.SetValueWithoutNotify(Mathf.RoundToInt(-1f * currentDrive.target));
            }
        }

    }
}
