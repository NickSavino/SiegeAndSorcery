using System.Linq;
using UnityEngine;

public class BulletController : MonoBehaviour, Attacker, Projectile
{

    [field: SerializeField] public Collider _unitCollider { get; set; }
    [field: SerializeField] public GameObject _target { get; set; }

    
    [field: SerializeField] public float ATTACK_DAMAGE { get; set; }

    [field: SerializeField] public float VELOCITY { get; set; }

    private float _currentTime;
    [field: SerializeField] public Vector3 _directionVector { get; set; }

    private GameObject _bulletCollidedWith;

    private TowerController _towerController;

    private float MAX_TRAVEL_DISTANCE = 40; // SHOULD BE GLOBAL CONSTANT / ENUM
    private float _distanceTravelled;

    void Start()
    {
        _directionVector = _directionVector.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Travel();
    }


    public void SetTowerController(TowerController controller)
    {
        _towerController = controller;
    }



    public void AttackTarget()      // attacking only units for right now
    {
        if (_target.TryGetComponent<UnitController>(out UnitController targetScript))
        {
            targetScript.TakeDamage(ATTACK_DAMAGE);
            if (targetScript.IsDead())
            {
                _towerController.ClearTarget();
            }
            Destroy(gameObject);
        }
    }



    public void GetNearestEnemyUnit()
    {
        throw new System.NotImplementedException();
    }

    public void IsColliding()
    {
        if (_target.TryGetComponent<UnitController>(out UnitController targetScript))
        {
            targetScript.TakeDamage(ATTACK_DAMAGE);
            Destroy(gameObject);
        }
    }

    public void Pause()
    {
        throw new System.NotImplementedException();
    }

    public void Travel()
    {

        /*
         *  This collision detection needs to be optimized, but OnTriggerEnter and OnCollisionEnter are fucking useless if you 
         *  don't want to include rigidbodies...
         */


        Collider[] hitColliders = Physics.OverlapBox(transform.position ,transform.localScale);     // sphere / mesh collider


        float distance = Time.deltaTime * VELOCITY;
        _distanceTravelled += distance;

        if (_target.TryGetComponent<Collider>(out Collider hitbox))
        {
            if (hitColliders.Contains(hitbox))
            {
                AttackTarget();
            }
        }
        if (_distanceTravelled >= MAX_TRAVEL_DISTANCE)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate(_directionVector * distance);
        }
    }
}
