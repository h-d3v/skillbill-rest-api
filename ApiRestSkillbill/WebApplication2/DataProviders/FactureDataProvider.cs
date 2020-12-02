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
            if (facture == null) return false;
            
                SqlConnection con =   new SqlConnection(CONNECTION_STRING);
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

                    if (j != facture.PayeursEtMontant.Count)
                    {
                        Console.WriteLine(j);
                        Console.WriteLine(facture.PayeursEtMontant.Count);
                        transaction.Rollback();
                        return false;
                    }



                    if (facture.Photos != null || facture.Photos.Count>0)
                    { //TODO
                        int k = 0;
                        mySqlCommand.Parameters.AddWithValue("url", "non implémenté");
                        foreach (var photo in facture.Photos)
                        {
                            mySqlCommand.CommandText = $"insert into photo(id_facture, image, url) VALUES ({i},@image{k},@url)" ;
                            mySqlCommand.CommandType = CommandType.Text;
                            Byte[] bytes = Convert.FromBase64String(photo.LowResEncodeBase64);
                            mySqlCommand.Parameters.AddWithValue("image" + k, bytes);
                            k += mySqlCommand.ExecuteNonQuery();
                        }
                        
                    }
                    
                }
                else
                {
                    transaction.Rollback();
                    return false;
                }
                transaction.Commit();
                con.Close();
                return true;
            
            
           // return false;
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
            mySqlCommand.CommandText = "insert into photo(id_facture, image, url) VALUES (@idf,@image,@url)" ;
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("idf", id);
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
                if (!dataReader.IsDBNull(2))//image dans la rangée
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

        public bool ModifierFacture(Facture facture)
        {
            if (facture.Id <= 0) return false;
            bool estReussi= false;
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            SqlTransaction transaction = con.BeginTransaction();
            mySqlCommand.Connection = con;
            mySqlCommand.Transaction = transaction;
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("id", facture.Id);
            try
            {
                 if (facture.Nom != null) 
                 { 
                     mySqlCommand.CommandText = "update facture set nom=@nom where id=@id ";
                   
                     mySqlCommand.Parameters.AddWithValue("nom", facture.Nom);
                     int iRow = mySqlCommand.ExecuteNonQuery();
                     estReussi = (iRow == 1);
                     if (!estReussi)
                     {
                         transaction.Rollback();
                         con.Close();
                         return false;
                     }
                 }

                 if (facture.DateCreation != null)
                 {
                     mySqlCommand.CommandText = "update facture set date_facture=@date where id=@id ";
                     mySqlCommand.Parameters.AddWithValue("date", facture.DateCreation);
                     int iRow = mySqlCommand.ExecuteNonQuery();
                     estReussi = (iRow == 1);
                     if (!estReussi)
                     {
                         transaction.Rollback();
                         con.Close();
                         return false;
                     }
                 }

                 if (facture.MontantTotal >= 0)
                 {
                     mySqlCommand.CommandText = "update facture set montant_total=@montant where id=@id ";
                     mySqlCommand.Parameters.AddWithValue("montant", facture.MontantTotal);
                     int iRow = mySqlCommand.ExecuteNonQuery();
                     estReussi = (iRow == 1);
                     if (!estReussi)
                     {
                         transaction.Rollback();
                         con.Close();
                         return false;
                     }
                 }


                 if (facture.PayeursEtMontant != null)
                 {
                     HashSet<UtilisateurPayeur> utilisateurPayeurs = facture.PayeursEtMontant;
                     int i = 555;
                     int idFacture = facture.Id;
                     int compteurRang = 0;
                     foreach (var VARIABLE in utilisateurPayeurs)
                     {
                         mySqlCommand.CommandText =
                             $"update utilisateur_facture set montant_paye = @mp{i} where id_utilisateur=@idUser{i} and id_facture = {idFacture} ";
                         mySqlCommand.Parameters.AddWithValue("idUser" + i, VARIABLE.UtilisateurId);
                         mySqlCommand.Parameters.AddWithValue("mp" + i, VARIABLE.MontantPaye);
                         int dbRow = mySqlCommand.ExecuteNonQuery();
                         compteurRang += dbRow;

                         if (dbRow == 0)
                         {
                             mySqlCommand.CommandText =
                                 $"insert into utilisateur_facture(id_facture, id_utilisateur, montant_paye) VALUES ( {idFacture}, @idUser{i}, @mp{i} ) ";
                             dbRow = mySqlCommand.ExecuteNonQuery();
                             compteurRang += dbRow;
                         }

                         i++;
                     }


                     estReussi = (compteurRang >= 1);
                     if (!estReussi)
                     {
                         transaction.Rollback();
                         con.Close();
                         return false;
                     }
                 }
            
                 transaction.Commit();
            
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
                transaction.Rollback();
            }
            finally{con.Close();}


            return estReussi;

        }


        public Facture TrouverFactureParId(int factureId)
        {
            SqlConnection con =  new SqlConnection(CONNECTION_STRING);
            con.Open();
            SqlCommand mySqlCommand = con.CreateCommand();
            mySqlCommand.CommandText = "select facture.id as idFacture , groupe, utilisateur_createur, date_facture, montant_total, nom , id_utilisateur, montant_paye from facture join utilisateur_facture uf on facture.id = uf.id_facture  where facture.id=@id";
            mySqlCommand.CommandType = CommandType.Text;
            mySqlCommand.Parameters.AddWithValue("id", factureId);
            DbDataReader dbDataReader = mySqlCommand.ExecuteReader();
            Facture facture = null;
            while (dbDataReader.Read())
            {
                if (facture == null)
                {
                    facture= new Facture();
                    facture.Id = (int) dbDataReader["idFacture"];
                    facture.UtilisateurCreateurId =(int) dbDataReader["utilisateur_createur"];
                    facture.IdGroupe = (int) dbDataReader["groupe"];
                    facture.Nom = (string) dbDataReader["nom"];
                    facture.MontantTotal = Convert.ToSingle( dbDataReader["montant_total"]);
                    facture.DateCreation = ((DateTime) dbDataReader["date_facture"]).ToString();
                    facture.Photos = new List<Photo>();
                    facture.PayeursEtMontant = new HashSet<UtilisateurPayeur>();
                }
           
                UtilisateurPayeur utilisateurPayeur = new UtilisateurPayeur();
                utilisateurPayeur.MontantPaye =Convert.ToSingle(dbDataReader["montant_paye"]);
                
                utilisateurPayeur.UtilisateurId = (int) dbDataReader["id_utilisateur"];
                facture.PayeursEtMontant.Add(utilisateurPayeur);
                
            }
            dbDataReader.Close();
            if (facture != null)
            {
                mySqlCommand = con.CreateCommand();
                mySqlCommand.CommandText = "select id, id_facture, image, url  from photo where id_facture=@id" ;
                mySqlCommand.CommandType = CommandType.Text;
                mySqlCommand.Parameters.AddWithValue("id", factureId);
                dbDataReader = mySqlCommand.ExecuteReader();
                while (dbDataReader.Read())
                {
                   Photo photo = new Photo();
                   photo.Id = (int) dbDataReader["id"];
                   photo.IdFacture = (int) dbDataReader["id_facture"];
                   photo.Uri = (string) dbDataReader["url"];
                   long size = dbDataReader.GetBytes(2, 0, null, 0, 0); 
                   var result = new byte[size];
                   int bufferSize = 1024;
                   long bytesRead = 0;
                   int curPos = 0;
                   while (bytesRead < size)
                   {
                       bytesRead += dbDataReader.GetBytes(2, curPos, result, curPos, bufferSize);
                       curPos += bufferSize;
                   }

                   string str = Convert.ToBase64String(result);
                   photo.LowResEncodeBase64 = Convert.ToBase64String(result);
                   facture.Photos.Add(photo);

                }
                dbDataReader.Close();

            }

       
            
            
            
            con.Close();
            return facture;
        }
    }
}