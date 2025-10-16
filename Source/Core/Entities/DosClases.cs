// A diferencia de Gambas, en C# puede haber mas de una clase por archivo.
// Y el nombre del archivo no es el nombre de la clase necesariamente.

// Esta es la definicion de la clase. 
// Equivale a un archivo cadLine.class y a otro cadCircle.class en Gb.

using Gaucho;

namespace Gaucho
{
    // Arma la clase cadLine que implementa la interfaz IEntity.
    // Una interfaz es un contrato que obliga a la clase a implementar ciertos metodos.
    // Entonces puedo crear una instancia de la clase y asignarla a una variable del tipo de la interfaz.
    // En Gb usabamos clsJob , que era una variable global.

    public class cadLine: IEntity
    {
        public static string Gender = "LINE";
        // Constructor, equivalente a _New en Gambas.
        public cadLine()
        {
            Console.WriteLine("Creo una linea");
        }

        public void Draw()
        {
            Console.WriteLine("Dibujo una linea");
            PrivateMethod();
            return;
        }

        private void PrivateMethod()
        {
            Console.WriteLine("Metodo privado de cadLine");
            return;
        }

    }


    public class cadCircle: IEntity
    {
        public static string Gender = "CIRCLE";
        public cadCircle()
        {
            Console.WriteLine("Creo un circulo");
        }

        public void  Draw()
        {
            Console.WriteLine("Dibujo un circulo");
            return;
        }
    }

}