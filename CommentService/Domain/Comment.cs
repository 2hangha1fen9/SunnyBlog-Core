﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CommentService.Domain
{
    public partial class Comment
    {
        public Comment()
        {
            InverseParent = new HashSet<Comment>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("content", TypeName = "ntext")]
        public string Content { get; set; }
        [Column("userId")]
        public int UserId { get; set; }
        [Column("articleId")]
        public int ArticleId { get; set; }
        [Column("createTime", TypeName = "datetime")]
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 1审核通过0未审核
        /// </summary>
        [Column("status")]
        public int Status { get; set; }
        [Column("parentId")]
        public int? ParentId { get; set; }
        /// <summary>
        /// 1已读0未读
        /// </summary>
        [Column("isRead")]
        public int IsRead { get; set; }

        [ForeignKey("ParentId")]
        [InverseProperty("InverseParent")]
        public virtual Comment Parent { get; set; }
        [InverseProperty("Parent")]
        public virtual ICollection<Comment> InverseParent { get; set; }
    }
}