using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    [SerializeField]
    
    private GameObject enemyPrefab = null;

    private SpawnPoint[] spawnPointArray = null;
    private List<Enemy> enemyList = null;
    private Action<int> onEnemyDie = null;


    private void Awake()
    {
        spawnPointArray 
            = GetComponentsInChildren<SpawnPoint>();
        enemyList = new List<Enemy>();
    }

    public void Init(Player player, Action<int> onEnemyDie)
    {
        this.onEnemyDie = onEnemyDie;
        StartCoroutine(CoSpawnEnemies(player));
    }


    private IEnumerator CoSpawnEnemies(Player player)
    {
        while(true)
        {
            if(enemyList.Count < 5)
            { 
            SpawnEnemy(player);
            yield return new WaitForSeconds(1f);
            }
            yield return null;
        }
    }

    private void SpawnEnemy(Player player)
    {
        //Enemy를 소환하려면
        //Prefab을 등록하고
        //GameObject를 소환한 다음에
        //Enemy 컴포넌트를 가져오고
        //Init 함수 호출해줘야 함

        int randomIndex = UnityEngine.Random.Range(0, spawnPointArray.Length);
        Vector3 spawnPosition 
            = spawnPointArray[randomIndex].transform.position;
        Quaternion spawnRotation
            = spawnPointArray[randomIndex].transform.rotation;

        GameObject go = Instantiate(
            enemyPrefab, 
            spawnPosition, 
            spawnRotation,
            this.transform);
        Enemy enemyComponent 
            = go.GetComponent<Enemy>();
        if(enemyComponent != null)
        {
            enemyComponent.Init(player, OnEnemyDie);
        }

        enemyList.Add(enemyComponent);

    }

    private void OnEnemyDie(Enemy enemy, int score)
    {
        RemoveEnemy(enemy);
        onEnemyDie?.Invoke(score);
    }

    private void RemoveEnemy(Enemy enemy)
    {
        enemyList.Remove(enemy);
    }


}
