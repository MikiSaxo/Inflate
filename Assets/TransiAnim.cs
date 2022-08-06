using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TransiAnim : MonoBehaviour
{
    public float TimeTransi;
    public static TransiAnim Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void MakeTransiOn()
    {
        transform.DOScaleY(33f, TimeTransi).SetEase(Ease.InExpo);
    }

    public void MakeTransiOff()
    {
        transform.DOScaleY(1f, TimeTransi).SetEase(Ease.InExpo);
    }
}
