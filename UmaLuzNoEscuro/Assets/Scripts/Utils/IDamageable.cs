using UnityEngine;

public interface IDamageable
{
    void TakeDamage(float damage);
}

public interface IAttacker
{
    void Attack(Collider target, float damage);
}