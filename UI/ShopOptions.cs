using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopOptions : MonoBehaviour
{
    //The button prefab that will be changed
    public GameObject buttonPrefab;

    //The scrollable pannel that buttons will be created
    public GameObject buttonPanel;

    //A list to store all of the options (Prefabs)
    public List<GameObject> options;

    // (Temp) A value to hold the position of the turret height.
    // Will be updated to be dynamic when we have more interesting terrain
    public float turretYValue;

    //A list of the created buttons for each option
    private List<Button> buttons = new List<Button>();

    //Flag and reference for drag-and-drop placement
    private bool placing;
    private GameObject placingObject;

    // For transforming mouse pos into world space
    private Camera c;

    void Start()
    {
        //initialize variables
        placing = false;

        //initialization coroutine
        StartCoroutine(Initiate());

        c = FindObjectOfType<Camera>();
    }

    void Update()
    {
        //If placing an object
        if (placing)
        {

            //Update objects position
            Vector3 temp = c.ScreenToWorldPoint(Input.mousePosition);
            placingObject.transform.position = new(temp.x, turretYValue, temp.z);


            //Stop updating object when mouse is released
            if (Input.GetButtonUp("Fire1") && IsValidPosition(temp))
            {
                GameManager.Instance.PlayerCurrency.UseMana(placingObject.GetComponent<AttackUnit>().Cost);
                placing = false;
                placingObject = null;
                
            }
        }
    }

    bool IsValidPosition(Vector3 pos)
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    //This is the logic for when a button is clicked.
    //  All shop-button functionality should be called from here.
    void OptionOnClick(int index)
    {
        AttackUnit u = options[index].GetComponent<AttackUnit>();
        if (u.GetComponent<AttackUnit>().Cost <= GameManager.Instance.PlayerCurrency.CurMana)
        {

            //Spawn prefab
            placingObject = Instantiate(options[index]);

            //Set flag and active object
            placing = true;
        }
        else
        {
            Debug.Log("Player does not have the mana to buy this");
        }
    }

    //Initialization coroutine This is so it performs once after Start has finished being called
    IEnumerator Initiate()
    {
        //wait for first frame to finish
        yield return new WaitForEndOfFrame();

        //Get panel dimensions
        RectTransform panel = GetComponent<ExpandShop>().shop.GetComponent<RectTransform>();

        //Padding from the top of the panel
        float topPadding = GetComponent<ExpandShop>().goldText.GetComponent<RectTransform>().rect.height * 2;

        //Create all buttons and set sprites
        //TODO: Set sprite based on unit that the button spawns
        for (int i = 0; i < options.Count; i++)
        {
            //Add button to list
            buttons.Add(Instantiate(buttonPrefab, buttonPanel.transform).GetComponent<Button>());
            
            //Add event trigger (This is so the button fires on button down and not button up
            int tempi = i;

            //Add event trigger component
            EventTrigger trigger = buttons[i].gameObject.AddComponent<EventTrigger>();

            //Create event trigger
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;

            //Create listener for event trigger
            pointerDown.callback.AddListener((e) => { OptionOnClick(tempi); });

            //Add event trigger to object
            trigger.triggers.Add(pointerDown);

            //Get reference of button as a rect transform
            RectTransform buttonTransform = buttons[i].GetComponent<RectTransform>();

            //get which position the button will be in
            int col = (i % 2);
            int row = (int)Mathf.Floor(i / 2);

            //Set button's position on in the panel
            buttonTransform.anchoredPosition = new Vector3((((panel.rect.width / 2) * col) + (panel.rect.width / 4)),
                (( -(panel.rect.height / 5) * row) - topPadding) - buttonTransform.rect.height / 2);

            // Update text on the button to show unit name + cost
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = options[i].name + "\nCost:" 
                + options[i].GetComponent<AttackUnit>().Cost;
        }
    }
}
