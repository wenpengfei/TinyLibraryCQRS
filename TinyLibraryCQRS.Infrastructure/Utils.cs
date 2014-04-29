/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System.Configuration;

namespace TinyLibraryCQRS.Infrastructure
{
    public static class Utils
    {
        public static string EventLogApplication
        {
            get
            {
                string v = ConfigurationManager.AppSettings["EventLogApplication"];
                if (string.IsNullOrEmpty(v))
                    v = "TinyLibraryCQRS";
                return v;
            }
        }
        public static string EventLog
        {
            get { return "Application"; }
        }
    }
}
