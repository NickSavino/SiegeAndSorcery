using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEditor.Animations;

public class UnitController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private NavMeshAgent _navMeshAgent;
    private Camera _camera;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private int _health = 100;

    [SerializeField]
    private bool spriteFlip;

    [SerializeField]
    private float MIN_STRUCT_ATTACK_DISTANCE;

    [SerializeField]
    private float MIN_UNIT_ATTACK_DISTANCE;

    [SerializeField]
    private int _team;


    private Collider _unitCollider;
    public GameObject _destination;

    void Start()
    {

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        // use transform to find child, get game object of transform, then its collider

        // transform.Find searches just for children of this game object, NOT the entire scene
        _unitCollider = transform.Find(STRUCTS_NAMES.UNIT_COLLIDER).gameObject.GetComponent<Collider>();
  
    }

    // Update is called once per frame
    void Update()
    {
        if (_destination != null)
        {
            _navMeshAgent.SetDestination(_destination.transform.position);
        }

        animateIfRunning();
        animateIfAttacking();
        animateDeath();
        flipSprite();
        GetNearEnemyUnits();
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

    void animateDeath()
    {
        if (_health <= 0)
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



    void GetNearEnemyUnits()
    {
        int count = 0;
        Collider[] hitColliders = Physics.OverlapBox(_unitCollider.transform.position, _unitCollider.transform.localScale);
            
        foreach (Collider thatCollider in hitColliders)
        {
            GameObject otherObject = thatCollider.gameObject;
            UnitController otherUnit = otherObject.GetComponent<UnitController>();

            // don't count self

            if (otherUnit != null)          // colliding agent is unit
            {
 
                if (otherUnit._team != this._team)   // unit belongs to different team!
                {
                    ++count;
                }
            }
        }
        Debug.Log(count);
        //return false;
    }

}
