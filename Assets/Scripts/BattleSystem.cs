using System.Collections;
using Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum BattleState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}

public class BattleSystem : MonoBehaviour
{
    [Header("Battle state")]
    public BattleState state;

    [Header("UI Elements")]
    public Animator combatUIAnimator;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI actionText;
    public Image spriteRenderer;

    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");

    private GameObject _player;
    private GameObject _enemy;
    private Animator _playerAnimator;
    private Animator _enemyAnimator;
    private Entity _playerEntity;
    private Entity _enemyEntity;

    private Dialogue.Dialogue _wonDialogue;
    public Dialogue.Dialogue lostDialogue;
    
    public BattleSystem SetupBattle(GameObject player, GameObject enemy)
    {
        state = BattleState.Start;
        _player = player;
        _enemy = enemy;

        _playerEntity = player.GetComponent<Entity>();
        _enemyEntity = enemy.GetComponent<Entity>();
        
        _playerAnimator = player.GetComponent<Animator>();
        _enemyAnimator = enemy.GetComponent<Animator>();

        Debug.Log($"Battle started! Player HP: {_playerEntity.currentHp}, Enemy HP: {_enemyEntity.currentHp}");
        combatUIAnimator.SetBool(IsOpen, true);

        StartBattle();
        return this;
    }

    public BattleSystem SetupDialogues(Dialogue.Dialogue wonDialogue)
    {
        _wonDialogue = wonDialogue;
        return this;
    }
    
    public void StartBattle()
    {
        state = BattleState.PlayerTurn;
        PlayerTurn();
    }

    private void PlayerTurn()
    {
        nameText.text = _playerEntity.character.name;
        actionText.text = "¡Elige tu acción!";
    }

    private void EnemyTurn()
    {
        nameText.text = _enemyEntity.character.name;
        actionText.text = $"¡{_enemyEntity.character.name} Ataca!";
        StartCoroutine(EnemyAttack());
    }

    private IEnumerator PlayerAttack()
    {
        actionText.text = $"¡{_playerEntity.character.name} ataca!";
        
        _playerAnimator.SetBool(IsAttacking, true);
        yield return new WaitForSeconds(2f);
        _playerAnimator.SetBool(IsAttacking, false);

        // Calculate damage
        _enemyEntity.TakeDamage(_playerEntity.damage);

        if (_enemyEntity.HasDied())
        {
            state = BattleState.Won;
            EndBattle();
            yield break;
        }

        state = BattleState.EnemyTurn;
        EnemyTurn();
        yield return null;
    }

    private IEnumerator EnemyAttack()
    {
        _enemyAnimator.SetBool(IsAttacking, true);
        yield return new WaitForSeconds(2f);
        _enemyAnimator.SetBool(IsAttacking, false);

        _playerEntity.TakeDamage(_enemyEntity.damage);

        if (_playerEntity.HasDied())
        {
            state = BattleState.Lost;
            EndBattle();
            yield break;
        }

        state = BattleState.PlayerTurn;
        PlayerTurn();
        yield return null;
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn) return;
        StartCoroutine(PlayerAttack());
    }

    public void OnDefenseButton()
    {
        if (state != BattleState.PlayerTurn) return;
        // StartCoroutine(PlayerAttack());
    }

    private void EndBattle()
    {
        combatUIAnimator.SetBool(IsOpen, false);
        if (state == BattleState.Won)
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue(_wonDialogue);
            var unlockable = _enemy.GetComponent<Unlockable>();
            if (unlockable != null)
            {
                _player.GetComponent<Player>().UnlockScenario(unlockable.scenario);
                _playerEntity.ApplyImprovements(unlockable);
                Destroy(_enemy);
                Debug.Log($"{unlockable.scenario} unlocked!");
            }
            else
            {
                Debug.LogWarning("No Unlockable component found on the enemy.");
            }
            Debug.Log("Player won the battle!");
        }
        else
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue(lostDialogue);
            Debug.Log("Player lost the battle!");
        }
    }
}