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

        // GET api/values/5
        [Route("api/utilisateurs/{id}/groupes/{idGroupe}")]
        public Groupe Get(int id, int idGroupe)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.TrouverGroupeParID(idGroupe);
        }

        // POST créer un groupe
        [Route("api/utilisateurs/{id}/groupes")]
        public bool Post([FromUri] string nom, int id, int monnaie)
        {
            GroupeDataProvider groupeDataProvider = new GroupeDataProvider();
            return groupeDataProvider.CreerGroupe(nom, id, monnaie);
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
