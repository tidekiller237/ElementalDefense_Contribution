using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouceProjectile : Projectile
{
    public int maxBounces;
    private int bounceCount;
    public float detectionRadius;
    public LayerMask enemyMask;

    Enemy[] prevEnemies;

    protected override void Start()
    {
        base.Start();

        prevEnemies = new Enemy[maxBounces];
        prevEnemies[0] = Target.GetComponent<Enemy>();
        bounceCount = 0;
    }

    protected override void OnHitListener()
    {
        if (Target)
        {
            Target.DealDamage(Damage, unitElement);

            if (doesStatusEffect)
            {
                Target.SetStatus(conditionID);
            }

            if (impactParticles)
                Instantiate(impactParticles, transform.position, Quaternion.identity);

            // Create and play the explosion effect upon collision with an enemy
            if (explosionPrefab != null)
            {
                GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            }
        }
        else
        {
            Destroy(gameObject);
        }

        //if there are bounces left, select a new target in range and repeat the fire process
        if (bounceCount < maxBounces)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, detectionRadius, enemyMask);

            if(cols.Length > 0)
            {
                foreach(Collider col in cols)
                {
                    if (col.GetComponent<Enemy>())
                    {
                        bool check = true;
                        for(int i = 0; i < prevEnemies.Length; i++)
                        {
                            if (prevEnemies[i] != null && col.GetComponent<Enemy>().GetInstanceID() == prevEnemies[i].GetInstanceID())
                            {
                                check = false;
                                break;
                            }
                        }

                        if (check)
                        {
                            startPoint = Target.transform.position;
                            Target = col.GetComponent<Enemy>();
                            t = 0;
                            bounceCount++;
                            prevEnemies[bounceCount] = Target;
                            Debug.Log("Bounce | " + bounceCount);
                            return;
                        }
                    }
                }
            }
        }

        Destroy(gameObject);
    }
}
