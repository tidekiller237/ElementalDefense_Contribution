using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireUnit3FireWall : MonoBehaviour
{
    public float TravelSpeed;
    public float MaxDistance;
    [Tooltip("Maximum length of the wall (2 x value)")]
    public float MaxLength;
    public int Damage;
    public UnitElement Element;

    [Header("Debugging")]
    public bool Loop;

    Vector3 InitialPosition;
    Vector3 InitialVelocity;
    Vector3 InitialScale;
    List<Collider> HitColliders;

    // Start is called before the first frame update
    void Start()
    {
        InitialPosition = transform.position;
        InitialVelocity = transform.forward.normalized;
        InitialScale = transform.localScale;
        HitColliders = new List<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check distance from start
        float distance = Vector3.Distance(InitialPosition, transform.position);
        if (distance < MaxDistance)
        {
            // Update position
            transform.position += InitialVelocity * TravelSpeed * Time.deltaTime;
            transform.localScale = new Vector3(Mathf.Lerp(0, InitialScale.x * MaxLength, distance / MaxDistance), 1, 1);
        }
        else
        {
            // Deactivate when maximum distance is reached
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!HitColliders.Contains(other) && other.GetComponent<Enemy>() != null)
        {
            HitColliders.Add(other);
            other.GetComponent<Enemy>().DealDamage(Damage, Element);
            other.GetComponent<Enemy>().SetStatus(ConditionID.brn);
        }
    }
}
