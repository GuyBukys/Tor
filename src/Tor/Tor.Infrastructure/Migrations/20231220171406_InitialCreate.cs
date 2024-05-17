using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MaximumStaffMembers = table.Column<short>(type: "smallint", nullable: false),
                    IsPaymentsIncluded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserToken = table.Column<string>(type: "text", nullable: false),
                    FirstLogin = table.Column<bool>(type: "boolean", nullable: false),
                    AppType = table.Column<short>(type: "smallint", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PhoneNumber_Prefix = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber_Number = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    InvitationId = table.Column<string>(type: "text", nullable: false),
                    IsPaymentMandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: true),
                    Address_Street = table.Column<string>(type: "text", nullable: true),
                    Address_ApartmentNumber = table.Column<short>(type: "smallint", nullable: true),
                    Address_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Longtitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Instructions = table.Column<string>(type: "text", nullable: true),
                    PaymentDetails_BankNumber = table.Column<string>(type: "text", nullable: true),
                    PaymentDetails_BranchCode = table.Column<string>(type: "text", nullable: true),
                    PaymentDetails_BeneficiaryName = table.Column<string>(type: "text", nullable: true),
                    MaximumTravelDistance = table.Column<int>(type: "integer", nullable: true),
                    Settings_BookingMinimumTimeInAdvanceInMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 180),
                    Settings_BookingMaximumTimeInAdvanceInDays = table.Column<int>(type: "integer", nullable: false, defaultValue: 14),
                    Settings_CancelAppointmentMinimumTimeInMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 180),
                    Settings_RescheduleAppointmentMinimumTimeInMinutes = table.Column<int>(type: "integer", nullable: false, defaultValue: 180),
                    HomepageNote = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Logo_Name = table.Column<string>(type: "text", nullable: true),
                    Logo_Url = table.Column<string>(type: "text", nullable: true),
                    Cover_Name = table.Column<string>(type: "text", nullable: true),
                    Cover_Url = table.Column<string>(type: "text", nullable: true),
                    WeeklySchedule = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesses_Tiers_TierId",
                        column: x => x.TierId,
                        principalTable: "Tiers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PhoneNumber_Prefix = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber_Number = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: true),
                    Address_Street = table.Column<string>(type: "text", nullable: true),
                    Address_ApartmentNumber = table.Column<short>(type: "smallint", nullable: true),
                    Address_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Longtitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Instructions = table.Column<string>(type: "text", nullable: true),
                    ProfileImage_Name = table.Column<string>(type: "text", nullable: true),
                    ProfileImage_Url = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceToken = table.Column<string>(type: "text", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Devices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Business_Portfolio",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Business_Portfolio", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Business_Portfolio_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessCategory",
                columns: table => new
                {
                    BusinessesId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategoriesId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessCategory", x => new { x.BusinessesId, x.CategoriesId });
                    table.ForeignKey(
                        name: "FK_BusinessCategory_Businesses_BusinessesId",
                        column: x => x.BusinessesId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessCategory_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_Amenities",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Amenities", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_Amenities_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_CustomWorkingDays",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DailySchedule_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: false),
                    DailySchedule_TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: true),
                    DailySchedule_TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_CustomWorkingDays", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_CustomWorkingDays_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_Locations",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Locations", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_Locations_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_Notes",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Notes", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_Notes_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_PhoneNumbers",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_PhoneNumbers", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_PhoneNumbers_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_Rules",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_Rules", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_Rules_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Businesses_SocialMedias",
                columns: table => new
                {
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<short>(type: "smallint", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_SocialMedias", x => new { x.BusinessId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_SocialMedias_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiftcardPresets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ShelfLife = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidSincePurchase = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Price_Currency = table.Column<string>(type: "text", nullable: false),
                    Value_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Value_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiftcardPresets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GiftcardPresets_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(10000)", maxLength: 10000, nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Position = table.Column<short>(type: "smallint", nullable: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PhoneNumber_Prefix = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber_Number = table.Column<string>(type: "text", nullable: false),
                    Address_City = table.Column<string>(type: "text", nullable: true),
                    Address_Street = table.Column<string>(type: "text", nullable: true),
                    Address_ApartmentNumber = table.Column<short>(type: "smallint", nullable: true),
                    Address_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Longtitude = table.Column<double>(type: "double precision", nullable: true),
                    Address_Instructions = table.Column<string>(type: "text", nullable: true),
                    ProfileImage_Name = table.Column<string>(type: "text", nullable: true),
                    ProfileImage_Url = table.Column<string>(type: "text", nullable: true),
                    WeeklySchedule = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffMembers_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffMembers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessClient",
                columns: table => new
                {
                    BusinessesId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessClient", x => new { x.BusinessesId, x.ClientsId });
                    table.ForeignKey(
                        name: "FK_BusinessClient_Businesses_BusinessesId",
                        column: x => x.BusinessesId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessClient_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FavoriteBusinesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    MuteNotifications = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavoriteBusinesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavoriteBusinesses_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FavoriteBusinesses_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IssuedGiftcards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    BusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "numeric", nullable: false),
                    PurchasedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidSincePurchase = table.Column<TimeSpan>(type: "interval", nullable: false),
                    Price_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Price_Currency = table.Column<string>(type: "text", nullable: false),
                    Value_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Value_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssuedGiftcards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IssuedGiftcards_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IssuedGiftcards_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Businesses_CustomWorkingDay_RecurringBreaks",
                columns: table => new
                {
                    DailyScheduleCustomWorkingDayBusinessId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyScheduleCustomWorkingDayId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Interval = table.Column<short>(type: "smallint", nullable: false),
                    TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses_CustomWorkingDay_RecurringBreaks", x => new { x.DailyScheduleCustomWorkingDayBusinessId, x.DailyScheduleCustomWorkingDayId, x.Id });
                    table.ForeignKey(
                        name: "FK_Businesses_CustomWorkingDay_RecurringBreaks_Businesses_Cust~",
                        columns: x => new { x.DailyScheduleCustomWorkingDayBusinessId, x.DailyScheduleCustomWorkingDayId },
                        principalTable: "Businesses_CustomWorkingDays",
                        principalColumns: new[] { "BusinessId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientNotification",
                columns: table => new
                {
                    ClientsId = table.Column<Guid>(type: "uuid", nullable: false),
                    NotificationsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientNotification", x => new { x.ClientsId, x.NotificationsId });
                    table.ForeignKey(
                        name: "FK_ClientNotification_Clients_ClientsId",
                        column: x => x.ClientsId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientNotification_Notifications_NotificationsId",
                        column: x => x.NotificationsId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ScheduledFor = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<short>(type: "smallint", nullable: false),
                    ClientDetails_Name = table.Column<string>(type: "text", nullable: false),
                    ClientDetails_Email = table.Column<string>(type: "text", nullable: false),
                    ClientDetails_PhoneNumber_Prefix = table.Column<string>(type: "text", nullable: false),
                    ClientDetails_PhoneNumber_Number = table.Column<string>(type: "text", nullable: false),
                    ClientDetails_Address_City = table.Column<string>(type: "text", nullable: true),
                    ClientDetails_Address_Street = table.Column<string>(type: "text", nullable: true),
                    ClientDetails_Address_ApartmentNumber = table.Column<short>(type: "smallint", nullable: true),
                    ClientDetails_Address_Latitude = table.Column<double>(type: "double precision", nullable: true),
                    ClientDetails_Address_Longtitude = table.Column<double>(type: "double precision", nullable: true),
                    ClientDetails_Address_Instructions = table.Column<string>(type: "text", nullable: true),
                    ServiceDetails_Name = table.Column<string>(type: "text", nullable: false),
                    ServiceDetails_Description = table.Column<string>(type: "text", nullable: false),
                    ServiceDetails_Amount_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    ServiceDetails_Amount_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Location = table.Column<short>(type: "smallint", nullable: false),
                    IsPaymentMandatory = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedDateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Amount_Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Amount_Currency = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers_CustomWorkingDays",
                columns: table => new
                {
                    StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DailySchedule_IsWorkingDay = table.Column<bool>(type: "boolean", nullable: false),
                    DailySchedule_TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: true),
                    DailySchedule_TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers_CustomWorkingDays", x => new { x.StaffMemberId, x.Id });
                    table.ForeignKey(
                        name: "FK_StaffMembers_CustomWorkingDays_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers_ReservedTimeSlots",
                columns: table => new
                {
                    StaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AtDate = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers_ReservedTimeSlots", x => new { x.StaffMemberId, x.Id });
                    table.ForeignKey(
                        name: "FK_StaffMembers_ReservedTimeSlots_StaffMembers_StaffMemberId",
                        column: x => x.StaffMemberId,
                        principalTable: "StaffMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments_Durations",
                columns: table => new
                {
                    ServiceDetailsAppointmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    ValueInMinutes = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments_Durations", x => new { x.ServiceDetailsAppointmentId, x.Id });
                    table.ForeignKey(
                        name: "FK_Appointments_Durations_Appointments_ServiceDetailsAppointme~",
                        column: x => x.ServiceDetailsAppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services_Durations",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    ValueInMinutes = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services_Durations", x => new { x.ServiceId, x.Id });
                    table.ForeignKey(
                        name: "FK_Services_Durations_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StaffMembers_CustomWorkingDay_RecurringBreaks",
                columns: table => new
                {
                    DailyScheduleCustomWorkingDayStaffMemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    DailyScheduleCustomWorkingDayId = table.Column<int>(type: "integer", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Interval = table.Column<short>(type: "smallint", nullable: false),
                    TimeRange_StartTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    TimeRange_EndTime = table.Column<TimeSpan>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffMembers_CustomWorkingDay_RecurringBreaks", x => new { x.DailyScheduleCustomWorkingDayStaffMemberId, x.DailyScheduleCustomWorkingDayId, x.Id });
                    table.ForeignKey(
                        name: "FK_StaffMembers_CustomWorkingDay_RecurringBreaks_StaffMembers_~",
                        columns: x => new { x.DailyScheduleCustomWorkingDayStaffMemberId, x.DailyScheduleCustomWorkingDayId },
                        principalTable: "StaffMembers_CustomWorkingDays",
                        principalColumns: new[] { "StaffMemberId", "Id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayName", "Name", "Type", "Url" },
                values: new object[,]
                {
                    { new Guid("208c5853-b795-4b54-95aa-0dfb615a4843"), "מספרה", "Barbershop", (short)1, "" },
                    { new Guid("48ca5438-3987-4cbf-ba8a-b736a17bfc9a"), "ציפורניים", "NailSalon", (short)2, "" },
                    { new Guid("50f9c731-29e5-490f-86a3-e4fff61b3160"), "שיער", "HairSalon", (short)3, "" },
                    { new Guid("7cd54324-407a-4876-beb1-f3d7d68d10a2"), "עיסוי ורפואה משלימה", "Massage", (short)4, "" },
                    { new Guid("960de054-89d7-4f66-849d-fc129137e0f0"), "פירסינג", "Piercing", (short)6, "" },
                    { new Guid("abe0f760-2cb1-426e-96aa-a202ed13a6df"), "גבות וריסים", "EyebrowsAndLashes", (short)5, "" },
                    { new Guid("b733e0c7-a4f0-44e7-92d3-8c5af8799b7c"), "איפור", "Makeup", (short)7, "" },
                    { new Guid("cceddb81-ba91-45fb-b1c5-871ac3bb5c93"), "אימונים אישיים", "PersonalTrainer", (short)8, "" },
                    { new Guid("d3c9378f-df01-465c-a6a1-fbfbadac4880"), "חיות מחמד", "PetServices", (short)9, "" },
                    { new Guid("f2e123ed-59a1-4893-99e9-7d11a6691186"), "אחר", "Other", (short)10, "" }
                });

            migrationBuilder.InsertData(
                table: "Tiers",
                columns: new[] { "Id", "Description", "IsPaymentsIncluded", "MaximumStaffMembers", "Type" },
                values: new object[,]
                {
                    { new Guid("7ca11ea5-626a-4f7c-9677-b3e0cfe6f669"), "free tier for new businesses", false, (short)3, (short)1 },
                    { new Guid("c6d5d1e0-6081-4382-9d48-f7243b357411"), "premium tier with more features", true, (short)5, (short)2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ClientId",
                table: "Appointments",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_StaffMemberId",
                table: "Appointments",
                column: "StaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCategory_CategoriesId",
                table: "BusinessCategory",
                column: "CategoriesId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessClient_ClientsId",
                table: "BusinessClient",
                column: "ClientsId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_InvitationId",
                table: "Businesses",
                column: "InvitationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_TierId",
                table: "Businesses",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientNotification_NotificationsId",
                table: "ClientNotification",
                column: "NotificationsId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_UserId",
                table: "Clients",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_UserId",
                table: "Devices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteBusinesses_BusinessId",
                table: "FavoriteBusinesses",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_FavoriteBusinesses_ClientId",
                table: "FavoriteBusinesses",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_GiftcardPresets_BusinessId",
                table: "GiftcardPresets",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedGiftcards_BusinessId",
                table: "IssuedGiftcards",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_IssuedGiftcards_ClientId",
                table: "IssuedGiftcards",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_BusinessId",
                table: "Notifications",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Services_StaffMemberId",
                table: "Services",
                column: "StaffMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_BusinessId",
                table: "StaffMembers",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffMembers_UserId",
                table: "StaffMembers",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserToken",
                table: "Users",
                column: "UserToken",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments_Durations");

            migrationBuilder.DropTable(
                name: "Business_Portfolio");

            migrationBuilder.DropTable(
                name: "BusinessCategory");

            migrationBuilder.DropTable(
                name: "BusinessClient");

            migrationBuilder.DropTable(
                name: "Businesses_Amenities");

            migrationBuilder.DropTable(
                name: "Businesses_CustomWorkingDay_RecurringBreaks");

            migrationBuilder.DropTable(
                name: "Businesses_Locations");

            migrationBuilder.DropTable(
                name: "Businesses_Notes");

            migrationBuilder.DropTable(
                name: "Businesses_PhoneNumbers");

            migrationBuilder.DropTable(
                name: "Businesses_Rules");

            migrationBuilder.DropTable(
                name: "Businesses_SocialMedias");

            migrationBuilder.DropTable(
                name: "ClientNotification");

            migrationBuilder.DropTable(
                name: "Devices");

            migrationBuilder.DropTable(
                name: "FavoriteBusinesses");

            migrationBuilder.DropTable(
                name: "GiftcardPresets");

            migrationBuilder.DropTable(
                name: "IssuedGiftcards");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Services_Durations");

            migrationBuilder.DropTable(
                name: "StaffMembers_CustomWorkingDay_RecurringBreaks");

            migrationBuilder.DropTable(
                name: "StaffMembers_ReservedTimeSlots");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Businesses_CustomWorkingDays");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "StaffMembers_CustomWorkingDays");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "StaffMembers");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tiers");
        }
    }
}
