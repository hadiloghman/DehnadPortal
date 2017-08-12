﻿using SharedLibrary.Models;
using ShenoYadLibrary.Models;
using System;
using System.Text.RegularExpressions;
using System.Linq;

namespace ShenoYadLibrary
{
    public class HandleMo
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void ReceivedMessage(MessageObject message, Service service)
        {
            var content = message.Content;
            var messagesTemplate = ServiceHandler.GetServiceMessagesTemplate();
            if (message.ReceivedFrom.Contains("FromApp") && !message.Content.All(char.IsDigit))
            {
                message = MessageHandler.SetImiChargeInfo(message, 0, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.InvalidContentWhenSubscribed);
                MessageHandler.InsertMessageToQueue(message);
                return;
            }
            else if (message.ReceivedFrom.Contains("AppVerification") && message.Content.Contains("sendverification"))
            {
                var verficationMessage = message.Content.Split('-');
                message.Content = messagesTemplate.Where(o => o.Title == "VerificationMessage").Select(o => o.Content).FirstOrDefault();
                message.Content = message.Content.Replace("{CODE}", verficationMessage[1]);
                message = MessageHandler.SetImiChargeInfo(message, 0, 0, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.InvalidContentWhenSubscribed);
                MessageHandler.InsertMessageToQueue(message);
                return;
            }
            var isUserSendsSubscriptionKeyword = ServiceHandler.CheckIfUserSendsSubscriptionKeyword(message.Content, service);
            var isUserWantsToUnsubscribe = ServiceHandler.CheckIfUserWantsToUnsubscribe(message.Content);

            if ((isUserWantsToUnsubscribe == true || message.IsReceivedFromIntegratedPanel == true) && !message.ReceivedFrom.Contains("IMI"))
            {
                SharedLibrary.HandleSubscription.UnsubscribeUserFromTelepromoService(service.Id, message.MobileNumber);
                return;
            }

            if (!message.ReceivedFrom.Contains("IMI") && (isUserSendsSubscriptionKeyword == true || isUserWantsToUnsubscribe == true))
                return;
            if (message.ReceivedFrom.Contains("Register"))
                isUserSendsSubscriptionKeyword = true;
            else if (message.ReceivedFrom.Contains("Unsubscribe"))
                isUserWantsToUnsubscribe = true;

            if (isUserSendsSubscriptionKeyword == true || isUserWantsToUnsubscribe == true)
            {
                if (isUserSendsSubscriptionKeyword == true && isUserWantsToUnsubscribe == false)
                {
                    var user = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);
                    if (user != null && user.DeactivationDate == null)
                    {
                        message = MessageHandler.SendServiceHelp(message, messagesTemplate);
                        MessageHandler.InsertMessageToQueue(message);
                        return;
                    }
                }
                if (service.Enable2StepSubscription == true && isUserSendsSubscriptionKeyword == true)
                {
                    bool isSubscriberdVerified = ShenoYadLibrary.ServiceHandler.IsUserVerifedTheSubscription(message.MobileNumber, message.ServiceId, content);
                    if (isSubscriberdVerified == false)
                    {
                        //message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                        //message.Content = messagesTemplate.Where(o => o.Title == "SendVerifySubscriptionMessage").Select(o => o.Content).FirstOrDefault();
                        //MessageHandler.InsertMessageToQueue(message);
                        return;
                    }
                }
                var serviceStatusForSubscriberState = SharedLibrary.HandleSubscription.HandleSubscriptionContent(message, service, isUserWantsToUnsubscribe);
                if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated || serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated || serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Renewal)
                {
                    if (message.IsReceivedFromIntegratedPanel)
                    {
                        message.SubUnSubMoMssage = "ارسال درخواست از طریق پنل تجمیعی غیر فعال سازی";
                        message.SubUnSubType = 2;
                    }
                    else
                    {
                        message.SubUnSubMoMssage = message.Content;
                        message.SubUnSubType = 1;
                    }
                }
                var subsciber = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);
                if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated)
                {
                    Subscribers.CreateSubscriberAdditionalInfo(message.MobileNumber, service.Id);
                    Subscribers.AddSubscriptionPointIfItsFirstTime(message.MobileNumber, service.Id);
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                    ContentManager.AddSubscriberToSinglechargeQueue(message.MobileNumber, content);
                }
                else if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated)
                {
                    ContentManager.DeleteFromSinglechargeQueue(message.MobileNumber);
                    ServiceHandler.CancelUserInstallments(message.MobileNumber);
                    //var subscriberId = SharedLibrary.HandleSubscription.GetSubscriberId(message.MobileNumber, message.ServiceId);
                    //message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                    return;
                }
                else if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Renewal)
                {
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                    var subscriberId = SharedLibrary.HandleSubscription.GetSubscriberId(message.MobileNumber, message.ServiceId);
                    Subscribers.SetIsSubscriberSendedOffReason(subscriberId.Value, false);
                    ContentManager.AddSubscriberToSinglechargeQueue(message.MobileNumber, content);
                }
                else
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.InvalidContentWhenNotSubscribed);

                message.Content = MessageHandler.PrepareSubscriptionMessage(messagesTemplate, serviceStatusForSubscriberState);
                MessageHandler.InsertMessageToQueue(message);
                //if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated || serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Renewal)
                //{
                //    message.Content = content;
                //    ContentManager.HandleSinglechargeContent(message, service, subsciber, messagesTemplate);
                //}
                return;
            }
            var subscriber = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);

            if (subscriber == null)
            {
                message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                MessageHandler.InsertMessageToQueue(message);
                return;
            }
            message.SubscriberId = subscriber.Id;
            if (subscriber.DeactivationDate != null)
            {
                message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                MessageHandler.InsertMessageToQueue(message);
                return;
            }
            message.Content = content;
            ContentManager.HandleContent(message, service, subscriber, messagesTemplate);

        }

        public static Singlecharge ReceivedMessageForSingleCharge(MessageObject message, Service service)
        {
            message.Content = message.Price.ToString();
            var content = message.Content;
            var singlecharge = new Singlecharge();
            if (message.Content.All(char.IsDigit))
            {
                var price = Convert.ToInt32(message.Content);
                var imiObject = MessageHandler.GetImiChargeObjectFromPrice(price, null);
                message.Content = imiObject.ChargeCode.ToString();
            }
            var messagesTemplate = ServiceHandler.GetServiceMessagesTemplate();
            var isUserSendsSubscriptionKeyword = ServiceHandler.CheckIfUserSendsSubscriptionKeyword(message.Content, service);
            var isUserWantsToUnsubscribe = ServiceHandler.CheckIfUserWantsToUnsubscribe(message.Content);
            var subscriber = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);

            if (subscriber == null)
                isUserSendsSubscriptionKeyword = true;
            if (isUserSendsSubscriptionKeyword == true || isUserWantsToUnsubscribe == true)
            {
                if (isUserSendsSubscriptionKeyword == true && isUserWantsToUnsubscribe == false)
                {
                    var user = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);
                    if (user != null && user.DeactivationDate == null)
                    {
                        message.Content = content;
                        singlecharge = ContentManager.HandleSinglechargeContent(message, service, user, messagesTemplate);
                        return singlecharge;
                    }
                }
                var serviceStatusForSubscriberState = SharedLibrary.HandleSubscription.HandleSubscriptionContent(message, service, isUserWantsToUnsubscribe);
                if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated || serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated || serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Renewal)
                {
                    if (message.IsReceivedFromIntegratedPanel)
                    {
                        message.SubUnSubMoMssage = "ارسال درخواست از طریق پنل تجمیعی غیر فعال سازی";
                        message.SubUnSubType = 2;
                    }
                    else
                    {
                        message.SubUnSubMoMssage = message.Content;
                        message.SubUnSubType = 1;
                    }
                }
                var subsciber = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);
                if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated)
                {
                    Subscribers.CreateSubscriberAdditionalInfo(message.MobileNumber, service.Id);
                    Subscribers.AddSubscriptionPointIfItsFirstTime(message.MobileNumber, service.Id);
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                }
                else if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated)
                {
                    var subscriberId = SharedLibrary.HandleSubscription.GetSubscriberId(message.MobileNumber, message.ServiceId);
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                }
                else
                {
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated);
                    var subscriberId = SharedLibrary.HandleSubscription.GetSubscriberId(message.MobileNumber, message.ServiceId);
                    //Subscribers.SetIsSubscriberSendedOffReason(subscriberId.Value, false);
                }
                //message.Content = MessageHandler.PrepareSubscriptionMessage(messagesTemplate, serviceStatusForSubscriberState);
                //MessageHandler.InsertMessageToQueue(message);
                if (serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Activated || serviceStatusForSubscriberState == SharedLibrary.HandleSubscription.ServiceStatusForSubscriberState.Renewal)
                {
                    message.Content = content;
                    singlecharge = ContentManager.HandleSinglechargeContent(message, service, subsciber, messagesTemplate);
                }
                return singlecharge;
            }
            subscriber = SharedLibrary.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);

            if (subscriber == null)
            {
                message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                MessageHandler.InsertMessageToQueue(message);
                return null;
            }
            message.SubscriberId = subscriber.Id;
            if (subscriber.DeactivationDate != null)
            {
                message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                MessageHandler.InsertMessageToQueue(message);
                return null;
            }
            message.Content = content;
            singlecharge = ContentManager.HandleSinglechargeContent(message, service, subscriber, messagesTemplate);
            return singlecharge;
        }
    }
}