using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private static bool canvasExists = false;

    void Awake()
    {
        if (!canvasExists)
        {
            
            DontDestroyOnLoad(gameObject);
            canvasExists = true;
        }
        else
        {
            
            Destroy(gameObject);
        }
    }


}
