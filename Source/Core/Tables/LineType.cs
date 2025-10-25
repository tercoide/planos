// Gambas class file
public class LineType
{
    public const string Gender = "LINETYPE";
    public int Colour;
    // public string LineType;
    public string Name="";
    public string Description ="";
    public string id = "";
    public List<float> TrameLength = new List<float>(); // allways start with pixels drawn, then space length (in negative)
    public List<int> TrameType = new List<int>();
    public List<string> TrameData = new List<string>();    //si es una figura, el numero, sino el texto
    public List<string> TrameStyle = new List<string>(); // style handle
    public List<float> TrameScale = new List<float>();
    public List<float> TrameRotation = new List<float>();
    public List<float> TrameOffX = new List<float>();
    public List<float> TrameOffY = new List<float>();
    public int nTrames;
    public float Length;
    public bool Continuous;
    public int Index;
    public int Flags;
}