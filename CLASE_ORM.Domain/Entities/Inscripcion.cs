namespace CLASE_ORM.Domain.Entities
{
    public class Inscripcion
    {
        public int Id { get; set; }

        public int AlumnoId { get; set; }
        public Alumno Alumno { get; set; }

        public int MateriaId { get; set; }
        public Materia Materia { get; set; }

        // Pendiente | Aprobada | Rechazada
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    }
}
