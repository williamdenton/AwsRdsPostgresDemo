using Microsoft.EntityFrameworkCore.Migrations;

namespace WilliamDenton.AwsRdsPostgresDemo.Migrations
{
	public partial class snake_case : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			// migrationBuilder.DropPrimaryKey(
			//     name: "pk_customers",
			//     table: "customers");

			// migrationBuilder.AddPrimaryKey(
			//     name: "PK_customers",
			//     table: "customers",
			//     column: "id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			// migrationBuilder.DropPrimaryKey(
			//     name: "PK_customers",
			//     table: "customers");

			// migrationBuilder.AddPrimaryKey(
			//     name: "pk_customers",
			//     table: "customers",
			//     column: "id");
		}
	}
}
