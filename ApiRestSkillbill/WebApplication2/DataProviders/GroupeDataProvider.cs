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

        public Groupe CreerGroupe(string nom, int idUtilisateur, int Monnaie)
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
                if (AjouterMembre(idUtilisateur, Convert.ToInt32(o)))
                {
                    return new Groupe(){Id = Convert.ToInt32(o), Nom = nom};
                }
               
            }

            con.Close();
            return null;
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
        
        
        public bool AjouterMembre(string courriel, int idGroupe)
        {
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select id from Utilisateurs where courriel=@courriel";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("courriel", courriel);
            DbDataReader dbDataReader = mySqlCommand.ExecuteReader();
            int idUtilisateur = 0;
            if (dbDataReader.Read())
            {
                idUtilisateur = (int) dbDataReader["id"];
                dbDataReader.Close();
            }
            else return false;

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

        public Groupe TrouverGroupeAvecMembresParID(int idGroupe)
        {
            Groupe groupe = null;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
          //  mySqlCommand.CommandText = "select nom, monnaie, utilisateur_createur, date_creation, id_utilisateur, id_groupe from Groupes join utilisateur_groupe ug on Groupes.id = ug.id_groupe where id=@id  ";

            mySqlCommand.CommandText =
              "select Groupes.nom, Groupes.monnaie, utilisateur_createur, U.nom as username, date_creation, id_utilisateur, id_groupe from Groupes join utilisateur_groupe ug on Groupes.id = ug.id_groupe join Utilisateurs U on U.id = ug.id_utilisateur where Groupes.id=@id";
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
                Groupe groupeSql = new Groupe(){Id =  (int) dataReader["id_groupe"] };
                if (groupe == null || groupe.Id!=groupeSql.Id)
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
                Utilisateur utilisateur = new Utilisateur
                {
                    Id= (int) dataReader["id_utilisateur"],
                    Nom = (String) dataReader["username"]
                };
                groupe.UtilisateursAbonnes.Add(utilisateur);
                
            }
            dataReader.Close();
            con.Close();

            return groupe;
        }

        public List<Groupe> TrouverGroupesParUtilisateur(int id)
        {
            List<Groupe> groupes = null;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select id, nom, monnaie, utilisateur_createur, date_creation, u.id_groupe, ug.id_utilisateur from Groupes join utilisateur_groupe ug on Groupes.id = ug.id_groupe join utilisateur_groupe u on Groupes.id = u.id_groupe where u.id_utilisateur=@id order by id" ;
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("id", id);
            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            Groupe groupe = new Groupe();
            groupe.Id = -1;
            while (dataReader.Read())
            {
                if(groupes==null) groupes = new List<Groupe>();
                if (groupe.Id != (int) dataReader["id"])
                {
                    if(groupe.Id!=-1) groupes.Add(groupe);
                    
                    groupe = new Groupe();
                    groupe.Id = (int) dataReader["id"];
                    groupe.Nom = (string) dataReader["nom"];
                    groupe.UtilisateurCreateur = new Utilisateur(){Id = (int) dataReader["utilisateur_createur"]}  ;
                    groupe.UtilisateursAbonnes= new List<Utilisateur>();
                    groupe.UtilisateursAbonnes.Add(new Utilisateur(){Id = (int) dataReader["id_utilisateur"]});
                    
                }
                else
                {
                    groupe.UtilisateursAbonnes.Add(new Utilisateur(){Id = (int) dataReader["id_utilisateur"]});
                }
            }
            groupes?.Add(groupe);

            return groupes;
        }
    }
    
    
}