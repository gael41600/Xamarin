using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akavache;
using System.Reactive.Linq;
using projet.MODELS;
using projet.VIEWMODELS;
using projet.VIEWS;
using Storm.Mvvm.Forms;
using Xamarin.Forms;

namespace projet
{
    public partial class MainPage : BaseContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new VMMain();
            list.RefreshCommand = new Command(() =>
            {
                Refreshlist();
            });
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem;
        }

        private void Refreshlist()
        {
            var vmmain = (VMMain)BindingContext;
            vmmain.RefreshList();
            list.IsRefreshing = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var test = (VMMain)this.BindingContext;

            test.IsConnected = test.Trs.isConnected();
            test.NotIsConnected = !test.Trs.isConnected();

            if (test.IsConnected)
            {
                MainAddButton.IsVisible = true;
                MainProfilButton.IsVisible = true;
                MainDeconnexion.IsVisible = true;
                MainConnexion.IsVisible = false;
            }
            else
            {
                MainAddButton.IsVisible = false;
                MainProfilButton.IsVisible = false;
                MainDeconnexion.IsVisible = false;
                MainConnexion.IsVisible = true;
            }
        }
    }
}
