﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RecipeBook.Infrastructure.Persistence;

namespace RecipeBook.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(RecipeBookDbContext))]
    [Migration("20210401152400_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AmountId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AmountId");

                    b.HasIndex("RecipeId");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Instruction")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("Steps");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.UsedOccasion", b =>
                {
                    b.Property<DateTime>("When")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<TimeSpan>("Duration")
                        .HasColumnType("interval");

                    b.Property<int?>("RecipeId")
                        .HasColumnType("integer");

                    b.HasKey("When");

                    b.HasIndex("RecipeId");

                    b.ToTable("UsedOccasions");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Units.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.ToTable("Units");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Units.Mass", b =>
                {
                    b.HasBaseType("RecipeBook.Core.Domain.Units.Unit");

                    b.Property<double>("Kilograms")
                        .HasColumnType("double precision");

                    b.ToTable("Masses");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Units.Volume", b =>
                {
                    b.HasBaseType("RecipeBook.Core.Domain.Units.Unit");

                    b.Property<double>("Liters")
                        .HasColumnType("double precision");

                    b.ToTable("Volumes");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Ingredient", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Units.Unit", "Amount")
                        .WithMany()
                        .HasForeignKey("AmountId");

                    b.HasOne("RecipeBook.Core.Domain.Recipes.Recipe", null)
                        .WithMany("Ingredients")
                        .HasForeignKey("RecipeId");

                    b.Navigation("Amount");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Step", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Recipes.Recipe", null)
                        .WithMany("Steps")
                        .HasForeignKey("RecipeId");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.UsedOccasion", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Recipes.Recipe", null)
                        .WithMany("UsedOccasions")
                        .HasForeignKey("RecipeId");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Units.Mass", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Units.Unit", null)
                        .WithOne()
                        .HasForeignKey("RecipeBook.Core.Domain.Units.Mass", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Units.Volume", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Units.Unit", null)
                        .WithOne()
                        .HasForeignKey("RecipeBook.Core.Domain.Units.Volume", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Recipe", b =>
                {
                    b.Navigation("Ingredients");

                    b.Navigation("Steps");

                    b.Navigation("UsedOccasions");
                });
#pragma warning restore 612, 618
        }
    }
}
