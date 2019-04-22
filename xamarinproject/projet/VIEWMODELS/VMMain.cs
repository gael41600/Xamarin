using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using Plugin.Permissions;
using projet.MODELS;
using Storm.Mvvm;
using Xamarin.Forms;
using System.Reactive.Linq;
using Akavache;
using Plugin.Connectivity;

namespace projet.VIEWMODELS
{
    public class VMMain : ViewModelBase
    {
        private readonly INavigationService _navigationService;

        private bool _isConnected;

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                SetProperty(ref _isConnected, value);
                OnPropertyChanged("IsConnected");
            }
        }

        private bool _notIsConnected;
        public bool NotIsConnected
        {
            get => _notIsConnected;
            set
            {
                SetProperty(ref _notIsConnected, value);
                OnPropertyChanged("NotIsConnected");
            }
        }

        private Position _currentpos;
        public Position Position
        {
            get => _currentpos;
            set => SetProperty(ref _currentpos, value);
        }


        private ObservableCollection<Place> _lieuList = new ObservableCollection<Place>();
        public ObservableCollection<Place> ListeLieux
        {
            get => _lieuList;
            set => SetProperty(ref _lieuList, value);

        }

        private RestService _trs = new RestService();
        public RestService Trs
        {
            get => _trs;
            set => SetProperty(ref _trs, value);
        }

        private Place _selectedItem;
        public Place SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                SetProperty(ref _selectedItem, value);
                if (SelectedItem != null)
                {
                    var navigationService = new NavigationService();
                    //System.Diagnostics.Debug.WriteLine(SelectedItem);
                    navigationService.NavigateToDetailPageParam(new Dictionary<string, object>()
                    {
                        {"item", SelectedItem},
                        { "restService", Trs }
                    }, SelectedItem, Trs);
                }
                _selectedItem = null;
            }
        }

        public Command PullToRefreshCommand { get; private set; }
        public Command NavigateToLieuCommand { get; private set; }
        public Command NavigateToDetailCommand { get; private set; }
        public Command NavigateToCommentaireCommand { get; private set; }
        public Command NavigateToMainCommand { get; private set; }
        public Command NavigateBackCommand { get; private set; }
        public Command NavigateToConnexion { get; private set; }
        public Command NavigateToProfilCommand { get; private set; }
        public Command MainDeconnexion { get; private set; }
        public Command ActualizePage { get; private set; }


        public VMMain()
        {
            IsConnected = Trs.isConnected();
            NotIsConnected = !Trs.isConnected();

            System.Diagnostics.Debug.WriteLine("Je suis connecté : " + IsConnected);

            NavigateToLieuCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateToLieuPage(new Dictionary<string, object>()
                {
                    {"restService",Trs }
                });
            });

            ActualizePage = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateToMainPage();
            });

            NavigateToProfilCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateToProfilPage(new Dictionary<string, object>()
                {
                    { "restService", Trs }
                });
            });

            NavigateToConnexion = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateToConnexionPage(new Dictionary<string, object>() {
                    { "restService", Trs },
                    { "MainVM", this }
                });
            });

            MainDeconnexion = new Command(async () => {

                //On vire le refreshToken du cache pour enlever la connexion automatique!
                try
                {
                    await BlobCache.LocalMachine.Invalidate("refreshToken");
                }
                catch (Exception)
                { }
                Trs.deconnexion();
                IsConnected = Trs.isConnected();
                NotIsConnected = !Trs.isConnected();
            });

            NavigateToDetailCommand = new Command(() => {
                if (SelectedItem != null)
                {
                    var navigationService = new NavigationService();
                    //System.Diagnostics.Debug.WriteLine(SelectedItem);
                    navigationService.NavigateToDetailPageParam(new Dictionary<string, object>()
                    {
                        {"item", SelectedItem},
                        { "restService", Trs }
                    }, SelectedItem, Trs);
                }
            });



            NavigateBackCommand = new Command(() => {
                var navigationService = new NavigationService();
                navigationService.NavigateBack();
            });
            GetPlaces();
        }


        public override async Task OnResume()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Plugin.Permissions.Abstractions.Permission.Location);

            if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Plugin.Permissions.Abstractions.Permission.Location);
                if (results.ContainsKey(Plugin.Permissions.Abstractions.Permission.Location))
                {
                    status = results[Plugin.Permissions.Abstractions.Permission.Location];
                }
            }

            if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
            {
                Position myPos = null;
                try
                {
                    var locator = CrossGeolocator.Current;
                    locator.DesiredAccuracy = 100;
                    myPos = await locator.GetLastKnownLocationAsync();

                    if (myPos != null)
                    {
                        Position = myPos;
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Unable to locate", "Erreur de localisation", "OK");
                    }
                }
                catch
                {
                    await App.Current.MainPage.DisplayAlert("Unable to locate", "Erreur de localisation", "OK");
                }

            }
                RefreshList();

            //CONNEXION AUTOMATIQUE
            if (!Trs.isConnected())
            {
                try
                {
                    string refreshtok = await BlobCache.LocalMachine.GetObject<string>("refreshToken");

                    if (await Trs.RefreshAuth(refreshtok))
                    {
                        System.Diagnostics.Debug.WriteLine("JE SUIS CONNECTE GRACE AU REFRESH TOKEN");
                        //Stockage du nouveau token, le précédent étant devenu expiré 

                        await BlobCache.LocalMachine.InsertObject("refreshToken", Trs.getToken()._refresh_token);

                        IsConnected = Trs.isConnected();
                        NotIsConnected = !IsConnected;

                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("JE NE SUIS PAS CONNECTE, QUELQUE CHOSE S'EST MAL PASSE :/");
                    }
                }
                catch (Exception e)
                {
                    //Pas de token dans le cache, on ne se connecte pas 
                    System.Diagnostics.Debug.WriteLine("ICI :: :: : :" + e.Message);
                }
            }
        }
    

        public VMMain(INavigationService navigationService)
        {
            _navigationService = navigationService;


            NavigateToCommentaireCommand = new Command(() => {
                _navigationService.NavigateToCommentairePage();
            });

            NavigateToMainCommand = new Command(() => {
                _navigationService.NavigateToMainPage();
            });

            NavigateBackCommand = new Command(() => {
                _navigationService.NavigateBack();
            });
        }

        public float CalculDistance(float lieu_Lat, float lieu_Long, double myLat, double myLong)
        {
            double myLatitude = Convert.ToDouble(Math.PI * myLat / 180);
            double myLongitude = Convert.ToDouble(Math.PI * myLong/ 180);
            double lieuLatitude = Convert.ToDouble(Math.PI * lieu_Lat / 180);
            double lieuLongitude = Convert.ToDouble(Math.PI * lieu_Long / 180);

            return (float) (6173 * Math.Acos(Math.Sin(myLatitude) * Math.Sin(lieuLatitude) + Math.Cos(myLatitude) * Math.Cos(lieuLatitude) * Math.Cos(lieuLongitude - myLongitude)));
        }

        public async void GetPlaces()
        {
            List<Place> l = new List<Place>();

            if (CrossConnectivity.Current.IsConnected)
            {
                l = await Trs.GetPlaces();
                System.Diagnostics.Debug.WriteLine("La liste est chargée depuis l'API!");
                //Puis mettre en cache
                await BlobCache.LocalMachine.InsertObject("listLieux",l);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Vous n'etes pas connecté !");
                //Recuperer la liste venant du cache 

                try
                {
                   l =  await BlobCache.LocalMachine.GetObject<List<Place>>("listLieux");
                    System.Diagnostics.Debug.WriteLine("La liste vient du cache");
                }
                catch (KeyNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("La liste n'est pas dans le cache ! ");
                }
            }
            

            if (Position == null) //La position peut valoir null au premier lancement, quand la permission de localisation n'a pas encore été autorisée ! 
            {
                foreach (var item in l)
                {
                    item.distance = -1;
                    _lieuList.Add(item);
                }
            }
            else
            {
                foreach (var item in l)
                {
                    item.distance = CalculDistance(item.latitude, item.longitude, Position.Latitude, Position.Longitude);
                    _lieuList.Add(item);
                }
            }
        }

        public void RefreshList()
        {
            ListeLieux.Clear();
            GetPlaces();
        }
    } 
}
