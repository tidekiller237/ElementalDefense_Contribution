using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap
{
    GameStatistics stats;

    public GameObject residualPrefab;
    public float residualDuration;
    public LayerMask enemyLayer;
    public bool triggered;

    public bool isSpawnedByWaterGrass;
    public float spawnRadius;

    private void Awake()
    {
        if (isSpawnedByWaterGrass)
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, spawnRadius, LayerMask.GetMask("Turret"));
            for (int i = 0; i < cols.Length; i++)
            {
                if (cols[i].gameObject == gameObject)
                {
                    continue;
                }

                if (cols[i].TryGetComponent(out SpikeTrap st) && st.isSpawnedByWaterGrass)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void Start()
    {
        stats = GameManager.Instance.gameSaveData.playerData.statistics;

        // Don't need to do radius stuff if this is the thorns spawned by water/grass
        if (isSpawnedByWaterGrass)
            return;

        ShopManager shop = FindObjectOfType<ShopManager>();
        if (shop && !shop.placing)
        {
            ToggleRadiusDisplay(false);
        }
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Trigger()
    {
        //return early if there is already a grass effect active in the area
        Collider[] cols = Physics.OverlapSphere(transform.position, EffectiveRadius);

        foreach(Collider col in cols)
        {
            if (col.GetComponent<SpikeTrapResidual>() != null)
                return;

            if(col.GetComponent<SpikeTrap>() != null)
            {
                if (col.GetComponent<SpikeTrap>().triggered)
                    return;
            }
        }

        triggered = true;

        switch (element)
        {
            case UnitElement.Fire:
                stats.IncrementValue(TrackedStatisticsType.FireElementUsage);
                break;
            case UnitElement.Water:
                stats.IncrementValue(TrackedStatisticsType.WaterElementUsage);
                break;
            case UnitElement.Grass:
                stats.IncrementValue(TrackedStatisticsType.GrassElementUsage);
                break;
            default:
                break;
        }

        PlayShootNoise();

        SpikeTrapResidual residual = Instantiate(residualPrefab).GetComponent<SpikeTrapResidual>();

        residual.transform.position = transform.position;
        residual.transform.localScale = Vector3.one * (EffectiveRadius / 10f);
        residual.element = element;
        residual.damagePerSecond = damage;
        residual.radius = EffectiveRadius;
        residual.duration = residualDuration;
        residual.enemyLayer = enemyLayer;

        DestroyTrap();
    }
}
