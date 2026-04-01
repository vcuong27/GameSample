using System;
using UnityEngine;

public enum UnitType
{
    NONE = 0,
    SWORDMAN = 100,
    ARCHER = 200,
    CAVALRY = 300
}

public enum UnitState
{
    NONE = 0,
    TRAINING,
    IDLE,
    MOVING,
    ATTACKING
}

[Serializable]
public class IUnitData
{
    public UnitType Type;
    public int ID;
    public UnitState State;
    public int Level;
    public int MaxLevel;
    public int Attack;
    public int Defense;
    public int HP;
    public int Speed;
}
