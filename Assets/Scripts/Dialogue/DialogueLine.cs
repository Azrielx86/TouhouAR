using UnityEngine;

namespace Dialogue
{
    [System.Serializable]
    public class DialogueLine
    {
        public Character character;
        [TextArea(3, 10)] public string text;
    }
}