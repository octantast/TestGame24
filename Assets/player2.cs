using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player2 : MonoBehaviour
{
    public Player player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("redCubes"))// об червоне
        {
            player.gameOver.SetActive(true);
            player.speed = 0;
            player.lostPlayer.SetActive(true);
            player.thisGoBody.SetActive(false);
        }
    }
}
