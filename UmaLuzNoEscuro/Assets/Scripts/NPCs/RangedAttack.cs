using UnityEngine;

public class RangedAttack : MonoBehaviour, IAttacker
{
    [SerializeField] private GameObject _projectile;

    public void Attack(Collider target, float damage)
    {
        var projectile = Instantiate(_projectile, transform.position, transform.rotation)
            .GetComponent<Fireball>();
            
        projectile.Damage = damage;  
        projectile.Direction = target.transform.position - transform.position;
    }
}
