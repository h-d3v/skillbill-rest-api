using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class NotificationsController : ApiController
    {
       
        public HttpResponseMessage Post([FromUri] int id,[FromBody] Message message)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            Groupe groupe = groupeDataProvider.TrouverGroupeAvecMembresParID(id);
            
            MessageDataProvider messageDataProvider = new MessageDataProvider();
            bool reponse = messageDataProvider.EnregistrerMessage(message, groupe);

            if (reponse)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        
        [ResponseType(typeof(List<Message>))]
        public HttpResponseMessage Get([FromUri] int id)
        {
            MessageDataProvider messageDataProvider = new MessageDataProvider();
            return Request.CreateResponse(HttpStatusCode.OK, messageDataProvider.TrouverMessageParUtilisateur(id));
        }

        public HttpResponseMessage Delete(int id)
        {
            MessageDataProvider messageDataProvider = new MessageDataProvider();
            bool reponse = messageDataProvider.SupprimerTousLesMessageParUtilisteur(id);
            if (reponse)
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            else
                return Request.CreateResponse(HttpStatusCode.BadRequest);
        }
        
    }

}