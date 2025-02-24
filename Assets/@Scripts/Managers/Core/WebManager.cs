using System;
using System.Net;
using System.Text;
using Clicker.ConfigData;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Clicker.Manager
{
    public class WebManager
    {
        public string BaseUrl { get; set; }
        
        private WebSettings _webSettings;
        public void Init()
        {
            _webSettings = Managers.Resource.Load<WebSettings>(nameof(WebSettings));
            
            IPAddress ipv4 = Util.GetIpv4Address(_webSettings.ip);
            if (ipv4 == null)
            {
                return;
            }
            
            BaseUrl = $"http://{ipv4}:{_webSettings.port}";
        }

        public async UniTask<T> SendRequest<T>(string url, object obj)
        {
            return await SendRequestAsync<T>(url, obj, "POST");
        }

        private async UniTask<T> SendRequestAsync<T>(string url, object obj, string method)
        {
            if (string.IsNullOrEmpty(url))
            {
                Init();
            }
            
            string sendUrl = $"{BaseUrl}{url}";
            using (var uwr = new UnityWebRequest(sendUrl, method))
            {    
                byte[] jsonBytes = null;
                if (obj != null)
                {
                    string json = JsonConvert.SerializeObject(obj);
                    jsonBytes = Encoding.UTF8.GetBytes(json);
                }
                uwr.uploadHandler = new UploadHandlerRaw(jsonBytes);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                Debug.Log($"{sendUrl}");
                try
                {
                    await uwr.SendWebRequest().ToUniTask();
                }
                catch (Exception e)
                {
                    LogUtils.LogError($"{nameof(SendRequestAsync)} / error : {e.Message}");
                    return default;
                }
                
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    string text = uwr.downloadHandler.text;
                    return JsonConvert.DeserializeObject<T>(text);
                }
            }

            LogUtils.LogError($"{nameof(SendRequestAsync)} / failed get res {url}");
            return default;
        }
        
    }
    
    
}