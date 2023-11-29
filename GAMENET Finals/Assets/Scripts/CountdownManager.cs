using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CountdownManager : MonoBehaviourPunCallbacks
{
    public Text timerText;

    public float timeToStart = 5.0f;

    void Start()
    {
        timerText = GameManager.instance.startTimerText;
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (timeToStart > 0)
            {
                timeToStart -= Time.deltaTime;
                photonView.RPC("SetTime", RpcTarget.AllBuffered, timeToStart);
            }
            else if (timeToStart < 0)
            {
                photonView.RPC("StartRace", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    public void SetTime(float time)
    {
        if (time > 0)
        {
            timerText.text = time.ToString("F1");
        }
        else
        {
            timerText.text = "";
        }
    }

    [PunRPC]
    public void StartRace()
    {
        GetComponent<PlayerController>().isControlEnabled = true;
        BombManager.instance.isBombEnabled = true;

        this.enabled = false;
    }
}
