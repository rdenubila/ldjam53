using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour
{
    public int enemiesToCall = 5;
    public float enemiesDistanceToCall = 35;
    List<EnemyController> enemies = new List<EnemyController>();

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        StartRandomAttack();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void FillEnemiesList()
    {
        enemies = FindObjectsOfType<EnemyController>().ToList<EnemyController>();
    }

    public void UpdateEnemyList() => FillEnemiesList();

    public void CallNearEnemies()
    {
        FillEnemiesList();
        IEnumerable<EnemyController> enemyList = enemies
            .Where((enemy) => enemy.DistanceFromPlayer() <= enemiesDistanceToCall)
            .OrderBy((enemy) => enemy.DistanceFromPlayer())
            .Take(enemiesToCall);

        foreach (EnemyController enemy in enemyList)
        {
            enemy.SetIsAttacking(true);
        }
    }

    private bool attackStarted = false;
    public void StartAttack()
    {
        FillEnemiesList();
        IEnumerable<EnemyController> enemyList = enemies
            .Where((enemy) => enemy.IsAttacking() && enemy.health > 0);

        if (enemyList.Count() > 0)
        {
            attackStarted = true;
            EnemyController currentEnemy = enemyList.ElementAt(Random.Range(0, enemyList.Count()));
            currentEnemy.Attack();
        }
        else
        {
            StartRandomAttack();
        }
    }

    public void AttackFinished()
    {
        attackStarted = false;
        StartRandomAttack();
    }

    private void StartRandomAttack()
    {
        print("StartRandomAttack");
        Invoke(
            "StartAttack",
            Random.Range(3f, 6f)
        );
    }

}
