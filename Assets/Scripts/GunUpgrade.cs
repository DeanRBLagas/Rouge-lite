using UnityEngine;

public class GunUpgrade : MonoBehaviour, IItem
{
    private UIManager _uiManager;
    private Player _player;
    private void Awake()
    {
        _uiManager = FindObjectOfType<UIManager>();
        _player = FindObjectOfType<Player>();
    }

    public void OnPickup()
    {
        if (_player.Inventory.Contains(gameObject)) return;
        gameObject.SetActive(false);
        _uiManager.AddToInventory(0);
        _player.Inventory.Add(gameObject);
    }

    public void OnUse()
    {
        _player.CurrentHealth = _player.MaxHealth;
        _player.MaxHealth += 10;
        _player.GunUpgrade = Random.value < 0.5f;
        if (_player.GunUpgrade && _player.AmountOfBullets < 5)
        {
            _player.AmountOfBullets++;
        }
        else
        {
            _player.Damage += 1;
        }
    }
}
