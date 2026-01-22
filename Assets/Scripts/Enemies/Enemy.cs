using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxhealth;
    public float health;
    public float speed;
    public int ID;

    public void Init()
    {
        health = maxhealth;
        
    }
}
