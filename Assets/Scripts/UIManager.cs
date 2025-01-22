using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviourPunCallbacks
{
    public GameObject GameOver;

    [SerializeField] private GameObject _WaveClearUI;
    [SerializeField] private List<GameObject> _ItemList = new List<GameObject>();
    [SerializeField] private Transform _PlayerLocation;
    [SerializeField] private WaveManager _WaveManager;
    [SerializeField] private Player _Player;
    [SerializeField] private TextMeshProUGUI _PlayerHealth;
    [SerializeField] private TextMeshProUGUI _CurrentWave;
    [SerializeField] private TextMeshProUGUI _ZombiesLeft;
    [SerializeField] private List<Transform> InventorySlots = new List<Transform>();
    [SerializeField] private List<GameObject> _ItemImages = new List<GameObject>();
    [SerializeField] private List<GameObject> _CurrentItemsInInventory = new List<GameObject>();

    private delegate void GameReset();
    private event GameReset OnGameReset;
    private int playersReady = 0;

    private void Start()
    {
        OnGameReset += PlayerReset;
        OnGameReset += WaveReset;
        OnGameReset += ItemReset;
        OnGameReset += UIReset;
        OnGameReset += TimeReset;
        _Player = FindObjectOfType<Player>();
        _PlayerLocation = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        PlayerHealth();
        CurrentWave();
    }

    public void ChooseReward(int item)
    {
        PhotonNetwork.Instantiate(_ItemList[item].name, _PlayerLocation.position, _PlayerLocation.rotation);
        photonView.RPC("PlayerMadeChoice", RpcTarget.MasterClient);
        //ChoiceMade();
    }

    public void ChoiceMade()
    {
        _WaveClearUI.SetActive(false);
        _WaveManager.StartWave();
    }

    public void PlayerHealth()
    {
        _PlayerHealth.text = "Health: " + _Player.CurrentHealth.ToString();
        if (_Player.CurrentHealth <= 0)
        {
            _PlayerHealth.text = "Health: 0";
        }
    }

    public void CurrentWave()
    {
        int actualWave = _WaveManager.CurrentWave - 1;
        _CurrentWave.text = "Wave: " + actualWave.ToString();
        _ZombiesLeft.text = "Zombies left: " + _WaveManager.ZombieList.Count;
    }

    public void AddToInventory(int number)
    {
        GameObject newItem = Instantiate(_ItemImages[number], InventorySlots[_CurrentItemsInInventory.Count]);
        _CurrentItemsInInventory.Add(newItem);
    }

    public void RemoveFromInventory()
    {
        Destroy(_CurrentItemsInInventory[0]);
        _CurrentItemsInInventory.RemoveAt(0);

        foreach (GameObject item in _CurrentItemsInInventory)
        {
            RectTransform picture = item.GetComponent<RectTransform>();
            picture.anchoredPosition = new Vector2(picture.anchoredPosition.x - 100, picture.anchoredPosition.y);
        }
    }

    public void PlayerReset()
    {
        _PlayerLocation.position = Vector3.zero;
        _Player.MaxHealth = 100;
        _Player.CurrentHealth = 100;
        _Player.Damage = 1;
        _Player.AmountOfBullets = 1;
        _Player.IsDead = false;
        _Player.Animator.SetBool("Dead", false);
        _Player.Gun.SetActive(true);
        _Player.Inventory.Clear();
    }

    public void WaveReset()
    {
        _WaveManager.CurrentWave = 1;
        _WaveManager.ZombieHealthIncrease = 0;
        _WaveManager.ZombieSpeedIncrease = 0;
        _WaveManager.ZombieList.Clear();
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        _WaveManager.StartWave();
    }

    public void ItemReset()
    {
        GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
        foreach (GameObject item in items)
        {
            Destroy(item);
        }
    }

    public void UIReset()
    {
        GameOver.SetActive(false);
        _WaveClearUI.SetActive(false);
        for (int i = 0; i < _CurrentItemsInInventory.Count; i++)
        {
            Destroy(_CurrentItemsInInventory[0]);
            _CurrentItemsInInventory.RemoveAt(0);
        }
    }

    public void TimeReset()
    {
        Time.timeScale = 1f;
    }

    public void Reset()
    {
        OnGameReset?.Invoke();
    }

    [PunRPC]
    public void PlayerMadeChoice()
    {
        _WaveClearUI.SetActive(false);
        playersReady++;
        if (playersReady == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            photonView.RPC("StartNextWave", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartNextWave()
    {
        _WaveClearUI.SetActive(false);
        _WaveManager.StartWave();
        playersReady = 0;
    }
}
