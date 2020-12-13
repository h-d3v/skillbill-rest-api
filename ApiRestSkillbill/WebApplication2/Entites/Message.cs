using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WebApplication2.Entites
{
   
        public class Message
        {[JsonConverter(typeof(StringEnumConverter))]
            public enum TypeMsg
            {
                Ajout,
                Suppression,
                AjoutMembre,
                Modification
            }

            public TypeMsg Typemessage { get; set; }
            
            public string MsgJson { get; set; }

            public Utilisateur Expediteur { get; set; }

            public Message(TypeMsg typemessage, string msgJson, Utilisateur expediteur)
            {
                Typemessage = typemessage;
                MsgJson = msgJson;
                Expediteur = expediteur;
            }

            public Message()
            {
            }
        }
    
}