using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : PushTrapSphere
{
    protected override IEnumerator Effect()
    {
        while (true)
        {
            radius += Time.deltaTime * expansionSpeed;

            yield return null;
        }
    }

    protected override void RegisterEnemy(EnemyLocomotion enemy)
    {
        if (!registeredEnemies.Contains(enemy))
        {
            registeredEnemies.Add(enemy);
            enemy.ActivatePushPathfinding();
        }
    }

    protected override void ClearEffect()
    {
        foreach (EnemyLocomotion e in registeredEnemies)
        {
            e.DeactivatePushPathfinding();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<EnemyLocomotion>() == null)
            return;

        EnemyLocomotion loco = collision.collider.GetComponent<EnemyLocomotion>();

        Waypoint originPos = (loco.currentWaypoint.parent != null) ? loco.currentWaypoint.parent : new(loco.spawnPosition, PathNodeType.Normal);
        Waypoint tempPos = originPos;
        float relation = -100;
        int count = 0;

        //make sure that the enemies being pushed by the wave are BEHIND the trap when it triggers.
        //  enemies further along the path than the trap's position should not be affected by the wave.
        while (relation < 0 && count < 5)
        {
            //project position onto waypoint segment line
            Vector3 projVec = transform.position;
            Vector3 pathVec = loco.currentWaypoint.position - tempPos.position;
            Vector3 posVec = loco.transform.position - tempPos.position;
            projVec = ((Vector3.Dot(posVec, pathVec) / pathVec.sqrMagnitude) * pathVec) + tempPos.position;

            //check the relative position to the originPos
            relation = Vector3.Dot((loco.transform.position - tempPos.position), (projVec - tempPos.position));

            //update current waypoint and try again
            tempPos = (tempPos.parent != null) ? tempPos.parent : new(loco.spawnPosition, PathNodeType.Normal);

            //infinite loop prevention
            count++;
        }

        if (relation < 0 || Vector3.Distance(loco.transform.position, tempPos.position) > Vector3.Distance(transform.position, tempPos.position))
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        else
            RegisterEnemy(loco);
    }
}
