using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapResidual : MonoBehaviour
{
    public UnitElement element;
    public int damagePerSecond;
    public float radius;
    public float duration;
    public LayerMask enemyLayer;
    private bool end;
    private float timeStamp;

    private void Start()
    {
        timeStamp = Time.time;
        end = false;
        StartCoroutine(Effect());
        StartCoroutine(Duration());
    }

    private void Update()
    {
        if (end)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    private IEnumerator Duration()
    {
        yield return new WaitForSeconds(duration);
        end = true;
    }

    private IEnumerator Effect()
    {
        while (true)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, radius, enemyLayer);

            foreach(Collider col in cols)
            {
                if (col.GetComponent<Enemy>())
                {
                    Enemy e = col.GetComponent<Enemy>();
                    e.DealDamage(damagePerSecond, element);
                }
            }

            yield return new WaitForSeconds(1);
        }
    }
}
