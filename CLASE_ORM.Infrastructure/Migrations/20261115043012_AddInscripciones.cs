using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CLASE_ORM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInscripciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Crear la tabla Inscripciones primero (todavia con Alumnos.MateriaId vivo)
            migrationBuilder.CreateTable(
                name: "Inscripciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlumnoId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inscripciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inscripciones_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_AlumnoId",
                table: "Inscripciones",
                column: "AlumnoId");

            migrationBuilder.CreateIndex(
                name: "IX_Inscripciones_MateriaId",
                table: "Inscripciones",
                column: "MateriaId");

            // 2. Migrar los datos existentes: la materia unica de cada alumno pasa a ser
            //    una Inscripcion ya Aprobada, antes de borrar la columna vieja.
            migrationBuilder.Sql(@"
                INSERT INTO Inscripciones (AlumnoId, MateriaId, Estado, FechaSolicitud)
                SELECT Id, MateriaId, 'Aprobada', GETUTCDATE() FROM Alumnos;
            ");

            // 3. Ahora si, borrar la relacion vieja Alumno -> Materia (1 a 1)
            migrationBuilder.DropForeignKey(
                name: "FK_Alumnos_Materias_MateriaId",
                table: "Alumnos");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_MateriaId",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "MateriaId",
                table: "Alumnos");

            // 4. Vincular Usuario -> Alumno
            migrationBuilder.AddColumn<int>(
                name: "AlumnoId",
                table: "Usuarios",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_AlumnoId",
                table: "Usuarios",
                column: "AlumnoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Alumnos_AlumnoId",
                table: "Usuarios",
                column: "AlumnoId",
                principalTable: "Alumnos",
                principalColumn: "Id");

            // 5. Dato de prueba: iaz@test.com queda vinculado al alumno "jaz" (ya tenia
            //    "Historia" aprobada via el paso 2).
            migrationBuilder.Sql(@"
                UPDATE Usuarios SET AlumnoId = (SELECT TOP 1 Id FROM Alumnos WHERE Nombre = 'jaz')
                WHERE Email = 'iaz@test.com';
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Alumnos_AlumnoId",
                table: "Usuarios");

            migrationBuilder.DropTable(
                name: "Inscripciones");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_AlumnoId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "AlumnoId",
                table: "Usuarios");

            migrationBuilder.AddColumn<int>(
                name: "MateriaId",
                table: "Alumnos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_MateriaId",
                table: "Alumnos",
                column: "MateriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Alumnos_Materias_MateriaId",
                table: "Alumnos",
                column: "MateriaId",
                principalTable: "Materias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
