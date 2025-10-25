// Esta seria la clase base de las entidades. Sera heredada por ellas y tendra una
// interface que es una clase que representa alguna entidad y responde a 
// otras clases que necesitan cosas de las entidades, como dibujarse con Draw.
namespace Gaucho
{
    public class EntityBase : IEntity
    {
        // public  void EntityBase()
        // {

        // }

        public static void Draw()
        {
            Console.WriteLine("Llamada a diibujar desde la EntityBase");
            return;
        }
 public static void Draw2()
        {
            Console.WriteLine("Imprimo desde Base");
            return;
        }

    }


    public interface IEntity
    {
        
        public void Draw() { }

        public void Draw2()

        {
            Console.WriteLine("Imprimo desde la interface IEntity");
            return;
        }
        
        public void SaveDxfData(Entity e ) { }
    }
    
    

}
