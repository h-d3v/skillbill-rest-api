using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using WebApplication2.DataProviders;

namespace WebApplication2.Controllers
{
    public class RegisterController:ApiController
    {
        public bool Post( string courriel,  string nom, [FromBody] string mdp)
        {
            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();
            return dataProvider.CreerUtilisateur(nom, courriel, mdp);
        }
    }
}