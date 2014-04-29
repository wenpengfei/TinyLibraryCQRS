/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System.ComponentModel.DataAnnotations;

namespace TinyLibraryCQRS.Presentations.Mvc3.Common.QueryServices
{
    [MetadataType(typeof(UserAccountDataObjectMetadata))]
    partial class UserAccountDataObject { }

    public class UserAccountDataObjectMetadata
    {
        [Required(ErrorMessage="Display Name is required.")]
        public string DisplayName { get; set; }

        [Required(ErrorMessage="Email is required.")]
        [RegularExpression(@"^([0-9a-zA-Z]([-\.\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,9})$",
            ErrorMessage="Invalid Email format.")]
        public string Email { get; set; }
    }
    
}
