using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ButtonControl : MonoBehaviour
{
    [SerializeField] private GameObject _buttonOn;
    [SerializeField] private GameObject _buttonOff;


    public void ButtonOn()
    {
        _buttonOn.SetActive(true);
        _buttonOff.SetActive(false);
    }
    public void ButtonOff()
    {
        _buttonOn.SetActive(false);
        _buttonOff.SetActive(true);
    }
}
