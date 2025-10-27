// Gambas class file
using Gaucho;

public class Sheet
{
    public const string Gender = "SHEET";
    public string Name = "";                       //// Nombre visible en la tabstrip
    public string id = "";                         //// the handle de este LAYOUT
    public Block Block = new Block();                       //// el bloque asociado a esta hoja
    public bool IsModel = false;                   //// si es Model, esta hoja no puede usarse para insertar Viewports de models
    // public model3d As New Model3d
    public double PaperSizeW = 420F;           //// mm
    public double PaperSizeH = 297F;           //// mm
    // public int BackGroundColor = Colors.Black;       ////
    // public int WhiteAndBlack = Colors.White;       ////

    public double Scale = 1;                   //// a general scale for its parts
    public PrintStyle pPrintStyle = new PrintStyle();

    //// position inside parent GL Area
    public double PanX;          //// PIXELS
    public double PanY;          //// PIXELS
    public double PanZ = 0;

    public double RotX = 0;
    public double RotY = 0;          //// radians
    public double RotZ = 0;

    public double ScaleZoom = 1;
    public double ScaleZoomLast = 1;
    //// Escalado para evitar errores de precision matematica
    // public double PanBaseX = 0;                     //// pixeles
// public double PanBaseY = 0;                     //// pixeles
    public double PanBaseRealX = 0;                 //// REAL
    public double PanBaseRealY = 0;                 //// REAL

    public double ScaleZoomBase = 1;

    public Viewport Viewport = new Viewport();                         //// el viewport actual en uso

    public Dictionary<string, Viewport> Viewports = new Dictionary<string, Viewport>(); // of Viewports
    public int TabOrder = 0;
    public Dictionary<string, Entity> Entities = new Dictionary<string, Entity>();                      //// Apunta a .BLock.entities
    public List<Entity> EntitiesSelected = new List<Entity>();
    public List<Entity> EntitiesSelectedPrevious = new List<Entity>();
    public List<Entity> EntitiesVisibles = new List<Entity>();

    public Entity SkipSearch = new();                         //// entidad que no se tendran en cuenta en las busquedas
                                                    // public EntitiesSelected As New Collection

    public Gtk.GLArea? GlSheet = new ();
    public int GlListAllEntities = 0;
    public int GlListEntitiesSelected = 0;
    public int GlList3D = 0;
    // public Scene3D scene = new Scene3D();
    public bool Shown = false; // esto podria ser una flag, determino si ya la mostre una vez

    public Grip[] Grips = [];

    public Sheet()
    {
        PanX = -PaperSizeW / 2;
        PanY = -PaperSizeH / 2;
    }
}