using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace projet.MODELS
{
    public class PlaceImage
    {
        [JsonIgnore]
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public int image_id { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public PlaceImage(int id, string title, string description, int image_id, float latitude, float longitude)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.image_id = image_id;
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
