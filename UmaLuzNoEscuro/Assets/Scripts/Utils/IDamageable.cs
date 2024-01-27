using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage, string attackerTag);
}

public interface IAttacker
{
    void Attack(Collider target, float damage);
}