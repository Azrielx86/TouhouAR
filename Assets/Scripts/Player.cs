using System.Collections;
using UnityEngine;
using Vuforia;

public class Player : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");

    [Header("Stats")]
    public float health = 100;

    public float defense = 0;
    public float attack = 10;

    [Header("Max stats")]
    public float maxHealth = 100;

    public float maxDefense = 100;
    public float maxAttack = 100;

    [Header("Player objects")]
    public GameObject model;

    public Animator animator;

    [Header("Movement")]
    public float speed = 1.0f;

    private bool _isMoving = false;
    private bool _canMove;

    public void CanMove(bool canMove)
    {
        _canMove = canMove;
    }

    public void MoveToTarget(ObserverBehaviour target)
    {
        if (!_canMove)
            return;

        if (!_isMoving)
            StartCoroutine(MoveModelToTarget(target));
    }

    private IEnumerator MoveModelToTarget(ObserverBehaviour target)
    {
        animator.SetBool(IsRunning, true);
        _isMoving = true;

        if (target is null)
        {
            animator.SetBool(IsRunning, false);
            _isMoving = false;
            yield break;
        }

        var startPosition = model.transform.position;
        var endPosition = target.transform.position;
        model.transform.LookAt(new Vector3(endPosition.x, model.transform.position.y, endPosition.z));

        float journey = 0;

        while (journey <= 1f)
        {
            journey += Time.deltaTime * speed;
            model.transform.position = Vector3.Lerp(startPosition, endPosition, journey);
            yield return null;
        }

        transform.parent = target.transform;
        animator.SetBool(IsRunning, false);
        _isMoving = false;
    }
}