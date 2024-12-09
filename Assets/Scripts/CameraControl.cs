using Assimp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private GameObject CameraTop;
    [SerializeField] private GameObject CameraFront;
    [SerializeField] private GameObject CameraBack;
    [SerializeField] private GameObject CameraLeft;
    [SerializeField] private GameObject CameraRight;
    [SerializeField] private GameObject CameraReset;
    [SerializeField] private GameObject CameraGrip;
    [SerializeField] private UnityEngine.UI.Button ButtonTop;
    [SerializeField] private UnityEngine.UI.Button ButtonFront;
    [SerializeField] private UnityEngine.UI.Button ButtonBack;
    [SerializeField] private UnityEngine.UI.Button ButtonLeft;
    [SerializeField] private UnityEngine.UI.Button ButtonRight;
    [SerializeField] private UnityEngine.UI.Button ButtonReset;
    [SerializeField] private UnityEngine.UI.Button ButtonCameraGrip;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown("s") | Input.GetKeyDown(KeyCode.Keypad5))
        {
            SetCameraTop();
        }
        if (Input.GetKeyDown("w") | Input.GetKeyDown(KeyCode.Keypad8))
        {
            SetCameraFront();
        }
        if (Input.GetKeyDown("x") | Input.GetKeyDown(KeyCode.Keypad2))
        {
            SetCameraBack();
        }
        if (Input.GetKeyDown("a") | Input.GetKeyDown(KeyCode.Keypad4))
        {
            SetCameraLeft();
        }
        if (Input.GetKeyDown("d") | Input.GetKeyDown(KeyCode.Keypad6))
        {
            SetCameraRight();
        }
        if (Input.GetKeyDown(KeyCode.Space) | Input.GetKeyDown(KeyCode.Keypad0))
        {
            SetCameraReset();
        }
    }

    public void SetCameraTop()
    {
        CameraTop.SetActive(true);
        CameraFront.SetActive(false);
        CameraBack.SetActive(false);
        CameraLeft.SetActive(false);
        CameraRight.SetActive(false);
        CameraReset.SetActive(false);
        CameraGrip.SetActive(false);
        ButtonTop.Select();
    }

    public void SetCameraGrip()
    {
        CameraTop.SetActive(false);
        CameraFront.SetActive(false);
        CameraBack.SetActive(false);
        CameraLeft.SetActive(false);
        CameraRight.SetActive(false);
        CameraReset.SetActive(false);
        CameraGrip.SetActive(true);
        ButtonCameraGrip.Select();
    }
    public void SetCameraFront()
    {
        CameraTop.SetActive(false);
        CameraFront.SetActive(true);
        CameraBack.SetActive(false);
        CameraLeft.SetActive(false);
        CameraRight.SetActive(false);
        CameraReset.SetActive(false);
        CameraGrip.SetActive(false);
        ButtonFront.Select();
    }

    public void SetCameraBack()
    {
        CameraTop.SetActive(false);
        CameraFront.SetActive(false);
        CameraBack.SetActive(true);
        CameraLeft.SetActive(false);
        CameraRight.SetActive(false);
        CameraReset.SetActive(false);
        CameraGrip.SetActive(false);
        ButtonBack.Select();
    }

    public void SetCameraLeft()
    {
        CameraTop.SetActive(false);
        CameraFront.SetActive(false);
        CameraBack.SetActive(false);
        CameraLeft.SetActive(true);
        CameraRight.SetActive(false);
        CameraReset.SetActive(false);
        CameraGrip.SetActive(false);
        ButtonLeft.Select();
    }

    public void SetCameraRight()
    {
        CameraTop.SetActive(false);
        CameraFront.SetActive(false);
        CameraBack.SetActive(false);
        CameraLeft.SetActive(false);
        CameraRight.SetActive(true);
        CameraReset.SetActive(false);
        CameraGrip.SetActive(false);
        ButtonRight.Select();
    }
    public void SetCameraReset()
    {
        CameraTop.SetActive(false);
        CameraFront.SetActive(false);
        CameraBack.SetActive(false);
        CameraLeft.SetActive(false);
        CameraRight.SetActive(false);
        CameraReset.SetActive(true);
        CameraGrip.SetActive(false);
        ButtonReset.Select();
    }
}
