using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using WebApplication2.Entites;

namespace WebApplication2.DataProviders
{
    public class GroupeDataProvider
    {
        private readonly string CONNECTION_STRING = "Server=localhost\\SQLEXPRESS;Database=skillbill;Trusted_Connection=True";

        public bool CreerGroupe(string nom, int idUtilisateur, int Monnaie)
        {
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "insert into groupes (nom, monnaie,utilisateur_createur,date_creation) VALUES (@nom, @monnaie, @util, getdate()); SELECT SCOPE_IDENTITY()";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "nom",
                Value = nom
            });
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int64,
                ParameterName = "util",
                Value = idUtilisateur
            });
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int16,
                ParameterName = "monnaie",
                Value = Monnaie
            });

            Object o = mySqlCommand.ExecuteScalar();
            
            if (Convert.ToInt32(o) >0)
            {
                return AjouterMembre(idUtilisateur, Convert.ToInt32(o));
            }

            con.Close();
            return false;
        }

        public bool AjouterMembre(int idUtilisateur, int idGroupe)
        {
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "insert into utilisateur_groupe(id_utilisateur, id_groupe ) values (@u , @g)";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int32,
                ParameterName = "u",
                Value = idUtilisateur
            });
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int32,
                ParameterName = "g",
                Value = idGroupe
            });
            int i =  mySqlCommand.ExecuteNonQuery();
            con.Close();
            return i == 1;
        }

        public bool ModifierNom(int idGroupe, string nom)
        {
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "Update Groupes set nom=@nom where id=@id";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int32,
                ParameterName = "id",
                Value = idGroupe
            });
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.String,
                ParameterName = "nom",
                Value = nom
            });
            int i =  mySqlCommand.ExecuteNonQuery();
            con.Close();
            return i == 1;
        }

        public Groupe TrouverGroupeParID(int idGroupe)
        {
            Groupe groupe = null;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select nom, monnaie, utilisateur_createur, date_creation, id_utilisateur, id_groupe from Groupes join utilisateur_groupe ug on Groupes.id = ug.id_groupe where id=@id  ";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.Add(new SqlParameter()
            {
                DbType = DbType.Int32,
                ParameterName = "id",
                Value = idGroupe
            });
            DbDataReader dataReader = mySqlCommand.ExecuteReader();


            while  (dataReader.Read())
            {
                if (groupe == null)
                {
                    groupe= new Groupe()
                    {
                        Id = (int) dataReader["id_groupe"],
                        Monnaie = (int) dataReader["monnaie"],
                        UtilisateurCreateur = new Utilisateur(){Id= (int) dataReader["utilisateur_createur"]},
                        DateCreation = ((DateTime) dataReader["date_creation"]).ToString(),
                        Nom = (string) dataReader["nom"],
                        UtilisateursAbonnes = new List<Utilisateur>()
                        
                    };
                }
                Utilisateur utilisateur = new Utilisateur{Id= (int) dataReader["id_utilisateur"]};
                groupe.UtilisateursAbonnes.Add(utilisateur);
                
            }
            dataReader.Close();
            con.Close();

            return groupe;
        }

    }
    
    
}