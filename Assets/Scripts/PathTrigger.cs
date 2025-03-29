using Dialogue;
using UnityEngine;
using UnityEngine.Serialization;

public class PathTrigger : MonoBehaviour
{
    [FormerlySerializedAs("dialogue")]
    public Dialogue.Dialogue startDialogue;

    public Dialogue.Dialogue endDialogue;

    public Scenario scenario;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInChildren<Player>() is null) return;

        Debug.Log("Player collided with the trigger.");

        var dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null)
        {
            if (startDialogue != null)
            {
                dialogueManager.StartDialogue(startDialogue);
                dialogueManager.OnDialogueEnd += () =>
                {
                    FindFirstObjectByType<BattleSystem>()
                        .SetupBattle(other.gameObject, GetComponentInChildren<Enemy>().gameObject)
                        .SetupDialogues(endDialogue);
                    Debug.Log("Dialogue ended, callback called.");
                };
            }
            else
            {
                Debug.LogWarning("No Dialogue component found on the trigger.");
            }
        }
        else
        {
            Debug.LogWarning("No DialogueManager found in the scene.");
        }
    }
}