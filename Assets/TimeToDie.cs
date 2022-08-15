using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeToDie : MonoBehaviour
{
    [SerializeField] private float timeToDie = 0f;

    void Start()
    {
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(timeToDie);
        Destroy(gameObject);
    }
}
