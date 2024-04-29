using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{
    Primary,
    Special,
    Body
}


public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager Instance;
    // Assuming you have set these via the inspector
    public Button[] primaryWeaponButtons;
    public Button[] specialWeaponButtons;
    public Button[] BodyButtons;
    public Button launchButton;

    private WeaponButton selectedPrimaryWeapon;
    private WeaponButton selectedSpecialWeapon;
    private WeaponButton selectedBody;

    // [SerializeField] private int? selectedPrimaryWeapon;
    // [SerializeField] private int? selectedSpecialWeapon;


    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void SelectWeapon(WeaponButton weaponButton)
    {
        if (weaponButton.weaponType.ToString().Equals("Primary"))
        {
            if (selectedPrimaryWeapon != null && selectedPrimaryWeapon != weaponButton)
            {
                selectedPrimaryWeapon.Deselect();
            }

            selectedPrimaryWeapon = weaponButton;
        }
        else if (weaponButton.weaponType.ToString().Equals("Special"))
        {
            if (selectedSpecialWeapon != null && selectedSpecialWeapon != weaponButton)
            {
                selectedSpecialWeapon.Deselect();
            }

            selectedSpecialWeapon = weaponButton;
        }
        else if (weaponButton.weaponType.ToString().Equals("Body"))
        {
            if (selectedBody != null && selectedBody != weaponButton)
            {
                selectedBody.Deselect();
            }

            selectedBody = weaponButton;
        }
        // else if (!isPrimary && selectedSpecialWeapon != null)
        // {
        //     // Update the previously selected special button to unselected state
        //     UpdateButtonVisual(selectedSpecialWeapon, false);
        // }

        // Select the new button by changing its visual state
        // UpdateButtonVisual(button, true);

        // // Save the selected button
        // if (isPrimary)
        //     selectedPrimaryWeapon = button;
        // else
        //     selectedSpecialWeapon = button;

        // // Check if both selections are made
        // if (selectedPrimaryWeapon != null && selectedSpecialWeapon != null)
        // {
        //     launchButton.interactable = true; // Enable the launch button if both selections are made
        //}
    }

    private void UpdateUI()
    {
        // Update UI elements to reflect the current selections
    }

    public void ConfirmSelections()
    {
        // if (selectedPrimaryWeapon.HasValue && selectedSpecialWeapon.HasValue)
        // {
        //    Debug.Log("Primary Weapon: " + selectedPrimaryWeapon + " Special Weapon: " + selectedSpecialWeapon);
        // }
        // else
        // {
        //     // Show an error or prompt for complete selections
        // }
    }
}