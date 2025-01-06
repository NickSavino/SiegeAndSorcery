using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using UnityEditor.Animations;

public class UnitController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private Rigidbody _rigidBody;
    private NavMeshAgent _navMeshAgent;
    private Camera _camera;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private bool spriteFlip;

    [SerializeField]
    private float MIN_ATTACK_DISTANCE;

    public Vector3 _destination;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
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




    void flipSprite()
    {

        Vector3 spriteVector3 = _destination - transform.position;
        Vector2 spriteVector = new Vector2(spriteVector3.z, spriteVector3.x);

        Vector3 camVector3 = transform.position - _camera.transform.position;
        Vector2 camVector = new Vector2(camVector3.z, camVector3.x);

        float dotprod = Vector2.Dot(camVector, spriteVector);
        float denom = spriteVector.magnitude * camVector.magnitude;
        float theta = Mathf.Acos(dotprod / denom);
  
        if (theta > Mathf.PI / 2 && !_spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
        else if (theta < Mathf.PI / 2 && _spriteRenderer.flipX)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
    }
}
