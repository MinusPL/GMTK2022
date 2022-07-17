using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackVolumePlayer : MonoBehaviour
{
    public List<Enemy> enemiesInRange;

    private void Start()
    {
        enemiesInRange = new List<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<Enemy>().IsAlive())
            {
                if (!enemiesInRange.Contains(other.GetComponent<Enemy>()))
                {
                    enemiesInRange.Add(other.GetComponent<Enemy>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<Enemy>().IsAlive())
            {
                if (enemiesInRange.Contains(other.GetComponent<Enemy>()))
                {
                    enemiesInRange.Remove(other.GetComponent<Enemy>());
                }
            }
        }
    }
}
