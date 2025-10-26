// Gambas class file
public class LineType
{
    public const string Gender = "LINETYPE";
    public int Colour;
    // public string LineType;
    public string Name = "";
    public string Description = "";
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

public class MLineStyle
{
    // Basic identity / description (public fields)
    public string Name = string.Empty;
    public string Description = string.Empty;

    // Fill / cap / miter options
    public bool FillOn = false;
    public bool ShowMiters = false;

    public bool StartSquareCap = false;
    public bool StartInnerArc = false;
    public bool StartRound = false;

    public bool EndSquareCap = false;
    public bool EndInnerArc = false;
    public bool EndRound = false;

    // Colors, angles and element definitions
    public int FillColor = 0;

    /// <summary>Start angle in degrees (Gambas Float)</summary>
    public double StartAngle = 90.0;

    /// <summary>End angle in degrees (Gambas Float)</summary>
    public double EndAngle = 90.0;

    /// <summary>Number of elements (repeated hatch segments)</summary>
    public int Elements = 0;

    /// <summary>
    /// Element offsets (Gambas: ElemOffset As New Float[]).
    /// Stored as an array of doubles (x offsets or distances per element).
    /// </summary>
    public double[] ElemOffset = Array.Empty<double>();

    /// <summary>
    /// Element colors (Gambas: ElemColor As New Integer[]).
    /// </summary>
    public int[] ElemColor = Array.Empty<int>();

    /// <summary>
    /// Element linetypes (Gambas: ElemLinetype As New String[]; default BYLAYER in Gambas).
    /// </summary>
    public string[] ElemLinetype = Array.Empty<string>();

    /// <summary>
    /// Justification maximum offsets (top positive, bottom negative)
    /// </summary>
    public double JustificationTop = 0.0;
    public double JustificationBottom = 0.0;

    public MLineStyle() { }
}
