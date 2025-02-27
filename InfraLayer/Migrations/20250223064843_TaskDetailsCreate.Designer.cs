﻿// <auto-generated />
using InfraLayer.DB_Layer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InfraLayer.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250223064843_TaskDetailsCreate")]
    partial class TaskDetailsCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.36");

            modelBuilder.Entity("DomainLayer.Entities.TaskDetails", b =>
                {
                    b.Property<int>("TaskId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("TaskAssignedToId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TaskDescription")
                        .HasColumnType("TEXT");

                    b.Property<string>("TaskTitle")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("TaskId");

                    b.HasIndex("TaskAssignedToId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("DomainLayer.Entities.UserDetails", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DomainLayer.Entities.TaskDetails", b =>
                {
                    b.HasOne("DomainLayer.Entities.UserDetails", "TaskAssignedTo")
                        .WithMany()
                        .HasForeignKey("TaskAssignedToId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TaskAssignedTo");
                });
#pragma warning restore 612, 618
        }
    }
}
