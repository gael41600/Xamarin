using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace projet.VIEWMODELS
{
    class VMCreateAccount : ViewModelBase
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
        public Command CreateAccountCommand { get; private set; }

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

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public VMCreateAccount(){
            CreateAccountCommand = new Command(() => {
                if (Password != null && Password != "" && Prenom != null && Prenom != "" && Nom != null && Nom != "" && Email != null && Email != "")
                {
                    createAccount();
                }
            });
        }

        private async void createAccount()
        {
            await Trs.createAccount(Email, Prenom, Nom, Password);
            MainVM.Trs = Trs;
            var navigationService = new NavigationService();
            navigationService.NavigateBack();
            navigationService.NavigateBack();
        }
    }
}
