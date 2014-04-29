/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System.ComponentModel.DataAnnotations;

namespace TinyLibraryCQRS.Presentations.Mvc3.Common.QueryServices
{
    [MetadataType(typeof(BookDataObjectMetadata))]
    partial class BookDataObject { }

    public class BookDataObjectMetadata
    {
        [Required(ErrorMessage="You must specify the title for the book.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "You must specify the author for the book.")]
        public string Author { get; set; }

        [Required(ErrorMessage = "You must specify the description for the book.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "You must specify the ISBN for the book.")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "You must specify the total pages for the book.")]
        [Range(1, 2000, ErrorMessage="Total pages must be between 1 and 2000.")]
        public int Pages { get; set; }

        [Required(ErrorMessage = "You must specify the inventory for the book.")]
        [Range(1, 600, ErrorMessage = "Total pages must be between 1 and 600.")]
        public int Inventory { get; set; }
    }
}
