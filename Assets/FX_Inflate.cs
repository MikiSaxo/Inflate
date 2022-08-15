using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_Inflate : MonoBehaviour
{
    [SerializeField] private GameObject uiParent = null;
    [SerializeField] private GameObject fx_Sourcils = null;
    [SerializeField] private GameObject fx_Stress = null;

    [SerializeField] private GameObject inflate_Buttons = null;
    private GameObject whichFX = null;
    [SerializeField] private float durationShaking = 0f;
    [SerializeField] private AnimationCurve curve;


    public static FX_Inflate Instance;
    private void Awake()
    {
        Instance = this;
    }

    public void Start_FX_Inflate()
    {
        var _randomPrefab = Random.Range(-1, 1);
        if (_randomPrefab == -1)
            whichFX = Instantiate(fx_Sourcils, uiParent.transform);
        else
            whichFX = Instantiate(fx_Stress, uiParent.transform);

        var _randomSide = Random.Range(-1, 1);
        if (_randomSide == -1)
            whichFX.transform.localScale = new Vector3(_randomSide, 1, 1);

        StartShaking(inflate_Buttons);
    }

    public void StartShaking(GameObject which_Object)
    {
        StartCoroutine(Shaking(which_Object));
    }

    private IEnumerator Shaking(GameObject which_Object)
    {
        Vector3 startPosition = which_Object.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < durationShaking)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / durationShaking);
            which_Object.transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        which_Object.transform.position = startPosition;
    }
}
