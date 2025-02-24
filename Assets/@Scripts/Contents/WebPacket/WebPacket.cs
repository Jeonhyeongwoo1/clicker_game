using System;
using System.Collections.Generic;
using Clicker.Manager;
using Clicker.Utils;

namespace Clicker.WebPacket
{
    [Serializable]
    public class Response
    {
        
    }

    [Serializable]
    public class LoginAccountPacketReq
    {
        public string userId;
        public string token;
    }

    [Serializable]
    public class LoginAccountPacketRes
    {
        public Define.ProviderType providerType { get; set; }
        public bool success;
        public long accountDbId;
        public string jwt;
    }
    
    [Serializable]
    public class UpdateRankingPacketReq
    {
        public string jwt { get; set; } = string.Empty;
        public int score { get; set; }
    }

    [Serializable]
    public class UpdateRankingPacketRes
    {
        public bool success { get; set; } = false;
    }
    
    [Serializable]
    public class GetRankersPacketRes
    {
        public List<RankerData> rankers { get; set; }  = new List<RankerData>();
    }

    [Serializable]
    public class GetRankersPacketReq
    {
        public string jwt { get; set; } = string.Empty;
    }
    
    public class RankerData
    {
        public string username { get; set; }
        public int score { get; set; }
    }
}