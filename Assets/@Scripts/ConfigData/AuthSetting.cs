using Sirenix.OdinInspector;
using UnityEngine;

namespace Clicker.ConfigData
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AuthSetting", order = 1)]
    public class AuthSetting : ScriptableObject
    {
        [ShowInInspector]
        public readonly string _webClientId;
        
        [ShowInInspector]
        public readonly string FACEBOOK_APPID = "600688952563620";
    }
}