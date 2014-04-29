/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using TinyLibraryCQRS.Presentations.Mvc3.Common.QueryServices;

namespace TinyLibraryCQRS.Presentations.Mvc3.Common
{
    public static class Extensions
    {
        private static bool CanShow(HtmlHelper htmlHelper, AuthorizationAccountType authAccountType)
        {
            IIdentity userIdentity = htmlHelper.ViewContext.HttpContext.User.Identity;
            if (userIdentity.IsAuthenticated)
            {
                QueryServiceClient queryService = new QueryServiceClient();
                AccountType? accountType = queryService.GetAccountType(userIdentity.Name);
                queryService.Close();
                if (accountType != null)
                {
                    bool canShow = false;
                    switch (accountType.Value)
                    {
                        case AccountType.Admin:
                            if ((authAccountType & AuthorizationAccountType.Admin) == AuthorizationAccountType.Admin)
                                canShow = true;
                            else
                                canShow = false;
                            break;
                        case AccountType.User:
                            if ((authAccountType & AuthorizationAccountType.User) == AuthorizationAccountType.User)
                                canShow = true;
                            else
                                canShow = false;
                            break;
                        default:
                            break;
                    }
                    return canShow;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="authAccountType"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, AuthorizationAccountType authAccountType)
        {
            MvcHtmlString link = MvcHtmlString.Empty;
            if (CanShow(htmlHelper, authAccountType))
                link = htmlHelper.ActionLink(linkText, actionName);
            return link;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="authAccountType"></param>
        /// <returns></returns>
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, AuthorizationAccountType authAccountType)
        {
            MvcHtmlString link = MvcHtmlString.Empty;
            if (CanShow(htmlHelper, authAccountType))
                link = htmlHelper.ActionLink(linkText, actionName, controllerName);
            return link;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, AuthorizationAccountType authAccountType)
        {
            MvcHtmlString link = MvcHtmlString.Empty;
            if (CanShow(htmlHelper, authAccountType))
                link = htmlHelper.ActionLink(linkText, actionName, routeValues);
            return link;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes, AuthorizationAccountType authAccountType)
        {
            MvcHtmlString link = MvcHtmlString.Empty;
            if (CanShow(htmlHelper, authAccountType))
                link = htmlHelper.ActionLink(linkText, actionName, routeValues, htmlAttributes);
            return link;
        }
    }
}
