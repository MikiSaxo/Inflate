using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bounce : MonoBehaviour
{
    [SerializeField] private float timeBounce;

    void Start()
    {
        StartCoroutine(Bouncee());
    }

    IEnumerator Bouncee()
    {
        transform.DOScale(Vector3.one * 1.2f, timeBounce);
        yield return new WaitForSeconds(timeBounce);
        transform.DOScale(Vector3.one, timeBounce / 2);
        yield return new WaitForSeconds(1f);
        StartCoroutine(Bouncee());
    }
}
