// Gambas class file
using System.Drawing;

class Sheet
{
    public const string Gender = "SHEET";
    public string Name = "";                       //// Nombre visible en la tabstrip
    public string id = "";                         //// the handle de este LAYOUT
    public Block block = new Block();                       //// el bloque asociado a esta hoja
    public bool IsModel = false;                   //// si es Model, esta hoja no puede usarse para insertar Viewports de models
    // public model3d As New Model3d
    public float PaperSizeW = 420F;           //// mm
    public float PaperSizeH = 297F;           //// mm
    public Color BackGroundColor = Color.Black;       ////
    public Color WhiteAndBlack = Color.White;       ////

    public float Scale = 1;                   //// a general scale for its parts
    public PrintStyle pPrintStyle = new PrintStyle();

    //// position inside parent GL Area
    public float PanX;          //// PIXELS
    public float PanY;          //// PIXELS
    public float PanZ = 0;

    public float RotX = 0;
    public float RotY = 0;          //// radians
    public float RotZ = 0;

    public float ScaleZoom = 1;
    public float ScaleZoomLast = 1;
    //// Escalado para evitar errores de precision matematica
    // public float PanBaseX = 0;                     //// pixeles
// public float PanBaseY = 0;                     //// pixeles
    public float PanBaseRealX = 0;                 //// REAL
    public float PanBaseRealY = 0;                 //// REAL

    public float ScaleZoomBase = 1;

    //public Viewport Viewport = new Viewport();                         //// el viewport actual en uso

    //public Collection Viewports = new Collection(); // of Viewports
    public int TabOrder = 0;
    public List<Entity> Entities = new List<Entity>();                      //// Apunta a .BLock.entities
    public List<Entity> EntitiesSelected = new List<Entity>();
    public List<Entity> EntitiesSelectedPrevious = new List<Entity>();
    public List<Entity> EntitiesVisibles = new List<Entity>();

    public Entity SkipSearch = new Entity();                         //// entidad que no se tendran en cuenta en las busquedas
                                                    // public EntitiesSelected As New Collection

    //public gtk.GLArea GLArea = new gtk.GLArea();
    public int GlListAllEntities = 0;
    public int GlListEntitiesSelected = 0;
    public int GlList3D = 0;
    // public Scene3D scene = new Scene3D();
    public bool Shown = false; // esto podria ser una flag, determino si ya la mostre una vez

    public Grip[] Grips = new Grip[0];

    public Sheet()
    {
        PanX = -PaperSizeW / 2;
        PanY = -PaperSizeH / 2;
    }
}