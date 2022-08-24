﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Domain
{
    [Table("ArticleTag")]
    [Index("ArticleId", "TagId", Name = "IX_ArticleTag", IsUnique = true)]
    public partial class ArticleTag
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("articleId")]
        public int ArticleId { get; set; }
        [Column("tagId")]
        public int TagId { get; set; }

        [ForeignKey("ArticleId")]
        [InverseProperty("ArticleTags")]
        public virtual Article Article { get; set; }
        [ForeignKey("TagId")]
        [InverseProperty("ArticleTags")]
        public virtual Tag Tag { get; set; }
    }
}