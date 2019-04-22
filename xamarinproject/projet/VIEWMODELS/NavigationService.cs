using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Permissions;
using projet.MODELS;
using projet.VIEWS;
using Storm.Mvvm.Services;
using Xamarin.Forms;
using MvvmNavigationService = Storm.Mvvm.Services.INavigationService;

namespace projet.VIEWMODELS
{
    public class NavigationService : INavigationService
    {
        MvvmNavigationService _service;

        public NavigationService()
        {
           _service= DependencyService.Get<MvvmNavigationService>();
        }

        public async void NavigateBack()
        {
            await _service.PopAsync();
        }

        public async void NavigateToCommentairePage()
        {
            await _service.PushAsync<CommentairePage>();
        }

        public async void NavigateToCommentairePageParam(Dictionary<string, object> dico)
        {
            await _service.PushAsync<CommentairePage>(dico);
        }

        public async void NavigateToCreateAccountPage(Dictionary<string, object> dico)
        {
            await _service.PushAsync<CreateAccountPage>(dico);            
        }

        public async void NavigateToConnexionPage(Dictionary<string, object> dico)
        {
            await _service.PushAsync<ConnexionPage>(dico);
        }

        public async void NavigateToProfilPage(Dictionary<string, object> dico)
        {
            await _service.PushAsync<ProfilPage>(dico);
        }

        public async void NavigateToModifyProfilPage(Dictionary<string, object> dico)
        {
            await _service.PushAsync<ModifyProfilPage>(dico);
        }

        public async void NavigateToModifyPassPage(Dictionary<string, object> dico)
        {
            await _service.PushAsync<ModifyPassPage>(dico);
        }

        public async void NavigateToDetailPageParam(Dictionary<string,object> dico,Place item,RestService Trs)
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
            
            await _service.PushAsync(new DetailPage(item,Trs), dico);
        }

        public async void NavigateToLieuPage(Dictionary<string, object> dico)
        {
            await _service.PushAsync<LieuPage>(dico);
        }

        public async void NavigateToMainPage()
        {
            await _service.PushAsync<MainPage>();
        }

        private Page GetCurrentPage()
        {
            var currentPage = Application.Current.MainPage;
            return currentPage;
        }

    }
}
