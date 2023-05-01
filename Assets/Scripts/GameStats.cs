using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStats
{

    GameController _gameController;
    public GameStats(GameController gameController)
    {
        _gameController = gameController;
    }

    int totalBloodCount = 0;
    int currentBloodCount = 0;
    int bloodRate = 1;
    int bloodStockLimit = 10;

    int health = 5;
    int healthLimit = 5;


    public void Reset()
    {
        currentBloodCount = 0;
        health = healthLimit;

        InvokeAll();
    }

    public void AddBlood()
    {
        if (currentBloodCount < bloodStockLimit)
        {
            totalBloodCount++;
            currentBloodCount++;
            InvokeBloodUpdate();
        }
        else
        {
            _gameController._ui.ShowNotification("You're full of blood. Deliver it for your boss!");
        }
    }

    public void TakeDamage()
    {
        health--;
        InvokeHealthUpdate();
    }

    public void InvokeBloodUpdate() => OnBloodCountUpdated.Invoke(currentBloodCount, totalBloodCount, bloodStockLimit);
    public void InvokeHealthUpdate() => OnHealthUpdated.Invoke(health, healthLimit);

    public void InvokeAll()
    {
        InvokeBloodUpdate();
        InvokeHealthUpdate();
    }

    public int GetCurrentBlood() => currentBloodCount;
    public int GetBloodRate() => bloodRate;


    public delegate void BloodCountUpdated(int current, int total, int limit);
    public static event BloodCountUpdated OnBloodCountUpdated;

    public delegate void HealthUpdated(int current, int limit);
    public static event HealthUpdated OnHealthUpdated;
}
