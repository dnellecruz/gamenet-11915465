using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileTurret : Shooting
{
    public GameObject bulletPrefab;

    void Start()
    {
        isControlEnabled = false;

        GetComponent<PhotonView>().RPC("InitializeHealth", RpcTarget.AllBuffered, startHealth);
    }

    private void FixedUpdate()
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

    public override void Fire()
    {
        photonView.RPC("InstantiateBullet", RpcTarget.AllBuffered, bulletSpawnPoint.position, photonView.Owner.NickName);

        CheckIfLastPlayer();
    }

    [PunRPC]
    public void InstantiateBullet(Vector3 bulletStart, string playerName)
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletStart, Quaternion.identity);
        bullet.GetComponent<Projectile>().bulletOrigin = playerName;

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.AddForce(20 * transform.forward, ForceMode.Impulse);

        Destroy(bullet, 5.0f);
    }
}