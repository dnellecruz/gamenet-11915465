using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera camera;

    public GameObject hitEffectPrefab;

    [Header("HP Related Stuff")]
    public float startHealth = 100;
    private float health;
    public Image healthBar;

    private Animator animator;

    public int kills = 0;

    void Start()
    {
        health = startHealth;
        healthBar.fillAmount = health / startHealth;
        animator = this.GetComponent<Animator>();
    }

    public void Fire()
    {
        RaycastHit hit;
        Ray ray = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 200))
        {
            Debug.Log(hit.collider.gameObject.name);

            photonView.RPC("CreateHitEffects", RpcTarget.All, hit.point);

            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 25);

                if (hit.collider.gameObject.GetComponent<Shooting>().health <= 0)
                {
                    photonView.RPC("KillCounter", RpcTarget.AllBuffered);
                }
            }
        }
    }

    [PunRPC]
    public void TakeDamage(int damage, PhotonMessageInfo info)
    {
        this.health -= damage;
        this.healthBar.fillAmount = health / startHealth;

        if (health <= 0)
        {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
            
            StartCoroutine(KillFeed((string)info.Sender.NickName, (string)info.photonView.Owner.NickName));
        }
    }

    [PunRPC]
    public void CreateHitEffects(Vector3 position)
    {
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.2f);
    }

    public void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("isDead", true);
            StartCoroutine(RespawnCountdown());
        }
    }

    IEnumerator RespawnCountdown()
    {
        GameObject respawnText = GameObject.Find("Respawn Text");
        float respawnTime = 5.0f;

        while (respawnTime > 0)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime--;

            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning in " + respawnTime.ToString(".00");
        }

        animator.SetBool("isDead", false);
        respawnText.GetComponent<Text>().text = "";

        int random = Random.Range(0, GameManager.instance.spawnPoints.Length);
        this.transform.position = GameManager.instance.spawnPoints[random].transform.position;
        transform.GetComponent<PlayerMovementController>().enabled = true;

        photonView.RPC("RegainHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RegainHealth()
    {
        health = 100;
        healthBar.fillAmount = health / startHealth;
    }

    public IEnumerator KillFeed(string killer, string dead)
    {
        GameObject killFeedText = GameObject.Find("Kill Feed Text");
        killFeedText.GetComponent<Text>().text = killer + " killed " + dead;

        float killfeedTimer = 3.0f;

        while (killfeedTimer > 0)
        {
            yield return new WaitForSeconds(1.0f);
            killfeedTimer--;
        }

        killFeedText.GetComponent<Text>().text = "";
    }

    [PunRPC]
    public void KillCounter(PhotonMessageInfo info)
    {
        this.kills++;

        if (this.kills >= 10)
        {
            Debug.Log(info.photonView.Owner.NickName + " wins!");

            StartCoroutine(Winner((string)info.photonView.Owner.NickName));
        }
    }

    public IEnumerator Winner(string winner)
    {
        GameObject winnerText = GameObject.Find("Winner Text");
        winnerText.GetComponent<Text>().text = winner + " wins!";

        yield return new WaitForSeconds(3.0f);

        SceneManager.LoadScene("LobbyScene");
    }
}
