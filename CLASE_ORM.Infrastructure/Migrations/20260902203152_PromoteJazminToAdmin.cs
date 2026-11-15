using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLASE_ORM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PromoteJazminToAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Usuarios SET Rol = 'Admin' WHERE Email = 'jazmin.test@example.com';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Usuarios SET Rol = 'Usuario' WHERE Email = 'jazmin.test@example.com';");
        }
    }
}
