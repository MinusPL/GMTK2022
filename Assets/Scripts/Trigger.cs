using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TriggerType
{
    Interraction = 1,
    Gate = 2,
    DiceMinigame = 3,
    WalkAway = 4,
    NextLevel = 5,
    Reserved = 99,
}

public class Trigger : MonoBehaviour
{
    public UnityEvent<int, GameObject> OnPlayerInterractionTriggered;

    public TriggerType type;

    [SerializeField]
    private GameObject obj;

    private bool triggered = false;

    [SerializeField]
    private bool active = false;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && !triggered && active)
        {
            OnPlayerInterractionTriggered.Invoke((int)type, obj);
            triggered = true;
        }
    }

    public void EnableTrigger(bool flag)
    {
        active = flag;
    }
}
