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
	public partial class CreateAccountPage : ContentPage
	{
		public CreateAccountPage ()
		{
			InitializeComponent();
            BindingContext = new VMCreateAccount();
        }
	}
}