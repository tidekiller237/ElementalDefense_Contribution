using UnityEngine;

public abstract class Trap : MonoBehaviour, IPurchaseable
{
    public string shopName;
    public string shopDescription;
    public UnitElement element;
    public int damage;
    public float ActivationRadius;
    public float EffectiveRadius;
    public bool IsBeingPlaced = false;
    public bool IsArmed;
    public float secondsToArm;
    float createdTime;

    public AudioClip triggerNoise;
    [Range(0f, 1f)]
    public float volume;
    public Sprite image;

    public SpriteRenderer radiusDisplay;
    public Color displayValidPosColor;
    public Color displayInvalidPosColor;

    public GameObject explosionParticles;

    public string Name => name;
    public int Cost => cost;
    [SerializeField] protected int cost;

    public Sprite ShopImage => shopImage;
    [SerializeField] protected Sprite shopImage;

    public GameObject armedSprite;

    private void Start()
    {
        IsArmed = false;
        createdTime = -1;
    }

    protected virtual void Update()
    {
        // no need to do anything if the turret is being placed
        if (IsBeingPlaced)
            return;

        // set placed time
        if(!IsBeingPlaced && createdTime <= 0)
            createdTime = Time.time;
        
        // check if time has passed and trap is armed
        if (Time.time >= createdTime + secondsToArm)
            IsArmed = true;

        armedSprite.SetActive(IsArmed);

        if (IsArmed)
        {
            armedSprite.transform.Rotate(new(0f, 0f, 1f));

            if (CheckForTrigger())
                Trigger();
        }
    }

    public bool CheckForTrigger()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, ActivationRadius);

        foreach(Collider col in cols)
        {
            if(col.TryGetComponent(out Enemy enemy) && col.TryGetComponent(out EnemyLocomotion loco))
            {
                if (enemy.isUntargetable)
                    continue;

                Waypoint originPos = (loco.currentWaypoint.parent != null) ? loco.currentWaypoint.parent : new(loco.spawnPosition, PathNodeType.Normal);
                Waypoint tempPos = originPos;
                float relation = - 100;
                int count = 0;

                while(relation < 0 && count < 5)
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
                    continue;

                return true;
            }
        }

        return false;
    }

    protected abstract void Trigger();

    public void ToggleRadiusDisplay(bool show)
    {
        radiusDisplay.gameObject.SetActive(show);
        radiusDisplay.gameObject.transform.localScale = (new Vector3(ActivationRadius, ActivationRadius, ActivationRadius) * 2) / 3;
    }

    public void ShowValidPosition(bool isValid)
    {
        radiusDisplay.color = isValid ? displayValidPosColor : displayInvalidPosColor;
    }

    protected void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("VolcanoExplosion"))
            DestroyTrap();
    }

    public void DestroyTrap()
    {
        if (!IsBeingPlaced)
        {
            if (explosionParticles)
                Instantiate(explosionParticles, transform.position + Vector3.up * 10, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
