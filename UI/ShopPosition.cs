using UnityEngine;
using UnityEngine.UI;

public class ShopPosition : MonoBehaviour
{
    // The UI element that will be positioned at the bottom right corner of the screen
    public RectTransform uiElement;

    // Start is called before the first frame update
    void Awake()
    {
        // Set the anchor of the UI element to the bottom right corner of the screen
        uiElement.anchorMin = new Vector2(1, 0);
        uiElement.anchorMax = new Vector2(1, 0);

        // Set the width and height of the UI element to be a quarter and half of the screen's width and height, respectively
        uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width / 4);
        uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height / 2);

        // Set the position of the UI element to be offset from the anchor by half its width and height
        uiElement.anchoredPosition = new Vector2(-uiElement.rect.width / 2, uiElement.rect.height / 2);
    }
}
