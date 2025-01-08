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
    private float MIN_ATTACK_DISTANCE;

    public Vector3 _destination;

    void Start()
    {

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _camera = Camera.main;
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
  
    }

    // Update is called once per frame
    void Update()
    {
        if (_destination != null)
        {
            _navMeshAgent.SetDestination(_destination);
        }
        Debug.Log(_navMeshAgent.velocity.magnitude);
        animateIfRunning();
        animateIfAttacking();
        animateDeath();
        flipSprite();
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
        Vector3 diff = transform.position - _destination;
        diff.y = 0f;
        if (diff.magnitude <= MIN_ATTACK_DISTANCE)
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
     

        Vector3 distance = _destination - Camera.main.transform.position;
        Vector3 fromCamera = Camera.main.transform.forward;
        float check = Vector3.SignedAngle(distance, fromCamera, Vector3.up);

        Vector3 distanceToSprite = transform.position - Camera.main.transform.position;
        float spriteCheck = Vector3.SignedAngle(distanceToSprite, fromCamera, Vector3.up);
        Debug.Log(check);
        Debug.Log(spriteCheck);


        if (spriteCheck > check)       // object left of sprite
        {
            _spriteRenderer.flipX = spriteFlip == false ? true : false;               // true = left
        }
        else            // object right of sprite
        {
            _spriteRenderer.flipX = spriteFlip == false ? false: true;
        }




    }
}
