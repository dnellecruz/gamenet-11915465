using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerNameText;

    public Camera camera;

    void Start()
    {
        playerNameText.text = photonView.Owner.NickName;
        playerNameText.gameObject.SetActive(!photonView.IsMine);

        this.camera = transform.Find("Camera").GetComponent<Camera>();

        GameManager.instance.playersAlive.Add(photonView.Owner.NickName, gameObject);

        GetComponent<PlayerController>().enabled = photonView.IsMine;
        GetComponent<LastManStanding>().enabled = photonView.IsMine;

        camera.enabled = photonView.IsMine;
    }
}
