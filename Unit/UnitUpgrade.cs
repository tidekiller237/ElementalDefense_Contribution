using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnitUpgrade : MonoBehaviour
{
    // Other display objects
    public GameObject UnitRangeDisplayPrefab;
    public UnitRangeDisplay RangeDisplay;

    // UI parent panel
    public GameObject Parent;

    /* ===============================================================================
     * Current Text Elements Mapping:
     * 
     * 0 = current level
     * 1 = current cost
     * 2 = next cost
     * 3 = current fire rate
     * 4 = next fire rate
     * 5 = current range (radius)
     * 6 = next range (radius)
     * 7 = sell price
     * 8 = unit name
     * =============================================================================== */

    // UI elements (See above ^)
    public TextMeshProUGUI[] TextElements;

    // Target
    public UnitUpgradable Target;

    public ShopManager shop;

    [SerializeField] LayerMask turretLayer;
    private void OnEnable()
    {
        // disable panel
        if (Parent)
            Parent.SetActive(false);

        // Get UnitRangeDisplay object
        if (!RangeDisplay)
        {
            RangeDisplay = Instantiate(UnitRangeDisplayPrefab, transform).GetComponent<UnitRangeDisplay>();
        }
        
    }


    private bool ShopInactive()
    {
        if (shop == null)
        {
            shop = FindObjectOfType<ShopManager>();
            return true;
        }
        else
        {
            if (shop.placing)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }

    private void Update()
    {

        // don't bother with unit upgrades unless we're in an actual level
        if (SceneManager.GetActiveScene().name.Contains("Level"))
        {
            // If the level has been won, then turn off upgrade menu if it is on
            if (UIManager.Instance?.CurrentState == UIState.Game_GameWin || UIManager.Instance?.CurrentState == UIState.Game_GameOver ||
                UIManager.Instance?.CurrentState == UIState.Game_PauseMenu || UIManager.Instance?.CurrentState == UIState.Game_Options)
            {
                ClearTarget();
                OnEnable();
                return;
            }

            // Check for click on unit
            if (ShopInactive())
            {
                // Get the object under the mouse
                RaycastHit[] hits = CheckForTurret();

                // If a turret was hit
                foreach (RaycastHit hit in hits)
                {
                    // Don't show popup if turret is being placed
                    if (hit.collider != null && hit.collider.GetComponentInParent<AttackUnit>()
                        && !hit.collider.GetComponentInParent<AttackUnit>().IsBeingPlaced)
                    {
                        // Set target
                        Target = hit.collider.GetComponentInParent<UnitUpgradable>();
    
                        if (RangeDisplay != null)
                            // Move Range Object
                            RangeDisplay.transform.position = new Vector3(Target.transform.position.x, 0.05f, Target.transform.position.z);

                        UIManager.Instance.OpenUnitUpgradePanel();

                        break;
                    }
                    else
                    {
                        // Clear target
                        ClearTarget();
                    }
                }

                // Have to check in case nothing is hit
                if (hits.Length == 0)
                {
                    // Clear target
                    ClearTarget();
                }

            }

            // Update if there is a current "target"
            if (Target != null)
            {
                // Activate Window
                //Parent.SetActive(!Target.AU.IsBeingPlaced);
                Parent.SetActive(true);

                // Set values
                TextElements[0].text = "" + Target.Level;
                TextElements[1].text = "Cost: " + Target.CurrentCost;
                TextElements[2].text = "" + Target.NextCost;
                TextElements[3].text = "Fire Rate: " + Mathf.Floor(Target.AU.FireRateRoundsPerMinute);
                TextElements[4].text = "" + Mathf.Floor(Target.NextFireRate);
                TextElements[5].text = "Range: " + Target.AU.DetectionRadius.ToString("0.00");
                TextElements[6].text = "" + Target.NextRadius.ToString("0.00");
                TextElements[7].text = "" + Mathf.RoundToInt(Target.totalManaSpent * 0.65f);

                // Split returns an array of substrings, we want the 1st element in the array
                // which contains the name of the turret before the "(Clone)" text
                TextElements[8].text = "" + Target.AU.name.Split('(')[0];

                // Set color of cost text
                TextElements[1].color = (GameManager.Instance.playerCurrencty.CurMana >= Target.CurrentCost) ? Color.white : Color.red;

                // Update panel position
                Parent.transform.position = Input.mousePosition;

                // Clamp the panel position to the screen bounds
                Rect childRect = Parent.transform.GetChild(0).GetComponent<RectTransform>().rect;
                Parent.transform.position = new(Mathf.Clamp(Parent.transform.position.x, childRect.width / 2, Screen.width - (childRect.width / 2)),
                    Mathf.Clamp(Parent.transform.position.y, 0f, Screen.height - childRect.height),
                    Parent.transform.position.z);

                // Update values for Range Object
                RangeDisplay?.DisplayRange(Target.AU.DetectionRadius, Target.GetUpgradedValue(Target.Level + 1, Target.O_Radius, Target.UpperLimitFRadius, Target.SmoothnessFRadius));

                // If the player clicks on the unit, upgrade it
                if (!Target.AU.IsBeingPlaced)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        // Upgrade the unit
                        Target.Upgrade();
                    }
                    else if (Input.GetButtonDown("Fire2"))
                    {
                        Target.SellTurret();
                    }
                }
            }
            else
            {
                ClearTarget();
            }
        }
    }

    private RaycastHit[] CheckForTurret()
    {
        // Get the object under the mouse
        RaycastHit[] hit;

        hit = Physics.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Camera.main.transform.forward, 1000f, turretLayer);
        return hit;
    }

    private void ClearTarget()
    {
        // Clear target
        Target = null;

        UIManager.Instance.CloseUnitUpgradePanel();

        // Deactivate panel
        if (Parent)
            Parent.SetActive(false);

        if(RangeDisplay != null)
            // Deactivate range display
            RangeDisplay.Disable();
    }
}
