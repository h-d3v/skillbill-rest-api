using System.Collections.Generic;
using System.Web.Http;
using System.Web.Mvc;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class NotificationController : ApiController
    {
        // GET
        public List<Groupe> Get(int id)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.TrouverGroupesParUtilisateur(id);
        }
    }
}