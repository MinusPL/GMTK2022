using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TriggerType
{
    WalkAway = 0,
    JumpUp = 1,
    NextLevel = 99,
}

public class EndTrigger : MonoBehaviour
{
    public UnityEvent<int> OnPlayerEnterEndTrigger;

    public TriggerType type;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OnPlayerEnterEndTrigger.Invoke((int)type);
        }
    }
}
