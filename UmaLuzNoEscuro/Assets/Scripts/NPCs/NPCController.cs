using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


public enum NPCTypeID
{
    BlueTeam = 1479372276,
    RedTeam = -1923039037,
}

[RequireComponent(typeof(NavMeshAgent), typeof(MeshRenderer))]
public class NPCController : MonoBehaviour, IDamageable
{
    [HideInInspector] public Card Card;

    [Header("Stats")]
    [SerializeField, Range(0.1f, 3f)] private float _attackRange;
    [SerializeField] private float _attackCooldown;

    [Header("Assignment")]
    [SerializeField] private GameObject _fireball;
    [SerializeField] private LayerMask _assignable;
    [SerializeField] private Material _toBeAssignedMaterial;
    [SerializeField] private Material _defaultMaterial;

    [HideInInspector] public NavMeshAgent Agent;

    private float _currentHealth;

    private MeshRenderer _meshRenderer;
    private string _attackTarget;

    private bool _isWaitingForAssignment = false;

    private bool _canAttack = true;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();

        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        _currentHealth = Card.Info.Health;
    }

    private void Update()
    {
        if (Card.Owner != GameManager.CurrentTurn)
        {
            return;
        }

        _meshRenderer.material = _isWaitingForAssignment
          ? _toBeAssignedMaterial
          : _defaultMaterial;
    }

    private void FixedUpdate()
    {
        if (Card.Owner != GameManager.CurrentTurn)
        {
            _isWaitingForAssignment = false;

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            AssignTask();
        }

        if (GameManager.State != GameState.Simulating)
        {
            return;
        }

        Collider[] collision = Physics.OverlapSphere(transform.position, _attackRange, _assignable);

        for (int i = 0; i < collision.Length; i++)
        {
            if (collision[i].GetInstanceID() == transform.GetInstanceID())
            {
                continue;
            }

            string collidedWithTag = collision[i].tag;

            if (GameTagsFields.AllTags.Contains(collidedWithTag) && collidedWithTag != transform.tag)
            {
                Attack(collision[i]);
            }
        }
    }

    private async void Attack(Collider target)
    {
        if (!_canAttack)
        {
            return;
        }

        target.GetComponent<IDamageable>()?.TakeDamage(Card.Info.AttackDamage);
        await AttackCooldown();
    }

    private async Task AttackCooldown()
    {
        _canAttack = false;

        await Task.Delay(1000 * (int)_attackCooldown);

        _canAttack = true;
    }

    private void AssignTask()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool castedAssignRay =
            Physics.Raycast(ray.origin, ray.direction, out RaycastHit assignHit, _assignable)
            && !_isWaitingForAssignment;

        bool castedUnassignRay =
            Physics.Raycast(ray.origin, ray.direction, out RaycastHit unassignHit, _assignable)
            && _isWaitingForAssignment;

        if (castedAssignRay)
        {
            if (assignHit.transform.GetInstanceID() == transform.GetInstanceID())
            {
                _isWaitingForAssignment = true;

                return;
            }
        }

        // NOTE - If one npc is already selected
        if (castedUnassignRay)
        {
            // Unassign npc when clicked twice
            if (unassignHit.transform.GetInstanceID() == transform.GetInstanceID())
            {
                _isWaitingForAssignment = false;

                return;
            }

            // If clicked on another npc, also assign him to do some action for the next selection
            if (unassignHit.transform.tag == transform.tag)
            {
                return;
            }
        }

        if (
           !(Physics.Raycast(ray.origin, ray.direction, out var _, _assignable) && _isWaitingForAssignment)
        )
        {
            return;
        }

        if (GameTagsFields.AllTags.Contains(assignHit.transform.tag) && assignHit.transform.tag != transform.tag)
        {
            _attackTarget = assignHit.transform.tag;
        }

        Agent.destination = assignHit.point;
        _isWaitingForAssignment = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRange);
    }

    public void TakeDamage(float damage)
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
}
