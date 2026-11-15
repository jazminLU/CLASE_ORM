namespace CLASE_ORM.Domain.Entities
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        //INSCRIPCIONES DEL ALUMNO A MATERIAS (muchos a muchos via Inscripcion)
        public List<Inscripcion> Inscripciones { get; set; }

    }
}
