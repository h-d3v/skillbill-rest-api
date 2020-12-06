using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class FacturesController : ApiController
    {


        //AJouter une facture
        [ResponseType(typeof(bool))]
        public HttpResponseMessage Post([FromBody] Facture facture)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(facture.IdGroupe, header.GetValues("api-key").First()))
                {
                    FactureDataProvider factureDataProvider = new FactureDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, factureDataProvider.EnregistrerFacture(facture));
                }
            }

            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
        }

        [ResponseType(typeof(bool))]
        public HttpResponseMessage Post(Photo photo, [FromUri] int id)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesUserGroupeUtilisateur(id, header.GetValues("api-key").First()))
                {
                    FactureDataProvider factureDataProvider = new FactureDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK, factureDataProvider.AjouterPhoto(id, photo));
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 

        }

        
        [ResponseType(typeof(bool))]
        public HttpResponseMessage Put(Facture facture)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesFacture(header.GetValues("api-key").First(), facture.Id))
                {
                    FactureDataProvider factureDataProvider = new FactureDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK,  factureDataProvider.ModifierFacture(facture));
                }
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
        }

        
        [System.Web.Http.HttpGet]
        [ResponseType(typeof(bool))]
        public HttpResponseMessage GetFacture(int id)
        {
            var header = Request.Headers;
            if (header.Contains("api-key"))
            {
                if (VerifierDroits.VerifierAccesFacture(header.GetValues("api-key").First(), id))
                {
                    FactureDataProvider factureDataProvider= new FactureDataProvider();
                    return Request.CreateResponse(HttpStatusCode.OK,factureDataProvider.TrouverFactureParId(id));
                }
            }
        
            return Request.CreateResponse(HttpStatusCode.Unauthorized); 
        }

        [System.Web.Http.Route("api/factures/photos/{id}")]
        public Photo Get(int id) //TODO verifier que l'acces a la photo appartient a un utilisateur autorisé
        {
            FactureDataProvider factureDataProvider = new FactureDataProvider();
            return factureDataProvider.TrouverPhotoParId(id);
        }
    }
}