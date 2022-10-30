﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UserService;

#nullable disable

namespace UserService.Migrations
{
    [DbContext(typeof(UserDBContext))]
    partial class UserDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("UserService.Domain.ScoreUnit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("name");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("ScoreUnit");
                });

            modelBuilder.Entity("UserService.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Email")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("password");

                    b.Property<string>("Phone")
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11)")
                        .HasColumnName("phone");

                    b.Property<string>("Photo")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("photo");

                    b.Property<int>("Status")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("status")
                        .HasDefaultValueSql("((1))")
                        .HasComment("1启用-1禁用");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("username");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Username" }, "IX_User")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("UserService.Domain.UserDetail", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.Property<DateTime?>("Birthday")
                        .HasColumnType("date")
                        .HasColumnName("birthday");

                    b.Property<string>("Cover")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)")
                        .HasColumnName("cover");

                    b.Property<string>("Message")
                        .HasColumnType("longtext")
                        .HasColumnName("message");

                    b.Property<string>("Nick")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("nick");

                    b.Property<DateTime?>("RegisterTime")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasColumnName("registerTime")
                        .HasDefaultValueSql("(NOW())");

                    b.Property<string>("Remark")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)")
                        .HasColumnName("remark");

                    b.Property<decimal>("Score")
                        .HasColumnType("decimal(18,1)")
                        .HasColumnName("score");

                    b.Property<int?>("Sex")
                        .HasColumnType("int")
                        .HasColumnName("sex")
                        .HasComment("1男-1女0未知");

                    b.HasKey("UserId");

                    b.HasIndex(new[] { "UserId" }, "IX_UserDetail")
                        .IsUnique();

                    b.ToTable("UserDetail");
                });

            modelBuilder.Entity("UserService.Domain.UserFollow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.Property<int>("WatchId")
                        .HasColumnType("int")
                        .HasColumnName("watchId");

                    b.HasKey("Id");

                    b.HasIndex("WatchId");

                    b.HasIndex(new[] { "UserId", "WatchId" }, "IX_UserFollow")
                        .IsUnique();

                    b.ToTable("UserFollow");
                });

            modelBuilder.Entity("UserService.Domain.UserScore", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Reason")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)")
                        .HasColumnName("reason");

                    b.Property<DateTime>("Time")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(NOW())");

                    b.Property<int>("UserId")
                        .HasColumnType("int")
                        .HasColumnName("userId");

                    b.Property<decimal>("Value")
                        .HasColumnType("decimal(18,2)")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserScore");
                });

            modelBuilder.Entity("UserService.Domain.UserDetail", b =>
                {
                    b.HasOne("UserService.Domain.User", "User")
                        .WithOne("UserDetail")
                        .HasForeignKey("UserService.Domain.UserDetail", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Table_1_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserService.Domain.UserFollow", b =>
                {
                    b.HasOne("UserService.Domain.User", "User")
                        .WithMany("UserFollowUsers")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_UserFollow_User1");

                    b.HasOne("UserService.Domain.User", "Watch")
                        .WithMany("UserFollowWatches")
                        .HasForeignKey("WatchId")
                        .IsRequired()
                        .HasConstraintName("FK_UserFollow_User2");

                    b.Navigation("User");

                    b.Navigation("Watch");
                });

            modelBuilder.Entity("UserService.Domain.UserScore", b =>
                {
                    b.HasOne("UserService.Domain.User", "User")
                        .WithMany("UserScores")
                        .HasForeignKey("UserId")
                        .IsRequired()
                        .HasConstraintName("FK_Score_User");

                    b.Navigation("User");
                });

            modelBuilder.Entity("UserService.Domain.User", b =>
                {
                    b.Navigation("UserDetail");

                    b.Navigation("UserFollowUsers");

                    b.Navigation("UserFollowWatches");

                    b.Navigation("UserScores");
                });
#pragma warning restore 612, 618
        }
    }
}