using System.Collections.Generic;

namespace WebApplication2.Entites
{
    public class Facture
    {
        public int Id { get; set; }
        public int UtilisateurCreateurId { get; set; }
        public string DateCreation { get; set; }
        public List<Photo> Photos;
        public Dictionary<Utilisateur, float> PayeursEtMontant;
        public float MontantTotal { get; set; }
        public string Nom { get; set; }

        public int IdGroupe { get; set; }

        private bool Equals(Facture other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Facture) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}