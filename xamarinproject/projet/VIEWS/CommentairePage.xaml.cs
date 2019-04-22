using projet.VIEWMODELS;
using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace projet.VIEWS
{
    public partial class CommentairePage : ContentPage
    {
        public CommentairePage()
        {
            InitializeComponent();
            BindingContext = new VMCommentaire();
        }
    }
}
