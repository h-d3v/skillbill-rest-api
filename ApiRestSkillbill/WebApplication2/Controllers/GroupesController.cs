using System.Web.Http;
using WebApplication2.DataProviders;

namespace WebApplication2.Controllers
{
    public class GroupesController : ApiController
    {
        
        public bool Post([FromUri]int id, [FromUri] int  idUtilisateur)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.AjouterMembre(idUtilisateur, id);
        }

        public bool Put(int id, string nom)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.ModifierNom(id, nom);
        }
    }
}