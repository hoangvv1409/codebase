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

        /// <summary>
        /// Push Shipment Activity Notification
        /// </summary>
        /// <param name="shipmentModel">Shipment Model</param>
        /// <returns></returns>
        [Route("notification/shipments")]
        public HttpResponseMessage Post([FromBody]ShipmentModel shipmentModel)
        {
            if (shipmentModel == null) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Shipment must be not null");
            if (!ModelState.IsValid) return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);

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
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "WarehouseId and WarehouseName are required");
                    }

                    shipment = new Shipment(shipmentModel.ShipmentId, shipmentModel.SOID, shipmentModel.UserId, ShipmentState.Arrived, (int)shipmentModel.WarehouseId, shipmentModel.WarehouseName);
                    break;
                default:
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Wrong ShipmentState");
            }

            this.notificationService.PushShipmentNotification(shipment);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
