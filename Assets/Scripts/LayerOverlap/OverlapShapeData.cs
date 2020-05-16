using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlapShapeData :MonoBehaviour 
{

    private bool enteredScreen = false;

    public bool EnteredScreen {
        get {
            return enteredScreen;
        }
        set {
            enteredScreen = value;
        }
    }

}
