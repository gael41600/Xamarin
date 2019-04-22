using projet.VIEWMODELS;
using System;
using System.Collections.Generic;
using Storm.Mvvm.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using System.Diagnostics;
using projet.MODELS;

namespace projet.VIEWS
{
    public partial class DetailPage : TabbedPage
    {
        
        public DetailPage(Place item,RestService Trs)
        {
            InitializeComponent();
            BindingContext = new VMDetail();

            if (!Trs.isConnected())
            {
                DetailCommentaireButton.IsEnabled = false;
                DetailCommentaireButton.Text = "Vous devez être connecté pour ça";
            }

            try
            {
                Position mypos = new Position(item.latitude, item.longitude);
                mymap.MoveToRegion(MapSpan.FromCenterAndRadius(mypos, Distance.FromKilometers(5)));
                var pin = new Pin()
                {
                    Type = PinType.Place,
                    Position = mypos,
                    Label = item.title
                };
                mymap.Pins.Add(pin);
            }
            catch (Exception e)
            {
                Debug.WriteLine("-----------------|||"+e.Message);
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var test = (VMDetail)this.BindingContext;
            test.GetSimplePlace();
        }
    }
}
