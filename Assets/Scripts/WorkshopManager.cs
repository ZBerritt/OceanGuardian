using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkshopManager : MonoBehaviour
{
    [SerializeField] private TMP_Text doubloonsText;
    [SerializeField] private Button trawlerBtn;
    [SerializeField] private Button skimmerBtn;
    [SerializeField] private Button netBtn;

    private void Update()
    {
        ReloadUI();
    }

    private void ReloadUI()
    {
        if (GameManager.Instance == null) return;

        // Doubloon Count
        doubloonsText.SetText("Doubloons: " + GameManager.Instance.doubloons);

        // Values
        int boatLevel = GameManager.Instance.boatUpgradeLevel;
        int netLevel = GameManager.Instance.boatNetLevel;

        // Boat buttons
        if (boatLevel > 0)
        {
            SetAsUnlocked(trawlerBtn);
            if (boatLevel > 1)
            {
                SetAsUnlocked(skimmerBtn);
            }
        }
        // Net button
        if (netLevel > 0) {
            SetAsUnlocked(netBtn);
        }

        // Trash buttons
    }

    // Hacky way to get the button text from a button
    private TMP_Text GetButtonText(Button button)
    {
        return button.gameObject.GetComponent<RectTransform>().GetChild(0).GetComponent<TMP_Text>();
    }

    private void SetAsUnlocked(Button button)
    {
        button.interactable = false;
        GetButtonText(button).SetText("Unlocked");
    }

    public void ToggleWorkshop()
    {
        GetComponent<Canvas>().enabled = !GetComponent<Canvas>().enabled;
    }
}
