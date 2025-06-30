using UnityEngine;

public interface IDamageable : IHitable
{
    void TakeDamage(Vector2 damageSource, int damageAmount, float knockbackThrust);
}