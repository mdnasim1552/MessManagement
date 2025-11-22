using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__3214EC07FA976C93", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Unit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Unit__3214EC07F0A552C5", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CommonBills",
                columns: table => new
                {
                    BillId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessId = table.Column<int>(type: "int", nullable: false),
                    BillType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CommonBi__11F2FC6ADCB34CF6", x => x.BillId);
                });

            migrationBuilder.CreateTable(
                name: "MarketCosts",
                columns: table => new
                {
                    CostId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessId = table.Column<int>(type: "int", nullable: false),
                    MessMemberId = table.Column<int>(type: "int", nullable: false),
                    ExpenseDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Unit = table.Column<int>(type: "int", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MarketCo__8285233EDABEC44E", x => x.CostId);
                    table.ForeignKey(
                        name: "FK__MarketCos__UnitId__6A30C649",
                        column: x => x.Unit,
                        principalTable: "Unit",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Meals",
                columns: table => new
                {
                    MealId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessId = table.Column<int>(type: "int", nullable: false),
                    MessMemberId = table.Column<int>(type: "int", nullable: false),
                    MealDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Breakfast = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Lunch = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Dinner = table.Column<decimal>(type: "decimal(5,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Meals__ACF6A63D4EEB18A2", x => x.MealId);
                });

            migrationBuilder.CreateTable(
                name: "Mess",
                columns: table => new
                {
                    MessId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FromDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Mess__9CC50CDD06C6E9E2", x => x.MessId);
                });

            migrationBuilder.CreateTable(
                name: "MessMembers",
                columns: table => new
                {
                    MessMemberId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Member"),
                    JoinedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    Rent = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__MessMemb__2F8CE6B8620BB8A1", x => x.MessMemberId);
                    table.ForeignKey(
                        name: "FK__MessMembe__MessI__5FB337D6",
                        column: x => x.MessId,
                        principalTable: "Mess",
                        principalColumn: "MessId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GoogleId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CurrentMessId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC07148AD525", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Mess",
                        column: x => x.CurrentMessId,
                        principalTable: "Mess",
                        principalColumn: "MessId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getutcdate())"),
                    RevokedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__RefreshT__3214EC07B8D56923", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserRole__AF2760AD4B7A5E2E", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK__UserRoles__RoleI__5441852A",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__UserRoles__UserI__534D60F1",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonBills_MessId",
                table: "CommonBills",
                column: "MessId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketCosts_MessId",
                table: "MarketCosts",
                column: "MessId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketCosts_MessMemberId",
                table: "MarketCosts",
                column: "MessMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_MarketCosts_Unit",
                table: "MarketCosts",
                column: "Unit");

            migrationBuilder.CreateIndex(
                name: "IX_Meals_MessMemberId",
                table: "Meals",
                column: "MessMemberId");

            migrationBuilder.CreateIndex(
                name: "UQ__Meals__8A69AE4C9E41CEAE",
                table: "Meals",
                columns: new[] { "MessId", "MessMemberId", "MealDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Mess_CreatedBy",
                table: "Mess",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "UQ__MessMemb__D6581C8FCF77A519",
                table: "MessMembers",
                columns: new[] { "MessId", "Email" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__737584F6787F58D2",
                table: "Roles",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrentMessId",
                table: "Users",
                column: "CurrentMessId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D10534B23CDD0E",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK__CommonBil__MessI__6E01572D",
                table: "CommonBills",
                column: "MessId",
                principalTable: "Mess",
                principalColumn: "MessId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MarketCosts_Mess",
                table: "MarketCosts",
                column: "MessId",
                principalTable: "Mess",
                principalColumn: "MessId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__MarketCos__MessMemberId__6B24EA82",
                table: "MarketCosts",
                column: "MessMemberId",
                principalTable: "MessMembers",
                principalColumn: "MessMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK__Meals__MessId__66603565",
                table: "Meals",
                column: "MessId",
                principalTable: "Mess",
                principalColumn: "MessId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Meals__UserId__6754599E",
                table: "Meals",
                column: "MessMemberId",
                principalTable: "MessMembers",
                principalColumn: "MessMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK__Mess__CreatedBy__59063A47",
                table: "Mess",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Mess",
                table: "Users");

            migrationBuilder.DropTable(
                name: "CommonBills");

            migrationBuilder.DropTable(
                name: "MarketCosts");

            migrationBuilder.DropTable(
                name: "Meals");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropTable(
                name: "MessMembers");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Mess");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
