using System;
using System.Collections.Generic;
using System.Text;

namespace Assets.Script.Manager
{
    public class IBaseMessage
    {
        
    }

    [Serializable]
    public class AutenticationMessage : IBaseMessage
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class GetPlayerProfileMessage : IBaseMessage
    {
        public long playerID;
    }

  

}
