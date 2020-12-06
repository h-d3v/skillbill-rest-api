using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApplication2.Entites
{
    [DataContract]
    public class Utilisateur
    {
        [DataMember]
        public string Nom { get; set; }
        [DataMember]
        public string Prenom { get; set; }
        [DataMember]
        public string Courriel { get; set; }
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Monnaie { get; set; }
        [DataMember]
        public string MotDePasse { get; set; }

        [DataMember]
        public string MotDePasseMod { get; set; }
        
        [DataMember]
        public string ApiKey { get; set; }

        
        
        protected bool Equals(Utilisateur other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Utilisateur) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return Id.ToString();
        }
    }
}