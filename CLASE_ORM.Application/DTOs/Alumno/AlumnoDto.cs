namespace CLASE_ORM.Application.DTOs.Alumno
{
    public class AlumnoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<string> Materias { get; set; }
    }
}
