using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
    public Animator Animator;
    public int AmountOfBullets = 1;
    public int Damage = 1;
    public int MaxHealth = 100;
    public int CurrentHealth = 100;
    public bool IsDead = false;
    public List<GameObject> Inventory = new List<GameObject>();
    public GameObject Gun;
    public bool GunUpgrade;

    [SerializeField] private Transform _BarrelExit;
    [SerializeField] private GameObject _BulletPrefab;
    [SerializeField] private float _AngleStep = 5f;
    [SerializeField] private float _MoveSpeed = 5f;
    [SerializeField] UIManager uiManager;

    private Vector2 _moveInput;
    private Rougelite _inputActions;

    private void Awake()
    {
        _inputActions = new Rougelite();
        _inputActions.Player.Enable();
    }

    private void OnEnable()
    {
        _inputActions.Player.Fire.performed += OnFire;
        _inputActions.Player.Use.performed += OnUse;
    }

    private void OnDisable()
    {
        _inputActions.Player.Fire.performed -= OnFire;
        _inputActions.Player.Use.performed -= OnUse;
    }

    private void Update()
    {
        if (!IsDead)
        {
            _moveInput = _inputActions.Player.Movement.ReadValue<Vector2>();
        }
    }

    private void FixedUpdate()
    {
        transform.position += (Vector3)(_moveInput * _MoveSpeed * Time.fixedDeltaTime);
        if (_moveInput != new Vector2(0, 0))
        {
            Animator.SetFloat("Speed", 1);
        }
        else
        {
            Animator.SetFloat("Speed", 0);
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!IsDead)
        {
            IItem item = collision.GetComponent<IItem>();
            GameObject itemCheck = collision.gameObject;
            if (item != null && _inputActions.Player.Grab.inProgress && Inventory.Count < 3)
            {
                item.OnPickup();
            }
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (Inventory.Count > 0 && !IsDead)
        {
            GameObject itemBeingUsed = Inventory[0];
            IItem item = itemBeingUsed.GetComponent<IItem>();
            item.OnUse();
            Inventory.RemoveAt(0);
            Destroy(itemBeingUsed);
            uiManager.RemoveFromInventory();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0 && !IsDead)
        {
            for (int i = 0; i < AmountOfBullets; i++)
            {
                float offset = (i - (AmountOfBullets - 1) / 2f) * _AngleStep - 90;
                Quaternion newRotation = Gun.transform.rotation * Quaternion.Euler(0, 0, offset);
                GameObject bullet = Instantiate(_BulletPrefab, _BarrelExit.position, newRotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.Damage = Damage;
            }
        }
    }

    public void TakeDamage(int Damage)
    {
        CurrentHealth -= Damage;
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        IsDead = true;
        Animator.SetBool("Dead", true);
        Gun.SetActive(false);
        uiManager.GameOver.SetActive(true);
        Time.timeScale = 0;
    }
}
