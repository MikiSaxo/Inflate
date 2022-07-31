using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RayonsMovement : MonoBehaviour
{
    private float zAngle = 0f;
    void Start()
    {
        zAngle = Time.deltaTime * 3;
    }

    private void Update()
    {
        transform.Rotate(0, 0, zAngle, Space.Self);
    }
}
