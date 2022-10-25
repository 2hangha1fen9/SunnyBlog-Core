﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Domain
{
    [Table("ArtRegion")]
    [Index("Id", "ParentId", Name = "IX_ArtRegion")]
    [Index("Name", Name = "IX_ArtRegion_1", IsUnique = true)]
    public partial class ArtRegion
    {
        public ArtRegion()
        {
            Articles = new HashSet<Article>();
            InverseParent = new HashSet<ArtRegion>();
        }

        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("name")]
        [StringLength(20)]
        public string Name { get; set; }
        [Column("description", TypeName = "text")]
        public string Description { get; set; }
        [Column("parentId")]
        public int? ParentId { get; set; }
        /// <summary>
        /// 1启用-1禁用
        /// </summary>
        [Column("status")]
        public int Status { get; set; }

        [ForeignKey("ParentId")]
        [InverseProperty("InverseParent")]
        public virtual ArtRegion Parent { get; set; }
        [InverseProperty("Region")]
        public virtual ICollection<Article> Articles { get; set; }
        [InverseProperty("Parent")]
        public virtual ICollection<ArtRegion> InverseParent { get; set; }
    }
}