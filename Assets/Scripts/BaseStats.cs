using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BaseStats
{
    private int health;
    private int power;
    private int speed;
    private int knockback;


    public BaseStats(int health, int power, int speed, int knockback)
    {
        SetHealth(health);
        SetPower(power);
        SetSpeed(speed);
        SetKnockback(knockback);
    }
    public void SetPower(int power)
    {
        this.power = power;
    }
    public void SetHealth(int health)
    {
        if (health <= 0)
            health = 0;

        this.health = health;
    }

    public void SetSpeed(int speed)
    {
        this.speed = speed;
    }
    public void SetKnockback(int knockback)
    {
        this.knockback = knockback;
    }
    
    public int Health => health;
    public int Power => power;
    public int Speed => speed;
    public int Knockback => knockback;
}