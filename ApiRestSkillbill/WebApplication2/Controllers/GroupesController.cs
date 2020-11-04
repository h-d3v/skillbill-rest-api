using System.Web.Http;
using WebApplication2.DataProviders;

namespace WebApplication2.Controllers
{
    public class GroupesController : ApiController
    {
        
        public bool Put([FromUri]int idGroupe, [FromUri] int  idUtilisateur)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.AjouterMembre(idUtilisateur, idGroupe);
        }
        
    }
}