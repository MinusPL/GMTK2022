using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public bool playerInputEnabled = true;

    private bool diceMiniGamePlayed = false;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null) Debug.LogError("GameManager does not exist!");
            return _instance;
        }
    }

    [SerializeField]
    GameObject playerObject;

    [SerializeField]
    GameObject startingPoint;

    [SerializeField]
    GameObject playerCam;
    
    [SerializeField]
    GameObject waCam;

    [SerializeField]
    GameObject minigameCam;

    private List<GameObject> waypoints;
    [SerializeField]
    private GameObject WAPath;

    [SerializeField]
    private string nextSceneName;

    //This also controls how many dices are going to be rolled!
    [SerializeField]
    private int amountOfGates = 1;

    [SerializeField]
    private UIManager uiManager;

    List<int> rolledFaces;

    //Values deciding challenge difficulty, figure out a way to use that
    private int difficulty = 0;

    //Current challenge stuff
    private GameObject challengeGate;
    private List<EnemyController> challengeEnemies;

    [SerializeField]
    private GameObject firstGate;

    //Triggers
    [Header("Triggers")]
    [SerializeField]
    private GameObject diceMiniGameTrigger;
    [SerializeField]
    private GameObject firstGateTrigger;
    [SerializeField]
    private GameObject endLevelTrigger;

    [Header("Effects")]
    [SerializeField]
    private List<string> effectStrings;

    private bool killHealFlag = false;
    private bool killDamageFlag = false;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        LoadPlayer();

        waypoints = new List<GameObject>();

        foreach (Transform child in WAPath.transform)
        {
            waypoints.Add(child.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LoadPlayer()
    {
        
        playerObject = Instantiate(Resources.Load<GameObject>("Prefabs/PC2"), startingPoint.transform.position, Quaternion.Euler(0f,90f,0f));
        var camTarget = GameObject.FindGameObjectWithTag("Camera Target");
        camTarget.GetComponent<PlayerFollow>().player = playerObject;
        playerCam = GameObject.FindGameObjectWithTag("Player Camera");
        playerCam.GetComponent<CinemachineVirtualCamera>().LookAt = playerObject.transform;
        SwitchCamToPlayer();
        diceMiniGameTrigger.GetComponent<Trigger>().EnableTrigger(true);
    }

    public void NotifyInterraction(int type, GameObject sender)
    {
        switch((TriggerType)type)
        {
            case TriggerType.Gate:
                uiManager.ShowGateMenu(rolledFaces);
                playerInputEnabled = false;
                challengeGate = sender;
                break;
            case TriggerType.DiceMinigame:
                if (diceMiniGamePlayed) break;
                sender.GetComponent<DiceGame>().StartGame(amountOfGates);
                SwitchCamToMinigame();
                playerInputEnabled = false;
                diceMiniGamePlayed = true;
                break;
            case TriggerType.WalkAway:
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetWaypoints(waypoints);
                playerInputEnabled = false;
                playerCam.SetActive(false);
                waCam.SetActive(true);
                break;
            case TriggerType.NextLevel:
                SceneManager.LoadScene(nextSceneName);
                break;
        }
        
    }

    public void EndDiceMinigame(List<int> faces)
    {
        rolledFaces = faces;
        uiManager.ShowDiceMiniGameResults(rolledFaces);
        firstGateTrigger.GetComponent<Trigger>().EnableTrigger(true);
    }

    public void ReturnToGame()
    {
        SwitchCamToPlayer();
        playerInputEnabled = true;
        challengeGate = firstGate;
    }

    private void SwitchCamToMinigame()
    {
        minigameCam.SetActive(true);
        playerCam.SetActive(false);
        waCam.SetActive(false);
    }

    private void SwitchCamToPlayer()
    {
        minigameCam.SetActive(false);
        playerCam.SetActive(true);
        waCam.SetActive(false);
    }

    public void SetChallengeDifficulty(int value)
    {
        switch(value)
        {
            case 1:
            case 2:
                difficulty = 0;
                break;
            case 3:
            case 4:
                difficulty = 1;
                break;
            case 5:
            case 6:
                difficulty = 2;
                break;
        }
        rolledFaces.Remove(value);
        uiManager.CloseGateMenu();
        playerInputEnabled = true;
        challengeGate.GetComponent<GateController>().Open();
        challengeEnemies = challengeGate.GetComponent<GateController>().challengeEnemies;
        EnableGateAI();
        if (challengeGate.GetComponent<GateController>().challengeEnemies.Count == 0) OnEnemyDie(null);
    }

    public void OnEnemyDie(EnemyController enemy)
    {
        if (killHealFlag)
            playerObject.GetComponent<LivingEntity>().HealPercentage(0.02f);
        if (killDamageFlag)
            playerObject.GetComponent<LivingEntity>().DamagePercentageWithoutKill(0.02f);

        if(challengeEnemies.Contains(enemy))
        {
            challengeEnemies.Remove(enemy);
        }

        if(challengeEnemies.Count == 0)
        {
            uiManager.DisableDiceRolling();
            //Heal based on difficulty
            float healPercentage = difficulty == 0 ? .2f : difficulty == 1 ? .1f : 0f;
            playerObject.GetComponent<LivingEntity>().HealPercentage(healPercentage);
            if(challengeGate.GetComponent<GateController>().nextGate != null)
            {
                challengeGate.GetComponent<GateController>().nextGate.GetComponent<GateController>().gateTrigger.EnableTrigger(true);
                challengeGate = challengeGate.GetComponent<GateController>().nextGate;
            }
            else
            {
                endLevelTrigger.GetComponent<Trigger>().EnableTrigger(true);
            }
        }
    }

    public void OnPlayerHealthChanged(float percentage)
    {
        uiManager.SetHealthFill(percentage);
    }

    private void EnableGateAI()
    {
        foreach (var e in challengeEnemies)
        {
            e.aiEnabled = true;
        }
        uiManager.EnableDiceRolling();
    }

    public void SetEffect(int value)
    {
        short type = 0;

        switch(difficulty)
        {
            case 0:
                if (value >= 1 && value <= 3) type = 1;
                else if (value >= 4 && value <= 5) type = 0;
                else type = 5;
                break;
            case 1:
                if (value >= 1 && value <= 2) type = 1;
                else if (value >= 3 && value <= 4) type = 0;
                else type = 5;
                break;
            case 2:
                if (value == 1) type = 1;
                else if (value >= 2 && value <= 3) type = 0;
                else type = 5;
                break;
        }


        int effect = Random.Range(1, 6);
        if(type == 0)
        {
            uiManager.SetEffectString(effectStrings[0]);
        }
        else
        {
            uiManager.SetEffectString(effectStrings[effect + type]);
        }

        //Reset previous multipliers
        playerObject.GetComponent<PlayerController>().damageMultiplier = 1f;
        foreach(var e in challengeEnemies)
        {
            e.damageMultiplier = 1f;
        }
        //Reset all flags
        killDamageFlag = false;
        killHealFlag = false;

        //Do effect
        switch (effect+type)
        {
            case 1:
                playerObject.GetComponent<PlayerController>().damageMultiplier = 2f;
                break;
            case 2:
                foreach (var e in challengeEnemies)
                {
                    e.damageMultiplier = .5f;
                }
                break;
            case 3:
                killHealFlag = true;
                break;
            case 4:
                playerObject.GetComponent<LivingEntity>().HealPercentage(0.1f);
                break;
            case 5:
                foreach (var e in challengeEnemies)
                {
                    e.gameObject.GetComponent<LivingEntity>().DamagePercentage(0.1f);
                }
                break;
            case 6:
                playerObject.GetComponent<PlayerController>().damageMultiplier = .5f;
                break;
            case 7:
                foreach (var e in challengeEnemies)
                {
                    e.damageMultiplier = 2f;
                }
                break;
            case 8:
                killDamageFlag = true;
                break;
            case 9:
                playerObject.GetComponent<LivingEntity>().DamagePercentageWithoutKill(0.1f);
                break;
            case 10:
                //LOL NOTHING
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void OnReturn()
    {
        playerInputEnabled = !uiManager.ToggleIngameMenu();
        
    }

    public void EnablePlayerInput()
    {
        playerInputEnabled = true;
    }
}
