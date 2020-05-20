using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviourPunCallbacks
{
    #region Variables

    public Gun[] loadout;
    public Transform weaponParent;
    public GameObject bulletholePrefab;
    public LayerMask canBeShot;

    private float currentCooldown;
    private int currentIndex;
    private GameObject currentWeapon;

    #endregion


    #region MonoBehaviour Callbacks

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) { photonView.RPC("Equip", RpcTarget.AllViaServer, 0); }

        if (currentWeapon != null)
        {
            if (Input.GetMouseButtonDown(0) && currentCooldown <= 0)
            {
                photonView.RPC("Shoot", RpcTarget.All);
            }

            //weapon position elasticity
            currentWeapon.transform.localPosition = Vector3.Lerp(currentWeapon.transform.localPosition, Vector3.zero, Time.deltaTime * 4f);

            //CoolDown
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }

    }

    #endregion


    #region Private Methods

    [PunRPC]
    void Equip (int p_ind)
    {
        if (currentWeapon != null) Destroy(currentWeapon);

        currentIndex = p_ind;

        GameObject t_newWeapon = Instantiate(loadout[p_ind].prefab, weaponParent.position, weaponParent.rotation, weaponParent) as GameObject;
        t_newWeapon.transform.localPosition = Vector3.zero;
        t_newWeapon.transform.localEulerAngles = Vector3.zero;
        t_newWeapon.GetComponent<Sway>().isMine = photonView.IsMine;

        currentWeapon = t_newWeapon;
    }

    [PunRPC]
    void Shoot()
    {
        Transform t_spawn = transform.Find("Cameras/Normal Camera");

        //Bloom
        Vector3 t_bloom = t_spawn.position + t_spawn.forward * 1000f;
        t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.up;
        t_bloom += Random.Range(-loadout[currentIndex].bloom, loadout[currentIndex].bloom) * t_spawn.right;
        t_bloom -= t_spawn.position;
        t_bloom.Normalize();


        //Raycast
        RaycastHit t_hit = new RaycastHit();
        if (Physics.Raycast(t_spawn.position, t_bloom, out t_hit, 1000f, canBeShot))
        {
            GameObject t_newHole = Instantiate(bulletholePrefab, t_hit.point + t_hit.normal * 0.001f, Quaternion.identity) as GameObject;
            t_newHole.transform.LookAt(t_hit.point + t_hit.normal);
            Destroy(t_newHole, 5f);

            if (photonView.IsMine)
            {
                //Shooting Of Player
                if(t_hit.collider.gameObject.layer == 11)
                {
                    //RPC Call to Damage Player
                    t_hit.collider.gameObject.GetPhotonView().RPC("TakeDamage", RpcTarget.All, loadout[currentIndex].damage);
                }
            }
        }

        //Gun Fx
        currentWeapon.transform.Rotate(-loadout[currentIndex].recoil, 0, 0);
        currentWeapon.transform.position -= currentWeapon.transform.forward * loadout[currentIndex].kickback;
        
        //CoolDown
        currentCooldown = loadout[currentIndex].firerate;
    }

    [PunRPC]
    private void TakeDamage(int p_damage)
    {
        GetComponent<Motion>().TakeDamage(p_damage);
    }

    #endregion
}
