﻿using System.Web.Http;
using SharedLibrary.Models;
using System.Web;
using System.Net.Http;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Text;
using Newtonsoft.Json;
using System;

namespace Portal.Controllers
{
    public class HubController : ApiController
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Message(string text, string from, string to, string smsId, string userId)
        {
            string result = "";
            text = (text == null) ? "" : text;
            text = HttpUtility.UrlDecode(text, System.Text.UnicodeEncoding.Default);
            text = HttpUtility.UrlDecode(text, System.Text.UnicodeEncoding.Default);
            var messageObj = new MessageObject();
            messageObj.MobileNumber = from;
            messageObj.Content = text;
            messageObj.ShortCode = to;
            if (messageObj.Content.Contains("Renewal"))
            {
                if (DateTime.Now.Hour < 14)
                {
                    if (messageObj.ShortCode == "405505")
                    {
                        using (var entity = new ShahreKalamehLibrary.Models.ShahreKalamehEntities())
                        {
                            var singlecharge = new ShahreKalamehLibrary.Models.Singlecharge();
                            singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                            singlecharge.DateCreated = DateTime.Now;
                            singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                            singlecharge.Price = 500;
                            if (messageObj.Content == "Renewal")
                                singlecharge.IsSucceeded = true;
                            else
                                singlecharge.IsSucceeded = false;
                            singlecharge.IsApplicationInformed = false;
                            singlecharge.IsCalledFromInAppPurchase = false;
                            var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                            if (installment != null)
                                singlecharge.InstallmentId = installment.Id;
                            entity.Singlecharges.Add(singlecharge);
                            entity.SaveChanges();
                        }
                    }
                    else if (messageObj.ShortCode == "405519")
                    {
                        using (var entity = new SoratyLibrary.Models.SoratyEntities())
                        {
                            var singlecharge = new SoratyLibrary.Models.Singlecharge();
                            singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                            singlecharge.DateCreated = DateTime.Now;
                            singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                            singlecharge.Price = 400;
                            if (messageObj.Content == "Renewal")
                                singlecharge.IsSucceeded = true;
                            else
                                singlecharge.IsSucceeded = false;
                            singlecharge.IsApplicationInformed = false;
                            singlecharge.IsCalledFromInAppPurchase = false;
                            var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                            if (installment != null)
                                singlecharge.InstallmentId = installment.Id;
                            entity.Singlecharges.Add(singlecharge);
                            entity.SaveChanges();
                        }
                    }
                    else if (messageObj.ShortCode == "405522")
                    {
                        using (var entity = new DefendIranLibrary.Models.DefendIranEntities())
                        {
                            var singlecharge = new DefendIranLibrary.Models.Singlecharge();
                            singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                            singlecharge.DateCreated = DateTime.Now;
                            singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                            singlecharge.Price = 400;
                            if (messageObj.Content == "Renewal")
                                singlecharge.IsSucceeded = true;
                            else
                                singlecharge.IsSucceeded = false;
                            singlecharge.IsApplicationInformed = false;
                            singlecharge.IsCalledFromInAppPurchase = false;
                            var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                            if (installment != null)
                                singlecharge.InstallmentId = installment.Id;
                            entity.Singlecharges.Add(singlecharge);
                            entity.SaveChanges();
                        }
                    }
                }
                result = "1";
            }
            else
            {
                if (smsId == null || smsId == "" || smsId == "null")
                {
                    var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromShortCode(messageObj.ShortCode);
                    if (serviceInfo != null)
                    {
                        messageObj.Content = serviceInfo.AggregatorServiceId;
                        messageObj.IsReceivedFromIntegratedPanel = true;
                    }
                }
                else
                    messageObj.ShortCode = to;

                messageObj.MessageId = smsId;

                messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);

                if (messageObj.MobileNumber == "Invalid Mobile Number")
                    result = "-1";
                else
                {
                    messageObj.ShortCode = SharedLibrary.MessageHandler.ValidateShortCode(messageObj.ShortCode);
                    if (messageObj.Content == "Subscription")
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-Notify-Register" : null;
                    else if (messageObj.Content == "Unsubscription")
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-Notify-Unsubscription" : null;
                    else
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
                    SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                    result = "1";
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Notification(string text, string keyword, string channel, string from, string to, string NotificationId, string userId)
        {
            string result = "";
            text = (text == null) ? "" : text;
            text = HttpUtility.UrlDecode(text, System.Text.UnicodeEncoding.Default);
            text = HttpUtility.UrlDecode(text, System.Text.UnicodeEncoding.Default);
            var messageObj = new MessageObject();
            messageObj.MobileNumber = from;
            messageObj.Content = text;
            messageObj.ShortCode = to;

            if (messageObj.Content.Contains("Renewal"))
            {
                if (messageObj.ShortCode == "405505")
                {
                    using (var entity = new ShahreKalamehLibrary.Models.ShahreKalamehEntities())
                    {
                        var singlecharge = new ShahreKalamehLibrary.Models.Singlecharge();
                        singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        singlecharge.DateCreated = DateTime.Now;
                        singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                        singlecharge.Price = 500;
                        if (messageObj.Content == "Renewal")
                            singlecharge.IsSucceeded = true;
                        else
                            singlecharge.IsSucceeded = false;
                        singlecharge.IsApplicationInformed = false;
                        singlecharge.IsCalledFromInAppPurchase = false;
                        var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                        if (installment != null)
                            singlecharge.InstallmentId = installment.Id;
                        entity.Singlecharges.Add(singlecharge);
                        entity.SaveChanges();
                    }
                }
                else if (messageObj.ShortCode == "405519")
                {
                    using (var entity = new SoratyLibrary.Models.SoratyEntities())
                    {
                        var singlecharge = new SoratyLibrary.Models.Singlecharge();
                        singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        singlecharge.DateCreated = DateTime.Now;
                        singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                        singlecharge.Price = 400;
                        if (messageObj.Content == "Renewal")
                            singlecharge.IsSucceeded = true;
                        else
                            singlecharge.IsSucceeded = false;
                        singlecharge.IsApplicationInformed = false;
                        singlecharge.IsCalledFromInAppPurchase = false;
                        var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                        if (installment != null)
                            singlecharge.InstallmentId = installment.Id;
                        entity.Singlecharges.Add(singlecharge);
                        entity.SaveChanges();
                    }
                }
                else if (messageObj.ShortCode == "405522")
                {
                    using (var entity = new DefendIranLibrary.Models.DefendIranEntities())
                    {
                        var singlecharge = new DefendIranLibrary.Models.Singlecharge();
                        singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        singlecharge.DateCreated = DateTime.Now;
                        singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                        singlecharge.Price = 400;
                        if (messageObj.Content == "Renewal")
                            singlecharge.IsSucceeded = true;
                        else
                            singlecharge.IsSucceeded = false;
                        singlecharge.IsApplicationInformed = false;
                        singlecharge.IsCalledFromInAppPurchase = false;
                        var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                        if (installment != null)
                            singlecharge.InstallmentId = installment.Id;
                        entity.Singlecharges.Add(singlecharge);
                        entity.SaveChanges();
                    }
                }
                result = "1";
            }
            else
            {
                if (NotificationId == null || NotificationId == "" || NotificationId == "null")
                {
                    var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromShortCode(messageObj.ShortCode);
                    if (serviceInfo != null)
                    {
                        messageObj.Content = serviceInfo.AggregatorServiceId;
                        messageObj.IsReceivedFromIntegratedPanel = true;
                    }
                }
                else
                    messageObj.ShortCode = to;

                messageObj.MessageId = NotificationId;
                messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);

                if (messageObj.MobileNumber == "Invalid Mobile Number")
                    result = "-1";
                else
                {
                    messageObj.ShortCode = SharedLibrary.MessageHandler.ValidateShortCode(messageObj.ShortCode);
                    if (messageObj.Content == "Subscription")
                    {
                        //messageObj.Content = keyword;
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-Notify-Register" : null;
                    }
                    else if (messageObj.Content == "Unsubscription")
                    {
                        //messageObj.Content = keyword;
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-Notify-Unsubscription" : null;
                    }
                    else
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
                    SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                    result = "1";
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Message(string text, string keyword, string channel, string from, string to, string NotificationId, string userId)
        {
            string result = "";
            text = (text == null) ? "" : text;
            text = HttpUtility.UrlDecode(text, System.Text.UnicodeEncoding.Default);
            text = HttpUtility.UrlDecode(text, System.Text.UnicodeEncoding.Default);
            var messageObj = new MessageObject();
            messageObj.MobileNumber = from;
            messageObj.Content = text;
            messageObj.ShortCode = to;
            if (messageObj.Content.Contains("Renewal"))
            {
                if (messageObj.ShortCode == "405505")
                {
                    using (var entity = new ShahreKalamehLibrary.Models.ShahreKalamehEntities())
                    {
                        var singlecharge = new ShahreKalamehLibrary.Models.Singlecharge();
                        singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        singlecharge.DateCreated = DateTime.Now;
                        singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                        singlecharge.Price = 500;
                        if (messageObj.Content == "Renewal")
                            singlecharge.IsSucceeded = true;
                        else
                            singlecharge.IsSucceeded = false;
                        singlecharge.IsApplicationInformed = false;
                        singlecharge.IsCalledFromInAppPurchase = false;
                        var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                        if (installment != null)
                            singlecharge.InstallmentId = installment.Id;
                        entity.Singlecharges.Add(singlecharge);
                        entity.SaveChanges();
                    }
                }
                else if (messageObj.ShortCode == "405519")
                {
                    using (var entity = new SoratyLibrary.Models.SoratyEntities())
                    {
                        var singlecharge = new SoratyLibrary.Models.Singlecharge();
                        singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        singlecharge.DateCreated = DateTime.Now;
                        singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                        singlecharge.Price = 400;
                        if (messageObj.Content == "Renewal")
                            singlecharge.IsSucceeded = true;
                        else
                            singlecharge.IsSucceeded = false;
                        singlecharge.IsApplicationInformed = false;
                        singlecharge.IsCalledFromInAppPurchase = false;
                        var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                        if (installment != null)
                            singlecharge.InstallmentId = installment.Id;
                        entity.Singlecharges.Add(singlecharge);
                        entity.SaveChanges();
                    }
                }
                else if (messageObj.ShortCode == "405522")
                {
                    using (var entity = new DefendIranLibrary.Models.DefendIranEntities())
                    {
                        var singlecharge = new DefendIranLibrary.Models.Singlecharge();
                        singlecharge.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        singlecharge.DateCreated = DateTime.Now;
                        singlecharge.PersianDateCreated = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                        singlecharge.Price = 400;
                        if (messageObj.Content == "Renewal")
                            singlecharge.IsSucceeded = true;
                        else
                            singlecharge.IsSucceeded = false;
                        singlecharge.IsApplicationInformed = false;
                        singlecharge.IsCalledFromInAppPurchase = false;
                        var installment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment == false).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                        if (installment != null)
                            singlecharge.InstallmentId = installment.Id;
                        entity.Singlecharges.Add(singlecharge);
                        entity.SaveChanges();
                    }
                }
                result = "1";
            }
            else
            {
                if (NotificationId == null || NotificationId == "" || NotificationId == "null")
                {
                    var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromShortCode(messageObj.ShortCode);
                    if (serviceInfo != null)
                    {
                        messageObj.Content = serviceInfo.AggregatorServiceId;
                        messageObj.IsReceivedFromIntegratedPanel = true;
                    }
                }
                else
                    messageObj.ShortCode = to;

                messageObj.MessageId = NotificationId;

                messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);

                if (messageObj.MobileNumber == "Invalid Mobile Number")
                    result = "-1";
                else
                {
                    messageObj.ShortCode = SharedLibrary.MessageHandler.ValidateShortCode(messageObj.ShortCode);
                    messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress : null;
                    SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                    result = "1";
                }
            }
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage OTPStatus(string otpId, string statusId, string recipient)
        {
            string result = "";
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage SinglechargeDelivery(string ChargeId, string StatusId, string Recipient, string AppliedPrice, string TransactionCode, string description)
        {
            var singlechargeDelivery = new SinglechargeDelivery();
            singlechargeDelivery.DateReceived = DateTime.Now;
            singlechargeDelivery.MobileNumber = "0" + Recipient;
            singlechargeDelivery.ReferenceId = ChargeId;
            singlechargeDelivery.Status = StatusId;
            singlechargeDelivery.Description = description;
            singlechargeDelivery.IsProcessed = false;
            using (var entity = new PortalEntities())
            {
                entity.SinglechargeDeliveries.Add(singlechargeDelivery);
                entity.SaveChanges();
            }
            var result = "1";
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage Delivery([FromUri] string parameters)
        {
            //var singlechargeDelivery = new SinglechargeDelivery();
            //singlechargeDelivery.DateReceived = DateTime.Now;
            //singlechargeDelivery.MobileNumber = "0" + Recipient;
            //singlechargeDelivery.ReferenceId = ChargeId;
            //singlechargeDelivery.Status = StatusId;
            //singlechargeDelivery.Description = description;
            //singlechargeDelivery.IsProcessed = false;
            //using (var entity = new PortalEntities())
            //{
            //    entity.SinglechargeDeliveries.Add(singlechargeDelivery);
            //    entity.SaveChanges();
            //}
            var result = "1";
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }
    }
}
