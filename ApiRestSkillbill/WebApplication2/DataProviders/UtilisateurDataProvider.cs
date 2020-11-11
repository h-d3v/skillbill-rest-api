﻿using System;
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
        private readonly string CONNECTION_STRING =  "Server=localhost\\SQLEXPRESS;Database=skillbill;Trusted_Connection=True";
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
                utilisateur = new Utilisateur();

                utilisateur.Nom = (String) dataReader["nom"];
                //utilisateur.Prenom = (String) dataReader["prenom"];
                utilisateur.Courriel = (String) dataReader["courriel"];
                utilisateur.Id = (int) dataReader["id"];

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
                   // Prenom =  (String) dataReader["prenom"],
                    Courriel =  (String) dataReader["courriel"],
                    Id =  (int) dataReader["id"],
                    
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
            SqlCommand cmd = new SqlCommand("dbo.INSERT_utilisateur", con);
            cmd.CommandType = CommandType.StoredProcedure;

            //TODO verifier que les champs ne sont pas null

            cmd.Parameters.AddWithValue("@Nom", u.Nom);
            cmd.Parameters.AddWithValue("@Courriel", u.Courriel);
            cmd.Parameters.AddWithValue("@MotPasse", u.MotDePasse);

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
            SqlCommand cmd = new SqlCommand("dbo.Count_email", con);
            cmd.CommandType = CommandType.StoredProcedure;

            //TODO verifier que les champs ne sont pas null

            cmd.Parameters.AddWithValue("@Courriel", courriel);

            con.Open();
            try
            {
                nb = (int)cmd.ExecuteScalar();

            }
            catch (SqlException)
            {
                return true;
            }
            con.Close();
            Debug.WriteLine(nb);
            return nb == 1 ? true : false;
            
        }
    }
}