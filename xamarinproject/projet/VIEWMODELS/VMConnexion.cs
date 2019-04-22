using Akavache;
using System.Reactive.Linq;
using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace projet.VIEWMODELS
{
    public class VMConnexion : ViewModelBase
    {
        private RestService _trs;
        [NavigationParameter("restService")]
        public RestService Trs
        {
            get => _trs;
            set => SetProperty(ref _trs, value);
        }

        private VMMain _mainVM;
        [NavigationParameter("MainVM")]
        public VMMain MainVM
        {
            get => _mainVM;
            set => SetProperty(ref _mainVM, value);
        }
        public Command ConnexionCommand { get; private set; }
        public Command CreateAccountCommand { get; private set; }

        private bool _notGood = false;
        public bool NotGood
        {
            get => _notGood;
            set => SetProperty(ref _notGood, value);
        }

        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public VMConnexion()
        {
            ConnexionCommand = new Command(() => {
                if (Password != null && Password != "" && Email != null && Email != "")
                {
                    connexion();   
                }
            });

            CreateAccountCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateToCreateAccountPage(new Dictionary<string, object>() {
                    { "restService", Trs },
                    { "MainVM", MainVM }
                });
            });
        }

        private async void connexion()
        {
            bool res = await Trs.connexion(Email, Password);
            if (res)
            {
                MainVM.Trs = Trs;

                await BlobCache.LocalMachine.InsertObject("refreshToken",Trs.getToken()._refresh_token);

                var navigationService = new NavigationService();
                navigationService.NavigateBack();

            }
            else
            {
                NotGood = true;
            }
        }
    }
}
