using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : LivingEntity
{
    private void Start()
    {
        health = maxHealth;
    }
    public override void OnHealthChanged()
    {
        GameManager.Instance.OnPlayerHealthChanged(health / maxHealth);
    }

    public override void Die()
    {
        Debug.Log("YOU DIED");
    }
}
