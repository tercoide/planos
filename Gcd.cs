using System.Drawing;

static class Gcd
{
    // Equivale a Public entities As New Entity[] en Gambas
    // Todos los arrays de clases no nativas se arman como List<TipoDeClase>
    public static List<Entity> entities = new List<Entity>();

    public static Drawing drawing = new Drawing();


    public static float metros(float mm)
    {
        return mm / 1000.0F;
    }   

    public static float Pixels(double mm)
    {
        float a = (float)(mm * 96.0 / 25.4);
        return a;
    }
}