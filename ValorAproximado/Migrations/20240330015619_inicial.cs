using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValorAproximado.Migrations
{
    public partial class inicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConversationMessage");

            migrationBuilder.DropTable(
                name: "ConversationRequest");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConversationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConversationMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ConversationRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConversationMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConversationMessage_ConversationRequest_ConversationRequest~",
                        column: x => x.ConversationRequestId,
                        principalTable: "ConversationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConversationMessage_ConversationRequestId",
                table: "ConversationMessage",
                column: "ConversationRequestId");
        }
    }
}
