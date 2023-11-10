using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    public float currentHealth;
    public Image healthBar;

    [Header("Turret Related Stuff")]
    public Transform bulletSpawnPoint;
    public float fireRate = 0.5f;
    protected float fireTimer = 0;

    public bool isControlEnabled;
    public bool isAlive = true;

    void Start()
    {
        isControlEnabled = false;

        GetComponent<PhotonView>().RPC("InitializeHealth", RpcTarget.AllBuffered, startHealth);
    }

    void FixedUpdate()
    {
        if (isControlEnabled)
        {
            if (fireTimer < fireRate)
            {
                fireTimer += Time.deltaTime;
            }

            if (Input.GetButton("Fire1") && fireTimer > fireRate)
            {
                Fire();
                fireTimer = 0;
            }
        }
    }

    public virtual void Fire()
    {
        RaycastHit hit;

        if (Physics.Raycast(bulletSpawnPoint.position, transform.TransformDirection(Vector3.forward), out hit, 100))
        {
            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 5);
            }
        }

        CheckIfLastPlayer();
    }

    [PunRPC]
    public void InitializeHealth(float health)
    {
        this.currentHealth = health;
        this.healthBar.fillAmount = currentHealth / startHealth;
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        if (!isAlive) return;

        this.currentHealth -= damage;
        this.healthBar.fillAmount = currentHealth / startHealth;

        if (currentHealth <= 0)
        {
            isAlive = false;
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);

            StartCoroutine(DisplayKillFeed((string)info.Sender.NickName, (string)info.photonView.Owner.NickName));
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public IEnumerator DisplayKillFeed(string killer, string dead)
    {
        GameObject killFeedText = GameObject.Find("Kill Feed Text");
        killFeedText.GetComponent<Text>().text = killer + " killed " + dead;

        float killfeedTimer = 5.0f;

        while (killfeedTimer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            killfeedTimer--;
        }

        killFeedText.GetComponent<Text>().text = "";
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            GetComponent<LastManStanding>().PlayerDeath();
        }
    }

    public void CheckIfLastPlayer()
    {
        if (photonView.IsMine)
        {
            if (RacingGameManager.instance.playersAlive.Count == 1 && isAlive)
            {
                GetComponent<LastManStanding>().PlayerDeath();
            }
        }
    }
}
