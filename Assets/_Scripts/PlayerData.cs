using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "PlayerData", menuName = "My Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public string Name;
    public Color Color;
    public Color BG;
}
