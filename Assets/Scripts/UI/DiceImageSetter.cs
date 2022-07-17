using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class DiceImageSetter : MonoBehaviour
{
    [SerializeField]
    Image img;
    [SerializeField]
    List<Sprite> sprites;

    public void SetImage(int i)
    {
        img.sprite = sprites[i-1];
    }
}
