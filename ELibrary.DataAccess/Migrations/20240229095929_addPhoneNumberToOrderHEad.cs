﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ELibrary.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addPhoneNumberToOrderHEad : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "OrderHeaders");
        }
    }
}
