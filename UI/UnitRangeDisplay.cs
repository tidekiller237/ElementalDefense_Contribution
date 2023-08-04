using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRangeDisplay : MonoBehaviour
{
    // Objects
    public GameObject Inner;
    public GameObject Outer;
    public GameObject Mask;

    private void Start()
    {
        // Set gameobjects inactive at first
        Inner.SetActive(false);
        Outer.SetActive(false);
        Mask.SetActive(false);
    }

    public void DisplayRange(float currRange, float upRange)
    {
        // Activate gameobjects
        Inner.SetActive(true);
        Outer.SetActive(true);
        Mask.SetActive(true);

        // Set they're radius'
        Inner.transform.localScale = Vector2.one * (currRange * 2);
        Mask.transform.localScale = Vector2.one * (currRange * 2);
        Outer.transform.localScale = Vector2.one * (upRange * 2);
    }

    public void Disable()
    {
        // Deactivate gameobjects
        Inner.SetActive(false);
        Outer.SetActive(false);
        Mask.SetActive(false);
    }
}
