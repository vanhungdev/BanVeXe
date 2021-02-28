using WebsiteBanVeXe.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using API_NganLuong;
using WebsiteBanVeXe.nganluonAPI;
using WebsiteBanVeXe.MomoAPI;
using MoMo;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using WebsiteBanVeXe.Library;

namespace WebsiteBanVeXe.Controllers
{
    public class CheckoutController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        BanVeXeDbContext db = new BanVeXeDbContext();
        // GET: Checkout
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            var list = new List<ticket>();

            int id = int.Parse(fc["datve"]);
            var list1 = db.tickets.Find(id);
            ViewBag.songuoi = int.Parse(fc["songuoi"]);

            list.Add(list1);
            // neu co ve khu hoi
            if (!string.IsNullOrEmpty(fc["datveKH"]))
            {
                int id2 = int.Parse(fc["datveKH"]);
                var list2 = db.tickets.Find(id2);
                ViewBag.ve2 = id2;
                list.Add(list2);
            }
            ViewBag.ve1 = id;

            return View("", list.ToList());
        }
        [HttpPost]
        public ActionResult checkOut(order order, FormCollection fc)
        {
            string orderCode ="MaORDER"+DateTime.Now.Day + DateTime.Now.Month + DateTime.Now.Year + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            Session["OrderId"] = orderCode;
            float total = float.Parse(fc["total"]);
            int id1 = int.Parse(fc["veOnvay"]);
            string veReturn = fc["veReturn"];
            string payment_method = Request["option_payment"];
            if (payment_method.Equals("COD"))
            {
                // cap nhat thong tin sau khi dat hang thanh cong
                SaveOrder(order, total, id1, veReturn, "Mua vé Tại quầy", 2, orderCode);
                return View("checkOutComfin", order);
            }
            //Neu Thanh toan MOMO
            else if (payment_method.Equals("MOMO"))
            {
                //request params need to request to MoMo system
                string endpoint = momoInfo.endpoint;
                string partnerCode = momoInfo.partnerCode;
                string accessKey = momoInfo.accessKey;
                string serectkey = momoInfo.serectkey;
                string orderInfo = momoInfo.orderInfo;
                string returnUrl = momoInfo.returnUrl;
                string notifyurl = momoInfo.notifyurl;

                string amount = total.ToString();
                string orderid = Guid.NewGuid().ToString();
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";

                //Before sign HMAC SHA256 signature
                string rawHash = "partnerCode=" +
                    partnerCode + "&accessKey=" +
                    accessKey + "&requestId=" +
                    requestId + "&amount=" +
                    amount + "&orderId=" +
                    orderid + "&orderInfo=" +
                    orderInfo + "&returnUrl=" +
                    returnUrl + "&notifyUrl=" +
                    notifyurl + "&extraData=" +
                    extraData;

                log.Debug("rawHash = " + rawHash);

                MoMoSecurity crypto = new MoMoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);
                log.Debug("Signature = " + signature);

                //build body json request
                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };
                log.Debug("Json request to MoMo: " + message.ToString());
                string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
                JObject jmessage = JObject.Parse(responseFromMomo);

                SaveOrder(order, total, id1, veReturn, "Thanh toán MOMO", 2, orderCode);
                return Redirect(jmessage.GetValue("payUrl").ToString());
            }
            //Neu Thanh toan Ngan Luong
            else if (payment_method.Equals("NL"))
            {
                string str_bankcode = Request["bankcode"];
                RequestInfo info = new RequestInfo();
                info.Merchant_id = nganluongInfo.Merchant_id;
                info.Merchant_password = nganluongInfo.Merchant_password;
                info.Receiver_email = nganluongInfo.Receiver_email;
                info.cur_code = "vnd";
                info.bank_code = str_bankcode;
                info.Order_code = orderCode;
                info.Total_amount = total.ToString();
                info.fee_shipping = "0";
                info.Discount_amount = "0";
                info.order_description = "Thanh toán ngân lượng cho đơn hàng";
                info.return_url = nganluongInfo.return_url;
                info.cancel_url = nganluongInfo.cancel_url;
                info.Buyer_fullname = order.name;
                info.Buyer_email = order.email;
                info.Buyer_mobile = order.phone;
                APICheckoutV3 objNLChecout = new APICheckoutV3();
                ResponseInfo result = objNLChecout.GetUrlCheckout(info, payment_method);
                // neu khong gap loi gi
                if (result.Error_code == "00")
                {
                    SaveOrder(order, total, id1, veReturn, "Thanh toán ngân lượng", 2, orderCode);
                    // chuyen sang trang ngan luong
                    return Redirect(result.Checkout_url);
                }
                else
                {
                    ViewBag.errorPaymentOnline = result.Description;
                    return View("payment");
                }

            }
            //Neu Thanh Toán ATM online
            else if (payment_method.Equals("ATM_ONLINE"))
            {
                string str_bankcode = Request["bankcode"];
                RequestInfo info = new RequestInfo();
                info.Merchant_id = nganluongInfo.Merchant_id;
                info.Merchant_password = nganluongInfo.Merchant_password;
                info.Receiver_email = nganluongInfo.Receiver_email;
                info.cur_code = "vnd";
                info.bank_code = str_bankcode;
                info.Order_code = orderCode;
                info.Total_amount = total.ToString();
                info.fee_shipping = "0";
                info.Discount_amount = "0";
                info.order_description = "Thanh toán ngân lượng cho đơn hàng";
                info.return_url = nganluongInfo.return_url;
                info.cancel_url = nganluongInfo.cancel_url;
                info.Buyer_fullname = order.name;
                info.Buyer_email = order.email;
                info.Buyer_mobile = order.phone;
                APICheckoutV3 objNLChecout = new APICheckoutV3();
                ResponseInfo result = objNLChecout.GetUrlCheckout(info, payment_method);
                // neu khong gap loi gi
                if (result.Error_code == "00")
                {
                    SaveOrder(order, total, id1, veReturn, "Thanh toán MTM Online qua internet Banking", 2, orderCode);
                    return Redirect(result.Checkout_url);
                }
                else
                {
                    ViewBag.errorPaymentOnline = result.Description;
                    return View("payment");
                }
            }
            return View("checkOutComfin", order);
        }
        // lay thong tin cac ve da book
        public ActionResult _BookingConnfig(int orderId)
        {
            var list = db.ordersdetails.Where(m => m.orderid == orderId).ToList();
            var list1 = new List<ticket>();
            foreach (var item in list)
            {
                ticket ticket = db.tickets.Find(item.ticketId);
                list1.Add(ticket);
            }

            return View("_BookingConnfig", list1.ToList());
        }

        public void SaveOrder(order order, float total,int id1,string veReturn, string paymentMethod, int StatusPayment, string ordercode)
        {
            order.created_ate = DateTime.Now;
            order.status = 1;
            order.total = total;
            order.deliveryPaymentMethod = paymentMethod;
            order.StatusPayment = StatusPayment;
            order.code = ordercode;
            db.orders.Add(order);
            db.SaveChanges();
            int lastOrderID = order.ID;
            ordersdetail orderDetail = new ordersdetail();
            orderDetail.ticketId = id1;
            orderDetail.orderid = lastOrderID;
            db.ordersdetails.Add(orderDetail);
            // tru so luong nghe
            var ticket = db.tickets.Find(id1);
            ticket.sold = order.guestTotal;
            db.Entry(ticket).State = EntityState.Modified;
            //neu ton tai ve 2 chieu
            if (!string.IsNullOrEmpty(veReturn))
            {
                int id2 = int.Parse(veReturn);
                ordersdetail orderDetail2 = new ordersdetail();
                orderDetail2.ticketId = id2;
                orderDetail2.orderid = lastOrderID;
                db.ordersdetails.Add(orderDetail2);
                // tru so luong nghe
                var ticket2 = db.tickets.Find(id2);
                ticket2.sold =  order.guestTotal;
                db.Entry(ticket2).State = EntityState.Modified;
            }
            db.SaveChanges();
            if(order.StatusPayment == 3)
            {
                string mailBody = renderHtmlEmail(order,"Sử dụng Email này để thanh toán tại quầy");
                // neeu chon hinh thuc thanh toan Mua ves
                SendEmail(order.email, order.name, mailBody);
            }
          
        }

        //Khi huy thanh toán Ngan Luong
        public ActionResult cancel_order()
        {

            return View("cancel_order");
        }
        //Khi thanh toán Ngan Luong XOng
        public ActionResult confirm_orderPaymentOnline()
        {
            String Token = Request["token"];
            RequestCheckOrder info = new RequestCheckOrder();
            info.Merchant_id = nganluongInfo.Merchant_id;
            info.Merchant_password = nganluongInfo.Merchant_password;
            info.Token = Token;
            APICheckoutV3 objNLChecout = new APICheckoutV3();
            ResponseCheckOrder result = objNLChecout.GetTransactionDetail(info);
            if (result.errorCode == "00")
            {
                String codeOrder = Session["OrderId"].ToString();
                var OrderInfo = db.orders.OrderByDescending(m => m.code == codeOrder).FirstOrDefault();
                OrderInfo.StatusPayment = 1;
                db.Entry(OrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.paymentStatus = OrderInfo.StatusPayment;
                ViewBag.Methodpayment = OrderInfo.deliveryPaymentMethod;

                //sendmail
                string mailBody = renderHtmlEmail(OrderInfo, "Đã thanh toán");
                SendEmail(OrderInfo.email, OrderInfo.name, mailBody);
                return View("checkOutComfin", OrderInfo);
            }
            else
            {
                ViewBag.status = false;
            }

            return View("confirm_orderPaymentOnline");
        }

        //Khi huy thanh toán MOMO
        public ActionResult cancel_order_momo()
        {

            return View("cancel_order");
        }
        //Khi Thanh toám momo xong
        public ActionResult confirm_orderPaymentOnline_momo()
        {

            String errorCode = Request["errorCode"];
            String orderId = Request["orderId"];
            if (errorCode == "0")
            {
                Session["SessionCart"] = null;
                var OrderInfo = db.orders.Where(m => m.code == orderId).FirstOrDefault();
                OrderInfo.StatusPayment = 1;
                db.Entry(OrderInfo).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.paymentStatus = OrderInfo.StatusPayment;
                ViewBag.Methodpayment = OrderInfo.deliveryPaymentMethod;
                //sendmail
                string mailBody = renderHtmlEmail(OrderInfo,"Đã thanh toán");
                SendEmail(OrderInfo.email, OrderInfo.name, mailBody);
                return View("checkOutComfin", OrderInfo);
            }
            else
            {
                ViewBag.status = false;
            }
            return View("confirm_orderPaymentOnline");
        }
        public void SendEmail(string CustomerEmail, string CustomerName, string mailBody)
        {

            MailMessage mm = new MailMessage(Util.email, CustomerEmail);
            mm.Subject = "[YATRA.COM] THÔNG BÁO XÁC NHẬN ĐƠN ĐẶT VÉ";
            mm.Body = mailBody;
            mm.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;
            /// Email dùng để gửi đi
            NetworkCredential nc = new NetworkCredential(Util.email, Util.password);
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
        }
        string renderHtmlEmail(order order,string statusPayment)
        {
            string mailBody = System.IO.File.ReadAllText(Server.MapPath("~/Views/Shared/mailTemplate.html"));
            mailBody = mailBody.Replace("{{name}}", order.name);
            mailBody = mailBody.Replace("{{orderCode}}", order.code);
            mailBody = mailBody.Replace("{{email}}", order.email);
            mailBody = mailBody.Replace("{{phone}}", order.phone);
            mailBody = mailBody.Replace("{{gioitinh}}", order.gioitinh);
            mailBody = mailBody.Replace("{{quoctich}}", order.quoctich);
            mailBody = mailBody.Replace("{{mess}}", order.mess);
            mailBody = mailBody.Replace("{{created_ate}}", order.created_ate.ToString("dd-MM-yyyy HH:mm:ss tt"));
            mailBody = mailBody.Replace("{{statuspayment}}", statusPayment);
            mailBody = mailBody.Replace("{{Methodpayment}}", order.deliveryPaymentMethod);
            mailBody = mailBody.Replace("{{total}}", order.total.ToString("N0") + "VND");

            var listOrder = db.ordersdetails.Where(m => m.orderid == order.ID).ToList();
            var listticket = new List<ticket>();
            foreach (var item in listOrder)
            {
                ticket tickett = db.tickets.Find(item.ticketId);
                listticket.Add(tickett);

            }
            int i = 1;
            foreach (var item in listticket)
            {
                string htmlListItem = @"<h4> Thông tin vé:</h4> <table style=' border-collapse: collapse;width: 100%;margin-top: 40px;'>
                    <tbody >
                            <tr>
                            <td style='font-weight:bold;border: 1px solid #ddd;padding: 8px;'>Mã vé </td>
                            <td  style='border: 1px solid #ddd;padding: 8px;'> " + item.code + @" </td>
                        </tr>
                        <tr>
                            <td style='font-weight:bold;border: 1px solid #ddd;padding: 8px;'>Nơi Xuất phát</td>
                            <td  style='border: 1px solid #ddd;padding: 8px;'> " + item.departure_address + @" </td>
                        </tr>
                        <tr>
                            <td  style='font-weight:bold;border: 1px solid #ddd;padding: 8px;'>Nơi đến</td>
                            <td style='border: 1px solid #ddd;padding: 8px;'>" + item.arrival_address + @"</td>
                        </tr>
                           <tr>
                            <td style='font-weight:bold;border: 1px solid #ddd;padding: 8px;' >Thời gian xuất bến</td>
                            <td style='border: 1px solid #ddd;padding: 8px;'>" + item.departure_date.ToString("dd-MM-yyyy HH:mm:ss") + @"  </td>
                        </tr>
                        <tr>
                            <td style='font-weight:bold;border: 1px solid #ddd;padding: 8px;'>Bến Xe</td>
                            <td style='border: 1px solid #ddd;padding: 8px;'> " + item.airport + @"</td>
                        </tr>
<tr>
                            <td style='font-weight:bold;border: 1px solid #ddd;padding: 8px;'>Biển số xe</td>
                            <td style='border: 1px solid #ddd;padding: 8px;'> " + item.licensePlates + @"</td>
                        </tr>
                          <tr>
                            <td style='font-weight:bold;border: 1px solid #ddd;padding: 8px;'>Giá vé</td>
                            <td style='border: 1px solid #ddd;padding: 8px;'> " + item.price.ToString("N0") + @" VND</td>
                        </tr>
                    </tbody>
                </table>";
                mailBody = mailBody.Replace("{{htmlListOrder" + i + "}}", htmlListItem);
                i++;
            }
            if (i < 3)
            {
                mailBody = mailBody.Replace("{{htmlListOrder2}}", "");

            }
            return mailBody;
        }
    }
}