using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Paty",
                url: "sobitie/{paty}",
                defaults: new { controller = "Home", action = "PatyDetails", paty = "" }
            );

            routes.MapRoute(
                name: "PatyList",
                url: "spisok-meropriatiy/{patyinner}",
                defaults: new { controller = "Paty", action = "CategoryDetails", patyinner = "" }
            );

            routes.MapRoute(
                name: "PatyCategorys",
                url: "meropriatiya/{patycat}",
                defaults: new { controller = "Paty", action = "PatyMenuDetails", patycat = "" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
