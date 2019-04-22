using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace projet.MODELS
{
    class UploadedImage
    {
        [JsonProperty]
        public int id { get; set; }

        public UploadedImage(int id)
        {
            this.id = id;
        }
    }
}
