using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonExpand : MonoBehaviour
{
    public bool called;
    public int expandedId;
    public LargeUnitView expanded;
    public TextMeshProUGUI text;
    public Image image;

    private void Update()
    {
        if(expanded == null)
        {
            try
            {
                // get expanded button
                expanded = GameObject.Find("ButtonBlowUp").GetComponent<LargeUnitView>();
            }
            catch
            {
                // if the expanded button is not loaded yet
                return;
            }

            // create new trigger for mouse over
            EventTrigger.Entry pointerOver = new EventTrigger.Entry();
            pointerOver.eventID = EventTriggerType.PointerEnter;
            pointerOver.callback.AddListener((e) =>
            {
                expanded.Open(text.text, image.sprite);
            });

            EventTrigger.Entry pointerOverExit = new EventTrigger.Entry();
            pointerOverExit.eventID = EventTriggerType.PointerExit;
            pointerOverExit.callback.AddListener((e) =>
            {
                expanded.Close();
            });

            // add triggers to button
            GetComponent<EventTrigger>()?.triggers.Add(pointerOver);
            GetComponent<EventTrigger>()?.triggers.Add(pointerOverExit);

            // get rid of text
            GetComponentInChildren<TextMeshProUGUI>().gameObject.SetActive(false);
        }
    }
}
