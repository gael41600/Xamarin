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
	public partial class ModifyProfilPage : ContentPage
	{
		public ModifyProfilPage ()
		{
			InitializeComponent ();
            BindingContext = new VMModifyProfil();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var modifyProfil = (VMModifyProfil)this.BindingContext;
            modifyProfil.GetMe();
            modifyProfil.GetImages();
        }
    }
}