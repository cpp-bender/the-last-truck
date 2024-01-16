using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "The Last Truck/Bullet Propties", fileName = "Bullet Properties")]
public class BulletProperties : ScriptableObject
{
    [SerializeField] float lifeTime = 5f;
    [SerializeField] float speed = 80f;

    public float LifeTime { get => lifeTime; set => lifeTime = value; }
    public float Speed { get => speed; set => speed = value; }

}
