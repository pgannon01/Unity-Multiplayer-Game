using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Mirror;

public class ResourcesDisplay : MonoBehaviour
{
    // Just some UI so no need to not be a monobehaviour
    [SerializeField] private TMP_Text resourcesText = null;

    private RTSPlayer player;

    private void Start() 
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

        ClientHandleResourcesUpdated(player.GetResources());

        player.ClientOnResourcesUpdated += ClientHandleResourcesUpdated;
    }

    private void OnDestroy() 
    {
        player.ClientOnResourcesUpdated -= ClientHandleResourcesUpdated;
    }

    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.text = $"Resources: {resources}";
    }
}
