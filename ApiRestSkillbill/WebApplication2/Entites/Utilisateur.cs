namespace WebApplication2.Entites
{
    public class Utilisateur
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Courriel { get; set; }
        public int Id { get; set; }
        public int Monnaie { get; set; }
        public string MotDePasse { get; set; }

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
    }
}