using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using UnityEngine.UI;

public class LastManStanding : MonoBehaviourPunCallbacks
{
    public enum RaiseEventsCode
    {
        WhoDiedEventCode = 0
    }

    public int deathOrder = 0;

    private void OnEnable()
    {
        PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
    }

    void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)RaiseEventsCode.WhoDiedEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;

            string nickNameOfDeadPlayer = (string)data[0];
            deathOrder = (int)data[1];
            int viewId = (int)data[2];

            Debug.Log(nickNameOfDeadPlayer + " " + deathOrder);

            GameObject orderUiText = RacingGameManager.instance.finisherTextUi[deathOrder - 1];
            orderUiText.SetActive(true);

            if (viewId == photonView.ViewID) // this is you
            {
                orderUiText.GetComponent<Text>().text = deathOrder + " " + nickNameOfDeadPlayer + " (YOU)";
                orderUiText.GetComponent<Text>().color = Color.red;
            }
            else
            {
                orderUiText.GetComponent<Text>().text = deathOrder + " " + nickNameOfDeadPlayer;
            }

            if (deathOrder != 1)
            {
                RacingGameManager.instance.playersAlive.Remove(nickNameOfDeadPlayer);
                StartCoroutine(DisplayEliminatedPlayer(nickNameOfDeadPlayer));
            }
        }
    }

    public GameObject camera;

    public void PlayerDeath()
    {
        GetComponent<PlayerSetup>().camera.transform.parent = null;
        GetComponent<VehicleMovement>().enabled = false;
        GetComponent<Shooting>().enabled = false;

        deathOrder = RacingGameManager.instance.playersAlive.Count;

        string nickName = photonView.Owner.NickName;
        int viewId = photonView.ViewID;

        //event data 
        object[] data = new object[] { nickName, deathOrder, viewId };

        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOption = new SendOptions
        {
            Reliability = false
        };

        PhotonNetwork.RaiseEvent((byte)RaiseEventsCode.WhoDiedEventCode, data, raiseEventOptions, sendOption);
    }

    public IEnumerator DisplayEliminatedPlayer(string dead)
    {
        GameObject eliminatedText = GameObject.Find("Eliminated Text");
        eliminatedText.GetComponent<Text>().text = dead + " was eliminated.";

        float eliminatedTimer = 3.0f;

        while (eliminatedTimer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            eliminatedTimer--;
        }

        if (RacingGameManager.instance.playersAlive.Count <= 1)
        {
            eliminatedText.GetComponent<Text>().text = RacingGameManager.instance.playersAlive[0] + " is the winner!";
        }
        else
        {
            eliminatedText.GetComponent<Text>().text = "";
        }
    }
}
