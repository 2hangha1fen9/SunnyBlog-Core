﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using ArticleService.Domain;

namespace ArticleService
{
    public partial class ArticleDBContext : DbContext
    {
        public ArticleDBContext(DbContextOptions<ArticleDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ArtCategory> ArtCategories { get; set; }
        public virtual DbSet<ArtRegion> ArtRegions { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticleTag> ArticleTags { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtCategory>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArtCategories)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_ArtCategory_Article");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.ArtCategories)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArtCategory_Category");
            });

            modelBuilder.Entity<ArtRegion>(entity =>
            {
                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1启用0禁用");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_ArtRegion_ArtRegion");
            });

            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.CommentStatus)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1可评论2需要审核评论0不可以评论");

                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsLock).HasComment("0未锁定1锁定");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1已发布2草稿3回收站0待审核");

                entity.HasOne(d => d.Region)
                    .WithMany(p => p.Articles)
                    .HasForeignKey(d => d.RegionId)
                    .HasConstraintName("FK_Article_ArtRegion");
            });

            modelBuilder.Entity<ArticleTag>(entity =>
            {
                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleTags)
                    .HasForeignKey(d => d.ArticleId)
                    .HasConstraintName("FK_ArticleTag_Article");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ArticleTags)
                    .HasForeignKey(d => d.TagId)
                    .HasConstraintName("FK_ArticleTag_Tags");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Category_Category");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}