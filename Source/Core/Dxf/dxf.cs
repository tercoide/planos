using System.Security.Cryptography;
namespace Gaucho;
class dxf
{
 // Gambas module file

 // Organizacion del DXF y sus Handles
 // ----------------------------------
 //
 // HEADERS no usan handles
 // CLASSES no usan handles
 // TABLES
 //       VPORT, LTYPE, LAYER, STYLES, VIEWS, APPID, UCS, DIMSTYLE
 //               handles = variable
 //               owner  = 0, el handle 0 representa el Drawing madre
 //               Las tablas hijas tienen como owner a la cabecera, ver la tabla siguiente para ejemplo
 //       BLOCK_RECORD
 //               handle = variable, generalmente el 1
 //               layout = 0 o un OBJECT LAYOUT en caso de ser una Sheet como *Model *Paper_Space u otras
 //               owner  = 0
 //                 ^     BLOCK_RECORD1
 //                 |                 handles = hBlock_record <-+
 //                 +--------------<  owner  = 0                |
 // BLOCKS                                                      |
 //       BLOCK 1                                               |
 //               handle = hBlock1                              |
 //               owner  = hBlock_record                     >--+ el handle que se corresponde en el BLOCK_RECORD
 //               ENTITY1
 //                      handle = hEntity1
 //                      owner  = hBlock_record
 //           Las Sheets como *Model o *Paper_Space tienen un BLOCK asociado sin entidades
 //
 // ENTITIES
 //       ENTITY 1
 //               handle = hEntity1
 //               owner  =
 //       ENTITY INSERT
 //               handle = hEntityInsert
 //               owner  =
 //       ENTITY ATTRIB
 //               handle = hEntity2
 //               owner  = hEntityInsert
 // OBJECTS
 //     handle = variable HO
 //     owner  = 0

 //       Dictionary<string, Dictionary>
 //
 //       OBJECT 1
 //               handle = hObject1
 //               owner  = HO
 //       OBJECT LAYOUT
 //               handle = hObject1
 //               owner  = HO

public float LoadingPercent ;         
private float LoadLastPercent ;         
private int LoadTotalBytes ;         
private int LoadedBytes ;         
private int iHandle = 2;
private StreamReader fp ;         
private StreamWriter hFile ;         

private string lpCode ;         
private string lpValue ;         

private int LastCodeReadIndex = 0;
private bool eExports ;         

public Dictionary<string, Dictionary> hContainers =[];         
private Dictionary<string, Dictionary> ReadTimes =[];         
private Dictionary<string, Dictionary> ReadEntities =[];         
public Dictionary<string, Dictionary> cEntitiesUnread =[];         
public int nEntitiesUnread ;         
public int nEntitiesRead ;         

 // Codigos del DXF
const string codEntity = "0";
const string codid = "5";
const string codidContainer = "330";
const string codColor = "62";
const string codLType = "6";
const string codLayer = "8";
const string codLWht = "370";
const string codName = "2";
const string codX0 = "10";
const string codY0 = "20";
const string codZ0 = "30";
const string codX1 = "11";
const string codY1 = "21";
const string codZ1 = "31";
const string codX2 = "12";
const string codY2 = "22";
const string codZ2 = "32";
const string codX3 = "13";
const string codY3 = "23";
const string codZ3 = "33";
const string codCenterX = "10";
const string codCenterY = "20";
const string codCenterZ = "30";
const string codRadius = "40";
const string codAngleStart = "50";
const string codAngleEnd = "51";

public string DWGtoDXF(string sDwgFile)
    {


    string str ;         
    string tmpfile ;         
     // elimino el archivo temporal que hubiese creado
    tmpfile = sDwgFile + ".tmp";
        if (File.Exists(tmpfile)) File.Delete(tmpfile);
     // convierte DWG a DXF version 20
     // Shell "/usr/local/bin/dwgread; //" & sDwgFile & "// -O DXF -a r2010 -o //" & tmpfile & "//" Wait To str
    Gcd.debugInfo("Resultados de la conversion DWG a DXF " + str);
    
    return tmpfile;

}

