using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class WeaponButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //public int weaponID; // Or use a more complex type like an object reference
    public GameObject weaponPrefab;
    public PlaneStats bodyScriptableObject;
    public WeaponType weaponType;

    public Sprite normalSprite;
    public Sprite highlightedSprite;
    public Sprite selectedSprite;

    private Button _button;
    private Image _image;

    private bool _isSelected;

    void Start()
    {
        _image = GetComponent<Image>();
        _isSelected = false;
        SetNormalState();
    }

     public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            SetHighlightedState();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_isSelected)
        {
            SetNormalState();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SetSelectedState();
        LoadoutManager.Instance.SelectWeapon(this);
    }

    public void Deselect()
    {
        _isSelected = false;
        SetNormalState();
    }

    private void SetNormalState()
    {
        _image.sprite = normalSprite;
    }

    private void SetHighlightedState()
    {
        _image.sprite = highlightedSprite;
    }

    private void SetSelectedState()
    {
        _isSelected = true;
        _image.sprite = selectedSprite;
    }


}