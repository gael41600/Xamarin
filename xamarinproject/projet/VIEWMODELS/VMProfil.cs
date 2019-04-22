using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace projet.VIEWMODELS
{
    class VMProfil : ViewModelBase
    {
        public Command NavigateToModifyProfilCommand { get; private set; }

        public Command NavigateToModifyPassCommand { get; private set; }

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

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _prenom;
        public string Prenom
        {
            get => _prenom;
            set => SetProperty(ref _prenom, value);
        }

        private string _nom;
        public string Nom
        {
            get => _nom;
            set => SetProperty(ref _nom, value);
        }

        private string _image;
        public string Image
        {
            get => _image;
            set => SetProperty(ref _image, value);
        }

        public VMProfil()
        {
                NavigateToModifyProfilCommand = new Command(() => {      
                var navigationService = new NavigationService();
                navigationService.NavigateToModifyProfilPage(new Dictionary<string, object>() {
                    {"restService", Trs }
                });
               
            });

            NavigateToModifyPassCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateToModifyPassPage(new Dictionary<string, object>() {
                    {"restService", Trs }
                });
            });
        }

        public async void getMe()
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
    }
}
