using System;
using System.Collections.Generic;
using UnityEngine;

namespace Clicker.Entity
{
    public class PathFindingTest : MonoBehaviour
    {
        [SerializeField] private List<Vector3> path;
        private Vector3 endPos;
        private Vector3 startPos;
        
        public void SetPathList(List<Vector3> path, Vector3 startPos, Vector3 endPos)
        {
            this.path = path;
            this.endPos = endPos;
            this.startPos = startPos;
        }

        private void OnDrawGizmos()
        {
            if (path == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;
            foreach (Vector3 vector3Int in path)
            {
                Gizmos.DrawSphere(vector3Int, 0.3f);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startPos, 1f);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(endPos, 1f);
        }
    }
}