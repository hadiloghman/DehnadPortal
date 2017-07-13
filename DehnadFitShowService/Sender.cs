﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedLibrary.Models;
using FitShowLibrary.Models;
using System.Linq;
using System.Collections;

namespace DehnadFitShowService
{
    class Sender
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public void SendHandler()
        {
            try
            {
                var today = DateTime.Now.Date;
                List<AutochargeMessagesBuffer> autochargeMessages;
                List<EventbaseMessagesBuffer> eventbaseMessages;
                List<OnDemandMessagesBuffer> onDemandMessages;
                int readSize = Convert.ToInt32(Properties.Settings.Default.ReadSize);
                int takeSize = Convert.ToInt32(Properties.Settings.Default.Take);
                bool retryNotDelieveredMessages = Properties.Settings.Default.RetryNotDeliveredMessages;
                string aggregatorName = Properties.Settings.Default.AggregatorName;
                var serviceAdditionalInfo = SharedLibrary.ServiceHandler.GetAdditionalServiceInfoForSendingMessage("FitShow", aggregatorName);

                var threadsNo = SharedLibrary.MessageHandler.CalculateServiceSendMessageThreadNumbers(readSize, takeSize);
                var take = threadsNo["take"];
                var skip = threadsNo["skip"];

                using (var entity = new FitShowEntities())
                {
                    entity.Configuration.AutoDetectChangesEnabled = false;
                    autochargeMessages = ((IEnumerable)SharedLibrary.MessageHandler.GetUnprocessedMessages(entity, SharedLibrary.MessageHandler.MessageType.AutoCharge, 200)).OfType<AutochargeMessagesBuffer>().ToList();
                    eventbaseMessages = ((IEnumerable)SharedLibrary.MessageHandler.GetUnprocessedMessages(entity, SharedLibrary.MessageHandler.MessageType.EventBase, 200)).OfType<EventbaseMessagesBuffer>().ToList();
                    onDemandMessages = ((IEnumerable)SharedLibrary.MessageHandler.GetUnprocessedMessages(entity, SharedLibrary.MessageHandler.MessageType.OnDemand, 200)).OfType<OnDemandMessagesBuffer>().ToList();

                    if (retryNotDelieveredMessages && autochargeMessages.Count == 0 && eventbaseMessages.Count == 0)
                    {
                        TimeSpan retryEndTime = new TimeSpan(23, 30, 0);
                        var now = DateTime.Now.TimeOfDay;
                        if (now < retryEndTime)
                        {
                            entity.RetryUndeliveredMessages();
                        }
                    }

                    if (DateTime.Now.Hour < 23 && DateTime.Now.Hour > 7)
                    {
                        SharedLibrary.MessageHandler.SendSelectedMessages(entity, autochargeMessages, skip, take, serviceAdditionalInfo, aggregatorName);
                        SharedLibrary.MessageHandler.SendSelectedMessages(entity, eventbaseMessages, skip, take, serviceAdditionalInfo, aggregatorName);
                    }
                    SharedLibrary.MessageHandler.SendSelectedMessages(entity, onDemandMessages, skip, take, serviceAdditionalInfo, aggregatorName);
                }
            }
            catch (Exception e)
            {
                logs.Error("Error in SendHandler:" + e);
            }
        }
        public static void SendAutochargeMessages(List<AutochargeMessagesBuffer> messages, int[] skip, int[] take, Dictionary<string, string> serviceAdditionalInfo, string aggregatorName)
        {
            if (messages.Count == 0)
                return;

            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < take.Length; i++)
            {
                using (var entity = new FitShowEntities())
                {
                    var chunkedMessages = messages.Skip(skip[i]).Take(take[i]).ToList();
                    if (aggregatorName == "Hamrahvas")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToHamrahvas(entity, chunkedMessages, serviceAdditionalInfo));
                    else if (aggregatorName == "PardisImi")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToPardisImi(entity, chunkedMessages, serviceAdditionalInfo));
                    else if (aggregatorName == "Telepromo")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToTelepromo(entity, chunkedMessages, serviceAdditionalInfo));
                }
            }
            Task.WaitAll(TaskList.ToArray());
        }

        public static void SendEventbaseMessages(List<EventbaseMessagesBuffer> messages, int[] skip, int[] take, Dictionary<string, string> serviceAdditionalInfo, string aggregatorName)
        {
            if (messages.Count == 0)
                return;

            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < take.Length; i++)
            {
                using (var entity = new FitShowEntities())
                {
                    var chunkedMessages = messages.Skip(skip[i]).Take(take[i]).ToList();
                    if (aggregatorName == "Hamrahvas")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToHamrahvas(entity, chunkedMessages, serviceAdditionalInfo));
                    else if (aggregatorName == "PardisImi")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToPardisImi(entity, chunkedMessages, serviceAdditionalInfo));
                    else if (aggregatorName == "Telepromo")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToTelepromo(entity, chunkedMessages, serviceAdditionalInfo));
                }
            }
            Task.WaitAll(TaskList.ToArray());
        }

        public static void SendOnDemandMessages(List<OnDemandMessagesBuffer> messages, int[] skip, int[] take, Dictionary<string, string> serviceAdditionalInfo, string aggregatorName)
        {
            if (messages.Count == 0)
                return;

            List<Task> TaskList = new List<Task>();
            for (int i = 0; i < take.Length; i++)
            {
                using (var entity = new FitShowEntities())
                {
                    var chunkedMessages = messages.Skip(skip[i]).Take(take[i]).ToList();
                    if (aggregatorName == "Hamrahvas")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToHamrahvas(entity, chunkedMessages, serviceAdditionalInfo));
                    else if (aggregatorName == "PardisImi")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToPardisImi(entity, chunkedMessages, serviceAdditionalInfo));
                    else if (aggregatorName == "Telepromo")
                        TaskList.Add(FitShowLibrary.MessageHandler.SendMesssagesToTelepromo(entity, chunkedMessages, serviceAdditionalInfo));
                }
            }
            Task.WaitAll(TaskList.ToArray());
        }
    }
}
