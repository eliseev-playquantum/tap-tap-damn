using UnityEngine;
using System;

public enum e_GameMode { Classic, Hell, PvP };

[Serializable]
public class ItemClass
{
    public int id;
    public bool enable = false;
}

[Serializable]
public class ModeClass
{
    public e_GameMode gameMode;
    public int maxlevel;
}

[Serializable]
public class PurseClass
{
    public int HardMoney;
    public int SoftMoney;
}