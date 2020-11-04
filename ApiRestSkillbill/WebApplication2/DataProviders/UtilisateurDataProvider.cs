using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using WebApplication2.Entites;
using MySqlConnector;
using System.Data.SqlClient;

namespace WebApplication2.DataProviders
{
    public class UtilisateurDataProvider
    {

        private readonly string CONNECTION_STRING = "Server=tcp:jdetest.database.windows.net,1433;Initial Catalog=jdeTest;Persist Security Info=False;User ID=jdetest;Password=Test@JDE.com;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        
        public Utilisateur SeConnecter(string courriel, string motPasse)
        {
            Utilisateur utilisateur = null;
            SqlConnection con = new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select nom,prenom,courriel,id from Utilisateurs where courriel=@courriel AND mot_de_passe =@motPasse";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "courriel",
                Value = courriel
            });
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "motPasse",
                Value = motPasse
            });
            
            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            
        
            if (dataReader.Read())
            {
                utilisateur = new Utilisateur()
                {
                    Nom = (String) dataReader["nom"],
                    Prenom =  (String) dataReader["prenom"],
                    Courriel =  (String) dataReader["courriel"],
                    Id =  (int) dataReader["id"]
                };
            }
            dataReader.Close();
            con.Close();

            return utilisateur;
        }


        public List<Utilisateur> TrouverTous()
        {
            List<Utilisateur> utilisateurs = new List<Utilisateur>();
            SqlConnection con = new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select nom,prenom,courriel,id from Utilisateurs";
            mySqlCommand.CommandType = CommandType.Text;
            
            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            
        
            while  (dataReader.Read())
            {
               Utilisateur utilisateur = new Utilisateur()
                {
                    Nom = (String) dataReader["nom"],
                    Prenom =  (String) dataReader["prenom"],
                    Courriel =  (String) dataReader["courriel"],
                    Id =  (int) dataReader["id"],
                    
                };
               utilisateurs.Add(utilisateur);
            }
            dataReader.Close();
            con.Close();

       

            return utilisateurs;
        }

        public bool CreerUtilisateur(string nom, string courriel, string motDePasse)
        {
            SqlConnection con= new SqlConnection(CONNECTION_STRING);
            SqlCommand cmd = new SqlCommand("dbo.INSERT_utilisateur", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Nom", nom);
            cmd.Parameters.AddWithValue("@Courriel", courriel);
            cmd.Parameters.AddWithValue("@MotPasse", motDePasse);

            con.Open();
            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected == 1)
                {
                    return true;
                }
            }
            catch (SqlException)
            {
                return false;
            }


                

            return false;
           
        }
    }
}