using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static Queue<int> EnemyIDsToSpawn;
    //Variable to determine the continuation of the game
    public bool GameShouldEnd;

    private void Start()
    {
        EnemyIDsToSpawn = new Queue<int>();
        Spawner.Init();
        
        StartCoroutine(Gameloop());
        InvokeRepeating("SpawnTest", 0f, 1f);
    }

    void SpawnTest()
    {
        EnqueueEnemyIDsToSpawn(1);
    }

    IEnumerator Gameloop()
    {
        while (GameShouldEnd == false)
        {
            if (EnemyIDsToSpawn.Count > 0)
            {
                //Spawning Enemies
                for(int i = 0; i < EnemyIDsToSpawn.Count;i++)
                {
                    Spawner.SpawnEnemy(EnemyIDsToSpawn.Dequeue());
                }
            }

            yield return null;
        }
    }

    public static void EnqueueEnemyIDsToSpawn(int ID)
    {
        EnemyIDsToSpawn.Enqueue(ID);
    }
}
