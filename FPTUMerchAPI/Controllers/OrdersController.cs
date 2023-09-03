using FPTUMerchAPI.Model;
using FireSharp.Extensions;
using Google.Cloud.Firestore;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FPTUMerchAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        string path = AppDomain.CurrentDomain.BaseDirectory + @"fptumerch.json";
        // GET: api/<OrdersController>
        [HttpGet]
        public async Task<ActionResult> GetOrders()
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                List<Orders> ordersList = new List<Orders>();
                Query Qref = database.Collection("Order").OrderBy("CreateDate");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();
                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Orders order = docsnap.ConvertTo<Orders>();
                        order.OrderID = docsnap.Id;
                        order.orderDetails = new List<OrderDetail>();
                        Query coll = database.Collection("Order").Document(docsnap.Id).Collection("OrderDetail");
                        QuerySnapshot queryColl = await coll.GetSnapshotAsync();
                        foreach (DocumentSnapshot docsnap2 in queryColl)
                        {
                            if (docsnap2.Exists)
                            {
                                OrderDetail orderDetail = docsnap2.ConvertTo<OrderDetail>();
                                orderDetail.OrderDetailID = docsnap2.Id;
                                orderDetail.OrderID = docsnap.Id;
                                order.orderDetails.Add(orderDetail);
                            }
                        }
                        ordersList.Add(order);
                    }
                }
                return Ok(ordersList);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [HttpGet]
        public async Task<ActionResult> GetActiveOrders()
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                List<Orders> ordersList = new List<Orders>();
                Query Qref = database.Collection("Order").OrderBy("CreateDate");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();
                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Orders order = docsnap.ConvertTo<Orders>();
                        order.OrderID = docsnap.Id;
                        order.orderDetails = new List<OrderDetail>();
                        Query coll = database.Collection("Order").Document(docsnap.Id).Collection("OrderDetail");
                        QuerySnapshot queryColl = await coll.GetSnapshotAsync();
                        foreach (DocumentSnapshot docsnap2 in queryColl)
                        {
                            if (docsnap2.Exists)
                            {
                                OrderDetail orderDetail = docsnap2.ConvertTo<OrderDetail>();
                                orderDetail.OrderDetailID = docsnap2.Id;
                                orderDetail.OrderID = docsnap.Id;
                                order.orderDetails.Add(orderDetail);
                            }
                        }
                        ordersList.Add(order);
                    }
                }
                return Ok(ordersList.Where(x=> x.Status.Equals(true)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<ActionResult> GetAlreadyPaidOrders()
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                List<Orders> ordersList = new List<Orders>();
                Query Qref = database.Collection("Order").OrderBy("CreateDate");
                QuerySnapshot snap = await Qref.GetSnapshotAsync();
                foreach (DocumentSnapshot docsnap in snap)
                {
                    if (docsnap.Exists)
                    {
                        Orders order = docsnap.ConvertTo<Orders>();
                        order.OrderID = docsnap.Id;
                        order.orderDetails = new List<OrderDetail>();
                        Query coll = database.Collection("Order").Document(docsnap.Id).Collection("OrderDetail");
                        QuerySnapshot queryColl = await coll.GetSnapshotAsync();
                        foreach (DocumentSnapshot docsnap2 in queryColl)
                        {
                            if (docsnap2.Exists)
                            {
                                OrderDetail orderDetail = docsnap2.ConvertTo<OrderDetail>();
                                orderDetail.OrderDetailID = docsnap2.Id;
                                orderDetail.OrderID = docsnap.Id;
                                order.orderDetails.Add(orderDetail);
                            }
                        }
                        ordersList.Add(order);
                    }
                }
                return Ok(ordersList.Where(x => x.PaidStatus.Equals(true)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/<OrdersController>/5
        [HttpGet("{OrderId}")]
        public async Task<ActionResult> GetOrdersByOrderID(string OrderId)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                Orders order = new Orders();
                DocumentReference docRef = database.Collection("Order").Document(OrderId);
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                if (docSnap.Exists)
                {
                    order = docSnap.ConvertTo<Orders>();
                    order.OrderID = docSnap.Id;
                    order.orderDetails = new List<OrderDetail>();
                    Query coll = database.Collection("Order").Document(docSnap.Id).Collection("OrderDetail");
                    QuerySnapshot queryColl = await coll.GetSnapshotAsync();
                    foreach (DocumentSnapshot docsnap2 in queryColl)
                    {
                        if (docsnap2.Exists)
                        {
                            OrderDetail orderDetail = docsnap2.ConvertTo<OrderDetail>();
                            orderDetail.OrderDetailID = docsnap2.Id;
                            orderDetail.OrderID = docSnap.Id;
                            order.orderDetails.Add(orderDetail);
                        }
                    }
                    return Ok(order);
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/<OrdersController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Orders Order)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                bool checkOrderID = false; // Check if order ID exist
                string documentID = "";
                //CHECK IF NEW ORDER ID EXISTS
                do
                {
                    documentID = generateOrderID();
                    Query qRefOrder = database.Collection("Order");
                    QuerySnapshot qSnapOrder = await qRefOrder.GetSnapshotAsync();
                    if(qSnapOrder.Count > 0)
                    {
                        foreach (DocumentSnapshot docSnapOrder in qSnapOrder)
                        {
                            if (docSnapOrder.Id == documentID)
                            {
                                checkOrderID = false;
                                break;
                            }
                            else
                            {
                                checkOrderID = true;
                                continue;
                            }
                        }
                    }
                    else
                    {
                        checkOrderID = true;
                        continue;
                    }
                } while (checkOrderID == false);
                //---------------------------------------------------------------------
                DocumentReference docRef = database.Collection("Order").Document(documentID);
                var specified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                float totalPrice = 0;

                if(Order.orderDetails == null)
                {
                    return BadRequest("Cannot make order, there is no order detail is the order detail list");
                }
                else
                {
                    /*CALCULATE TOTAL PRICE OF ORDER AND IF PRODUCT EXISTS*/
                    foreach (var item in Order.orderDetails)
                    {
                        DocumentReference docRefProduct = database.Collection("Product").Document(item.ProductID);
                        DocumentSnapshot docSnapProduct = await docRefProduct.GetSnapshotAsync();
                        if (docSnapProduct.Exists)
                        {
                            Product product = docSnapProduct.ConvertTo<Product>();
                            totalPrice += product.Price * item.Amount;
                        }
                        else
                        {
                            return BadRequest("The product is not correct, please try again.");
                        }
                    }
                    /*CHECK IF DISCOUNT CODE CORRECT*/
                    if (Order.DiscountCodeID != null && Order.DiscountCodeID != "" && Order.DiscountCodeID.Length != 0)
                    {
                        DocumentReference docRefDiscountCode = database.Collection("DiscountCode").Document(Order.DiscountCodeID);
                        DocumentSnapshot docSnapDiscountCode = await docRefDiscountCode.GetSnapshotAsync();
                        if (!docSnapDiscountCode.Exists)
                        {
                            return BadRequest("The Discount Code is not exist");
                        }
                        else
                        {
                            DiscountCode discountCode = docSnapDiscountCode.ConvertTo<DiscountCode>();
                            // FALSE: MÃ CHƯA KÍCH HOẠT, TRUE: MÃ ĐÃ KÍCH HOẠT
                            if (discountCode.Status == false)
                            {
                                return BadRequest("The Discount Code is not activated yet");
                            }
                            else
                            {
                                Dictionary<string, object> updateDiscountCode = new Dictionary<string, object>()
                                {
                                    { "Status",discountCode.Status },
                                    { "NumberOfTimes", discountCode.NumberOfTimes + 1},
                                    { "KPI", discountCode.KPI}
                                };
                                await docRefDiscountCode.SetAsync(updateDiscountCode);
                                totalPrice = totalPrice * 9 / 10;
                            }
                        }
                    }
                    /*ADD ORDER BASIC INFORMATION*/
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "DiscountCodeID", Order.DiscountCodeID},
                        { "OrdererName", Order.OrdererName},
                        { "OrdererPhoneNumber", Order.OrdererPhoneNumber},
                        { "OrdererEmail", Order.OrdererEmail},
                        { "DeliveryAddress", Order.DeliveryAddress},
                        { "TotalPrice", totalPrice},
                        { "CreateDate", specified.ToTimestamp()},
                        { "Note", Order.Note },
                        { "EarningMethod", Order.EarningMethod},
                        { "Payments", Order.Payments },
                        { "Status", true},
                        { "PaidStatus", false },
                        { "Shipper", null},
                        { "ShippedStatus", false}
                    };
                    await docRef.SetAsync(data);
                    CollectionReference coll = docRef.Collection("OrderDetail");
                    Dictionary<string, object> orderDetailList = new Dictionary<string, object>();
                    /*ADD ORDER DETAIL*/
                    foreach (var item in Order.orderDetails)
                    {
                        orderDetailList.Add("ProductID", item.ProductID);
                        orderDetailList.Add("Amount", item.Amount);
                        orderDetailList.Add("Note", item.Note);
                        orderDetailList.Add("CreateDate", specified.ToTimestamp());
                        await coll.AddAsync(orderDetailList);
                        orderDetailList = new Dictionary<string, object>();
                    }
                    return Ok();
                }
                
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<OrdersController>/5
        // Update Order's Entities, Not Order Detail's Entities
        // Update includes: Name, Phone Number, Email, Address, Payments Shipper
        [HttpPut("{OrderId}")]
        public async Task<ActionResult> Put(string OrderId, [FromBody] Orders order)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRef = database.Collection("Order").Document(OrderId);
                var specified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                //GET THE CURRENT ORDER TOTAL PRICE
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                if (docSnap.Exists)
                {
                    Orders orderTotalPrice = docSnap.ConvertTo<Orders>();
                    /*UPDATE ORDER BASIC DETAILS*/
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "OrdererName", order.OrdererName},
                        { "OrdererPhoneNumber", order.OrdererPhoneNumber},
                        { "OrdererEmail", order.OrdererEmail},
                        { "DeliveryAddress", order.DeliveryAddress},
                        { "TotalPrice", orderTotalPrice.TotalPrice},
                        { "CreateDate", specified.ToTimestamp()},
                        { "Note", order.Note },
                        { "EarningMethod", order.EarningMethod},
                        { "Payments", order.Payments },
                        { "Status", order.Status},
                        { "PaidStatus", order.PaidStatus},
                        { "Shipper", order.Shipper},
                        { "ShippedStatus", order.ShippedStatus }
                    };
                    await docRef.SetAsync(data);
                    return Ok();
                }
                else
                {
                    return BadRequest("The order not exist");
                }

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{OrderId}/{Shipper}")]
        public async Task<ActionResult> AddShipper(string OrderId, string Shipper)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRef = database.Collection("Order").Document(OrderId);
                var specified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                if (docSnap.Exists)
                {
                    Orders order = docSnap.ConvertTo<Orders>();
                    /*UPDATE ORDER BASIC DETAILS*/
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "DiscountCodeID", order.DiscountCodeID },
                        { "OrdererName", order.OrdererName},
                        { "OrdererPhoneNumber", order.OrdererPhoneNumber},
                        { "OrdererEmail", order.OrdererEmail},
                        { "DeliveryAddress", order.DeliveryAddress},
                        { "TotalPrice", order.TotalPrice},
                        { "CreateDate", specified.ToTimestamp()},
                        { "Note", order.Note },
                        { "EarningMethod", order.EarningMethod},
                        { "Payments", order.Payments },
                        { "Status", order.Status},
                        { "PaidStatus", order.PaidStatus},
                        { "Shipper", Shipper},
                        { "ShippedStatus", order.ShippedStatus}
                    };
                    await docRef.SetAsync(data);
                    return Ok();
                }
                else
                {
                    return BadRequest("The order not exist");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/<OrdersController>/5
        [HttpPut("{OrderId}")]
        public async Task<ActionResult> PaidConfirm(string OrderId)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRef = database.Collection("Order").Document(OrderId);
                var specified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                if (docSnap.Exists)
                {
                    Orders order = docSnap.ConvertTo<Orders>();
                    /*UPDATE ORDER BASIC DETAILS*/
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "DiscountCodeID", order.DiscountCodeID },
                        { "OrdererName", order.OrdererName},
                        { "OrdererPhoneNumber", order.OrdererPhoneNumber},
                        { "OrdererEmail", order.OrdererEmail},
                        { "DeliveryAddress", order.DeliveryAddress},
                        { "TotalPrice", order.TotalPrice},
                        { "CreateDate", specified.ToTimestamp()},
                        { "Note", order.Note },
                        { "EarningMethod", order.EarningMethod},
                        { "Payments", order.Payments },
                        { "Status", order.Status},
                        { "Shipper", order.Shipper},
                        { "ShippedStatus", order.ShippedStatus}
                    };
                    if (order.PaidStatus == true)
                    {
                        data.Add("PaidStatus", false);
                    }
                    else if (order.PaidStatus == false)
                    {
                        data.Add("PaidStatus", true);
                    }
                    else
                    {
                        data.Add("PaidStatus", null);
                    }
                    //UPDATE KPI FOR SALER
                    DocumentReference docRefDiscountID = database.Collection("DiscountCode").Document(order.DiscountCodeID);
                    DocumentSnapshot docSnapDiscountID = await docRefDiscountID.GetSnapshotAsync();
                    if (docSnapDiscountID.Exists)
                    {
                        DiscountCode discountCode = docSnapDiscountID.ConvertTo<DiscountCode>();
                        discountCode.DiscountCodeID = docSnapDiscountID.Id;
                        Dictionary<string, object> discountCodeUpdate = new Dictionary<string, object>()
                        {
                            { "Status", discountCode.Status},
                            { "NumberOfTimes", discountCode.NumberOfTimes},
                        };
                        if (order.PaidStatus == true)
                        {
                            discountCodeUpdate.Add("KPI", discountCode.KPI - 1);
                        }
                        else if (order.PaidStatus == false)
                        {
                            discountCodeUpdate.Add("KPI", discountCode.KPI + 1);
                        }
                        else
                        {
                            discountCodeUpdate.Add("KPI", discountCode.KPI);
                        }
                        await docRefDiscountID.SetAsync(discountCodeUpdate);
                    }
                    await docRef.SetAsync(data);
                    return Ok();
                }
                else
                {
                    return BadRequest("The order not exist");
                }
            } catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{OrderId}")]
        public async Task<ActionResult> ShippedConfirm(string OrderId)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRef = database.Collection("Order").Document(OrderId);
                var specified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
                DocumentSnapshot docSnap = await docRef.GetSnapshotAsync();
                if (docSnap.Exists)
                {
                    Orders order = docSnap.ConvertTo<Orders>();
                    /*UPDATE ORDER BASIC DETAILS*/
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "DiscountCodeID", order.DiscountCodeID },
                        { "OrdererName", order.OrdererName},
                        { "OrdererPhoneNumber", order.OrdererPhoneNumber},
                        { "OrdererEmail", order.OrdererEmail},
                        { "DeliveryAddress", order.DeliveryAddress},
                        { "TotalPrice", order.TotalPrice},
                        { "CreateDate", specified.ToTimestamp()},
                        { "Note", order.Note },
                        { "EarningMethod", order.EarningMethod},
                        { "Payments", order.Payments },
                        { "Status", order.Status},
                        { "PaidStatus", order.PaidStatus},
                        { "Shipper", order.Shipper },
                    };
                    if (order.ShippedStatus == true)
                    {
                        data.Add("ShippedStatus", false);
                    }
                    else if (order.ShippedStatus == false)
                    {
                        data.Add("ShippedStatus", true);
                    }
                    else
                    {
                        data.Add("ShippedStatus", null);
                    }
                    await docRef.SetAsync(data);
                    return Ok();
                }
                else
                {
                    return BadRequest("The order not exist");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/<OrdersController>/5
        [HttpDelete("{OrderId}")]
        public async Task<ActionResult> Delete(string OrderId)
        {
            try
            {
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);
                FirestoreDb database = FirestoreDb.Create("fptumerch-abcde");
                DocumentReference docRef = database.Collection("Order").Document(OrderId);
                DocumentSnapshot snap = await docRef.GetSnapshotAsync();
                if (snap.Exists)
                {
                    Orders order = snap.ConvertTo<Orders>();
                    Dictionary<string, object> data = new Dictionary<string, object>()
                    {
                        { "OrdererName", order.OrdererName},
                        { "OrdererPhoneNumber", order.OrdererPhoneNumber},
                        { "OrdererEmail", order.OrdererEmail},
                        { "DeliveryAddress", order.DeliveryAddress},
                        { "TotalPrice", order.TotalPrice},
                        { "CreateDate", order.CreateDate},
                        { "Note", order.Note },
                        { "EarningMethod", order.EarningMethod},
                        { "Payments", order.Payments },
                        { "Status", false}, //TRUE: Not cancelled, FALSE: cancelled
                        { "PaidStatus", order.PaidStatus},
                        { "Shipper", order.Shipper},
                        { "ShippedStatus", order.ShippedStatus }
                    };
                    await docRef.SetAsync(data);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [NonAction]
        public string generateOrderID()
        {
            string result = "FM2023";
            Random rnd = new Random();
            for(int i=0; i <= 4; i++)
            {
                result += rnd.Next(0, 10).ToString(); // returns random integers >= 0 and < 9
            }
            return result;
        }

    }
}
