using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BombManager : MonoBehaviourPunCallbacks
{
    public static BombManager instance = null;

    public float bombTimer;

    public bool isBombEnabled;

    public List<GameObject> playersAlive;

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
        isBombEnabled = false;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && isBombEnabled)
        {
            if (bombTimer > 0)
            {
                bombTimer -= Time.deltaTime;
            }
            else if (bombTimer <= 0)
            {
                playersAlive = new List<GameObject>(GameManager.instance.playersAlive.Values);

                if (playersAlive.Count > 1)
                {
                    foreach (GameObject player in playersAlive)
                    {
                        player.GetComponent<PhotonView>().RPC("Explode", RpcTarget.AllBuffered);
                    }

                    StartRound();
                }
            }
        }
    }

    public void StartRound()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            playersAlive = new List<GameObject>(GameManager.instance.playersAlive.Values);

            bombTimer = Random.Range(20.0f, 45.0f);

            int player = Random.Range(0, playersAlive.Count);
            playersAlive[player].GetComponent<PhotonView>().RPC("GetBomb", RpcTarget.AllBuffered);
        }
    }
}
