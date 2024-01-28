using UnityEngine;

public class MeleeAttack : MonoBehaviour, IAttacker
{
    [SerializeField] private ParticleSystem _attackParticles;
    [SerializeField] private Animator _mAnimator;

    private void Start() {
        _mAnimator = GetComponent<Animator>();
    }

    public void Attack(Collider target, float damage)
    {
        Debug.Log("Imagina funciona");
        var particle = Instantiate(_attackParticles, transform.position, Quaternion.identity);
        var main = particle.main;

        main.stopAction = ParticleSystemStopAction.Destroy;
        _mAnimator.SetTrigger("Attack");
        particle.Play();

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable?.TakeDamage(damage, gameObject.tag);
        }
    }
}
