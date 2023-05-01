using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelInfo
{
    public int meleeCount;
    public int pistolCount;
    public int tankCount;
    public int bloodCount;
}

public class GameController : MonoBehaviour
{
    public int enemiesToCall = 5;
    public float enemiesDistanceToCall = 35;
    [SerializeField] List<EnemyController> enemies = new List<EnemyController>();
    IconPosition teethIcon;
    List<GameObject> enemySpawnPoints;
    int currentLevel = 0;
    public LevelInfo[] levelInfo;
    public GameObject meleeHuntPrefab;
    public GameObject pistolHuntPrefab;
    public GameObject tankHuntPrefab;
    public GameStats gameStats;
    public UiController _ui;

    void Start()
    {
        _ui = GetComponent<UiController>();
        gameStats = new GameStats(this);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        AddIcon();
        enemySpawnPoints = GameObject.FindGameObjectsWithTag("HunterSpawnPoint").ToList<GameObject>();
        InitLevel();

        gameStats.InvokeAll();
    }

    public void ReceiveBlood()
    {
        LevelInfo info = GetCurrentLevel();
        int blood = gameStats.GetTotalBlood();

        if (blood >= info.bloodCount)
        {
            LevelUp();
        }

        gameStats.Reset();
        InitLevel();
    }

    void LevelUp()
    {
        _ui.ShowNotification("Level Up!");
        currentLevel++;
    }

    public void GameOver() {
        PlayerPrefs.SetInt("Points", gameStats.GetTotalBlood());
        SceneManager.LoadScene("GameOver");
    }

    void InitLevel()
    {
        DestroyAllEnemies();
        SpawnEnemies();
        StartRandomAttack();
    }

    private void DestroyAllEnemies()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Hunter");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }
    }

    private void AddIcon()
    {
        GameObject icon = FindObjectOfType<UiController>().AddIcon("Teeth", null, Vector3.up * 5f);
        teethIcon = icon.GetComponent<IconPosition>();
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
        StartRandomAttack();
    }

    public void AttackFinished()
    {
        attackStarted = false;
        StartRandomAttack();
    }

    private void StartRandomAttack()
    {
        CancelInvoke("StartAttack");
        Invoke(
            "StartAttack",
            Random.Range(1.5f, 3f)
        );
    }

    public void HandleIconVisibility(GameObject obj)
    {
        teethIcon.gameObject.SetActive(obj != null);
        teethIcon.ReplaceObj(obj);
    }

    LevelInfo GetCurrentLevel() => levelInfo[currentLevel];

    void SpawnEnemies()
    {
        LevelInfo info = GetCurrentLevel();
        int count = info.meleeCount + info.pistolCount + info.tankCount;
        IEnumerable<GameObject> currentPoints = enemySpawnPoints.OrderBy(item => Random.Range(0, enemySpawnPoints.Count()));

        int iPoint = 0;
        for (int i = 0; i < info.meleeCount; i++)
        {
            InstantiateEnemy(meleeHuntPrefab, currentPoints.ElementAt(iPoint));
            iPoint++;
        }

        for (int i = 0; i < info.pistolCount; i++)
        {
            InstantiateEnemy(pistolHuntPrefab, currentPoints.ElementAt(iPoint));
            iPoint++;
        }

        for (int i = 0; i < info.tankCount; i++)
        {
            InstantiateEnemy(tankHuntPrefab, currentPoints.ElementAt(iPoint));
            iPoint++;
        }
    }

    void InstantiateEnemy(GameObject prefab, GameObject point)
    {
        GameObject obj = Instantiate(prefab, point.transform.position, point.transform.rotation);
    }

}
