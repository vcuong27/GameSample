using System;

public enum UnitType
{
    NONE = 0,
    SWORDMAN = 100,
    SWORDMAN1,
    SWORDMAN2,
    SWORDMAN10,
    ARCHER,
    CAVALRY
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
    public int ID;
    public UnitType Type;
    public UnitState State;
    public int Level;
    public int MaxLevel;
    public int Attack;
    public int Defense;
    public int HP;
    public int Speed;
}
