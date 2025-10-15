public class Letter
{

    // Las letras LFF se dibujan como una LWPOLYLINE, con trazos y bulges.
    public int Code;
    public float[][] FontGlyps = new float[0][];              // las lineas que dibujan uns letra
    public float[][] FontBulges = new float[0][]; // los semicirculos que forman la letra

}

public class Font
{

    public string FileName="";
    public string FontName="";
    public float LetterSpacing=0;
    public float WordSpacing=0;
    public float LineSpacingFactor=0;
    public Dictionary<string, Letter> Letter = new Dictionary<string, Letter>(); // de LetterSt                // Codigo UTF-8 de las letras
}