using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dracula : MonoBehaviour
{
    public GameController _gameController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) {
            _gameController.ReceiveBlood();
        }
    }
}
