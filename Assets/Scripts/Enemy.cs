using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Entity _entity;

    [Header("Enemy objects")]
    public GameObject model;

    public Animator animator;
}
