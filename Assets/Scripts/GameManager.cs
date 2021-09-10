using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Add System.Serializable so the objects can be serialized and
// its value can be set from inspector
[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;
}

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null; 
    public static GameManager Instance
    {
        get 
        { 
            if (_instance == null) 
            { 
                _instance = FindObjectOfType<GameManager> (); 
            }
            return _instance; 
        } 
    }

    // [Range (min, max)] function keeps the value between min & max
    [Range (0f, 1f)] 

    public float AutoCollectPercentage = 0.1f; 
    public ResourceConfig[] ResourcesConfigs;

    // Added in "Differentiate Upgrade color"
    public Sprite[] ResourcesSprites;

    public Transform ResourcesParent; 
    public ResourceController ResourcePrefab; 
    public Text GoldInfo; 
    public Text AutoCollectInfo;

    // Added in "Creating Incremental Tap"
    public TapText TapTextPrefab;
    public Transform CoinIcon;

    private List<ResourceController> _activeResources = new List<ResourceController> (); 
    private List<TapText> _tapTextPool = new List<TapText> ();  // Added in "Creating Incremental Tap"
    
    private float _collectSecond; 
    public double TotalGold;

    // Start is called before the first frame update
    void Start()
    {
        AddAllResources ();
    }

    // Update is called once per frame
    void Update()
    {
        // function to execute CollectPerSecond every second 
        _collectSecond += Time.unscaledDeltaTime;
        if (_collectSecond >= 1f) 
        { 
            CollectPerSecond (); 
            _collectSecond = 0f; 
        }

        // Added in "Differentiate Upgrade color"
        CheckResourceCost ();

        // Added in "Creating Incremental Tap"
        CoinIcon.transform.localScale = Vector3.LerpUnclamped (CoinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        CoinIcon.transform.Rotate (0f, 0f, Time.deltaTime * -100f);
    }

    private void AddAllResources ()
    {
        // Added in "Add Unlock Resources feature"
        bool showResources = true;

        foreach (ResourceConfig config in ResourcesConfigs) 
        { 
            GameObject obj = Instantiate (ResourcePrefab.gameObject, ResourcesParent, false); 
            ResourceController resource = obj.GetComponent<ResourceController> (); 
 
            resource.SetConfig (config);

            // Added in "Add Unlock Resources feature"
            obj.gameObject.SetActive (showResources);

            // Added in "Add Unlock Resources feature"
            if (showResources && !resource.IsUnlocked)
            {
                showResources = false;
            }

            _activeResources.Add (resource); 
        }
    }
    

    private void CollectPerSecond () 
    { 
        double output = 0; 
        foreach (ResourceController resource in _activeResources) 
        {
            // Added in "Add Unlock Resources feature"
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput ();
            }
        }
        output *= AutoCollectPercentage; 

        // T("F1") makes number with 1 digit after comma
        AutoCollectInfo.text = $"PENGUMPUL OTOMATIS : { output.ToString ("F1") } / DETIK"; 
        AddGold (output);
    }

    public void AddGold (double value)
    {
        TotalGold += value;
        GoldInfo.text = $"Gold: { TotalGold.ToString ("0") }";
    }

    // Added in "Creating Incremental Tap" 
    public void CollectByTap (Vector3 tapPosition, Transform parent)
    {
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            // Added in "Add Unlock Resources feature"
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput ();
            }
        }

        TapText tapText = GetOrCreateTapText ();
        tapText.transform.SetParent (parent, false);
        tapText.transform.position = tapPosition;
        tapText.Text.text = $"+{ output.ToString ("0") }";
        tapText.gameObject.SetActive (true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;
        AddGold (output);
    }
    
    // Added in "Creating Incremental Tap"
    private TapText GetOrCreateTapText ()
    {
        TapText tapText = _tapTextPool.Find (t => !t.gameObject.activeSelf);
        if (tapText == null)
        {
            tapText = Instantiate (TapTextPrefab).GetComponent<TapText> ();
            _tapTextPool.Add (tapText);
        }
        return tapText;
    }

    // Added in "Differetiate Upgrade color"
    private void CheckResourceCost ()
    {
        foreach (ResourceController resource in _activeResources)
        {
            // Added in "Add Unlock Resources feature"
            bool isBuyable = false;
            if (resource.IsUnlocked)
            {
                isBuyable = TotalGold >= resource.GetUpgradeCost ();
            }
            else
            {
                isBuyable = TotalGold >= resource.GetUnlockCost ();
            }

            resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];
        }
    }

    // Added in "Add Unlock Resources feature"
    public void ShowNextResource ()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive (true);
                break;
            }
        }
    }
}
