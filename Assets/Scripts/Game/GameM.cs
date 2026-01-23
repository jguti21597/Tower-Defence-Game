using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Jobs;
using UnityEngine.Jobs;

public class GameM : MonoBehaviour
{
    private static Queue<int> EnemyIDsToSpawn;
    private static Queue<Enemy> EnemiesToRemove;
    //Variable to determine the continuation of the game
    public static Vector3[] NodePositions;
    public Transform NodeParent;
    public bool GameShouldEnd;

    private void Start()
    {
        EnemyIDsToSpawn = new Queue<int>();
        EnemiesToRemove = new Queue<Enemy>();
        Spawner.Init();

        NodePositions = new Vector3[NodeParent.childCount];
        for (int i=0;i < NodePositions.Length; i++)
        {
            NodePositions[i] = NodeParent.GetChild(i).position;
        }
        
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

            NativeArray<int> NodeIndices = new NativeArray<int>(Spawner.LiveEnemies.Count, Allocator.TempJob);
            NativeArray<Vector3> NodesToUse = new NativeArray<Vector3>(NodePositions, Allocator.TempJob);
            NativeArray<float> EnemySpeeds = new NativeArray<float>(Spawner.LiveEnemies.Count, Allocator.TempJob);
            TransformAccessArray EnemyAccess = new TransformAccessArray(Spawner.LiveEnemiesTransform.ToArray(), 2);

            for (int i = 0; i < Spawner.LiveEnemies.Count; i++)
            {
                EnemySpeeds[i] = Spawner.LiveEnemies[i].speed;
                NodeIndices[i] = Spawner.LiveEnemies[i].NodeIndex;
            }

            MoveEnemiesJob MoveJob = new MoveEnemiesJob
            {
                NodePositions = NodesToUse, 
                EnemySpeed = EnemySpeeds, 
                NodeIndex = NodeIndices, 
                deltaTime = Time.deltaTime
            };
            JobHandle MoveJobHandle = MoveJob.Schedule(EnemyAccess);
            MoveJobHandle.Complete();

            for (int i=0; i < Spawner.LiveEnemies.Count ; i++)
            {
                Spawner.LiveEnemies[i].NodeIndex = NodeIndices[i];
                if (Spawner.LiveEnemies[i].NodeIndex == NodePositions.Length)
                {
                    EnqueueEnemyToRemove(Spawner.LiveEnemies[i]);
                }
            }
            
            NodeIndices.Dispose();
            NodesToUse.Dispose();
            EnemySpeeds.Dispose();
            EnemyAccess.Dispose();

            if (EnemiesToRemove.Count > 0)
            {
                //Removing Enemies
                for(int i = 0; i < EnemiesToRemove.Count;i++)
                {
                    Spawner.KillEnemy(EnemiesToRemove.Dequeue());
                }
            }

            yield return null;
        }
    }

    public static void EnqueueEnemyIDsToSpawn(int ID)
    {
        EnemyIDsToSpawn.Enqueue(ID);
    }
    public static void EnqueueEnemyToRemove(Enemy EnemyToRemove)
    {
        EnemiesToRemove.Enqueue(EnemyToRemove);
    }    
}

public struct MoveEnemiesJob : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> NodePositions;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> EnemySpeed;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> NodeIndex;
    public float deltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if(NodeIndex[index] < NodePositions.Length)
        {
            Vector3 NextPosition = NodePositions[NodeIndex[index]];
            transform.position = Vector3.MoveTowards(transform.position, NextPosition, EnemySpeed[index] * deltaTime);
            if (transform.position == NextPosition)
            {
                NodeIndex[index]++;
            }
        }
        
    }
}