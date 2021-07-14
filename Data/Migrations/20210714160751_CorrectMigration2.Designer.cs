﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleLibraryWebsite.Data;

namespace SimpleLibraryWebsite.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210714160751_CorrectMigration2")]
    partial class CorrectMigration2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Book", b =>
                {
                    b.Property<int>("BookID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddingDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Author")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Genre")
                        .HasColumnType("int");

                    b.Property<bool>("IsBorrowed")
                        .HasColumnType("bit");

                    b.Property<int?>("ReaderID")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("BookID");

                    b.HasIndex("ReaderID");

                    b.ToTable("Books");
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Loan", b =>
                {
                    b.Property<int>("LoanID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<DateTime>("LentFrom")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LentTo")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReaderID")
                        .HasColumnType("int");

                    b.HasKey("LoanID");

                    b.HasIndex("ReaderID");

                    b.ToTable("Loans");
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Reader", b =>
                {
                    b.Property<int>("ReaderID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ReaderID");

                    b.ToTable("Readers");
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Request", b =>
                {
                    b.Property<int>("RequestID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BookID")
                        .HasColumnType("int");

                    b.Property<int>("NumberOfUpvotes")
                        .HasColumnType("int");

                    b.Property<int>("ReaderID")
                        .HasColumnType("int");

                    b.HasKey("RequestID");

                    b.HasIndex("BookID");

                    b.HasIndex("ReaderID");

                    b.ToTable("Requests");
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Book", b =>
                {
                    b.HasOne("SimpleLibraryWebsite.Models.Reader", null)
                        .WithMany("Books")
                        .HasForeignKey("ReaderID");
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Loan", b =>
                {
                    b.HasOne("SimpleLibraryWebsite.Models.Reader", null)
                        .WithMany("Loans")
                        .HasForeignKey("ReaderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Request", b =>
                {
                    b.HasOne("SimpleLibraryWebsite.Models.Book", "Book")
                        .WithMany()
                        .HasForeignKey("BookID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SimpleLibraryWebsite.Models.Reader", "Reader")
                        .WithMany("Requests")
                        .HasForeignKey("ReaderID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Book");

                    b.Navigation("Reader");
                });

            modelBuilder.Entity("SimpleLibraryWebsite.Models.Reader", b =>
                {
                    b.Navigation("Books");

                    b.Navigation("Loans");

                    b.Navigation("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
