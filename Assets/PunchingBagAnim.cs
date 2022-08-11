using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchingBagAnim : MonoBehaviour
{
    [SerializeField] private Vector3 punchDir;
    [SerializeField] private float powerStart;
    private float powerActual;
    [SerializeField] private float powerAdd;
    [SerializeField] private Rigidbody2D rb;

    public static PunchingBagAnim Instance;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ResetPunch();
    }

    public void Punch()
    {
        rb.velocity = Vector2.zero;
        powerActual += powerAdd;

        int randoDir = Random.Range(-1, 1);
        if (randoDir == 0)
            randoDir = 1;

        rb.AddForce(punchDir * randoDir * powerActual);
    }

    public void ResetPunch()
    {
        rb.velocity = Vector3.zero;
        transform.localPosition = new Vector3(0, 120, 1);
        powerActual = powerStart;
    }
}
