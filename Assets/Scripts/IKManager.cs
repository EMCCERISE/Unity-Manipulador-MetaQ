using RosMessageTypes.Std;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    // Start is called before the first frame update

    //
    public JointScript m_root;


    public JointScript m_end;

    public GameObject m_target;

    public float m_threshhold = 0.05f;

    public float m_rate = 5.0f;


    float CalculateSlope(JointScript _joint)
    {
        float deltaTheta = 0.01f;
        float distance1 = GetDistance(m_end.transform.position, m_target.transform.position);

        _joint.Rotate(deltaTheta);

        float distance2 = GetDistance(m_end.transform.position, m_target.transform.position);

        _joint.Rotate(-deltaTheta);

        return (distance2 - distance1) / deltaTheta;

    }

    // Update is called once per frame
    void Update()
    {
        if (GetDistance(m_end.transform.position, m_target.transform.position) > m_threshhold)
        {
            float slope = CalculateSlope(m_root); ;
            m_root.Rotate(-slope * m_rate);
        }
        
    }

    float GetDistance(Vector3 _point1, Vector3 _point2)
    {
        return Vector3.Distance(_point1, _point2);
    }
}
