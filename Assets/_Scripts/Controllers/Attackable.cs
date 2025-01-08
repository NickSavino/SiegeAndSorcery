using UnityEngine;
using UnityEngine.AI;

public interface Attackable
{
    float _health { get; set; }
    int _team { get; set; }   


    void TakeDamage(float damage);

    bool IsDead();

    void SetDead();
}

public interface Attacker
{
    public void AttackTarget(MonoBehaviour scriptToAttack, float attackDistance);
}
