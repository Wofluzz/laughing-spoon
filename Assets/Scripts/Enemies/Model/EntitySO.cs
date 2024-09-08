using Inventory.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Entities/Entities")]
public class EntitySO : ScriptableObject
{
    [SerializeField] public string Name;

    [SerializeField] public Sprite EntitySprite;
    [SerializeField] public RuntimeAnimatorController EntityAnimationTree = null;
    [SerializeField] public EntityType EntityType;

    //Monster & Boss
    [SerializeField] public bool IsInvincible;
    [SerializeField] public int MaxHealth;
    [SerializeField] public float DamageToPlayer;

    //Loots
    [SerializeField] public bool IsLoots;
    [SerializeField] public List<Loots> Loots;
    [SerializeField] public LootPoolSO LootPool;
    [SerializeField] public int ID => GetInstanceID();

    //Movements
    [SerializeField] public bool IsMove;
    [SerializeField] public float MoveSpeed;
    [SerializeField] public float rotateSpeed;
    [SerializeField] public bool CanFollowPlayer;

    //AI
    [SerializeField] public float DetectionRadius;

    //Audio
    [SerializeField] public Sounds[] Sounds;
}

[Serializable]
public struct Sounds
{
    public string Name;
    public AudioClip Audio;
}

public enum EntityType
{
    Monster,
    Animals,
    Boss,
    Humans,
}