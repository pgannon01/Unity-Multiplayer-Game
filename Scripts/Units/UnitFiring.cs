using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private GameObject projectilePrefab = null;
    [SerializeField] private Transform projectileSpawnPoint = null;
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float rotationSpeed = 20f;

    private float lastFireTime;

    Targetable target;

    [ServerCallback]
    private void Update() 
    {
        target = targeter.GetTarget();

        if(target == null)
        {
            return;
        }

        if(!CanFireAtTarget()) 
        {
            return;
        }

        // For rotating the unit to the enemy
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Handle firing
        if (Time.time > (1 / attackSpeed) + lastFireTime)
        {
            // Where our projectile should be aiming
            Quaternion projectileRotation = Quaternion.LookRotation(target.GetAimAtPoint().position - projectileSpawnPoint.position);

            // we can now fire
            GameObject projectileInstance = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileRotation);

            NetworkServer.Spawn(projectileInstance, connectionToClient);

            lastFireTime = Time.time;
        }
    }

    [Server]
    private bool CanFireAtTarget()
    {
        // Check distance from this unit to its target
        return (target.transform.position - transform.position).sqrMagnitude <= attackRange * attackRange;
    }
}
