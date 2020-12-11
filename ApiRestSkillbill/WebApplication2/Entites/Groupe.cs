using System.Collections.Generic;

namespace WebApplication2.Entites
{
    public class Groupe
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Monnaie { get; set; }
        public Utilisateur UtilisateurCreateur { get; set; }
        public List<Utilisateur> UtilisateursAbonnes { get; set; }
        public string DateCreation { get; set; }
        public List<Facture> factures { get; set; }
    }
}