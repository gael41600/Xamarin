using Akavache;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projet.MODELS
{
    public class RestService
    {
        HttpClient client;
        Token tok;

        public RestService()
        {
            client = new HttpClient();
            //client.MaxResponseContentBufferSize = 256000;
        }

        public async Task PatchProfil(string prenom, string nom,int image_id)
        {
            var uri = new Uri(String.Format("https://td-api.julienmialon.com/me/", string.Empty));

            var json = "{\"first_name\": \"" + prenom + "\", \"last_name\": \"" + nom + "\", \"image_id\":" + image_id+ "}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, uri) { Content = content };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<bool> PatchPass(string ancienMDP, string nouveauMDP)
        {
            var uri = new Uri(String.Format("https://td-api.julienmialon.com/me/password", string.Empty));

            var json = "{\"old_password\": \"" + ancienMDP + "\", \"new_password\": \"" + nouveauMDP + "\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, uri) { Content = content };

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;
        }

        public async Task PostPlace(PlaceImage place)
        {
            var uri = new Uri(String.Format("https://td-api.julienmialon.com/places", string.Empty));
            var json = JsonConvert.SerializeObject(place);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Debug.WriteLine(response.StatusCode.ToString() + response.RequestMessage);
            }
        }



        public async Task PostCommentaire(int id, string comment)
        {
            var uri = new Uri(String.Format("https://td-api.julienmialon.com/places/" + id + "/comments", string.Empty));
            string json = "{\n\"text\":\"" + comment + "\"\n}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                string res = await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<SimplePlace> GetPlaces(int id)
        {
            var uri = new Uri(String.Format("https://td-api.julienmialon.com/places/" + id, string.Empty));
            var reponse = await client.GetAsync(uri);
            SimplePlace res = null;

            if (reponse.IsSuccessStatusCode)
            {
                var content = await reponse.Content.ReadAsStringAsync();
                res = JsonConvert.DeserializeObject<ClientResponse<SimplePlace>>(content).data;
            }
            return res;
        }

        public async Task<Author> GetMe()
        {
            Author res = null;
            if (!CrossConnectivity.Current.IsConnected)
            {
                try
                {
                    res = await BlobCache.LocalMachine.GetObject<Author>("me");
                    return res;
                }
                catch (KeyNotFoundException)
                {
                    await App.Current.MainPage.DisplayAlert("Attention", "Nous n'avons pas pu récupérer votre profil car vous êtes hors ligne", "OK");
                    return null;
                }
            }
            else
            {
                var uri = new Uri(string.Format("https://td-api.julienmialon.com/me/", string.Empty));
                var reponse = await client.GetAsync(uri);
                if (reponse.IsSuccessStatusCode)
                {
                    var content = await reponse.Content.ReadAsStringAsync();

                    res = JsonConvert.DeserializeObject<ClientResponse<Author>>(content).data;
                }
                await BlobCache.LocalMachine.InsertObject("me",res);
                return res;
            }
        }

        public async Task<List<Place>> GetPlaces()
        {
            var uri = new Uri(string.Format("https://td-api.julienmialon.com/places", string.Empty));

            var reponse = await client.GetAsync(uri);

            List<Place> res = new List<Place>();

            if (reponse.IsSuccessStatusCode)
            {
                var content = await reponse.Content.ReadAsStringAsync();
                foreach(PlaceImage place in JsonConvert.DeserializeObject<ClientResponse<List<PlaceImage>>>(content).data)
                {
                    res.Add(new Place(place.id,place.title,place.description, "https://td-api.julienmialon.com/images/" + place.image_id,place.latitude,place.longitude));
                }
            }
            return res;
        }

        public async Task<List<string>> GetAllImages()
        {
            List<string> lesImages = new List<string>();
            bool thereIsMoreImage = true;
            int imageId = 1;
            while (thereIsMoreImage)
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://td-api.julienmialon.com/images/" + imageId);
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    lesImages.Add("https://td-api.julienmialon.com/images/" + imageId);
                    imageId++;
                }
                else
                {
                    Debug.WriteLine(response.StatusCode);
                    thereIsMoreImage = false;
                }
            }
            return lesImages;
        }

        public async Task<bool> getImageById(string imageID)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, imageID);
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public async Task<int> PostImage(byte[] imageData) // Or byteArray directly
        { 

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://td-api.julienmialon.com/images");

            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Le deuxième paramètre doit absolument être "file" ici sinon ça ne fonctionnera pas
            requestContent.Add(imageContent, "file", "file.jpg");

            request.Content = requestContent;

            HttpResponseMessage response = await client.SendAsync(request);

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Image uploaded!");
                ClientResponse<UploadedImage> myObject = JsonConvert.DeserializeObject<ClientResponse<UploadedImage>>(result);
                return myObject.data.id;
            }
            else
            {
                
            }
            return -1;
        }

        public async Task createAccount(string email, string prenom, string nom, string password)
        {
            var uri = new Uri(String.Format("https://td-api.julienmialon.com/auth/register", string.Empty));
            var json = "{\"email\":\""+email+"\",\"first_name\":\""+prenom+"\",\"last_name\":\""+nom+"\",\"password\":\""+password+"\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(uri, content);
            
            if (response.IsSuccessStatusCode)
            {
                /*Réponse dans test : {"data":{"access_token":"2e797ff690f6491babc0c56c7672caa7","refresh_token":"efabfd545a3e4399b93fd614cf257356","expires_in":3599,"token_type":"Bearer"},"is_success":true,"error_code":null,"error_message":null}*/
                string res = await response.Content.ReadAsStringAsync();
                tok = JsonConvert.DeserializeObject<ClientResponse<Token>>(res).data;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok._token_type, tok._access_token);
            }
        }

        public async Task<bool> connexion(string email, string password)
        {
            var uri = new Uri(string.Format("https://td-api.julienmialon.com/auth/login", string.Empty));
            var json = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                /*Réponse dans test : {"data":{"access_token":"2e797ff690f6491babc0c56c7672caa7","refresh_token":"efabfd545a3e4399b93fd614cf257356","expires_in":3599,"token_type":"Bearer"},"is_success":true,"error_code":null,"error_message":null}*/
                string res = await response.Content.ReadAsStringAsync();
                tok = JsonConvert.DeserializeObject<ClientResponse<Token>>(res).data;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok._token_type, tok._access_token);
                return true;
            }
            return false;
        }

        public async Task<bool> RefreshAuth(string refreshtoken)
        {
            var uri = new Uri(string.Format("https://td-api.julienmialon.com/auth/refresh",string.Empty));
            var json = " {\"refresh_token\":\"" + refreshtoken + "\"}";
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(uri, content);

            if (response.IsSuccessStatusCode)
            {
                string resContent = await response.Content.ReadAsStringAsync();
                Token res = JsonConvert.DeserializeObject<ClientResponse<Token>>(resContent).data;
                tok = res;
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tok._token_type, tok._access_token);
                return true;
            }
            else
            {
                Debug.WriteLine(response.StatusCode + "\n\t"+response.ReasonPhrase);
            }

            return false;
        }

        public Token getToken()
        {
            return tok;
        }

        public bool isConnected()
        {
            if(tok != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void deconnexion()
        {
            client.DefaultRequestHeaders.Authorization = null;
            tok = null;
        }
    }
}
