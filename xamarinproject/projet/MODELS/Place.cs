using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace projet.MODELS
{
    public class Place
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public float distance { get; set; }
        public Place(int id, string title, string description, string image, float latitude, float longitude)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.image = image;
            this.latitude = latitude;
            this.longitude = longitude;
        }
    }
}
