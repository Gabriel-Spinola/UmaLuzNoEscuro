
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NeutralObjective : MonoBehaviour, IDamageable
{
    [SerializeField] private LayerMask _assignable;
    [SerializeField] private GameObject _projectile;

    [SerializeField, Range(.1f, 15f)] private float _attackRange;
    [SerializeField] private float _totalHealth;
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackCooldown;
    private Animator _mAnimator;
    private float _currentHealth;
    private bool _canAttack = true;

    private void Start()
    {
        _currentHealth = _totalHealth;
        _mAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (GameManager.State is not GameState.Simulating)
        {
            return;
        }

        Collider[] collisions = Physics.OverlapSphere(transform.position, _attackRange, _assignable);

        for (int i = 0; i < collisions.Length; i++)
        {
            // NOTE - Breaks projectile
            // transform.LookAt(collisions[0].transform);

            if (collisions[i].tag != transform.tag)
            {
                Attack(collisions[i]);
            }
        }
    }

    private async void Attack(Collider target)
    {
        if (!_canAttack)
        {
            return;
        }

        _mAnimator.SetTrigger("Attack");
        var enemyDirection = target.transform.position - transform.position;
        var projectile = Instantiate(_projectile, transform.position, transform.rotation)
            .GetComponent<Fireball>();

        projectile.tag = gameObject.tag;
        projectile.Damage = _attackDamage;
        projectile.Direction = enemyDirection;

        await AttackCooldown();
    }

    private async Task AttackCooldown()
    {
        _canAttack = false;

        await Task.Delay(1000 * (int)_attackCooldown);

        _canAttack = true;
    }

    public void TakeDamage(float damage, string attackerTag)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            Die(); 
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDestroy() {
        _mAnimator.SetTrigger("Die");
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }
}
