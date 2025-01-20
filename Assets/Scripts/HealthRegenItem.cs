using UnityEngine;

public class HealthRegenItem : MonoBehaviour, IItem
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
        _uiManager.AddToInventory(1);
        gameObject.SetActive(false);
        _player.Inventory.Add(gameObject);
    }

    public void OnUse()
    {
        _player.CurrentHealth = _player.MaxHealth;
    }
}
