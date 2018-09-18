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
using System.Data.Entity;
using System.Threading.Tasks;

namespace Portal.Controllers
{
    public class AppController : ApiController
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<string> OtpAllowedServiceCodes = new List<string>() { /*"Soltan", */ "DonyayeAsatir", "MenchBaz", "Soraty", "DefendIran", "AvvalYad", "BehAmooz500", "Darchin" };
        private List<string> AppMessageAllowedServiceCode = new List<string>() { /*"Soltan",*/ "ShahreKalameh", "DonyayeAsatir", "Tamly", "JabehAbzar", "ShenoYad", "FitShow", "Takavar", "MenchBaz", "AvvalPod", "AvvalYad", "Soraty", "DefendIran", "TahChin", "Nebula", "Dezhban", "MusicYad", "Phantom", "Medio", "BehAmooz500", "ShenoYad500", "Tamly500", "AvvalPod500", "Darchin", "Dambel", "Aseman", "Medad", "PorShetab", "TajoTakht", "LahzeyeAkhar", "Hazaran", "JhoobinDambel", "JhoobinMedad", "JhoobinMusicYad", "JhoobinPin", "JhoobinPorShetab", "JhoobinTahChin" };
        private List<string> VerificactionAllowedServiceCode = new List<string>() { /*"Soltan",*/ "ShahreKalameh", "DonyayeAsatir", "Tamly", "JabehAbzar", "ShenoYad", "FitShow", "Takavar", "MenchBaz", "AvvalPod", "AvvalYad", "Soraty", "DefendIran", "TahChin", "Nebula", "Dezhban", "MusicYad", "Phantom", "Medio", "BehAmooz500", "ShenoYad500", "Tamly500", "AvvalPod500", "Darchin", "Dambel", "Aseman", "Medad", "PorShetab", "TajoTakht", "LahzeyeAkhar", "Hazaran", "JhoobinDambel", "JhoobinMedad", "JhoobinMusicYad", "JhoobinPin", "JhoobinPorShetab", "JhoobinTahChin" };
        private List<string> TimeBasedServices = new List<string>() { "ShahreKalameh", "Tamly", "JabehAbzar", "ShenoYad", "FitShow", "Takavar", "AvvalPod", "TahChin", "Nebula", "Dezhban", "MusicYad", "Phantom", "Medio", "ShenoYad500", "Tamly500", "AvvalPod500", "Darchin", "Dambel", "Medad", "PorShetab", "TajoTakht", "LahzeyeAkhar", "Hazaran", "JhoobinDambel", "JhoobinMedad", "JhoobinMusicYad", "JhoobinPin", "JhoobinPorShetab", "JhoobinTahChin" };
        private List<string> PriceBasedServices = new List<string>() { /*"Soltan",*/ "DonyayeAsatir", "MenchBaz", "Soraty", "DefendIran", "AvvalYad", "BehAmooz500", "Darchin" };

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> OtpCharge([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            try
            {
                if (messageObj.Number != null)
                {
                    messageObj.MobileNumber = messageObj.Number;
                    result.Number = messageObj.MobileNumber;
                }
                else
                    result.MobileNumber = messageObj.MobileNumber;
                var hash = SharedLibrary.Security.GetSha256Hash("OtpCharge" + messageObj.ServiceCode + messageObj.MobileNumber);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    if (messageObj.Price == null)
                        messageObj.Price = 0;
                    if (messageObj.ServiceCode == "NabardGah")
                        messageObj.ServiceCode = "Soltan";
                    else if (messageObj.ServiceCode == "ShenoYad")
                        messageObj.ServiceCode = "ShenoYad500";
                    else if (!OtpAllowedServiceCodes.Contains(messageObj.ServiceCode) && messageObj.Price.Value > 7) // Hub use price 5 and 6 for sub and unsub
                        result.Status = "This ServiceCode does not have permission for OTP operation";
                    else
                    {
                        messageObj.MobileNumber = SharedLibrary.MessageHandler.NormalizeContent(messageObj.MobileNumber);
                        if (messageObj.Number != null)
                        {
                            messageObj.Number = SharedLibrary.MessageHandler.NormalizeContent(messageObj.Number);
                            messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateLandLineNumber(messageObj.Number);
                        }
                        else
                            messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                        if (messageObj.MobileNumber == "Invalid Mobile Number")
                            result.Status = "Invalid Mobile Number";
                        else if (messageObj.MobileNumber == "Invalid Number")
                            result.Status = "Invalid Number";
                        else
                        {
                            var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                            if (service == null)
                                result.Status = "Invalid serviceCode";
                            else
                            {
                                var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(service.Id);
                                if (messageObj.Content != null)
                                {
                                    using (var entity = new PortalEntities())
                                    {
                                        var mo = new ReceievedMessage()
                                        {
                                            MobileNumber = messageObj.MobileNumber,
                                            ShortCode = serviceInfo.ShortCode,
                                            ReceivedTime = DateTime.Now,
                                            PersianReceivedTime = SharedLibrary.Date.GetPersianDateTime(DateTime.Now),
                                            Content = (messageObj.Content == null) ? " " : messageObj.Content,
                                            IsProcessed = true,
                                            IsReceivedFromIntegratedPanel = false,
                                            IsReceivedFromWeb = false,
                                            ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-OtpCharge" : null
                                        };
                                        entity.ReceievedMessages.Add(mo);
                                        entity.SaveChanges();
                                    }
                                }
                                var subscriber = SharedLibrary.HandleSubscription.GetSubscriber(messageObj.MobileNumber, service.Id);
                                //if (messageObj.Price.Value == 0 && subscriber != null && subscriber.DeactivationDate == null)
                                //    result.Status = "User already subscribed";
                                //else
                                //{
                                var minuetesBackward = DateTime.Now.AddMinutes(-5);
                                if (service.ServiceCode == "Phantom")
                                {
                                    using (var entity = new PhantomLibrary.Models.PhantomEntities())
                                    {
                                        var imiChargeCode = new PhantomLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = PhantomLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = PhantomLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = PhantomLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content; 
                                            var logId = PhantomLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new PhantomLibrary.Models.Singlecharge();
                                                string aggregatorName = "MobinOneMapfa";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MapfaOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            PhantomLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "TajoTakht")
                                {
                                    using (var entity = new TajoTakhtLibrary.Models.TajoTakhtEntities())
                                    {
                                        var imiChargeCode = new TajoTakhtLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = TajoTakhtLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = TajoTakhtLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = TajoTakhtLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = TajoTakhtLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new TajoTakhtLibrary.Models.Singlecharge();
                                                string aggregatorName = "MobinOneMapfa";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MapfaOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            TajoTakhtLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Hazaran")
                                {
                                    using (var entity = new HazaranLibrary.Models.HazaranEntities())
                                    {
                                        var imiChargeCode = new HazaranLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = HazaranLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = HazaranLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = HazaranLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = HazaranLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new HazaranLibrary.Models.Singlecharge();
                                                string aggregatorName = "MobinOneMapfa";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MapfaOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            HazaranLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "LahzeyeAkhar")
                                {
                                    using (var entity = new LahzeyeAkharLibrary.Models.LahzeyeAkharEntities())
                                    {
                                        var imiChargeCode = new LahzeyeAkharLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = LahzeyeAkharLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = LahzeyeAkharLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = LahzeyeAkharLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = LahzeyeAkharLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new LahzeyeAkharLibrary.Models.Singlecharge();
                                                string aggregatorName = "MobinOneMapfa";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MapfaOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            LahzeyeAkharLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Soltan")
                                {
                                    using (var entity = new SoltanLibrary.Models.SoltanEntities())
                                    {
                                        var imiChargeCode = new SoltanLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = SoltanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = SoltanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = SoltanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = SoltanLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new SoltanLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            SoltanLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "MenchBaz")
                                {
                                    using (var entity = new MenchBazLibrary.Models.MenchBazEntities())
                                    {
                                        var imiChargeCode = new MenchBazLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = MenchBazLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = MenchBazLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = MenchBazLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = MenchBazLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new MenchBazLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            MenchBazLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "DonyayeAsatir")
                                {
                                    using (var entity = new DonyayeAsatirLibrary.Models.DonyayeAsatirEntities())
                                    {
                                        var imiChargeCode = new DonyayeAsatirLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = DonyayeAsatirLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = DonyayeAsatirLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = DonyayeAsatirLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = DonyayeAsatirLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new DonyayeAsatirLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            DonyayeAsatirLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Aseman")
                                {
                                    using (var entity = new AsemanLibrary.Models.AsemanEntities())
                                    {
                                        var imiChargeCode = new AsemanLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = AsemanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = AsemanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = AsemanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = AsemanLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new AsemanLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            AsemanLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "AvvalPod")
                                {
                                    using (var entity = new AvvalPodLibrary.Models.AvvalPodEntities())
                                    {
                                        var imiChargeCode = new AvvalPodLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = AvvalPodLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = AvvalPodLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = AvvalPodLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new AvvalPodLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "AvvalPod500")
                                {
                                    using (var entity = new AvvalPod500Library.Models.AvvalPod500Entities())
                                    {
                                        var imiChargeCode = new AvvalPod500Library.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = AvvalPod500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = AvvalPod500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = AvvalPod500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = AvvalPod500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new AvvalPod500Library.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            AvvalPod500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "AvvalYad")
                                {
                                    using (var entity = new AvvalYadLibrary.Models.AvvalYadEntities())
                                    {
                                        var imiChargeCode = new AvvalYadLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = AvvalYadLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = AvvalYadLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = AvvalYadLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new AvvalYadLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "BehAmooz500")
                                {
                                    using (var entity = new BehAmooz500Library.Models.BehAmooz500Entities())
                                    {
                                        var imiChargeCode = new BehAmooz500Library.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = BehAmooz500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = BehAmooz500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = BehAmooz500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = BehAmooz500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new BehAmooz500Library.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            BehAmooz500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "FitShow")
                                {
                                    using (var entity = new FitShowLibrary.Models.FitShowEntities())
                                    {
                                        var imiChargeCode = new FitShowLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = FitShowLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = FitShowLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = FitShowLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = FitShowLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new FitShowLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            FitShowLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "JabehAbzar")
                                {
                                    using (var entity = new JabehAbzarLibrary.Models.JabehAbzarEntities())
                                    {
                                        var imiChargeCode = new JabehAbzarLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = JabehAbzarLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = JabehAbzarLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = JabehAbzarLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = JabehAbzarLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new JabehAbzarLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            JabehAbzarLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "ShenoYad")
                                {
                                    using (var entity = new ShenoYadLibrary.Models.ShenoYadEntities())
                                    {
                                        var imiChargeCode = new ShenoYadLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = ShenoYadLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = ShenoYadLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = ShenoYadLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = ShenoYadLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new ShenoYadLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            ShenoYadLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "ShenoYad500")
                                {
                                    using (var entity = new ShenoYad500Library.Models.ShenoYad500Entities())
                                    {
                                        var imiChargeCode = new ShenoYad500Library.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = ShenoYad500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = ShenoYad500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = ShenoYad500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = ShenoYad500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new ShenoYad500Library.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            ShenoYad500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Takavar")
                                {
                                    using (var entity = new TakavarLibrary.Models.TakavarEntities())
                                    {
                                        var imiChargeCode = new TakavarLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = TakavarLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = TakavarLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = TakavarLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = TakavarLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new TakavarLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            TakavarLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Tamly")
                                {
                                    using (var entity = new TamlyLibrary.Models.TamlyEntities())
                                    {
                                        var imiChargeCode = new TamlyLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = TamlyLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = TamlyLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = TamlyLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = TamlyLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new TamlyLibrary.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            TamlyLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Tamly500")
                                {
                                    using (var entity = new Tamly500Library.Models.Tamly500Entities())
                                    {
                                        var imiChargeCode = new Tamly500Library.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = Tamly500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = Tamly500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = Tamly500Library.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = Tamly500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new Tamly500Library.Models.Singlecharge();
                                                string aggregatorName = "Telepromo";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.TelepromoOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            Tamly500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Dezhban")
                                {
                                    using (var entity = new DezhbanLibrary.Models.DezhbanEntities())
                                    {
                                        var imiChargeCode = new DezhbanLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = DezhbanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = DezhbanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = DezhbanLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = DezhbanLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new DezhbanLibrary.Models.Singlecharge();
                                                string aggregatorName = "MciDirect";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MciDirectOtpCharge(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            DezhbanLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "ShahreKalameh")
                                {
                                    using (var entity = new ShahreKalamehLibrary.Models.ShahreKalamehEntities())
                                    {
                                        var imiChargeCode = new ShahreKalamehLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = ShahreKalamehLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = ShahreKalamehLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = ShahreKalamehLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = ShahreKalamehLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new ShahreKalamehLibrary.Models.Singlecharge();
                                                string aggregatorName = "MciDirect";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MciDirectOtpCharge(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            ShahreKalamehLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Soraty")
                                {
                                    using (var entity = new SoratyLibrary.Models.SoratyEntities())
                                    {
                                        var imiChargeCode = new SoratyLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = SoratyLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = SoratyLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = SoratyLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = SoratyLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new SoratyLibrary.Models.Singlecharge();
                                                string aggregatorName = "MciDirect";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MciDirectOtpCharge(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                                if (result.Status == "SUCCESS-Pending Confirmation")
                                                {

                                                    var messagesTemplate = SoratyLibrary.ServiceHandler.GetServiceMessagesTemplate();
                                                    int isCampaignActive = 0;
                                                    var campaign = entity.Settings.FirstOrDefault(o => o.Name == "campaign");
                                                    if (campaign != null)
                                                        isCampaignActive = Convert.ToInt32(campaign.Value);
                                                    var isInBlackList = SharedLibrary.MessageHandler.IsInBlackList(messageObj.MobileNumber, service.Id);
                                                    if (isInBlackList == true)
                                                        isCampaignActive = 0;
                                                    if (isCampaignActive == 1)
                                                    {
                                                        SharedLibrary.HandleSubscription.AddToTempReferral(messageObj.MobileNumber, service.Id, messageObj.Content);
                                                        messageObj.ShortCode = serviceInfo.ShortCode;
                                                        messageObj.MessageType = (int)SharedLibrary.MessageHandler.MessageType.OnDemand;
                                                        messageObj.ProcessStatus = (int)SharedLibrary.MessageHandler.ProcessStatus.TryingToSend;
                                                        messageObj.Content = messagesTemplate.Where(o => o.Title == "CampaignOtpFromUniqueId").Select(o => o.Content).FirstOrDefault();
                                                        SoratyLibrary.MessageHandler.InsertMessageToQueue(messageObj);
                                                    }
                                                }
                                            }
                                            SoratyLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "DefendIran")
                                {
                                    using (var entity = new DefendIranLibrary.Models.DefendIranEntities())
                                    {
                                        var imiChargeCode = new DefendIranLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = DefendIranLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = DefendIranLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = DefendIranLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = DefendIranLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new DefendIranLibrary.Models.Singlecharge();
                                                string aggregatorName = "MciDirect";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MciDirectOtpCharge(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            DefendIranLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "SepidRood")
                                {
                                    using (var entity = new SepidRoodLibrary.Models.SepidRoodEntities())
                                    {
                                        var imiChargeCode = new SepidRoodLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = SharedLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = SharedLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = SharedLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new SepidRoodLibrary.Models.Singlecharge();
                                                string aggregatorName = "PardisImi";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.PardisImiOtpChargeRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Nebula")
                                {
                                    using (var entity = new NebulaLibrary.Models.NebulaEntities())
                                    {
                                        var imiChargeCode = new NebulaLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = NebulaLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = NebulaLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = NebulaLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = NebulaLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new NebulaLibrary.Models.Singlecharge();
                                                string aggregatorName = "MobinOne";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MobinOneOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            NebulaLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Medio")
                                {
                                    using (var entity = new MedioLibrary.Models.MedioEntities())
                                    {
                                        var imiChargeCode = new MedioLibrary.Models.ImiChargeCode();
                                        if (messageObj.Price.Value == 0)
                                            messageObj = MedioLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                                        else if (messageObj.Price.Value == -1)
                                        {
                                            messageObj.Price = 0;
                                            messageObj = MedioLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                                        }
                                        else
                                            messageObj = MedioLibrary.MessageHandler.SetImiChargeInfo(entity, imiChargeCode, messageObj, messageObj.Price.Value, 0, null);
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var otpMessage = "webservice";
                                            if (messageObj.Content != null)
                                                otpMessage = otpMessage + "-" + messageObj.Content;
                                            var logId = MedioLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "request", otpMessage);
                                            var isOtpExists = entity.Singlecharges.Where(o => o.MobileNumber == messageObj.MobileNumber && o.Price == 0 && o.Description == "SUCCESS-Pending Confirmation" && o.DateCreated > minuetesBackward).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                            if (isOtpExists != null && messageObj.Price.Value == 0)
                                            {
                                                result.Status = "Otp request already exists for this subscriber";
                                            }
                                            else
                                            {
                                                var singleCharge = new MedioLibrary.Models.Singlecharge();
                                                string aggregatorName = "MobinOneMapfa";
                                                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                                singleCharge = await SharedLibrary.MessageSender.MapfaOTPRequest(entity, singleCharge, messageObj, serviceAdditionalInfo);
                                                result.Status = singleCharge.Description;
                                            }
                                            MedioLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                        }
                                    }
                                }
                                else if (service.ServiceCode == "Darchin")
                                {
                                    using (var entity = new DarchinLibrary.Models.DarchinEntities())
                                    {
                                        if (messageObj.Price.Value < 0)
                                        {
                                            messageObj.Content = "Unsubscribe";
                                        }
                                        else if (messageObj.Price != 7000)
                                            messageObj.Price = null;
                                        if (messageObj.Price == null)
                                            result.Status = "Invalid Price";
                                        else
                                        {
                                            var singleCharge = new DarchinLibrary.Models.Singlecharge();
                                            string aggregatorName = "SamssonTci";
                                            var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                            if (messageObj.Price >= 0)
                                                singleCharge = await SharedLibrary.MessageSender.SamssonTciOTPRequest(typeof(DarchinLibrary.Models.DarchinEntities), singleCharge, messageObj, serviceAdditionalInfo);
                                            else
                                            {
                                                var activeTokens = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && o.IsUserCanceledTheInstallment != true).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                                if (activeTokens != null)
                                                {
                                                    messageObj.Token = activeTokens.UserToken;
                                                    singleCharge = await SharedLibrary.MessageSender.SamssonTciSinglecharge(typeof(DarchinLibrary.Models.DarchinEntities), typeof(DarchinLibrary.Models.Singlecharge), messageObj, serviceAdditionalInfo, true);
                                                }
                                                else
                                                    singleCharge.Description = "User Does Not Exists";
                                                DarchinLibrary.HandleMo.ReceivedMessage(messageObj, service);
                                            }
                                            result.Status = singleCharge.Description;
                                            result.Token = singleCharge.ReferenceId;
                                        }
                                    }
                                }
                                else
                                    result.Status = "Service does not defined";
                                //}
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Excepiton in OtpCharge method: ", e);
            }
            var json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> OtpConfirm([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            try
            {
                if (messageObj.Number != null)
                {
                    messageObj.MobileNumber = messageObj.Number;
                    result.Number = messageObj.MobileNumber;
                }
                else
                    result.MobileNumber = messageObj.MobileNumber;
                var hash = SharedLibrary.Security.GetSha256Hash("OtpConfirm" + messageObj.ServiceCode + messageObj.MobileNumber);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    if (messageObj.ServiceCode == "NabardGah")
                        messageObj.ServiceCode = "Soltan";
                    else if (messageObj.ServiceCode == "ShenoYad")
                        messageObj.ServiceCode = "ShenoYad500";

                    messageObj.MobileNumber = SharedLibrary.MessageHandler.NormalizeContent(messageObj.MobileNumber);
                    if (messageObj.Number != null)
                    {
                        messageObj.Number = SharedLibrary.MessageHandler.NormalizeContent(messageObj.Number);
                        messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateLandLineNumber(messageObj.Number);
                    }
                    else
                        messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                    
                    if (messageObj.MobileNumber == "Invalid Mobile Number")
                        result.Status = "Invalid Mobile Number";
                    else if (messageObj.MobileNumber == "Invalid Number")
                        result.Status = "Invalid Number";
                    else
                    {
                        var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                        if (service == null)
                            result.Status = "Invalid Service Code";
                        else
                        {
                            var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(service.Id);
                            if (messageObj.Content != null)
                            {
                                using (var entity = new PortalEntities())
                                {
                                    var mo = new ReceievedMessage()
                                    {
                                        MobileNumber = messageObj.MobileNumber,
                                        ShortCode = serviceInfo.ShortCode,
                                        ReceivedTime = DateTime.Now,
                                        PersianReceivedTime = SharedLibrary.Date.GetPersianDateTime(DateTime.Now),
                                        Content = (messageObj.Content == null) ? " " : messageObj.Content,
                                        IsProcessed = true,
                                        IsReceivedFromIntegratedPanel = false,
                                        IsReceivedFromWeb = false,
                                        ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-OtpConfirm" : null
                                    };
                                    entity.ReceievedMessages.Add(mo);
                                    entity.SaveChanges();
                                }
                            }
                            if (service.ServiceCode == "Phantom")
                            {
                                using (var entity = new PhantomLibrary.Models.PhantomEntities())
                                {
                                    var logId = PhantomLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = PhantomLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MobinOneMapfa";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MapfaOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    PhantomLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "TajoTakht")
                            {
                                using (var entity = new TajoTakhtLibrary.Models.TajoTakhtEntities())
                                {
                                    var logId = TajoTakhtLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = TajoTakhtLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MobinOneMapfa";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MapfaOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    TajoTakhtLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Hazaran")
                            {
                                using (var entity = new HazaranLibrary.Models.HazaranEntities())
                                {
                                    var logId = HazaranLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = HazaranLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MobinOneMapfa";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MapfaOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    HazaranLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "LahzeyeAkhar")
                            {
                                using (var entity = new LahzeyeAkharLibrary.Models.LahzeyeAkharEntities())
                                {
                                    var logId = LahzeyeAkharLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = LahzeyeAkharLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MobinOneMapfa";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MapfaOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    LahzeyeAkharLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Soltan")
                            {
                                using (var entity = new SoltanLibrary.Models.SoltanEntities())
                                {
                                    var logId = SoltanLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = SoltanLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    SoltanLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "MenchBaz")
                            {
                                using (var entity = new MenchBazLibrary.Models.MenchBazEntities())
                                {
                                    var logId = MenchBazLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = MenchBazLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    MenchBazLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Aseman")
                            {
                                using (var entity = new AsemanLibrary.Models.AsemanEntities())
                                {
                                    var logId = AsemanLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = AsemanLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    AsemanLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "AvvalPod")
                            {
                                using (var entity = new AvvalPodLibrary.Models.AvvalPodEntities())
                                {
                                    var singleCharge = AvvalPodLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                }
                            }
                            else if (service.ServiceCode == "AvvalPod500")
                            {
                                using (var entity = new AvvalPod500Library.Models.AvvalPod500Entities())
                                {
                                    var logId = AvvalPod500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = AvvalPod500Library.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    AvvalPod500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "AvvalYad")
                            {
                                using (var entity = new AvvalYadLibrary.Models.AvvalYadEntities())
                                {
                                    var singleCharge = AvvalYadLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                }
                            }
                            else if (service.ServiceCode == "BehAmooz500")
                            {
                                using (var entity = new BehAmooz500Library.Models.BehAmooz500Entities())
                                {
                                    var logId = BehAmooz500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = BehAmooz500Library.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    BehAmooz500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "DonyayeAsatir")
                            {
                                using (var entity = new DonyayeAsatirLibrary.Models.DonyayeAsatirEntities())
                                {
                                    var logId = DonyayeAsatirLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = DonyayeAsatirLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    DonyayeAsatirLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "FitShow")
                            {
                                using (var entity = new FitShowLibrary.Models.FitShowEntities())
                                {
                                    var logId = FitShowLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = FitShowLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    FitShowLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "JabehAbzar")
                            {
                                using (var entity = new JabehAbzarLibrary.Models.JabehAbzarEntities())
                                {
                                    var logId = JabehAbzarLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = JabehAbzarLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    JabehAbzarLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "ShenoYad")
                            {
                                using (var entity = new ShenoYadLibrary.Models.ShenoYadEntities())
                                {
                                    var logId = ShenoYadLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = ShenoYadLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    ShenoYadLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "ShenoYad500")
                            {
                                using (var entity = new ShenoYad500Library.Models.ShenoYad500Entities())
                                {
                                    var logId = ShenoYad500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = ShenoYad500Library.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    ShenoYad500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Takavar")
                            {
                                using (var entity = new TakavarLibrary.Models.TakavarEntities())
                                {
                                    var logId = TakavarLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = TakavarLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    TakavarLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Tamly")
                            {
                                var logId = TamlyLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                using (var entity = new TamlyLibrary.Models.TamlyEntities())
                                {
                                    var singleCharge = TamlyLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    TamlyLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Tamly500")
                            {
                                using (var entity = new Tamly500Library.Models.Tamly500Entities())
                                {
                                    var logId = Tamly500Library.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = Tamly500Library.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "Telepromo";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.TelepromoOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    Tamly500Library.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Dezhban")
                            {
                                using (var entity = new DezhbanLibrary.Models.DezhbanEntities())
                                {
                                    var logId = DezhbanLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = DezhbanLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MciDirect";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MciDirectOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    DezhbanLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "ShahreKalameh")
                            {
                                using (var entity = new ShahreKalamehLibrary.Models.ShahreKalamehEntities())
                                {
                                    var logId = ShahreKalamehLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = ShahreKalamehLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MciDirect";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MciDirectOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    ShahreKalamehLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Soraty")
                            {
                                using (var entity = new SoratyLibrary.Models.SoratyEntities())
                                {
                                    var logId = SoratyLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = SoratyLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MciDirect";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MciDirectOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    SoratyLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "DefendIran")
                            {
                                using (var entity = new DefendIranLibrary.Models.DefendIranEntities())
                                {
                                    var logId = DefendIranLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = DefendIranLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MciDirect";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MciDirectOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    DefendIranLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "SepidRood")
                            {
                                using (var entity = new SepidRoodLibrary.Models.SepidRoodEntities())
                                {
                                    var singleCharge = new SepidRoodLibrary.Models.Singlecharge();
                                    singleCharge = SharedLibrary.MessageHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "PardisImi";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.PardisImiOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                }
                            }
                            else if (service.ServiceCode == "Nebula")
                            {
                                using (var entity = new NebulaLibrary.Models.NebulaEntities())
                                {
                                    var logId = NebulaLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = NebulaLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MobinOne";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MobinOneOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    NebulaLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Medio")
                            {
                                using (var entity = new MedioLibrary.Models.MedioEntities())
                                {
                                    var logId = MedioLibrary.MessageHandler.OtpLog(messageObj.MobileNumber, "confirm", "webservice-" + messageObj.ConfirmCode);
                                    var singleCharge = MedioLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        string aggregatorName = "MobinOneMapfa";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.MapfaOTPConfirm(entity, singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        result.Status = singleCharge.Description;
                                    }
                                    MedioLibrary.MessageHandler.OtpLogUpdate(logId, result.Status.ToString());
                                }
                            }
                            else if (service.ServiceCode == "Darchin")
                            {
                                using (var entity = new DarchinLibrary.Models.DarchinEntities())
                                {
                                    var singleCharge = DarchinLibrary.ServiceHandler.GetOTPRequestId(entity, messageObj);
                                    if (singleCharge == null)
                                        result.Status = "No Otp Request Found";
                                    else
                                    {
                                        messageObj.Price = singleCharge.Price;
                                        messageObj.Token = singleCharge.ReferenceId;
                                        var token = singleCharge.ReferenceId;
                                        string aggregatorName = "SamssonTci";
                                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                        singleCharge = await SharedLibrary.MessageSender.SamssonTciOTPConfirm(typeof(DarchinLibrary.Models.DarchinEntities), singleCharge, messageObj, serviceAdditionalInfo, messageObj.ConfirmCode);
                                        if (singleCharge.Description == "SUCCESS")
                                        {
                                            messageObj.Content = "Register";
                                            DarchinLibrary.HandleMo.ReceivedMessage(messageObj, service);
                                        }
                                        result.Status = singleCharge.Description;
                                        result.Token = token;
                                    }
                                }
                            }
                            else
                                result.Status = "Service does not defined";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Excepiton in OtpConfirm method: ", e);
            }
            var json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> WebMessage([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            result.Status = "";
            try
            {
                var hash = SharedLibrary.Security.GetSha256Hash("WebMessage" + messageObj.ServiceCode + messageObj.MobileNumber);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else if (messageObj.Content == null)
                    result.Status = "Content cannot be null";
                else if (messageObj.ServiceCode == null)
                    result.Status = "ServiceCode cannot be null";
                else
                {
                    messageObj.MobileNumber = SharedLibrary.MessageHandler.NormalizeContent(messageObj.MobileNumber);
                    var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                    if (service == null)
                        result.Status = "Invalid ServiceCode";
                    else
                    {
                        var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(service.Id);
                        using (var entity = new PortalEntities())
                        {
                            var mo = new ReceievedMessage()
                            {
                                MobileNumber = messageObj.MobileNumber,
                                ShortCode = serviceInfo.ShortCode,
                                ReceivedTime = DateTime.Now,
                                PersianReceivedTime = SharedLibrary.Date.GetPersianDateTime(DateTime.Now),
                                Content = (messageObj.Content == null) ? " " : messageObj.Content,
                                IsProcessed = false,
                                IsReceivedFromIntegratedPanel = false,
                                IsReceivedFromWeb = false,
                                ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-WebMessage" : null
                            };
                            entity.ReceievedMessages.Add(mo);
                            entity.SaveChanges();
                        }
                        result.Status = "Success";
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in WebMessage: ", e);
                result.Status = "Failed";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Register([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            result.Token = messageObj.Token;
            result.Status = "";
            try
            {
                var hash = SharedLibrary.Security.GetSha256Hash("Register" + messageObj.ServiceCode + messageObj.Number);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                    if (service == null)
                        result.Status = "Invalid serviceCode";
                    else
                    {
                        if (service.ServiceCode == "Darchin")
                        {
                            using (var entity = new DarchinLibrary.Models.DarchinEntities())
                            {
                                if (messageObj.Price.Value == 7000)
                                {
                                    messageObj.Content = "ir.darchin.app;Darchin123";
                                }
                                if (messageObj.Price != 7000)
                                    result.Status = "Invalid Price";
                                else
                                {
                                    var singleCharge = new DarchinLibrary.Models.Singlecharge();
                                    string aggregatorName = "SamssonTci";
                                    var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);
                                    singleCharge = await SharedLibrary.MessageSender.SamssonTciOTPRequest(typeof(DarchinLibrary.Models.DarchinEntities), singleCharge, messageObj, serviceAdditionalInfo);
                                    result.Status = singleCharge.Description;
                                    result.Token = singleCharge.ReferenceId;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in Deregister:" + e);
                result.Status = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<HttpResponseMessage> Deregister([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            result.Token = messageObj.Token;
            result.Status = "";
            try
            {
                var hash = SharedLibrary.Security.GetSha256Hash("Deregister" + messageObj.ServiceCode + messageObj.Token);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                    if (service == null)
                        result.Status = "Invalid serviceCode";
                    else
                    {
                        if (service.ServiceCode == "Darchin")
                        {
                            using (var entity = new DarchinLibrary.Models.DarchinEntities())
                            {
                                if (messageObj.Price.Value == -7000)
                                {
                                    messageObj.Content = "ir.darchin.app;Darchin123";
                                }
                                else
                                    result.Status = "Invalid Price";
                                if (result.Status != "Invalid Price")
                                {
                                    var singleCharge = new DarchinLibrary.Models.Singlecharge();
                                    string aggregatorName = "SamssonTci";
                                    var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage(messageObj.ServiceCode, aggregatorName);

                                    singleCharge = await SharedLibrary.MessageSender.SamssonTciSinglecharge(typeof(DarchinLibrary.Models.DarchinEntities), typeof(DarchinLibrary.Models.Singlecharge), messageObj, serviceAdditionalInfo, true);
                                    result.Status = singleCharge.IsSucceeded;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in Deregister:" + e);
                result.Status = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(json, System.Text.Encoding.UTF8, "text/plain");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage AppMessage([FromBody]MessageObject messageObj)
        {
            logs.Info("1");
            dynamic result = new ExpandoObject();
            try
            {
                result.Status = "error";
                var hash = SharedLibrary.Security.GetSha256Hash("AppMessage" + messageObj.ServiceCode + messageObj.MobileNumber + messageObj.Content.Substring(0,3));
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    if (messageObj.ServiceCode == "NabardGah")
                        messageObj.ServiceCode = "Soltan";

                    var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                    var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(service.Id);
                    messageObj.ShortCode = serviceInfo.ShortCode;
                    messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                    if (messageObj.MobileNumber == "Invalid Mobile Number")
                        result.Status = "Invalid Mobile Number";
                    else if (!AppMessageAllowedServiceCode.Contains(messageObj.ServiceCode))
                        result.Status = "This ServiceCode does not have permission";
                    else
                    {
                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-FromApp" : null;
                        SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                        result.Status = "Success";
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in AppMessage:" + e);
                result.Status = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage IsUserSubscribed([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            if (messageObj.Number != null)
            {
                messageObj.MobileNumber = messageObj.Number;
                result.Number = messageObj.MobileNumber;
            }
            else
                result.MobileNumber = messageObj.MobileNumber;
            try
            {
                var hash = SharedLibrary.Security.GetSha256Hash("IsUserSubscribed" + messageObj.ServiceCode + messageObj.MobileNumber);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    messageObj.MobileNumber = SharedLibrary.MessageHandler.NormalizeContent(messageObj.MobileNumber);
                    if (messageObj.Number != null)
                    {
                        messageObj.Number = SharedLibrary.MessageHandler.NormalizeContent(messageObj.Number);
                        messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateLandLineNumber(messageObj.Number);
                    }
                    else
                        messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                    
                    if (messageObj.MobileNumber == "Invalid Mobile Number")
                        result.Status = "Invalid Mobile Number";
                    else if (messageObj.MobileNumber == "Invalid Number")
                        result.Status = "Invalid Number";
                    else if (!VerificactionAllowedServiceCode.Contains(messageObj.ServiceCode))
                        result.Status = "This ServiceCode does not have permission";
                    else
                    {
                        var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                        if (service == null)
                        {
                            result.Status = "Invalid ServiceCode";
                        }
                        else
                        {
                            var subscriber = SharedLibrary.HandleSubscription.GetSubscriber(messageObj.MobileNumber, service.Id);
                            if (subscriber != null && subscriber.DeactivationDate == null)
                                result.Status = "Subscribed";
                            else
                                result.Status = "NotSubscribed";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in IsUserSubscribed:" + e);
                result.Status = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Verification([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            result.MobileNumber = messageObj.MobileNumber;
            try
            {
                var hash = SharedLibrary.Security.GetSha256Hash("Verification" + messageObj.ServiceCode + messageObj.MobileNumber);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    if (messageObj.ServiceCode == "NabardGah")
                        messageObj.ServiceCode = "Soltan";
                    messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                    if (messageObj.MobileNumber == "Invalid Mobile Number")
                        result.Status = "Invalid Mobile Number";
                    else if (!VerificactionAllowedServiceCode.Contains(messageObj.ServiceCode))
                        result.Status = "This ServiceCode does not have permission";
                    else
                    {
                        if (messageObj.ServiceCode == "Tamly")
                            messageObj.ServiceCode = "Tamly500";
                        else if (messageObj.ServiceCode == "AvvalPod")
                            messageObj.ServiceCode = "AvvalPod500";
                        else if (messageObj.ServiceCode == "AvvalYad")
                            messageObj.ServiceCode = "BehAmooz500";
                        else if (messageObj.ServiceCode == "ShenoYad")
                            messageObj.ServiceCode = "ShenoYad500";
                        var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                        if (service == null)
                        {
                            result.Status = "Invalid ServiceCode";
                        }
                        else
                        {
                            messageObj.ServiceId = service.Id;
                            messageObj.ShortCode = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(service.Id).ShortCode;
                            var subscriber = SharedLibrary.HandleSubscription.GetSubscriber(messageObj.MobileNumber, messageObj.ServiceId);
                            if (subscriber != null && subscriber.DeactivationDate == null)
                            {
                                Random random = new Random();
                                var verficationId = random.Next(1000, 9999).ToString();
                                messageObj.Content = "SendVerification-" + verficationId;
                                if (messageObj.ServiceCode == "Tamly500" || messageObj.ServiceCode == "ShenoYad500" || messageObj.ServiceCode == "AvvalPod500" || messageObj.ServiceCode == "BehAmooz500")
                                    messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-New500-AppVerification" : null;
                                else
                                    messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-AppVerification" : null;
                                SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                                result.Status = "Subscribed";
                                result.ActivationCode = verficationId;
                            }
                            else
                            {
                                if (service.ServiceCode == "DonyayeAsatir")
                                {
                                    var sub = SharedLibrary.HandleSubscription.GetSubscriber(messageObj.MobileNumber, 10004);
                                    if (sub != null && sub.DeactivationDate == null)
                                    {
                                        messageObj.ServiceId = 10004;
                                        messageObj.ShortCode = "307229";
                                        Random random = new Random();
                                        var verficationId = random.Next(1000, 9999).ToString();
                                        messageObj.Content = "SendVerification-" + verficationId;
                                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-AppVerification" : null;
                                        SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                                        result.Status = "Subscribed";
                                        result.ActivationCode = verficationId;
                                    }
                                    else
                                    {
                                        messageObj.Content = "SendServiceSubscriptionHelp";
                                        if (messageObj.ServiceCode == "Tamly500" || messageObj.ServiceCode == "ShenoYad500" || messageObj.ServiceCode == "AvvalPod500" || messageObj.ServiceCode == "BehAmooz500")
                                            messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-New500-Verification" : null;
                                        else
                                            messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-Verification" : null;
                                        SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                                        result.Status = "NotSubscribed";
                                        result.ActivationCode = null;
                                    }
                                }
                                else
                                {
                                    messageObj.Content = "SendServiceSubscriptionHelp";
                                    if (messageObj.ServiceCode == "Tamly500" || messageObj.ServiceCode == "ShenoYad500" || messageObj.ServiceCode == "AvvalPod500" || messageObj.ServiceCode == "BehAmooz500")
                                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-New500-Verification" : null;
                                    else
                                        messageObj.ReceivedFrom = HttpContext.Current != null ? HttpContext.Current.Request.UserHostAddress + "-Verification" : null;
                                    SharedLibrary.MessageHandler.SaveReceivedMessage(messageObj);
                                    result.Status = "NotSubscribed";
                                    result.ActivationCode = null;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in Verification:" + e);
                result.Status = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage SubscriberStatus([FromBody]MessageObject messageObj)
        {
            dynamic result = new ExpandoObject();
            result.MobileNumber = messageObj.MobileNumber;
            try
            {
                var hash = SharedLibrary.Security.GetSha256Hash("SubscriberStatus" + messageObj.ServiceCode + messageObj.MobileNumber);
                if (messageObj.AccessKey != hash)
                    result.Status = "You do not have permission";
                else
                {
                    if (messageObj.ServiceCode == "NabardGah")
                        messageObj.ServiceCode = "Soltan";
                    messageObj.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(messageObj.MobileNumber);
                    if (messageObj.MobileNumber == "Invalid Mobile Number")
                        result.Status = "Invalid Mobile Number";
                    else if (!VerificactionAllowedServiceCode.Contains(messageObj.ServiceCode))
                        result.Status = "This ServiceCode does not have permission";
                    else
                    {
                        if (messageObj.ServiceCode == "Tamly")
                            messageObj.ServiceCode = "Tamly500";
                        else if (messageObj.ServiceCode == "AvvalYad")
                            messageObj.ServiceCode = "BehAmooz500";
                        else if (messageObj.ServiceCode == "ShenoYad")
                            messageObj.ServiceCode = "ShenoYad500";
                        var service = SharedLibrary.ServiceHandler.GetServiceFromServiceCode(messageObj.ServiceCode);
                        if (service == null)
                        {
                            result.Status = "Invalid ServiceCode";
                        }
                        else
                        {
                            messageObj.ServiceId = service.Id;
                            messageObj.ShortCode = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(service.Id).ShortCode;
                            var subscriber = SharedLibrary.HandleSubscription.GetSubscriber(messageObj.MobileNumber, messageObj.ServiceId);
                            var daysLeft = 0;
                            var pricePayed = -1;
                            if (messageObj.ServiceCode == "Soltan")
                            {
                                using (var entity = new SoltanLibrary.Models.SoltanEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                        pricePayed = -1;
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "DonyayeAsatir")
                            {
                                using (var entity = new DonyayeAsatirLibrary.Models.DonyayeAsatirEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var singlecharge = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                        if (singlecharge != null)
                                        {
                                            pricePayed = 0;
                                        }
                                        else
                                            pricePayed = -1;
                                    }
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "MenchBaz")
                            {
                                using (var entity = new MenchBazLibrary.Models.MenchBazEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                        pricePayed = -1;
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "Soraty")
                            {
                                using (var entity = new SoratyLibrary.Models.SoratyEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                        pricePayed = -1;
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "DefendIran")
                            {
                                using (var entity = new DefendIranLibrary.Models.DefendIranEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                        pricePayed = -1;
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "ShahreKalameh")
                            {
                                using (var entity = new ShahreKalamehLibrary.Models.ShahreKalamehEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Aseman")
                            {
                                using (var entity = new AsemanLibrary.Models.AsemanEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Tamly")
                            {
                                using (var entity = new TamlyLibrary.Models.TamlyEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Tamly500")
                            {
                                using (var entity = new Tamly500Library.Models.Tamly500Entities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Dezhban")
                            {
                                using (var entity = new DezhbanLibrary.Models.DezhbanEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now && o.IsUserCanceledTheInstallment != true).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "TahChin")
                            {
                                using (var entity = new TahChinLibrary.Models.TahChinEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "MusicYad")
                            {
                                using (var entity = new MusicYadLibrary.Models.MusicYadEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Dambel")
                            {
                                using (var entity = new DambelLibrary.Models.DambelEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Medad")
                            {
                                using (var entity = new MedadLibrary.Models.MedadEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "PorShetab")
                            {
                                using (var entity = new PorShetabLibrary.Models.PorShetabEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "JabehAbzar")
                            {
                                using (var entity = new JabehAbzarLibrary.Models.JabehAbzarEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "ShenoYad")
                            {
                                using (var entity = new ShenoYadLibrary.Models.ShenoYadEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "ShenoYad500")
                            {
                                using (var entity = new ShenoYad500Library.Models.ShenoYad500Entities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "FitShow")
                            {
                                using (var entity = new FitShowLibrary.Models.FitShowEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Takavar")
                            {
                                using (var entity = new TakavarLibrary.Models.TakavarEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "AvvalPod")
                            {
                                using (var entity = new AvvalPodLibrary.Models.AvvalPodEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "AvvalPod500")
                            {
                                using (var entity = new AvvalPod500Library.Models.AvvalPod500Entities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "AvvalYad")
                            {
                                using (var entity = new AvvalYadLibrary.Models.AvvalYadEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                        pricePayed = -1;
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "BehAmooz500")
                            {
                                using (var entity = new BehAmooz500Library.Models.BehAmooz500Entities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                        pricePayed = -1;
                                    else
                                    {
                                        var originalPriceBalancedForInAppRequest = singlechargeInstallment.PriceBalancedForInAppRequest;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest == null)
                                            singlechargeInstallment.PriceBalancedForInAppRequest = 0;
                                        pricePayed = singlechargeInstallment.PricePayed - singlechargeInstallment.PriceBalancedForInAppRequest.Value;
                                        singlechargeInstallment.PriceBalancedForInAppRequest += pricePayed;
                                        if (singlechargeInstallment.PriceBalancedForInAppRequest != originalPriceBalancedForInAppRequest)
                                        {
                                            entity.Entry(singlechargeInstallment).State = EntityState.Modified;
                                            entity.SaveChanges();
                                        }
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "Nebula")
                            {
                                using (var entity = new NebulaLibrary.Models.NebulaEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now && o.IsUserCanceledTheInstallment != true).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Phantom")
                            {
                                using (var entity = new PhantomLibrary.Models.PhantomEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Hazaran")
                            {
                                using (var entity = new HazaranLibrary.Models.HazaranEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "Medio")
                            {
                                using (var entity = new MedioLibrary.Models.MedioEntities())
                                {
                                    var now = DateTime.Now;
                                    using (var portalEntity = new SharedLibrary.Models.PortalEntities())
                                    {
                                        var singlechargeInstallment = portalEntity.Subscribers.Where(o => o.MobileNumber == messageObj.MobileNumber && o.ServiceId == 10030).FirstOrDefault();
                                        if (singlechargeInstallment == null)
                                        {
                                            daysLeft = 0;
                                        }
                                        else if (singlechargeInstallment.DeactivationDate != null)
                                            daysLeft = 0;
                                        else
                                            daysLeft = 30;
                                    }
                                }
                            }
                            else if (messageObj.ServiceCode == "TajoTakht")
                            {
                                using (var entity = new TajoTakhtLibrary.Models.TajoTakhtEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            else if (messageObj.ServiceCode == "LahzeyeAkhar")
                            {
                                using (var entity = new LahzeyeAkharLibrary.Models.LahzeyeAkharEntities())
                                {
                                    var now = DateTime.Now;
                                    var singlechargeInstallment = entity.SinglechargeInstallments.Where(o => o.MobileNumber == messageObj.MobileNumber && DbFunctions.AddDays(o.DateCreated, 30) >= now).OrderByDescending(o => o.DateCreated).FirstOrDefault();
                                    if (singlechargeInstallment == null)
                                    {
                                        var installmentQueue = entity.SinglechargeWaitings.FirstOrDefault(o => o.MobileNumber == messageObj.MobileNumber);
                                        if (installmentQueue != null)
                                            daysLeft = 30;
                                        else
                                            daysLeft = 0;
                                    }
                                    else
                                        daysLeft = 30 - now.Subtract(singlechargeInstallment.DateCreated).Days;
                                }
                            }
                            if (daysLeft > 0 || pricePayed > -1)
                            {
                                result.Status = "Active";
                                if (daysLeft > 0)
                                    result.DaysLeft = daysLeft;
                                else
                                    result.PricePayed = pricePayed;
                            }
                            else
                            {
                                result.Status = "NotActive";
                                result.DaysLeft = null;
                                result.PricePayed = null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in SubscriberStatus:" + e);
                result.Status = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetIrancellOtpUrl(string serviceCode, string callBackParam)
        {
            dynamic result = new ExpandoObject();
            try
            {
                if (serviceCode != null && (serviceCode == "TahChin" || serviceCode == "MusicYad" || serviceCode == "Dambel" || serviceCode == "Medad" || serviceCode == "PorShetab"))
                {
                    string timestampParam = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string requestIdParam = Guid.NewGuid().ToString();
                    var price = "3000";
                    var modeParam = "1"; //Web
                    var pageNo = 0;
                    var authKey = "393830313130303036333739";
                    var sign = "";
                    var cpId = "980110006379";

                    var serviceId = SharedLibrary.ServiceHandler.GetServiceId(serviceCode).Value;
                    var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(serviceId);
                    if (serviceCode == "TahChin")
                        pageNo = 146;
                    else if (serviceCode == "MusicYad")
                        pageNo = 206;
                    else if (serviceCode == "Dambel")
                        pageNo = 0;
                    else if (serviceCode == "Medad")
                        pageNo = 0;
                    else if (serviceCode == "PorShetab")
                    {
                        pageNo = 299;
                        price = "5000";
                    }

                    sign = SharedLibrary.HelpfulFunctions.IrancellSignatureGenerator(authKey, cpId, serviceInfo.AggregatorServiceId, price, timestampParam, requestIdParam);
                    var url = string.Format(@"http://92.42.51.91/CGGateway/Default.aspx?Timestamp={0}&RequestID={1}&pageno={2}&Callback={3}&Sign={4}&mode={5}"
                                            , timestampParam, requestIdParam, pageNo, callBackParam, sign, modeParam);
                    result.Status = "Success";
                    result.uuid = requestIdParam;
                    result.Description = url;
                }
                else
                {
                    result.Status = "Invalid Service Code";
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in GetIrancellOtpUrl:" + e);
                result.Status = "Error";
                result.Description = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage GetIrancellUnsubUrl(string serviceCode, string callBackParam)
        {
            dynamic result = new ExpandoObject();
            try
            {
                if (serviceCode != null && (serviceCode == "TahChin" || serviceCode == "MusicYad" || serviceCode == "Dambel" || serviceCode == "Medad" || serviceCode == "PorShetab"))
                {
                    string timestampParam = DateTime.Now.ToString("yyyyMMddhhmmss");
                    string requestIdParam = Guid.NewGuid().ToString();
                    var price = "3000";
                    var modeParam = "1"; //Web
                    var pageNo = 0;
                    var authKey = "393830313130303036333739";
                    var sign = "";
                    var cpId = "980110006379";

                    var serviceId = SharedLibrary.ServiceHandler.GetServiceId(serviceCode).Value;
                    var serviceInfo = SharedLibrary.ServiceHandler.GetServiceInfoFromServiceId(serviceId);
                    if (serviceCode == "TahChin")
                        pageNo = 146;
                    else if (serviceCode == "MusicYad")
                        pageNo = 206;
                    else if (serviceCode == "Dambel")
                        pageNo = 0;
                    else if (serviceCode == "Medad")
                        pageNo = 0;
                    else if (serviceCode == "PorShetab")
                    {
                        pageNo = 299;
                        price = "5000";
                    }

                    sign = SharedLibrary.HelpfulFunctions.IrancellSignatureGenerator(authKey, cpId, serviceInfo.AggregatorServiceId, price, timestampParam, requestIdParam);
                    var url = string.Format(@"http://92.42.51.91/CGGateway/UnSubscribe.aspx?Timestamp={0}&RequestID={1}&CpCode={2}&Callback={3}&Sign={4}&mode={5}"
                                            , timestampParam, requestIdParam, cpId, callBackParam, sign, modeParam);
                    result.Status = "Success";
                    result.uuid = requestIdParam;
                    result.Description = url;
                }
                else
                {
                    result.Status = "Invalid Service Code";
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in GetIrancellOtpUrl:" + e);
                result.Status = "Error";
                result.Description = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DecryptIrancellMessage(string data)
        {
            dynamic result = new ExpandoObject();
            try
            {
                result.Status = "Error";
                result.Description = "General error occurred";
                result.uuid = "";
                var authKey = "393830313130303036333739";
                var message = SharedLibrary.HelpfulFunctions.IrancellEncryptedResponse(data, authKey);
                var splitedMessage = message.Split('&');
                foreach (var item in splitedMessage)
                {
                    if (item.Contains("msisdn"))
                        result.MobileNumber = SharedLibrary.MessageHandler.ValidateNumber(item.Remove(0, 7));
                    else if (item.ToLower().Contains("requestid"))
                        result.uuid = item.Remove(0, 10);
                    else if (item.Contains("status"))
                    {
                        var status = item.Remove(0, 7);
                        if (status == "00000000")
                        {
                            result.Status = "Success";
                            result.Description = "Successful Subscription";
                        }
                        else if (status == "22007201" || status == "22007238")
                        {
                            result.Status = "Error";
                            result.Description = "Already Subscribed";
                        }
                        else if (status == "10001211")
                        {
                            result.Status = "Error";
                            result.Description = "ServiceID + IP not whitelisted or used only 3G services service ID";
                        }
                        else if (status == "22007306")
                        {
                            result.Status = "Error";
                            result.Description = "MSISDN Blacklist";
                        }
                        else if (status == "22007230")
                        {
                            result.Status = "Error";
                            result.Description = "cannot be subscribed to by a third party";
                        }
                        else if (status == "22007330")
                        {
                            result.Status = "Error";
                            result.Description = "The account balance is Insufficient.";
                        }
                        else if (status == "22007306")
                        {
                            result.Status = "Error";
                            result.Description = "The user is blacklisted and cannot Subscribe to the product.";
                        }
                        else if (status == "99999999")
                        {
                            result.Status = "Error";
                            result.Description = "Subscription attempt failed. Please try again.";
                        }
                        else if (status == "88888888")
                        {
                            result.Status = "Error";
                            result.Description = "Cancel Button Clicked";
                        }
                        else
                        {
                            result.Status = "Error";
                            result.Description = "Unknown status code: " + status;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in DecryptIrancellMessage:" + e);
                result.Status = "Error";
                result.Description = "General error occurred";
            }
            var json = JsonConvert.SerializeObject(result);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(json, Encoding.UTF8, "application/json");
            return response;
        }
    }
}
