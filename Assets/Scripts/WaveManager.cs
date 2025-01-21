using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<GameObject> ZombieList = new List<GameObject>();
    public int CurrentWave = 1;
    public int ZombieHealthIncrease = 0;
    public float ZombieSpeedIncrease = 0;

    [SerializeField] private GameObject _WaveClearUI;
    [SerializeField] private GameObject _ZombiePrefab;
    [SerializeField] private GameObject _SlowZombiePrefab;
    [SerializeField] private List<Transform> _ZombieSpawns = new List<Transform>();
    [SerializeField] private int _HealthIncrease;
    [SerializeField] private float _SpeedIncrease;
    [SerializeField] private int _AmountOfWavesForHealthIncrease;
    [SerializeField] private int _AmountOfWavesForSpeedIncrease;

    private bool _isSlow;
    private GameObject _newZombie;

    private void Start()
    {
        CurrentWave = 1;
        StartWave();
    }

    private void Update()
    {
        if (ZombieList.Count == 0 && CurrentWave > 1)
        {
            WaveCleared();
        }
    }

    public void StartWave()
    {
        float amountOfZombies = Mathf.Pow(CurrentWave, 2) + 3;
        if(amountOfZombies > 400)
        {
            amountOfZombies = 403;
        }
        HealthIncrease();
        SpeedIncrease();

        if (_ZombiePrefab != null)
        {
            for (int i = 0; i < amountOfZombies; i++)
            {
                int randomSpawn = Random.Range(0, _ZombieSpawns.Count);
                _isSlow = Random.value < 0.75f;
                if (_isSlow )
                {
                    _newZombie = PhotonNetwork.Instantiate(_SlowZombiePrefab.name, _ZombieSpawns[randomSpawn].position, _ZombieSpawns[randomSpawn].rotation);
                }
                else
                {
                    _newZombie = PhotonNetwork.Instantiate(_ZombiePrefab.name, _ZombieSpawns[randomSpawn].position, _ZombieSpawns[randomSpawn].rotation);
                }
                BaseEnemy zombieScript = _newZombie.GetComponent<BaseEnemy>();
                if (zombieScript != null)
                {
                    zombieScript.WaveManager = this;
                    zombieScript.CurrentHealth += ZombieHealthIncrease;
                    zombieScript.Speed += ZombieSpeedIncrease;
                }
                ZombieList.Add(_newZombie);
            }
        }
        CurrentWave++;
    }

    private void HealthIncrease()
    {
        if (CurrentWave % _AmountOfWavesForHealthIncrease == 0)
        {
            ZombieHealthIncrease += _HealthIncrease;
        }
    }

    private void SpeedIncrease()
    {
        if (CurrentWave % _AmountOfWavesForSpeedIncrease == 0)
        {
            ZombieSpeedIncrease += _SpeedIncrease;
        }
    }

    public void WaveCleared()
    {
        _WaveClearUI.SetActive(true);
    }
}
