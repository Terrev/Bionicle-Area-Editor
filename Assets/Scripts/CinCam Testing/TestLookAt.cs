using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestLookAt : MonoBehaviour
{
    public Vector3 target;

    void LateUpdate()
    {
        transform.LookAt(target);
    }
}
