using UnityEngine;
using UnityEngine.AI;

public interface Attackable
{
    float _health { get; set; }
    int _team { get; set; }

    float DESTROY_TIME_LIMIT { get; set; }
    float _destroyTimer { get; set; }


    bool TakeDamage(float damage);      // true if dead after taking damage, false otherwise

    bool IsDead();

    void SetDead();

    void UpdateDestroyTimer();
}

public interface Attacker
{
    public void AttackTarget(MonoBehaviour scriptToAttack, float attackDistance);
}
