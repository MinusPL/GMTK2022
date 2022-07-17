using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRoller : MonoBehaviour
{
    [SerializeField]
    List<Sprite> sprites;

    [SerializeField]
    private Image img;

    [SerializeField]
    private float rollingTime = 0.1f;
    [SerializeField]
    private float diceRollTime = 5f;
    [SerializeField]
    private float diceRollCooldown = 10f;

    [SerializeField]
    private Image cooldownMask;
    private float cooldownMaskFill;
    private float targetCMFill;

    private float diceRollTimer = 0f;
    private float diceCooldownTimer = 0f;

    int rolledValue = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(diceRollTimer > 0f)
        {
            diceRollTimer -= Time.deltaTime;
            if(diceRollTimer <= 0f)
            {
                diceRollTimer = 0f;
                CancelInvoke("Roll");
                GameManager.Instance.SetEffect(rolledValue);
                diceCooldownTimer = diceRollCooldown;
            }
        }

        if(diceCooldownTimer > 0f)
        {
            diceCooldownTimer -= Time.deltaTime;
            if(diceCooldownTimer <= 0f)
            {
                diceCooldownTimer = 0f;
                StartRoll();
            }
        }
        cooldownMask.fillAmount =  diceCooldownTimer/ diceRollCooldown;
    }

    public void StartRoll()
    {
        diceRollTimer = diceRollTime;
        InvokeRepeating("Roll", 0f, rollingTime);
    }

    public void StopRolls()
    {
        diceRollTimer = 0f;
        diceCooldownTimer = 0f;
        CancelInvoke("Roll");
    }

    void Roll()
    {
        rolledValue = Random.Range(1, 7);
        img.sprite = sprites[rolledValue - 1];
    }
}
