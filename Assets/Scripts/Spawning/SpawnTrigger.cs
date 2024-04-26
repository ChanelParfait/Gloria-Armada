using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnPointName {Top_Left, Top_Right, Right_Top, Right_Bottom, Bottom_Right, Bottom_Left, Left_Bottom, Left_Top}
public class SpawnTrigger : MonoBehaviour
{

    public SpawnPointName spawnPointName;
    public int enemyIndex;
    public int spawnAmount;

    public int spawnInterval;

}
