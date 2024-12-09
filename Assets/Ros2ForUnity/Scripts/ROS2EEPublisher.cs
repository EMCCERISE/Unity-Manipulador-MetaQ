// Copyright 2019-2021 Robotec.ai.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UnityEngine;
using UnityEngine.UIElements;
using System;

namespace ROS2
{

/// <summary>
/// An example class provided for testing of basic ROS2 communication
/// </summary>
public class ROS2EEPublisher : MonoBehaviour
{
    // Start is called before the first frame update
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<geometry_msgs.msg.Pose> ee_pub;
    private int i;
        [SerializeField] private bool targetSent = false;

        [SerializeField] private GameObject _target;

        [Header("Button Control")]
        [SerializeField] private GameObject _buttonActive;
        [SerializeField] private Button _buttonSelected;
        [SerializeField] private GameObject _buttonInactive;

        private void Awake()
        {
            Environment.SetEnvironmentVariable("ROS_DOMAIN_ID", "42");
        }

        void Start()
        {
            ros2Unity = GetComponent<ROS2UnityComponent>();
            InvokeRepeating("PublishEE", 1, 1);
        }

        void PublishEE()
        {
            if (_target.activeInHierarchy)
            {
                targetSent = false;
            }

            if (ros2Unity.Ok() && !_target.activeInHierarchy && !targetSent)
            {
                if (ros2Node == null)
                {
                    ros2Node = ros2Unity.CreateNode("ROS2UnityTalkerNode");
                    ee_pub = ros2Node.CreatePublisher<geometry_msgs.msg.Pose>("unity_end_effector_pose");
                }

                i++;
                    geometry_msgs.msg.Pose msg = new geometry_msgs.msg.Pose();
                    msg.Position.X = _target.transform.position.z;
                    msg.Position.Y = -1f * _target.transform.position.x; 
                    msg.Position.Z = _target.transform.position.y - 0.63f;
                    msg.Orientation.X = _target.transform.rotation.x;
                    msg.Orientation.Y = _target.transform.rotation.y;
                    msg.Orientation.Z = _target.transform.rotation.z;
                    msg.Orientation.W = _target.transform.rotation.w;
                    msg.Orientation.X = Quaternion.identity.x;
                    msg.Orientation.Y = Quaternion.identity.y;
                    msg.Orientation.Z = Quaternion.identity.z;
                    msg.Orientation.W = Quaternion.identity.w;
                    ee_pub.Publish(msg);
                    Debug.Log("ROS2 diz: Enviada posição do cubo: " + msg.Position.X.ToString("#0.##") + "; " + msg.Position.Y.ToString("#0.##") + "; " + msg.Position.Z.ToString("#0.##") + " .");
                    targetSent = true;
            }
          
        }
     }


}  // namespace ROS2
