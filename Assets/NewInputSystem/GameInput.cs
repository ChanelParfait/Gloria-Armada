using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : object
{
    public static PlayerActions actions;

    /*
    public enum ActionMap {
        Gameplay,
        NumberOfActionMaps
    };
    static public InputActionMap[] actionMaps;
    */
    static GameInput() {
        actions = new PlayerActions();
    }
        /*

        int numberOfActionMaps = (int)ActionMap.NumberOfActionMaps;
        actionMaps = new InputActionMap[numberOfActionMaps];
        for(int i=0; i < numberOfActionMaps; ++i) {
            actionMaps[i] = actions.FindActionMap(ActionMap.Gameplay.ToString());
        }
    }

    static InputActionMap gameplayMap = actions.FindActionMap(ActionMap.Gameplay.ToString());

    public static void SetMapState(ActionMap actionMap, bool value) {

    }*/
}
