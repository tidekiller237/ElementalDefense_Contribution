using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpandShop : MonoBehaviour
{
    // The shop menu game object
    public GameObject shop;

    // The button that expands and collapses the shop menu
    public Button expandButton;

    //The text that displays gold amount
    public GameObject goldText;

    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.PlayerCurrency.CurrencyChanged += UpdateGoldText;
        else
            Debug.LogError("No instance of GameManager in the level");
    }

    // Start is called before the first frame update
    void Start()
    {
        // Hide the shop menu by default
        shop.SetActive(false);

        // Add a listener to the toggle button's onClick event
        expandButton.onClick.AddListener(ToggleShop);

        //References to transforms as "RectTransform"s
        RectTransform parentTransform = GetComponent<RectTransform>();
        RectTransform shopTransform = shop.GetComponent<RectTransform>();

        //Set Shop size
        shopTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentTransform.rect.width);
        shopTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height - (Screen.height / 5));

        //Set Shop position
        shopTransform.anchoredPosition = new Vector2(-shopTransform.rect.width / 2, shopTransform.rect.height / 2);

        //Set size of gold text
        goldText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, shopTransform.rect.width);
        goldText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, shopTransform.rect.height / 20);

        //Set position of gold text
        goldText.GetComponent<RectTransform>().anchoredPosition = Vector2.up * -(goldText.GetComponent<RectTransform>().rect.height);

        UpdateGoldText();
    }

    // Toggles the shop menu on or off
    void ToggleShop()
    {
        // If the shop menu is currently active, deactivate it
        // Move the button above the shop if it is open and below if not
        if (shop.activeSelf)
        {
            shop.SetActive(false);
            expandButton.transform.position = GetComponent<RectTransform>().position;
        }
        // Otherwise, activate it
        else
        {
            shop.SetActive(true);
            expandButton.transform.position = shop.GetComponent<RectTransform>().position + (Vector3.up * (shop.GetComponent<RectTransform>().rect.height / 2));
        }
    }

    public void UpdateGoldText()
    {
        Debug.Log("Updating mana...");

        if (GameManager.Instance)
            goldText.GetComponent<TextMeshProUGUI>().text = "Mana: " + GameManager.Instance.PlayerCurrency.CurMana;
        else
            Debug.LogError("There is no instance of the GameManager is this level");
    }

    private void OnDisable()
    {
        if (GameManager.Instance)
            GameManager.Instance.PlayerCurrency.CurrencyChanged -= UpdateGoldText;
        else
            Debug.LogError("No instance of GameManager in the level");
    }
}
