/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Diagnostics;
using Apworks.Exceptions;

namespace TinyLibraryCQRS.Infrastructure
{
    public class GeneralExceptionHandler : ExceptionHandler<Exception>
    {
        /// <summary>
        /// Returns a <see cref="System.Boolean"/> value which indicates
        /// whether the exception was handled.
        /// </summary>
        /// <param name="ex">The exception to be handled.</param>
        /// <returns>True if the exception was handled, otherwise, false, and the
        /// exception will be re-thrown to its caller method.</returns>
        protected override bool DoHandle(Exception ex)
        {
            EventLog.WriteEntry(Utils.EventLogApplication, ex.ToString(), EventLogEntryType.Error);
            return false;
        }
    }
}
