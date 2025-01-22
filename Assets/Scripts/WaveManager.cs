using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviourPunCallbacks
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

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartWave();
        }
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && ZombieList.Count == 0 && CurrentWave > 1)
        {
            WaveCleared();
        }
    }

    public void StartWave()
    {
        float amountOfZombies = Mathf.Pow(CurrentWave, 2) + 3;
        if (amountOfZombies > 400)
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

                GameObject newZombie;
                if (_isSlow)
                {
                    newZombie = PhotonNetwork.InstantiateRoomObject(
                        _SlowZombiePrefab.name,
                        _ZombieSpawns[randomSpawn].position,
                        _ZombieSpawns[randomSpawn].rotation);
                }
                else
                {
                    newZombie = PhotonNetwork.InstantiateRoomObject(
                        _ZombiePrefab.name,
                        _ZombieSpawns[randomSpawn].position,
                        _ZombieSpawns[randomSpawn].rotation);
                }

                BaseEnemy zombieScript = newZombie.GetComponent<BaseEnemy>();
                if (zombieScript != null)
                {
                    zombieScript.WaveManager = this;
                    zombieScript.CurrentHealth += ZombieHealthIncrease;
                    zombieScript.Speed += ZombieSpeedIncrease;
                }
                ZombieList.Add(newZombie);
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
        photonView.RPC("NotifyWaveCleared", RpcTarget.All);
    }

    [PunRPC]
    public void NotifyWaveCleared()
    {
        _WaveClearUI.SetActive(true);
    }
}
