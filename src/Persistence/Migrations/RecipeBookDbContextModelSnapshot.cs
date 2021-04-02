﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RecipeBook.Infrastructure.Persistence;

namespace RecipeBook.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(RecipeBookDbContext))]
    partial class RecipeBookDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Ingredient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<int?>("AmountId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RecipeName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AmountId");

                    b.HasIndex("RecipeName");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Recipe", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("Rating")
                        .HasColumnType("integer");

                    b.HasKey("Name");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Step", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Instruction")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Number")
                        .HasColumnType("integer");

                    b.Property<string>("RecipeName")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RecipeName");

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

                    b.Property<string>("RecipeName")
                        .HasColumnType("text");

                    b.HasKey("When");

                    b.HasIndex("RecipeName");

                    b.ToTable("UsedOccasions");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Units.Unit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .UseIdentityByDefaultColumn();

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
                        .HasForeignKey("RecipeName");

                    b.Navigation("Amount");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.Step", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Recipes.Recipe", null)
                        .WithMany("Steps")
                        .HasForeignKey("RecipeName");
                });

            modelBuilder.Entity("RecipeBook.Core.Domain.Recipes.UsedOccasion", b =>
                {
                    b.HasOne("RecipeBook.Core.Domain.Recipes.Recipe", null)
                        .WithMany("UsedOccasions")
                        .HasForeignKey("RecipeName");
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
