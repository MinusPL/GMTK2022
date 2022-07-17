using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : LivingEntity
{
    [SerializeField]
    TextMeshProUGUI text;
    // Start is called before the first frame update
    private Animator animator;

    [SerializeField]
    private float timeToDespawn = 10f;
    private float despawnTimer = 0f;

    private PlayerController player;
    private EnemyController cntrl;

    void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
        animator.SetBool("Dead", false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cntrl = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"{(health / maxHealth) * 100f} %";

        if(despawnTimer > 0f)
        {
            despawnTimer -= Time.deltaTime;
            if(despawnTimer <= 0f)
            {
                despawnTimer = 0f;
                Destroy(gameObject);
            }
        }
    }

    public override void Die()
    {
        animator.SetBool("Dead", true);
        despawnTimer = timeToDespawn;
        player.CheckDeadEnemy(this);
        cntrl.Death();
    }
}
