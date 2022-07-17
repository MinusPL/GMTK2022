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
        playerObject = Resources.Load<GameObject>("Prefabs/PC2");
        var playerObj = Instantiate(playerObject, startingPoint.transform.position, Quaternion.Euler(0f,90f,0f));
        var camTarget = GameObject.FindGameObjectWithTag("Camera Target");
        camTarget.GetComponent<PlayerFollow>().player = playerObj;
        playerCam = GameObject.FindGameObjectWithTag("Player Camera");
        playerCam.GetComponent<CinemachineVirtualCamera>().LookAt = playerObj.transform;
        SwitchCamToPlayer();
    }

    public void EndLevel(int type)
    {
        playerCam.SetActive(false);
        waCam.SetActive(true);
        switch((TriggerType)type)
        {
            case TriggerType.WalkAway:
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetWaypoints(waypoints);
                playerInputEnabled = false;
                break;
            case TriggerType.NextLevel:
                SceneManager.LoadScene(nextSceneName);
                break;
            default:
                Debug.Log("Ooopsie, this trigger is not supported!");
                break;
        }
    }

    public void NotifyInterraction(int type, GameObject sender)
    {
        switch((InterractionTriggerType)type)
        {
            case InterractionTriggerType.Gate:
                uiManager
                //sender.GetComponent<GateController>().Open();
                break;
            case InterractionTriggerType.DiceMinigame:
                if (diceMiniGamePlayed) break;
                sender.GetComponent<DiceGame>().StartGame(amountOfGates);
                SwitchCamToMinigame();
                playerInputEnabled = false;
                diceMiniGamePlayed = true;
                break;
        }
        
    }

    public void EndDiceMinigame(List<int> faces)
    {
        uiManager.ShowDiceMiniGameResults(faces);
    }

    public void ReturnToGame()
    {
        SwitchCamToPlayer();
        playerInputEnabled = true;
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

    }
}
