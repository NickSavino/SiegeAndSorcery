using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StructureController : MonoBehaviour, Attackable
{
    [field: SerializeField] public float _health { get; set; }
    [field: SerializeField] public int _team { get; set; }     // interface property from Attackable

    [field: SerializeField] public float DESTROY_TIME_LIMIT { get; set; }     // interface property from Attackable

    public float _destroyTimer { get; set; }
    [field: SerializeField] public GameObject _healthBar { get; set; }

    private float _maxHealth;           // need to set this in scriptable object

    StructureDamageEffect _damageEffect;     // using unit damage effect for right now


    private StructureManager _structureManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer[] childMeshes = GetComponentsInChildren<MeshRenderer>();

        if (TryGetComponent<MeshRenderer>(out MeshRenderer mesh))
        {
            _damageEffect = new StructureDamageEffect(mesh, childMeshes);
        }
        else
        {
            _damageEffect = new StructureDamageEffect(null, childMeshes);
        }
        _structureManager = StructureManager.GetStructureManager();
        _destroyTimer = -1f;
        _maxHealth = _health;
    }

    // Update is called once per frame
    void Update()
    {
        _damageEffect.UpdateTakeDamageTime();

        if (IsDead())
        {
            UpdateDestroyTimer();
        }
                // Structures,  by default, just take damage
    }


    public bool IsDead()
    {
        return _health <= 0;
    }

    public void SetDead()
    { 
        _structureManager.RemoveStructure(this);        // remove myself from the structure manager
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;
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

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _damageEffect.StartDamageEffect();
        if (IsDead())
        {
            SetDead();
        }
    }

    public void UpdateHealthBar(float newHealth)
    {
        //_healthBarFill.fillAmount = newHealth / _maxHealth;
        throw new NotImplementedException();
    }
}
