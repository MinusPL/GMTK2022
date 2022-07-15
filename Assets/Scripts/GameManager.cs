using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    public bool playerInputEnabled = true;

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

    private List<GameObject> waypoints;
    [SerializeField]
    private GameObject WAPath;

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
        playerObject = Resources.Load<GameObject>("Prefabs/Player");
        var playerObj = Instantiate(playerObject, startingPoint.transform.position, Quaternion.identity);
        var camTarget = GameObject.FindGameObjectWithTag("Camera Target");
        camTarget.GetComponent<PlayerFollow>().player = playerObj;
        GameObject.FindGameObjectWithTag("Player Camera").GetComponent<CinemachineVirtualCamera>().LookAt = playerObj.transform;
    }

    public void EndLevel(int type)
    {
        playerCam.SetActive(false);
        waCam.SetActive(true);
        if (type == 0)
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().SetWaypoints(waypoints);
            playerInputEnabled = false;
        }
    }
}
