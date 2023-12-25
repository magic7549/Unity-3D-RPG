using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragSlot : MonoBehaviour
{
    public Slot slot;

    private Image image;

    private void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void DragSetImage(Image _itemImage)
    {
        image.sprite = _itemImage.sprite;
        SetColor(1);
    }

    public void SetColor(float _alpha)
    {
        Color color = image.color;
        color.a = _alpha;
        image.color = color;
    }
}
