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
            Die();
        }
        Debug.Log($"Current Health: {health}");
    }

    public void Heal(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth;
        Debug.Log($"Current Health: {health}");
    }

    public virtual void Die()
    {
        //DIE

        Destroy(gameObject);
    }
}
