using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibrarySystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class FixBorrowStatusEnumsAndCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. إضافة عمود الحالة الأصلية للنسخة
            migrationBuilder.AddColumn<int>(
                name: "OriginalCopyStatus",
                table: "BorrowRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // 2. تحويل قيم الحالة النصية إلى أرقام قبل تغيير نوع العمود
            migrationBuilder.Sql(@"
                UPDATE BorrowRecords
                SET Status = CASE Status
                    WHEN 'Active' THEN 0
                    WHEN 'Returned' THEN 1
                    WHEN 'Overdue' THEN 2
                    ELSE 0
                END;
            ");

            // 3. تغيير نوع عمود الحالة من nvarchar إلى int
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "BorrowRecords",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. إعادة نوع العمود إلى nvarchar
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "BorrowRecords",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // 2. تحويل الأرقام إلى نصوص
            migrationBuilder.Sql(@"
                UPDATE BorrowRecords
                SET Status = CASE Status
                    WHEN 0 THEN 'Active'
                    WHEN 1 THEN 'Returned'
                    WHEN 2 THEN 'Overdue'
                    ELSE 'Active'
                END;
            ");

            // 3. حذف عمود الحالة الأصلية
            migrationBuilder.DropColumn(
                name: "OriginalCopyStatus",
                table: "BorrowRecords");
        }
    }
}
