using Plugin.Connectivity;
using Plugin.Geolocator.Abstractions;
using projet.MODELS;
using Storm.Mvvm;
using Storm.Mvvm.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace projet.VIEWMODELS
{

    class VMDetail : ViewModelBase
    {
        public Command NavigateToCommentaireCommand { get; private set; }
        public Command NavigateToMainCommand { get; private set; }

        private Place _place;
        [NavigationParameter("item")]
        public Place Place
        {
            get => _place;
            set => SetProperty(ref _place,value);
        }


        private RestService _trs;
        [NavigationParameter("restService")]
        public RestService Trs
        {
            get => _trs;
            set => SetProperty(ref _trs, value);
        }

        private SimplePlace _simplePlace;
        public SimplePlace SimplePlace
        {
            get => _simplePlace;
            set => SetProperty(ref _simplePlace, value);
        }

        private Map _map;
        public Map MyMap
        {
            get => _map;
            set => SetProperty(ref _map, value);
        }

        private ObservableCollection<Commentaire> _avis = new ObservableCollection<Commentaire>();
        public ObservableCollection<Commentaire> Avis
        {
            get => _avis;
            set => SetProperty(ref _avis, value);
        }


        public VMDetail()
        {

            NavigateToCommentaireCommand = new Command(() =>
            {
                var navigationService = new NavigationService();
                navigationService.NavigateToCommentairePageParam(new Dictionary<string, object>()
                    {
                        {"item", Place},
                        { "restService", Trs }
                    });

            });

            NavigateToMainCommand = new Command(() =>
            {
                var navigationService = new NavigationService();
                navigationService.NavigateToMainPage();
            });
        }

        public async void GetSimplePlace()
        {

            Avis.Clear();
            if (CrossConnectivity.Current.IsConnected)
            {
                SimplePlace = await Trs.GetPlaces(Place.id);
                foreach(Commentaire com in SimplePlace.comments)
                {
                    Avis.Add(com);
                }
            }
            else
            {
                string[] tmp = Place.image.Split('/');
                int image_id = int.Parse(tmp[tmp.Length - 1]);
                SimplePlace = new SimplePlace(Place.id, Place.title, Place.description, image_id, Place.latitude, Place.longitude, null);
            }
        }
    }
}
