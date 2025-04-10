using System.Collections;
using Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    [FormerlySerializedAs("attackEffect")]
    [Header("Effects")]
    public GameObject playerAttackEffect;

    public GameObject enemyAttackEffect;
    public GameObject healEffect;

    private static readonly int IsOpen = Animator.StringToHash("IsOpen");
    private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
    private static readonly int IsHealing = Animator.StringToHash("IsHealing");
    private static readonly int IsDead = Animator.StringToHash("IsDead");

    private GameObject _player;
    private GameObject _enemy;
    private Animator _playerAnimator;
    private Animator _enemyAnimator;
    private Entity _playerEntity;
    private Entity _enemyEntity;

    private Dialogue.Dialogue _wonDialogue;
    public Dialogue.Dialogue lostDialogue;

    private bool _isTurnEventInProgress = false;

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

    private IEnumerator PlayerHeal()
    {
        _isTurnEventInProgress = true;
        _playerAnimator.SetBool(IsHealing, true);
        yield return new WaitForSeconds(_playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        _playerAnimator.SetBool(IsHealing, false);
        _isTurnEventInProgress = false;

        _playerEntity.HealPercentage(0.15f);

        var effect = Instantiate(healEffect, _player.transform.position, Quaternion.identity);
        effect.transform.SetParent(_player.transform);
        var particles = effect.GetComponent<ParticleSystem>();
        Destroy(effect, particles.main.duration);

        state = BattleState.EnemyTurn;
        EnemyTurn();
        yield return null;
    }

    private IEnumerator PlayerAttack()
    {
        actionText.text = $"¡{_playerEntity.character.name} ataca!";

        _isTurnEventInProgress = true;
        _playerAnimator.SetBool(IsAttacking, true);
        yield return new WaitForSeconds(_playerAnimator.GetCurrentAnimatorStateInfo(0).length);
        _playerAnimator.SetBool(IsAttacking, false);
        _isTurnEventInProgress = false;

        // Calculate damage
        _enemyEntity.TakeDamage(_playerEntity.damage);
        // Show attack effect
        var effect = Instantiate(playerAttackEffect, _enemy.transform.position, Quaternion.identity);
        effect.transform.SetParent(_enemy.transform);
        var particles = effect.GetComponent<ParticleSystem>();
        Destroy(effect, particles.main.duration);


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
        _isTurnEventInProgress = true;
        _enemyAnimator.SetBool(IsAttacking, true);
        yield return new WaitForSeconds(_enemyAnimator.GetCurrentAnimatorStateInfo(0).length);
        _enemyAnimator.SetBool(IsAttacking, false);
        _isTurnEventInProgress = false;

        _playerEntity.TakeDamage(_enemyEntity.damage);

        var effect = Instantiate(enemyAttackEffect, _player.transform.position, Quaternion.identity);
        effect.transform.SetParent(_player.transform);
        var particles = effect.GetComponent<ParticleSystem>();
        Destroy(effect, particles.main.duration);

        if (_playerEntity.HasDied())
        {
            state = BattleState.Lost;
            _playerAnimator.SetBool(IsDead, true);
            EndBattle();
            yield break;
        }

        state = BattleState.PlayerTurn;
        PlayerTurn();
        yield return null;
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PlayerTurn || _isTurnEventInProgress) return;
        StartCoroutine(PlayerAttack());
    }

    public void OnDefenseButton()
    {
        if (state != BattleState.PlayerTurn || _isTurnEventInProgress) return;
        StartCoroutine(PlayerHeal());
    }

    private void EndBattle()
    {
        combatUIAnimator.SetBool(IsOpen, false);
        if (state == BattleState.Won)
        {
            FindFirstObjectByType<DialogueManager>().StartDialogue(_wonDialogue);
            var unlockable = _enemy.GetComponent<Unlockable>();
            if (unlockable is not null)
            {
                _player.GetComponent<Player>().UnlockScenario(unlockable.scenario);
                _playerEntity.ApplyImprovements(unlockable);
                Debug.Log($"{unlockable.scenario} unlocked!");
            }
            else
            {
                Debug.LogWarning("No Unlockable component found on the enemy.");
            }

            Destroy(_enemy);

            Debug.Log("Player won the battle!");
        }
        else
        {
            var dialogueManager = FindFirstObjectByType<DialogueManager>();
            dialogueManager.OnDialogueEnd += () => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); };
            dialogueManager.StartDialogue(lostDialogue);
            Debug.Log("Player lost the battle!");
        }
    }
}