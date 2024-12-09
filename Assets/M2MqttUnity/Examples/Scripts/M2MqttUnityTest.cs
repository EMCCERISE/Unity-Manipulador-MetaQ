using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using M2MqttUnity;
using System.Text.RegularExpressions;
using DG.Tweening;

namespace M2MqttUnity.Examples
{
    public class M2MqttUnityTest : M2MqttUnityClient
    {
        [SerializeField] private Slider _sliderA;
        [SerializeField] private Slider _sliderB;
        [SerializeField] private Slider _sliderC;
        [SerializeField] private Slider _sliderD;
        [SerializeField] private Slider _sliderE;
        [SerializeField] private Slider _sliderF;
        [SerializeField] private Slider _sliderGripper;
        [SerializeField] private GameObject _cubeTarget;
        [SerializeField] private GameObject _eeJoint;
        [SerializeField] private Button _buttonShadowActive;
        [SerializeField] private Button _buttonShadowInactive;
        [SerializeField] private Button _buttonGripOpen;
        [SerializeField] private Button _buttonGripClose;
        [SerializeField] public string _mqttTopicSend;
        [SerializeField] public string _mqttTopicReceive;
        [SerializeField] private float _sliderThreshold;
        [SerializeField] public float _updateInterval;
        [SerializeField] public float _lastUpdateInterval;
        [SerializeField] private float _gripperMaxOpeningAngle = 28f;
        [SerializeField] private float _gripperMaxClosingAngle = -17f;

        public bool autoTest = false;
        [Header("User Interface")]
        public InputField consoleInputField;
        public Toggle encryptedToggle;
        public InputField addressInputField;
        public InputField portInputField;
        public Button connectButton;
        public Button disconnectButton;
        public Button testPublishButton;
        public Button clearButton;

        // Adicione aqui uma referência ao seu elemento de texto na tela
        [SerializeField] private GameObject errorText;

        private List<string> eventMessages = new List<string>();
        private bool updateUI = false;
        private bool cubeMoveTrigger = false;
        private float cubeXPos = 0.0f;
        private float cubeYPos = 0.0f;
        private float cubeZPos = 0.0f;

        public float _sliderALastValue = 0f;
        public float _sliderBLastValue = 0f;
        public float _sliderCLastValue = 0f;
        public float _sliderDLastValue = 0f;
        public float _sliderELastValue = 0f;
        public float _sliderFLastValue = 0f;
        public float _sliderGripperLastValue = 0f;
        public float _lastUpdate = 0f;
        public float _lastReceivedUpdate = 0f;

        private Vector3 _lastTargetPosition;
        private float _lastTargetMoveTime = 0f;
        private bool _updateRealRobotJointsCalled = false;

        public void TestPublish()
        {
            if ((Mathf.Abs(_sliderALastValue - _sliderA.value) > _sliderThreshold) ||
                (Mathf.Abs(_sliderBLastValue - _sliderB.value) > _sliderThreshold) ||
                (Mathf.Abs(_sliderCLastValue - _sliderC.value) > _sliderThreshold) ||
                (Mathf.Abs(_sliderDLastValue - _sliderD.value) > _sliderThreshold) ||
                (Mathf.Abs(_sliderELastValue - _sliderE.value) > _sliderThreshold) ||
                (Mathf.Abs(_sliderFLastValue - _sliderF.value) > _sliderThreshold) ||
                (Mathf.Abs(_sliderGripperLastValue - _sliderGripper.value) > _sliderThreshold))
            {
                UpdateLastSliderValues();
                string messagePublished = "A" + (Mathf.RoundToInt(_sliderA.value)).ToString() +
                                          "B" + (Mathf.RoundToInt(_sliderB.value)).ToString() +
                                          "C" + (Mathf.RoundToInt(_sliderC.value)).ToString() +
                                          "D" + (Mathf.RoundToInt(_sliderD.value)).ToString() +
                                          "E" + (Mathf.RoundToInt(_sliderE.value)).ToString() +
                                          "F" + (Mathf.RoundToInt(_sliderF.value)).ToString() +
                                          "G" + (Mathf.RoundToInt(_sliderGripper.value)).ToString();
                client.Publish(_mqttTopicSend, System.Text.Encoding.UTF8.GetBytes(messagePublished), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                AddUiMessage("Publicado -> " + messagePublished);
            }
        }

        public void UpdateLastSliderValues()
        {
            _sliderALastValue = _sliderA.value;
            _sliderBLastValue = _sliderB.value;
            _sliderCLastValue = _sliderC.value;
            _sliderDLastValue = _sliderD.value;
            _sliderELastValue = _sliderE.value;
            _sliderFLastValue = _sliderF.value;
            _sliderGripperLastValue = _sliderGripper.value;
        }

        public void SetBrokerAddress(string brokerAddress)
        {
            if (addressInputField && !updateUI)
            {
                this.brokerAddress = brokerAddress;
            }
        }

        public void SetBrokerPort(string brokerPort)
        {
            if (portInputField && !updateUI)
            {
                int.TryParse(brokerPort, out this.brokerPort);
            }
        }

        public void SetEncrypted(bool isEncrypted)
        {
            this.isEncrypted = isEncrypted;
        }

        public void SetUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text = msg;
                updateUI = true;
            }
        }

