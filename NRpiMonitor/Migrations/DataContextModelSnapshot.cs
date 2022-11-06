﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NRpiMonitor.Database;

#nullable disable

namespace NRpiMonitor.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.10");

            modelBuilder.Entity("NRpiMonitor.Database.Model.PingResultDal", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("AvgRtt")
                        .HasColumnType("REAL");

                    b.Property<string>("Host")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("MaxRtt")
                        .HasColumnType("REAL");

                    b.Property<double>("MinRtt")
                        .HasColumnType("REAL");

                    b.Property<int>("ReceivedPackets")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SentPackets")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Host");

                    b.HasIndex("Timestamp");

                    b.ToTable("PingResults", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
