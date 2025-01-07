using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEditor.Animations;
using NUnit.Framework.Interfaces;

public class UnitController : MonoBehaviour, Attackable
{

    /*
     * Serialized Fields
     */

    //[SerializeField]
    [field: SerializeField]  public float _health { get; set; }     // interface property from Attackable

    [field: SerializeField] public int _team { get; set; }     // interface property from Attackable

    [SerializeField]
    private bool spriteFlip;

    [SerializeField]
    private float MIN_STRUCT_ATTACK_DISTANCE;

    [SerializeField]
    private float MIN_UNIT_ATTACK_DISTANCE;

    [SerializeField]
    private float ATTACKS_PER_SECOND;

    [SerializeField]
    private float ATTACK_DAMAGE;

    [SerializeField]
    private float ATTACK_FLASH_TIME;





    /*
     *  private fields
     */
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private UnitDamageEffect _damageEffect;
    private float _currentTime;     // time delta for attacks
    private Collider _unitCollider; // will be this unit's unit collider child object
    private GameObject _destination;    // destination / structure or unit to attack



    void Start()
    {

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _damageEffect = new UnitDamageEffect(_spriteRenderer);

        // use transform to find child, get game object of transform, then its collider

        // transform.Find searches just for children of this game object, NOT the entire scene
        _unitCollider = transform.Find(STRUCTS_NAMES.UNIT_COLLIDER).gameObject.GetComponent<Collider>();
  
    }

    // Update is called once per frame
    void Update()
    {
        if (_destination != null && _navMeshAgent.enabled)
        {
            _navMeshAgent.SetDestination(_destination.transform.position);
        }

        animateIfRunning();
        animateIfAttacking();
        animateDeath();
        flipSprite();

        if (!IsDead())
        {
            GetNearestEnemyUnit();
            AttackTarget();
        }
        _damageEffect.UpdateTakeDamageTime();   // take damage effect, called each frame
    }




    void animateIfRunning()
    {
        if (_navMeshAgent.velocity.magnitude > 0)
        {
            _animator.SetBool("isRunning", true);

        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
    }

    void animateIfAttacking()
    {
        {


            Vector3 diff = transform.position - _destination.transform.position;

            float distanceMetric = _destination.GetComponent<UnitController>() != null ? MIN_UNIT_ATTACK_DISTANCE : MIN_STRUCT_ATTACK_DISTANCE;

            diff.y = 0f;
            if (diff.magnitude <= distanceMetric)
            {
                _animator.SetBool("isAttacking", true);

            }
            else
            {
                _animator.SetBool("isAttacking", false);
            }
        }
    }

    void animateDeath()
    {
        if (IsDead())
        {
            _animator.SetBool("isAttacking", false);
            _animator.SetBool("isRunning", false);
            _animator.SetBool("isDead", true);
        }
        else
        {
            _animator.SetBool("isDead", false);
        }
    }

    public void SetDestination(GameObject destination)
    {
        this._destination = destination;
    }


    void flipSprite()
    {
        Vector3 distance = _destination.transform.position - Camera.main.transform.position;
        Vector3 fromCamera = Camera.main.transform.forward;
        float check = Vector3.SignedAngle(distance, fromCamera, Vector3.up);

        Vector3 distanceToSprite = transform.position - Camera.main.transform.position;
        float spriteCheck = Vector3.SignedAngle(distanceToSprite, fromCamera, Vector3.up);

        if (spriteCheck > check)       // object left of sprite
        {
            _spriteRenderer.flipX = spriteFlip == false ? true : false;               // true = left
        }
        else            // object right of sprite
        {
            _spriteRenderer.flipX = spriteFlip == false ? false: true;
        }
    }



    void GetNearestEnemyUnit()
    {
        int count = 0;
        Collider[] hitColliders = Physics.OverlapBox(_unitCollider.transform.position, _unitCollider.transform.localScale);

        float closestDistance = 0f;
        GameObject closest = null;
            
        foreach (Collider thatCollider in hitColliders)
        {
            GameObject otherObject = thatCollider.gameObject;
            UnitController otherUnit = otherObject.GetComponent<UnitController>();

            // don't count self

            if (otherUnit != null)          // colliding agent is unit
            {
                if (otherUnit._team != this._team)   // unit belongs to different team!
                {
                    float distance = (otherObject.transform.position - transform.position).magnitude;
                    if ((distance < closestDistance || closest == null) && !otherUnit.IsDead())
                    {
                        closest = otherObject;
                        closestDistance = distance;
                    }
                }
            }
        }
        if (closest != null)
        {
            _destination = closest;
            if (ObjectIsUnit(_destination))            // have to set stopping stopping distance for navmesh agent
            {
                _navMeshAgent.stoppingDistance = MIN_UNIT_ATTACK_DISTANCE;
            }
            else    // is structure
            {
                _navMeshAgent.stoppingDistance = MIN_STRUCT_ATTACK_DISTANCE;
            }
        }
    }


    public void TakeDamage(float damage)
    {
        _health -= damage;
        _damageEffect.StartDamageEffect();          // start damage effect
        if (_health <= 0f)
        {
            SetDead();
        }
    }



    private void AttackTarget()
    {
        float distanceVector = (_destination.transform.position - transform.position).magnitude;
        float attackDistance;
        Attackable scriptToAttack;
        if (ObjectIsUnit(_destination))
        {
            attackDistance = MIN_UNIT_ATTACK_DISTANCE;
            scriptToAttack = _destination.GetComponent<UnitController>();
        }
        else
        {
            attackDistance = MIN_STRUCT_ATTACK_DISTANCE;
            scriptToAttack = _destination.GetComponent<StructureController>();
        }

        if (distanceVector <= attackDistance)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= ATTACKS_PER_SECOND)
            {
                _currentTime = 0;
                scriptToAttack.TakeDamage(ATTACK_DAMAGE);
            }
        }
    }



    bool ObjectIsUnit(GameObject obj)
    {
        return obj.GetComponent<UnitController>() != null;
    }

    public bool IsDead()
    {
        return _health <= 0;
    }


    public void SetDead()
    {
        GetComponent<NavMeshAgent>().enabled = false;
       // GetComponent<Collider>
    }
}
