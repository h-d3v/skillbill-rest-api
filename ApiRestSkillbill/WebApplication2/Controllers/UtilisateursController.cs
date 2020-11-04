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
        public string Get(int id)
        {
            return "value";
        }

     
        // POST api/values
        public void Post([FromBody] string value)
        {
            
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
