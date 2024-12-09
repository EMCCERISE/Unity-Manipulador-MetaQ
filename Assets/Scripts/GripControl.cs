using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GripControl : MonoBehaviour
{
    [SerializeField] private Slider _sliderGripper;
    [SerializeField] private GameObject _buttonOpen;
    [SerializeField] private GameObject _buttonClose;
    [SerializeField] private float _maxOpeningAngle;
    [SerializeField] private float _maxClosingAngle;


    public void OpenGrip()
    {
        _sliderGripper.value = _maxOpeningAngle;
        _buttonOpen.SetActive(false);
        _buttonClose.SetActive(true);
    }
    public void CloseGrip()
    {
        _sliderGripper.value = _maxClosingAngle;
        _buttonOpen.SetActive(true);
        _buttonClose.SetActive(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_buttonOpen.activeInHierarchy)
            {
                OpenGrip();
            }
            else
            {
                CloseGrip();
            }
        }
    }
}
