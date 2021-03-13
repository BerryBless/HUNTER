using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 6f;
    void Start()
    {
        
    }

    void Update()
    {
        float axisV = Input.GetAxisRaw("Vertical");
        float axisH = Input.GetAxisRaw("Horizontal");
        Vector3 dir = Vector3.zero;
        if (axisV != 0)
        {
            dir = axisV > 0 ? Vector3.up : Vector3.down;
        }
        else if (axisH != 0)
        {
            dir = axisH > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
        }
        transform.position += dir * Time.deltaTime * _speed;
    }
}
