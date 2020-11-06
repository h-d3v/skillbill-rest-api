using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class RegisterController:ApiController
    {
        public Utilisateur Post(Utilisateur utilisateur)
        {
            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();
            return dataProvider.CreerUtilisateur(utilisateur);
        }
    }
}