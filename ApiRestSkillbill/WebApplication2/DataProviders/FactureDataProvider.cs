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
                    facture.PayeursEtMontant = new HashSet<UtilisateurPayeur>();
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

        public bool AjouterPhoto(int id, Photo _photo)
        {
           
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "insert into photo(id_facture, image, url) VALUES (@id,@image,@url)" ;
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("id", id);
            mySqlCommand.Parameters.AddWithValue("url", "non implémenté");
            Byte[] bytes = Convert.FromBase64String(_photo.LowResEncodeBase64);
            mySqlCommand.Parameters.Add("image", SqlDbType.VarBinary).Value= bytes;
            int i = mySqlCommand.ExecuteNonQuery();
            con.Close();

            return i==1;
        }

        public Photo TrouverPhotoParId(int id)
        {
            Photo photo = null;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select id, id_facture, image, url from photo where id =@id" ;
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("id", id);

            DbDataReader dataReader = mySqlCommand.ExecuteReader();
            if (dataReader.Read())
            {
                photo = new Photo()
                {
                    Id = (int) dataReader["id"],
                    Uri = (string) dataReader["url"],
                    IdFacture = (int) dataReader["id_facture"]
                };
                byte[] result = null;
                if (!dataReader.IsDBNull(2))
                {
                    long size = dataReader.GetBytes(2, 0, null, 0, 0); //get the length of data 
                    result = new byte[size];
                    int bufferSize = 1024;
                    long bytesRead = 0;
                    int curPos = 0;
                    while (bytesRead < size)
                    {
                        bytesRead += dataReader.GetBytes(2, curPos, result, curPos, bufferSize);
                        curPos += bufferSize;
                    }

                    string str = Convert.ToBase64String(result);

                    photo.LowResEncodeBase64 = Convert.ToBase64String(result);
                }


            }

            return photo;

        }
        

    }
}