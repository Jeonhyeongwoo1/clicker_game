using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Clicker.Entity;
using Clicker.Manager;
using Clicker.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

public class GridView : MonoBehaviour
{
    [ShowInInspector]
    public Dictionary<Vector3Int, BaseObject> cellDict = new();
    
    
    private void Update()
    {
        cellDict = Managers.Map._cellDict;
    }

    private void OnDrawGizmos()
    {
        var dict = Managers.Map._cellDict;
        if (dict.Count == 0)
        {
            return;
        }
        
        foreach (var (key, value) in dict)
        {
            if (value.ObjectType == Define.EObjectType.Hero)
            {
        
                Gizmos.color = Color.yellow;        
            }
            else
            {
        
                Gizmos.color = Color.red;
            }
            Gizmos.DrawSphere(Managers.Map.CellToWorld(key), 0.5f);
        }
        
    }
}
