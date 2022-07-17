using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dice : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float ShuffleTime = 0.2f;

    [SerializeField]
    private Transform faces;

    List<Transform> sides;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sides = new List<Transform>();
        foreach(Transform child in faces)
        {
            sides.Add(child);
        }
    }

    public void StopShuffle()
    {
        CancelInvoke("Shuffle");
    }

    public void StartShuffling()
    {
        InvokeRepeating("Shuffle", 0f, ShuffleTime);
    }

    private void Shuffle()
    {
        float dirX = Random.Range(-100, 100);
        float dirY = Random.Range(-100, 100);
        float dirZ = Random.Range(-100, 100);
        Vector3 dir = new Vector3(dirX, dirY, dirZ).normalized;
        rb.AddForce(dir * 100);
        rb.AddTorque(dirX, dirY, dirZ);
    }

    public int CheckTopFace(Vector3 tableDownDir)
    {
        float bestDot = -1f;
        Transform bestSide = sides[0];

        foreach(Transform t in sides)
        {
            float dot = Vector3.Dot(tableDownDir, t.forward);
            if(dot > bestDot)
            {
                bestSide = t;
                bestDot = dot;
            }
        }

        return bestSide.gameObject.GetComponent<DiceSideVector>().sideValue;
    }
}
