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
        {// a suprimer juste pour visualiser les utilisateurs lors du debugage
            UtilisateurDataProvider utilisateurDataProvider = new UtilisateurDataProvider();
            return utilisateurDataProvider.TrouverTous();
        }

   
        [Route("api/utilisateurs/{id}/groupes/{idGroupe}")]
        [ResponseType(typeof(Groupe))]
        public HttpResponseMessage Get(int id, int idGroupe)
        {
            
            var request = Request;
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(idGroupe,header.GetValues("api-key").First()))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, groupeDataProvider.TrouverGroupeAvecMembresParID(idGroupe));
                }

            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
           
        }

        [Route("api/utilisateurs/{id}/groupes")]
        [ResponseType(typeof( List<Groupe>))]
        public HttpResponseMessage Get(int id)
        {
            var request = Request;
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUtilisateur(header.GetValues("api-key").First(), id))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, groupeDataProvider.TrouverGroupesParUtilisateur(id));
                }

            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
            
          
        }


        // POST créer un groupe
        [Route("api/utilisateurs/{id}/groupes")] [ResponseType((typeof(Groupe)))]
        public HttpResponseMessage Post([FromUri] int id, [FromBody] Groupe groupe)
        {
            var request = Request;
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUtilisateur(header.GetValues("api-key").First(), id))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, groupeDataProvider.CreerGroupe(groupe.Nom, id, groupe.Monnaie));
                }
            }
            
            return Request.CreateResponse(HttpStatusCode.Unauthorized);  ;
        }

     

        [Route("api/utilisateurs/modifier/{id}")]
        [HttpPut]
        [ResponseType(typeof(Utilisateur))]
        public HttpResponseMessage ModifierUnParametre([FromUri] int id, [FromBody] Utilisateur user)
        {
            var request = Request;
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUtilisateur(header.GetValues("api-key").First(), id))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    
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
                            return Request.CreateResponse(HttpStatusCode.Forbidden);
                        if (e.Message.Equals("Le mot de passe est requis"))
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                    catch (SqlException e)
                    {
                        if (e.Number == 2627) //unique constraint Key du Courriel
                            return new HttpResponseMessage(HttpStatusCode.Conflict);
                    }
                }
            }
            
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 



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
