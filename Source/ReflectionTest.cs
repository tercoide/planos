// Refexion es lo que en Gb llamamos instrospeccion de clases.
// Permite listar las clases de un namespace y crear instancias de ellas en tiempo de ejecucion.
// En este ejemplo, listamos las clases del namespace Gaucho y creamos instancias de las que contienen "cad" en su nombre.
// Refexion es lo que en Gb llamamos instrospeccion de clases.
// Permite listar las clases de un namespace y crear instancias de ellas en tiempo de ejecucion.
// En este ejemplo, listamos las clases del namespace Gaucho y creamos instancias de las que contienen "cad" en su nombre.
// Particularmente, el uso que le damos es crear las instancias de las
// clases de CAD, tanto las Tools como las que manejan las entidades.


using System.Reflection;
using Gaucho;

namespace Gaucho
{
public class ReflectionTest
{
    public static void List(string targetNamespace)
    {
        

        // Get the assembly containing the types you are interested in.
        // For the currently executing assembly:
        Assembly assembly = Assembly.GetExecutingAssembly(); 
        
        // Alternatively, to load a specific assembly by name:
        // Assembly assembly = Assembly.Load("YourAssemblyName");

        // Get all types from the assembly
        Type[] types = assembly.GetTypes();

        // Filter the types to find classes within the target namespace
        var classesInNamespace = types
            .Where(t => t.IsClass && t.Namespace == targetNamespace)
            .ToList();

        Console.WriteLine($"Classes in namespace '{targetNamespace}':");
        foreach (Type classType in classesInNamespace)
        {
            Console.WriteLine($"- {classType.Name}");
            if (classType.Name.Contains("cad"))
            {
 
            var myObject = (IEntity?)Activator.CreateInstance(classType);
            myObject?.Draw();
            }
        }
    }
}
}

