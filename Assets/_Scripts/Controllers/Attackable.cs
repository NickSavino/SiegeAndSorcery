using UnityEngine;
using UnityEngine.AI;

public interface Attackable
{
    float _health { get; set; }


    void TakeDamage(float damage);


    bool IsDead();

    void SetDead();
}
