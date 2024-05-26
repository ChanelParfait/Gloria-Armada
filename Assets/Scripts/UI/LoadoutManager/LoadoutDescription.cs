using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadoutDescription : MonoBehaviour
{
    public GameObject descText;

    public void DescMachinegun()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Machinegun <br> <br>Classic fast-firing weapon. Hold the fire button to rapidly shoot bullets with average damage.";
    }

    public void DescScattergun()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Scattergun <br> <br>Fires 3 bullets in a moderate spread with limited range that are individually weak, but packs a punch if multiple shots connect.";
    }

    public void DescChargeCannon()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Charge Cannon <br> <br>Hold the fire button to charge your projectile, increasing the projectile's potential damage and travel speed. Maximum charge penetrates enemies.";
    }

    public void DescGuidedMissile()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Guided Missile <br> <br>Fires a high-damage rocket that move towards enemies. Can hold up to 4 charges.";
    }

    public void DescBeegBomba()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Beeg Bomba <br> <br>Fires a big and slow, unguided bomb that explodes at a certain distance or on impact, wrecking everything in its explosion. Can hold up to 3 charges.";
    }

    public void DescNRGLazer()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "N.R.G Lazer <br> <br>Hold the special button to fire a beam of energy in the jet's direction. The longer you fire it, the deadlier it gets.";
    }

    public void DescStandard()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Standard <br> <br>Original body configuration with balanced mobility and firepower. No benefits or downsides included.";
    }

    public void DescLight()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Light Build <br> <br>Removal of some armor from the jet results in reduction of its overall weight, allowing the user to pilot the vessel with improved speed, better contol and precision.";
    }

    public void DescHeavy()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Heavy Build <br> <br>Modifies the jet's equipped weapons to make them more deadly, but such upgrades require additional payload to the jet, causing the vessel to move slower.";
    }

    public void DescMaster()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Master Upgrade <br> <br>For ace pilots only. A heavily modified body configuration is applied to the jet, significantly amplifying the jet's capabilities in both mobility and firepower. However, all armor from the vessel will be removed to make space for all the upgrades, meaning a hit from any enemy weapon will result in certain death.";
    }

    public void DescNone()
    {
        descText.GetComponent<TextMeshProUGUI>().text = " ";
    }
}
