using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace csharpwebsite.Server.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    AvatarPath = table.Column<string>(nullable: true),
                    Grade = table.Column<int>(nullable: false),
                    Admin = table.Column<bool>(nullable: false),
                    Instructor = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Subject = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notes_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Source = table.Column<string>(nullable: true),
                    Page = table.Column<int>(nullable: true),
                    Subject = table.Column<int>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessionSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    HostId = table.Column<Guid>(nullable: false),
                    MaxAttendees = table.Column<int>(nullable: false),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Subjects = table.Column<ushort>(nullable: false),
                    Grade = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionSlots_Users_HostId",
                        column: x => x.HostId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AuthorId = table.Column<Guid>(nullable: false),
                    Content = table.Column<string>(nullable: false),
                    ReplyId = table.Column<Guid>(nullable: true),
                    TopQuestionId = table.Column<Guid>(nullable: true),
                    TopNoteId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Replies_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Replies_Replies_ReplyId",
                        column: x => x.ReplyId,
                        principalTable: "Replies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replies_Notes_TopNoteId",
                        column: x => x.TopNoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Replies_Questions_TopQuestionId",
                        column: x => x.TopQuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionAttendance",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AttendeeId = table.Column<Guid>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionAttendance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionAttendance_Users_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionAttendance_SessionSlots_SessionId",
                        column: x => x.SessionId,
                        principalTable: "SessionSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notes_AuthorId",
                table: "Notes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_AuthorId",
                table: "Questions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_AuthorId",
                table: "Replies",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_ReplyId",
                table: "Replies",
                column: "ReplyId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_TopNoteId",
                table: "Replies",
                column: "TopNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Replies_TopQuestionId",
                table: "Replies",
                column: "TopQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionAttendance_AttendeeId",
                table: "SessionAttendance",
                column: "AttendeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionAttendance_SessionId",
                table: "SessionAttendance",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SessionSlots_HostId",
                table: "SessionSlots",
                column: "HostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Replies");

            migrationBuilder.DropTable(
                name: "SessionAttendance");

            migrationBuilder.DropTable(
                name: "Notes");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "SessionSlots");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
