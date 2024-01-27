using UnityEngine;

public class MeleeAttack : MonoBehaviour, IAttacker
{
    [SerializeField] private ParticleSystem _attackParticles;

    public void Attack(Collider target, float damage)
    {
        Debug.Log("Imagina funciona");
        var particle = Instantiate(_attackParticles, transform.position, Quaternion.identity);
        var main = particle.main;

        main.stopAction = ParticleSystemStopAction.Destroy;
        particle.Play();

        if (target.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable?.TakeDamage(damage, gameObject.tag);
        }
    }
}
