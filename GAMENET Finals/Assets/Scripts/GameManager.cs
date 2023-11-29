using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject[] playerPrefabs;
    public Transform[] spawnPoints;
    public GameObject[] leaderboardUI;

    public static GameManager instance = null;

    public Text startTimerText;

    public Dictionary<string, GameObject> playersAlive = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            Vector3 instantiatePosition = spawnPoints[actorNumber - 1].position;
            PhotonNetwork.Instantiate(playerPrefabs[actorNumber - 1].name, instantiatePosition, Quaternion.identity);
        }

        foreach (GameObject go in leaderboardUI)
        {
            go.SetActive(false);
        }
    }
}
