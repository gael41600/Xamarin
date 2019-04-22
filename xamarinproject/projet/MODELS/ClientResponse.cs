using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace projet.MODELS
{
    class ClientResponse<Type>
    {
        [JsonProperty("data")]
        public Type data { get; set; }

        [JsonProperty("is_success")]
        public bool is_success { get; set; }

        [JsonProperty("error_code")]
        public string error_code { get; set; }

        [JsonProperty("error_message")]
        public string error_message { get; set; }

        public ClientResponse(Type data, bool is_success, string error_code, string error_message)
        {
            this.data = data;
            this.is_success = is_success;
            this.error_code = error_code;
            this.error_message = error_message;
        }
    }
}
