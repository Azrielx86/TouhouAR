using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Serialization;
using Vuforia;

[RequireComponent(typeof(Entity))]
public class Player : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("IsRunning");

    public Entity entity;

    [Header("Player objects")]
    public GameObject model;

    public Animator animator;

    [Header("Movement")]
    public float speed = 1.0f;

    private bool _isMoving = false;
    private bool _canMove;

    [SerializeField]
    private List<Scenario> scenarios = new() { Scenario.PathA, Scenario.PathB };

    public void UnlockScenario(Scenario scenario) => scenarios.Add(scenario);

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

        if (target is null || !scenarios.Contains(target.GetComponent<PathTrigger>().scenario))
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

        var parent = transform.parent;
        transform.parent = target.transform;
        Destroy(parent.gameObject);
        animator.SetBool(IsRunning, false);
        _isMoving = false;
    }
}