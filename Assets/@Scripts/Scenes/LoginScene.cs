using System;
using System.Collections;
using System.Collections.Generic;
using Clicker.Manager;
using Clicker.Utils;
using Clicker.WebPacket;
using UnityEngine;
using UnityEngine.UI;

namespace Clicker.Scene
{
    public class LoginScene : UI_Scene
    {
        [SerializeField] private Button _facebookLoginButton;
        [SerializeField] private Button _googleLoginButton;
        [SerializeField] private Button _guestLoginButton;

        public override bool Init()
        {
            if (base.Init() == false)
            {
                return false;
            }
            
            _facebookLoginButton.SafeAddButtonListener(OnClickFacebookLogin);
            _googleLoginButton.SafeAddButtonListener(OnClickGoogleLogin);
            _guestLoginButton.SafeAddButtonListener(OnClickGuestLogin);

            return true;
        }

        private void OnClickGuestLogin()
        {
            
        }

        private void OnClickGoogleLogin()
        {
            // Managers.Auth.TryGoogleLogin();
        }
        
        private async void OnClickFacebookLogin()
        {
            AuthResult result = await Managers.Auth.TryFacebookLogin();
            OnLoginSuccess(result, Define.ProviderType.Facebook);
        }

        private async void OnLoginSuccess(AuthResult result, Define.ProviderType providerType)
        {
            LoginAccountPacketReq req = new LoginAccountPacketReq()
            {
                userId = result.uniqueId,
                token = result.token,
            };

            string url = "";
            switch (providerType)
            {
                case Define.ProviderType.Guest:
                    url = "guest";
                    break;
                case Define.ProviderType.Google:
                    url = "google";
                    break;
                case Define.ProviderType.Facebook:
                    url = "facebook";
                    break;
            }
            
            var response = await Managers.Web.SendRequest<LoginAccountPacketRes>($"api/account/login/{url}" ,req);
            if (!response.success)
            {
                LogUtils.LogError("Failed login facebook");
                return;
            }

            Managers.Jwt = response.jwt;
            UpdateRanking();
            // Managers.Scene.LoadScene(Define.EScene.GameScene);
        }

        private async void UpdateRanking()
        {
            try
            {
                UpdateRankingPacketReq rankingPacketReq = new UpdateRankingPacketReq()
                {
                    jwt = Managers.Jwt,
                    score = 100
                };
            
                var rankingResponse = await Managers.Web.SendRequest<UpdateRankingPacketRes>($"api/ranking/update", rankingPacketReq);
            }
            catch (Exception e)
            {
                
            }
        }

        private async void GetRanking()
        {
            GetRankersPacketReq req = new GetRankersPacketReq();
            var res = await Managers.Web.SendRequest<GetRankersPacketRes>($"api/ranking/getrankers", req);
            
        }
    }
}