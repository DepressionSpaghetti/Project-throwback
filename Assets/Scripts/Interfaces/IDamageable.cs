using UnityEngine;


public struct DamageInfo
{
    public float ammount;
    public Vector3 hitPoint;
    public Vector3 hitDirection;
    public GameObject attacker;
}


public interface IDamageable
{
    float CurrentHealth { get;}
    bool IsDead { get;}


    void TakeDamage(DamageInfo info);

    void Die();
}
