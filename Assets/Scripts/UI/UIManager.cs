using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [Header("Dice Minigame Stuff")]
    [SerializeField]
    private GameObject diceMinigameResultsPlane;
    [SerializeField]
    private Transform diceResultsImageList;
    [SerializeField]
    private GameObject diceImagePrefab;
    [Header("Challenege Difficulty Stuff")]
    [SerializeField]
    private GameObject challengeDifficultyPlane;
    [SerializeField]
    private Transform diceButtonList;
    [SerializeField]
    private GameObject diceButtonPrefab;

    [Header("Player Health")]
    [SerializeField]
    private Image playerHealth;

    [SerializeField]
    private DiceRoller roller;

    [SerializeField]
    private TextMeshProUGUI effectText;

    private float targetPlayerHealth = 1f;
    float refPlayerHealth;

    [SerializeField]
    private GameObject ingameMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerHealth.fillAmount = Mathf.SmoothDamp(playerHealth.fillAmount, targetPlayerHealth, ref refPlayerHealth, 0.2f);
    }

    public void ShowDiceMiniGameResults(List<int> faces)
    {
        foreach (var i in faces)
        {
            var di = Instantiate(diceImagePrefab);
            di.GetComponent<DiceImageSetter>().SetImage(i);
            di.transform.SetParent(diceResultsImageList);
        }
        diceMinigameResultsPlane.SetActive(true);
    }

    public void ShowGateMenu(List<int> faces)
    {
        //populate buttons
        foreach (var i in faces)
        {
            var di = Instantiate(diceButtonPrefab);
            di.GetComponent<DiceButton>().SetImage(i);
            di.transform.SetParent(diceButtonList);
        }
        challengeDifficultyPlane.SetActive(true);
    }

    public void CloseGateMenu()
    {
        challengeDifficultyPlane.SetActive(false);
        foreach (Transform c in diceButtonList)
        {
            Destroy(c.gameObject);
        }
    }

    public void SetHealthFill(float fill)
    {
        targetPlayerHealth = fill;
    }

    public void EnableDiceRolling()
    {
        roller.StartRoll();
    }

    public void DisableDiceRolling()
    {
        roller.StopRolls();
        effectText.text = "No Active Effect";
    }

    public void SetEffectString(string effect)
    {
        effectText.text = effect;
    }

    public bool ToggleIngameMenu()
    {
        ingameMenu.SetActive(!ingameMenu.activeSelf);
        return ingameMenu.activeSelf;
    }



}
