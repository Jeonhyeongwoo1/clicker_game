using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Clicker.ConfigData;
using Clicker.Utils;
using Cysharp.Threading.Tasks;
using Facebook.Unity;
using UnityEngine;
using Google;

namespace Clicker.Manager
{
    public class AuthResult
    {
        public Define.ProviderType providerType;
        public string uniqueId;
        public string token;
        public bool isSuccess;
    }
    
    public class AuthManager
    {

        private Action<AuthResult> _onLoginScene;
        private GoogleSignInConfiguration configuration;
        private AuthSetting _authSetting;
        public void Initialize()
        {
            _authSetting = Managers.Resource.Load<AuthSetting>(nameof(AuthSetting));
            
            if (FB.IsInitialized == false)
            {
                FB.Init(_authSetting.FACEBOOK_APPID, onInitComplete: OnFacebookInitComplete);
            }
        }
        
        #region Facebook

        private UniTaskCompletionSource<AuthResult> _loginCts;
        private AuthResult _authResult;
        public async UniTask<AuthResult> TryFacebookLogin()
        {
            _loginCts = new UniTaskCompletionSource<AuthResult>();
            List<string> permissions = new List<string>() { "gaming_profile", "email" };
            FB.LogInWithReadPermissions(permissions, FacebookAuthCallback);

            AuthResult result = await _loginCts.Task;
            return (result);
        }

        private void OnFacebookInitComplete()
        {
            if (FB.IsInitialized == false)
            {
                return;
            }
            
            Debug.Log("OnFacebookInitComplete");
            FB.ActivateApp();
        }

        private void FacebookLogin()
        {
          
        }

        private void FacebookAuthCallback(ILoginResult result)
        {
            AccessToken token = AccessToken.CurrentAccessToken;
            AuthResult authResult = new AuthResult
            {
                providerType = Define.ProviderType.Facebook,
                uniqueId = token.UserId,
                token = token.TokenString,
                isSuccess = true
            };

            _loginCts.TrySetResult(authResult);
        }
        
        #endregion

        #region Guest

        public void TryGuestLogin(Action<AuthResult> onLoginSuccess)
        {
            _onLoginScene = onLoginSuccess;

            AuthResult result = new AuthResult()
            {
                providerType = Define.ProviderType.Guest,
                uniqueId = SystemInfo.deviceUniqueIdentifier,
                token = ""
            };
            
            _onLoginScene?.Invoke(result);
        }

        #endregion
    }
}