        public void AddUiMessage(string msg)
        {
            if (consoleInputField != null)
            {
                consoleInputField.text += msg + "\n";
                updateUI = true;
            }
        }

        protected override void OnConnecting()
        {
            base.OnConnecting();
            SetUiMessage("Conectando ao broker " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        }

        protected override void OnConnected()
        {
            base.OnConnected();
            SetUiMessage("Conectado!\n");
            UpdateLastSliderValues();
            _lastUpdate = Time.time;
            if (autoTest)
            {
                TestPublish();
            }
        }

        protected override void SubscribeTopics()
        {
            client.Subscribe(new string[] { _mqttTopicReceive }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        }

        protected override void UnsubscribeTopics()
        {
            client.Unsubscribe(new string[] { _mqttTopicReceive });
        }

        protected override void OnConnectionFailed(string errorMessage)
        {
            AddUiMessage("Conexão falhou! " + errorMessage);
        }

        protected override void OnDisconnected()
        {
            AddUiMessage("Desconectado.");
        }

        protected override void OnConnectionLost()
        {
            AddUiMessage("Conexão perdida!");
        }

        private void UpdateUI()
        {
            if (client == null)
            {
                if (connectButton != null)
                {
                    connectButton.interactable = true;
                    disconnectButton.interactable = false;
                    testPublishButton.interactable = false;
                }
            }
            else
            {
                if (testPublishButton != null)
                {
                    testPublishButton.interactable = client.IsConnected;
                }
                if (disconnectButton != null)
                {
                    disconnectButton.interactable = client.IsConnected;
                }
                if (connectButton != null)
                {
                    connectButton.interactable = !client.IsConnected;
                }
            }
            if (addressInputField != null && connectButton != null)
            {
                addressInputField.interactable = connectButton.interactable;
                addressInputField.text = brokerAddress;
            }
            if (portInputField != null && connectButton != null)
            {
                portInputField.interactable = connectButton.interactable;
                portInputField.text = brokerPort.ToString();
            }
            if (encryptedToggle != null && connectButton != null)
            {
                encryptedToggle.interactable = connectButton.interactable;
                encryptedToggle.isOn = isEncrypted;
            }
            if (clearButton != null && connectButton != null)
            {
                clearButton.interactable = connectButton.interactable;
            }
            updateUI = false;
        }

        protected override void Start()
        {
            SetUiMessage("Pronto.");
            updateUI = true;
            // Esconde o texto de erro ao iniciar
            if (errorText != null)
            {
                errorText.gameObject.SetActive(false);
            }
            base.Start();
        }

        protected override void DecodeMessage(string topic, byte[] message)
        {
            string msg = System.Text.Encoding.UTF8.GetString(message);
            StoreMessage(msg);
            if (topic == _mqttTopicReceive)
            {
                if (autoTest)
                {
                    autoTest = false;
                    Disconnect();
                }
            }
        }

        private void StoreMessage(string eventMsg)
        {
            eventMessages.Add(eventMsg);
        }

        private void ProcessMessage(string msg)
        {
            AddUiMessage("Recebido -> " + msg);
            _lastReceivedUpdate = Time.time;

            // Se a mensagem recebida for "ZZZ", exibir o texto e ocultar após 2 segundos
            if (msg == "ZZZ" && errorText != null)
            {
                errorText.gameObject.SetActive(true);
                StartCoroutine(HideErrorTextAfterDelay(2f));
                return; // Não precisa processar regex abaixo se for ZZZ
            }

            string input = msg;
            string pattern = @"[A-Za-z]-?\d+";
            cubeXPos = _cubeTarget.transform.position.x;
            cubeYPos = _cubeTarget.transform.position.y;
            cubeZPos = _cubeTarget.transform.position.z;

            MatchCollection matches = Regex.Matches(input, pattern);

            foreach (Match match in matches)
            {
                if (match.Value.Contains("A"))
                {
                    _sliderA.SetValueWithoutNotify(float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float));
                }
                else if (match.Value.Contains("B"))
                {
                    _sliderB.SetValueWithoutNotify(float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float));
                }
                else if (match.Value.Contains("C"))
                {
                    _sliderC.SetValueWithoutNotify(float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float));
                }
                else if (match.Value.Contains("D"))
                {
                    _sliderD.SetValueWithoutNotify(float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float));
                }
                else if (match.Value.Contains("E"))
                {
                    _sliderE.SetValueWithoutNotify(float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float));
                }
                else if (match.Value.Contains("F"))
                {
                    _sliderF.SetValueWithoutNotify(float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float));
                }
                else if (match.Value.Contains("G"))
                {
                    float gripVal = float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float);
                    if (gripVal.Equals(0f))
                    {
                        _buttonGripOpen.gameObject.SetActive(false);
                        _buttonGripClose.gameObject.SetActive(true);
                        _sliderGripper.SetValueWithoutNotify(_gripperMaxOpeningAngle);
                    }
                    else if (gripVal.Equals(1f))
                    {
                        _buttonGripOpen.gameObject.SetActive(true);
                        _buttonGripClose.gameObject.SetActive(false);
                        _sliderGripper.SetValueWithoutNotify(_gripperMaxClosingAngle);
                    }
                    else if (gripVal.Equals(-1f))
                    {
                        if (_buttonGripOpen.gameObject.activeInHierarchy)
                        {
                            _buttonGripOpen.onClick.Invoke();
                        }
                        else
                        {
                            _buttonGripClose.onClick.Invoke();
                        }
                    }
                    else
                    {
                        _sliderGripper.SetValueWithoutNotify(gripVal);
                    }
                }
                else if (match.Value.Contains("S"))
                {
                    if (_buttonShadowActive.gameObject.activeInHierarchy)
                    {
                        _buttonShadowActive.onClick.Invoke();
                    }
                    else
                    {
                        _buttonShadowInactive.onClick.Invoke();
                    }
                }
                else if (match.Value.Contains("X"))
                {
                    cubeMoveTrigger = true;
                    cubeXPos = float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float);
                }
                else if (match.Value.Contains("Y"))
                {
                    cubeMoveTrigger = true;
                    cubeYPos = float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float);
                }
                else if (match.Value.Contains("Z"))
                {
                    cubeMoveTrigger = true;
                    cubeZPos = float.Parse(match.Value.Substring(1), System.Globalization.NumberStyles.Float);
                }
                if (cubeMoveTrigger)
                {
                    _buttonShadowActive.gameObject.SetActive(true);
                    _buttonShadowInactive.gameObject.SetActive(false);
                    _eeJoint.SetActive(true);
                    _cubeTarget.SetActive(true);
                    Vector3 cubePosition = new Vector3(cubeXPos / 1000, cubeYPos / 1000, cubeZPos / 1000);
                    _cubeTarget.transform.DOMove(cubePosition, 1.0f).SetEase(Ease.Linear);
                    cubeMoveTrigger = false;
                }
            }
        }

        protected override void Update()
        {
            base.Update(); // call ProcessMqttEvents()

            if ((Mathf.Abs(Time.time - _lastUpdate) > _updateInterval) & (Mathf.Abs(Time.time - _lastReceivedUpdate) > _lastUpdateInterval))
            {
                _lastUpdate = Time.time;
                TestPublish();
            }

            if (eventMessages.Count > 0)
            {
                foreach (string msg in eventMessages)
                {
                    ProcessMessage(msg);
                }
                eventMessages.Clear();
            }
            if (updateUI)
            {
                UpdateUI();
            }

            if (_cubeTarget.transform.position != _lastTargetPosition)
            {
                _lastTargetPosition = _cubeTarget.transform.position;
                _lastTargetMoveTime = Time.time;
                _updateRealRobotJointsCalled = false;
            }
            else if (Time.time - _lastTargetMoveTime > 3f && !_updateRealRobotJointsCalled)
            {
                string messagePublished =
                    "X" + Mathf.RoundToInt(_cubeTarget.transform.position.x * 1000f).ToString() +
                    "Y" + Mathf.RoundToInt(_cubeTarget.transform.position.y * 1000f).ToString() +
                    "Z" + Mathf.RoundToInt(_cubeTarget.transform.position.z * 1000f).ToString() +
                    "Qx" + Mathf.RoundToInt(_cubeTarget.transform.rotation.x * 1000f).ToString() +
                    "Qy" + Mathf.RoundToInt(_cubeTarget.transform.rotation.y * 1000f).ToString() +
                    "Qz" + Mathf.RoundToInt(_cubeTarget.transform.rotation.z * 1000f).ToString() +
                    "Qw" + Mathf.RoundToInt(_cubeTarget.transform.rotation.w * 1000f).ToString();
                client.Publish(_mqttTopicSend, System.Text.Encoding.UTF8.GetBytes(messagePublished), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
                AddUiMessage("Publicado IK -> " + messagePublished);
                _updateRealRobotJointsCalled = true;
            }
        }

        IEnumerator HideErrorTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            errorText.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        private void OnValidate()
        {
            if (autoTest)
            {
                autoConnect = true;
            }
        }

        public void SwapTopic()
        {
            string _mqttTopicTemp = _mqttTopicSend;
            _mqttTopicSend = _mqttTopicReceive;
            _mqttTopicReceive = _mqttTopicTemp;
        }
    }
}
