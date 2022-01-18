using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class GameOverDisplay : MonoBehaviour
{
    // Completely client side
    [SerializeField] private GameObject gameOverDisplayParent = null;
    [SerializeField] private TMP_Text winnerNameText = null;

    private void Start() 
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDestroy() 
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void ClientHandleGameOver(string winner)
    {
        winnerNameText.text = $"{winner} has Won!";

        gameOverDisplayParent.SetActive(true);
    }

    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            // Stop hosting game
            NetworkManager.singleton.StopHost();
        }

        else
        {
            // stop client
            NetworkManager.singleton.StopClient(); // automatically sends us back to the home screen
        }
    }
}
