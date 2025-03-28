using UnityEngine;

namespace Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
    public class Dialogue : ScriptableObject
    {
        public DialogueLine[] lines;
    }
}
