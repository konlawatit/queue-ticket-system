using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QueueSystem.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QueueStates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Round = table.Column<int>(type: "int", nullable: false),
                    LastSequence = table.Column<int>(type: "int", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueStates", x => x.Id);
                    table.CheckConstraint("CK_QueueStates_LastSequence", "[LastSequence] BETWEEN -1 AND 259");
                    table.CheckConstraint("CK_QueueStates_Singleton", "[Id] = 1");
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Round = table.Column<int>(type: "int", nullable: false),
                    Sequence = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "varchar(2)", unicode: false, maxLength: 2, nullable: false),
                    IssuedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.CheckConstraint("CK_Tickets_Sequence", "[Sequence] BETWEEN 0 AND 259");
                });

            migrationBuilder.InsertData(
                table: "QueueStates",
                columns: new[] { "Id", "LastSequence", "Round", "UpdatedAt" },
                values: new object[] { 1, -1, 1, new DateTimeOffset(new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)) });

            migrationBuilder.CreateIndex(
                name: "UX_Tickets_Round_Sequence",
                table: "Tickets",
                columns: new[] { "Round", "Sequence" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QueueStates");

            migrationBuilder.DropTable(
                name: "Tickets");
        }
    }
}
