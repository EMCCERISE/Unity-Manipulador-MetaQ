using System;
using UnityEngine;
using sensor_msgs.msg;

namespace ROS2
{

    public class ROS2JointStateListener : MonoBehaviour
    {

        private ROS2UnityComponent ros2Unity;
        private ROS2Node ros2Node;
        private ISubscription<JointState> jointStateSub;

        [SerializeField] private UnityEngine.UI.Slider _sliderA;
        [SerializeField] private UnityEngine.UI.Slider _sliderB;
        [SerializeField] private UnityEngine.UI.Slider _sliderC;
        [SerializeField] private UnityEngine.UI.Slider _sliderD;
        [SerializeField] private UnityEngine.UI.Slider _sliderE;
        [SerializeField] private UnityEngine.UI.Slider _sliderF;
        [SerializeField] private UnityEngine.GameObject buttonSliders;
        [SerializeField] private UnityEngine.GameObject buttonROS2;
        [SerializeField] private UnityEngine.GameObject ROS2Hook;
        private double sliderATemp = 0.0d;
        private double sliderBTemp = 0.0d;
        private double sliderCTemp = 0.0d;
        private double sliderDTemp = 0.0d;
        private double sliderETemp = 0.0d;
        private double sliderFTemp = 0.0d;


        void Start()
        {
            // Find the ROS2UnityComponent in the scene
            ros2Unity = GetComponent<ROS2UnityComponent>();
        }

        void Update()
        {
            if (ros2Node == null && ros2Unity.Ok())
            {
                // Create a node
                ros2Node = ros2Unity.CreateNode("joint_state_listener_node");
                Debug.Log($"Teste");
                // Subscribe to the /joint_states topic
                jointStateSub = ros2Node.CreateSubscription<JointState>(
                    "joint_states",
                    msg =>
                    {
                        for (int i = 0; i < msg.Position.Length; i++)
                        {
                            //Debug.Log($"Joint {i}: Position {(Mathf.Rad2Deg * msg.Position[i]).ToString("#0")}");
                            switch (i) { 
                                case 2: 
                                    sliderATemp = msg.Position[i]; 
                                    break;
                                case 0:
                                    sliderBTemp = msg.Position[i];
                                    break;
                                case 5:
                                    sliderCTemp = msg.Position[i];
                                    break;
                                case 1:
                                    sliderDTemp = msg.Position[i];
                                    break;
                                case 3:
                                    sliderETemp = msg.Position[i];
                                    break;
                                case 4:
                                    sliderFTemp = msg.Position[i];
                                    break;
                                case 6:
                                    //sliderGTemp = msg.Position[i];
                                    break;
                                default:
                                    break;
                            }
                        }
                    });
            }
            _sliderA.value = -1f * Mathf.RoundToInt(Mathf.Rad2Deg * ((float)sliderATemp));
            _sliderB.value = -1f * Mathf.RoundToInt(Mathf.Rad2Deg * ((float)sliderBTemp));
            _sliderC.value = -1f * Mathf.RoundToInt(Mathf.Rad2Deg * ((float)sliderCTemp));
            _sliderD.value = -1f * Mathf.RoundToInt(Mathf.Rad2Deg * ((float)sliderDTemp));
            _sliderE.value = -1f * Mathf.RoundToInt(Mathf.Rad2Deg * ((float)sliderETemp));
            _sliderF.value = -1f * Mathf.RoundToInt(Mathf.Rad2Deg * ((float)sliderFTemp));

        }

        public void TurnOnSliders()
        {
            buttonROS2.SetActive(false);
            buttonSliders.SetActive(true);
            ROS2Hook.SetActive(false);
        }

        public void TurnOnROS2()
        {
            buttonROS2.SetActive(true);
            buttonSliders.SetActive(false);
            ROS2Hook.SetActive(true);
        }
    }
}