﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CommentService.Domain;

namespace CommentService
{
    public partial class CommentDBContext : DbContext
    {

        public CommentDBContext(DbContextOptions<CommentDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Like> Likes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.CreateTime).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IsRead).HasComment("1已读0未读");

                entity.Property(e => e.Status)
                    .HasDefaultValueSql("((1))")
                    .HasComment("1审核通过0未审核");

                entity.HasOne(d => d.Parent)
                    .WithMany(p => p.InverseParent)
                    .HasForeignKey(d => d.ParentId)
                    .HasConstraintName("FK_Comments_Comments");
            });

            modelBuilder.Entity<Like>(entity =>
            {
                entity.Property(e => e.Status).HasComment("0点赞1收藏");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}