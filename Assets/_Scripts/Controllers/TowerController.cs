using System;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TowerController : MonoBehaviour, Attacker
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject _towerTop;
    private GameObject _towerCannon;
    private GameObject _bulletSpawnPoint;

    [SerializeField]
    private int X_ROTATION_MAX = 16;

    [SerializeField] private float BULLET_COOLDOWN = 0.5f;


    private float _currentTime;


    [field: SerializeField] public Collider _unitCollider { get; set; }     // interface property from Attackable
    [field: SerializeField] public float ATTACK_DAMAGE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    GameObject BULLET_PREFAB;

    void Start()
    {
        _towerTop = transform.Find("TowerTop").gameObject;
        _towerCannon = _towerTop.transform.Find("TowerCannon").gameObject;
        _bulletSpawnPoint = _towerTop.transform.Find("BulletSpawnPoint").gameObject;
        _currentTime = 0f;

        _unitCollider = transform.Find(STRUCTS_NAMES.UNIT_COLLIDER).gameObject.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

        GetNearestEnemyUnit();
        RotateTowardEnemy();
        TryShooting();

    }




    void RotateTowardEnemy()
    {
        if (_target != null)
        {
            var enemyLocation = _target.transform.position;
            Vector3 topRotation = (enemyLocation - _towerTop.transform.position);


            Quaternion rotation = Quaternion.LookRotation(topRotation, Vector3.up);
            rotation.x = rotation.x > X_ROTATION_MAX ? X_ROTATION_MAX : rotation.x;
            _towerTop.transform.rotation = rotation;
        }
    }


    void TryShooting()
    {
        if (_target != null)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime >= BULLET_COOLDOWN)
            {
                _currentTime = 0f;
                GameObject bullet = Instantiate(BULLET_PREFAB);
                bullet.transform.position = _bulletSpawnPoint.transform.position;
                Vector3 directionVector = (_target.transform.position - bullet.transform.position).normalized;
                if (bullet.TryGetComponent<BulletController>(out BulletController bulletController))
                {
                    bulletController._directionVector = directionVector;
                    bulletController._target = _target;
                    bulletController.SetTowerController(this);
                }
            }
        }
        else
        {
            if (_currentTime != 0f)
            {
                _currentTime = 0f;          // reset current time if no current target
            }
        }
    }

    public void GetNearestEnemyUnit()
    {
        if (_target == null)        // need to be finding new target
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


    public void ClearTarget()
    {
        _target = null;
    }

 


    public void AttackTarget()
    {
        throw new NotImplementedException();
    }

  
}
