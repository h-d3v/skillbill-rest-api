using System.Web.Http;
using System.Web.Mvc;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class FacturesController : ApiController
    {


        public bool Post([FromBody] Facture facture)
        {
            FactureDataProvider factureDataProvider = new FactureDataProvider();
            return factureDataProvider.EnregistrerFacture(facture);
        }
    }
}