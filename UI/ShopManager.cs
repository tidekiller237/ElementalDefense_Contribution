using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class ShopManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    //public TextMeshProUGUI goldText;

    public GameObject hudObject;
    public GameObject shopPanel;

    public bool shopEnabled;
    public bool shopOpen;
    
    [Tooltip("A list of all the turret options the player will have.")]
    public List<AttackUnit> shopList;
    [Tooltip("A list of all path disrupter options in the shop.")]
    public List<Trap> trapList;
    public List<WeatherTotem> weatherList;
    public GameObject buttonPrefab;

    [Tooltip("The parent object for the shop buttons.")]
    public Transform buttonParent;

    //Flag and reference for drag-and-drop placement
    public bool placing;
    public bool placingTriggered;
    private GameObject placingObject;
    bool shopping;

    // (Temp) A value to hold the position of the turret height.
    // Will be updated to be dynamic when we have more interesting terrain
    public float turretYValue;
    [SerializeField] LayerMask nonTurret;

    // Shop animation coroutines
    public float shopCloseSpeed;
    public GameObject[] backdrops = new GameObject[2];
    Vector2[] openPositions = new Vector2[2];
    Vector2[] closePositions = new Vector2[2];

    public Action OnTurretPlaced;
    public Action OnTotemPlaced;

    // Wave based unlock turrets
    int waveNumber = 1;
    Level level;
    bool elementalRegion = false;
    List<Button> shopButtons = new List<Button>();

    private void Awake()
    {
        if (shopPanel)
        {
            DisableShop();
        }
        else
        {
            Debug.LogError("Shop canvas is null- please rectify");
        }

        placingTriggered = false;

        openPositions[0] = Vector2.zero;
        openPositions[1] = Vector2.zero;
        closePositions[0] = Vector2.left * -backdrops[1].GetComponent<RectTransform>().rect.width;
        closePositions[1] = Vector2.left * -backdrops[1].GetComponent<RectTransform>().rect.width;
    }

    private void OnEnable()
    {
        if (GameManager.Instance)
            GameManager.Instance.PlayerCurrency.CurrencyChanged += UpdateCurrency;
        else
            Debug.LogError("No instance of GameManager exists in the level");
    }

    public void OpenShop()
    {
        shopOpen = true;
        StopAllCoroutines();
        StartCoroutine(OpenShopAnimation());
    }

    public void CloseShop()
    {
        shopOpen = false;
        StopAllCoroutines();
        StartCoroutine(CloseShopAnimation());
    }

    public void ToggleShop()
    {
        if (!shopOpen) OpenShop();
        else CloseShop();
    }

    public void EnableShop()
    {
        shopPanel.gameObject.SetActive(true);
        shopEnabled = true;
    }

    public void DisableShop()
    {
        shopPanel.gameObject.SetActive(false);
        shopEnabled = false;
    }

    // Control the wave update variables
    private void GetLevel()
    {
        if (level == null)
        {
            level = GameManager.Instance.CurrentLevel;
            if (level == null)
                return;
        }

        if (level != GameManager.Instance.CurrentLevel)
        {
            level = GameManager.Instance.CurrentLevel;
            waveNumber = 1;
        }

        else
        {
            waveNumber = level.mainSpawner.waveNumber + 1;
            UpdateButtons();
        }
    }

    // Active and deactivate the shop turret buttons based on the wave number
    private void UpdateButtons()
    {
        for (int i = 0; i < shopButtons.Count; i++)
        {
            switch (waveNumber)
            {
                case 1:
                    if (i % 3 != 0)
                    {
                        shopButtons[i].interactable = false;
                    }
                    else
                    {
                        shopButtons[i].interactable = true;
                    }
                    break;
                case 2:
                    if (i % 3 == 2)
                    {
                        shopButtons[i].interactable = false;
                    }
                    else
                    {
                        shopButtons[i].interactable = true;
                    }
                    break;
                default:
                    shopButtons[i].interactable = true;
                    break;
            }
        }
    }

    private void Start()
    {
        UpdateCurrency();
        for (int i = 0; i < shopList.Count; i++)
        {
            GameObject b = Instantiate(buttonPrefab, buttonParent.transform);
            ButtonExpand expand = b.GetComponent<ButtonExpand>();
            expand.text.text = shopList[i].name + "\nCost: " + shopList[i].Cost;
            expand.image.sprite = shopList[i].image;

            // Add the turret buttons to a list for future manipulation
            if (b)
            {
                shopButtons.Add(b.GetComponent<Button>());
            }

            //Add event trigger (This is so the button fires on button down and not button up
            int tempi = i;

            //Add event trigger component
            EventTrigger trigger = b.AddComponent<EventTrigger>();

            //Create event trigger
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;

            //Create listener for event trigger
            pointerDown.callback.AddListener((e) => { OptionOnClick(tempi); });

            //Add event trigger to object
            trigger.triggers.Add(pointerDown);
        }

        for (int i = 0; i < trapList.Count; i++)
        {
            GameObject b = Instantiate(buttonPrefab, buttonParent.transform);
            ButtonExpand expand = b.GetComponent<ButtonExpand>();
            expand.text.text = trapList[i].name + "\nCost: " + trapList[i].Cost;
            expand.image.sprite = trapList[i].image;

            //Add event trigger (This is so the button fires on button down and not button up
            int tempi = i;

            //Add event trigger component
            EventTrigger trigger = b.AddComponent<EventTrigger>();

            //Create event trigger
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;

            //Create listener for event trigger
            pointerDown.callback.AddListener((e) => { OptionOnClick(tempi + shopList.Count); });

            //Add event trigger to object
            trigger.triggers.Add(pointerDown);
        }

        for (int i = 0; i < weatherList.Count; i++)
        {
            GameObject b = Instantiate(buttonPrefab, buttonParent.transform);
            ButtonExpand expand = b.GetComponent<ButtonExpand>();
            expand.text.text = weatherList[i].name + "\nCost: " + weatherList[i].Cost;
            expand.image.sprite = weatherList[i].ShopImage;

            //Add event trigger (This is so the button fires on button down and not button up
            int tempi = i;

            //Add event trigger component
            EventTrigger trigger = b.AddComponent<EventTrigger>();

            //Create event trigger
            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;

            //Create listener for event trigger
            pointerDown.callback.AddListener((e) => { OptionOnClick(tempi + shopList.Count + trapList.Count); });

            //Add event trigger to object
            trigger.triggers.Add(pointerDown);
        }

        shopEnabled = true;
    }

    private void Update()
    {
        if (GameManager.Instance.GamePaused)
            return;

        if (hudObject.activeInHierarchy && !shopEnabled)
        {
            EnableShop();
        }
        else if(!hudObject.activeInHierarchy && shopEnabled)
        {
            DisableShop();
        }

        if (shopEnabled)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (shopOpen)
                {
                    CloseShop();
                }
                else
                {
                    OpenShop();
                }
            }

            bool reopenShop = false;

            //If placing an object
            if (placing)
            {
                if (Input.GetButtonUp("Fire2"))
                {
                    placing = false;

                    Destroy(placingObject);
                    placing = false;
                    placingObject = null;

                    return;
                }

                //Update objects position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit terrainData;

                Vector3 temp = Vector3.zero;

                if (Physics.Raycast(ray, out terrainData, 1000.0f, nonTurret))
                {
                    temp = terrainData.point;

                    placingObject.transform.position = temp;

                    placingObject.GetComponent<AttackUnit>()?.ShowValidPosition(IsValidPosition(terrainData));
                    placingObject.GetComponent<WeatherTotem>()?.ShowValidPosition(IsValidPosition(terrainData));
                    placingObject.GetComponent<Trap>()?.ShowValidPosition(IsValidPositionOnPath(terrainData));

                    //Stop updating object when mouse is released
                    if (Input.GetButtonUp("Fire1"))
                    {
                        if (placingObject.GetComponent<AttackUnit>() && IsValidPosition(terrainData) && !shopping)
                        {
                            GameManager.Instance.PlayerCurrency.UseMana(placingObject.GetComponent<AttackUnit>().Cost);
                            placingObject.GetComponent<AttackUnit>().IsBeingPlaced = false;
                            placingObject.GetComponent<AttackUnit>().ToggleRadiusDisplay(false);
                            placing = false;
                            placingObject = null;
                            reopenShop = true;
                            OnTurretPlaced?.Invoke();
                        } 
                        else if(placingObject.GetComponent<Trap>() && IsValidPositionOnPath(terrainData) && !shopping)
                        {
                            GameManager.Instance.PlayerCurrency.UseMana(placingObject.GetComponent<Trap>().Cost);
                            placingObject.GetComponent<Trap>().IsBeingPlaced = false;
                            placingObject.GetComponent<Trap>().ToggleRadiusDisplay(false);
                            placing = false;
                            placingObject = null;
                            reopenShop = true;
                        }
                        else if (placingObject.GetComponent<WeatherTotem>() && IsValidPosition(terrainData) && !shopping)
                        {
                            GameManager.Instance.PlayerCurrency.UseMana(placingObject.GetComponent<WeatherTotem>().Cost);
                            placingObject.GetComponent<WeatherTotem>().ToggleRadiusDisplay(false);
                            placingObject.GetComponent<WeatherTotem>().isBeingPlaced = false;
                            placing = false;
                            placingObject = null;
                            reopenShop = true;
                            OnTotemPlaced?.Invoke();
                        }
                        else
                        {
                            Destroy(placingObject);
                            placing = false;
                            placingObject = null;
                            OpenShop();
                        }
                    }
                }
            }
            else
            {
                OptionOnClick(CheckHotkeys());
            }

            if (reopenShop)
            {
                OpenShop();
                reopenShop = false;
            }
            else if (placingTriggered)
            {
                CloseShop();
                placingTriggered = false;
            }
        }

        if (!GameManager.Instance.debugMode)
        {
            // Update current level;
            if (SceneManager.GetActiveScene().name != "Boss Level" && SceneManager.GetActiveScene().name != "Main Menu")
            {
                GetLevel();
                elementalRegion = true;
            }
            else
            {
                elementalRegion = false;
            }
        }
    }

    bool IsValidPosition(RaycastHit payload)
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    bool IsValidPositionOnPath(RaycastHit payload)
    {
        //Redacted code. I did not write the code here, so it was removed to not steal from author.
    }

    private void SpawnTurret(int index)
    {
        AttackUnit u = shopList[index];
        if (u.GetComponent<AttackUnit>().Cost <= GameManager.Instance.PlayerCurrency.CurMana)
        {

            //Spawn prefab
            placingObject = Instantiate(shopList[index]).gameObject;
            placingObject.GetComponent<AttackUnit>().IsBeingPlaced = true;
            placingObject.GetComponent<AttackUnit>().ToggleRadiusDisplay(true);

            //Set flag and active object
            placing = true;
            placingTriggered = true;
        }
        else
        {
            Debug.Log("Player does not have the mana to buy this");
        }
    }

    void OptionOnClick(int index)
    {
        // Do nothing if game is paused
        if (GameManager.Instance.GamePaused || index < 0 || index >= shopList.Count + trapList.Count + weatherList.Count)
            return;

        if (index < shopList.Count)
        {
            if (elementalRegion)
            {
                switch (waveNumber)
                {
                    case 1:
                        if (index % 3 == 0)
                        {
                            SpawnTurret(index);
                        }
                        break;
                    case 2:
                        if (index % 3 == 0 || index % 3 == 1)
                        {
                            SpawnTurret(index);
                        }
                        break;
                    default:
                        SpawnTurret(index);
                        break;
                }
            }
            else
            {
                SpawnTurret(index);
            }

            return;
        }
        else
            index -= shopList.Count;

        if (index < trapList.Count)
        {
            Trap u = trapList[index];
            if (u.GetComponent<Trap>().Cost <= GameManager.Instance.PlayerCurrency.CurMana)
            {
                //Spawn prefab
                placingObject = Instantiate(trapList[index]).gameObject;
                placingObject.GetComponent<Trap>().IsBeingPlaced = true;
                placingObject.GetComponent<Trap>().ToggleRadiusDisplay(true);

                //set flag and activate object
                placing = true;
                placingTriggered = true;
            }
            else
            {
                Debug.Log("Player does not have the mana to buy this");
            }

            return;
        } 
        else
            index -= trapList.Count;

        if (index < weatherList.Count)
        {
            WeatherTotem wt = weatherList[index];
            if (wt.GetComponent<WeatherTotem>().Cost <= GameManager.Instance.PlayerCurrency.CurMana)
            {
                //Spawn prefab
                placingObject = Instantiate(weatherList[index]).gameObject;
                placingObject.GetComponent<WeatherTotem>().ToggleRadiusDisplay(true);
                placingObject.GetComponent<WeatherTotem>().isBeingPlaced = false;
                //set flag and activate object
                placing = true;
                placingTriggered = true;
            }
            else
            {
                Debug.Log("Player does not have the mana to buy this");
            }

            return;
        }
    }

    public IEnumerator OpenShopAnimation()
    {
        float tCounter = 0;
        Vector2 initPositionA = backdrops[0].GetComponent<RectTransform>().localPosition;
        Vector2 initPositionB = backdrops[1].GetComponent<RectTransform>().localPosition;

        while (tCounter < 1)
        {
            tCounter += Time.deltaTime * shopCloseSpeed;
            RectTransform rect = backdrops[0].GetComponent<RectTransform>();
            rect.localPosition = Vector2.Lerp(initPositionA, openPositions[0], Mathf.Min(tCounter, 1));
            rect = backdrops[1].GetComponent<RectTransform>();
            rect.localPosition = Vector2.Lerp(initPositionB, openPositions[1], Mathf.Min(tCounter, 1));

            yield return null;
        }
    }

    public IEnumerator CloseShopAnimation()
    {
        float tCounter = 0;
        Vector2 initPositionA = backdrops[0].GetComponent<RectTransform>().localPosition;
        Vector2 initPositionB = backdrops[1].GetComponent<RectTransform>().localPosition;

        while (tCounter < 1)
        {
            tCounter += Time.deltaTime * shopCloseSpeed;
            RectTransform rect = backdrops[0].GetComponent<RectTransform>();
            rect.localPosition = Vector2.Lerp(initPositionA, closePositions[0], Mathf.Min(tCounter, 1));
            rect = backdrops[1].GetComponent<RectTransform>();
            rect.localPosition = Vector2.Lerp(initPositionB, closePositions[1], Mathf.Min(tCounter, 1));

            yield return null;
        }
    }

    // Control the ability to click through the shop to place a turret
    public void OnPointerEnter(PointerEventData eventData)
    {
        shopping = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        shopping = false;
    }

    // Used to determine if the scroll should affect the camera or shop
    public bool GetShopping()
    {
        return shopping;
    }

    // Used to return hotkey used
    public int CheckHotkeys()
    {
        // 1-9: Turrets
        // F, G, B: Traps (fire, geyser, brambles)
        // V, R, T: Totems (volcano, rain, tornado)

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            return 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            return 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            return 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            return 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            return 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            return 8;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            return 9;
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            return 10;
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            return 11;
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            return 12;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            return 13;
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            return 14;
        }

        return -1;
    }
}
