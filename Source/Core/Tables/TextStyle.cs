public class TextStyle
{
    // identification
    public string Id = "";
    public bool Available = false;

    public string Name = "";

    // style
    public float FixedH_40 = 0;
    public float fLastHeightUsed_42 = 0;
    public string sFont_3 = "";
    public string sBigFont_4 = "";

    public int Flags = 0;
    public int iDirection = 0;   // 2=mirror X;  40=mirror Y
    public float WidthFactor = 0;
    public float ObliqueAngle = 0;

    // temas de MText
    public string FontName = "";
    public bool Bold = false;
    public bool Italic = false;
    public int Paragraph = 0; // no se como se usa
    public int cadColor = 0;
    public bool StrikeTrough = false;
    public float SpacingFactor = 0;
    public int CodePage = 0;  // ??????????
    public int AlignVert = 0;
    public int AlignHoriz = 0;
    public int[] TabSpacing = [0, 4, 6, 90]; // 0,4,6,90 user defined
    public int TabSpacingDefault = 4;
}