using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushTrap : Trap
{
    public GameObject trapSphere;
    public float expansionSpeed;
    public float duration;
    public LayerMask enemyLayer;

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

        //spawn a sphere collider that will expand and push enemies backwards
        PushTrapSphere sphere = Instantiate(trapSphere).GetComponent<PushTrapSphere>();

        sphere.transform.localScale = new(EffectiveRadius * 10, EffectiveRadius * 10, EffectiveRadius * 10);
        sphere.transform.position = transform.position;
        sphere.element = element;
        sphere.expansionSpeed = expansionSpeed;
        sphere.duration = duration;
        sphere.initialRadius = EffectiveRadius;
        sphere.enemyLayer = enemyLayer;

        DestroyTrap();
    }
}
