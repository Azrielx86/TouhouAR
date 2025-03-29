using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour
{
    public Character character;
    public int damage;
    public int maxHp;
    public int currentHp;
    public float defense;
    public float maxDefense;

    public void TakeDamage(int dmg)
    {
        currentHp -= dmg - (int)(dmg * defense);
        defense = 0f;
    }

    public void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp)
            currentHp = maxHp;
    }

    public bool HasDied() => currentHp <= 0;

    public void IncreaseDamage(int amount) => damage += amount;

    public void SetDefense(float value) => defense = value;
}