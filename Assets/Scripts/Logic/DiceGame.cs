using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DiceGame : MonoBehaviour
{
    private int diceCount = 1;
    [SerializeField]
    private GameObject dicePrefab;
    [SerializeField]
    private GameObject diceSpawn;

    [SerializeField]
    private float diceSpawnTime = 0.2f;

    private List<Dice> dice;

    private bool gameRunning = false;

    [SerializeField]
    private Transform tableDownDirection;

    [SerializeField]
    private AudioSource diceRolling;

    int spawnedDice = 0;
    // Start is called before the first frame update
    void Start()
    {
        dice = new List<Dice>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGame(int count)
    {
        diceCount = count;
        gameRunning = true;
        InvokeRepeating("SpawnDice", 0f, diceSpawnTime);
        diceRolling.Play();
    }

    private void SpawnDice()
    {
        var d = Instantiate(dicePrefab, diceSpawn.transform.position, Quaternion.identity);
        d.GetComponent<Dice>().StartShuffling();
        dice.Add(d.GetComponent<Dice>());
        spawnedDice++;
        if (spawnedDice >= diceCount)
            CancelInvoke("SpawnDice");
    }

    public void OnShuffle(InputValue value)
    {
        if (gameRunning)
        {
            foreach (var d in dice)
            {
                d.StopShuffle();
            }
            gameRunning = false;
            diceRolling.Stop();
            Invoke("GameResults", 2f);

        }
    }

    private void GameResults()
    {
        
        List<int> faces = new List<int>();
        foreach(var d in dice)
        {
            faces.Add(d.CheckTopFace(tableDownDirection.forward));
        }
        GameManager.Instance.EndDiceMinigame(faces);
    }
}
