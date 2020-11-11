using System.Collections.Generic;
using System.Web.Http;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class GroupesController : ApiController
    {
        
        public bool Post([FromUri]int id, [FromUri] int  idUtilisateur)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.AjouterMembre(idUtilisateur, id);
        }

        public bool Post([FromUri] int id, [FromUri] string courriel)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.AjouterMembre(courriel, id);
        }

        public bool Put(int id, string nom)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.ModifierNom(id, nom);
        }

        [Route("api/utilisateurs/{id}/groupes/{idGroupe}/factures")]
        public List<Facture> Get(int idGroupe)
        {
            FactureDataProvider factureDataProvider = new FactureDataProvider();
            return factureDataProvider.getFacturesParGroupe(idGroupe);
        }
    }
}