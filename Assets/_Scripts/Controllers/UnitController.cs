using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;
public class UnitController : MonoBehaviour, Attackable, Attacker
{

    /*
     * Serialized Fields
     */

    //[SerializeField]
    [field: SerializeField]  public float _health { get; set; }     // interface property from Attackable

    [field: SerializeField] public int _team { get; set; }     // interface property from Attackable

    [field: SerializeField] public float DESTROY_TIME_LIMIT { get; set; }     // interface property from Attackable

    [field: SerializeField] public float ATTACK_DAMAGE { get; set; }     // interface property from Attackable

    [field: SerializeField] public GameObject _healthBar { get; set; }     // interface property from Attackable

    [SerializeField]
    private Image _healthBarFill;

    public float _destroyTimer { get; set; }

    [SerializeField]
    private bool spriteFlip;

    [SerializeField]
    private float MIN_STRUCT_ATTACK_DISTANCE;

    [SerializeField]
    private float MIN_UNIT_ATTACK_DISTANCE;

    [SerializeField]
    private float ATTACKS_PER_SECOND;

    [SerializeField]
    private float ATTACK_FLASH_TIME;
    
  
    private float _maxHealth;


    /*
     *  private fields
     */
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private UnitDamageEffect _damageEffect;
    private float _currentTime;     // time delta for attacks
    [field: SerializeField] public Collider _unitCollider { get; set; }     // interface property from Attackable

    [SerializeField]
    private GameObject _destination;    // destination / structure or unit to attack

    private StructureManager _structureManager;



    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.stoppingDistance = MIN_STRUCT_ATTACK_DISTANCE / 2;    // first destination will always be structure as per UnitSpawner
        _navMeshAgent.destination = _destination.transform.position;


        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _damageEffect = new UnitDamageEffect(_spriteRenderer);

        // use transform to find child, get game object of transform, then its collider

        // transform.Find searches just for children of this game object, NOT the entire scene
        _unitCollider = transform.Find(STRUCTS_NAMES.UNIT_COLLIDER).gameObject.GetComponent<Collider>();

        _structureManager = StructureManager.GetStructureManager();

        _destroyTimer = -1f;

        _maxHealth = _health;
        _healthBar.transform.Find("Background").Find("Fill").gameObject.TryGetComponent<Image>(out _healthBarFill);   // need to understand how Nick got Image so cleanly, need to do that
        //_healthBarFill = transform.Find("Background").Find("Fill").gameObject.GetComponent<Image>();
        _healthBarFill.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDead())
        {
            UpdateDestroyTimer();
        }
        animateIfRunning();
        animateIfAttacking();
        animateDeath();
        flipSprite();       // requires destination


        animateIfRunning();
        animateIfAttacking();
        animateDeath();

        if (!IsDead())
        {
            GetNearestEnemyUnit();
            if (_destination != null)
            {
                AttackTarget();
            }
        }
        if (_destination == null)       // killed current target and no units nearby
        {
            GetNewStructureDestination();
        }
        _damageEffect.UpdateTakeDamageTime();   // take damage effect, called each frame
    }




    void animateIfRunning()
    {
        if (_navMeshAgent.velocity.magnitude > 0 && !_animator.GetBool("isAttacking"))
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
        if (_destination != null)
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
        else
        {
            _animator.SetBool("isAttacking", false);        // nothing to attack
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
        if (_destination != null)
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
                _spriteRenderer.flipX = spriteFlip == false ? false : true;
            }
        }
    }



    void GetNewStructureDestination()
    {
        if (_destination == null)   // if no units nearby and no structure selected
        {
            
            StructureController newDestination = _structureManager.FindNearestEnemyStructure(gameObject.transform.position, _team);
            if (newDestination != null)
            {
                _destination = newDestination.gameObject;
                _navMeshAgent.destination = _destination.transform.position;
                _navMeshAgent.stoppingDistance = MIN_STRUCT_ATTACK_DISTANCE / 2;
            }
        }
    }



    public void GetNearestEnemyUnit()
    {
        if (_destination != null)    
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
            if (closest != null)        // found enemy unit
            {
                _destination = closest;
                _navMeshAgent.destination = closest.transform.position;
                _navMeshAgent.stoppingDistance = MIN_UNIT_ATTACK_DISTANCE / 2;
            }

        }
    }


    public void TakeDamage(float damage)
    {
        _health -= damage;
        _damageEffect.StartDamageEffect();          // start damage effect
        UpdateHealthBar(_health);
        if (_health <= 0f)
        {
            SetDead();
        }
    }



    public void AttackTarget()
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

                if (scriptToAttack.IsDead())
                {
                    _destination = null;
                }
            }

     
        }
    }



    bool ObjectIsUnit(GameObject obj)
    {
        if (obj == null)
            return false;
        return obj.GetComponent<UnitController>() != null;
    }

    public bool IsDead()
    {
        return _health <= 0;
    }

    public void UpdateDestroyTimer()
    {
        if (_destroyTimer < 0f) // start timer
        {
            _destroyTimer = 0f;
        }
        else
        {
            _destroyTimer += Time.deltaTime;    // update timer
            if (_destroyTimer > DESTROY_TIME_LIMIT)
            {
                Destroy(gameObject);
            }
        }

    }
    public void SetDead()
    {
        GetComponent<NavMeshAgent>().enabled = false;
       // GetComponent<Collider>
    }

    public void UpdateHealthBar(float newHealth)
    {
        // if (newHealth <= 0f && _healthBar.activeSelf)
        // {
        //     _healthBar.SetActive(false); // just disable, will be destroyed with unit after
        // }
        //  else if (newHealth > 0f) {
        _healthBarFill.fillAmount = newHealth / _maxHealth;
        Debug.Log(_healthBarFill.fillAmount);
       // }
    }

}
