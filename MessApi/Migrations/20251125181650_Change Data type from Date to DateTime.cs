using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDatatypefromDatetoDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())")
                .Annotation("Relational:DefaultConstraintName", "DF__Users__CreatedAt__38996AB5");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getutcdate())")
                .Annotation("Relational:DefaultConstraintName", "DF__RefreshTo__Creat__3F466844");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "MessMembers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Member",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "Member")
                .Annotation("Relational:DefaultConstraintName", "DF__MessMember__Role__5DCAEF64");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                table: "MessMembers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())")
                .Annotation("Relational:DefaultConstraintName", "DF__MessMembe__Joine__5EBF139D");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Mess",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())")
                .Annotation("Relational:DefaultConstraintName", "DF__Mess__CreatedAt__5812160E");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpenseDate",
                table: "MarketCosts",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())")
                .OldAnnotation("Relational:DefaultConstraintName", "DF__Users__CreatedAt__38996AB5");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "datetime",
                nullable: false,
                defaultValueSql: "(getutcdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldDefaultValueSql: "(getutcdate())")
                .OldAnnotation("Relational:DefaultConstraintName", "DF__RefreshTo__Creat__3F466844");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "MessMembers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                defaultValue: "Member",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldDefaultValue: "Member")
                .OldAnnotation("Relational:DefaultConstraintName", "DF__MessMember__Role__5DCAEF64");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedAt",
                table: "MessMembers",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())")
                .OldAnnotation("Relational:DefaultConstraintName", "DF__MessMembe__Joine__5EBF139D");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Mess",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldDefaultValueSql: "(getdate())")
                .OldAnnotation("Relational:DefaultConstraintName", "DF__Mess__CreatedAt__5812160E");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ExpenseDate",
                table: "MarketCosts",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");
        }
    }
}
