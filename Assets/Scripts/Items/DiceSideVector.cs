using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSideVector : MonoBehaviour
{
    public int sideValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        switch(sideValue)
        {
            case 1:
                Gizmos.color = Color.red;
                break;
            case 2:
                Gizmos.color = Color.green;
                break;
            case 3:
                Gizmos.color = Color.blue;
                break;
            case 4:
                Gizmos.color = Color.yellow;
                break;
            case 5:
                Gizmos.color = Color.cyan;
                break;
            case 6:
                Gizmos.color = Color.magenta;
                break;
        }
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
