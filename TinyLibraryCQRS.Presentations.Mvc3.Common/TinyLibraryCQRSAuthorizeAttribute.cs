/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.Web.Mvc;
using QueryServiceNS = TinyLibraryCQRS.Presentations.Mvc3.Common.QueryServices;

namespace TinyLibraryCQRS.Presentations.Mvc3.Common
{
    public class TinyLibraryCQRSAuthorizeAttribute : AuthorizeAttribute
    {
        private AuthorizationAccountType accountType;

        public TinyLibraryCQRSAuthorizeAttribute()
        {
            accountType = AuthorizationAccountType.Admin | AuthorizationAccountType.User;
        }

        public TinyLibraryCQRSAuthorizeAttribute(AuthorizationAccountType accountType)
        {
            this.accountType = accountType;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            if (this.accountType == (AuthorizationAccountType.Admin | AuthorizationAccountType.User))
                return true;

            QueryServiceNS.QueryServiceClient queryService = new QueryServiceNS.QueryServiceClient();
            QueryServiceNS.AccountType? qsAccountType = queryService.GetAccountType(httpContext.User.Identity.Name);
            if (qsAccountType == null)
                return false;

            switch(qsAccountType.Value)
            {
                case QueryServiceNS.AccountType.Admin:
                    if ((this.accountType & AuthorizationAccountType.Admin) == AuthorizationAccountType.Admin)
                        return true;
                    else
                        return false;
                case QueryServiceNS.AccountType.User:
                    if ((this.accountType & AuthorizationAccountType.User) == AuthorizationAccountType.User)
                        return true;
                    else
                        return false;
                default:
                    break;
            }
            return false;
        }
    }
}
