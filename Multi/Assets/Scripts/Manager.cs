using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{

    public string player_prefab;
    public Transform[] spawn_point;

    // Start is called before the first frame update
    private void Start()
    {
        Spawn();
    }

    // Update is called once per frame
    public void Spawn()
    {
        Transform t_spawn = spawn_point[Random.Range(0, spawn_point.Length)];
        PhotonNetwork.Instantiate(player_prefab, t_spawn.position, t_spawn.rotation);
    }
}
