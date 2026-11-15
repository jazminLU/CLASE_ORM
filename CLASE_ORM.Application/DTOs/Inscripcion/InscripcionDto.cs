namespace CLASE_ORM.Application.DTOs.Inscripcion
{
    public class InscripcionDto
    {
        public int Id { get; set; }
        public int MateriaId { get; set; }
        public string MateriaNombre { get; set; }
        public int AlumnoId { get; set; }
        public string AlumnoNombre { get; set; }
        public string Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
    }
}
