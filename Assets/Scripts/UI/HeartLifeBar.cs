using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLifeBar : MonoBehaviour
{
    public GameObject heartPrefab;
    //public PlayerLife playerLife;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private int maxHealth = 6; 


    private void OnEnable()
    {
        PlayerPlane.OnPlayerDamage += DrawHearts;
        //PlayerLife.OnPlayerHeal += DrawHearts;
    }

    private void OnDisable()
    {
        PlayerPlane.OnPlayerDamage -= DrawHearts;
        //PlayerLife.OnPlayerHeal -= DrawHearts;
    }

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts(PlayerPlane plane)
    {
        ClearHearts();
        {
            float maxLifeRemainder = maxHealth % 2;
            int heartsToMake = (int)((maxHealth * 0.5f) + maxLifeRemainder);
            for (int i = 0; i < heartsToMake; i++)
            {
                CreateEmptyHeart();
            }

            for(int i = 0; i < hearts.Count; i++)
            {
                int heartStatusRemainder = Mathf.Clamp(plane.currentHealth - (i * 2), 0, 2);
                hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
            }
        }
    }

    public void DrawHearts()
    {   
        // Draw Hearts at Full Health
        ClearHearts();
        {
            float maxLifeRemainder = maxHealth % 2;
            int heartsToMake = (int)((maxHealth * 0.5f) + maxLifeRemainder);
            for (int i = 0; i < heartsToMake; i++)
            {
                CreateEmptyHeart();
            }

            for(int i = 0; i < hearts.Count; i++)
            {
                int heartStatusRemainder = Mathf.Clamp(maxHealth - (i * 2), 0, 2);
                hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
            }
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
        hearts = new List<HealthHeart>();
    }
}
