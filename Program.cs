// See https://aka.ms/new-console-template for more information

using Gaucho;

namespace Gaucho
{
    class Program
    {
        // Aca se inicia la ejecucion del programa.
        static void Main(string[] args)
        {
            Console.WriteLine("Pruebo instanciar las clases directamente:");

            var line = new cadLine();
            line.Draw();

            Console.WriteLine("Instancio una interfaz:");
            IEntity job = new cadLine();
            job.Draw();


            job = new cadCircle();
            job.Draw();


            // En C# no hyay Collection como en Gambas, pero hay Dictionary que es similar.
            // Tambien hay List, que es una lista de cosas, pero no tiene clave.
            // Un Dictionary es una coleccion de pares clave-valor.
            // La clave es un string y el valor algo que debe estar definido.
            // No puede ser un Variant como en Gambas.
            Console.WriteLine("Pruebo el diccionario de entidades");
            Dictionary<string, IEntity> cad = new Dictionary<string, IEntity>();
            cad.Add("LINE", new cadLine());
            cad.Add("CIRCLE", new cadCircle());

            cad["LINE"].Draw();
            cad["CIRCLE"].Draw();


            Console.WriteLine("Pruebo la reflexion");
            ReflectionTest.List("Gaucho");


            // Ahora intento llenar una lista de entidades

            Gcd.drawing.Sheet.Entities.Add(new Entity());
            Gcd.drawing.Sheet.Entities.Add(new Entity());
            Gcd.drawing.Sheet.Entities.Add(new Entity());

            Gcd.drawing.Sheet.Entities[0].Gender = "LINE";
            Gcd.drawing.Sheet.Entities[1].Gender = "CIRCLE";
            Gcd.drawing.Sheet.Entities[2].Gender = "POLYGON";

            foreach (var ent in Gcd.drawing.Sheet.Entities)
            {
                if (!cad.ContainsKey(ent.Gender))
                {
                    Console.WriteLine($"No tengo la clase {ent.Gender}");
                    continue;
                }   
                Console.WriteLine($"Entidad de tipo {ent.Gender}");
                cad[ent.Gender].Draw();
            }

        }
    }
}
 


