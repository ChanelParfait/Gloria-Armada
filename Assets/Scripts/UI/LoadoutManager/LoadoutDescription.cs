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
        descText.GetComponent<TextMeshProUGUI>().text = "Light Build <br> <br>A modified body configuration that trades some armor in favor of more precise and responsive flight maneuverability.";
    }

    public void DescHeavy()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Heavy Build <br> <br>A modified body configuration that adds additional payload to increase the jet's armor and firepower, but its weight also makes the aircraft travel slightly slower and is harder to control.";
    }

    public void DescMaster()
    {
        descText.GetComponent<TextMeshProUGUI>().text = "Master Upgrade <br> <br>For ace pilots only. The aircraft is stripped of all of its armor to greatly enhance offensive and mobility capabilities, in exchange for dying in one hit.";
    }

    public void DescNone()
    {
        descText.GetComponent<TextMeshProUGUI>().text = " ";
    }
}
