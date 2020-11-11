using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class RegisterController:ApiController
    {
        /*public Utilisateur Post(Utilisateur utilisateur)
        {
            if (utilisateur == null)
            {

            }
            
            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();
            return dataProvider.CreerUtilisateur(utilisateur);
        }*/

        [ResponseType(typeof(Utilisateur))]
        public HttpResponseMessage Post(Utilisateur user)
        {
            if(user==null || user.Courriel==null || user.Nom==null || user.MotDePasse==null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();

            Utilisateur userRetour=dataProvider.CreerUtilisateur(user);

            if (userRetour == null)
            {
                return new HttpResponseMessage(HttpStatusCode.Conflict);
            }
            return Request.CreateResponse(HttpStatusCode.Created, user);
        }

        [HttpHead]
        //verifie si le compte est existant
        public IHttpActionResult Head(string courriel)
        {
            if(courriel==null || courriel=="")
            { 
                return BadRequest(); 
            }
            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();

            bool emailExistant=dataProvider.UtilisateurExiste(courriel);

            if (emailExistant)
            {
                return Conflict();
            }

            else
            {
                return Ok();
            }
        }

    }
}