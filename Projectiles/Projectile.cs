using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    public int Damage = 30;

    public float ExplosionForce = 100.0f;
    [HideInInspector] public UnitElement unitElement;
    public ConditionID conditionID;

    public GameObject explosionPrefab;
    
    // Particles that the projectile will make upon colliding with an object. Will not harm enemies.
    public GameObject impactParticles;

    public bool doesStatusEffect = false;

    float speed;
    public UnityEvent OnHit;

    protected Enemy Target;
    protected Vector3 startPoint;
    protected float t;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    protected virtual void Update()
    {
        //if the projectile has a target, move towards that target's position to make it look
        //  like a homing projectile and trigger it's effect upon "impact" (t = 1)
        if (Target != null)
        {
            if (t <= 1)
            {
                t += Time.deltaTime * speed;
                transform.position = Vector3.Lerp(startPoint, Target.transform.position, Mathf.Min(t, 1));
            }
            else
            {
                OnHit?.Invoke();
            }
        }
        else
        {
            //this will only reach if the target is destroyed before the projectile reaches it
            Destroy(gameObject);
        }
    }

    public void SetTarget(Vector3 spawnPosition, Enemy target, float projectileTime)
    {
        startPoint = spawnPosition;
        speed = 1 / projectileTime;
        Target = target;
        t = 0;
    }

    protected virtual void OnHitListener()
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
            
        Destroy(gameObject);
        
    }

    protected void DestroyBullet()
    {
        Destroy(gameObject);
    }
}
