using Plugin.Connectivity;
using Plugin.Media;
using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xamarin.Forms;

namespace projet.VIEWMODELS
{
    class VMModifyProfil : ViewModelBase
    {

        private Plugin.Media.Abstractions.MediaFile myFile;

        public Command ModifyProfilCommand { get; private set; }
        public Command TakeAPhoto { get; private set; }
        public Command TakeAnImage { get; private set; }

        public Command UploadImage { get; private set; }

        private RestService _trs;
        [NavigationParameter("restService")]
        public RestService Trs
        {
            get => _trs;
            set => SetProperty(ref _trs, value);
        }

        private Author _me;
        public Author Me
        {
            get => _me;
            set => SetProperty(ref _me, value);
        }

        private string _image;
        public string Image
        {
            get => _image;
            set
            {
                SetProperty(ref _image, value);
                try
                {
                    string[] tmp = _image.Split('/');
                    Me.image_id = int.Parse(tmp[tmp.Length - 1]);

                } catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }
        }

        private ObservableCollection<string> _imagelist = new ObservableCollection<string>();
        public ObservableCollection<string> ImageList
        {
            get => _imagelist;
            set => SetProperty(ref _imagelist, value);
        }

        public VMModifyProfil()
        {
            ModifyProfilCommand = new Command(() =>
            {
                PatchProfil();
                var navigationService = new NavigationService();
                navigationService.NavigateBack();
            });

            UploadImage = new Command(async () =>
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    MemoryStream memoryStream = new MemoryStream();
                    myFile.GetStream().CopyTo(memoryStream);
                    myFile.Dispose();
                    byte[] pictureArray = memoryStream.ToArray();

                    Me.image_id = await Trs.PostImage(pictureArray);

                    _imagelist.Add("https://td-api.julienmialon.com/images/" + Me.image_id);

                }  
                else
                {
                    await App.Current.MainPage.DisplayAlert("Vous êtes hors ligne", "Il faut être en ligne pour pouvoir uploader une image", "OK");
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
                    CustomPhotoSize=75,
                    CompressionQuality=30
                });

                if (file != null)
                {
                    myFile = file;
                    UploadImage.Execute(null);
                }
            });

            TakeAnImage = new Command(async () =>
            {
                if (CrossMedia.Current.IsPickPhotoSupported)
                {
                    var file = await CrossMedia.Current.PickPhotoAsync();

                    if (file != null)
                    {
                        myFile = file;
                        UploadImage.Execute(null);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Photo picking unsupported", "Terrible erreur", "Continue");
                }
            });
        }
    

        private async void PatchProfil()
        {

            if (CrossConnectivity.Current.IsConnected)
            {
                await Trs.PatchProfil(Me.first_name, Me.last_name, (int)Me.image_id);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Vous êtes hors ligne", "Il faut être en ligne pour pouvoir modifer le profil", "OK");
            }

        }

        public async void GetMe()
        {
            Author author = await Trs.GetMe();

            if (author != null)
            {
                Me = new Author(-1, author.first_name, author.last_name, author.email, author.image_id);
            }
            else
            {
                Me = new Author(-1, "HorsLigne", "HorsLigne", "HorsLigne@Horsligne.com", null);
                Image = null;
            }
        }

        public async void GetImages()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                _imagelist.Clear();
                string url = "https://td-api.julienmialon.com/images/";
                int id = 1;
                bool thereISMore = true;
                while (thereISMore)
                {
                    if (!await Trs.getImageById(url + id))
                    {
                        thereISMore = false;
                    }
                    else
                    {
                        _imagelist.Add(url + id);
                    }
                    id++;
                }
            }
        }
    }
}
