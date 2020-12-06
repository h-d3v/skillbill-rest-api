using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Routing;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class GroupesController : ApiController
    {
        
        //Ajoute un utilisateur a un groupe avec son id
        [ResponseType(typeof(bool))]
        public HttpResponseMessage Post([FromUri] int id, [FromUri] int idUtilisateur)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(id, header.GetValues("api-key").First()))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, groupeDataProvider.AjouterMembre(idUtilisateur, id));
                }
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        //Ajoute un utilisateur a un groupe avec son courriel TODO gestion excepton quand l'utilisateur est deja dans le groupe
        [ResponseType(typeof(bool))]
        public HttpResponseMessage Post([FromUri] int id, [FromUri] string courriel)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(id, header.GetValues("api-key").First()))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, groupeDataProvider.AjouterMembre(courriel, id));
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
        }

        //Modifie le nom du groupe
        [ResponseType(typeof(bool))]
        public HttpResponseMessage Put(int id, string nom)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(id, header.GetValues("api-key").First()))
                {
                    GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK,  groupeDataProvider.ModifierNom(id, nom));
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
        }

        [Route("api/utilisateurs/{id}/groupes/{idGroupe}/factures")]
        [ResponseType(typeof(List<Facture>))]public HttpResponseMessage Get(int idGroupe)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(idGroupe, header.GetValues("api-key").First()))
                {
                    FactureDataProvider factureDataProvider = new FactureDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK,factureDataProvider.getFacturesParGroupe(idGroupe));
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
        }
    }
}