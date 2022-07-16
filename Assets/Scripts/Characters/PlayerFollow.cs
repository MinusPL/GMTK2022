using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    [SerializeField]
    private Vector3 transformOffset;

    
    private void Start()
    {
        transform.position = player.transform.position + transformOffset;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + transformOffset;
    }
}
