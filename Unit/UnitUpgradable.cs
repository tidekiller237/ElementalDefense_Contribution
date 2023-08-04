using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitUpgradable : MonoBehaviour
{
    public int maxLevel = 5;

    [Header("Upgrade Curve: Fire Radius")]
    public float UpperLimitFRadius;
    public float SmoothnessFRadius;

    [Header("Upgrade Curve: Fire Rate")]
    public float UpperLimitFRate;
    public float SmoothnessFRate;

    [Header("Gathered Data (Do not edit)")]
    public AttackUnit AU;

    public int Level;

    public float O_Radius;
    public float O_FireRate;

    public int InitialCost;
    public int CurrentCost;

    // The amount of mana spent on the turret (purchasing + upgrades).
    // Used in calculating the sell price
    public int totalManaSpent;

    private void Start()
    {
        // Get the attack unit component
        AU = GetComponent<AttackUnit>();

        // Set initial values
        Level = 1;
        O_Radius = AU.DetectionRadius;
        O_FireRate = AU.FireRateRoundsPerMinute;
        CurrentCost = GetUpgradeCost(Level);
        totalManaSpent += AU.Cost;
    }

    public bool Upgrade()
    {
        if(GameManager.Instance.playerCurrencty.CurMana >= CurrentCost && Level < maxLevel)
        {
            totalManaSpent += CurrentCost;
            //update Gold
            GameManager.Instance.playerCurrencty.UseMana(CurrentCost);

            // Increase the level of the unit
            Level++;

            // Increase values based on upgrade function
            AU.DetectionRadius = GetUpgradedValue(Level, O_Radius, UpperLimitFRadius, SmoothnessFRadius);
            AU.FireRateRoundsPerMinute = Mathf.FloorToInt(GetUpgradedValue(Level, O_FireRate, UpperLimitFRate, SmoothnessFRate));    // Convert to integer

            // Set new cost of upgrade
            CurrentCost = GetUpgradeCost(Level);

            return true;
        }

        return false;
    }

    // Upgrade increase function for units
    public float GetUpgradedValue(int level, float original, float upper, float smooth)
    {
        // Function f(l, o) = (l * UpperLimit) / (l + Rate) + o
        return ((level * upper) / (level + smooth)) + original;
    }

    // Upgrade cost function for units
    public int GetUpgradeCost(int level)
    {
        // Function = ((l + 1) ^ 2 ) / 2
        return Mathf.FloorToInt(Mathf.Pow(level + 1, 2) / 2) + InitialCost;
    }

    // Properties to simplify getting upgraded values
    public float NextCost { get { return GetUpgradeCost(Level + 1); } }
    public float NextFireRate { get { return GetUpgradedValue(Level + 1, O_FireRate, UpperLimitFRate, SmoothnessFRate); } }
    public float NextRadius { get { return GetUpgradedValue(Level + 1, O_Radius, UpperLimitFRadius, SmoothnessFRadius); } }

    public void SellTurret()
    {
        int sellPrice = Mathf.RoundToInt(totalManaSpent * 0.65f);
        GameManager.Instance.playerCurrencty.CurMana += sellPrice;

        Destroy(gameObject);
    }
}
