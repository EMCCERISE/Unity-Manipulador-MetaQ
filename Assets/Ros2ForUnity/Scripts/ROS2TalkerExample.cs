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
using System;

namespace ROS2
{

/// <summary>
/// An example class provided for testing of basic ROS2 communication
/// </summary>
public class ROS2TalkerExample : MonoBehaviour
{
    // Start is called before the first frame update
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<std_msgs.msg.String> chatter_pub;
    private int i;

    [SerializeField] private UnityEngine.UI.Slider _sliderA;
    [SerializeField] private UnityEngine.UI.Slider _sliderB;
    [SerializeField] private UnityEngine.UI.Slider _sliderC;
    [SerializeField] private UnityEngine.UI.Slider _sliderD;
    [SerializeField] private UnityEngine.UI.Slider _sliderE;
    [SerializeField] private UnityEngine.UI.Slider _sliderF;

        private void Awake()
        {
            Environment.SetEnvironmentVariable("ROS_DOMAIN_ID", "42");
        }

        void Start()
    {
        ros2Unity = GetComponent<ROS2UnityComponent>();
        InvokeRepeating("PublishChatter", 1, 1);
    }

    void PublishChatter()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("ROS2UnityTalkerNode");
                chatter_pub = ros2Node.CreatePublisher<std_msgs.msg.String>("chatter");
            }

            i++;
            std_msgs.msg.String msg = new std_msgs.msg.String();
            msg.Data = "ROS2 diz: Leitura " + i + ": A" + (_sliderA.value).ToString("#0") + "B" + (_sliderB.value).ToString("#0") + "C" + (_sliderC.value).ToString("#0") + "D" + (_sliderD.value).ToString("#0") + "E" + (_sliderE.value).ToString("#0") + "F" + (_sliderF.value).ToString("#0") + ").";
            chatter_pub.Publish(msg);
            
        }
    }
}

}  // namespace ROS2
