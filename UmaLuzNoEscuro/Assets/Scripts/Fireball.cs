using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask _whatIsAssignable;

    [Header("Stats")]
    [SerializeField, Range(.1f, 5f)] private float _explosionRadius;
    [SerializeField] private float _projectileSpeed;

    [HideInInspector] public float Damage;
    [HideInInspector] public Vector3 Direction;

    private void Update()
    {
        transform.Translate(Direction * _projectileSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collided with, " + collision.gameObject.name);
        Collider[] explosionCollisions = Physics.OverlapSphere(
            collision.transform.position,
            _explosionRadius,
            _whatIsAssignable
        );

        for (int i = 0; i < explosionCollisions.Length; i++)
        {
            Debug.Log(explosionCollisions.Length);
            string collidedWithTag = explosionCollisions[i].tag;

            if (GameTagsFields.AllTags.Contains(collidedWithTag) && collidedWithTag != transform.tag)
            {
                explosionCollisions[i].GetComponent<IDamageable>()?.TakeDamage(Damage);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
