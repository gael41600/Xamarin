using projet.VIEWMODELS;
using System;
using System.Collections.Generic;
using Storm.Mvvm.Forms;

using Xamarin.Forms;

namespace projet.VIEWS
{
    public partial class LieuPage : BaseContentPage
    {
        public LieuPage()
        {
            InitializeComponent();
            BindingContext = new VMLieu();
        }
    }
}
