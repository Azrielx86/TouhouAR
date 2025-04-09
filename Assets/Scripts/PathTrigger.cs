using Dialogue;
using UnityEngine;
using UnityEngine.Serialization;

public class PathTrigger : MonoBehaviour
{
    [FormerlySerializedAs("dialogue")]
    public Dialogue.Dialogue startDialogue;

    public Dialogue.Dialogue endDialogue;

    public Scenario scenario;

    public GameObject prefab;
    
    public HealthBar enemyHealthBar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>() is null) return;

        Debug.Log("Player collided with the trigger.");

        var dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager != null)
        {
            if (startDialogue != null)
            {
                dialogueManager.StartDialogue(startDialogue);

                // Instanciar el objeto prefab
                var obj = Instantiate(prefab, transform.position, Quaternion.identity);
                var objEntity = obj.GetComponent<Entity>();
                objEntity.healthBar = enemyHealthBar;
                enemyHealthBar.SetHealth(objEntity.currentHp);
                enemyHealthBar.SetMaxHealth(objEntity.maxHp);
                
                obj.transform.parent = transform;
                
                obj.transform.position = new Vector3(transform.position.x - 1.5f, transform.position.y, transform.position.z);
                other.gameObject.transform.LookAt(new Vector3(obj.transform.position.x, other.gameObject.transform.position.y, obj.transform.position.z));
                obj.transform.LookAt(new Vector3(other.gameObject.transform.position.x, obj.transform.position.y, other.gameObject.transform.position.z));
                
                // Eliminar el canvas
                var canvas = GetComponentInChildren<Canvas>();
                if (canvas != null)
                    Destroy(canvas.gameObject);
                
                
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