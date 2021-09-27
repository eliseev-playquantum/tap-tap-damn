using UnityEngine;
using System;

public enum e_GameMode { Classic, Hell, PvP };

[Serializable]
public class ItemClass
{
    public int id;
    public bool enable = false;
}