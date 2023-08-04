using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTrapSphere : MonoBehaviour
{
    public UnitElement element;
    public float expansionSpeed;
    public float duration;
    public float initialRadius;
    public LayerMask enemyLayer;
    protected List<EnemyLocomotion> registeredEnemies;
    protected bool end;
    protected float radius;

    private void Start()
    {
        end = false;
        registeredEnemies = new List<EnemyLocomotion>();
        radius = initialRadius;
        StartCoroutine(Duration());
        StartCoroutine(Effect());
    }

    private void Update()
    {
        if (end)
        {
            StopAllCoroutines();
            ClearEffect();
            Destroy(gameObject);
        }
        else
        {
            transform.localScale = Vector3.one * radius;
            transform.GetChild(0).localScale = Vector3.one * radius;
        }
    }

    private IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        end = true;
    }

    protected virtual void RegisterEnemy(EnemyLocomotion enemy)
    {
        //Grab enemies hit by the expanding wave and disable their pathfinding
        // this allows them to be pushed naturally by the sphere.
        if (!registeredEnemies.Contains(enemy))
        {
            registeredEnemies.Add(enemy);
            enemy.ActivateReversePathfinding(expansionSpeed);
        }
    }

    protected virtual void ClearEffect()
    {
        foreach(EnemyLocomotion e in registeredEnemies)
        {
            e.DeactivateReversePathfinding();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<EnemyLocomotion>() == null)
            return;

        EnemyLocomotion loco = collision.collider.GetComponent<EnemyLocomotion>();

        if (Vector3.Distance(loco.transform.position, loco.currentWaypoint.position) < Vector3.Distance(transform.position, loco.currentWaypoint.position))
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        else
        {
            RegisterEnemy(loco);
        }
    }
}
