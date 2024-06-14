using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    private bool hasKey = false;

    // Property to get or set the value of hasKey
    public bool HasKey
    {
        get { return hasKey; }
        set { hasKey = value; }
    }
}