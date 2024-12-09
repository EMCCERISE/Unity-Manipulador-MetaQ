using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEditor;
using System.Reflection;

public class _3DObjectFollowCursorMyCobot : MonoBehaviour
{
    [Header("Button Control")]
    [SerializeField] private GameObject _buttonActive;
    [SerializeField] private Button _buttonSelected;
    [SerializeField] private GameObject _buttonInactive;
    [SerializeField] private GameObject buttonSliders;


    [Header("IK Parameters")]
    [SerializeField] private GameObject _eeJoint;
    [SerializeField] private GameObject _target;
    [SerializeField] public Rigidbody _rigidbody;
    [SerializeField] private float _updateInterval = 1f;

    [Header("Slider Controllers")]
    [SerializeField] public UnityEngine.UI.Slider _sliderA;
    [SerializeField] public UnityEngine.UI.Slider _sliderB;
    [SerializeField] public UnityEngine.UI.Slider _sliderC;
    [SerializeField] public UnityEngine.UI.Slider _sliderD;
    [SerializeField] public UnityEngine.UI.Slider _sliderE;
    [SerializeField] public UnityEngine.UI.Slider _sliderF;
    [SerializeField] public UnityEngine.UI.Slider _sliderDepth;

    [Header("Robot Grip")]
    [SerializeField] public GameObject _gripL;
    [SerializeField] public GameObject _gripR;


    [Header("Shadow Robot")]
    [SerializeField] public GameObject _shadowA;
    [SerializeField] public GameObject _shadowB;
    [SerializeField] public GameObject _shadowC;
    [SerializeField] public GameObject _shadowD;
    [SerializeField] public GameObject _shadowE;
    [SerializeField] public GameObject _shadowF;

    private bool _keyWasPressed = false;
    private bool _IKModeStarted = false;
    private float _lastUpdate = 0f;
    [SerializeField] private float _bodyAOffset = 0f;
    [SerializeField] private float _bodyBOffset = 0f;
    [SerializeField] private float _bodyCOffset = 0f;
    [SerializeField] private float _bodyDOffset = 00f;
    [SerializeField] private float _bodyEOffset = 90f;
    [SerializeField] private float _bodyFOffset = 00f;
    [SerializeField] private float _bodyAMult = 1f;
    [SerializeField] private float _bodyBMult = -1f;
    [SerializeField] private float _bodyCMult = -1f;
    [SerializeField] private float _bodyDMult = -1f;
    [SerializeField] private float _bodyEMult = 1f;
    [SerializeField] private float _bodyFMult = 1f;

    [Header("Debug")]
    [SerializeField] public GameObject _referenceObject;


#if UNITY_ANDROID || UNITY_WEBGL
    private bool _jointsUpdated = false;
    private float _mouseDownTimer = 0f;
    private float _mouseUpTimer = 0f;

    private Vector3 _lastTargetPosition;
    private float _lastTargetMoveTime = 0f;
    private bool _updateRealRobotJointsCalled = false;
#endif

