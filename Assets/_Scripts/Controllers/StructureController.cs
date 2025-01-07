using UnityEngine;

public class StructureController : MonoBehaviour, Attackable
{
    [field: SerializeField] public float _health { get; set; }
    [field: SerializeField] public int _team { get; set; }     // interface property from Attackable
    StructureDamageEffect _damageEffect;     // using unit damage effect for right now

    public bool IsDead()
    {
        return _health <= 0;
    }

    public void SetDead()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void TakeDamage(float damage)
    {
        _health -= damage;
        _damageEffect.StartDamageEffect();
        if (IsDead())
        {
            Debug.Log("Dead");
            SetDead();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _damageEffect = new StructureDamageEffect(GetComponent<MeshRenderer>());
    }

    // Update is called once per frame
    void Update()
    {
        _damageEffect.UpdateTakeDamageTime();
                // Structures,  by default, just take damage
    }
}
