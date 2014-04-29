/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using Apworks;

namespace TinyLibraryCQRS.Model
{
    public abstract class Account : SourcedAggregateRoot
    {
        #region Ctor
        internal Account() : base() { }
        internal Account(long id) : base(id) { }
        #endregion

        #region Public Properties
        public string UserName { get; protected set; }
        public string Password { get; protected set; }
        public string DisplayName { get; protected set; }
        public string Email { get; protected set; }
        #endregion
    }
}
