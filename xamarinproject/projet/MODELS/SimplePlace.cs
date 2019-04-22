using System;
using System.Collections.Generic;
using System.Text;

namespace projet.MODELS
{
    public class SimplePlace
    {
        public int id { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }

        public List<Commentaire> comments { get; set; }
        public SimplePlace(int id, String title, String description, int image_id, float latitude, float longitude, List<Commentaire> comments)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.image = ("https://td-api.julienmialon.com/images/"+image_id);
            this.latitude = latitude;
            this.longitude = longitude;
            this.comments = comments;
        }
    }
}
