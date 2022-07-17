using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    [SerializeField]
    private GameObject mover;

    [SerializeField]
    private GameObject waypoint;
    [SerializeField]
    private GameObject gateOrigin;

    [SerializeField]
    private float moveTime = 2f;
    private float moveTimer = 0;

    private bool shouldMove = false;

   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldMove)
        {
            moveTimer += Time.deltaTime;
            mover.transform.position = Vector3.Lerp(gateOrigin.transform.position, waypoint.transform.position, moveTimer/moveTime);
            if(moveTimer > moveTime)
            {
                shouldMove = false;
            }
        }
    }

    public void Open()
    {
        shouldMove = true;
    }

}
