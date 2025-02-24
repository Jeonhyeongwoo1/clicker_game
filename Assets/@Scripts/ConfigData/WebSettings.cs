using Sirenix.OdinInspector;
using UnityEngine;

namespace Clicker.ConfigData
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WebSettings", order = 1)]
    public class WebSettings : ScriptableObject
    {
        [ShowInInspector]
        public readonly string ip;
        
        [ShowInInspector]
        public readonly string port;
    }
}