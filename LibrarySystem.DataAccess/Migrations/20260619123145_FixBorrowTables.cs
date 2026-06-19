using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixBorrowTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_ItemCopies_CopyId",
                table: "BorrowRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ResourceTemplates_TemplateId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_SystemUsers_OwnerId",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values");

            migrationBuilder.DropIndex(
                name: "IX_Bookmarks_UserId_ItemId",
                table: "Bookmarks");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_ItemCopies_CopyId",
                table: "BorrowRecords",
                column: "CopyId",
                principalTable: "ItemCopies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ResourceTemplates_TemplateId",
                table: "Items",
                column: "TemplateId",
                principalTable: "ResourceTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_SystemUsers_OwnerId",
                table: "Resources",
                column: "OwnerId",
                principalTable: "SystemUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRecords_ItemCopies_CopyId",
                table: "BorrowRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_ResourceTemplates_TemplateId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Resources_SystemUsers_OwnerId",
                table: "Resources");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values");

            migrationBuilder.CreateIndex(
                name: "IX_Bookmarks_UserId_ItemId",
                table: "Bookmarks",
                columns: new[] { "UserId", "ItemId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRecords_ItemCopies_CopyId",
                table: "BorrowRecords",
                column: "CopyId",
                principalTable: "ItemCopies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ResourceTemplates_TemplateId",
                table: "Items",
                column: "TemplateId",
                principalTable: "ResourceTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Resources_SystemUsers_OwnerId",
                table: "Resources",
                column: "OwnerId",
                principalTable: "SystemUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
