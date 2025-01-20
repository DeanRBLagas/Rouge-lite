using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BaseEnemy : MonoBehaviour, IDamageable
{
    public float Speed;
    public WaveManager WaveManager;
    public int CurrentHealth;

    [SerializeField] protected float AttackInterval;
    [SerializeField] protected int Damage;
    protected IDamageable PlayerDamageable;
    protected bool CanAttack = false;

    [SerializeField] private List<GameObject> _ItemDrops;
    [SerializeField] private float _MinimumDistance;
    [SerializeField] private int _MinimumChance;
    [SerializeField] private int _MaximumChance;

    private Animator _animator;
    private Transform _player;

    private void Awake()
    {
        _player = FindObjectOfType<Player>().transform;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        var step = Speed * Time.deltaTime;

        Vector3 directionToPlayer = _player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer > _MinimumDistance)
        {
            Vector3 targetPosition = _player.position - directionToPlayer.normalized * _MinimumDistance;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            PlayerDamageable = player;
            CanAttack = true;
            StartCoroutine(Attack());
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        CanAttack = false;
    }

    protected virtual IEnumerator Attack()
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

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
        _animator.SetTrigger("Hit");
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        WaveManager.ZombieList.Remove(gameObject);
        Destroy(gameObject);

        int random = Random.Range(_MinimumChance, _MaximumChance);
        if (random <= 0)
        {
            int randomIndex = Random.Range(0, _ItemDrops.Count);
            GameObject randomItem = _ItemDrops[randomIndex];
            Instantiate(randomItem, transform.position, transform.rotation);
        }
    }
}
