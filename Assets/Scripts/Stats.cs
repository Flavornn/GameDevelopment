using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stats", menuName ="ScriptableObjects/Stats")]
public class Stats : ScriptableObject
{
    [Header("Bullet Stats")]
    public int _bulletDamage = 10;
    public float _bulletSpeed = 5f;
    public float _bulletSize = 1f;
    public int _bounceCount = 0;

    [Header("Misc Stats")]
    public float _fireRate = 1f;
    public float _reloadTime = 2f;
    public int _maxAmmo = 5;

    [Header("Player Stats")]
    public int _health = 100;
    public float _speed = 5f;
    public float _jumpHeight = 2f;
}
