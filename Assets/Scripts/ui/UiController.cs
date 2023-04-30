using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class Icons
{
    public string name;
    public GameObject obj;
}

public class UiController : MonoBehaviour
{
    public GameObject canvas;
    public List<Icons> icons;

    public GameObject AddIcon(string name, GameObject objToFollow, Vector3 offset)
    {
        GameObject obj = Instantiate(GetIconByName(name).obj);
        obj.transform.SetParent(canvas.transform, false);

        obj.GetComponent<IconPosition>().Init(objToFollow, canvas.transform, offset);

        return obj;
    }

    Icons GetIconByName(string iconName)
    {
        return icons.Where(icon => icon.name == iconName).First();
    }
}
