/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;

namespace TinyLibraryCQRS.Events.Accounts
{
    [Serializable]
    public class UserBorrowBookEvent : UserAccountBookTransferEvent
    {
        
    }
}
