using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RayonsMovement : MonoBehaviour
{
    [SerializeField] private float speed = 0f;
    void Start()
    {
        transform.DORotate(new Vector3(0, 0, 360), speed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }
}
