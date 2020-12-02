using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using WebApplication2.Entites;
using MySqlConnector;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;

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
            mySqlCommand.CommandText = "select nom,prenom,courriel,id,monnaie from Utilisateurs where courriel=@courriel AND mot_de_passe =@motPasse";
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
                utilisateur = new Utilisateur();

                utilisateur.Nom = (String) dataReader["nom"];
                //utilisateur.Prenom = (String) dataReader["prenom"];
                utilisateur.Courriel = (String) dataReader["courriel"];
                utilisateur.Id = (int) dataReader["id"];
                utilisateur.Monnaie = (String)dataReader["monnaie"];
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
            mySqlCommand.CommandText = "select nom,prenom,courriel,id, monnaie from Utilisateurs";
            mySqlCommand.CommandType = CommandType.Text;

            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            
        
            while  (dataReader.Read())
            {
               Utilisateur utilisateur = new Utilisateur()
                {
                    Nom = (String) dataReader["nom"],
                   // Prenom =  (String) dataReader["prenom"],
                    Courriel =  (String) dataReader["courriel"],
                    Id =  (int) dataReader["id"],
                    Monnaie = (string) dataReader["monnaie"]
                    
                    
                };
               utilisateurs.Add(utilisateur);
            }
            dataReader.Close();
            con.Close();

            

            return utilisateurs;
        }

        public Utilisateur CreerUtilisateur(Utilisateur u)
        {
            SqlConnection con = new SqlConnection(CONNECTION_STRING);
            SqlCommand cmd = new SqlCommand("INSERT_utilisateur", con);
            cmd.CommandType = CommandType.StoredProcedure;

            //TODO verifier que les champs ne sont pas null

            cmd.Parameters.AddWithValue("@Nom", u.Nom);
            cmd.Parameters.AddWithValue("@Courriel", u.Courriel);
            cmd.Parameters.AddWithValue("@MotPasse", u.MotDePasse);
            cmd.Parameters.AddWithValue("@Monnaie", u.Monnaie); 
            con.Open();
            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();

            }
            catch (SqlException)
            {
                return null;
            }
            con.Close();
            return SeConnecter(u.Courriel, u.MotDePasse);

        }



        public bool UtilisateurExiste(string courriel)
        {

            int nb;
            SqlConnection con = new SqlConnection(CONNECTION_STRING);
            SqlCommand cmd = new SqlCommand("Count_email", con);
            cmd.CommandType = CommandType.StoredProcedure;

            //TODO verifier que les champs ne sont pas null

            cmd.Parameters.AddWithValue("@Courriel", courriel);

            con.Open();
            try
            {
                nb = (int)cmd.ExecuteScalar();

            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message.ToString());
                return true;
            }
            con.Close();
            Debug.WriteLine(nb);
            return nb == 1 ? true : false;
        }

        //todo retourner le nouvel utilisateur si la modification a faite, sinon retour null
        public Utilisateur  MettreAJours(Utilisateur utilisateurModifie)
        {
            int nbRowsAffected;
            SqlConnection con = new SqlConnection(CONNECTION_STRING);
            SqlCommand sqlCommand = con.CreateCommand();
            sqlCommand.CommandText = "update utilisateurs set nom=@nom,courriel=@courriel,monnaie=@monnaie,mot_de_passe=@motPasse where id=@id";
            sqlCommand.CommandType = CommandType.Text;
            sqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "courriel",
                Value = utilisateurModifie.Courriel
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "motPasse",
                Value = utilisateurModifie.MotDePasse
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "nom",
                Value = utilisateurModifie.Nom
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "monnaie",
                Value = utilisateurModifie.Monnaie
            });

            sqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int32,
                ParameterName = "id",
                Value = utilisateurModifie.Id
            });

            con.Open();
            try
            {
                nbRowsAffected = (int)sqlCommand.ExecuteNonQuery();

            }
            catch (SqlException e)
            {
                Debug.WriteLine(e.Message.ToString());
                return null;
            }
            con.Close();
            Debug.WriteLine(nbRowsAffected);
            return nbRowsAffected == 1 ? SeConnecter(utilisateurModifie.Courriel, utilisateurModifie.MotDePasse) : null;
        }
    }
}