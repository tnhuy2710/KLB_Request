﻿// <auto-generated />
using System;
using CoreApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoreApi.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.1-rtm-30846")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("CoreApi.Models.CustomProperty", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Key");

                    b.Property<string>("TargetType");

                    b.Property<string>("TargetValue");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("CustomProperties");
                });

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

                    b.Property<int?>("Confirm");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("Description");

                    b.Property<string>("FileType");

                    b.Property<string>("Name");

                    b.Property<DateTimeOffset>("PublishDate");

                    b.Property<int>("SheetIndex");

                    b.Property<string>("ViewPermissions");

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

                    b.Property<int?>("Confirm");

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

                    b.Property<string>("ViewPermissions");

                    b.HasKey("Id");

                    b.HasIndex("FormId");

                    b.ToTable("FormSteps");
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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

                    b.Property<string>("EmpCode");

                    b.Property<string>("FullName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<DateTimeOffset?>("LockoutStart");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Title");

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset?>("AvailableFrom");

                    b.Property<string>("CurrentStepId");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<DateTimeOffset?>("ExpireIn");

                    b.Property<string>("FormId");

                    b.Property<string>("InputValues");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CurrentStepId");

                    b.HasIndex("FormId");

                    b.HasIndex("UserId");

                    b.ToTable("UserForms");
                });

            modelBuilder.Entity("CoreApi.Models.UserFormAssign", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("EmpCode");

                    b.Property<string>("StepId");

                    b.Property<long>("UserFormId");

                    b.HasKey("Id");

                    b.HasIndex("StepId");

                    b.HasIndex("UserFormId");

                    b.ToTable("UserFormAssigns");
                });

            modelBuilder.Entity("CoreApi.Models.UserFormLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Action");

                    b.Property<string>("AuthorEmpCode");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("Message");

                    b.Property<string>("StepId");

                    b.Property<string>("TargetEmpCode");

                    b.Property<long>("UserFormId");

                    b.HasKey("Id");

                    b.ToTable("UserFormLogs");
                });

            modelBuilder.Entity("CoreApi.Models.UserFormStepValues", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("FormValue");

                    b.Property<string>("StepId");

                    b.Property<long>("UserFormId");

                    b.HasKey("Id");

                    b.HasIndex("StepId");

                    b.HasIndex("UserFormId");

                    b.ToTable("UserFormStepValues");
                });

            modelBuilder.Entity("CoreApi.Models.UserFormValue", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("Key");

                    b.Property<long>("UserFormId");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.HasIndex("UserFormId");

                    b.ToTable("UserFormValues");
                });

            modelBuilder.Entity("CoreApi.Models.UserFormValueStorage", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("A");

                    b.Property<string>("AA");

                    b.Property<string>("AB");

                    b.Property<string>("AC");

                    b.Property<string>("AD");

                    b.Property<string>("AE");

                    b.Property<string>("AF");

                    b.Property<string>("AG");

                    b.Property<string>("AH");

                    b.Property<string>("AI");

                    b.Property<string>("AJ");

                    b.Property<string>("AK");

                    b.Property<string>("AL");

                    b.Property<string>("AM");

                    b.Property<string>("AN");

                    b.Property<string>("AO");

                    b.Property<string>("AP");

                    b.Property<string>("AQ");

                    b.Property<string>("AR");

                    b.Property<string>("AS");

                    b.Property<string>("AT");

                    b.Property<string>("AU");

                    b.Property<string>("AV");

                    b.Property<string>("AW");

                    b.Property<string>("AX");

                    b.Property<string>("AY");

                    b.Property<string>("AZ");

                    b.Property<string>("B");

                    b.Property<string>("C");

                    b.Property<string>("D");

                    b.Property<DateTimeOffset>("DateCreated");

                    b.Property<string>("E");

                    b.Property<string>("F");

                    b.Property<string>("G");

                    b.Property<string>("H");

                    b.Property<string>("I");

                    b.Property<string>("J");

                    b.Property<string>("K");

                    b.Property<string>("L");

                    b.Property<string>("M");

                    b.Property<string>("N");

                    b.Property<string>("O");

                    b.Property<string>("P");

                    b.Property<string>("Q");

                    b.Property<string>("R");

                    b.Property<int>("RowNumber");

                    b.Property<string>("S");

                    b.Property<string>("T");

                    b.Property<string>("U");

                    b.Property<long>("UserFormId");

                    b.Property<string>("V");

                    b.Property<string>("W");

                    b.Property<string>("X");

                    b.Property<string>("Y");

                    b.Property<string>("Z");

                    b.HasKey("Id");

                    b.HasIndex("UserFormId");

                    b.ToTable("UserFormValueStorages");
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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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

            modelBuilder.Entity("CoreApi.Models.UserFormAssign", b =>
                {
                    b.HasOne("CoreApi.Models.FormStep", "FormStep")
                        .WithMany()
                        .HasForeignKey("StepId");

                    b.HasOne("CoreApi.Models.UserForm", "UserForm")
                        .WithMany()
                        .HasForeignKey("UserFormId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoreApi.Models.UserFormStepValues", b =>
                {
                    b.HasOne("CoreApi.Models.FormStep", "FormStep")
                        .WithMany()
                        .HasForeignKey("StepId");

                    b.HasOne("CoreApi.Models.UserForm", "UserForm")
                        .WithMany()
                        .HasForeignKey("UserFormId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoreApi.Models.UserFormValue", b =>
                {
                    b.HasOne("CoreApi.Models.UserForm", "UserForm")
                        .WithMany()
                        .HasForeignKey("UserFormId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("CoreApi.Models.UserFormValueStorage", b =>
                {
                    b.HasOne("CoreApi.Models.UserForm", "UserForm")
                        .WithMany()
                        .HasForeignKey("UserFormId")
                        .OnDelete(DeleteBehavior.Cascade);
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
