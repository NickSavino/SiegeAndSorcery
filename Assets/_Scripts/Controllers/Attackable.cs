using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public interface Attackable
{

    GameObject _healthBar { get; set; }
    float _health { get; set; }
    int _team { get; set; }

    float DESTROY_TIME_LIMIT { get; set; }
    float _destroyTimer { get; set; }


    void TakeDamage(float damage);      // true if dead after taking damage, false otherwise

    bool IsDead();

    void SetDead();

    void SetAlive();

    void UpdateDestroyTimer();


    void UpdateHealthBar(float health);
}

public interface Attacker
{

    Collider _unitCollider { get; set; }

    float ATTACK_DAMAGE { get; set; }
    public void AttackTarget();
    public void GetNearestEnemyUnit();
}




public interface Projectile
{

    GameObject _target { get; set; }
    Vector3 _directionVector { get; set; }

    float VELOCITY { get; set; }
    public void Travel();
    public void Pause();
    public void IsColliding();


}