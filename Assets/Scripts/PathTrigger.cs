using Dialogue;
using UnityEngine;

public class PathTrigger : MonoBehaviour
{
    public Dialogue.Dialogue dialogue;
        
    private void OnTriggerEnter(Collider other)
    {
        // if (!other.CompareTag("Player")) return;
        
        if (other.gameObject.GetComponentInChildren<Player>() is null) return;
        
        Debug.Log("Player collided with the trigger.");
        
        GetComponentInChildren<MobControl>().OnEventTriggered();
        
        var dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null)
        {
            if (dialogue != null)
            {
                dialogueManager.StartDialogue(dialogue);
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
