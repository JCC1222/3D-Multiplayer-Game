using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{

    public string player_prefab;
    public Transform spawn_point;

    // Start is called before the first frame update
    private void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    public void Spawn()
    {
        PhotonNetwork.Instantiate(player_prefab, spawn_point.position, spawn_point.rotation);
    }
}
