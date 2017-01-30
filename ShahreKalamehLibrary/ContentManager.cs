﻿using SharedLibrary.Models;
using SoltanLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.Data.Entity;

namespace SoltanLibrary
{
    public class ContentManager
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Singlecharge HandleSinglechargeContent(MessageObject message, Service service, Subscriber subscriber, List<MessagesTemplate> messagesTemplate)
        {
            Singlecharge singlecharge = new Singlecharge();
            message = MessageHandler.SetImiChargeInfo(message, 0, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Unspecified);
            try
            {
                var content = Convert.ToInt32(message.Content);
                bool chargecodeFound = false;
                var imiChargeCodes = ServiceHandler.GetImiChargeCodes();
                foreach (var imiChargecode in imiChargeCodes)
                {
                    if (imiChargecode.ChargeCode == content)
                    {
                        var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage("Soltan", "Telepromo");
                        message = MessageHandler.SetImiChargeInfo(message, imiChargecode.Price, 0, null);
                        chargecodeFound = true;
                        singlecharge = MessageHandler.SendSinglechargeMesssageToTelepromo(message, serviceAdditionalInfo).Result;
                        break;
                    }
                }
                if (chargecodeFound == false)
                {
                    message = MessageHandler.SendServiceHelp(message, messagesTemplate);
                    MessageHandler.InsertMessageToQueue(message);
                }
                if (singlecharge.IsSucceeded == true)
                {
                    message.Content = "خرید شما به مبلغ " + message.Price * 10 + " ریال با موفقیت انجام شد.";
                    message = MessageHandler.SetImiChargeInfo(message, 0, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Unspecified);
                    MessageHandler.InsertMessageToQueue(message);
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in ContentManager: ", e);
            }
            return singlecharge;
        }

        public static void DeleteFromSinglechargeQueue(string mobileNumber)
        {
            try
            {
                using (var entity = new SoltanEntities())
                {
                    var singlechargeQueue = entity.SinglechargeWaitings.Where(o => o.MobileNumber == mobileNumber);
                    entity.SinglechargeWaitings.RemoveRange(singlechargeQueue);
                    entity.SaveChanges();
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in DeleteFromSinglechargeQueue: ", e);
            }
        }

        public static bool AddSubscriberToSinglechargeQueue(string mobileNumber, string content)
        {
            try
            {
                using (var entity = new SoltanEntities())
                {
                    var chargeCode = Convert.ToInt32(content);
                    var imichargeCode = entity.ImiChargeCodes.FirstOrDefault(o => o.ChargeCode == chargeCode);
                    if (imichargeCode == null)
                        return false;
                    var singlechargeQueueItem = new SinglechargeWaiting();
                    singlechargeQueueItem.MobileNumber = mobileNumber;
                    singlechargeQueueItem.Price = imichargeCode.Price;
                    singlechargeQueueItem.DateAdded = DateTime.Now;
                    singlechargeQueueItem.PersianDateAdded = SharedLibrary.Date.GetPersianDateTime(DateTime.Now);
                    singlechargeQueueItem.IsLastDayWarningSent = false;
                    entity.SinglechargeWaitings.Add(singlechargeQueueItem);
                    entity.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in AddSubscriberToSinglechargeQueue: ", e);
                return false;
            }
        }

        public static void HandleContent(MessageObject message, Service service, Subscriber subscriber, List<MessagesTemplate> messagesTemplate)
        {
            try
            {
                using (var entity = new SoltanEntities())
                {
                    message = MessageHandler.SetImiChargeInfo(message, 0, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Unspecified);
                    if(message.Content == "77")
                    {
                        message.Content = messagesTemplate.Where(o => o.Title == "Content77Response").Select(o => o.Content).FirstOrDefault();
                        MessageHandler.InsertMessageToQueue(message);
                        return;
                    }
                    if (message.Content != "111" || message.Content != "1" || message.Content != "2" || message.Content != "3" || message.Content != "4" || message.Content != "5" || message.Content != "6" || message.Content != "7" || message.Content != "8" || message.Content != "9")
                    {
                        message = MessageHandler.SendServiceHelp(message, messagesTemplate);
                        MessageHandler.InsertMessageToQueue(message);
                        return;
                    }
                    var isUserAlreadyInSinglechargeQueue = IsUserAlreadyInSinglechargeQueue(message.MobileNumber);
                    if (isUserAlreadyInSinglechargeQueue == true)
                    {
                        message = MessageHandler.UserHasActiveSinglecharge(message, messagesTemplate);
                        MessageHandler.InsertMessageToQueue(message);
                        return;
                    }
                    var isUserAlreadyChargedThisMonth = IsUserAlreadyChargedThisMonth(message.MobileNumber);
                    if (isUserAlreadyChargedThisMonth == true)
                    {
                        message = MessageHandler.UserHasActiveSinglecharge(message, messagesTemplate);
                        MessageHandler.InsertMessageToQueue(message);
                        return;
                    }

                    var isSuccessful = AddSubscriberToSinglechargeQueue(message.MobileNumber, message.Content);
                    if(isSuccessful == true)
                    {
                        message.Content = messagesTemplate.Where(o => o.Title == "WelcomeMessage").Select(o => o.Content).FirstOrDefault();
                        MessageHandler.InsertMessageToQueue(message);
                    }
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in HandleContent: ", e);
            }
        }

        private static bool IsUserAlreadyChargedThisMonth(string mobileNumber)
        {
            try
            {
                using (var entity = new SoltanEntities())
                {
                    var lastMonth = DateTime.Today.AddDays(-30);
                    var isUserAlreadychargedThisMonth = entity.Singlecharges.FirstOrDefault(o => o.MobileNumber == mobileNumber && (DbFunctions.TruncateTime(o.DateCreated) <= DateTime.Now.Date && DbFunctions.TruncateTime(o.DateCreated) >= lastMonth));
                    if (isUserAlreadychargedThisMonth == null)
                        return false;
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in IsUserAlreadyChargedThisMonth: ", e);
            }
            return true;
        }

        private static bool IsUserAlreadyInSinglechargeQueue(string mobileNumber)
        {
            try
            {
                using (var entity = new SoltanEntities())
                {
                    var isUserAlreadyInSinglechargeQueue = entity.SinglechargeWaitings.Where(o => o.MobileNumber == mobileNumber);
                    if (isUserAlreadyInSinglechargeQueue == null)
                        return false;
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in IsUserAlreadyInSinglechargeQueue: ", e);
            }
            return true;
        }
    }
}