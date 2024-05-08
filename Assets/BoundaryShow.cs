using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundaryShow : MonoBehaviour
{
    Transform player;
    Camera cam;
    PlaySpaceBoundary playSpaceBoundary;
    Image image;

    [SerializeField] Vector2 playerScreenPos;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerPhys").transform;
        cam = Camera.main;
        playSpaceBoundary = cam.GetComponentInChildren<PlaySpaceBoundary>();
        image = GetComponent<Image>();
        image.CrossFadeAlpha(0, 0, true);
    }

    void playerToScreenSpace()
    {
        Vector3 screenPos = cam.WorldToScreenPoint(player.position);
        //Normalize the screenPos -1 to 1
        screenPos.x = (screenPos.x / cam.pixelWidth) * 2 - 1;
        screenPos.y = (screenPos.y / cam.pixelHeight) * 2 - 1;
        playerScreenPos = new Vector2(screenPos.x, screenPos.y);
    }

    // Update is called once per frame
    void Update()
    {
        playerToScreenSpace();
        if (Mathf.Abs(playerScreenPos.x) > 0.75 || Mathf.Abs(playerScreenPos.y) > 0.7)
        {
            float alpha = Mathf.Max(Mathf.Abs((playerScreenPos.x - 0.7f) * 4), Mathf.Abs((playerScreenPos.y) - 0.7f) * 4);
            image.CrossFadeAlpha(alpha, 0, true);
        }
        else
        {
            image.CrossFadeAlpha(0, 0.1f, true);
        }
        
    }
}
