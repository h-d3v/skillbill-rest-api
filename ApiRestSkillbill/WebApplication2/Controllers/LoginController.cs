using System.Web.Http;
using WebApplication2.DataProviders;
using WebApplication2.Entites;

namespace WebApplication2.Controllers
{
    public class LoginController : ApiController
    {

        public Utilisateur Post([FromBody] Utilisateur utilisateur)
        {
            UtilisateurDataProvider dataProvider = new UtilisateurDataProvider();
            return dataProvider.SeConnecter(utilisateur.Courriel, utilisateur.MotDePasse);
        }
    }
}
