using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI playerNameText;
    public Image healthBar;
    public GameObject turret;

    public Camera camera;

    void Start()
    {
        playerNameText.gameObject.SetActive(!photonView.IsMine);

        this.camera = transform.Find("Camera").GetComponent<Camera>();

        RacingGameManager.instance.playersAlive.Add(photonView.Owner.NickName);

        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("rc"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;

            GetComponent<Shooting>().enabled = false;
            GetComponent<LastManStanding>().enabled = false;

            healthBar.gameObject.SetActive(false);
            turret.SetActive(false);
        }
        else if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsValue("dr"))
        {
            GetComponent<VehicleMovement>().enabled = photonView.IsMine;
            GetComponent<LapController>().enabled = photonView.IsMine;
            camera.enabled = photonView.IsMine;

            GetComponent<Shooting>().enabled = photonView.IsMine;
            GetComponent<LastManStanding>().enabled = photonView.IsMine;

            healthBar.gameObject.SetActive(true);
            turret.SetActive(true);
        }

        playerNameText.text = photonView.Owner.NickName;
    }
}
