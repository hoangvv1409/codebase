using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VinEcom.MobileNotification.Enums;
using VinEcom.MobileNotification.InternalApi.Models;
using VinEcom.MobileNotification.Service;

namespace VinEcom.MobileNotification.InternalApi.Controllers
{
    public class OrderController : ApiController
    {
        private NotificationService notificationService;

        public OrderController(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        // POST api/order
        [Route("notification/orders")]
        public void Post([FromBody]OrderModel orderModel)
        {
            if (!ModelState.IsValid) return;

            Order order = null;
            switch (orderModel.OrderState)
            {
                case 1:
                    order = new Order(orderModel.SOID, orderModel.UserId, OrderState.Confirm);
                    break;
                case 2:
                    order = new Order(orderModel.SOID, orderModel.UserId, OrderState.Cancel);
                    break;
                case 3:
                    order = new Order(orderModel.SOID, orderModel.UserId, OrderState.Paid);
                    break;
                case 4:
                    order = new Order(orderModel.SOID, orderModel.UserId, OrderState.Shipping);
                    break;
                case 5:
                    if (orderModel.WarehouseId == null || string.IsNullOrEmpty(orderModel.WarehouseName) ||
                        string.IsNullOrWhiteSpace(orderModel.WarehouseName))
                    {
                        return;
                    }

                    order = new Order(orderModel.SOID, orderModel.UserId, OrderState.Arrived, (int)orderModel.WarehouseId, orderModel.WarehouseName);
                    break;
                case 6:
                    order = new Order(orderModel.SOID, orderModel.UserId, OrderState.PartiallyCancel);
                    break;
                default:
                    order = null;
                    return;
            }

            this.notificationService.PushOrderNotification(order);
        }
    }
}
