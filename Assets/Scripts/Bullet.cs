using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int Damage;

    [SerializeField] private int _Speed;

    private int _durability;

    private void Start()
    {
        _durability = Damage;
        Destroy(gameObject, 4f);
    }

    private void Update()
    {
        transform.Translate(Vector3.up * _Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        BaseEnemy enemyHealth = other.GetComponent<BaseEnemy>();
        if (damageable != null)
        {
            _durability -= enemyHealth.CurrentHealth;
            damageable.TakeDamage(Damage);
            if (_durability <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
