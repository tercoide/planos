using Gaucho;
public class Block
{
    public const string Gender = "BLOCK";
    public string Name = "";                       //// nombre del bloque
    public string Description = "";
    public float x0 = 0;                          //// Insertion point X
    public float y0 = 0;                          //// Insertion point Y
    public float z0 = 0;                          //// Insertion point Z
    public int LineWt = -3;
    public int InsertionPlace;
    public int InsertUnits;   // 0=unitless
    public int Explotability; // 1=true
    public int Scalability;   // 1=true
    public int Flags;
    public string Layer = "";
    public Entity? Parent;                 //// la entidad a la que esta asociado este bloque o null
    public Sheet? Sheet;                   //// la hoja asociada
    public Dictionary<string, Entity> entities = []; //// las entidades que forman este bloque
    public bool Filled = false;                //// si este bloque ya se lleno de las entidades que lo definen
    public string id = "";                          // Code 5
    public string idContainer = "";                 // propiedad usado para compatibilidad con DXF
    public string idAsociatedLayout = "";           // code 340 del Layout asociado, o sea la Sheet donde se dibuja
    public bool IsReciclable = false;              //// si puedo reutilizarlo
    public bool IsAuxiliar = false;                //// Si es parte de otro bloque/hoja
    // public Model3D model3D;
    // Bloques e Insertos

    // los Bloques son conjuntos de entidades y que forman parte del dibujo. Incluso el Model es un bloque.
    // Estan en:
    //               Drawing.Blocks as Collection, ordenados por su nombre (no tienen Handle)

    // los Inserts son creados a partir de Blocks, y estan como entidad en

    //               Drawing.Entities , con su handle

    // pero tienen un bloque asociado que se guarda en:

    //               Drawing.Inserts as Collection, ordenados por su Handle, en este bloque estan referenciadas las entidades del insert (que pueden ser otro insert)
}