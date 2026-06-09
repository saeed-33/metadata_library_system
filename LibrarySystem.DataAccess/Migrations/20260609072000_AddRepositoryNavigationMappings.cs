using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRepositoryNavigationMappings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Resources_Id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemSets_Resources_Id",
                table: "ItemSets");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Resources_Id",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Vocabularies_VocabularyId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values");

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "Media",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "MimeType",
                table: "Media",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Media_ItemId",
                table: "Media",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TemplateId",
                table: "Items",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ResourceTemplates_TemplateId",
                table: "Items",
                column: "TemplateId",
                principalTable: "ResourceTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Resources_Id",
                table: "Items",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSets_Resources_Id",
                table: "ItemSets",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Items_ItemId",
                table: "Media",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Resources_Id",
                table: "Media",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Vocabularies_VocabularyId",
                table: "Properties",
                column: "VocabularyId",
                principalTable: "Vocabularies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ResourceTemplates_TemplateId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Resources_Id",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemSets_Resources_Id",
                table: "ItemSets");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Items_ItemId",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Media_Resources_Id",
                table: "Media");

            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Vocabularies_VocabularyId",
                table: "Properties");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values");

            migrationBuilder.DropIndex(
                name: "IX_Media_ItemId",
                table: "Media");

            migrationBuilder.DropIndex(
                name: "IX_Items_TemplateId",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AltText",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "Media");

            migrationBuilder.DropColumn(
                name: "MimeType",
                table: "Media");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Resources_Id",
                table: "Items",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemSets_Resources_Id",
                table: "ItemSets",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Media_Resources_Id",
                table: "Media",
                column: "Id",
                principalTable: "Resources",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Vocabularies_VocabularyId",
                table: "Properties",
                column: "VocabularyId",
                principalTable: "Vocabularies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_Properties_PropertyId",
                table: "Values",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
