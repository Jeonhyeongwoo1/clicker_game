using Clicker.ConfigData;
using UnityEngine;

namespace Clicker.Utils
{
    public static class LogUtils
    {
        private static bool EnableLog = true;
        
        public static void Log(string message)
        {
            if (EnableLog)
            {
                Debug.Log(message);
            }
        }

        // 경고 로그 출력 함수
        public static void LogWarning(string message)
        {
            if (EnableLog)
            {
                Debug.LogWarning(message);
            }
        }

        // 오류 로그 출력 함수
        public static void LogError(string message)
        {
            if (EnableLog)
            {
                Debug.LogError(message);
            }
        }
    }

}