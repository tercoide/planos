// Gambas class file
public class LineType
{
    public const string Gender = "LINETYPE";
    public int Colour;
    // public string LineType;
    public string Name="";
    public string Description ="";
    public string id = "";
    public float[] TrameLength = new float[0]; // allways start with pixels drawn, then space length (in negative)
    public int[] TrameType = new int[0];
    public string[] TrameData = new string[0];    //si es una figura, el numero, sino el texto
    public string[] TrameStyle = new string[0]; // style handle
    public float[] TrameScale = new float[0];
    public float[] TrameRotation = new float[0];
    public float[] TrameOffX = new float[0];
    public float[] TrameOffY = new float[0];
    public int nTrames;
    public float Length;
    public bool Continuous;
    public int Index;
    public int Flags;
}