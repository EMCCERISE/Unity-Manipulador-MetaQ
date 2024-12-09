using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JointScript : MonoBehaviour
{
    public JointScript m_child;

    public JointScript GetChild()
    {
        return m_child;
    }

    public void Rotate(float _angle)
    {
        transform.Rotate(Vector3.up * _angle);
    }
}
