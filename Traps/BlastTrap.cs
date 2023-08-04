using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastTrap : Trap
{
    GameStatistics stats;

    private void Start()
    {
        stats = GameManager.Instance.gameSaveData.playerData.statistics;
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void Trigger()
    {
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

        Collider[] cols = Physics.OverlapSphere(transform.position, EffectiveRadius);

        foreach (Collider col in cols)
        {
            if (col.GetComponent<Enemy>())
            {
                if (col.GetComponent<Enemy>().isUntargetable)
                    continue;

                col.GetComponent<Enemy>().DealDamage(damage, element);
            }
        }

        DestroyTrap();
    }
}
