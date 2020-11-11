using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
        public bool Post([FromUri] int id, [FromBody] Groupe groupe)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.CreerGroupe(groupe.Nom, id, groupe.Monnaie);
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
