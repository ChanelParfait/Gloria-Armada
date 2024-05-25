using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLifeBar : MonoBehaviour
{
    public PlayerPlane playerHealth;
    public GameObject heartPrefab;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private float maxHealth; 

    private void OnEnable()
    {
        PlayerPlane.OnPlayerDamage += DrawHearts;
        PlayerPlane.OnUpdateHealth += DrawHearts;
    }

    private void OnDisable()
    {
        PlayerPlane.OnPlayerDamage -= DrawHearts;
        PlayerPlane.OnUpdateHealth -= DrawHearts;
    }

    private void Start()
    {
        Debug.Log("Starting healthbar");
        maxHealth = playerHealth.maxHealth;
        Debug.Log("Player Max Health = " + maxHealth);
        DrawHearts();
    }

    public void DrawHearts(PlayerPlane plane)
    {
        Debug.Log("Draw Hearts");
        ClearHearts();
        int heartsToMake = Mathf.CeilToInt(maxHealth / 2.0f);
        Debug.Log("Hearts to make: " + heartsToMake);

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(plane.CurrentHealth - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void DrawHearts(float _maxHealth){
        this.maxHealth = _maxHealth;
        DrawHearts();
    }

    public void DrawHearts()
    {   
        Debug.Log("Draw Hearts at Full Health");
        ClearHearts();
        int heartsToMake = Mathf.CeilToInt(maxHealth / 2.0f);
        Debug.Log("Hearts to make: " + heartsToMake);

        for (int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        for (int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(maxHealth - (i * 2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    public void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);

        HealthHeart heartComponent = newHeart.GetComponent<HealthHeart>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    public void ClearHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts.Clear();
    }
}
