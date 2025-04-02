using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour
{
    public Character character;
    [CanBeNull]
    public HealthBar healthBar;
    public int damage;
    public int maxHp;
    public int currentHp;
    public float defense;
    public float maxDefense = 0.5f;

    private void Start()
    {
        if (healthBar == null) return;
        healthBar.SetMaxHealth(maxHp);
        healthBar.SetHealth(currentHp);
    }
    
    public void ApplyImprovements(Unlockable unlockable)
    {
        IncreaseDamage(unlockable.damageIncrease);
        IncreaseDefense(unlockable.defenseIncrease);
        Heal(unlockable.recoverHp);
    }
    
    public void TakeDamage(int dmg)
    {
        currentHp -= dmg - (int)(dmg * defense);
        healthBar?.SetHealth(currentHp);
        defense = 0f;
    }

    private void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp)
            currentHp = maxHp;
        healthBar?.SetHealth(currentHp);
    }
    
    public void HealPercentage(float percentage)
    {
        currentHp += (int)(maxHp * percentage);
        if (currentHp > maxHp)
            currentHp = maxHp;
        healthBar?.SetHealth(currentHp);
    }

    public bool HasDied() => currentHp <= 0;

    private void IncreaseDamage(float amount) => damage += (int)(damage * amount);

    private void IncreaseDefense(float amount)
    {
        defense += (int)(defense * amount);
        if (defense >= maxDefense)
            defense = 0.5f;
    }
}