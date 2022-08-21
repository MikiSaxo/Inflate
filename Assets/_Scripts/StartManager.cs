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
    [SerializeField] private GameObject[] licorne;

    [SerializeField] private float howMuchToGrow;
    private float actualgrow = 1f;

    void Start()
    {
        actualgrow = 1f;
        LaunchStartScale();
    }

    private void LaunchStartScale()
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
        logoTitle.transform.DOScale(Vector3.one, timeBounce / 2);
        rayons.transform.DOScale(Vector3.one, timeBounce / 2);
        yield return new WaitForSeconds(1f);
        StartCoroutine(BounceLogo());
    }

    public void GrowLogo()
    {
        if (actualgrow > 2f)
            return;

        actualgrow += howMuchToGrow;
        Vector3 _grow = Vector3.one * actualgrow;
        logo.transform.DOScale(_grow, .2f);

        if (actualgrow > 2f)
            StartCoroutine(EasterEggDiscover());
    }

    IEnumerator EasterEggDiscover()
    {
        print("easterEgg unlocked");

        logo.transform.DOKill();
        logo.transform.localScale = Vector3.zero;
        licorne[0].SetActive(true);
        licorne[1].SetActive(true);
        
        Manager.Instance.HasDiscoverEasterEgg();
        ShakeAnim.Instance.StartShakingCam(20);

        yield return new WaitForSeconds(1f);

        logo.transform.DOScale(Vector3.one, timeToArrive).SetEase(Ease.InExpo);
    }
}
