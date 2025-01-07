using System.Collections;
using System.Collections.Generic;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    [Space]
    [Space]
    [Header("For Designer")]
    public Define.ObjectType ObjectType;
    public Define.CreatureState CreatureType;
    public int DataTemplateID;
    public string Name;
}