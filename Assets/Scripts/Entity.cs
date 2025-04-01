using UnityEngine;
using UnityEngine.Serialization;

public class Entity : MonoBehaviour
{
    public Character character;
    public HealthBar healthBar;
    public int damage;
    public int maxHp;
    public int currentHp;
    public float defense;
    public float maxDefense = 0.5f;

    private void Start()
    {
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
        defense = 0f;
    }

    private void Heal(int amount)
    {
        currentHp += amount;
        if (currentHp > maxHp)
            currentHp = maxHp;
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