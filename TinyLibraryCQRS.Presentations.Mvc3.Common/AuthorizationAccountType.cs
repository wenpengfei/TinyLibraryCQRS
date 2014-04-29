/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Presentations.Mvc3.Common
{
    [Flags]
    public enum AuthorizationAccountType
    {
        Admin = 1,
        User = 2
    }
}
