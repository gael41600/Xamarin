using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace projet.VIEWMODELS
{
    class VMModifyPass : ViewModelBase
    {
        public Command ModifyPassCommand { get; private set; }

        private RestService _trs;
        [NavigationParameter("restService")]
        public RestService Trs
        {
            get => this._trs;
            set => SetProperty(ref this._trs, value);
        }

        private string _ancienMDP;
        public string AncienMDP
        {
            get => _ancienMDP;
            set => SetProperty(ref _ancienMDP, value);
        }

        private string _nouveauMDP;
        public string NouveauMDP
        {
            get => _nouveauMDP;
            set => SetProperty(ref _nouveauMDP, value);
        }

        private bool _notGood = false;
        public bool NotGood
        {
            get => _notGood;
            set => SetProperty(ref _notGood, value);
        }

        public VMModifyPass()
        {
            ModifyPassCommand = new Command(() => {
                if (AncienMDP != null && AncienMDP != "" && NouveauMDP != null && NouveauMDP != "")
                {
                    PatchPass();
                }
            });
        }

        private async void PatchPass()
        {
            bool res = await Trs.PatchPass(AncienMDP, NouveauMDP);
            if (res)
            {
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
