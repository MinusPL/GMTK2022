using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [SerializeField]
    protected float maxHealth;
    [SerializeField]
    protected float health;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsAlive()
    {
        return health > 0f;
    }
    public void Damage(float amount)
    {
        health -= amount;
        if(health <= 0f)
        {
            health = 0f;
            Die();
        }
        OnHealthChanged();
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth;
        OnHealthChanged();
    }

    public void HealPercentage(float percentage)
    {
        float newHealth = maxHealth * percentage;
        health = newHealth + health > maxHealth ? maxHealth : newHealth + health;
        OnHealthChanged();
    }

    public void DamagePercentageWithoutKill(float percentage)
    {
        float newHealth = maxHealth * percentage;
        health -= newHealth;
        if (health < 0) health = 1;
        OnHealthChanged();
    }

    public void DamagePercentage(float percentage)
    {
        float newHealth = maxHealth * percentage;
        health -= newHealth;
        if (health <= 0f)
        {
            health = 0f;
            Die();
        }
        OnHealthChanged();
    }

    public virtual void OnHealthChanged()
    {

    }

    public virtual void Die()
    {
        //DIE

        Destroy(gameObject);
    }
}
