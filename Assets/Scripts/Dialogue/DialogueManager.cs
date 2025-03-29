using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Dialogue
{
    /// <summary>
    /// Dialogue manager handles the display of dialogue lines in the game.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public delegate void DialogueEndCallback();
        private static readonly int IsOpen = Animator.StringToHash("IsOpen");

        public TextMeshProUGUI nameText;
        public TextMeshProUGUI dialogueText;
        public Image spriteRenderer;

        public Animator animator;

        public GameObject player;

        private Queue<DialogueLine> _sentences;

        public event DialogueEndCallback OnDialogueEnd;

        private void Start()
        {
            _sentences = new Queue<DialogueLine>();
        }

        public void StartDialogue(Dialogue dialogue)
        {
            player.GetComponent<Player>().CanMove(false);
            
            animator.SetBool(IsOpen, true);

            _sentences.Clear();

            foreach (var line in dialogue.lines)
                _sentences.Enqueue(line);
            
            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (_sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            var sentence = _sentences.Dequeue();

            dialogueText.text = sentence.text;
            nameText.text = sentence.character.charactrerName;
            spriteRenderer.sprite = sentence.character.sprite;

            Debug.Log(sentence.character.charactrerName + ": " + sentence.text);
        }

        public void EndDialogue()
        {
            player.GetComponent<Player>().CanMove(true);
            OnDialogueEnd?.Invoke();
            OnDialogueEnd = null;
            animator.SetBool(IsOpen, false);
        }
    }
}