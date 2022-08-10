using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShakeAnim : MonoBehaviour
{
    [SerializeField] private bool start = false;
    [SerializeField] private AnimationCurve curve;
    public float durationShaking = 1f;
    public float durationZoom = 0f;

    public static ShakeAnim Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(Shaking());
        }
    }

    private IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < durationZoom)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / durationZoom);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }

    public void StartShaking()
    {
        StartCoroutine(Shaking());
    }

    private IEnumerator ZoomCam(float _numberZoom)
    {
        gameObject.GetComponent<Camera>().DOKill();
        gameObject.GetComponent<Camera>().DOOrthoSize(5f - _numberZoom, .1f);
        yield return new WaitForSeconds(durationZoom);
        gameObject.GetComponent<Camera>().DOOrthoSize(5f, .05f);
    }

    public void StartZoom(float _numberZoom)
    {
        StartCoroutine(ZoomCam(_numberZoom));
    }
}
