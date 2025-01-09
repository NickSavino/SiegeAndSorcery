using System;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TowerController : MonoBehaviour, Attacker
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject _enemy;
    private Vector3 _towerPos;
    private GameObject _towerTop;
    private GameObject _towerCannon;
    private StructureDamageEffect _damageEffect;

    [SerializeField]
    private int X_ROTATION_MAX = 16;

    [SerializeField] private int BULLET_COOLDOWN = 2;


    private float _currentTime;


    [field: SerializeField] public Collider _unitCollider { get; set; }     // interface property from Attackable


    GameObject _target;

    void Start()
    {
    //    _enemy = GameObject.Find("Player");
        _towerTop = transform.Find("TowerTop").gameObject;
        _towerCannon = transform.Find("TowerCannon").gameObject;
        _currentTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        GetNearestEnemyUnit();
        RotateTowardEnemy();

    }


    void RotateTowardEnemy()
    {
        if (_target != null)
        {
            var enemyLocation = _enemy.transform.position;
            Vector3 topRotation = (enemyLocation - _towerTop.transform.position);


            Quaternion rotation = Quaternion.LookRotation(topRotation, Vector3.up);
            rotation.x = rotation.x > X_ROTATION_MAX ? X_ROTATION_MAX : rotation.x;
            _towerTop.transform.rotation = rotation;
        }
    }


    void TryShooting()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime >= BULLET_COOLDOWN)
        {
            //   var bullet = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            // bullet.AddComponent<Rigidbody>();
            // bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            // var instantiatedBullet = GameObject.Instantiate(bullet, _towerCannon.transform.position, _towerCannon.transform.rotation);

            //  instantiatedBullet.GetComponent<Rigidbody>().AddForce((_enemy.transform.position - _towerCannon.transform.position).normalized * bulletImpulse, ForceMode.Impulse);
            //  _lastTime = Time.time;

        }
    }

    public void GetNearestEnemyUnit()
    {
        if (_target != null)
        {
            if (TryGetComponent<StructureController>(out StructureController controller))
            {

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
                        if (otherUnit._team != controller._team)   // unit belongs to different team!
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
                    _target = closest;
                }
            }

        }
    }




 


    public void AttackTarget(MonoBehaviour scriptToAttack, float attackDistance)
    {
        throw new NotImplementedException();
    }

  
}
