using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask _whatIsAssignable;
    [SerializeField] private ParticleSystem _explosionParticles;

    [Header("Stats")]
    [SerializeField, Range(.1f, 5f)] private float _explosionRadius;
    [SerializeField] private float _projectileSpeed;

    [HideInInspector] public float Damage;
    [HideInInspector] public Vector3 Direction;

    private void Start()
    {
        Debug.Log("fireball tag: " + gameObject.tag);
    }

    private void Update()
    {
        transform.Translate(Direction * _projectileSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Fireball from " + gameObject.tag);
        Collider[] explosionCollisions = Physics.OverlapSphere(
            collision.transform.position,
            _explosionRadius,
            _whatIsAssignable
        );

        for (int i = 0; i < explosionCollisions.Length; i++)
        {
            string collidedWithTag = explosionCollisions[i].tag;

            if (GameTagsFields.AllTags.Contains(collidedWithTag) && collidedWithTag != transform.tag)
            {
                var particle = Instantiate(_explosionParticles, transform.position, Quaternion.identity);
                var main = particle.main;

                main.stopAction = ParticleSystemStopAction.Destroy;
                particle.Play();

                explosionCollisions[i].GetComponent<IDamageable>()?.TakeDamage(Damage, gameObject.tag);
                Destroy(gameObject);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
