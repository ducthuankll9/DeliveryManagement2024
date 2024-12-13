using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryManagement.Helper
{
    public class Constants
    {
        public const string defaultString = "creating";

        // DATABASE
        public const string DB_DBNAME = "DeliveryDatabase";
        
        public const string DB_TableLinehaul = "Linehaul";
        public const string DB_TableLinehaulOrder = "Linehaul_Order";
        public const string DB_TableLinehaulPackage = "Linehaul_Package";
        public const string DB_TableOrder = "Order";
            public const string DB_Order_Weight = "TotalWeight";
            public const string DB_Order_Price = "OrderPrice";
            public const string DB_Order_Fee = "Fee";
            public const string DB_Order_Paid = "Paid";
            public const string DB_Order_OnDelivering = "OnDelivering";
            public const string DB_Order_CurrentStation = "CurrentStationID";
            public const string DB_Order_ID = "OrderID";

        public const string DB_TableOrderStatus = "Order_Status";
        public const string DB_TableOrderItems = "OrderItems";
        public const string DB_TableOrderProblem = "OrderProblem";
        public const string DB_TablePackage = "Package";
        public const string DB_TablePackageOrder = "Package_Order";
        public const string DB_TableShippingRates = "ShippingRates";
        public const string DB_TableStaff = "Staff";
        public const string DB_TableStation = "Station";
            public const string Value_Station_Default = "    ";

        public const string DB_TableStatus = "Status";
            public const string Value_Status_Created = "Created";
            public const string Value_Status_Completed = "Completed";
            public const string Value_Status_Updated = "Updated";
            public const string Value_Status_Received = "Received";
            public const string Value_Status_Packing = "Packing";
            public const string Value_Status_Packed = "Packed";
            public const string Value_Status_Transiting = "Transiting";
            public const string Value_Status_Transited = "Transited";
            public const string Value_Status_Sending = "Sending";
            public const string Value_Status_Finished = "Finished";
            public const string Value_Status_Breaked = "Breaked";
            public const string Value_Status_Cancelled = "Cancelled";

        public const string DB_TableVehicle = "Vehicle";

    }
}