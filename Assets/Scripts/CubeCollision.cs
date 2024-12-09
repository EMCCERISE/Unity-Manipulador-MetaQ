using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCollision : MonoBehaviour
{
    [SerializeField] public Rigidbody _cube;

    // Update is called once per frame
    void OnCollisionStay()
    {
        _cube.velocity = Vector3.zero;
    }

}
