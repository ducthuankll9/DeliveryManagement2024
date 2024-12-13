using System.Web.Mvc;

namespace DeliveryManagement.Areas.Drivers
{
    public class DriversAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Drivers";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Drivers_default",
                "Drivers/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}