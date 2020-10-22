using UnityEngine;

interface IDamageable
{
    void TakeDamage(float damage);
}

interface IHealable
{
    void RestoreHealth(float health);
}