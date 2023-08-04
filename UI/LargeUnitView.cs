using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class LargeUnitView : MonoBehaviour
{
    // call list, this is to prevent overlapping/errors
    public bool showing;
    public string str;
    public Sprite sprite;

    // visual components
    public Image image;
    public TextMeshProUGUI text;

    GameObject child;

    private void Start()
    {
        showing = false;

        child = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        if(UIManager.Instance.CurrentState != UIState.Game_ShopMenu || Input.GetButtonDown("Fire1"))
        {
            Close();
            return;
        }

        RectTransform rect = GetComponent<RectTransform>();
        rect.anchoredPosition = new(Mathf.Clamp(Input.mousePosition.x, 100, Screen.width - 100), rect.anchoredPosition.y);

        if (showing)
        {
            child.SetActive(true);
            text.text = str;
            image.sprite = sprite;
        }
        else
        {
            child.SetActive(false);
        }
    }

    // returns the id
    public void Open(string text, Sprite img)
    {
        showing = true;
        str = text;
        sprite = img;
    }

    public void Close()
    {
        showing = false;
    }
}
