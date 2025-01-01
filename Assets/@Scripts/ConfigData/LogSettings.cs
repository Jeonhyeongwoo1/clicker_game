using UnityEngine;

namespace Clicker.ConfigData
{
    [CreateAssetMenu(fileName = "LogSettings", menuName = "Settings/LogSettings")]
    public class LogSettings : ScriptableObject
    {
        public bool EnableLog = true; // 로그 활성화 여부
    }
}