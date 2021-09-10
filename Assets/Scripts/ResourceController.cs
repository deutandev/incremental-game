using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResourceController : MonoBehaviour
{
    // Added in "Create Upgrade Button"
    public Button ResourceButton;
    public Image ResourceImage;

    public Text ResourceDescription;
    public Text ResourceUpgradeCost;
    public Text ResourceUnlockCost;

    private ResourceConfig _config;

    private int _level = 1;

    public void SetConfig (ResourceConfig config)
    {
        _config = config;

        // ToString("0") used to remove zeros after comma
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
        ResourceUnlockCost.text = $"Unlock Cost\n{ _config.UnlockCost }";
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";
    }

    public double GetOutput()
    {
        return _config.Output*_level;
    }

    public double GetUpgradeCost()
    {
        return _config.UnlockCost;
    }

    // Added in "Create Upgrade Button"
    public double GetUnlockCost ()
    {
        return _config.UnlockCost;
    }

    // Added in "Create Upgrade Button"
    public void UpgradeLevel ()
    {
        double upgradeCost = GetUpgradeCost ();
        if (GameManager.Instance.TotalGold < upgradeCost)
        {
            return;
        }

        GameManager.Instance.AddGold (-upgradeCost);
        _level++;
        ResourceUpgradeCost.text = $"Upgrade Cost\n{ GetUpgradeCost () }";
        ResourceDescription.text = $"{ _config.Name } Lv. { _level }\n+{ GetOutput ().ToString ("0") }";
    }

    // Start is called before the first frame update
    void Start()
    {
        // Added in "Create Upgrade Button"
        ResourceButton.onClick.AddListener (UpgradeLevel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
