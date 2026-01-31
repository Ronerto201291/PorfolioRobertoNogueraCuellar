using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PruebaAngular.Infrastructure.Data.Migrations
{
    public partial class AddNotificationSubscribers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotificationSubscribers",
                columns: table => new
                {
                    SubscriberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSubscribers", x => x.SubscriberId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotificationSubscribers_Email",
                table: "NotificationSubscribers",
                column: "Email",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotificationSubscribers");
        }
    }
}
