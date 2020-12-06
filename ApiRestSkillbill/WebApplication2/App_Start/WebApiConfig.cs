using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;

namespace WebApplication2
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Configuration et services API Web
            config.EnableCors();
            // Itinéraires de l'API Web
            config.MapHttpAttributeRoutes();
            
            config.Formatters.JsonFormatter.SerializerSettings = 
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore};

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
