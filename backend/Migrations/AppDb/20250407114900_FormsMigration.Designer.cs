﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using backend.Data;

#nullable disable

namespace backend.Migrations.AppDb
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250407114900_FormsMigration")]
    partial class FormsMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AnswerId"));

                    b.Property<int>("CorrespondingFormFormId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("StudentAnswersId")
                        .HasColumnType("int");

                    b.HasKey("AnswerId");

                    b.HasIndex("CorrespondingFormFormId");

                    b.HasIndex("StudentAnswersId");

                    b.ToTable("Answer");
                });

            modelBuilder.Entity("ChoosablePreference", b =>
                {
                    b.Property<int>("ChoosablePreferenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ChoosablePreferenceId"));

                    b.Property<int?>("AnswerId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ChoosablePreferenceId");

                    b.HasIndex("AnswerId");

                    b.ToTable("ChoosablePreference");
                });

            modelBuilder.Entity("Form", b =>
                {
                    b.Property<int>("FormId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("FormId"));

                    b.HasKey("FormId");

                    b.ToTable("Form");
                });

            modelBuilder.Entity("ObligatoryPreference", b =>
                {
                    b.Property<int>("ObligatoryPreferenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ObligatoryPreferenceId"));

                    b.Property<int>("FormForWhichCorrespondFormId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ObligatoryPreferenceId");

                    b.HasIndex("FormForWhichCorrespondFormId");

                    b.ToTable("ObligatoryPreference");
                });

            modelBuilder.Entity("OptionForObligatoryPreference", b =>
                {
                    b.Property<int>("OptionForObligatoryPreferenceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("OptionForObligatoryPreferenceId"));

                    b.Property<string>("OptionForPreference")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("PreferenceObligatoryPreferenceId")
                        .HasColumnType("int");

                    b.HasKey("OptionForObligatoryPreferenceId");

                    b.HasIndex("PreferenceObligatoryPreferenceId");

                    b.ToTable("OptionForObligatoryPreference");
                });

            modelBuilder.Entity("OptionForObligatoryPreference_Answer", b =>
                {
                    b.Property<int>("AnswerId")
                        .HasColumnType("int");

                    b.Property<int>("OptionForObligatoryPreferenceId")
                        .HasColumnType("int");

                    b.HasKey("AnswerId", "OptionForObligatoryPreferenceId");

                    b.HasIndex("OptionForObligatoryPreferenceId");

                    b.ToTable("OptionForObligatoryPreference_Answer");
                });

            modelBuilder.Entity("StudentAnswers", b =>
                {
                    b.Property<int>("StudentAnswersId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StudentAnswersId"));

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("StudentAnswersId");

                    b.ToTable("StudentAnswers");
                });

            modelBuilder.Entity("Answer", b =>
                {
                    b.HasOne("Form", "CorrespondingForm")
                        .WithMany()
                        .HasForeignKey("CorrespondingFormFormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StudentAnswers", "StudentAnswers")
                        .WithMany("Answers")
                        .HasForeignKey("StudentAnswersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CorrespondingForm");

                    b.Navigation("StudentAnswers");
                });

            modelBuilder.Entity("ChoosablePreference", b =>
                {
                    b.HasOne("Answer", null)
                        .WithMany("Choosable")
                        .HasForeignKey("AnswerId");
                });

            modelBuilder.Entity("ObligatoryPreference", b =>
                {
                    b.HasOne("Form", "FormForWhichCorrespond")
                        .WithMany("Obligatory")
                        .HasForeignKey("FormForWhichCorrespondFormId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FormForWhichCorrespond");
                });

            modelBuilder.Entity("OptionForObligatoryPreference", b =>
                {
                    b.HasOne("ObligatoryPreference", "Preference")
                        .WithMany("Options")
                        .HasForeignKey("PreferenceObligatoryPreferenceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Preference");
                });

            modelBuilder.Entity("OptionForObligatoryPreference_Answer", b =>
                {
                    b.HasOne("Answer", null)
                        .WithMany()
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("OptionForObligatoryPreference", null)
                        .WithMany()
                        .HasForeignKey("OptionForObligatoryPreferenceId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Answer", b =>
                {
                    b.Navigation("Choosable");
                });

            modelBuilder.Entity("Form", b =>
                {
                    b.Navigation("Obligatory");
                });

            modelBuilder.Entity("ObligatoryPreference", b =>
                {
                    b.Navigation("Options");
                });

            modelBuilder.Entity("StudentAnswers", b =>
                {
                    b.Navigation("Answers");
                });
#pragma warning restore 612, 618
        }
    }
}
