﻿// <auto-generated />
using CoreApi.Data;
using CoreApi.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace CoreApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20180419084217_RefactorAllTables")]
    partial class RefactorAllTables
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoreApi.Models.Device", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<DateTimeOffset>("DateUpdated");

                    b.Property<string>("DeviceToken");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name")
                        .HasMaxLength(255);

                    b.Property<string>("OsName")
                        .HasMaxLength(255);

                    b.Property<string>("OsVersion")
                        .HasMaxLength(100);

                    b.Property<string>("Uuid")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("Uuid")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("CoreApi.Models.Form", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("CloseDate");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("Description");

                    b.Property<string>("FileType");

                    b.Property<string>("Name");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<int>("SheetIndex");

                    b.HasKey("Id");

                    b.ToTable("Forms");
                });

            modelBuilder.Entity("CoreApi.Models.FormStep", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("AvailableFrom");

                    b.Property<string>("Claims")
                        .IsRequired();

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<DateTimeOffset>("DateUpdated");

                    b.Property<string>("Description");

                    b.Property<DateTimeOffset?>("ExpireIn");

                    b.Property<string>("FormId")
                        .IsRequired();

                    b.Property<string>("GroupIds")
                        .IsRequired();

                    b.Property<int>("Index");

                    b.Property<bool>("IsAllowSendEmail");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("NextStepId");

                    b.Property<string>("PrevStepId");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("FormPermissions");
                });

            modelBuilder.Entity("CoreApi.Models.Group", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(255);

                    b.Property<int>("GroupType");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTimeOffset?>("LockoutStart");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("CoreApi.Models.Option", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("Key")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("Options");
                });

            modelBuilder.Entity("CoreApi.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<int>("Level");

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("CoreApi.Models.Token", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DeviceTarget");

                    b.Property<DateTimeOffset>("ExpireIn");

                    b.Property<int>("Type");

                    b.Property<string>("UserTarget");

                    b.Property<string>("Value")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Value")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("Tokens");
                });

            modelBuilder.Entity("CoreApi.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<DateTimeOffset>("DateUpdated");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTimeOffset?>("LockoutStart");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("OrgNbr");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CoreApi.Models.UserDevice", b =>
                {
                    b.Property<string>("DeviceId");

                    b.Property<string>("UserId");

                    b.Property<DateTimeOffset>("ExpireIn");

                    b.Property<DateTimeOffset>("LastAccess");

                    b.Property<string>("Token");

                    b.HasKey("DeviceId", "UserId");

                    b.HasIndex("DeviceId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Token")
                        .IsUnique()
                        .HasFilter("[Token] IS NOT NULL")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("UserId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("UserDevices");
                });

            modelBuilder.Entity("CoreApi.Models.UserForm", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset?>("AvailableFrom");

                    b.Property<string>("CurrentStepId");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("FormId");

                    b.Property<string>("InputValues");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CurrentStepId");

                    b.HasIndex("FormId");

                    b.HasIndex("UserId");

                    b.ToTable("UserForms");
                });

            modelBuilder.Entity("CoreApi.Models.UserFormLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Action");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("Message");

                    b.Property<string>("StepId");

                    b.Property<long>("UserFormId");

                    b.HasKey("Id");

                    b.ToTable("UserFormLogs");
                });

            modelBuilder.Entity("CoreApi.Models.UserGroup", b =>
                {
                    b.Property<string>("GroupId");

                    b.Property<string>("UserId");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("GroupId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("UserId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("UserGroups");
                });

            modelBuilder.Entity("CoreApi.Models.Whitelist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20);

                    b.Property<string>("Username")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("Whitelists");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("CoreApi.Models.FormStep", b =>
                {
                    b.HasOne("CoreApi.Models.Form", "Form")
                        .WithMany()
                        .HasForeignKey("FormId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoreApi.Models.UserDevice", b =>
                {
                    b.HasOne("CoreApi.Models.Device", "Device")
                        .WithMany("UserDevices")
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoreApi.Models.User", "User")
                        .WithMany("UserDevices")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoreApi.Models.UserForm", b =>
                {
                    b.HasOne("CoreApi.Models.FormStep", "CurrentStep")
                        .WithMany()
                        .HasForeignKey("CurrentStepId");

                    b.HasOne("CoreApi.Models.Form", "Form")
                        .WithMany()
                        .HasForeignKey("FormId");

                    b.HasOne("CoreApi.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("CoreApi.Models.UserGroup", b =>
                {
                    b.HasOne("CoreApi.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoreApi.Models.User", "User")
                        .WithMany("UserGroups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("CoreApi.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("CoreApi.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("CoreApi.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("CoreApi.Models.Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("CoreApi.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("CoreApi.Models.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
