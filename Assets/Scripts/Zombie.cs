using System.Collections;
using UnityEngine;

public class Zombie : BaseEnemy
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerDamageable = collision.GetComponent<IDamageable>();
            CanAttack = true;
            StartCoroutine(Attack());
        }
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        CanAttack = false;
    }

    protected override IEnumerator Attack()
    {
        if (CanAttack)
        {
            if (PlayerDamageable != null)
            {
                PlayerDamageable.TakeDamage(Damage);
            }
            yield return new WaitForSeconds(AttackInterval);
            StartCoroutine(Attack());
        }
    }
}
