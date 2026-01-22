using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{   
    //Enemies that are currently alive
    public static List<Enemy> LiveEnemies;
    //ID and prefab for the enemies used in EnemySpawnData class
    public static Dictionary<int, GameObject> EnemyPrefabs;
    //Queues for different kind of enemies to be spawned
    public static Dictionary<int, Queue<Enemy>> EnemyObjectPools;
    
    private static bool IsInitialized;
    public static void Init()
    {
        if (!IsInitialized){
            EnemyPrefabs = new Dictionary<int, GameObject>();
            EnemyObjectPools = new Dictionary<int, Queue<Enemy>>();
            LiveEnemies = new List<Enemy>();

            //Getting all enemies within Resources folder
            EnemySpawnData[] Enemies = Resources.LoadAll<EnemySpawnData>("Enemies");

            foreach(EnemySpawnData enemy in Enemies)
            {
                EnemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
                EnemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
            } 

            IsInitialized = true;
        }
        
    }

    public static Enemy SpawnEnemy(int EnemyID)
    {
        Enemy SpawnedEnemy = null;
        
        //Checking enemy type exists
        if (EnemyPrefabs.ContainsKey(EnemyID))
        {
            Queue<Enemy> ReferencedQueue = EnemyObjectPools[EnemyID];
            //Initializing next enemy in line
            if(ReferencedQueue.Count > 0)
            {
                SpawnedEnemy = ReferencedQueue.Dequeue();
                SpawnedEnemy.Init();
            }
            else
            {
                GameObject NewEnemy = Instantiate(EnemyPrefabs[EnemyID], Vector3.zero, Quaternion.identity);
                SpawnedEnemy = NewEnemy.GetComponent<Enemy>();
                SpawnedEnemy.Init();
            }
        }
        else
        {
            return null;
        }
        return SpawnedEnemy;
    }


}
