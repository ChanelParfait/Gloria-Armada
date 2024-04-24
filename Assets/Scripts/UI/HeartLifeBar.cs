using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLifeBar : MonoBehaviour
{
    public GameObject heartPrefab;
    public PlayerLife playerLife;
    List<HealthHeart> hearts = new List<HealthHeart>();

    private void OnEnable()
    {
        PlayerLife.OnPlayerDamage += DrawHearts;
        PlayerLife.OnPlayerHeal += DrawHearts;
    }

    private void OnDisable()
    {
        PlayerLife.OnPlayerDamage -= DrawHearts;
        PlayerLife.OnPlayerHeal -= DrawHearts;
    }

    private void Start()
    {
        DrawHearts();
    }

    public void DrawHearts()
    {
        ClearHearts();
        {
            float maxLifeRemainder = playerLife.maxLife % 2;
            int heartsToMake = (int)((playerLife.maxLife * 0.5f) + maxLifeRemainder);
            for (int i = 0; i < heartsToMake; i++)
            {
                CreateEmptyHeart();
            }

            for(int i = 0; i < hearts.Count; i++)
            {
                int heartStatusRemainder = (int)Mathf.Clamp(playerLife.life - (i * 2), 0, 2);
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
