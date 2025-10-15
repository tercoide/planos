// Gambas class file
public class Layer
{
    public const string Gender = "LAYER";
    public int Colour = 0;
    public LineType? LineType ; // String = "CONTINUOUS"
    public string Name = "";
    public int LineWt = -3;
    public string id="";
    public bool Visible = true;
    public bool Frozen = false;
    public bool Locked = false;
    public bool Printable = true;
    public bool Hidden = false;                // Esta propiedad no puede ser modificada por el user
    public int glList;
    public bool flgForDeletion = false;
    public bool flgForRegen = false;

    // Variables utiles para automatizar procesos de edicion genralizados
    public string[] _NoShow = ["gllist", "flgForDeletion", "flgForRegen"];
    public string[] _ReadOnly = ["id"];
    // public Dictionary<string,string> _EditWith = ["colour": fColors, "linetype": "linetypes"];
}
