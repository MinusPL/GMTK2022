using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum InterractionTriggerType
{
    Interraction = 1,
    Gate = 2,
    DiceMinigame = 3,
    Reserved = 99,
}

public class InterractionTrigger : MonoBehaviour
{
    public UnityEvent<int, GameObject> OnPlayerInterractionTriggered;

    public InterractionTriggerType type;

    [SerializeField]
    private GameObject obj;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OnPlayerInterractionTriggered.Invoke((int)type, obj);
        }
    }
}
