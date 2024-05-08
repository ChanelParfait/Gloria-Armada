using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Speaker", menuName = "Dialogue/Speaker")]
public class Speaker : ScriptableObject {
    public string speakerName;
    public Sprite image;
    public SpeakerType type;
    public Faction faction;
}
