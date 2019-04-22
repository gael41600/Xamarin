using Newtonsoft.Json;

namespace projet.MODELS
{
    public class Author
    {
        private int id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email { get; set; }
        public int? image_id { get; set; }
        [JsonIgnore]
        public string image { get; set; }
        [JsonIgnore]
        public string FullName
        {
            get
            {
                return first_name + " " + last_name; 
            }
        }

        /* test */

        public Author(int id, string first_name, string last_name, string email, int? image_id) {
            this.id = id;
            this.first_name = first_name;
            this.last_name = last_name;
            this.email = email;
            this.image_id = image_id;
            if(image_id != null)
                image = "https://td-api.julienmialon.com/images/" + image_id;
            else
                image = "https://td-api.julienmialon.com/images/1";
        }
    }
}