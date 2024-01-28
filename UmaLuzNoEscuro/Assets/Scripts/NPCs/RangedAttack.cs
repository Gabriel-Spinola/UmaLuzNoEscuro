using UnityEngine;

public class RangedAttack : MonoBehaviour, IAttacker
{
    [SerializeField] private GameObject _projectile;

    [SerializeField] private Animator _mAnimator;

    private void Start() {
        _mAnimator = GetComponent<Animator>();
    }

    public void Attack(Collider target, float damage)
    {
        _mAnimator.SetTrigger("Attack");
        var projectile = Instantiate(_projectile, transform.position, transform.rotation)
            .GetComponent<Fireball>();

        projectile.gameObject.tag = gameObject.tag;
        projectile.Damage = damage;  
        projectile.Direction = target.transform.position - transform.position;
    }
}
