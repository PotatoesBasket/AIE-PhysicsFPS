using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinManager : MonoBehaviour
{
    public static WinManager current = null;

    [SerializeField] GameObject winPanel = null;
    [SerializeField] GameObject HUDPanel = null;
    [SerializeField] Text enemyCount = null;

    List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        if (current == null)
            current = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        Enemy[] es = FindObjectsOfType<Enemy>();

        foreach (Enemy e in es)
            enemies.Add(e);
    }

    private void Update()
    {
        enemyCount.text = enemies.Count.ToString();

        if (enemies.Count == 0)
        {
            GameManager.current.SetPauseState(true);
            winPanel.SetActive(true);
            HUDPanel.SetActive(false);
            enabled = false; // stop running updates on this script
        }
    }
    
    public void RemoveFromList(Enemy enemy)
    {
        enemies.Remove(enemy);
    }
}