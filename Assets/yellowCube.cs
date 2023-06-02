using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yellowCube : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("redCubes")) // об червоне
        {
            transform.SetParent(player.platforms[player.currentPlatform].transform);
            player.yellowCubes.Remove(gameObject);
            player.yellowCubes.Add(gameObject);
            player.countYellow -= 1;
            
        }


    }

}
