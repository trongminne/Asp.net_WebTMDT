//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebBanHang.Context
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Web;

    public partial class Comment
    {
        public int id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Comment1 { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }

        [NotMapped]
        public System.Web.HttpPostedFileBase ImageUpload1 { get; set; }
        public System.Web.HttpPostedFileBase ImageUpload2 { get; set; }

        public string video { get; set; }
        public HttpPostedFileBase VideoFile { get; set; }

        public Nullable<System.DateTime> CreatTime { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }

    public partial class CommentDTO
    {
        public int id { get; set; }
        public Nullable<int> ProductId { get; set; }
        public Nullable<int> UserId { get; set; }
        public string Comment1 { get; set; }
        public string img1 { get; set; }
        public string img2 { get; set; }
        public string video { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public Nullable<System.DateTime> CreatTime { get; set; }
        public string NameProduct { get; set; }
        public string AvatarProduct { get; set; }

    }
}
