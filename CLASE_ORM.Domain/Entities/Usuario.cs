namespace CLASE_ORM.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Rol { get; set; } = "Usuario";

        public int? AlumnoId { get; set; }
        public Alumno? Alumno { get; set; }
    }
}