    #region Update
    void Update()
    {
        if (Input.GetKey(KeyCode.Q).Equals(true))
        {
            _buttonActive.SetActive(true);
            _buttonInactive.SetActive(false);
            _keyWasPressed = true;
        } else if (_keyWasPressed)
        {
            _keyWasPressed = false;
            _buttonActive.SetActive(false);
            _buttonInactive.SetActive(true);
        }
        if (_buttonActive.activeInHierarchy.Equals(true))
        {
            _eeJoint.SetActive(true);
            _target.SetActive(true);
            _sliderDepth.gameObject.SetActive(true);
            _IKModeStarted = true;
        }
        else
        {
            if (_IKModeStarted)
            {
                _IKModeStarted = false;
                if (buttonSliders.activeInHierarchy)
                {
                    //UpdateRealRobotJoints();
                }    
            }
            _eeJoint.SetActive(false);
            _target.SetActive(false);
            _sliderDepth.gameObject.SetActive(false);
        }
        if (_buttonActive.activeInHierarchy.Equals(true) & (Input.mouseScrollDelta.y != 0))
        {
            _sliderDepth.value += Input.mouseScrollDelta.y * 0.01f;
        }
#if UNITY_ANDROID || UNITY_WEBGL
// Verificação de movimento do _target
        if (_target.transform.position != _lastTargetPosition)
        {
            _lastTargetPosition = _target.transform.position;
            _lastTargetMoveTime = Time.time;
            _updateRealRobotJointsCalled = false; // Reseta a chamada ao detectar movimento
        }
        else if (Time.time - _lastTargetMoveTime > 2f && !_updateRealRobotJointsCalled)
        {
            //UpdateRealRobotJoints();
            _updateRealRobotJointsCalled = true; // Marca que a função foi chamada para evitar repetição
        }

        if (Input.GetMouseButtonDown(0))
        {
            _mouseDownTimer = Time.time;
        }
        if (_buttonActive.activeInHierarchy.Equals(true) & Input.GetMouseButtonUp(0) & !EventSystem.current.IsPointerOverGameObject())
        {
            _jointsUpdated = false;
            _mouseUpTimer = Time.time;
        }
        if (_buttonActive.activeInHierarchy.Equals(true) & (Time.time - _mouseUpTimer > 2f) & !_jointsUpdated) 
        {
            _jointsUpdated = true;
            //UpdateRealRobotJoints();
        }
        if (_buttonActive.activeInHierarchy.Equals(true) & !EventSystem.current.IsPointerOverGameObject() & (Input.GetMouseButton(0) | Input.GetKey(KeyCode.Q).Equals(true)) & (Time.time - _lastUpdate > _updateInterval) & (Time.time - _mouseDownTimer > 0.15f))
        {

            if (_gripL.transform.localEulerAngles.x != _shadowE.transform.localEulerAngles.x)
            {
                _shadowE.transform.DOLocalRotate(_gripL.transform.localEulerAngles, 0.5f);
            }
            if (_gripR.transform.localEulerAngles.x != _shadowF.transform.localEulerAngles.x)
            {
                _shadowF.transform.DOLocalRotate(_gripR.transform.localEulerAngles, 0.5f);
            }


            Vector3 position = Camera.allCameras[0].ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.allCameras[0].nearClipPlane + _sliderDepth.value)));
            _rigidbody.transform.DOMove(position, 0.1f).SetEase(Ease.Linear);
            _lastUpdate = Time.time;

            //Debug.Log(Mathf.DeltaAngle(0, _shadowA.transform.localEulerAngles.z));
            //Debug.Log("Rotations: " + _shadowA.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyA.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowB.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyB.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowC.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyC.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowD.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyD.transform.localEulerAngles.y.ToString("#.##0") + ".");
            //_rigidbody.position = Camera.current.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y.ToString("#.##0"), (Camera.current.nearClipPlane + 0.05f)));
        }
#else
        if (_buttonActive.activeInHierarchy.Equals(true) & !EventSystem.current.IsPointerOverGameObject() & (Input.GetMouseButton(0) | Input.GetKey(KeyCode.Q).Equals(true)) & (Time.time - _lastUpdate > _updateInterval))
        {

            if (_gripL.transform.localEulerAngles.x != _shadowE.transform.localEulerAngles.x)
            {
                _shadowE.transform.DOLocalRotate(_gripL.transform.localEulerAngles, 0.5f);
            }
            if (_gripR.transform.localEulerAngles.x != _shadowF.transform.localEulerAngles.x)
            {
                _shadowF.transform.DOLocalRotate(_gripR.transform.localEulerAngles, 0.5f);
            }


            Vector3 position = Camera.allCameras[0].ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, (Camera.allCameras[0].nearClipPlane + _sliderDepth.value)));
            //_rigidbody.MovePosition(position);
            _rigidbody.transform.DOMove(position, 0.1f).SetEase(Ease.Linear);
            //Debug.Log("As: " + (Mathf.DeltaAngle(0, _shadowA.transform.localEulerAngles.y) + _bodyAOffset).ToString("#.##0"));
            _lastUpdate = Time.time;

            //Debug.Log(Mathf.DeltaAngle(0, _shadowA.transform.localEulerAngles.z));
            //Debug.Log("Rotations: " + _shadowA.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyA.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowB.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyB.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowC.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyC.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowD.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyD.transform.localEulerAngles.y.ToString("#.##0") + ".");
            //_rigidbody.position = Camera.current.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y.ToString("#.##0"), (Camera.current.nearClipPlane + 0.05f)));
        }

