using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class TowerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private GameObject _enemy;
    private Vector3 _towerPos;
    private GameObject _towerTop;
    private GameObject _towerCannon;

    [SerializeField]
    private int xRotationMax = 16;

    [SerializeField] private int bulletCooldown = 2;

    private float _lastTime = 0f;
    
    public float bulletImpulse = 100f;
    
    public float bulletLifeTime = 5f;
    
    void Start()
    {
        _enemy = GameObject.Find("Player");
        _towerTop = GameObject.Find("TowerTop");
        _towerCannon = GameObject.Find("TowerCannon");
        _lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        var enemyLocation = _enemy.transform.position;

        Vector3 topRotation = (enemyLocation - _towerTop.transform.position);

        
        Quaternion rotation = Quaternion.LookRotation(topRotation, Vector3.up);
        rotation.x = rotation.x > xRotationMax ? xRotationMax : rotation.x;
        _towerTop.transform.rotation = rotation;

        if (Time.time - _lastTime > bulletCooldown)
        {
            var bullet = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            bullet.AddComponent<Rigidbody>();
            bullet.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            
            var instantiatedBullet = GameObject.Instantiate(bullet, _towerCannon.transform.position, _towerCannon.transform.rotation);
        
            instantiatedBullet.GetComponent<Rigidbody>().AddForce((_enemy.transform.position - _towerCannon.transform.position).normalized * bulletImpulse, ForceMode.Impulse);
            _lastTime = Time.time;
            
            Destroy(instantiatedBullet, bulletLifeTime);

        }
        
      
    }
}
