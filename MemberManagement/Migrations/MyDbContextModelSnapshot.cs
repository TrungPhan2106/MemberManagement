﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StudioManagement.Data;

#nullable disable

namespace StudioManagement.Migrations
{
    [DbContext(typeof(MyDbContext))]
    partial class MyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("StudioManagement.Data.Member", b =>
                {
                    b.Property<int>("MemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.Property<short>("Gender")
                        .HasColumnType("bit");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("longtext");

                    b.Property<short>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("JoinedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("MemberUUId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("varchar(36)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("varchar(13)");

                    b.Property<int>("StudioID")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.HasKey("MemberId");

                    b.HasIndex("StudioID");

                    b.ToTable("Member");
                });

            modelBuilder.Entity("StudioManagement.Data.Studio", b =>
                {
                    b.Property<int>("StudioID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("StudioAddress")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("StudioName")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.Property<string>("StudioPhone")
                        .IsRequired()
                        .HasMaxLength(45)
                        .HasColumnType("varchar(45)");

                    b.Property<string>("StudioPic")
                        .HasColumnType("longtext");

                    b.HasKey("StudioID");

                    b.ToTable("Studio");
                });

            modelBuilder.Entity("StudioManagement.Data.StudioRegister", b =>
                {
                    b.Property<int>("RegisterId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("MemberId")
                        .HasColumnType("int");

                    b.Property<int>("StudioID")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdatedDate")
                        .HasColumnType("datetime(6)");

                    b.HasKey("RegisterId");

                    b.HasIndex("MemberId");

                    b.HasIndex("StudioID");

                    b.ToTable("StudioMember");
                });

            modelBuilder.Entity("StudioManagement.Data.Member", b =>
                {
                    b.HasOne("StudioManagement.Data.Studio", "Studio")
                        .WithMany()
                        .HasForeignKey("StudioID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Studio");
                });

            modelBuilder.Entity("StudioManagement.Data.StudioRegister", b =>
                {
                    b.HasOne("StudioManagement.Data.Member", "Member")
                        .WithMany()
                        .HasForeignKey("MemberId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudioManagement.Data.Studio", "Studio")
                        .WithMany()
                        .HasForeignKey("StudioID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Member");

                    b.Navigation("Studio");
                });
#pragma warning restore 612, 618
        }
    }
}
