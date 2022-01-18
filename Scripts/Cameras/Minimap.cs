using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Minimap : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private RectTransform minimapRect = null;
    [SerializeField] private float mapScale = 20f;
    [SerializeField] private float offset = -6;

    private Transform playerCameraTransform;

    private void Update() 
    {
        if (playerCameraTransform != null)
        {
            return;
        }

        if (NetworkClient.connection.identity == null)
        {
            return;
        }

        playerCameraTransform = NetworkClient.connection.identity.GetComponent<RTSPlayer>().GetCameraTransform();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MoveCamera();
    }

    public void OnDrag(PointerEventData eventData)
    {
        MoveCamera();
    }

    // Move the camera to where we click on the minimap
    private void MoveCamera()
    {
        // Read our mouse position
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // If the mouse position is not inside the minimap, return
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(minimapRect, mousePosition, null, out Vector2 localPoint))
        {
            return;
        }

        Vector2 lerp = new Vector2((localPoint.x - minimapRect.rect.x) / minimapRect.rect.width, (localPoint.y - minimapRect.rect.y) / minimapRect.rect.height);

        Vector3 newCamearPosition = new Vector3(Mathf.Lerp(-mapScale, mapScale, lerp.x), playerCameraTransform.position.y, Mathf.Lerp(-mapScale, mapScale, lerp.y));

        // Tell the camera to move to our clicked location
        playerCameraTransform.position = newCamearPosition + new Vector3(0f, 0f, offset);
    }

    
}
