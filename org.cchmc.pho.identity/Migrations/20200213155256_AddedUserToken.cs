using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

namespace org.cchmc.pho.identity.Migrations
{
    [ExcludeFromCodeCoverage]
    public partial class AddedUserToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9230ff23-ad67-4c6f-9737-76f70e0dac0a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "971aff2c-2c6c-4864-8ad0-0cff5beea02a");

            migrationBuilder.AddColumn<string>(
                name: "Token",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "30ee3011-d92e-4bc5-bb36-3baa4e318c0a", "194d18e6-e3b9-43db-af72-ad80804170aa", "Role1", "ROLE1" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "7e470207-2759-44f7-b042-fa794a50dabe", "12224c10-d0c9-4563-b8cb-280a83430c4f", "Role2", "ROLE2" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "30ee3011-d92e-4bc5-bb36-3baa4e318c0a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7e470207-2759-44f7-b042-fa794a50dabe");

            migrationBuilder.DropColumn(
                name: "Token",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "9230ff23-ad67-4c6f-9737-76f70e0dac0a", "6b76499a-f8a9-4590-85a9-dc65c3183fdd", "Role1", "ROLE1" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "971aff2c-2c6c-4864-8ad0-0cff5beea02a", "85065619-042b-468c-a601-5657cb7a341e", "Role2", "ROLE2" });
        }
    }
}
