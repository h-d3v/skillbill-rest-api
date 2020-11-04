namespace WebApplication2.Entites
{
    public class Facture
    {
        public int Id { get; set; }
        public Utilisateur UtilisateurCreateur { get; set; }
        public string DateCreation { get; set; }
        
    }
}