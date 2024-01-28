using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


public enum NPCTypeID
{
    BlueTeam = 1479372276,
    RedTeam = -1923039037,
}

[RequireComponent(typeof(NavMeshAgent), typeof(MeshRenderer), typeof(IAttacker))]
public class NPCController : MonoBehaviour, IDamageable
{
    [HideInInspector] public Card Card;

    [Header("Stats")]
    [SerializeField, Range(0.1f, 5f)] private float _attackRange;
    [SerializeField] private float _attackCooldown;

    [Header("Assignment")]
    [SerializeField] private LayerMask _assignable;
    [SerializeField] private Material _toBeAssignedMaterial;
    [SerializeField] private Material _purpleTeamMaterial;
    [SerializeField] private Material _redTeamMaterial;

    [HideInInspector] public NavMeshAgent Agent;

    private float _currentHealth;

    private Material _defaultMaterial;
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

        _defaultMaterial = GameManager.CurrentTurn == Turns.Player1 ? _purpleTeamMaterial : _redTeamMaterial;
        _meshRenderer.materials[^1] = _defaultMaterial;
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

        if (Input.GetMouseButtonDown(0) && GameManager.State == GameState.Turns)
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
                if (_attackTarget is null || _attackTarget is "")
                {
                    Agent.destination = Vector3.zero;
                }
                else if (_attackTarget != collidedWithTag)
                {
                    return;
                }

                Agent.destination = transform.position;
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

        Debug.Log("Will start the attack");
        await AttackCooldown(_attackCooldown / 2f);
        Debug.Log("Attacked");
        GetComponent<IAttacker>().Attack(target, Card.Info.AttackDamage);
        await AttackCooldown(_attackCooldown / 2f);
    }

    private async Task AttackCooldown(float cooldown)
    {
        _canAttack = false;

        await Task.Delay(1000 * (int)cooldown);

        _canAttack = true;
    }

    private void AssignTask()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool castedAssignRay =
            Physics.Raycast(ray.origin, ray.direction, out RaycastHit assignHit, Mathf.Infinity, _assignable)
            && !_isWaitingForAssignment;

        bool castedUnassignRay =
            Physics.Raycast(ray.origin, ray.direction, out RaycastHit unassignHit, Mathf.Infinity, _assignable)
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
           !(Physics.Raycast(ray.origin, ray.direction, out var _, Mathf.Infinity, _assignable) && _isWaitingForAssignment)
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

    public void TakeDamage(float damage, string attackTag)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            var targetPlayer = Card.Owner == Turns.Player1 ? Turns.Player2 : Turns.Player1;

            FindObjectOfType<PlayerController>().AddLightningPoints(targetPlayer, Card.Info.Cost);
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
