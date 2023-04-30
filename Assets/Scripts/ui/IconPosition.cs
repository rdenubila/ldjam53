using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconPosition : MonoBehaviour
{
    GameObject _obj;
    private RectTransform canvasRectTransform; // referência ao RectTransform do Canvas
    private Image image; // referência à imagem que será posicionada
    private RectTransform imageRectTransform; // referência ao RectTransform da imagem
    Vector3 _offset;


    private void Awake()
    {
        image = GetComponent<Image>();
        imageRectTransform = image.GetComponent<RectTransform>();
    }

    public void Init(GameObject obj, Transform canvas, Vector3 offset)
    {
        _obj = obj;
        _offset = offset;
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    public void ReplaceObj(GameObject obj) => _obj = obj;

    private void LateUpdate()
    {
        if (!_obj) return;

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(_obj.transform.position + _offset);
        Vector2 imagePosition = new Vector2(
            ((viewportPosition.x * canvasRectTransform.sizeDelta.x) - (canvasRectTransform.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * canvasRectTransform.sizeDelta.y) - (canvasRectTransform.sizeDelta.y * 0.5f))
        );
        imageRectTransform.anchoredPosition = imagePosition;



    }
}