 // Carga el DXF y lo mete en cModel del dibujo actual
 // Verbose=0 nada, 1=minimo, 2=grupos, 3=todo
public bool LoadFile(string sFile, Drawing drw, bool IgnoreTables= false, bool IgnoreBlocks= false, bool IgnoreHeader= false, int VerboseLevel= 0, bool UpdateGraphics= True, bool ReadObjects= True)
    {


    double t = Timer;
    Dictionary<string, Dictionary> cLlaveActual ;         
    Dictionary<string, Dictionary> cSectionActual ;         
    Dictionary<string, Dictionary> cTable ;         
    Dictionary<string, Dictionary> cToFill ;         

    fp =  Stream(sFile, FileMode.Open, FileAccess.Read); // For Read

    if ( !fp ) Error.Raise("Error !");

    LoadedBytes = 0;
    LoadTotalBytes = Lof(fp);

    // cEntitiesUnread = Dictionary<string, Dictionary>;
    // nEntitiesUnread = 0;
    // nEntitiesRead = 0;
    hContainers = new Dictionary<string, Dictionary>(); // Clave = Handle , Dato = Colection

        while ( (!fp.EndOfStream)
        {
            //Wait 0.0001
            ReadData;
            if (lpCode == "0" && lpValue == "SECTION")
            {

                // vemos que seccion es
                ReadData;
                if (lpCode == "2" && lpValue == "HEADER" && !IgnoreHeader)
                {
                    // // creo la llave, pero solo si es necesario
                    // if (!cToFill.ContainsKey("HEADER"))
                    // {
                    //     cLlaveActual = Dictionary<string, Dictionary>;
                    //     cToFill.Add("HEADER", cLlaveActual);
                    // }
                    // else
                    // {
                    //     cLlaveActual = cToFill["HEADER"];
                    // }

                    Load1HeadersDirect(drw.Headers);
                    if (VerboseLevel > 2) gcd.debugInfo("Leidos Headers", false, false, true);


                }

                if (lpCode == "2" && lpValue == "CLASSES")
                {

                    Load2Classes(drw);
                    if (VerboseLevel > 2) gcd.debugInfo("Leidas Classes", false, false, true);

                }

                if (lpCode == "2" && lpValue == "TABLES" && !IgnoreTables)
                {
                    if (!cToFill.ContainsKey("TABLES"))
                    {
                        var cLlaveActual = new Dictionary<string, Dictionary>();
                        cToFill.Add("TABLES", cLlaveActual);
                    }
                    else
                    {
                        cLlaveActual = cToFill["TABLES"];
                    }
                    Load3Tables(cLlaveActual);
                    if (VerboseLevel > 2) gcd.debugInfo("Leidos Tables", false, false, true);

                    // con las tablas cargadas, llenamoslas colecciones de objetos
                    ReadViewports(cToFill, drw);
                    ReadLTypes(cToFill, drw);
                    ReadStyles(cToFill, drw);
                    ReadLayers(cToFill, drw);
                    if (VerboseLevel > 2) gcd.debugInfo("Tables al Drawing", false, false, true);

                }

                //
                if (lpCode == "2" && lpValue == "BLOCKS" && !IgnoreBlocks)
                {
                    // creo la llave
                    cLlaveActual = new Dictionary<string, Dictionary>();
                    cToFill.Add("BLOCKS", cLlaveActual);
                    Load4Blocks(cLlaveActual);
                    if (VerboseLevel > 2) gcd.debugInfo("Leidos Blocks", false, false, true);

                }

                if (lpCode == "2" && lpValue == "ENTITIES")
                {
                    // creo la llave
                    cLlaveActual = new Dictionary<string, Dictionary>();
                    cToFill.Add("ENTITIES", cLlaveActual);

                    Load5Entities(cLlaveActual);
                    if (VerboseLevel > 2) gcd.debugInfo("Leidas Entidades", false, false, true);

                }
                //
                if (lpCode == "2" && lpValue == "OBJECTS")
                {

                    if (!cToFill.ContainsKey("OBJECTS"))
                    {
                        cLlaveActual = new Dictionary<string, Dictionary>();
                        cToFill.Add("OBJECTS", cLlaveActual);
                    }
                    else
                    {
                        cLlaveActual = cToFill["OBJECTS"];
                    }

                    Load6Objects(cLlaveActual);
                    if (VerboseLevel > 2) gcd.debugInfo("Leidos Objetos", false, false, true);

                }

                if (lpCode == "2" && lpValue == "THUMBNAILIMAGE")
                {

                    if (!cToFill.ContainsKey("THUMBNAILIMAGE"))
                    {
                        cLlaveActual = new Dictionary<string, Dictionary>();
                        cToFill.Add("THUMBNAILIMAGE", cLlaveActual);
                    }
                    else
                    {
                        cLlaveActual = cToFill["THUMBNAILIMAGE"];
                    }

                    Load7Thumbnail(cLlaveActual);

                }

            }
        }
        ;
    gcd.debugInfo("DXF a Dictionary<string, Dictionary>",false,false,true, True);

    if ( ReadObjects ) ReadObjectsFromDXF(cToFill, drw);
    if ( UpdateGraphics )
    {
        ImportBlocksFromDXF(cToFill, drw);
        
         //depre clsEntities.BuildPoi()
        DXFtoEntity(cToFill["ENTITIES"], drw);
        
        gcd.debugInfo("Drawing generated",false,false,true, True);
         //clsEntities.DeExtrude(drw)
        clsEntities.BuildGeometry();
        
         //gcd.DigestInserts()
        SetViewports(cToFill, drw);
        gcd.debugInfo("Geometry generated",false,false,true, True);
        
    }

     // For Each ft As Float In ReadTimes
     //     gcd.debugInfo(ReadEntities[ReadTimes.Key] & " " & gb.Tab & ReadTimes.Key & gb.Tab & gb.Tab & gb.Tab & " total time: " & Format(fT, "0.0000"))
     // Next
     // Wait
     // If VerboseLevel > 1 Then
     //     If VerboseLevel > 2 Then
     //         gcd.debuginfo("DXF: Leidas " & nEntitiesread & " entidades")
     //         If cEntitiesUnread.Count > 0 Then
     //             gcd.debuginfo("DXF: Un total de" & nEntitiesUnread & " entidades no pudieron ser leidas:")
     //             For Each unread As String In cEntitiesUnread
     //                 Print unread
     //             Next
     //         Endif
     //         Print
     //     End If
     //     gcd.debuginfo("DXF: fin lectura en " & Str(Timer - t))
     //
     // Else
     //     gcd.debuginfo("DXF: fin lectura en " & Str(Timer - t))
     // End If
     // Wait
    return false;

}

private void DiscardBlocks(Drawing drw)
    {


    Block b ;         

    foreach ( var b in drw.Blocks)
    {
        if ( (Left(b.name) == "*") && (b.idAsociatedLayout != "0") ) drw.Blocks.Remove(drw.Blocks.Key);
    }

}

private void ReadData()
    {


    lpCode = fp.ReadLine();
    lpValue = fp.ReadLine();

    LoadedBytes += lpCode != null ? lpCode.Length : 0;
    LoadedBytes += lpValue != null ? lpValue.Length : 0;

    if (lpCode != null && lpCode.Length > 0 && lpCode.Substring(lpCode.Length - 1, 1) == gb.Cr) lpCode = lpCode.Substring(0, lpCode.Length - 1);
    if (lpValue != null && lpValue.Length > 0 && lpValue.Substring(lpValue.Length - 1, 1) == gb.Cr) lpValue = lpValue.Substring(0, lpValue.Length - 1);

    lpCode = lpCode.Trim();
    lpValue = lpValue.Trim();

     // updating percentage

    LoadingPercent = LoadedBytes / LoadTotalBytes;

    if ( LoadingPercent - LoadLastPercent > 0.01 )
    {
        Gcd.debugInfo("Loging file " + CInt(LoadingPercent * 100) + "%", True, True);
        LoadLastPercent = LoadingPercent;
    }

}

private void Load1HeadersDirect(Headers Headers)
    {


     // Los header se guardan asi
     //   9                     Indica que es una variable
     // $EXTMAX                 Nombre de la variable
     //  10                     Tipo de dato1
     // 198.0411690635561       Dato1
     //  20                     Tipo de dato2
     // 178.7767572407179       Dato2
     //  30                     etc
     //   0

     // Yo usare dos colecciones

    string sVarName ;         
    string[] cVariable ;         
    string v ;         
     float[] slx ;         
     Integer[] inx ;         
    int i ;         

    ReadData;
        do {

            if (lpCode == "0" && lpValue == "ENDSEC") Break;

            if (lpcode == "9") // nueva variable
            {
                cVariable = new string[] { };
                sVarName = Mid(lpvalue, 2);

                do { // este bucle es por si la variable es un array
                    ReadData;
                    if (lpcode == "0" || lpCode == "9") Break;
                    cVariable.Add(lpvalue);
                }
            if (!SetValues(sVarName, cVariable)) gcd.debugInfo("Var " + sVarName + " not found.");
                Inc i;

            }

        } while ( (!Eof(fp));  
    

    gcd.debuginfo("DXF: Leidas " + i + " variables de ambiente");

}

private void Load2Classes(Drawing drwLoading)
    {


    CadClass cClass ;

        do {

            if (lpValue == "CLASSES") ReadData;
            if (lpValue == "ENDSEC") return;

            cClass = CadClass;
            drwLoading.CadClasses.Add(cClass);

            ReadData;

            while ( ((lpcode<> "0") && !fp.EndOfStream());
            {
            if (lpcode == "0") cClass.recordtype = lpValue;
            if (lpcode == "1") cClass.recordname = lpValue;
            if (lpcode == "2") cClass.CPPName = lpValue;
            if (lpcode == "3") cClass.AppName = lpValue;
            if (lpcode == "90") cClass.ProxyCapp = CInt(lpValue);
            if (lpcode == "91") cClass.InstanceCount = CInt(lpValue);
            if (lpcode == "280") cClass.ProxyFlag = CInt(lpValue);
            if (lpcode == "281") cClass.EntityFlag = CInt(lpValue);

            ReadData;

        };

    }

}

private void Load3Tables(Dictionary<string, Dictionary> cTables)
    {


    string sTableName ;         
    string sTableid ;          // in hex
    string sTableContainer ;          // in hex , 0 = nobody
    int iTableEntries ;         
    Dictionary<string, Dictionary> cTable ;         

     // creamos una table inicial con los handles de las tables
    cTable = Dictionary<string, Dictionary>;
    cTables.Add("__AuxData__", cTable);

    ReadData;
    do {
        if ( Eof(fp) ) Break;

        if ( lpCode == "0" && lpValue == "ENDSEC" ) Break;

        if ( lpCode == "0" && lpValue == "TABLE" )
        {

             // OBTENGO DATOS DE LA TABLA
             // -1 APP: entity name(changes Each Time a drawing Is Opened)
             // 0 Object type(TABLE)
             // 2 Table name
             // 5 Handle
             // 330 Soft - pointer ID / handle To owner object
             // 100 Subclass marker(AcDbSymbolTable)
             // 70 Maximum number Of entries In table
            ReadData;
            while ( lpcode <> "0")
            {

                if ( lpcode == "5" ) sTableid = lpvalue;

                if ( lpcode == "2" ) sTableName = lpvalue;
                if ( lpcode == "330" ) sTableContainer = lpvalue;

                 //If sTableName = "VIEW" Then Stop

                 // WARNING: este dato no es valido para todas las versiones de DXF
                 // en algunos archivos hay mas tablas que lo que indica este numero
                 // No hay que darle importancia a este numero!!!
                if ( lpcode == "70" ) iTableEntries = CInt(lpvalue);

                ReadData;
            }

         

            cTable = Dictionary<string, Dictionary>;

            cTables.Add(sTableName, cTable);

             // verifico que la tabla no tenga entradas, lo que me altera la carga
            if ( lpvalue != "ENDTAB" )
            {
                 //Object(cTable, sTableHandle)
                Load31Table(cTable, iTableEntries);
            }
        }
        ReadData;
    }

}
 // Lee todas las tables de esta table

private void Load31Table(Dictionary<string, Dictionary> cVars, int iEntries)
    {


     // Yo usare dos colecciones

    string sTableName ;         
    string sid ;         
    Dictionary<string, Dictionary> cTable = [];         
    int i ;         

    int iCode ;         
    string Key ;         

     // Tengo q leer iEntries
     //For i = 1 To iEntries
    do {
        Inc i;
        cTable = Dictionary<string, Dictionary>;
        sTableName = "";
        iCode = 0;

        ReadData;

         // esto lee todas las tables en la table

         //If lpCode = "0" Then Break

        while ( lpcode != "0"){
            Key = lpcode;
            if ( cTable.ContainsKey(Key) )
            {
                do {
                    iCode += 1;
                    Key = lpcode + "_" + CStr(iCode);

                    if ( ! cTable.ContainsKey(Key) ) Break;
                }
            }
            cTable.Add(Key, lpvalue);

            if ( lpcode == Me.codid ) sTableName = lpvalue;
            ReadData;

        }
         //If cTable.Count = 1 Then Stop
        if ( cTable.Count > 0 )
        {
            if ( sTableName == "" ) sTableName = CStr(i);
            cVars.Add(sTableName, cTable);

        }

        if ( lpcode == "0" && lpValue == "ENDTAB" ) Break;

    }

    if ( cTable.ContainsKey("5") )
    {
        sid = cTable["5"];
    }
    else if ( cTable.ContainsKey("105") )
    {
        sid = cTable["105"];
    }
    else if ( cTable.ContainsKey("2") )
    {
        sid = cTable["2"];
    }
    else
    {
        sid = gcd.id();

    }
     //Object(cTable, sHandle)

     //gcd.JSONtoLayers

    gcd.debugInfo("DXF: Leidas" + cTable.Values.Count.ToString() + " tablas");

}

private void Load4Blocks(Dictionary<string, Dictionary> cBlocks)
    {


    Block mBlock ;         
    Variant unread ;         
    int i ;         

    string sTableName ;         

    Dictionary<string, Dictionary> cTable ;         
    Dictionary<string, Dictionary> cEntities ;         

    int iCode ;         
    string Key ;         

    ReadData;
    do {

        mBlock =  Block;

        if ( lpCode == "0" && lpValue == "ENDSEC" ) Break;

        if ( (lpcode == "0") && (lpvalue == "BLOCK") )
        {
            Inc i;
            cTable = Dictionary<string, Dictionary>;

            ReadData;

            if ( lpcode == "" ) Break;

            while ( lpcode != "0") {
                Key = lpcode;
                if ( cTable.ContainsKey(Key) )
                {
                    do
                    {
                        iCode += 1;
                        Key = lpcode + "_" + CStr(iCode);

                    } while ( cTable.ContainsKey(Key));
                    
                }

                if ( lpcode == Me.codid ) sTableName = lpvalue;
                cTable.Add(Key, lpvalue);
                ReadData;

            } // fin del encabezado del Block, siguen sus entidades
             //Object(cTable, cTable["5"])
             // si estoy leyendo bloques, significa que estoy abriendo un plano
            cEntities = Dictionary<string, Dictionary>;
            cTable.Add("entities", cEntities);

            Load5Entities(cEntities);

            if ( sTableName == "" ) sTableName = CStr(i);

            cBlocks.Add(sTableName, cTable);

        }
    }

    gcd.debuginfo("DXF: Leidos " + cBlocks.Count + " bloques");

}

private void Load5Entities(Dictionary<string, Dictionary> cEntities)
    {


    string[] sClave ;         
    string[] sValue ;         
    string sEntidad ;         
    string sKey ;         
    Object clsidr ;         
    Entity eNueva ;         
    bool Reads ;         

    Dictionary<string, Dictionary> cEntity ;         
    int iEntity ;         

    int iCode ;         
    string Key ;

        do {
            //Debug lpcode, lpvalue
            sClave = String[];

            sValue = String[];

            if (lpValue == "ENTITIES") ReadData;
            if (lpValue == "ENDSEC") return;

            sEntidad = lpValue;
            Inc iEntity;
            cEntity = new Dictionary<string, string>();

            cEntity.Add("0", sEntidad);
            iCode = 0;

            // Leo descentralizadamente las entidades
            ReadData;

            while ((lpcode != "0") && !fp.EndOfStream) {

            Key = lpcode;
            if (cEntity.ContainsKey(Key))
            {
                do
                {
                    iCode += 1;
                    Key = lpcode + "_" + CStr(iCode);

                    
                } while (cEntity.ContainsKey(Key));
            }

            if (sEntidad != "ENDSEC") cEntity.Add(Key, lpvalue);
            ReadData;

        }

         //Object(cEntity, cEntity[dxf.codHandle])

        if ( cEntity.ContainsKey(dxf.codid) )
        {
            sKey = cEntity[dxf.codid];
        }
        if ( sKey == "" )
        {

            sKey = gcd.Id();

        }

        if ( sEntidad != "ENDBLK" ) cEntities.Add(sKey, cEntity);

        if ( sEntidad == "ENDBLK" || sEntidad == "ENDSEC" ) return;

    }

}

private void Load6Objects(Dictionary<string, Dictionary> cObjects)
    {


    string[] sClave ;         
    string[] sValue ;         
    string sEntidad ;         
    string h ;         
    Object clsidr ;         
    Entity eNueva ;         
    bool Reads ;         

    Dictionary<string, Dictionary> cObject ;         
    int iObject ;         

    int iCode ;         
    string Key ;         

    do {
         //Debug lpcode, lpvalue
        sClave =  String[];

        sValue =  String[];

        if ( lpValue == "OBJECTS" ) ReadData;
        if ( lpValue == "ENDSEC" ) return;

        sEntidad = lpValue;
        Inc iObject;
        cObject = Dictionary<string, Dictionary>;

        cObject.Add("0", sEntidad);
        iCode = 0;

         // Leo descentralizadamente las entidades
        ReadData;

         //If sEntidad = "HATCH" Then Stop
        while ( (lpcode != "0") && !fp.EndOfStream) {

            Key = lpcode;
            if ( cObject.ContainsKey(Key) )
            {
                do {
                    iCode += 1;
                    Key = lpcode + "_" + CStr(iCode);

                    
                } while ( !cObject.ContainsKey(Key) );
            }
            cObject.Add(Key, lpvalue);
            ReadData;

        }
         //Object(cObject, cObject["5"])
        if ( cObject.ContainsKey("5") )
        {
            h = cObject["5"];
        }
        else
        {
            h = CStr(iObject);
        }
        cObjects.Add(h, cObject);

        if ( sEntidad == "ENDBLK" || sEntidad == "" ) return;

    }

}

private void Load7Thumbnail(Dictionary<string, Dictionary> cThumbnail)
    {


    int iCode ;         
    string Key ;         

    do {

        if ( lpValue == "ENDSEC" ) return;

         // Leo descentralizadamente las entidades
        ReadData;

        while ( (lpcode != "0") && !fp.EndOfStream) {

            Key = lpcode;
            if ( cThumbnail.ContainsKey(Key) )
            {
                do {
                    iCode += 1;
                    Key = lpcode + "_" + CStr(iCode);

                   
                } while ( !cThumbnail.ContainsKey(Key) );    
            }
            cThumbnail.Add(Key, lpvalue);
            ReadData;

        }

    } while ( !fp.EndOfStream );

}

public void ReconstructHandles(Drawing drw)
    {


    DictEntry di ;         
    DictList item ;         
    Block b ;         
     // Empiezo por los Bloques importantes
    foreach (var b in drw.Blocks)
    {
        if ( b.name == "*Model_Space" )
        {
            b.idContainer = Handle(2);
            b.id = Handle();
        }
        else if ( Left(b.name, 6) == "*Paper" )
        {
            b.idContainer = Handle();
            b.id = Handle();
        }
        else
        {
            b.id = "";
            b.idContainer = "";
        }
    }
     // Ahora creo un Diccionario
    drw.Dictionary<string, Dictionary>.id = Handle();
    di =  DictEntry;

    di.name = "ACAD_LAYOUT";
    di.id = Handle();
    drw.Dictionary<string, Dictionary>.Definitions.Add(di.name, di);

     // Ahora creo una entrada de diccionario para los Layout
    foreach ( var b in drw.Blocks)
    {
        if ( b.Sheet )
        {
            b.Sheet.id = Handle();
            item =  DictList;
            item.name = b.Sheet.Name;
            item.idSoftOwner = b.Sheet.id;
            drw.Dictionary<string, Dictionary>.Definitions["ACAD_LAYOUT"].items.Add(item);
        }
    }

     // asigno handles a las Tables

}

public int SaveFile(string sName, Drawing drwToSAve, bool LoadMinimal= false, bool SaveHeader= True, bool SaveTables= True, bool SaveBlocks= True, bool SaveThumbnail= True)
    {


    hFile = File.Open(sName, FileMode.Create);

     // Las HANDLES
     // 0 -> Es el Drawing
     // 1 -> Es la tabla Block_Record
     // 2 -> Es la entrada en el Block_Record del model view
     // 3 -> Es el BLOCK del *Model
     // 4 -> Es el Block_Record del *Paper
     // 5 -> Es el BLOCK del *Paper
     // A -> Es el OBJECT LAYOUT asociado al *Model
     // B -> Es el OBJECT LAYOUT asociado al *Paper
     //   -> Tabla LType
     //   -> Tabla Layer
     //   -> Tabla Style
     //   -> Tabla DimStyle
     //10 -> Tabla Viewport
     //   -> Tabla View
     //   -> Tabla AppId

     //
     // Las TABLES tiene su entrada y su owner es el 0=Drawing
     // Los BLOCKS tienen su handle y su owner es la entrada en el Block_Record
     // Las entidades tienen su handle y su owner es un BLOCK o el MODEL o algun PAPER o alguna entidad contenedora (INSERT HATCH, etc), que son bloques tambien
    gcd.ResetChronograph;
    ReconstructHandles(drwToSAve);
    // Inc Application.Busy;
    if ( SaveHeader )
    {
        if ( Save1HeadersAndVarsDirect(drwToSAve) ) Goto Error1;

        if ( Save2Classes(drwToSAve) ) Goto Error1;
    }
    if ( SaveTables )
    {
        if ( Save3TablesDirect(drwToSAve) ) Goto Error1;
    }
    if ( SaveBlocks )
    {
        if ( Save4BlocksDirect(drwToSAve) ) Goto Error1;

    }

    if ( Save5EntitiesDirect(drwToSAve) ) Goto Error1;
    if ( Save6Objects(drwToSAve) ) return false;
    if ( SaveThumbnail )
    {
        Save7ThumbNail(Null);
    }

    hFile.Close;
    // Dec Application.Busy;
    gcd.debugInfo(("Saved to ") + sName,false,false, True, True);
    return 0;
     // Catch
Error1:;
     //     hFile.Close
     //
     //     Message.Error(("Error saving file"))
     //     Return -1

}

private int Save1HeadersAndVarsDirect(Drawing drw)
    {


    string sValues ;         
    string[] stxHeaders ;         

    hFile.WriteLine( "999" + "\n");
    hFile.WriteLine( "GambasCAD" + "\n");
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "SECTION" + "\n");
    hFile.WriteLine( "2" + "\n");
    hFile.WriteLine( "HEADER" + "\n");

     //Intento guardar algunas cosas utiles para cuando abra de nuevo este archivo
    drw.Headers.CLAYER = drw.CurrLayer.Name; // Current LAYER

    stxHeaders = drw.Headers.ExportDXF();

    foreach ( sValues in stxHeaders)
    {
        hFile.WriteLine( sValues + "\n");
    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");

}

 // Las classes de cad no las usamos. En teoria, no tienen ninguna utilidad fuera de AutoCAD.
 // Abriendo un DXF, se guadaran todas las classes a efectos de recosntruir el DXF.
private int Save2Classes(Drawing drwSaving)
    {


    CadClass cClass ;         

     // Dim i As Integer

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "SECTION" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "CLASSES" + "\n");

     // For Each cClass In drwSaving.CadClasses
     //     Print hFile, "  0"
     //     Print hFile, "CLASS"
     //
     //     Print hFile, "  1"
     //     Print hFile, cClass.RecordName
     //
     //     Print hFile, "  2"
     //     Print hFile, cClass.CPPName
     //
     //     Print hFile, "  3"
     //     Print hFile, cClass.AppName
     //
     //     Print hFile, " 90"
     //     Print hFile, CStr(cClass.ProxyCapp)
     //
     //     Print hFile, " 91"
     //     Print hFile, CStr(cClass.InstanceCount)
     //
     //     Print hFile, "280"
     //     Print hFile, CStr(cClass.ProxyFlag)
     //
     //     Print hFile, "281"
     //     Print hFile, CStr(cClass.EntityFlag)
     //
     // Next
     // end section code
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");

}

private int Save3TablesDirect(Drawing drw)
    {


    Dictionary<string, Dictionary> cTable ;         
    Dictionary<string, Dictionary> cTableEntry ;         
    Dictionary<string, Dictionary> cVar ;         
    Dictionary<string, Dictionary> cVars ;         
    string sValues ;         
    string lpclave ;         
    string sHandle1 ;         
    string ss ;         
    string sTableid ;         
    int i ;         
    int iDimCounter ;         
    int iSheetCounter ;         
    Sheet s ;         
    Block b ;         
    Entity e ;         
    Entity e2 ;         

     // antes que nada armo el block_record
     //sHandle1 = Handle(1)
     // Handle_Block_REcord = Dictionary<string, Dictionary>
     // Handle_Block = Dictionary<string, Dictionary>
     // Handle_Layout = Dictionary<string, Dictionary>
     //

     // For Each s In drw.Sheets
     //
     //     s.Block.idContainer = Handle()
     //     s.Block.id = Handle()
     //     s.id = Handle()
     //     s.Block.idAsociatedLayout = s.id
     //     For Each e In s.Block.entities
     //         e.IdContainer = s.Block.idContainer
     //         e.id = Handle()
     //     Next
     //
     // Next
     // For Each s In drw.Sheets
     //
     //     s.Block.idContainer = Handle()
     //     s.Block.id = Handle()
     //     s.id = Handle()
     // Next
     // hObjects = Handle()
     // hDictionary<string, Dictionary> = Handle()
     // For Each b In drw.Blocks
     //     If b.Sheet Then Continue
     //     b.idContainer = Handle()
     //     b.id = Handle()
     //     // For Each e In b.entities
     //     //     e.IdContainer = b.idContainer
     //     //     e.id = Handle()
     //     // Next
     // Next

     // For Each s In drw.Sheets
     //     For Each e In s.Entities
     //         If InStr(e.Gender, "DIMENSION") > 0 Then
     //             e.pBlock.name = "*D" & Str(iDimCounter)
     //             e.pBlock.idContainer = Handle()
     //             e.pBlock.id = Handle()
     //             For Each e2 In e.pBlock.entities
     //                 e2.IdContainer = e.pBlock.idContainer
     //                 e2.id = Handle()
     //             Next
     //             // Handle_Block_Record.Add(Handle(), e.pBlock.name)
     //             // Handle_Block.Add(Handle(), e.pBlock.name)
     //             Inc iDimCounter
     //         Endif
     //     Next
     //     Inc iSheetCounter
     // Next

     // For Each s In drw.Sheets
     //     For Each e In s.Entities
     //         If InStr(e.Gender, "DIMENSION") > 0 Then
     //             For Each e2 In e.pBlock.entities
     //                 e2.IdContainer = e.pBlock.idContainer
     //                 e2.id = Handle()
     //             Next
     //             // Handle_Block_Record.Add(Handle(), e.pBlock.name)
     //             // Handle_Block.Add(Handle(), e.pBlock.name)
     //             Inc iDimCounter
     //         Endif
     //     Next
     //     Inc iSheetCounter
     // Next

     // Empiezo la seccion tables
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "SECTION" + "\n");
    hFile.WriteLine( "2" + "\n");
    hFile.WriteLine( "TABLES" + "\n");

    Save3TableViewPorts(drw);
    Save3TableLineTypes(drw);
    Save3TableLayers(drw);
    Save3TableTextStyles(drw);
    Save3TableViews(drw);
    Save3TableAppID(drw);
    Save3TableUCSs(drw);
    Save3TableDimStyles(drw);
    save31BlockRecord(drw);
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");

}

private int Save3TableAppID(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle();

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "APPID" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.AppIDs.Count) + "\n");

