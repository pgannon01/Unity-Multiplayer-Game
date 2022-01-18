using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;
    
    #region Server

    public override void OnStartServer()
    {
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }

    public override void OnStopServer()
    {
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    [ServerCallback]
    private void Update() 
    {
        Targetable target = targeter.GetTarget();

        // For chasing units/target
        if (target != null)
        {
            if ((target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange)
            {
                // chase
                agent.SetDestination(target.transform.position);
            }
            else if (agent.hasPath)
            {
                // stop chasing
                agent.ResetPath();
            }

            return;
        }

        // Don't clear the units path in the same frame we're trying to calculate it in
        if (!agent.hasPath)
        {
            return;
        }

        // When our units get close to the area we clicked, stop them from moving and sliding around each other, with all of them competing to get to the location
        if (agent.remainingDistance > agent.stoppingDistance)
        {
            // If our unit hasn't reached the clicked spot, keep going
            return;
        }

        // When we're within the set stopping distance, clear the path so the unit stops
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 position)
    {
        ServerMove(position);
    }

    [Server]
    public void ServerMove(Vector3 position)
    {
        targeter.ClearTarget();

        if (!NavMesh.SamplePosition(position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            return;
        }

        agent.SetDestination(hit.position);
    }

    [Server]
    private void ServerHandleGameOver()
    {
        agent.ResetPath();
    }

    #endregion
}
