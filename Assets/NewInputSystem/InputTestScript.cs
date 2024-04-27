using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTestScript : MonoBehaviour
{
    void Update() {
        Vector3 relativeMovement = PlayerInput.Move();
        transform.Translate(relativeMovement);
    }
}
