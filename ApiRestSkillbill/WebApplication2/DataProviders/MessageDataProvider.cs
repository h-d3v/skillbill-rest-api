using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Antlr.Runtime;
using WebApplication2.Entites;

namespace WebApplication2.DataProviders
{
    public class MessageDataProvider
    {
        private readonly string CONNECTION_STRING = "Server=localhost\\SQLEXPRESS;Database=skillbill;Trusted_Connection=True";
        //private readonly string CONNECTION_STRING = "Server=tcp:jdeinc.database.windows.net,1433;Initial Catalog=skillbilljde;Persist Security Info=False;User ID=tumbleweed;Password=lecithinedetournesole-471;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        /*
         * Le json est une solution provisoire TODO mettre les vrais champs de la base de donnée de chaques msgJSON 
         */
        
        
        public bool EnregistrerMessage(Message message, Groupe groupe)
        {
            SqlConnection con = new SqlConnection(CONNECTION_STRING);
            SqlCommand sqlCommand = con.CreateCommand();
            sqlCommand.CommandText = "insert into message (typemessage, msgjson) VALUES (@type, @json); SELECT SCOPE_IDENTITY()";
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.AddWithValue("type", message.Typemessage);
            sqlCommand.Parameters.AddWithValue("json", message.MsgJson);
            con.Open();
            int id = Convert.ToInt32( sqlCommand.ExecuteScalar());
            sqlCommand.Parameters.AddWithValue("id", id);
            int nbRow = 0;
            
            
            foreach (var utilisateur in groupe.UtilisateursAbonnes)
            {
                if(message.Expediteur.Id==utilisateur.Id)continue;
                sqlCommand.CommandText = $"INSERT into Message_utilisateur(id_message, id_utilisateur) VALUES(@id, {utilisateur.Id})";
                try
                {
                    nbRow+=sqlCommand.ExecuteNonQuery();
                }
                catch (SqlException e)
                {
                   
                    return false;
                }
            }
            con.Close();
            return nbRow > 0;
        }

        
        public List<Message> TrouverMessageParUtilisateur(int id)
        {
            List<Message> messages = new List<Message>();
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "SELECT Typemessage, Msgjson from message join message_utilisateur mu on mu.id_message=message.id where mu.id_utilisateur=@id ";
                sqlCommand.Parameters.AddWithValue("id", id);
                sqlCommand.CommandType = CommandType.Text;
                sqlConnection.Open();
                using (DbDataReader dataReader = sqlCommand.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Message message = new Message();
                        Enum.TryParse((string) dataReader["Typemessage"], out Message.TypeMsg typeMsg);
                        message.Typemessage = typeMsg;
                        message.MsgJson = (string) dataReader["Msgjson"];
                        messages.Add(message);
                    }
                    
                }
            }

            return messages;
        }


        public bool SupprimerTousLesMessageParUtilisteur(int idUtilisateur)
        {
            using (SqlConnection sqlConnection = new SqlConnection(CONNECTION_STRING))
            {
                SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = "DELETE FROM message_utilisateur where id_utilisateur=@idUtilisateur";
                sqlCommand.Parameters.AddWithValue("idUtilisateur", idUtilisateur);
                sqlCommand.CommandType = CommandType.Text;
                sqlConnection.Open();
                int nbRow = sqlCommand.ExecuteNonQuery();
                if (nbRow > 0)
                {
                    //TODO supprimer le message de la bd si il n'a plus de FK constraint pour les utilisateurs
                    return true;
                }

                return false;

            }
        }
    }
}