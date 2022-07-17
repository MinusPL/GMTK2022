using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject diceMinigameResultsPlane;
    [SerializeField]
    private Transform diceResultsImageList;
    [SerializeField]
    private GameObject diceImagePrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    }
}
