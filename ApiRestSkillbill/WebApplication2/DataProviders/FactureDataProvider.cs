using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using WebApplication2.Entites;

namespace WebApplication2.DataProviders
{
    public class FactureDataProvider
    {
        private readonly string CONNECTION_STRING = "Server=localhost\\SQLEXPRESS;Database=skillbill;Trusted_Connection=True";

        public bool EnregistrerFacture(Facture facture)
        {
            if (facture.Photos == null)
            {
                SqlConnection con =  new SqlConnection(CONNECTION_STRING);
                con.Open();
                SqlCommand mySqlCommand = con.CreateCommand();
                SqlTransaction transaction = con.BeginTransaction();
                mySqlCommand.Connection = con;
                mySqlCommand.Transaction = transaction;
                mySqlCommand.CommandText = "insert into facture (groupe, utilisateur_createur, date_facture, montant_total, nom) VALUES (@groupe, @utilisateur_createur, @date_facture, @montant_total, @nom); SELECT SCOPE_IDENTITY()";
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Parameters.Add(new SqlParameter()
                {
                    DbType = DbType.Int32,
                    ParameterName = "groupe",
                    Value = facture.IdGroupe
                });
                mySqlCommand.Parameters.AddWithValue("utilisateur_createur", facture.UtilisateurCreateurId);
                mySqlCommand.Parameters.AddWithValue("date_facture", facture.DateCreation );
                mySqlCommand.Parameters.AddWithValue("montant_total", facture.MontantTotal);
                mySqlCommand.Parameters.AddWithValue("nom", facture.Nom);

                
                var i = Convert.ToInt32( mySqlCommand.ExecuteScalar());
                if (i >= 1)
                {
                    int j = 0;
                    
                    foreach (var VARIABLE in facture.PayeursEtMontant)
                    {
                       
                        mySqlCommand.CommandText = $"insert into utilisateur_facture (id_facture, id_utilisateur, montant_paye) VALUES ({i}, @id_utilisateur{j}, @montant_paye{j}) ";
                        
                        mySqlCommand.Parameters.AddWithValue($"id_utilisateur{j}", ( VARIABLE.UtilisateurId));
                        mySqlCommand.Parameters.AddWithValue($"montant_paye{j}", VARIABLE.MontantPaye);
                        j += mySqlCommand.ExecuteNonQuery();
                        
                    }

                    if (j > 0)
                    {
                        transaction.Commit();
                        return true; 
                    }

                   
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
                con.Close();
            }
            
            return false;
        }

        public List<Facture> getFacturesParGroupe(int idGroupe)
        {
            List <Facture> factures = null;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();   
            mySqlCommand.CommandText = "select id, groupe, utilisateur_createur, date_facture, montant_total, nom, id_utilisateur, montant_paye from facture join utilisateur_facture uf on facture.id = uf.id_facture where groupe=@groupe ORDER BY id_facture";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("groupe", idGroupe);
            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            
            Facture facture = null;
            while (dataReader.Read())
            {
                if (factures == null)
                {
                    factures = new List<Facture>();
                }
                
                Facture factureLigne = new Facture();
                
                if(facture==null) facture=new Facture();
                factureLigne.Id = (int) dataReader["id"];
                if (factureLigne.Id!=facture.Id)
                {
                    if (facture.Id > 0)
                    {
                        factures.Add(facture);
                        facture= new Facture();
                    }
                    

                    facture.Id = (int) dataReader["id"];
                    facture.DateCreation = ((DateTime) dataReader["date_facture"]).ToString();
                    facture.MontantTotal = Convert.ToSingle ( dataReader["montant_total"]);
                    facture.UtilisateurCreateurId = (int) dataReader["utilisateur_createur"];
                    facture.Nom = (string) dataReader["nom"];
                    facture.PayeursEtMontant = new List<UtilisateurPayeur>();
                    facture.PayeursEtMontant.Add(new UtilisateurPayeur()
                    {
                        UtilisateurId = (int) dataReader["id_utilisateur"],
                        MontantPaye = Convert.ToSingle( dataReader["montant_paye"])
                    });
                }
                else
                {
                    facture?.PayeursEtMontant.Add(new UtilisateurPayeur()
                    {
                        UtilisateurId = (int) dataReader["id_utilisateur"],
                        MontantPaye = Convert.ToSingle( dataReader["montant_paye"])
                    });
                }
                
            }

            factures?.Add(facture); //Ajoute la derniere facture de la boucle while


            dataReader.Close();
            con.Close();

            return factures;
        }


     /*   public bool AjouterFactureGroupe(Facture facture)
        {
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();   
            mySqlCommand.CommandText = "insert into facture(groupe, utilisateur_createur, date_facture, montant_total, nom) VALUES (@groupe, @utilisateur_createur, @date_facture, @montant_total, @nom) ";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("groupe", facture.IdGroupe);
            mySqlCommand.Parameters.AddWithValue("utilisateur_createur", facture.UtilisateurCreateur.Id);
            mySqlCommand.Parameters.AddWithValue("date_facture", DateTime.TryParse(facture.DateCreation, out DateTime dateTime));
            mySqlCommand.Parameters.AddWithValue("montant_total", facture.MontantTotal);
            mySqlCommand.Parameters.AddWithValue("nom", facture.Nom);
            
            
            
            return false;
        }
        */
    }
}