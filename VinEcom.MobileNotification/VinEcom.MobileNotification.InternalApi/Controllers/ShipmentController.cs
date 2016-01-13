using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.InternalApi.Models;
using VinEcom.MobileNotification.Service;

namespace VinEcom.MobileNotification.InternalApi.Controllers
{
    public class ShipmentController : ApiController
    {
        private NotificationService notificationService;

        public ShipmentController(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [Route("notification/shipments")]
        public void Post([FromBody]ShipmentModel shipmentModel)
        {
            if (!ModelState.IsValid) return;

            Shipment shipment = null;
            switch (shipmentModel.ShipmentState)
            {
                case 1:
                    shipment = new Shipment(shipmentModel.ShipmentId, shipmentModel.SOID, shipmentModel.UserId, ShipmentState.Shipping);
                    break;
                case 2:
                    if (shipmentModel.WarehouseId == null || string.IsNullOrEmpty(shipmentModel.WarehouseName) ||
                        string.IsNullOrWhiteSpace(shipmentModel.WarehouseName))
                    {
                        return;
                    }

                    shipment = new Shipment(shipmentModel.ShipmentId, shipmentModel.SOID, shipmentModel.UserId, ShipmentState.Arrived, (int)shipmentModel.WarehouseId, shipmentModel.WarehouseName);
                    break;
                default:
                    shipment = null;
                    return;
            }

            this.notificationService.PushShipmentNotification(shipment);
        }
    }
}