#endif

    }

    void UpdateRealRobotJoints()
    {
        //Debug.Log("Al: " + (Mathf.DeltaAngle(0, _shadowA.transform.localEulerAngles.y) + _bodyAOffset).ToString("#.##0"));
        //Debug.Log("Ae: " + (Mathf.DeltaAngle(0, _shadowA.transform.eulerAngles.y) + _bodyAOffset).ToString("#.##0"));
        //Debug.Log("Az: " + (_shadowA.transform.eulerAngles.y + _bodyAOffset).ToString("#.##0"));
        Debug.Log("A- Calc:" + (_bodyBMult * (Mathf.DeltaAngle(0, _shadowB.transform.localEulerAngles.y) + _bodyBOffset)).ToString("#.##0") + "/Ins:" + ((GetInspectorRotation(_shadowB.transform).y)).ToString("#.##0") + "/Ang:" + (_shadowB.transform.localEulerAngles.y).ToString("#.##0"));
        Debug.Log("A- Quaternion Diff ID:" + (Quaternion.identity * Quaternion.Inverse(_shadowB.transform.rotation)).eulerAngles.x);
        Debug.Log("R- Quaternion Diff:" + (Quaternion.Inverse(_referenceObject.transform.rotation)) * Quaternion.Inverse(_shadowB.transform.rotation));
        //#if UNITY_ANDROID || UNITY_WEBGL
#if UNITY_ANDROID || UNITY_WEBGL
        _sliderA.value = _bodyAMult * (Mathf.DeltaAngle(0, _shadowA.transform.localEulerAngles.y) + _bodyAOffset);
        _sliderB.value = _bodyBMult * (Mathf.DeltaAngle(0, _shadowB.transform.localEulerAngles.x) + _bodyBOffset);
        _sliderC.value = _bodyCMult * (Mathf.DeltaAngle(0, _shadowC.transform.localEulerAngles.y) + _bodyCOffset);
        _sliderD.value = _bodyDMult * (Mathf.DeltaAngle(0, _shadowD.transform.localEulerAngles.y) + _bodyDOffset);
        _sliderE.value = _bodyEMult * (Mathf.DeltaAngle(0, _shadowE.transform.localEulerAngles.x) + _bodyEOffset);
        _sliderF.value = _bodyFMult * (Mathf.DeltaAngle(0, _shadowF.transform.localEulerAngles.y) + _bodyFOffset);
#else
        _sliderA.value = _bodyAMult * (GetInspectorRotation(_shadowA.transform).y + _bodyAOffset);
        _sliderB.value = _bodyBMult * (GetInspectorRotation(_shadowB.transform).x + _bodyBOffset);
        _sliderC.value = _bodyCMult * (GetInspectorRotation(_shadowC.transform).y + _bodyCOffset);
        _sliderD.value = _bodyDMult * (GetInspectorRotation(_shadowD.transform).y + _bodyDOffset);
        _sliderE.value = _bodyEMult * (GetInspectorRotation(_shadowE.transform).x + _bodyEOffset);
        _sliderF.value = _bodyFMult * (GetInspectorRotation(_shadowF.transform).y + _bodyFOffset);
#endif
        //_sliderA.value = _bodyAMult * (GetInspectorRotation(_shadowA.transform).y + _bodyAOffset);
        //Debug.Log("A: " + (_bodyAMult * (Mathf.DeltaAngle(0, _shadowA.transform.localEulerAngles.y) + _bodyAOffset)).ToString("#.##0"));
        //Debug.Log("Bl: " + (Mathf.DeltaAngle(0, _shadowB.transform.localEulerAngles.y) + _bodyBOffset).ToString("#.##0"));
        //Debug.Log("Be: " + (Mathf.DeltaAngle(0, _shadowB.transform.eulerAngles.y) + _bodyBOffset).ToString("#.##0"));
        //Debug.Log("Bz: " + (_shadowB.transform.eulerAngles.y + _bodyBOffset).ToString("#.##0"));
       // _sliderB.value = _bodyBMult * (GetInspectorRotation(_shadowB.transform).x + _bodyBOffset);
        //Debug.Log("B: " + (_bodyBMult * (Mathf.DeltaAngle(0, _shadowB.transform.localEulerAngles.x) + _bodyBOffset)).ToString("#.##0") + "/" + ((GetInspectorRotation(_shadowB.transform).x)).ToString("#.##0") + "/" + (_shadowB.transform.localEulerAngles.x).ToString("#.##0"));
        //Debug.Log("Cl: " + (Mathf.DeltaAngle(0, _shadowC.transform.localEulerAngles.y) + _bodyCOffset).ToString("#.##0"));
        //Debug.Log("Ce: " + (Mathf.DeltaAngle(0, _shadowC.transform.eulerAngles.y) + _bodyCOffset).ToString("#.##0"));
        //Debug.Log("Cz: " + (_shadowC.transform.eulerAngles.y + _bodyCOffset).ToString("#.##0"));
        //_sliderC.value = _bodyCMult * (GetInspectorRotation(_shadowC.transform).y + _bodyCOffset);
        //Debug.Log("C: " + (_bodyCMult * (Mathf.DeltaAngle(0, _shadowC.transform.localEulerAngles.y) + _bodyCOffset)).ToString("#.##0"));
        //Debug.Log("Dl: " + (Mathf.DeltaAngle(0, _shadowD.transform.localEulerAngles.y) + _bodyDOffset).ToString("#.##0"));
        //Debug.Log("De: " + (Mathf.DeltaAngle(0, _shadowD.transform.eulerAngles.y) + _bodyDOffset).ToString("#.##0"));
        //Debug.Log("Dz: " + (_shadowD.transform.eulerAngles.y + _bodyDOffset).ToString("#.##0"));
        //_sliderD.value = _bodyDMult * (GetInspectorRotation(_shadowD.transform).y + _bodyDOffset);
        //Debug.Log("D: " + (_bodyDMult * (Mathf.DeltaAngle(0, _shadowD.transform.localEulerAngles.y) + _bodyDOffset)).ToString("#.##0"));
        //Debug.Log("Rotations: " + _shadowA.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyA.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowB.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyB.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowC.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyC.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowD.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyD.transform.localEulerAngles.y.ToString("#.##0") + ".");
       // _sliderE.value = _bodyEMult * (GetInspectorRotation(_shadowE.transform).x + _bodyEOffset);
        //Debug.Log("E: " + (_bodyEMult * (Mathf.DeltaAngle(0, _shadowE.transform.localEulerAngles.x) + _bodyEOffset)).ToString("#.##0"));
        //Debug.Log("Rotations: " + _shadowA.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyA.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowB.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyB.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowC.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyC.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowD.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyD.transform.localEulerAngles.y.ToString("#.##0") + ".");
       // _sliderF.value = _bodyFMult * (GetInspectorRotation(_shadowF.transform).y + _bodyFOffset);
        //Debug.Log("F: " + (_bodyFMult * (Mathf.DeltaAngle(0, _shadowF.transform.localEulerAngles.y) + _bodyFOffset)).ToString("#.##0"));
        //Debug.Log("Rotations: " + _shadowA.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyA.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowB.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyB.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowC.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyC.transform.localEulerAngles.y.ToString("#.##0") + ", " + _shadowD.transform.localEulerAngles.y.ToString("#.##0") + "/" + _bodyD.transform.localEulerAngles.y.ToString("#.##0") + ".");
    }
#endregion

    private Vector3 GetInspectorRotation(Transform targetTransform)
    {
        Vector3 vect3 = Vector3.zero;
        MethodInfo mth = typeof(Transform).GetMethod("GetLocalEulerAngles", BindingFlags.Instance | BindingFlags.NonPublic);
        PropertyInfo pi = typeof(Transform).GetProperty("rotationOrder", BindingFlags.Instance | BindingFlags.NonPublic);
        object rotationOrder = null;
        if (pi != null)
        {
            rotationOrder = pi.GetValue(targetTransform, null);
        }
        if (mth != null)
        {
            object retVector3 = mth.Invoke(targetTransform, new object[] { rotationOrder });
            vect3 = (Vector3)retVector3;
            Debug.Log("Get Inspector Euler:" + vect3);
        }
        return vect3;
    }
}
