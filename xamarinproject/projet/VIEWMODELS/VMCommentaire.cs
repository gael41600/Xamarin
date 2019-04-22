using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace projet.VIEWMODELS
{
    class VMCommentaire : ViewModelBase
    {
        public Command NavigateToDetailCommand { get; private set; }
        public Command NavigateBackCommand { get; private set; }

        private string _texte;
        public string Texte
        {
            get { return _texte; }
            set { SetProperty(ref _texte, value); }
        }

        private Place _place;
        [NavigationParameter("item")]
        public Place Place
        {
            get => _place;
            set => SetProperty(ref _place, value);
        }

        private RestService _trs;
        [NavigationParameter("restService")]
        public RestService Trs
        {
            get => _trs;
            set => SetProperty(ref _trs, value);
        }

        public VMCommentaire()
        {
            NavigateToDetailCommand = new Command(async ()  => {
                
                if (Texte != null)
                {

                    await Trs.PostCommentaire(Place.id, Texte);

                    var navigationService = new NavigationService();
                    navigationService.NavigateBack();


                }
                else
                {
                    //Message d'avertiseement pour remplir les champs ?? 
                }
            });

            NavigateBackCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateBack();
            });
        }

    }
}