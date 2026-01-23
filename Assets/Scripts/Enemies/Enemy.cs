using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxhealth;
    public float health;
    public float speed;
    public int ID;
    public int NodeIndex;

    public void Init()
    {
        health = maxhealth;
        transform.position = GameM.NodePositions[0];
        NodeIndex = 0;
    }
}
