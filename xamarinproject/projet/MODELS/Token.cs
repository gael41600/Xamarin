using System;
using System.Collections.Generic;
using System.Text;

namespace projet.MODELS
{
    public class Token
    {
        public string _access_token { get; set; }
        public string _refresh_token { get; set; }
        public int _expires_in { get; set; }
        public string _token_type { get; set; }

        public Token(string access_token, string refresh_token, int expires_in, string token_type)
        {
            _access_token = access_token;
            _refresh_token = refresh_token;
            _expires_in = expires_in;
            _token_type = token_type;
        }
    }
}
