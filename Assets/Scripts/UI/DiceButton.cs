using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceButton : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    List<Sprite> sprites;

    private int diceValue;

    private void Start()
    {
        button.onClick.AddListener(OnDiceButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImage(int i)
    {
        button.image.sprite = sprites[i - 1];
        diceValue = i;
    }

    public void OnDiceButtonClicked()
    {
        GameManager.Instance.SetChallengeDifficulty(diceValue);
    }
}