     // APPID
    APPID oneAppid ;         
    foreach ( oneAppid in drw.AppIDs)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "APPID" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n"); //oneAppid.id
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbRegAppTableRecord" + "\n");
        hFile.WriteLine( "  2" + "\n");
        hFile.WriteLine( oneAppid.APPName_2 + "\n");
        hFile.WriteLine( " 70" + "\n");
        hFile.WriteLine( CStr(oneAppid.Flags_70) + "\n");
    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableLayers(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle();

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "LAYER" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.Layers.Count) + "\n");

    Layer oneLayer ;         
    foreach ( oneLayer in drw.Layers)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "LAYER" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbLayerTableRecord" + "\n");
        hFile.WriteLine( "  2" + "\n");
        hFile.WriteLine( oneLayer.Name + "\n");
        hFile.WriteLine( " 70" + "\n"); // layer flags, bit coded
        hFile.WriteLine( CStr(-oneLayer.Frozen - oneLayer.Locked * 4) + "\n");
        hFile.WriteLine( " 62" + "\n");
        hFile.WriteLine( oneLayer.Colour * IIf(oneLayer.Visible, 1, -1) + "\n");
        hFile.WriteLine( "  6" + "\n");
        hFile.WriteLine( oneLayer.LineType.Name + "\n");
        hFile.WriteLine( "290" + "\n"); // plotting flag
        hFile.WriteLine( IIf(oneLayer.Printable, "1", "0") + "\n");
        hFile.WriteLine( "370" + "\n"); // lit
        hFile.WriteLine( CStr(oneLayer.Lit) + "\n");
        hFile.WriteLine( "390" + "\n"); // plotstyle object
        hFile.WriteLine( " " + "\n");
        hFile.WriteLine( "347" + "\n"); // material
        hFile.WriteLine( " " + "\n");

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableTextStyles(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle();

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "STYLE" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.TextStyles.Count) + "\n");

    TextStyle oneTextStyle ;         
    foreach ( oneTextStyle in drw.TextStyles)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "STYLE" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbTextStyleTableRecord" + "\n");
        hFile.WriteLine( "  2" + "\n");
        hFile.WriteLine( oneTextStyle.name + "\n");
        hFile.WriteLine( " 70" + "\n"); // flags, bit coded
        hFile.WriteLine( oneTextStyle.Flags + "\n");
        hFile.WriteLine( " 40" + "\n");
        hFile.WriteLine( CStr(oneTextStyle.FixedH_40) + "\n");
        hFile.WriteLine( " 41" + "\n");
        hFile.WriteLine( CStr(oneTextStyle.WidthFactor) + "\n");
        hFile.WriteLine( " 50" + "\n");
        hFile.WriteLine( CStr(oneTextStyle.ObliqueAngle) + "\n");
        hFile.WriteLine( " 71" + "\n");
        hFile.WriteLine( CStr(oneTextStyle.iDirection) + "\n");
        hFile.WriteLine( " 42" + "\n");
        hFile.WriteLine( CStr(oneTextStyle.fLastHeightUsed_42) + "\n");
        hFile.WriteLine( "  3" + "\n");
        hFile.WriteLine( oneTextStyle.sFont_3 + "\n");
        hFile.WriteLine( "  4" + "\n");
        hFile.WriteLine( oneTextStyle.sBigFont_4 + "\n");

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableDimStyles(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.DimStyles)

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "DIMSTYLE" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.DimStyles.Count) + "\n");

    DimStyle oneDimtStyle ;         
    foreach ( oneDimtStyle in drw.DimStyles)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "DIMSTYLE" + "\n");
        hFile.WriteLine( " 105" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( " 330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( " 100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( " 100" + "\n");
        hFile.WriteLine( "AcDbDimStyleTableRecord" + "\n");
        hFile.WriteLine( "  2" + "\n");
        hFile.WriteLine( oneDimtStyle.name + "\n");
        hFile.WriteLine( " 70" + "\n"); // flags, bit coded
        hFile.WriteLine( "0" + "\n"); // no lo usamos
        SaveCode(3, oneDimtStyle.DIMPOST);
        SaveCode(4, oneDimtStyle.DIMAPOST);
        SaveCode(5, oneDimtStyle.DIMBLK);
        SaveCode(6, oneDimtStyle.DIMBLK1);
        SaveCode(7, oneDimtStyle.DIMBLK2);
        SaveCode(40, oneDimtStyle.DIMSCALE);
        SaveCode(41, oneDimtStyle.DIMASZ);
        SaveCode(42, oneDimtStyle.DIMEXO);
        SaveCode(43, oneDimtStyle.DIMDLI);
        SaveCode(44, oneDimtStyle.DIMEXE);
        SaveCode(45, oneDimtStyle.DIMRND);
        SaveCode(46, oneDimtStyle.DIMDLE);
        SaveCode(47, oneDimtStyle.DIMTP);
        SaveCode(48, oneDimtStyle.DIMTM);
        SaveCode(140, oneDimtStyle.DIMTXT);
        SaveCode(141, oneDimtStyle.DIMCEN);
        SaveCode(142, oneDimtStyle.DIMTSZ);
        SaveCode(143, oneDimtStyle.DIMALTF);
        SaveCode(144, oneDimtStyle.DIMLFAC);
        SaveCode(145, oneDimtStyle.DIMTVP);
        SaveCode(146, oneDimtStyle.DIMTFAC);
        SaveCode(147, oneDimtStyle.DIMGAP);
        SaveCode(148, oneDimtStyle.DIMALTRND);
        SaveCode(71, oneDimtStyle.DIMTOL);
        SaveCode(72, oneDimtStyle.DIMLIM);
        SaveCode(73, oneDimtStyle.DIMTIH);
        SaveCode(74, oneDimtStyle.DIMTOH);
        SaveCode(75, oneDimtStyle.DIMSE1);
        SaveCode(76, oneDimtStyle.DIMSE2);
        SaveCode(77, oneDimtStyle.DIMTAD);
        SaveCode(78, oneDimtStyle.DIMZIN);
        SaveCode(79, oneDimtStyle.DIMAZIN);
        SaveCode(170, oneDimtStyle.DIMALT);
        SaveCode(171, oneDimtStyle.DIMALTD);
        SaveCode(172, oneDimtStyle.DIMTOFL);
        SaveCode(173, oneDimtStyle.DIMSAH);
        SaveCode(174, oneDimtStyle.DIMTIX);
        SaveCode(175, oneDimtStyle.DIMSOXD);
        SaveCode(176, oneDimtStyle.DIMCLRD);
        SaveCode(177, oneDimtStyle.DIMCLRE);
        SaveCode(178, oneDimtStyle.DIMCLRT);
        SaveCode(179, oneDimtStyle.DIMADEC);
        SaveCode(270, oneDimtStyle.DIMUNIT);
        SaveCode(271, oneDimtStyle.DIMDEC);
        SaveCode(272, oneDimtStyle.DIMTDEC);
        SaveCode(273, oneDimtStyle.DIMALTU);
        SaveCode(274, oneDimtStyle.DIMALTTD);
        SaveCode(275, oneDimtStyle.DIMAUNIT);
        SaveCode(276, oneDimtStyle.DIMFRAC);
        SaveCode(277, oneDimtStyle.DIMLUNIT);
        SaveCode(278, oneDimtStyle.DIMDSEP);
        SaveCode(279, oneDimtStyle.DIMTMOVE);
        SaveCode(280, oneDimtStyle.DIMJUST);
        SaveCode(281, oneDimtStyle.DIMSD1);
        SaveCode(282, oneDimtStyle.DIMSD2);
        SaveCode(283, oneDimtStyle.DIMTOLJ);
        SaveCode(284, oneDimtStyle.DIMTZIN);
        SaveCode(285, oneDimtStyle.DIMALTZ);
        SaveCode(286, oneDimtStyle.DIMALTTZ);
        SaveCode(287, oneDimtStyle.DIMFIT);
        SaveCode(288, oneDimtStyle.DIMUPT);
        SaveCode(289, oneDimtStyle.DIMATFIT);
        SaveCode(340, oneDimtStyle.DIMTXSTY);
        SaveCode(341, oneDimtStyle.DIMLDRBLK);
        SaveCode(342, oneDimtStyle.DIMBLK);
        SaveCode(343, oneDimtStyle.DIMBLK1);
        SaveCode(344, oneDimtStyle.DIMBLK2);
        SaveCode(371, oneDimtStyle.DIMLWD);
        SaveCode(372, oneDimtStyle.DIMLWE);

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableLineTypes(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.LineTypes)

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "LTYPE" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.LineTypes.Count) + "\n");

    LineType oneLtype ;         
    foreach ( oneLtype in drw.LineTypes)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "LTYPE" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbLinetypeTableRecord" + "\n");
        SaveCode(2, oneLtype.Name);
        SaveCode(70, oneLtype.Flags);
        SaveCode(3, oneLtype.Description);
        SaveCode(72, 65); // para compatibilidad
        SaveCode(73, oneLtype.nTrames);
        SaveCode(40, CStr(oneLtype.Length));
        foreach ( fLenght As double in oneLtype.TrameLength)
        {
            SaveCode(49, CStr(fLenght));
        }

         // Hay tipos de linea mas complejos, que se generan con codigos que GambasCAD no maneja de momento

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableUCSs(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.UCSs)

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "UCS" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.UCSs.Count) + "\n");

    UCS oneUCS ;         
    foreach ( oneUCS in drw.UCSs)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "UCS" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbUCSTableRecord" + "\n");
        SaveCode(2, oneUCS.Name_2);
        SaveCode(70, oneUCS.Flags_70);
        SaveCode(10, oneUCS.OriginX_10);
        SaveCode(20, oneUCS.OriginY_20);
        SaveCode(30, oneUCS.OriginZ_30);

        SaveCode(11, oneUCS.XAxisX_11);
        SaveCode(21, oneUCS.XAxisY_21);
        SaveCode(31, oneUCS.XAxisZ_31);

        SaveCode(12, oneUCS.YAxisX_12);
        SaveCode(22, oneUCS.YAxisY_22);
        SaveCode(32, oneUCS.YAxisZ_32);

        SaveCode(79, 0);

        SaveCode(146, oneUCS.Elevation_146);
        SaveCode(346, oneUCS.BaseUCS_346);

        SaveCode(13, oneUCS.OriginForThisOrthographicTypeX_13);
        SaveCode(23, oneUCS.OriginForThisOrthographicTypeY_23);
        SaveCode(33, oneUCS.OriginForThisOrthographicTypeZ_33);

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableViews(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.Views)

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "VIEW" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.Views.Count) + "\n");

    View oneView ;         
    string sData ;         
    foreach ( oneView in drw.Views)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "VIEW" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbViewTableRecord" + "\n");
        foreach ( sData in oneView.Datos)
        {
            hFile.WriteLine( sData + "\n");
        }

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save3TableViewPorts(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.Viewports)

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "VPORT" + "\n");
    hFile.WriteLine( "  5" + "\n"); // handle
    hFile.WriteLine( hTableHandle + "\n");
    hFile.WriteLine( "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "0" + "\n");
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine("  70" + "\n");
    hFile.WriteLine(CStr(drw.Viewports.Count) + "\n");

    Viewport oneViewport ;         
    string sData ;         
    foreach ( oneViewport in drw.Viewports)
    {

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "VPORT" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle propio
        hFile.WriteLine( Handle() + "\n");
        hFile.WriteLine( "  330" + "\n"); // handle del padre
        hFile.WriteLine( hTableHandle + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( "  100" + "\n");
        hFile.WriteLine( "AcDbViewportTableRecord" + "\n");
        foreach ( sData in oneViewport.Datos)
        {
            hFile.WriteLine( sData + "\n");
        }

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int save31BlockRecord(Drawing drw)
    {


    Block eBlock ;         
    Sheet s ;         
    Dictionary<string, Dictionary> cTable ;         
    Dictionary<string, Dictionary> cTableEntry ;         
    Dictionary<string, Dictionary> cVar ;         
    Dictionary<string, Dictionary> cVars ;         
    string sValues ;         
    string lpclave ;         
    string sHandle1 ;         
    string sTableid ;         
    string sBlockName ;         
    string sBlockHandle ;         
    int i ;         
    int iDimCounter ;         
    int iPaperSpaceCounter ;         
    Block b ;         
    Entity e ;         

    string idAsociatedLayout ;         

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "TABLE" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "BLOCK_RECORD" + "\n");
    hFile.WriteLine( "  5" + "\n");
    hFile.WriteLine( "1" + "\n"); // siempre es 1
    hFile.WriteLine( "  330" + "\n");
    hFile.WriteLine( "0" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    hFile.WriteLine( "  100" + "\n");
    hFile.WriteLine( "AcDbSymbolTable" + "\n");
    hFile.WriteLine( "  70" + "\n");
    hFile.WriteLine( CStr(drw.Sheets.Count + drw.Blocks.Count + iDimCounter) + "\n");

    foreach ( b in drw.Blocks)
    {
        if ( b.idContainer == "" )
        {
            b.idContainer = Handle();
            b.id = Handle();
        }

        if ( b.Sheet )
        {
            idAsociatedLayout = b.Sheet.id;
        }
        else
        {
            idAsociatedLayout = "0";
        }

        hFile.WriteLine( "  0" + "\n");
        hFile.WriteLine( "BLOCK_RECORD" + "\n");
        hFile.WriteLine( "  5" + "\n"); // handle
        hFile.WriteLine( b.idContainer + "\n");
        hFile.WriteLine( " 330" + "\n");
        hFile.WriteLine( "1" + "\n");
        hFile.WriteLine( " 100" + "\n");
        hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
        hFile.WriteLine( " 100" + "\n");
        hFile.WriteLine( "AcDbBlockTableRecord" + "\n");
        hFile.WriteLine( "  2" + "\n");
        hFile.WriteLine( b.name + "\n");
        hFile.WriteLine( " 340" + "\n");
        hFile.WriteLine( idAsociatedLayout + "\n");
        hFile.WriteLine( "  70" + "\n");
        hFile.WriteLine( b.InsertUnits + "\n");
        hFile.WriteLine( " 280" + "\n");
        hFile.WriteLine( b.Explotability + "\n");
        hFile.WriteLine( " 281" + "\n");
        hFile.WriteLine( b.Scalability + "\n");
    }

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");

}

private int Save4BlocksDirect(Drawing drw)
    {


    int i ;         
    int iPaperSpaceCounter ;         
    bool bCan ;         
    int iii ;         
    Block eBlock ;         
     String[] stxEnty ;         
    Entity eEnty ;         
    string sValues ;         
    string lpclave ;         
    string sBlockName ;         
    Sheet s ;         
    Dictionary<string, Dictionary> eBlocks ;         
    Entity e ;         
    Entity e2 ;         

    SaveCode(0, "SECTION");
    SaveCode(2, "BLOCKS");

    foreach ( eBlock in drw.Blocks)
    {

        SaveCode(0, "BLOCK");
        SaveCode(5, eBlock.id);
        SaveCode(330, eBlock.idContainer);
        SaveCode(100, "AcDbEntity");
        SaveCode(8, eBlock.layer);
        SaveCode(100, "AcDbBlockBegin");
        SaveCode(2, eBlock.name);
        SaveCode(70, eBlock.Flags);
        SaveCode(codX0, eBlock.x0);
        SaveCode(cody0, eBlock.y0);
        SaveCode(codz0, eBlock.z0);
        SaveCode(3, eBlock.name);
        SaveCode(1, ""); // X ref path
        if ( (eBlock.entities.Count > 0) && (eBlock.name != "*Model_Space") )
        {

            foreach ( eEnty in eBlock.entities)
            {
                eEnty.id = Handle();
                DXFSaveCommonEntityData(eEnty);
                Gcd.CCC[eEnty.gender].SaveDxfData(eEnty);

            }

             // que pasa con el ENDBLK?
             // Al leer, lo guardo como una entidad y, por lo tanto lo tengo en el bloque, pero...corresponde?
             // no seria mejor generarlo? al fin y al cabo es solo una señal para el lector de archivos (como el SEQEND)

        }
        SaveCode(0, "ENDBLK");
        SaveCode(5, Handle());
        SaveCode(330, eBlock.idContainer);
        SaveCode(100, "AcDbEntity");
        SaveCode(8, "0");
        SaveCode(100, "AcDbBlockEnd");

    }

    SaveCode(0, "ENDSEC");

}

private void DXFSaveCommonEntityData(Entity eEnty)
    {


    string sGender ;         

    sGender = eEnty.Gender;
    if ( InStr(sGender, "DIMENSION_") > 0 ) sGender = "DIMENSION";
    SaveCode(dxf.codEntity, sGender);
    SaveCode(dxf.codid, eEnty.id);
    SaveCode(dxf.codidContainer, eEnty.Container.idContainer);
    SaveCode("100", "AcDbEntity");
    SaveCode(dxf.codLayer, eEnty.pLayer.Name);
    SaveCode(dxf.codLType, eEnty.LineType.Name);
    SaveCode(dxf.codColor, eEnty.colour);
    SaveCode(dxf.codLWht, CStr(eEnty.Liidth));

}

private int Save5EntitiesDirect(Drawing drwToSAve)
    {


    Entity eEnty ;         
    Entity eBlock ;         
    string sGender ;         
    int i ;         
    bool bCan ;         
    bool bAttribPresent ;         
    Sheet s ;         
    Dictionary<string, Dictionary> cEntities ;         

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "SECTION" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "ENTITIES" + "\n");
    foreach ( var s in drwToSAve.Sheets)
    {

         // here go all entities
        foreach ( var eEnty in s.Entities)
        {

            eEnty.id = Handle(); //FIXME: el problema es que ahora el Key en la Colection <> Id

            DXFSaveCommonEntityData(eEnty);
            Gcd.CCC[eEnty.gender].SaveDxfData(eEnty);

            if ( eEnty.Gender == "INSERT" )
            {

                bAttribPresent = false;
                if ( (eEnty.pBlock.entities.Count > 0) )
                {
                    eEnty.pBlock.idContainer = eEnty.id;

                    foreach ( eBlock in eEnty.pBlock.entities)
                    {

                        if ( eBlock.Gender == "ATTRIB" )
                        {
                            bAttribPresent = True;

                            eBlock.idContainer = Handle();
                            DXFSaveCommonEntityData(eBlock);
                            Gcd.CCC[eBlock.gender].SaveDxfData(eBlock);

                        }
                    }

                     // que pasa con el ENDBLK?
                     // Al leer, lo guardo como una entidad y, por lo tanto lo tengo en el bloque, pero...corresponde?
                     // no seria mejor generarlo? al fin y al cabo es solo una señal para el lector de archivos (como el SEQEND)
                    if ( bAttribPresent )
                    {
                        SaveCode(0, "SEQEND");
                        SaveCode(5, Handle());
                        SaveCode(330, eEnty.id);
                        SaveCode(100, "AcDbEntity");
                        SaveCode(8, "0");
                        SaveCode(100, "AcDbBlockEnd");
                    }

                }

            }
            else if ( (eEnty.Gender == "POLYLINE") || (eEnty.Gender == "POLYLINE_2D") )
            {
                if ( (eEnty.pBlock.entities.Count > 0) )
                {
                    eEnty.pBlock.id = eEnty.id;
                    eEnty.pBlock.idContainer = eEnty.id;
                    foreach ( var eBlock in eEnty.pBlock.entities)
                    {
                        eBlock.id = Handle();
                        DXFSaveCommonEntityData(eBlock);
                        Gcd.CCC[eBlock.gender].SaveDxfData(eBlock);

                    }

                }

                SaveCode(0, "SEQEND");
                SaveCode(5, Handle());
                SaveCode(330, eEnty.id);
                SaveCode(100, "AcDbEntity");
                SaveCode(8, "0");
                SaveCode(100, "AcDbBlockEnd");
            }
             // Else    // trato de exportar como vino
             //
             //     gcd.debugInfo(("No puedo guardar este tipo de entidades ") & eEnty.Gender,false,false,true)
             // End If
        }
    }

     // end section code
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");

    return;

}

private bool Save6Objects(Drawing drw)
    {


    string sValues ;         
    string lpclave ;         
    int i ;         

    Dictionary<string, Dictionary> cObject ;         
    Dictionary<string, Dictionary> cObjects ;         
    Sheet s ;         
    DictEntry de ;         

     // armo un diccionary, requerido por acad
    cObject = Dictionary<string, Dictionary>;
     // InsertCode(0, "Dictionary<string, Dictionary>", cObject)
     // InsertCode(5, drw.Dictionary<string, Dictionary>.id, cObject)
     // InsertCode(330, "0", cObject)
     // For Each de In drw.Dictionary<string, Dictionary>.Definitions
     //     InsertCode(3, de.name, cObject)
     //     InsertCode(350, de.id, cObject)
     // Next
     //
     // cObjects.Add(cObject, "Dictionary<string, Dictionary>")

    SaveCode(0, "SECTION");
    SaveCode(2, "OBJECTS");
    SaveCode(0, "Dictionary<string, Dictionary>"); // Definiciones del diccionario
    SaveCode(5, drw.Dictionary<string, Dictionary>.id);
    SaveCode(330, 0);
    SaveCode(100, "AcDbDictionary<string, Dictionary>");
    SaveCode(280, 0);
    SaveCode(281, 1);
    foreach ( de in drw.Dictionary<string, Dictionary>.Definitions)
    {
        SaveCode(3, de.name);
        SaveCode(350, de.id);
    }

    foreach ( de in drw.Dictionary<string, Dictionary>.Definitions)
    {

        SaveCode(0, "Dictionary<string, Dictionary>");
        SaveCode(5, de.id);
        SaveCode(330, drw.Dictionary<string, Dictionary>.id);
        SaveCode(100, "AcDbDictionary<string, Dictionary>");
         //SaveCode(280, 0)
        SaveCode(281, 1);
        foreach ( dl As DictList in de.items)
        {
            SaveCode(3, dl.name);
            SaveCode(350, dl.idSoftOwner);
        }

    }
     // SaveCode(2, "Dictionary<string, Dictionary>")
     // SaveCode(5, "B")
     // SaveCode(330, "A")
     // SaveCode(100, "AcDbDictionary<string, Dictionary>")
     // SaveCode(280, 0)
     // SaveCode(281, 1)

     // FIXME: reparar los papaerspaces
    objLayout.ExportDXF(drw);
     // For Each s In drw.Sheets
     //     cObject = Dictionary<string, Dictionary>
     //     objLayout.ExportDXF(s, cObject)
     //     // pequeña correccion del handle
     //     cObject["330"] = drw.Dictionary<string, Dictionary>.id
     //     cObjects.Add(cObject, s.Name)
     // Next
     // For Each cObject In cObjects
     //     SaveColection(cObject)
     // Next
     // end section code
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");

}

private int Save7ThumbNail(Image imgGLArea)
    {


    string sValues ;         
    string lpclave ;         
    int i ;         
    Dictionary<string, Dictionary> cThumbs ;         

    SaveCode(0, "SECTION");
    SaveCode(2, "THUMBNAILIMAGE");
     // cThumbs = cData["THUMBNAILIMAGE"]
     // If Not IsNull(cThumbs) Then
     //   For Each sValues In cThumbs
     //     lpclave = cThumbs.Key
     //     I = InStr(lpclave, "_")
     //     If i > 0 Then lpclave = Left(lpclave, i - 1)
     //     Print hFile, lpclave
     //     Print hFile, sValues
     //   Next
     // End If
     // // end section code
    SaveCode(0, "ENDSEC");

     // end file code
    SaveCode(0, "EOF");

}

 // Inserta un codigo en una coleccion considerando repeticiones de la Key
public void InsertCode(int iCode, Variant sData, Dictionary<string, Dictionary> cAcumulator)
    {


    string Key ;         
    int iCodeAux ;         

    Key = Str(iCode);
    if ( cAcumulator.ContainsKey(Key) )
    {
        do {
            iCodeAux += 1;
            Key = Str(iCode) + "_" + CStr(iCodeAux);

            if ( ! cAcumulator.ContainsKey(Key) ) Break;
        }
    }
    cAcumulator.Add(CStr(sData), Key);

}

 //   Helper para leer DXF: retorna la posicion en la que encontro la clave o -1 si no la encontro
 //   iCode = el codigo DXF
 //   stxClaves = array de claves DXF
 //   stxValues = array de valores DXF
 //   RetValue = el valor a retornar, pasado por referencia
 //   iStartPos = la posivion inicial en los array para la busqueda (def = 0)
 //   ExactPos = si se busca solo en la posicion inicial (def = false)
public int ReadCode(int iCode, string[] stxClaves, string[] stxValues, Variant ByRef RetValue, int iStartPos= 0, bool ExactPos= false)
    {


    int i ;         
    int iMax ;         

    if ( iStartPos < 0 ) return iStartPos;
    if ( stxClaves.max != stxValues.max )
    {
        Debug "ReadCode: error, bad lists";
        return -1;
    }
    if ( ExactPos ) iMax = iStartPos else imax = stxClaves.Max;
    for ( i = iStartPos; i <= iMax; i + 1)
    {
        if ( CInt(stxClaves[i]) == iCode )
        {
            switch ( TypeOf(RetValue))
            {
                case gb.Integer:
                    RetValue = CInt(stxValues[i]);
                    return i;
                case gb.Float:
                    RetValue = CFloat(stxValues[i]);
                    return i;
                case gb.String:
                    RetValue = stxValues[i];
                    return i;
            }

        }
    }
    return -1;

}
 //   Helper para leer DXF: retorna la posicion en la que encontro la clave, -posicion si encotro el escape o 0 si no la encontro
 //   iCode = el codigo DXF
 //   stxClaves = array de claves DXF
 //   stxValues = array de valores DXF
 //   RetValue = el valor a retornar, pasado por referencia
 //   iStartPos = la posicion inicial en los array para la busqueda (def = 0)
 //   iEscapeCode = si encuentra este codigo, sale

public int ReadCodePlus(int iExpectedCode, string[] stxClaves, string[] stxValues, Variant ByRef RetValue, int iStartPos= 0, int iEscapeCode= -1, int iStartCode= -1)
    {


    int i ;         
    int iMax ;         
    bool StartOK = True;

    if ( stxClaves.max != stxValues.max )
    {
        Debug "ReadCode: error, bad lists";
        return -1;
    }
     //If ExactPos Then iMax = iStartPos Else imax = stxClaves.Max
    if ( iStartCode >= 0 ) StartOK = false;
    if ( iStartPos < 0 ) return -1;
    for ( i = iStartPos; i <= stxClaves.max; i + 1)
    {
         // veo si proveyo un codigo inicial
        if ( iStartCode >= 0 )
        {

            if ( CInt(stxClaves[i]) == iStartCode )
            {
                StartOK = True;
            }
        }

        if ( ! StartOK ) Continue;

        if ( CInt(stxClaves[i]) == iExpectedCode )
        {
            switch ( TypeOf(RetValue))
            {
                case gb.Integer:
                    RetValue = CInt(stxValues[i]);

                case gb.Float:
                    RetValue = CFloat(stxValues[i]);
                case gb.Single:
                    RetValue = CSingle(stxValues[i]);

                case gb.String:
                    RetValue = stxValues[i];

            }

            return i + 1;
        }
        else if ( CInt(stxClaves[i]) == iEscapeCode )
        {
            switch ( TypeOf(RetValue))
            {
                case gb.Integer:
                    RetValue = 0;

                case gb.Float:
                    RetValue = 0;
                case gb.Single:
                    RetValue = 0;

                case gb.String:
                    RetValue = "";

            }

            return -i;
             // Else If iEscapeCode = -1 Then
             //     Select Case TypeOf(RetValue)
             //         Case gb.Integer
             //             RetValue = 0
             //
             //         Case gb.Float
             //             RetValue = 0
             //
             //         Case gb.String
             //             RetValue = ""
             //
             //     End Select
             //
             //     Return -i

        }
    }
    return 0;

}

 // Lee el codigo de la coleccion que se importa del DXF, puede ignorar lo que esta entre llaves {} que es info de ACAD privativa y puede empezar desde el ultimo leido antes
public int GoToCodeFromCol(Dictionary<string, Dictionary> cDxfEntityData, int iCode, string sValue)
    {


    string s ;         
    string sKey ;         
    int i ;         
    int p ;         
    bool valid ;         
    bool OpenedSection ;         

    foreach ( s in cDxfEntityData)
    {
        Inc i;

        if ( s == sValue )
        {
            LastCodeReadIndex = i;
            return i;
        }
    }
    return 0;

}

 // Lee el codigo de la coleccion que se importa del DXF, puede ignorar lo que esta entre llaves {} que es info de ACAD privativa y puede empezar desde el ultimo leido antes
public string ReadCodeFromCol(Dictionary<string, Dictionary> cDxfEntityData, int iCode, bool ReadNext= false, bool IgnoreAcadData= True, Variant vDefaultValue= "")
    {


    string s ;         
    string sKey ;         
    int i ;         
    int p ;         
    bool valid ;         
    bool OpenedSection ;         

    foreach ( s in cDxfEntityData)
    {
        Inc i;

        if ( ReadNext )
        {
            if ( i < LastCodeReadIndex )
            {
                valid = false;
            }
            else
            {
                valid = True;
            }
        }
        else
        {
            valid = True;

        }

        if ( Left(s, 1) == "{" )
        {
            OpenedSection = True;

        }

        if ( Left(s, 1) == "}" && OpenedSection )
        {
            OpenedSection = false;

        }

        if ( valid && ! OpenedSection )
        {
            sKey = cDxfEntityData.Key;
             // elimino el posible _
            p = InStr(sKey, "_");
            if ( p > 0 )
            {
                sKey = Left(sKey, p - 1);
            }
            if ( CInt(skey) == iCode )
            {
                LastCodeReadIndex = i;
                return s;
            }
        }

    }

    return CStr(vDefaultValue);

}

public void SaveCode(string sCode, string sValue)
    {


    string sToPrint ;         

    hFile.WriteLine( Format(sCode, "###0") + "\n");
     // If IsFloat(sValue) Then
     //   sToPrint = CStr(svalue)
     // Else
     //   sToPrint = svalue
     // Endif
    hFile. .Write(svalue + "\n");

}

public void SaveCodeInv(string sValue, string sCode)
    {


    SaveCode(scode, svalue);

}

private void SaveColection(Dictionary<string, Dictionary> cData)
    {


    string sValues ;         
    string lpclave ;         
    int i ;         

    foreach ( sValues in cData)
    {
        lpclave = cData.Key;
        I = InStr(lpclave, "_");
        if ( i > 0 ) lpclave = Left(lpclave, i - 1);
        SaveCode(lpclave, sValues);
    }

}

 // // Adds a  object to object by handle Dictionary<string, Dictionary>
 // Private Sub Object(drw As Drawing, o As Variant, sHandle As String)
 //
 //     If sHandle = "" Then Return
 //
 //     If drw.Handles.ContainsKey(sHandle) Then
 //         gcd.debugInfo("WARNING: Handle repedida " & sHandle)
 //     Else
 //
 //         drw.handles.Add(o, sHandle)
 //     End If
 //
 // End

 // Reads layers Dictionary<string, Dictionary> and puts data in oLayers
public void ReadViewports(Dictionary<string, Dictionary> cVptData, Drawing drw)
    {


    Viewport v ;         

     // // primero eliminamos lo q haya
    if ( ! cVptData["TABLES"].ContainsKey("VPORT") ) return;
    Drw.Viewports.Clear;
    foreach ( cViewp As Dictionary<string, Dictionary> in cVptData["TABLES"]["VPORT"])
    {
        v =  Viewport;
         // hLay.Name = cLay[dxf.codName]
         // hLay.Visible = CInt(cLay[dxf.codColor]) >= 0
         // hLay.Colour = Abs(CInt(cLay[dxf.codColor]))
         // hLay.handle = cLay[dxf.codHandle]
         // If hLay.handle = "" Then hLay.handle = gcd.Handle()()
         // Drw.oLayers.Add(hLay, hLay.handle)
    }

     // // es inaceptable no tener al menos un layrr
     // If drw.oLayers.Count = 0 Then
     //     hLay =  Layer
     //     hLay.Name = "0"
     //     hLay.Visible = True
     //     hLay.Colour = 0
     //     hLay.handle = gcd.Handle()()
     //     Drw.oLayers.Add(hLay, hLay.handle)
     // Endif
     //
     // // aprovecho para setear el layer actual
     // Drw.CurrLayer = Drw.oLayers[Drw.oLayers.First]

}

 // Reads layers Dictionary<string, Dictionary> and puts data in oLayers
public void ReadLayers(Dictionary<string, Dictionary> cLaydata, Drawing drw)
    {


    Layer hLay ;         

     // // primero eliminamos lo q haya
    Drw.Layers.Clear;
    foreach ( cLay As Dictionary<string, Dictionary> in cLayData["TABLES"]["LAYER"])
    {
        hLay =  Layer;
        hLay.Name = cLay[dxf.codName];
        hLay.id = cLay[dxf.codid];
        hLay.Visible = CInt(cLay[dxf.codColor]) >= 0;
        hLay.Colour = Abs(CInt(cLay[dxf.codColor]));
        hLay.LineType = drw.LineTypes[cLay[Me.codLType]];

        Try hLay.Lit = cLay["370"]; // algunos dxf no traen esta info
        if ( hLay.Lit == 0 ) hLay.Lit = 1;

        if ( hLay.id == "" ) hLay.id = gcd.Id();
        Drw.Layers.Add(hLay.Name, hLay);
    }

     // es inaceptable no tener al menos un layrr
    if ( drw.Layers.Count == 0 )
    {
        hLay =  Layer;
        hLay.Name = "0";
        hLay.Visible = True;
        hLay.Colour = 0;
        hLay.LineType = drw.LineTypes[drw.LineTypes.First];
        hLay.id = gcd.Id();
        Drw.Layers.Add(hLay.Name, hLay);
    }

     // aprovecho para setear el layer actual
    Drw.CurrLayer = Drw.Layers[Drw.Layers.First];

     // o mejor este, pero puede fallar
    Try drw.CurrLayer = drw.Layers[drw.Headers.CLAYER];

}

 // Reads Styles and DimStyles Dictionary<string, Dictionary> and puts data in arrStyles
public void ReadStyles(Dictionary<string, Dictionary> cData, Drawing drw)
    {


    TextStyle hlty ;         
    int t ;         
    int i ;         
    double fTrameLength ;         
    string sH2 ;         
    string sNextKey ;         
    TextStyle RefStyle ;         
    DimStyle hdim ;         
    string n ;         

     // primero eliminamos lo q haya
    Drw.TextStyles.Clear;
     // Leo los styles de texto
    if ( cData["TABLES"].ContainsKey("STYLE") )
    {
        foreach ( c As Dictionary<string, Dictionary> in cData["TABLES"]["STYLE"])
        {
            hlty =  TextStyle;

            hlty.Name = Lower(c[dxf.codName]);
            hlty.Id = c[dxf.codid];
            if ( hLty.id == "" ) hLty.id = gcd.Id();
            hlty.sFont_3 = Lower(c["3"]);

            hlty.FixedH_40 = CFloat(c["40"]);

             // Esto no puede usarse asi, LastHeightUsed_2 es solo un dato de historial
             // If hlty.FixedH_40 = 0 Then hlty.FixedH_40 = CFloat(c["42"])

            if ( hlty.name != "" )
            {
                n = Lower(hlty.name);
            }
            else if ( hlty.sFont_3 != "" )
            {
                n = Utils.FileWithoutExtension(hlty.sFont_3);
            }
            else
            {
                Continue;
            }

            Drw.TextStyles.Add(n, hlty);

        }

        drw.CurrTextStyle = Drw.TextStyles[Drw.TextStyles.First];
    }

     // Leo lo styles de dimensiones
    if ( cData["TABLES"].ContainsKey("DIMSTYLE") )
    {
        foreach ( c As Dictionary<string, Dictionary> in cData["TABLES"]["DIMSTYLE"])
        {
            hdim =  DimStyle;

            hdim.Name = c[dxf.codName];

            if ( c.ContainsKey("105") )
            {
                hdim.id = c["105"];
            }
            else
            {
                hdim.id = gcd.Id();
            } // depre

            Try hdim.DIMSCALE = CFloat(c["40"]);
            if ( hdim.DIMSCALE == 0 ) hdim.DIMSCALE = 1;

            Try hdim.DIMASZ = CFloat(c["41"]);
            if ( hdim.DIMASZ == 0 ) hdim.DIMASZ = 1;

            Try hdim.DIMTXT = CFloat(c["140"]);
            if ( hdim.DIMTXT == 0 ) hdim.DIMTXT = 1;

            Try hdim.DIMTXSTY = c["340"];

            Try hdim.DIMBLK = cData["TABLES"]["BLOCK_RECORD"][c["341"]]["2"];
            if ( hdim.DIMBLK == "" ) hdim.DIMBLK = "_" + gcd.Drawing.Headers.DIMBLK;
            Try hdim.DIMBLK1 = cData["TABLES"]["BLOCK_RECORD"][c["343"]]["2"];
            if ( hdim.DIMBLK1 == "" ) hdim.DIMBLK1 = "_" + gcd.Drawing.Headers.DIMBLK1;
            Try hdim.DIMBLK2 = cData["TABLES"]["BLOCK_RECORD"][c["344"]]["2"];
            if ( hdim.DIMBLK2 == "" ) hdim.DIMBLK2 = "_" + gcd.Drawing.Headers.DIMBLK2;

            if ( hdim.DIMTXSTY != "" )
            {
                RefStyle = gcd.FindStyleByid(hdim.DIMTXSTY);
                if ( ! IsNull(RefStyle) )
                {
                    if ( RefStyle.FixedH_40 > 0 ) hdim.DIMTXT = RefStyle.FixedH_40;
                    hdim.DIMTXSTY = RefStyle.sFont_3;
                }
            }

            if ( hdim.name != "" ) Drw.DimStyles.Add(hdim.name, hdim);

        }

        drw.CurrDimStyle = Drw.DimStyles[Drw.DimStyles.First];
    }

}

 // Reads LineTypes Dictionary<string, Dictionary> and puts data in arrLTypes
public void ReadLTypes(Dictionary<string, Dictionary> cData, Drawing drw)
    {


    LineType hlty ;         
    int t ;         
    int i ;         
    int ri ;         
    double fTrameLength ;         
    string sNextKey ;         
    string r ;         
    bool AbsoluteRotation ;         
    bool IsText ;         
    bool IsShape ;         

     // primero eliminamos lo q haya
    Drw.LineTypes.Clear;
    foreach ( c As Dictionary<string, Dictionary> in cData["TABLES"]["LTYPE"])
    {
        hlty =  LineType;
        hlty.Name = UCase(c[dxf.codName]);
        hlty.Description = c["3"];
        if ( c.ContainsKey("5") ) hlty.id = c["5"];
        if ( hLty.id == "" ) hLty.id = gcd.Id();
        Try hlty.nTrames = CInt(c["73"]);
        if ( hlty.nTrames > 0 ) hlty.Length = CFloat(dxf.ReadCodeFromCol(c, 40));
        i = 0;
        for ( t = 1; t <= hlty.nTrames; t + 1)
        {

            r = dxf.ReadCodeFromCol(c, 49, True);
            hlty.TrameLength.Add(CFloat(r));
            ri = CInt(dxf.ReadCodeFromCol(c, 74, True,, 0));
            hlty.TrameType.Add(ri);
            switch ( ri)
            {
                case 0:
                     // nada
                default:
                    if ( (ri && 1) == 1 ) AbsoluteRotation = True else absoluterotation = false;
                    if ( (ri && 2) == 2 ) IsText = True else IsText = false;
                    if ( (ri && 4) == 4 ) IsShape = True else IsShape = false;
                    hlty.TrameData.Add(dxf.ReadCodeFromCol(c, 75, True));
                    hlty.TrameStyle.Add(dxf.ReadCodeFromCol(c, 340, True));
                    hlty.TrameScale.Add(dxf.ReadCodeFromCol(c, 46, True));
                    hlty.TrameRotation.Add(dxf.ReadCodeFromCol(c, 50, True));
                    hlty.TrameOffX.Add(dxf.ReadCodeFromCol(c, 44, True));
                    hlty.TrameStyle.Add(dxf.ReadCodeFromCol(c, 45, True));
                    hlty.TrameData.Add(dxf.ReadCodeFromCol(c, 9, True));

            }

        }

        Drw.LineTypes.Add(hlty, hlty.Name);

    }
    if ( DRW.LineTypes.Count == 0 )
    {
        hlty =  LineType;
        hlty.Name = "CONTINUOUS";
        hlty.Description = "";
        hlty.id = gcd.id();
        hlty.nTrames = 0;
        drw.LineTypes.Add(hlty, hlty.Name);

    }

    Drw.CurrLineType = Drw.LineTypes[Drw.LineTypes.First];

}

public void ImportBlocksFromDXF(Dictionary<string, Dictionary> colData, Drawing drw) //, obxEntities As Entity[]) As Integer
    {


    int iTotalEntities ;         
    Dictionary<string, Dictionary> colent ;         
    Dictionary<string, Dictionary> colBlk ;         
     Float[] flxPoints ;         
    double[] P ;         
    string hBlock ;         
    Dictionary<string, Dictionary> cParent ;         
    Dictionary<string, Dictionary> cEntyList ;         
    Variant hEnty ;         
    int iEnty ;         
    Variant[] cEnty ;         
    int i ;         
     Sheet S ;         
    Block Block ;         
    bool hasBlocks ;         
    bool hasTables ;         
    bool hasBlockRecord ;         
    bool hasEntities ;         
    Dictionary<string, Dictionary> cBlocks ;         
    Dictionary<string, Dictionary> cTables ;         
    Dictionary<string, Dictionary> cBlockRecord ;         
    Dictionary<string, Dictionary> cEntities ;         
     Block b ;         

    if ( colData.ContainsKey("BLOCKS") ) cBlocks = colData["BLOCKS"];
    if ( colData.ContainsKey("TABLES") )
    {
        cTables = colData["TABLES"];
        if ( colData["TABLES"].ContainsKey("BLOCK_RECORD") ) cBlockRecord = cTables["BLOCK_RECORD"];
    }
    if ( ! cBlocks ) return;
     // For Each colBlk In colData["TABLES"]["BLOCK_RECORD"]
     //     Dim Block As  Block
     //     Block.entities = Dictionary<string, Dictionary>
     //     Block.name = colBlk[dxf.codName]
     //     Block.handle = colBlk[dxf.codHandle]
     //     Block.HandleOwnerParent = colBlk[dxf.codHandleOwner]
     //     Block.HandleAsociatedLayout = colBlk["340"]
     //     Try Block.InsertUnits = colBlk["70"]
     //     Try Block.Explotability = colBlk["280"]
     //     Try Block.Scalability = colBlk["281"]
     //     drw.oBlocks.Add(Block, Block.handle)
     //
     // Next
     // hay DXF sin TAbles
    if ( cTables )
    {

        foreach ( colBlk in cBlocks)
        {
            Block =  Block;
            Block.id = colBlk[dxf.codid];
            Block.entities = Dictionary<string, Dictionary>;
            Block.name = colBlk[dxf.codName];
            Block.layer = colBlk[dxf.codLayer];
            Try Block.x0 = colBlk[dxf.codX0];
            Try Block.y0 = colBlk[dxf.codY0];
            Try Block.z0 = colBlk[dxf.codZ0];
            Try Block.flags = colBlk["70"];

            if ( cBlockRecord )
            {
                if ( cBlockRecord.ContainsKey(colBlk["330"]) )
                {
                    Block.idContainer = cBlockRecord[colBlk["330"]]["5"];
                    Block.idAsociatedLayout = cBlockRecord[colBlk["330"]]["340"];
                     // If Block.idAsociatedLayout <> "0" Then
                     //     hContainers.Add(Block, Block.idAsociatedLayout)
                     // Else
                    hContainers.Add(Block, Block.idContainer);
                     // End If
                }
            }
             //If Left(Block.name, 1) = "*" Then    // puede ser una Dim o una Sheet
             // If InStr(Block.name, "_Space") > 0 Then
             //     // es una sheet, que ya fue creada por ReadObjectsFromDXF
             // Else

            Drw.Blocks.Add(Block, Block.name);

             //Endif

             //Endif

        }
    }
    if ( (Drw.Blocks.Count == 0) || ! Drw.Blocks.ContainsKey("*Model_Space") ) // agrego el Model
    {

         // lo agrego a los bloques

        b.name = "*Model_Space";
        b.entities = Dictionary<string, Dictionary>;
        b.idContainer = gcd.Id();
        b.id = gcd.Id();
        b.idAsociatedLayout = gcd.Id();
        b.IsAuxiliar = True;
        b.IsReciclable = false;
        Drw.Blocks.Add(b, b.name);
    }

     // voy a hacer un chequeo final, porque algunos DXF vienen sin el bloque Model
    if ( ! Drw.Sheets.ContainsKey("Model") )
    {
         // creo la Sheet
        S =  Sheet;
        s.Name = "Model";
        s.IsModel = True;
        s.Block = Drw.Blocks["*Model_Space"];
        s.id = Drw.Blocks["*Model_Space"].idAsociatedLayout;
        Drw.Sheets.Add(s, "Model");
        drw.Sheet = s;
        drw.Model = s;

    }

     // // ahora vinculo las Sheets con su bloque
    foreach ( s in Drw.Sheets)
    {
        foreach ( b in drw.Blocks)
        {
            if ( b.idAsociatedLayout == s.id )
            {
                s.Block = b;
                s.Entities = b.entities;
                b.Sheet = s;
                Break;
            }
        }
    }

    foreach ( colBlk in cBlocks)
    {

        if ( colBlk.ContainsKey("entities") )
        {

            DXFtoEntity(colBlk["entities"], drw, drw.Blocks[colBlk["2"]]);

        }
        Inc i;

    }

}

public void SetViewports(Dictionary<string, Dictionary> cDxfData, Drawing drw)
    {


    Entity e ;         
    Viewport v ;         
    string n ;         
    string hLayout ;         
    Sheet s ;         
    Block b ;         

    foreach ( b in drw.Blocks)
    {
        if ( drw.Sheets.ContainsKey(b.idAsociatedLayout) )
        {
            foreach ( e in b.entities)
            {
                drw.Sheets[b.idAsociatedLayout].entities.Add(e, e.id);
                 // Try n = cDxfData["BLOCKS"][b.name]["2"]                      // nombre del bloque
                 // Try hLayout = cDxfData["TABLES"]["BLOCK_RECORD"][n]["340"]
                 // If hLayout <> "" Then
                 //     s.Entities = b.entities
                 //
                 //
            }
        }

    }
    foreach ( s in drw.Sheets)
    {
        foreach ( e in s.entities)
        {
            if ( e.Gender == cadViewport.Gender )
            {
                s.Viewports.Add(e.pBlock, e.id);
                 //cadViewport.SetViewport(e, s)
            }
        }
         //gcd.Drawing.Sheet = s
         //cadZoomE.Start()
    }

}

 // Importa las cosas de manera descentralizada
public void DXFtoEntity(Dictionary<string, Dictionary> cDxfEntities, Drawing drw, Block bContainer)
    {


    Dictionary<string, Dictionary> e ;         
    Dictionary<string, Dictionary> obx ;         
    Dictionary<string, Dictionary> cLastParent ;         
    Entity entNueva ;         
    bool flgIsPolyline ;         
    bool IsDummy ;         
    bool EntitiesToModel ;         
    Block pBlockPolyline ;         
    Block b ;         
    string sid ;         
    string sName ;         
    string sIdLayout ;         

    double fTime ;         
    Date t ;         
    Variant hContainer ;         

    foreach ( e in cDxfEntities) // Para cada Coleccion de datos de vrx
    {
        if ( e.ContainsKey(dxf.codEntity) ) // es una entidad?
        {
             // entonces, creamos una nueva
             // poner en minuscula para anular la entidad
            if ( InStr("VIEWPORT LEADER HATCH POLYLINE ENDBLK SEQEND VERTEX POINT ATTDEF ATTRIB LINE LWPOLYLINE CIRCLE ELLIPSE ARC TEXT MTEXT SPLINE SOLID INSERT DIMENSION DIMENSION_LINEAR DIMENSION_DIAMETEr DIMENSION_RADIUs DIMENSION_ANG3Pt DIMENSION_ALIGNED DIMENSION_ORDINATE LARGE_RADIAL_DIMENSION ARC_DIMENSION MLINE", UCase(e[dxf.codEntity])) == 0 ) IsDummy = True else IsDummy = false;

            if ( UCase(e[dxf.codEntity]) == "ENDBLK" ) Continue;

            if ( IsDummy )
            {
                 // no esta implementada
                gcd.debuginfo("Entidad no implementada o con errores: " + e[dxf.codId] + "," + e[dxf.codEntity]);

            }
            else
            {

                t = Timer;

                entNueva = clsEntities.DXFImportToEntity(drw, e, IsDummy);

                if ( IsNull(entNueva) ) Continue; // si esta implementada, llenamos los datos
                if ( entNueva.Gender == cadEndBlk.Gender ) Continue;

                 //stats
                if ( ! ReadTimes.ContainsKey(entNueva.Gender) ) ReadTimes.Add(fTime, entNueva.Gender);
                if ( ! ReadEntities.ContainsKey(entNueva.Gender) ) ReadEntities.Add(1, entNueva.Gender);

                 //Debug "Contenedor", hContainer
                 // -hBlock-Record
                 // -hInsert
                 // -hHatch               OTRA ENTIDAD que tenga .pBlock.enities as Dictionary<string, Dictionary>
                 // -Polyline

                if ( bContainer ) entNueva.Container = bContainer;
                if ( entNueva.Container )
                {
                    obx = entNueva.Container.entities;
                }
                else if ( bContainer )
                {
                    obx = bContainer.entities; //drw.Blocks[entNueva.IdContainer]
                     // Else If hContainers.ContainsKey(entNueva.idContainer) Then
                     //     obx = hContainers[entNueva.idContainer].entities
                }
                else
                {
                     //If Not obx Then
                    obx = drw.Sheet.Entities;
                    entNueva.Container = drw.Sheet.Block;
                }

                switch ( entNueva.Gender)
                {
                    case "HATCH", "INSERT", "POLYLINE", "POLYLINE_2D":
                        if ( entNueva.pBlock )
                        {
                            hContainers.Add(entNueva.pBlock, entNueva.Id);
                        }
                        if ( entNueva.Gender == "POLYLINE" )
                        {
                            if ( (entNueva.iParam[cadPolyline.iiiPolylineType] && 64) == 64 )
                            {
                                drw.Has3dEntities = True;
                            }
                        }
                }
                 //End If

                if ( e[dxf.codEntity] != "SEQEND" ) obx.Add(entNueva, entNueva.Id);

                 // //gcd.debugInfo("Leida entidad tipo" & entNueva.Gender & " id " & entNueva.id,false,false,true)
                 // If e[dxf.codEntity] = "POLYLINE" Then //Stop
                 //     flgIsPolyline = True
                 //     //obx = entNueva.pBlock
                 //     // pBlockPolyline =  Block
                 //     // pBlockPolyline.entities = Dictionary<string, Dictionary>
                 //     //
                 //     // entNueva.pBlock = pBlockPolyline
                 // End If
                 // If (e[dxf.codEntity] = "SEQEND") And (flgIsPolyline = True) Then
                 //     flgIsPolyline = false
                 //     obx.Remove(obx.Last)
                 //     obx = Null
                 //
                 // End If

                fTime = (Timer - t);
                ReadTimes[entNueva.Gender] += fTime;

                ReadEntities[entNueva.Gender] = ReadEntities[entNueva.Gender] + 1;

                 // If entNueva.Gender = "HATCH" Then
                 //     ReadTimes.Add(fTime, entNueva.Handle)
                 // Endif
                 // If entNueva.HandleOwner = "640" Then Debug "640", entNueva.Gender

                entNueva = Null; // limpiamos

            }
        }
    }

}

 // Transforma una coleccion en dos array de strings
public void DigestColeccion(Dictionary<string, Dictionary> c, string[] ByRef sClaves, string[] ByRef sValues)
    {


    string lpValue ;         
    string lpclave ;         
    int i ;         

    sClaves.Clear;
    sValues.Clear;

    foreach ( lpValue in c)
    {
        lpclave = c.Key;
        I = InStr(lpclave, "_");
        if ( i > 0 ) lpclave = Left(lpclave, i - 1);
        sClaves.Add(lpclave); // el codigo es el tipo de variable
        sValues.Add(lpValue);

    }

}

public void ReadObjectsFromDXF(Dictionary<string, Dictionary><string, Dictionary<string, Dictionary>> cData, Drawing drw)
    {


    Dictionary<string, Dictionary> cObject ;         
    Entity entNueva ;         
     String[] sClaves ;         
     String[] sValues ;         
    Sheet s ;         
    MLineStyle m ;         
    DictEntry entry ;         
    int i ;         
    int flags ;         
    int i2 ;         

    gcd.debugInfo("Importing DXF object data",false,false,true);
     //Handle_Layout = Dictionary<string, Dictionary>
    if ( ! cData.ContainsKey("OBJECTS") ) return;
    foreach ( cObject in cData["OBJECTS"])
    {

        if ( cObject["0"] == "Dictionary<string, Dictionary>" )
        {

            this.DigestColeccion(cObject, ByRef sClaves, ByRef sValues);
            entry =  DictEntry;
             //entry.idSoftOwner = dxf.ReadCodeFromCol(cObject, 330, True)
            entry.name = dxf.ReadCodeFromCol(cObject, 3, True); // name
            entry.idSoftOwner = dxf.ReadCodeFromCol(cObject, 330, True);

        }

        if ( cObject["0"] == "LAYOUT" )
        {

            this.DigestColeccion(cObject, ByRef sClaves, ByRef sValues);
            s =  Sheet;
            objLayout.importDXF(s, cObject);
            if ( s.name == "" ) s.name = s.pPrintStyle.ViewName;
            if ( S.Name == "" ) s.name = "Sheet " + Str(drw.Sheets.Count);
            drw.Sheets.Add(s, s.name);
             //Handle_Layout.Add(s.Name, s.id)
             //hContainers.Add(s.Entities, s.id)
            if ( LCase(s.Name) == "model" )
            {
                s.IsModel = True;
                drw.Sheet = s;
                drw.Model = s;

            }

        }

        if ( cObject["0"] == "MLINESTYLE" )
        {

            this.DigestColeccion(cObject, ByRef sClaves, ByRef sValues);
            m =  MLineStyle;
            i = ReadCodePlus(2, sClaves, sValues, ByRef m.Name, 0);
            i = ReadCodePlus(70, sClaves, sValues, ByRef flags, i);
            m.FillOn = BTst(flags, 1);
            m.ShowMiters = BTst(flags, 2);
            m.StartSquareCap = BTst(flags, 5);
            m.StartInnerArc = BTst(flags, 6);
            m.StartRound = BTst(flags, 7);
            m.EndSquareCap = BTst(flags, 9);
            m.EndInnerArc = BTst(flags, 10);
            m.EndRound = BTst(flags, 11);

            i = ReadCodePlus(3, sClaves, sValues, ByRef m.Description, i);

            i = ReadCodePlus(62, sClaves, sValues, ByRef m.FillColor, i);

            i = ReadCodePlus(51, sClaves, sValues, ByRef m.StartAngle, i);

            i = ReadCodePlus(52, sClaves, sValues, ByRef m.EndAngle, i);
            i = ReadCodePlus(71, sClaves, sValues, ByRef m.Elements, i);

            m.ElemOffset.Resize(m.Elements);
            m.ElemColor.Resize(m.Elements);
            m.ElemLinetype.Resize(m.Elements);

            for ( i2 = 1; i2 <= m.Elements; i2 + 1)
            {
                i = ReadCodePlus(49, sClaves, sValues, ByRef m.ElemOffset[i2 - 1], i);
                i = ReadCodePlus(62, sClaves, sValues, ByRef m.ElemColor[i2 - 1], i);
                i = ReadCodePlus(6, sClaves, sValues, ByRef m.ElemLinetype[i2 - 1], i);
                if ( (m.ElemOffset[i2 - 1] > 0) && (m.JustificationTop < m.ElemOffset[i2 - 1]) ) m.JustificationTop = m.ElemOffset[i2 - 1];
                if ( (m.ElemOffset[i2 - 1] < 0) && (m.JustificationBottom > m.ElemOffset[i2 - 1]) ) m.JustificationBottom = m.ElemOffset[i2 - 1];
            }
            drw.MLineStyles.Add(m, m.name);

        }

    }

}

 // busca un bloque con ese block record
private  Block GetBlock(Dictionary<string, Dictionary> cBlocks, string hBlockRecord)
    {


            

    foreach ( var b in cBlocks)
    {
        if ( b.IdContainer == hBlockRecord ) return b;

    }

    return Null;

}

 // Devuelve una nueva handle que acumula en la var publica hexHandle
private string Handle( int iStart  = -1) 
    {
        if ( iStart < 0 )
        {
             iHandle++;

        }
        else
        {
        iHandle = iStart;

        }
    return Hex(iHandle);

    }

public bool SetValues(string sVarName, Variant[] vValue)
    {


     float[] slx =[];         
     int[] inx =[];         
    int Tipo ;         
    Variant v ;         
    string sVarName2 ;         

    if ( IsNumber(Left(sVarName, 1)) ) sVarName2 = "__" + sVarName else sVarName2 = sVarName;

    if ( vValue.Count > 1 )
    {
         // determino el tipo de array
        tipo = Object.GetProperty(Me, "_" + sVarName);
        if ( (tipo > 9) && (tipo < 70) )
        {
            foreach ( var v in vValue)
            {
                slx.Add(v);
            }
            Object.SetProperty(Me, sVarName2, slx);
        }
        else
        {
            foreach (var  v in vValue)
            {
                inx.Add(v);
            }
            Object.SetProperty(Me, sVarName2, inx);
        }

    }
    else
    {

        Object.SetProperty(Me, sVarName2, vValue[0]);

    }
    return True;
Catch;
    return false;

}

//  // Devuelve una string apta DXF
// public string[] ExportDXF()
//     {


//      String[] stx ;         
//     int i ;         
//     int tipo ;         
//     int iVal ;         
//     Object  obj = Me;
//     Class  MyClass = Object.Class(obj);
//     string Var ;         

//      Single[] slx ;         
//      Integer[] inx ;         

//     float sl ;         

//     foreach ( var in myClass.Symbols)
//     {
//         if ( InStr(var, "_") > 0 ) Continue;
//         stx.Add("9");
//         stx.Add("$" + var);

//          // Verifying that it is a property or a variable.
//         if ( (MyClass[var].kind == Class.Variable) || (MyClass[var].kind == Class.Property) )
//         {
//             tipo = Object.GetProperty(obj, "_" + var);
//             if ( MyClass[var].Type == "Single[]" || MyClass[var].Type == "Integer[]" ) //is an array
//             {

//                 if ( tipo == 10 ) // es un array
//                 {
//                     slx = Object.GetProperty(Me, var);
//                     stx.Add("10");
//                     if ( slx.Count >= 1 ) stx.Add(slx[0]) else stx.Add(0);
//                     stx.Add("20");
//                     if ( slx.Count >= 2 ) stx.Add(slx[1]) else stx.Add(0);
//                     if ( slx.Count == 3 )
//                     {
//                         stx.Add("30");
//                         if ( slx.Count >= 3 ) stx.Add(slx[2]) else stx.Add(0);
//                     }
//                 }
//                 else if ( tipo == 70 )
//                 {
//                     inx = Object.GetProperty(Me, var);
//                     foreach (var iVal in inx)
//                     {
//                         stx.Add("70");
//                         stx.Add(i);
//                     }

//                 }
//                 else if ( tipo == 40 )
//                 {
//                     slx = Object.GetProperty(Me, var);
//                     foreach ( var sl in slx)
//                     {
//                         stx.Add("40");
//                         stx.Add(sl);
//                     }

//                 }
//             }
//             else
//             {
//                 stx.Add(CStr(tipo));
//                 stx.Add(Object.GetProperty(Me, var));
//             }
//         }

//     }

//     return stx;

// }

}