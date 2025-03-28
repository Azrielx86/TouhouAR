using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Character", menuName = "Scriptable Objects/Character")]
public class Character : ScriptableObject
{
    [FormerlySerializedAs("name")] public new string charactrerName;
    public Sprite sprite;
}
