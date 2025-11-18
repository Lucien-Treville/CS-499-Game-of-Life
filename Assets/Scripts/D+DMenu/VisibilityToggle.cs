using UnityEngine;
using UnityEngine.UI;

public class VisibilityToggle : MonoBehaviour
{
    [SerializeField] private Toggle toggle;   // Reference to the Toggle
    [SerializeField] private GameObject panel; // Reference to the Panel you want to show/hide

    private void Awake()
    {
        // Safety check: if not assigned in Inspector, try to auto-get
        if (toggle == null)
            toggle = GetComponent<Toggle>();

        // Subscribe to the toggle event
        toggle.onValueChanged.AddListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        // Set panel active state based on toggle
        if (panel != null)
            panel.SetActive(isOn);
    }

    private void OnDestroy()
    {
        // Clean up listener when object is destroyed
        if (toggle != null)
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
    }
}