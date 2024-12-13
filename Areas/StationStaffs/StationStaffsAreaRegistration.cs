using System.Web.Mvc;

namespace DeliveryManagement.Areas.StationStaffs
{
    public class StationStaffsAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "StationStaffs";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "StationStaffs_default",
                "StationStaffs/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}