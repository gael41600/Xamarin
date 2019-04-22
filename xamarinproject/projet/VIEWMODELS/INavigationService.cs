using projet.MODELS;
using System;
using System.Collections.Generic;

namespace projet.VIEWMODELS
{
    public interface INavigationService
    {
        void NavigateBack();

        void NavigateToLieuPage(Dictionary<string, object> dico);

        void NavigateToConnexionPage(Dictionary<string, object> dico);

        void NavigateToCreateAccountPage(Dictionary<string, object> dico);

        void NavigateToProfilPage(Dictionary<string, object> dico);

        void NavigateToModifyProfilPage(Dictionary<string, object> dico);

        void NavigateToModifyPassPage(Dictionary<string, object> dico);

        void NavigateToDetailPageParam(Dictionary<string, object> dico,Place item,RestService Trs);

        void NavigateToCommentairePage();

        void NavigateToCommentairePageParam(Dictionary<string, object> dico);

        void NavigateToMainPage();
    }
}
