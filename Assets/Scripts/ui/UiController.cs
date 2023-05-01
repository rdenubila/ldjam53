using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

[System.Serializable]
public class Icons
{
    public string name;
    public GameObject obj;
}

[System.Serializable]
public class Notification
{
    public Text text;
    public GameObject obj;
    public float timeToHide = 5f;

    public void Show(string message)
    {
        text.text = message;
        obj.SetActive(true);
    }

    public void Hide()
    {
        obj.SetActive(false);
    }
}

[System.Serializable]
public class StatsView
{
    public Text textLabel;
    public RectTransform bar;
    public float barWidth = 250f;

    public void UpdateStats(string txt, float p)
    {
        textLabel.text = txt;
        bar.sizeDelta = new Vector2(p * barWidth, 12f);
    }
}

public class UiController : MonoBehaviour
{
    public GameObject canvas;
    public List<Icons> icons;
    public StatsView bloodView;
    public StatsView healthView;
    public Notification alert;

    void Awake()
    {
        HideNotification();

        GameStats.OnBloodCountUpdated += UpdateBloodStats;
        GameStats.OnHealthUpdated += UpdateHealthStats;
    }

    void UpdateBloodStats(int current, int total, int limit)
    {
        bloodView.UpdateStats(current + "/" + limit, (float)current / (float)limit);
    }

    void UpdateHealthStats(int current, int limit)
    {
        float p = (float)current / (float)limit;
        healthView.UpdateStats(p * 100f + "%", p);
    }

    public void ShowNotification(string message)
    {
        alert.Show(message);
        Invoke("HideNotification", alert.timeToHide);
    }

    void HideNotification()
    {
        alert.Hide();
    }

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
