namespace CLASE_ORM.Domain.Entities
{
    public class Materia
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        //AGREGAMOS LA NAVEGACIÓN INVERSA
        public List<Inscripcion> Inscripciones { get; set; }
    }
}
