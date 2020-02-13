using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace org.cchmc.pho.identity.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class InsertedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9230ff23-ad67-4c6f-9737-76f70e0dac0a", "6b76499a-f8a9-4590-85a9-dc65c3183fdd", "Role1", "ROLE1" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "971aff2c-2c6c-4864-8ad0-0cff5beea02a", "85065619-042b-468c-a601-5657cb7a341e", "Role2", "ROLE2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9230ff23-ad67-4c6f-9737-76f70e0dac0a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "971aff2c-2c6c-4864-8ad0-0cff5beea02a");
        }
    }
}
