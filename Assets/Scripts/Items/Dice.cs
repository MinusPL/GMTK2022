using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float ShuffleTime = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating("Shuffle", 0f, ShuffleTime);
    }

    public void OnShuffle(InputValue value)
    {
        CancelInvoke("Shuffle");
    }

    private void Shuffle()
    {
        float dirX = Random.Range(-500, 500);
        float dirY = Random.Range(-500, 500);
        float dirZ = Random.Range(-500, 500);
        Vector3 dir = new Vector3(dirX, dirY, dirZ).normalized;
        rb.AddForce(dir * 500);
        rb.AddTorque(dirX, dirY, dirZ);
    }
}
