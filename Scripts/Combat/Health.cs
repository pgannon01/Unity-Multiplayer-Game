using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth; // Want only the server to change this variable

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;

    #region Server

    // Set current health to max health
    public override void OnStartServer()
    {
        currentHealth = maxHealth;

        UnitBase.ServerOnPlayerDie += ServerHandlePlayerDie;
    }

    public override void OnStopServer()
    {
        UnitBase.ServerOnPlayerDie -= ServerHandlePlayerDie;
    }

    // Deal damage
    [Server]
    public void DealDamage(int damageAmount)
    {
        if (currentHealth == 0)
        {
            return;
        }

        // This will return the largest of 2 values, so either a health value above 0, or 0, to make sure it never goes into the negatives
        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth != 0)
        {
            return;
        }

        // Dead
        ServerOnDie?.Invoke();
    }

    [Server]
    private void ServerHandlePlayerDie(int connectionId) 
    {
        if (connectionToClient.connectionId != connectionId)
        {
            return;
        }

        // When players lose their base, kill all their units
        DealDamage(currentHealth);
    }

    #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
