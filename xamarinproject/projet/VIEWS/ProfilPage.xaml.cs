using projet.MODELS;
using projet.VIEWMODELS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace projet.VIEWS
{
	public partial class ProfilPage : ContentPage
	{
		public ProfilPage ()
		{
			InitializeComponent ();
            BindingContext = new VMProfil();
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            var test = (VMProfil)this.BindingContext;
            test.getMe();
        }
    }
}