using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StartManager : MonoBehaviour
{
    [SerializeField] private GameObject logoTitle;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject rayons;
    [SerializeField] private GameObject buttons;
    [SerializeField] private float timeToArrive;
    [SerializeField] private float timeBounce;

    void Start()
    {
        logo.transform.localScale = Vector3.zero;
        logo.transform.DOScale(Vector3.one, timeToArrive).SetEase(Ease.InExpo).OnComplete(LaunchBounceLogo);
    }

    private void LaunchBounceLogo()
    {
        StartCoroutine(BounceLogo());
        buttons.transform.DOMoveY(0, 2f);
    }

    IEnumerator BounceLogo()
    {
        logoTitle.transform.DOScale(Vector3.one * 1.2f, timeBounce);
        rayons.transform.DOScale(Vector3.one * 1.2f, timeBounce);
        yield return new WaitForSeconds(timeBounce);
        logoTitle.transform.DOScale(Vector3.one, timeBounce/2);
        rayons.transform.DOScale(Vector3.one, timeBounce/2);
        yield return new WaitForSeconds(1f);
        StartCoroutine(BounceLogo());
    }
}
