﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WilliamDenton.AwsRdsPostgresDemo.Models;

namespace WilliamDenton.AwsRdsPostgresDemo.Migrations
{
    [DbContext(typeof(DemoMigratorDbContext))]
    [Migration("20190831034343_AddCustomer")]
    partial class AddCustomer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .HasAnnotation("ProductVersion", "3.0.0-preview8.19405.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("WilliamDenton.AwsRdsPostgresDemo.Models.Customer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("bigint")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<Instant>("CreatedOn")
                        .HasColumnName("created_on")
                        .HasColumnType("timestamp");

                    b.Property<string>("CustomerCode")
                        .HasColumnName("customer_code")
                        .HasColumnType("character varying(10)")
                        .HasMaxLength(10);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("character varying(100)")
                        .HasMaxLength(100);

                    b.HasKey("Id")
                        .HasName("pk_customers");

                    b.HasIndex("CustomerCode")
                        .IsUnique();

                    b.HasIndex("Name");

                    b.ToTable("customers");
                });
#pragma warning restore 612, 618
        }
    }
}