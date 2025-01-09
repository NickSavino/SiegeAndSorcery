using UnityEngine;
using UnityEngine.AI;

public interface Attackable
{
    float _health { get; set; }
    int _team { get; set; }

    float DESTROY_TIME_LIMIT { get; set; }
    float _destroyTimer { get; set; }


    void TakeDamage(float damage);      // true if dead after taking damage, false otherwise

    bool IsDead();

    void SetDead();

    void UpdateDestroyTimer();
}

public interface Attacker
{

    Collider _unitCollider { get; set; }
    public void AttackTarget();
    public void GetNearestEnemyUnit();
}




public interface Projectile
{
    public void Travel();
    public void Pause();
    public void IsColliding();
    public void GetHeight();

}