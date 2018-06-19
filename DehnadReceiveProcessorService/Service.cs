﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace DehnadReceiveProcessorService
{
    public partial class Service : ServiceBase
    {
        static log4net.ILog logs = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Thread processThread;
        private Thread telepromoProcessThread;
        private Thread telepromoOtpConfirmProcessThread; 
        private Thread hubProcessThread;
        private Thread irancellProcessThread;
        private Thread mobinoneProcessThread;
        private Thread mobinOneMapfaProcessThread;
        private Thread samssonTciProcessThread;
        private Thread pardisPlatformProcessThread;
        private Thread mciDirectProcessThread;
        private Thread getIrancellsMoThread;
        private Thread getFtpThread;
        private ManualResetEvent shutdownEvent = new ManualResetEvent(false);
        public static List<SharedLibrary.Models.OperatorsPrefix> prefix;
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            //processThread = new Thread(MessageProcessorWorkerThread);
            //processThread.IsBackground = true;
            //processThread.Start();

            pardisPlatformProcessThread = new Thread(PardisPLatformMessageProcessorWorkerThread);
            pardisPlatformProcessThread.IsBackground = true;
            pardisPlatformProcessThread.Start();

            telepromoProcessThread = new Thread(TelepromoMessageProcessorWorkerThread);
            telepromoProcessThread.IsBackground = true;
            telepromoProcessThread.Start();

            telepromoOtpConfirmProcessThread = new Thread(TelepromoOtpConfirmProcessorWorkerThread);
            telepromoOtpConfirmProcessThread.IsBackground = true;
            telepromoOtpConfirmProcessThread.Start();

            hubProcessThread = new Thread(HubMessageProcessorWorkerThread);
            hubProcessThread.IsBackground = true;
            hubProcessThread.Start();

            irancellProcessThread = new Thread(IrancellMessageProcessorWorkerThread);
            irancellProcessThread.IsBackground = true;
            irancellProcessThread.Start();

            mobinoneProcessThread = new Thread(MobinOneMessageProcessorWorkerThread);
            mobinoneProcessThread.IsBackground = true;
            mobinoneProcessThread.Start();

            mobinOneMapfaProcessThread = new Thread(MobinOneMapfaMessageProcessorWorkerThread);
            mobinOneMapfaProcessThread.IsBackground = true;
            mobinOneMapfaProcessThread.Start();

            samssonTciProcessThread = new Thread(SamssonTciMessageProcessorWorkerThread);
            samssonTciProcessThread.IsBackground = true;
            samssonTciProcessThread.Start();

            mciDirectProcessThread = new Thread(MciDirectMessageProcessorWorkerThread);
            mciDirectProcessThread.IsBackground = true;
            mciDirectProcessThread.Start();

            getIrancellsMoThread = new Thread(IrancellMoWorkerThread);
            getIrancellsMoThread.IsBackground = true;
            getIrancellsMoThread.Start();

            getFtpThread = new Thread(FtpWorkerThread);
            getFtpThread.IsBackground = true;
            getFtpThread.Start();
        }

        protected override void OnStop()
        {
            try
            {
                shutdownEvent.Set();
                //if (!processThread.Join(3000))
                //{
                //    processThread.Abort();
                //}
                if (!telepromoProcessThread.Join(3000))
                {
                    telepromoProcessThread.Abort();
                }
                if (!hubProcessThread.Join(3000))
                {
                    hubProcessThread.Abort();
                }
                if (!irancellProcessThread.Join(3000))
                {
                    irancellProcessThread.Abort();
                }
                if (!mobinoneProcessThread.Join(3000))
                {
                    mobinoneProcessThread.Abort();
                }
                if (!mobinOneMapfaProcessThread.Join(3000))
                {
                    mobinOneMapfaProcessThread.Abort();
                }
                if (!samssonTciProcessThread.Join(3000))
                {
                    samssonTciProcessThread.Abort();
                }
                if (!pardisPlatformProcessThread.Join(3000))
                {
                    pardisPlatformProcessThread.Abort();
                }
                if (!mciDirectProcessThread.Join(3000))
                {
                    mciDirectProcessThread.Abort();
                }
                if (!getIrancellsMoThread.Join(3000))
                {
                    getIrancellsMoThread.Abort();
                }
                if (!getFtpThread.Join(3000))
                {
                    getFtpThread.Abort();
                }
                if (!telepromoOtpConfirmProcessThread.Join(3000))
                {
                    telepromoOtpConfirmProcessThread.Abort();
                }
            }
            catch (Exception exp)
            {
                logs.Info(" Exception in thread termination ");
                logs.Error(" Exception in thread termination " + exp);
            }

        }

        private void MessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.Process();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void TelepromoMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.TelepromoProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void TelepromoOtpConfirmProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.TelepromoOtpConfirmProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void HubMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.HubProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void MciDirectMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.MciDirectProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void IrancellMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.IrancellProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void MobinOneMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.MobinOneProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void MobinOneMapfaMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.MobinOneMapfaProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void SamssonTciMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.SamssonTciProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void PardisPLatformMessageProcessorWorkerThread()
        {
            try
            {
                using (var entity = new SharedLibrary.Models.PortalEntities())
                {
                    prefix = entity.OperatorsPrefixs.ToList();
                }
                var messageProcessor = new MessageProcesser();
                while (!shutdownEvent.WaitOne(0))
                {
                    messageProcessor.PardisPlatformProcess();
                    messageProcessor.PardisImiProcess();
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e)
            {
                logs.Error("Exception in MessageProcessorWorkerThread:", e);
            }
        }

        private void IrancellMoWorkerThread()
        {
            var irancell = new Irancell();
            while (!shutdownEvent.WaitOne(0))
            {
                irancell.GetMo();
                Thread.Sleep(5000);
            }
        }

        private void FtpWorkerThread()
        {
            var ftp = new Ftp();
            while (!shutdownEvent.WaitOne(0))
            {
                //if (DateTime.Now < DateTime.Parse("2018-06-20 00:00:00"))
                //{
                    if (DateTime.Now.TimeOfDay >= TimeSpan.Parse("10:30:00") && DateTime.Now.TimeOfDay < TimeSpan.Parse("10:35:00"))
                    {
                        ftp.TelepromoIncomeReport();
                        Thread.Sleep(23 * 60 * 60 * 1000);
                    }
                //}
                //else
                //    break;
                Thread.Sleep(2 * 60 * 1000);
            }
        }
    }
}
