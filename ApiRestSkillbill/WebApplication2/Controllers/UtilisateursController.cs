using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.UI.WebControls;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class UtilisateursController : ApiController
    {
        // GET api/values
        public List<Utilisateur> Get()
        {
            UtilisateurDataProvider utilisateurDataProvider = new UtilisateurDataProvider();
            return utilisateurDataProvider.TrouverTous();
        }

   
        [Route("api/utilisateurs/{id}/groupes/{idGroupe}")]
        public Groupe Get(int id, int idGroupe)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.TrouverGroupeAvecMembresParID(idGroupe);
        }

        [Route("api/utilisateurs/{id}/groupes")]
        public List<Groupe> Get(int id)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.TrouverGroupesParUtilisateur(id);
        }


        // POST créer un groupe
        [Route("api/utilisateurs/{id}/groupes")]
        public Groupe Post([FromUri] int id, [FromBody] Groupe groupe)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.CreerGroupe(groupe.Nom, id, groupe.Monnaie);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        [Route("api/utilisateurs/modifier/{id}")]
        [HttpPut]
        [ResponseType(typeof(Utilisateur))]
        public HttpResponseMessage ModifierUnParametre([FromUri] int id, [FromBody] Utilisateur user)
        {

            try
            {
                user.Id = id;
                UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();
                Utilisateur utilisateurModif = dataProvider.MettreAJour(user);
                return Request.CreateResponse(HttpStatusCode.OK, utilisateurModif);
            }
            catch (EntityDataSourceValidationException e)
            {
                if (e.Message.Equals("Le mot de passe ne correspond pas"))
                    return Request.CreateResponse(HttpStatusCode.Unauthorized);
                if (e.Message.Equals("Le mot de passe est requis"))
                    return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            catch (SqlException e)
            {
                if (e.Number == 2627) //unique constraint Key du Courriel
                    return new HttpResponseMessage(HttpStatusCode.Conflict);
            }


            return new HttpResponseMessage(HttpStatusCode.BadRequest);



            //retourne un 409 si l'email de l'utilisateur est deja pris
            // return utilisateurModif!=null ? Request.CreateResponse(HttpStatusCode.OK, utilisateurModif) : Request.CreateResponse(HttpStatusCode.Conflict, utilisateurModif);
        }
        
        


        //cette methode retourne l'utilisateur modifier si la modification est reussie
        [Route("api/utilisateurs/update/{id}")]
        [HttpPut]
        [ResponseType(typeof(Utilisateur))]
        public HttpResponseMessage Put([FromUri] int id, [FromBody] Utilisateur user)
        {
            //todo valider avec des regex
            if (user == null || user.Courriel == null || user.Nom == null ||
                user.MotDePasse == null || user.Monnaie == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
        
            user.Id = id;
            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();
            Utilisateur utilisateurModif= dataProvider.MettreAJours(user);
           
            //retourne un 409 si l'email de l'utilisateur est deja pris
            return utilisateurModif!=null ? Request.CreateResponse(HttpStatusCode.OK, utilisateurModif) : Request.CreateResponse(HttpStatusCode.Conflict, utilisateurModif);
        }
    }
}
