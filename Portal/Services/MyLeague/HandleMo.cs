﻿using Portal.Models;
using Portal.Shared;
using System.Text.RegularExpressions;

namespace Portal.Services.MyLeague
{
    public class HandleMo
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void ReceivedMessage(MessageObject message, Service service)
        {
            
            var messagesTemplate = ServiceHandler.GetServiceMessagesTemplate();
            var isUserWantsToUnsubscribe = ServiceHandler.CheckIfUserWantsToUnsubscribe(message.Content);
            if (service.OnKeywords.Contains(message.Content) || isUserWantsToUnsubscribe == true)
            {
                var serviceStatusForSubscriberState = HandleSubscription.HandleSubscriptionContent(message, service, isUserWantsToUnsubscribe);
                if (serviceStatusForSubscriberState == HandleSubscription.ServiceStatusForSubscriberState.Activated || serviceStatusForSubscriberState == HandleSubscription.ServiceStatusForSubscriberState.Deactivated || serviceStatusForSubscriberState == HandleSubscription.ServiceStatusForSubscriberState.Renewal)
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
                if (serviceStatusForSubscriberState == HandleSubscription.ServiceStatusForSubscriberState.Activated)
                {
                    Subscribers.CreateSubscriberAdditionalInfo(message.MobileNumber, service.Id);
                    Subscribers.AddSubscriptionPointIfItsFirstTime(message.MobileNumber, service.Id);
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, HandleSubscription.ServiceStatusForSubscriberState.Activated);

                }
                else if (serviceStatusForSubscriberState == HandleSubscription.ServiceStatusForSubscriberState.Deactivated)
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, HandleSubscription.ServiceStatusForSubscriberState.Deactivated);
                else
                {
                    message = MessageHandler.SetImiChargeInfo(message, 0, 21, HandleSubscription.ServiceStatusForSubscriberState.Renewal);
                }

                message.Content = Shared.MessageHandler.PrepareSubscriptionMessage(messagesTemplate, serviceStatusForSubscriberState);
                MessageHandler.InsertMessageToQueue(message);
                return;
            }
            var subscriber = Shared.HandleSubscription.GetSubscriber(message.MobileNumber, message.ServiceId);

            if (subscriber == null)
            {
                message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                MessageHandler.InsertMessageToQueue(message);
                return;
            }
            message.SubscriberId = subscriber.Id;
            if(subscriber.DeactivationDate != null)
            {
                if (Regex.IsMatch(message.Content, @"^[a-zA-Z]+$"))
                {
                    Subscribers.AddSubscriptionOffReasonPoint(subscriber.Id, service.Id);
                    MessageHandler.SetOffReason(subscriber, message, messagesTemplate);
                }
                else
                {
                    message = MessageHandler.InvalidContentWhenNotSubscribed(message, messagesTemplate);
                    MessageHandler.InsertMessageToQueue(message);
                }
                return;
            }
            ContentManager.HandleContent(message, service, subscriber, messagesTemplate);
        }
    }
}