using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tor.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubscriptionToBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Subscription_AllowedStaffMembers",
                table: "Businesses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Subscription_FreeTrialDuration",
                table: "Businesses",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "Subscription_IsFreeTrial",
                table: "Businesses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subscription_AllowedStaffMembers",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Subscription_FreeTrialDuration",
                table: "Businesses");

            migrationBuilder.DropColumn(
                name: "Subscription_IsFreeTrial",
                table: "Businesses");
        }
    }
}
