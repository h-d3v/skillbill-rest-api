using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using WebApplication2.Entites;
using System.Data.SqlClient;

namespace WebApplication2.DataProviders
{
    public class UtilisateurDataProvider
    {

        private readonly string CONNECTION_STRING = "Server=localhost\\SQLEXPRESS;Database=skillbill;Trusted_Connection=True";
        public Utilisateur SeConnecter(string courriel, string motPasse)
        {
            Utilisateur utilisateur = null;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
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
    }
}