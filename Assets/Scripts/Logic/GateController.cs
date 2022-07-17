using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GateType
{
    Move = 1,
    Rotate = 2,
    Drop = 3
}

public class GateController : MonoBehaviour
{
    [SerializeField]
    private GameObject mover;

    [SerializeField]
    private GameObject waypoint;
    [SerializeField]
    private GameObject gateOrigin;

    [SerializeField]
    public Trigger gateTrigger;

    [SerializeField]
    private float moveTime = 2f;
    private float moveTimer = 0;

    private bool shouldMove = false;
    private bool shouldRotate = false;

    private Vector3 gateOriginalRotation;
    [SerializeField]
    private Vector3 gateFinalRotation;

    [SerializeField]
    public GameObject nextGate;

    [SerializeField]
    public List<EnemyController> challengeEnemies;

    [SerializeField]
    private GateType type;

    // Start is called before the first frame update
    void Start()
    {
        gateOriginalRotation = transform.localRotation.eulerAngles;
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

        if(shouldRotate)
        {
            moveTimer += Time.deltaTime;
            mover.transform.rotation = Quaternion.Euler(Vector3.Lerp(gateOriginalRotation, gateOriginalRotation + gateFinalRotation, moveTimer / moveTime));
        }
    }

    public void Open()
    {
        switch(type)
        {
            case GateType.Move: shouldMove = true; break;
            case GateType.Rotate: shouldRotate = true; break;
            case GateType.Drop: mover.GetComponent<Rigidbody>().isKinematic = false; break;
        }
    }

}
