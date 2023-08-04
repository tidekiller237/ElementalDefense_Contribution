using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireMortar : Projectile
{
    public float aoeRadius;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void OnHitListener()
    {
        base.OnHitListener();

        Enemy[] enemies = GetEnemies();

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != Target)
                enemies[i].DealDamage(Damage, unitElement);
        }

        Destroy(gameObject);
    }

    private Enemy[] GetEnemies()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, aoeRadius, LayerMask.GetMask("Enemy"));

        Enemy[] enemies = new Enemy[cols.Length];
        for (int i = 0; i < cols.Length; i++)
        {
            if (cols[i].TryGetComponent(out Enemy e))
            {
                enemies[i] = e;
            }
            else
            {
                Debug.LogWarning("OverlapSphere in FireMortar returned an obj that is not an enemy: " + cols[i].name);
            }
        }

        return enemies;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}
