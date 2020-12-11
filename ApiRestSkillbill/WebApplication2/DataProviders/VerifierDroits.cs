using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web.UI.WebControls.WebParts;

namespace WebApplication2.DataProviders
{
    public static class VerifierDroits
    {
        private static readonly  string CONNECTION_STRING = "Server=tcp:jdeinc.database.windows.net,1433;Initial Catalog=skillbilljde;Persist Security Info=False;User ID=tumbleweed;Password=lecithinedetournesole-471;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public static bool VerifierAccesUserGroupeUtilisateur(int idGroupe, string api_key)
        {
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "select id from Utilisateurs join utilisateur_groupe ug on Utilisateurs.id = ug.id_utilisateur where Api_key = @apiKey and ug.id_groupe=@idGroupe";
                sqlCommand.Parameters.AddWithValue("idGroupe", idGroupe);
                sqlCommand.Parameters.AddWithValue("apiKey", api_key);
                using (DbDataReader dataReader = sqlCommand.ExecuteReader())
                {
                    if (dataReader.HasRows) return true;
                }
                
            }
            
            return false;
        }

        public static bool VerifierAccesUtilisateur(string api_key, int idUser)
        {
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "select id from Utilisateurs  where Api_key = @apiKey";
                sqlCommand.Parameters.AddWithValue("apiKey", api_key);
                using (DbDataReader dataReader = sqlCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        return (int) dataReader["id"] == idUser;
                    }
                }
            }

            return false;
        }

        public static bool VerifierAccesFacture(string api_key, int idFacture)
        {
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                sqlConnection.Open();
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = CommandType.Text;
                sqlCommand.CommandText = "select f.id as id from Utilisateurs join utilisateur_groupe ug on Utilisateurs.id = ug.id_utilisateur join facture f on f.groupe=ug.id_groupe where Api_key = @apiKey and f.id=@id;";
                sqlCommand.Parameters.AddWithValue("apiKey", api_key);
                sqlCommand.Parameters.AddWithValue("id", idFacture);
                using (DbDataReader dataReader = sqlCommand.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        return (int) dataReader["id"] == idFacture;
                    }
                }
                
            }

            return false;
        }
    }
}