using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeAnim : MonoBehaviour
{
    [SerializeField] private bool start = false;
    [SerializeField] private AnimationCurve curve;
    public float duration = 1f;

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

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
    }

    public void StartShaking()
    {
        StartCoroutine(Shaking());
    }
}
