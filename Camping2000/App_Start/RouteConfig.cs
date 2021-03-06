﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Camping2000
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("SpaceForCaravan",
              "Home/{action}/{id}",
              new { controller = "Home", action = "SpaceForCaravan", id = UrlParameter.Optional });

            routes.MapRoute("Account/SpaceForTent",
               "Home/{action}/{id}",
               new { controller = "Home", action = "SpaceForTent", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
