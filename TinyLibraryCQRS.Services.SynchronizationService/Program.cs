/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Services.SynchronizationService
{
#if CONSOLE
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Tiny Library CQRS Synchronization Service");
                Console.WriteLine("Copyright (C) 2009-2011, apworks.codeplex.com.");
                Console.WriteLine("Starting synchronization service...");
                Console.WriteLine();
                using (SynchronizationServiceProc proc = new SynchronizationServiceProc())
                {
                    proc.Started += (startedSender, startedE) =>
                        {
                            Console.WriteLine("Synchronization service started @{0}.", DateTime.Now);
                        };
                    proc.Stopped += (stoppedSender, stoppedE) =>
                        {
                            Console.WriteLine("Synchronization service stopped @{0}.", DateTime.Now);
                        };
                    proc.Processing += (procSender, procE) =>
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Id          : {0}", procE.MessageContent.MessageId);
                            Console.WriteLine("Sent Time   : {0}", procE.MessageContent.SentTime);
                            Console.WriteLine("Process Time: {0}", DateTime.Now);
                            Console.WriteLine("Processing ... ----------------------------------------------------");
                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                            Console.WriteLine(procE.MessageContent.Xml);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine();
                        };
                    proc.ProcessFailed += (procFailSender, procFailE) =>
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Id          : {0}", procFailE.MessageId);
                            Console.WriteLine("Sent Time   : {0}", procFailE.MessageContent.SentTime);
                            Console.WriteLine("Process Time: {0}", DateTime.Now);
                            Console.WriteLine("Process FAILED ----------------------------------------------------");
                            Console.ForegroundColor = ConsoleColor.DarkRed;
                            Console.WriteLine(procFailE.Exception);
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine();
                        };
                    proc.StartProc();
                    Console.ReadLine();
                    proc.StopProc();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
#endif
}
