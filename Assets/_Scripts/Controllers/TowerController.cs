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


    [field: SerializeField] public SphereCollider _unitCollider { get; set; }     // interface property from Attackable
    [field: SerializeField] public float ATTACK_DAMAGE { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    [SerializeField]
    private GameObject _target;

    [SerializeField]
    GameObject BULLET_PREFAB;

    public bool isPlaced { get; set; }    // don't let it shoot until its placed!

    void Start()
    {
        _towerTop = transform.Find("TowerTop").gameObject;
        _towerCannon = _towerTop.transform.Find("TowerCannon").gameObject;
        _bulletSpawnPoint = _towerTop.transform.Find("BulletSpawnPoint").gameObject;
        _currentTime = 0f;

        SphereCollider temp;
        transform.Find(STRUCTS_NAMES.UNIT_COLLIDER).gameObject.TryGetComponent<SphereCollider>(out temp);
        _unitCollider = temp;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaced)
        {
            Debug.Log("WOPRKING");
            GetNearestEnemyUnit();
            RotateTowardEnemy();
            TryShooting();
        }

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
        if (_target != null && ShotIsClear())
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

                Collider[] hitColliders = Physics.OverlapSphere(_unitCollider.transform.position, _unitCollider.radius);

                float closestDistance = 0f;
                GameObject closest = null;

                foreach (Collider thatCollider in hitColliders)
                {
                    GameObject otherObject = thatCollider.gameObject;

                    // don't count self

                    if (otherObject.TryGetComponent<UnitController>(out UnitController otherUnit))          // colliding agent is unit
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

    /*
     * Doesn't fire if friendly structure is in the way
     */
    private bool ShotIsClear()
    {

        Vector3 origin = _bulletSpawnPoint.transform.position;
        Vector3 distanceVector = _target.transform.position - _bulletSpawnPoint.transform.position;

        RaycastHit[] hits = Physics.RaycastAll(origin, distanceVector.normalized);


        foreach (RaycastHit hit in hits)        // iterate over hit colliders
        {

            if (hit.collider.gameObject.TryGetComponent<StructureController>(out StructureController structure))    // see if we are hitting a structure
            {
                StructureController tower;
                TryGetComponent<StructureController>(out tower);
                if (structure._team == tower._team && hit.distance < distanceVector.magnitude)  // if it is a friendly structure and is in front of target
                {
                    return false; // no clear shot!
                }
            }
        }   // clear shot
        return true;
    }

  
}
