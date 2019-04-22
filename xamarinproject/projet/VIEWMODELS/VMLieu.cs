using System;
using System.Collections.Generic;
using System.Text;
using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using Xamarin.Forms;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Media;
using System.Diagnostics;
using System.IO;

namespace projet.VIEWMODELS
{
    class VMLieu : ViewModelBase
    {

        private string _nomLieu;
        private string _descriptionLieu;

        private string _latitude;
        private string _longitude;

        private Plugin.Media.Abstractions.MediaFile myFile;

        

        public Command NavigateToMainCommand { get; private set; }
        public Command NavigateBackCommand { get; private set; }
        public Command TakeAPhoto { get; private set; }
        public Command TakeAnImage { get; private set; }

        //Propriétés 

        public string Nom
        {
            get => _nomLieu;
            set => SetProperty(ref _nomLieu, value);
        }

        public string Description
        {
            get => _descriptionLieu;
            set => SetProperty(ref _descriptionLieu, value);
        }

        public string Latitude
        {
            get => _latitude;
            set => SetProperty(ref _latitude, value);
        }

        public string Longitude
        {
            get => _longitude;
            set => SetProperty(ref _longitude, value);
        }

        private RestService _trs;
        [NavigationParameter("restService")]
        public RestService Trs
        {
            get => _trs;
            set => SetProperty(ref _trs, value);
        }

        public VMLieu()
        {

            NavigateToMainCommand = new Command(async () => {

                if (Nom != null && Latitude != null && Longitude != null && Description != null) //&& (myFile != null || mylink != null)
                {
                    try
                    {
                        int imageid=-1;
                        //Recup ID
                        if(myFile == null)
                        {
                            //Upload from link
                        }
                        else
                        {
                            MemoryStream memoryStream = new MemoryStream();
                            myFile.GetStream().CopyTo(memoryStream);
                            myFile.Dispose();
                            byte[] pictureArray =  memoryStream.ToArray();

                            imageid = await Trs.PostImage(pictureArray);
                        }

                        PlaceImage lieu = new PlaceImage(-1,Nom, Description, imageid, float.Parse(Latitude), float.Parse(Longitude));
                        await Trs.PostPlace(lieu);
                    }
                    catch (Exception e)
                    { //Erreur de parsing sur le float
                        //Afficher msg sur le float ??
                        Debug.WriteLine("ICI : " + e.Message);
                    }

                    var navigationService = new NavigationService();
                    navigationService.NavigateBack();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Il faut remplir tous les champs, et prendre une photo/image", "Erreur de fromulaire", "OK");
                }
            });

            TakeAPhoto = new Command(async () =>
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("No Camera", ":( No camera available.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    Name = DateTime.Now.ToShortTimeString() + ".jpg",
                    CustomPhotoSize = 75,
                    CompressionQuality = 30
                });

                if(file != null)
                {
                    myFile = file;
                }
            });

            TakeAnImage = new Command(async () =>
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var file = await CrossMedia.Current.PickPhotoAsync();

                    if(file != null)
                    {
                        myFile = file;
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Photo picking unsupported", "Terrible erreur", "Continue");
                }


                
            });


            NavigateBackCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateBack();
            });

        }

        public async Task<Position> GetCurrentLocation()
        {
            Position myPos = null;
            try
            {
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 100;
                myPos = await locator.GetLastKnownLocationAsync();

                if (myPos != null)
                {
                    return myPos;
                }
                else
                {
                    Latitude = "Objet null";
                    Console.WriteLine("Erreur ! ");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to get location : " + e.Message);
                return null;
            }
        }



        public override async Task OnResume()
        {
            Position myPos = null;
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);

                if(status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);
                    if (results.ContainsKey(Plugin.Permissions.Abstractions.Permission.Location))
                    {
                        status = results[Plugin.Permissions.Abstractions.Permission.Location];
                    }
                }


                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    myPos = await GetCurrentLocation();
                    if (myPos != null)
                    {
                        float lati = (float)myPos.Latitude;
                        float longi = (float)myPos.Longitude;

                        Latitude = lati.ToString("R");
                        Longitude = longi.ToString("R");
                    }
                    else
                    {
                        Nom = "Rambouillet";
                        Latitude = "48.65";
                        Longitude = "1.8333";


                        //Coordonnées GPS de Rambouillet
                        //Latitude: 48.65
                        //Longitude: 1.8333
                    }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Permissions non accordée", "L'application ne peut pas vous géolocaliser en raison d'une permission non accordée", "OK");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur : " + e.Message);
            }
        }

    }
}
