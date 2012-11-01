using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;

namespace GoocaBoocaBase
{
    // メモ: IIS6 または IIS7 のクラシック モードの詳細については、
    // http://go.microsoft.com/?LinkId=9394801 を参照してください

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // ルート名
                "{controller}/{action}/{id}", // パラメーター付きの URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // パラメーターの既定値
            );

            routes.MapRoute(
                "Answer",
                "{controller}/{action}/{research_id}-{image_id}-{answer_id}",
               new { controller = "Home", action = "Answer", research_id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Answer2",
                "{controller}/{action}/{research_id}-{image_id}-{answer_id}/{uid}",
                new { controller = "Home", action = "Answer", research_id = UrlParameter.Optional }
            );

            routes.MapRoute(
                "Answer3",
                "{controller}/{action}/{research_id}-{selected_image_id}-{noSelected_image_id}/{uid}",
                new { controller = "Home", action = "Answer", research_id = UrlParameter.Optional }
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<GoocaBoocaDataModels.GoocaBoocaDataBase>());

        }
    }
}