using System.Collections;
using System.Collections.Generic;
using Clicker.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
	[FormerlySerializedAs("eObjectType")]
	[Space]
	[Space]
	[Header("For Designer")]
	public Define.EObjectType ObjectType;
	public Define.ECreatureType CreatureType;
	public int DataTemplateID;
	public string Name;
}
