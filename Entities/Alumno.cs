namespace CLASE_ORM.Entities
{
    public class Alumno
    {
        public int Id { get; set; }
        public string Nombre { get; set; }


        //FK PARA LA CONVENCION
        public int MateriaId { get; set; } 

        //PROPIEDAD DE NAVEGACION 
        public Materia Materia { get; set; }

    }
}
