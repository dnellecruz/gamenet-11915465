using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float speed = 20;
    public float rotationSpeed = 100;
    public float currentSpeed = 0;

    public bool isControlEnabled;
    public bool isBombWithPlayer;

    public GameObject bomb;
    public GameObject player;

    public List<GameObject> playersInRange = new List<GameObject>();

    void Start()
    {
        isControlEnabled = false;
        bomb.SetActive(false);
    }

    void LateUpdate()
    {
        if (isControlEnabled)
        {
            float verticalMovement = Input.GetAxis("Vertical") * speed * Time.deltaTime;
            float horizontalMovement = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
            float rotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            transform.Translate(horizontalMovement, 0, verticalMovement);
            currentSpeed = verticalMovement;

            transform.Rotate(0, rotation, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isBombWithPlayer)
                {
                    PassBomb();
                }
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playersInRange.Add(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            playersInRange.Remove(collider.gameObject);
        }
    }

    void PassBomb()
    {
        if (playersInRange.Count > 0)
        {
            photonView.RPC("DeactivateBomb", RpcTarget.AllBuffered);
            playersInRange[0].gameObject.GetComponent<PhotonView>().RPC("GetBomb", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public void GetBomb()
    {
        bomb.SetActive(true);
        isBombWithPlayer = true;
        speed = 30;
    }

    [PunRPC]
    public void DeactivateBomb()
    {
        bomb.SetActive(false);
        isBombWithPlayer = false;
        speed = 20;
    }

    [PunRPC]
    public void Explode()
    {
        if (isBombWithPlayer)
        {
            player.SetActive(false);
            isBombWithPlayer = false;
            GetComponent<LastManStanding>().PlayerDeath();
        }
    }
}
