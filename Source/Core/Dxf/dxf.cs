using Gaucho;
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

 //       DICTIONARY
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
private File fp ;         
private File hFile ;         

private string lpCode ;         
private string lpValue ;         

private int LastCodeReadIndex = 0;
private bool eExports ;         

public Dictionary hContainers ;         
private Dictionary ReadTimes ;         
private Dictionary ReadEntities ;         
public Collection cEntitiesUnread ;         
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
    if ( Exist(tmpfile) ) Kill tmpfile;
     // convierte DWG a DXF version 2010
    Shell "/usr/local/bin/dwgread; //" & sDwgFile & "// -O DXF -a r2010 -o //" & tmpfile & "//" Wait To str
    Gcd.debuginfo("Resultados de la conversion DWG a DXF " + Str);
    Wait;
    return tmpfile;

}

 // Carga el DXF y lo mete en cModel del dibujo actual
 // Verbose=0 nada, 1=minimo, 2=grupos, 3=todo
public bool LoadFile(string sFile, Drawing drw, bool IgnoreTables= False, bool IgnoreBlocks= False, bool IgnoreHeader= False, int VerboseLevel= 0, bool UpdateGraphics= True, bool ReadObjects= True)
    {


    double t = Timer;
    Collection cLlaveActual ;         
    Collection cSectionActual ;         
    Collection cTable ;         
    Dictionary cToFill ;         

    fp = Open sFile; // For Read

    if ( ! fp ) Error.Raise("Error !");

    LoadedBytes = 0;
    LoadTotalBytes = Lof(fp);

    cEntitiesUnread = Dictionary;
    nEntitiesUnread = 0;
    nEntitiesRead = 0;
    hContainers = Dictionary; // Clave = Handle , Dato = Colection

    While Not Eof(fp);
         //Wait 0.0001
        ReadData;
        if ( lpCode == "0" && lpValue == "SECTION" )
        {

             // vemos que seccion es
            ReadData;
            if ( lpCode == "2" && lpValue == "HEADER" && ! IgnoreHeader )
            {
                 // creo la llave, pero solo si es necesario
                if ( ! cToFill.Exist("HEADER") )
                {
                    cLlaveActual = Dictionary;
                    cToFill.Add(cLlaveActual, "HEADER");
                }
                else
                {
                    cLlaveActual = cToFill["HEADER"];
                }

                Load1HeadersDirect(drw.Headers);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Leidos Headers",,, True);
                Wait;

            }

            if ( lpCode == "2" && lpValue == "CLASSES" )
            {

                Load2Classes(drw);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Leidas Classes",,, True);
                Wait;
            }

            if ( lpCode == "2" && lpValue == "TABLES" && ! IgnoreTables )
            {
                if ( ! cToFill.Exist("TABLES") )
                {
                    cLlaveActual = Dictionary;
                    cToFill.Add(cLlaveActual, "TABLES");
                }
                else
                {
                    cLlaveActual = cToFill["TABLES"];
                }
                Load3Tables(cLlaveActual);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Leidos Tables",,, True);
                Wait;
                 // con las tablas cargadas, llenamoslas colecciones de objetos
                ReadViewports(cToFill, drw);
                ReadLTypes(cToFill, drw);
                ReadStyles(cToFill, drw);
                ReadLayers(cToFill, drw);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Tables al Drawing",,, True);
                Wait;
            }

             //
            if ( lpCode == "2" && lpValue == "BLOCKS" && ! IgnoreBlocks )
            {
                 // creo la llave
                cLlaveActual = Dictionary;
                cToFill.Add(cLlaveActual, "BLOCKS");
                Load4Blocks(cLlaveActual);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Leidos Blocks",,, True);
                Wait;
            }

            if ( lpCode == "2" && lpValue == "ENTITIES" )
            {
                 // creo la llave
                cLlaveActual = Dictionary;
                cToFill.Add(cLlaveActual, "ENTITIES");

                Load5Entities(cLlaveActual);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Leidas Entidades",,, True);
                Wait;
            }
             //
            if ( lpCode == "2" && lpValue == "OBJECTS" )
            {

                if ( ! cToFill.Exist("OBJECTS") )
                {
                    cLlaveActual = Dictionary;
                    cToFill.Add(cLlaveActual, "OBJECTS");
                }
                else
                {
                    cLlaveActual = cToFill["OBJECTS"];
                }

                Load6Objects(cLlaveActual);
                if ( VerboseLevel > 2 ) gcd.debugInfo("Leidos Objetos",,, True);
                Wait;
            }

            if ( lpCode == "2" && lpValue == "THUMBNAILIMAGE" )
            {

                if ( ! cToFill.Exist("THUMBNAILIMAGE") )
                {
                    cLlaveActual = Dictionary;
                    cToFill.Add(cLlaveActual, "THUMBNAILIMAGE");
                }
                else
                {
                    cLlaveActual = cToFill["THUMBNAILIMAGE"];
                }

                Load7Thumbnail(cLlaveActual);
                Wait;
            }

        }
    Wend;
    gcd.debugInfo("DXF a Collection",,, True, True);

    if ( ReadObjects ) ReadObjectsFromDXF(cToFill, drw);
    if ( UpdateGraphics )
    {
        ImportBlocksFromDXF(cToFill, drw);
        Wait;
         //depre clsEntities.BuildPoi()
        DXFtoEntity(cToFill["ENTITIES"], drw);
        Wait;
        gcd.debugInfo("Drawing generated",,, True, True);
         //clsEntities.DeExtrude(drw)
        clsEntities.BuildGeometry();
        Wait;
         //gcd.DigestInserts()
        SetViewports(cToFill, drw);
        gcd.debugInfo("Geometry generated",,, True, True);
        Wait;
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
    return False;

}

private void DiscardBlocks(Drawing drw)
    {


    Block b ;         

    foreach ( b in drw.Blocks)
    {
        if ( (Left(b.name) == "*") && (b.idAsociatedLayout != "0") ) drw.Blocks.Remove(drw.Blocks.Key);
    }

}

private void ReadData()
    {


    Line Input #fp, lpcode;
    Line Input #fp, lpValue;

    LoadedBytes += Len(lpcode);
    LoadedBytes += Len(lpvalue);

    if ( Right(lpcode, 1) == gb.Cr ) lpcode = Left(lpcode, -1);
    if ( Right(lpvalue, 1) == gb.cr ) lpvalue = Left(lpvalue, -1);

    lpcode = Trim$(lpcode);
    lpvalue = Trim$(lpvalue);

     // updating percentage

    LoadingPercent = LoadedBytes / LoadTotalBytes;

    if ( LoadingPercent - LoadLastPercent > 0.01 )
    {
        gcd.debugInfo("Loging file " + CInt(LoadingPercent * 100) + "%", True, True);
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
    Variant[] cVariable ;         
    Variant v ;         
    New Single[] slx ;         
    New Integer[] inx ;         
    int i ;         

    ReadData;
    do {

        if ( lpCode == "0" && lpValue == "ENDSEC" ) Break;

        if ( lpcode == "9" ) // nueva variable
        {
            cVariable = new Variant[];
            sVarName = Mid(lpvalue, 2);

            do { // este bucle es por si la variable es un array
                ReadData;
                if ( lpcode == "0" || lpCode == "9" ) Break;
                cVariable.Add(lpvalue);
            }
            if ( ! SetValues(sVarName, cVariable) ) gcd.debugInfo("Var " + sVarName + " not found.");
            Inc i;

        }

    }

    gcd.debuginfo("DXF: Leidas " + i + " variables de ambiente");

}

private void Load2Classes(Drawing drwLoading)
    {


    CadClass cClass ;         

    do {

        if ( lpValue == "CLASSES" ) ReadData;
        if ( lpValue == "ENDSEC" ) return;

        cClass = new CadClass;
        drwLoading.CadClasses.Add(cClass);

        ReadData;

        While (lpcode <> "0") And Not Eof(fp);
            if ( lpcode == "0" ) cClass.recordtype = lpValue;
            if ( lpcode == "1" ) cClass.recordname = lpValue;
            if ( lpcode == "2" ) cClass.CPPName = lpValue;
            if ( lpcode == "3" ) cClass.AppName = lpValue;
            if ( lpcode == "90" ) cClass.ProxyCapp = CInt(lpValue);
            if ( lpcode == "91" ) cClass.InstanceCount = CInt(lpValue);
            if ( lpcode == "280" ) cClass.ProxyFlag = CInt(lpValue);
            if ( lpcode == "281" ) cClass.EntityFlag = CInt(lpValue);

            ReadData;

        Wend;

    }

}

private void Load3Tables(Collection cTables)
    {


    string sTableName ;         
    string sTableid ;          // in hex
    string sTableContainer ;          // in hex , 0 = nobody
    int iTableEntries ;         
    Collection cTable ;         

     // creamos una table inicial con los handles de las tables
    cTable = Dictionary;
    cTables.Add(cTable, "__AuxData__");

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
            While lpcode <> "0";

                if ( lpcode == "5" ) sTableid = lpvalue;

                if ( lpcode == "2" ) sTableName = lpvalue;
                if ( lpcode == "330" ) sTableContainer = lpvalue;

                 //If sTableName = "VIEW" Then Stop

                 // WARNING: este dato no es valido para todas las versiones de DXF
                 // en algunos archivos hay mas tablas que lo que indica este numero
                 // No hay que darle importancia a este numero!!!
                if ( lpcode == "70" ) iTableEntries = CInt(lpvalue);

                ReadData;
            Wend;

             // agrego datos a la tabla auxiliar del dibujo
            cTables["__AuxData__"].Add(sTableid, sTableName);

            cTable = Dictionary;

            cTables.Add(cTable, sTableName);

             // verifico que la tabla no tenga entradas, lo que me altera la carga
            if ( lpvalue != "ENDTAB" )
            {
                 //NewObject(cTable, sTableHandle)
                Load31Table(cTable, iTableEntries);
            }
        }
        ReadData;
    }

}
 // Lee todas las tables de esta table

private void Load31Table(Collection cVars, int iEntries)
    {


     // Yo usare dos colecciones

    string sTableName ;         
    string sid ;         
    Collection cTable ;         
    int i ;         

    int iCode ;         
    string NewKey ;         

     // Tengo q leer iEntries
     //For i = 1 To iEntries
    do {
        Inc i;
        cTable = Dictionary;
        sTableName = "";
        iCode = 0;

        ReadData;

         // esto lee todas las tables en la table

         //If lpCode = "0" Then Break

        While lpcode <> "0";
            NewKey = lpcode;
            if ( cTable.Exist(NewKey) )
            {
                do {
                    iCode += 1;
                    NewKey = lpcode + "_" + CStr(iCode);

                    if ( ! cTable.Exist(NewKey) ) Break;
                }
            }
            cTable.Add(lpvalue, NewKey);

            if ( lpcode == Me.codid ) sTableName = lpvalue;
            ReadData;

        Wend;
         //If cTable.Count = 1 Then Stop
        if ( cTable.Count > 0 )
        {
            if ( sTableName == "" ) sTableName = CStr(i);
            cVars.Add(cTable, sTableName);

        }

        if ( lpcode == "0" && lpValue == "ENDTAB" ) Break;

    }

    if ( cTable.Exist("5") )
    {
        sid = cTable["5"];
    }
    else if ( cTable.Exist("105") )
    {
        sid = cTable["105"];
    }
    else if ( cTable.Exist("2") )
    {
        sid = cTable["2"];
    }
    else
    {
        sid = gcd.Newid();

    }
     //NewObject(cTable, sHandle)

     //gcd.JSONtoLayers

    Try gcd.debuginfo("DXF: Leidas" + cTable.count + " tablas");

}

private void Load4Blocks(Collection cBlocks)
    {


    Block mBlock ;         
    Variant unread ;         
    int i ;         

    string sTableName ;         

    Collection cTable ;         
    Collection cEntities ;         

    int iCode ;         
    string NewKey ;         

    ReadData;
    do {

        mBlock = new Block;

        if ( lpCode == "0" && lpValue == "ENDSEC" ) Break;

        if ( (lpcode == "0") && (lpvalue == "BLOCK") )
        {
            Inc i;
            cTable = Dictionary;

            ReadData;

            if ( lpcode == "" ) Break;

            While lpcode <> "0";
                NewKey = lpcode;
                if ( cTable.Exist(NewKey) )
                {
                    do {
                        iCode += 1;
                        NewKey = lpcode + "_" + CStr(iCode);

                        if ( ! cTable.Exist(NewKey) ) Break;
                    }
                }

                if ( lpcode == Me.codid ) sTableName = lpvalue;
                cTable.Add(lpvalue, NewKey);
                ReadData;

            Wend; // fin del encabezado del Block, siguen sus entidades
             //NewObject(cTable, cTable["5"])
             // si estoy leyendo bloques, significa que estoy abriendo un plano
            cEntities = Dictionary;
            cTable.Add(cEntities, "entities");

            Load5Entities(cEntities);

            if ( sTableName == "" ) sTableName = CStr(i);

            cBlocks.Add(cTable, sTableName);

        }
    }

    gcd.debuginfo("DXF: Leidos " + cBlocks.Count + " bloques");

}

private void Load5Entities(Collection cEntities)
    {


    string[] sClave ;         
    string[] sValue ;         
    string sEntidad ;         
    string sKey ;         
    Object clsidr ;         
    Entity eNueva ;         
    bool Reads ;         

    Collection cEntity ;         
    int iEntity ;         

    int iCode ;         
    string NewKey ;         

    do {
         //Debug lpcode, lpvalue
        sClave = new String[];

        sValue = new String[];

        if ( lpValue == "ENTITIES" ) ReadData;
        if ( lpValue == "ENDSEC" ) return;

        sEntidad = lpValue;
        Inc iEntity;
        cEntity = Dictionary;

        cEntity.Add(sEntidad, "0");
        iCode = 0;

         // Leo descentralizadamente las entidades
        ReadData;

        While (lpcode <> "0") And Not Eof(fp);

            NewKey = lpcode;
            if ( cEntity.Exist(NewKey) )
            {
                do {
                    iCode += 1;
                    NewKey = lpcode + "_" + CStr(iCode);

                    if ( ! cEntity.Exist(NewKey) ) Break;
                }
            }

            if ( sEntidad != "ENDSEC" ) cEntity.Add(lpvalue, NewKey);
            ReadData;

        Wend;

         //NewObject(cEntity, cEntity[dxf.codHandle])

        if ( cEntity.Exist(dxf.codid) )
        {
            sKey = cEntity[dxf.codid];
        }
        if ( sKey == "" )
        {

            sKey = gcd.NewId();

        }

        if ( sEntidad != "ENDBLK" ) cEntities.Add(cEntity, sKey);

        if ( sEntidad == "ENDBLK" || sEntidad == "ENDSEC" ) return;

    }

}

private void Load6Objects(Collection cObjects)
    {


    string[] sClave ;         
    string[] sValue ;         
    string sEntidad ;         
    string h ;         
    Object clsidr ;         
    Entity eNueva ;         
    bool Reads ;         

    Collection cObject ;         
    int iObject ;         

    int iCode ;         
    string NewKey ;         

    do {
         //Debug lpcode, lpvalue
        sClave = new String[];

        sValue = new String[];

        if ( lpValue == "OBJECTS" ) ReadData;
        if ( lpValue == "ENDSEC" ) return;

        sEntidad = lpValue;
        Inc iObject;
        cObject = Dictionary;

        cObject.Add(sEntidad, "0");
        iCode = 0;

         // Leo descentralizadamente las entidades
        ReadData;

         //If sEntidad = "HATCH" Then Stop
        While (lpcode <> "0") And Not Eof(fp);

            NewKey = lpcode;
            if ( cObject.Exist(NewKey) )
            {
                do {
                    iCode += 1;
                    NewKey = lpcode + "_" + CStr(iCode);

                    if ( ! cObject.Exist(NewKey) ) Break;
                }
            }
            cObject.Add(lpvalue, NewKey);
            ReadData;

        Wend;
         //NewObject(cObject, cObject["5"])
        if ( cObject.Exist("5") )
        {
            h = cObject["5"];
        }
        else
        {
            h = CStr(iObject);
        }
        cObjects.Add(cObject, h);

        if ( sEntidad == "ENDBLK" || sEntidad == "" ) return;

    }

}

private void Load7Thumbnail(Collection cThumbnail)
    {


    int iCode ;         
    string NewKey ;         

    do {

        if ( lpValue == "ENDSEC" ) return;

         // Leo descentralizadamente las entidades
        ReadData;

        While (lpcode <> "0") And Not Eof(fp);

            NewKey = lpcode;
            if ( cThumbnail.Exist(NewKey) )
            {
                do {
                    iCode += 1;
                    NewKey = lpcode + "_" + CStr(iCode);

                    if ( ! cThumbnail.Exist(NewKey) ) Break;
                }
            }
            cThumbnail.Add(lpvalue, NewKey);
            ReadData;

        Wend;

    }

}

public void ReconstructHandles(Drawing drw)
    {


    DictEntry di ;         
    DictList item ;         
    Block b ;         
     // Empiezo por los Bloques importantes
    foreach ( b in drw.Blocks)
    {
        if ( b.name == "*Model_Space" )
        {
            b.idContainer = NewHandle(2);
            b.id = NewHandle();
        }
        else if ( Left(b.name, 6) == "*Paper" )
        {
            b.idContainer = NewHandle();
            b.id = NewHandle();
        }
        else
        {
            b.id = "";
            b.idContainer = "";
        }
    }
     // Ahora creo un Diccionario
    drw.Dictionary.id = NewHandle();
    di = new DictEntry;

    di.name = "ACAD_LAYOUT";
    di.id = NewHandle();
    drw.Dictionary.Definitions.Add(di, di.name);

     // Ahora creo una entrada de diccionario para los Layout
    foreach ( b in drw.Blocks)
    {
        if ( b.Sheet )
        {
            b.Sheet.id = NewHandle();
            item = new DictList;
            item.name = b.Sheet.Name;
            item.idSoftOwner = b.Sheet.id;
            drw.Dictionary.Definitions["ACAD_LAYOUT"].items.Add(item);
        }
    }

     // asigno handles a las Tables

}

public int SaveFile(string sName, Drawing drwToSAve, bool LoadMinimal= False, bool SaveHeader= True, bool SaveTables= True, bool SaveBlocks= True, bool SaveThumbnail= True)
    {


    hFile = Open sName for ( Create;

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
    Inc Application.Busy;
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
    if ( Save6Objects(drwToSAve) ) return False;
    if ( SaveThumbnail )
    {
        Save7ThumbNail(Null);
    }

    hFile.Close;
    Dec Application.Busy;
    gcd.debugInfo(("Saved to ") + sName,,, True, True);
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

    Console.WriteLine(#hFIle, "999" + "\n");
    Console.WriteLine(#hFIle, "GambasCAD" + "\n");
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "SECTION" + "\n");
    Console.WriteLine(#hFIle, "2" + "\n");
    Console.WriteLine(#hFIle, "HEADER" + "\n");

     //Intento guardar algunas cosas utiles para cuando abra de nuevo este archivo
    drw.Headers.CLAYER = drw.CurrLayer.Name; // Current LAYER

    stxHeaders = drw.Headers.ExportDXF();

    foreach ( sValues in stxHeaders)
    {
        Console.WriteLine(#hFile, sValues + "\n");
    }
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDSEC" + "\n");

}

 // Las classes de cad no las usamos. En teoria, no tienen ninguna utilidad fuera de AutoCAD.
 // Abriendo un DXF, se guadaran todas las classes a efectos de recosntruir el DXF.
private int Save2Classes(Drawing drwSaving)
    {


    CadClass cClass ;         

     // Dim i As Integer

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "SECTION" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "CLASSES" + "\n");

     // For Each cClass In drwSaving.CadClasses
     //     Print #hFIle, "  0"
     //     Print #hFIle, "CLASS"
     //
     //     Print #hFile, "  1"
     //     Print #hFIle, cClass.RecordName
     //
     //     Print #hFile, "  2"
     //     Print #hFIle, cClass.CPPName
     //
     //     Print #hFile, "  3"
     //     Print #hFIle, cClass.AppName
     //
     //     Print #hFile, " 90"
     //     Print #hFIle, CStr(cClass.ProxyCapp)
     //
     //     Print #hFile, " 91"
     //     Print #hFIle, CStr(cClass.InstanceCount)
     //
     //     Print #hFile, "280"
     //     Print #hFIle, CStr(cClass.ProxyFlag)
     //
     //     Print #hFile, "281"
     //     Print #hFIle, CStr(cClass.EntityFlag)
     //
     // Next
     // end section code
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDSEC" + "\n");

}

private int Save3TablesDirect(Drawing drw)
    {


    Collection cTable ;         
    Collection cTableEntry ;         
    Collection cVar ;         
    Collection cVars ;         
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
     //sHandle1 = NewHandle(1)
     // Handle_Block_REcord = Dictionary
     // Handle_Block = Dictionary
     // Handle_Layout = Dictionary
     //

     // For Each s In drw.Sheets
     //
     //     s.Block.idContainer = NewHandle()
     //     s.Block.id = NewHandle()
     //     s.id = NewHandle()
     //     s.Block.idAsociatedLayout = s.id
     //     For Each e In s.Block.entities
     //         e.IdContainer = s.Block.idContainer
     //         e.id = NewHandle()
     //     Next
     //
     // Next
     // For Each s In drw.Sheets
     //
     //     s.Block.idContainer = NewHandle()
     //     s.Block.id = NewHandle()
     //     s.id = NewHandle()
     // Next
     // hObjects = NewHandle()
     // hDictionary = NewHandle()
     // For Each b In drw.Blocks
     //     If b.Sheet Then Continue
     //     b.idContainer = NewHandle()
     //     b.id = NewHandle()
     //     // For Each e In b.entities
     //     //     e.IdContainer = b.idContainer
     //     //     e.id = NewHandle()
     //     // Next
     // Next

     // For Each s In drw.Sheets
     //     For Each e In s.Entities
     //         If InStr(e.Gender, "DIMENSION") > 0 Then
     //             e.pBlock.name = "*D" & Str(iDimCounter)
     //             e.pBlock.idContainer = NewHandle()
     //             e.pBlock.id = NewHandle()
     //             For Each e2 In e.pBlock.entities
     //                 e2.IdContainer = e.pBlock.idContainer
     //                 e2.id = NewHandle()
     //             Next
     //             // Handle_Block_Record.Add(NewHandle(), e.pBlock.name)
     //             // Handle_Block.Add(NewHandle(), e.pBlock.name)
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
     //                 e2.id = NewHandle()
     //             Next
     //             // Handle_Block_Record.Add(NewHandle(), e.pBlock.name)
     //             // Handle_Block.Add(NewHandle(), e.pBlock.name)
     //             Inc iDimCounter
     //         Endif
     //     Next
     //     Inc iSheetCounter
     // Next

     // Empiezo la seccion tables
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "SECTION" + "\n");
    Console.WriteLine(#hFIle, "2" + "\n");
    Console.WriteLine(#hFIle, "TABLES" + "\n");

    Save3TableViewPorts(drw);
    Save3TableLineTypes(drw);
    Save3TableLayers(drw);
    Save3TableTextStyles(drw);
    Save3TableViews(drw);
    Save3TableAppID(drw);
    Save3TableUCSs(drw);
    Save3TableDimStyles(drw);
    save31BlockRecord(drw);
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDSEC" + "\n");

}

private int Save3TableAppID(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle();

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "APPID" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.AppIDs.Count) + "\n");

     // APPID
    APPID oneAppid ;         
    foreach ( oneAppid in drw.AppIDs)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "APPID" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n"); //oneAppid.id
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbRegAppTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  2" + "\n");
        Console.WriteLine(#hFIle, oneAppid.APPName_2 + "\n");
        Console.WriteLine(#hFIle, " 70" + "\n");
        Console.WriteLine(#hFIle, CStr(oneAppid.Flags_70) + "\n");
    }
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableLayers(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle();

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "LAYER" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.Layers.Count) + "\n");

    Layer oneLayer ;         
    foreach ( oneLayer in drw.Layers)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "LAYER" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbLayerTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  2" + "\n");
        Console.WriteLine(#hFIle, oneLayer.Name + "\n");
        Console.WriteLine(#hFIle, " 70" + "\n"); // layer flags, bit coded
        Console.WriteLine(#hFIle, CStr(-oneLayer.Frozen - oneLayer.Locked * 4) + "\n");
        Console.WriteLine(#hFIle, " 62" + "\n");
        Console.WriteLine(#hFIle, oneLayer.Colour * IIf(oneLayer.Visible, 1, -1) + "\n");
        Console.WriteLine(#hFIle, "  6" + "\n");
        Console.WriteLine(#hFIle, oneLayer.LineType.Name + "\n");
        Console.WriteLine(#hFIle, "290" + "\n"); // plotting flag
        Console.WriteLine(#hFIle, IIf(oneLayer.Printable, "1", "0") + "\n");
        Console.WriteLine(#hFIle, "370" + "\n"); // linewt
        Console.WriteLine(#hFIle, CStr(oneLayer.LineWt) + "\n");
        Console.WriteLine(#hFIle, "390" + "\n"); // plotstyle object
        Console.WriteLine(#hFIle, " " + "\n");
        Console.WriteLine(#hFIle, "347" + "\n"); // material
        Console.WriteLine(#hFIle, " " + "\n");

    }
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableTextStyles(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle();

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "STYLE" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.TextStyles.Count) + "\n");

    TextStyle oneTextStyle ;         
    foreach ( oneTextStyle in drw.TextStyles)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "STYLE" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbTextStyleTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  2" + "\n");
        Console.WriteLine(#hFIle, oneTextStyle.name + "\n");
        Console.WriteLine(#hFIle, " 70" + "\n"); // flags, bit coded
        Console.WriteLine(#hFIle, oneTextStyle.Flags + "\n");
        Console.WriteLine(#hFIle, " 40" + "\n");
        Console.WriteLine(#hFIle, CStr(oneTextStyle.FixedH_40) + "\n");
        Console.WriteLine(#hFIle, " 41" + "\n");
        Console.WriteLine(#hFIle, CStr(oneTextStyle.WidthFactor) + "\n");
        Console.WriteLine(#hFIle, " 50" + "\n");
        Console.WriteLine(#hFIle, CStr(oneTextStyle.ObliqueAngle) + "\n");
        Console.WriteLine(#hFIle, " 71" + "\n");
        Console.WriteLine(#hFIle, CStr(oneTextStyle.iDirection) + "\n");
        Console.WriteLine(#hFIle, " 42" + "\n");
        Console.WriteLine(#hFIle, CStr(oneTextStyle.fLastHeightUsed_42) + "\n");
        Console.WriteLine(#hFIle, "  3" + "\n");
        Console.WriteLine(#hFIle, oneTextStyle.sFont_3 + "\n");
        Console.WriteLine(#hFIle, "  4" + "\n");
        Console.WriteLine(#hFIle, oneTextStyle.sBigFont_4 + "\n");

    }
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableDimStyles(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle(); //Utils.FindItem(drw.Tables, drw.DimStyles)

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "DIMSTYLE" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.DimStyles.Count) + "\n");

    DimStyle oneDimtStyle ;         
    foreach ( oneDimtStyle in drw.DimStyles)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "DIMSTYLE" + "\n");
        Console.WriteLine(#hFIle, " 105" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, " 330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, " 100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, " 100" + "\n");
        Console.WriteLine(#hFIle, "AcDbDimStyleTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  2" + "\n");
        Console.WriteLine(#hFIle, oneDimtStyle.name + "\n");
        Console.WriteLine(#hFIle, " 70" + "\n"); // flags, bit coded
        Console.WriteLine(#hFIle, "0" + "\n"); // no lo usamos
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
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableLineTypes(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle(); //Utils.FindItem(drw.Tables, drw.LineTypes)

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "LTYPE" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.LineTypes.Count) + "\n");

    LineType oneLtype ;         
    foreach ( oneLtype in drw.LineTypes)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "LTYPE" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbLinetypeTableRecord" + "\n");
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
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableUCSs(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle(); //Utils.FindItem(drw.Tables, drw.UCSs)

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "UCS" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.UCSs.Count) + "\n");

    UCS oneUCS ;         
    foreach ( oneUCS in drw.UCSs)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "UCS" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbUCSTableRecord" + "\n");
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
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableViews(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle(); //Utils.FindItem(drw.Tables, drw.Views)

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "VIEW" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.Views.Count) + "\n");

    View oneView ;         
    string sData ;         
    foreach ( oneView in drw.Views)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "VIEW" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbViewTableRecord" + "\n");
        foreach ( sData in oneView.Datos)
        {
            Console.WriteLine(#hFIle, sData + "\n");
        }

    }
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save3TableViewPorts(Drawing drw)
    {


    string hTableHandle ;         

    hTableHandle = NewHandle(); //Utils.FindItem(drw.Tables, drw.Viewports)

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "VPORT" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n"); // handle
    Console.WriteLine(#hFIle, hTableHandle + "\n");
    Console.WriteLine(#hFIle, "  330" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "0" + "\n");
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.Viewports.Count) + "\n");

    Viewport oneViewport ;         
    string sData ;         
    foreach ( oneViewport in drw.Viewports)
    {

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "VPORT" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle propio
        Console.WriteLine(#hFIle, NewHandle() + "\n");
        Console.WriteLine(#hFIle, "  330" + "\n"); // handle del padre
        Console.WriteLine(#hFIle, hTableHandle + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  100" + "\n");
        Console.WriteLine(#hFIle, "AcDbViewportTableRecord" + "\n");
        foreach ( sData in oneViewport.Datos)
        {
            Console.WriteLine(#hFIle, sData + "\n");
        }

    }
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int save31BlockRecord(Drawing drw)
    {


    Block eBlock ;         
    Sheet s ;         
    Collection cTable ;         
    Collection cTableEntry ;         
    Collection cVar ;         
    Collection cVars ;         
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

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "TABLE" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "BLOCK_RECORD" + "\n");
    Console.WriteLine(#hFIle, "  5" + "\n");
    Console.WriteLine(#hFIle, "1" + "\n"); // siempre es 1
    Console.WriteLine(#hFIle, "  330" + "\n");
    Console.WriteLine(#hFIle, "0" + "\n"); // handle al padre de esta tabla, que es 0 porque pertenece al drawing
    Console.WriteLine(#hFIle, "  100" + "\n");
    Console.WriteLine(#hFIle, "AcDbSymbolTable" + "\n");
    Console.WriteLine(#hFIle, "  70" + "\n");
    Console.WriteLine(#hFIle, CStr(drw.Sheets.Count + drw.Blocks.Count + iDimCounter) + "\n");

    foreach ( b in drw.Blocks)
    {
        if ( b.idContainer == "" )
        {
            b.idContainer = NewHandle();
            b.id = NewHandle();
        }

        if ( b.Sheet )
        {
            idAsociatedLayout = b.Sheet.id;
        }
        else
        {
            idAsociatedLayout = "0";
        }

        Console.WriteLine(#hFIle, "  0" + "\n");
        Console.WriteLine(#hFIle, "BLOCK_RECORD" + "\n");
        Console.WriteLine(#hFIle, "  5" + "\n"); // handle
        Console.WriteLine(#hFIle, b.idContainer + "\n");
        Console.WriteLine(#hFIle, " 330" + "\n");
        Console.WriteLine(#hFIle, "1" + "\n");
        Console.WriteLine(#hFIle, " 100" + "\n");
        Console.WriteLine(#hFIle, "AcDbSymbolTableRecord" + "\n");
        Console.WriteLine(#hFIle, " 100" + "\n");
        Console.WriteLine(#hFIle, "AcDbBlockTableRecord" + "\n");
        Console.WriteLine(#hFIle, "  2" + "\n");
        Console.WriteLine(#hFIle, b.name + "\n");
        Console.WriteLine(#hFIle, " 340" + "\n");
        Console.WriteLine(#hFIle, idAsociatedLayout + "\n");
        Console.WriteLine(#hFIle, "  70" + "\n");
        Console.WriteLine(#hFIle, b.InsertUnits + "\n");
        Console.WriteLine(#hFIle, " 280" + "\n");
        Console.WriteLine(#hFIle, b.Explotability + "\n");
        Console.WriteLine(#hFIle, " 281" + "\n");
        Console.WriteLine(#hFIle, b.Scalability + "\n");
    }

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDTAB" + "\n");

}

private int Save4BlocksDirect(Drawing drw)
    {


    int i ;         
    int iPaperSpaceCounter ;         
    bool bCan ;         
    int iii ;         
    Block eBlock ;         
    New String[] stxEnty ;         
    Entity eEnty ;         
    string sValues ;         
    string lpclave ;         
    string sBlockName ;         
    Sheet s ;         
    Collection eBlocks ;         
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
                eEnty.id = NewHandle();
                DXFSaveCommonEntityData(eEnty);
                Gcd.CCC[eEnty.gender].SaveDxfData(eEnty);

            }

             // que pasa con el ENDBLK?
             // Al leer, lo guardo como una entidad y, por lo tanto lo tengo en el bloque, pero...corresponde?
             // no seria mejor generarlo? al fin y al cabo es solo una señal para el lector de archivos (como el SEQEND)

        }
        SaveCode(0, "ENDBLK");
        SaveCode(5, NewHandle());
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
    SaveCode(dxf.codLWht, CStr(eEnty.LineWidth));

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
    Collection cEntities ;         

    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "SECTION" + "\n");
    Console.WriteLine(#hFIle, "  2" + "\n");
    Console.WriteLine(#hFIle, "ENTITIES" + "\n");
    foreach ( s in drwToSAve.Sheets)
    {

         // here go all entities
        foreach ( eEnty in s.Entities)
        {

            eEnty.id = NewHandle(); //FIXME: el problema es que ahora el Key en la Colection <> Id

            DXFSaveCommonEntityData(eEnty);
            Gcd.CCC[eEnty.gender].SaveDxfData(eEnty);

            if ( eEnty.Gender == "INSERT" )
            {

                bAttribPresent = False;
                if ( (eEnty.pBlock.entities.Count > 0) )
                {
                    eEnty.pBlock.idContainer = eEnty.id;

                    foreach ( eBlock in eEnty.pBlock.entities)
                    {

                        if ( eBlock.Gender == "ATTRIB" )
                        {
                            bAttribPresent = True;

                            eBlock.idContainer = NewHandle();
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
                        SaveCode(5, NewHandle());
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
                    foreach ( eBlock in eEnty.pBlock.entities)
                    {
                        eBlock.id = NewHandle();
                        DXFSaveCommonEntityData(eBlock);
                        Gcd.CCC[eBlock.gender].SaveDxfData(eBlock);

                    }

                }

                SaveCode(0, "SEQEND");
                SaveCode(5, NewHandle());
                SaveCode(330, eEnty.id);
                SaveCode(100, "AcDbEntity");
                SaveCode(8, "0");
                SaveCode(100, "AcDbBlockEnd");
            }
             // Else    // trato de exportar como vino
             //
             //     gcd.debugInfo(("No puedo guardar este tipo de entidades ") & eEnty.Gender,,, True)
             // End If
        }
    }

     // end section code
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDSEC" + "\n");

    return;

}

private bool Save6Objects(Drawing drw)
    {


    string sValues ;         
    string lpclave ;         
    int i ;         

    Collection cObject ;         
    Dictionary cObjects ;         
    Sheet s ;         
    DictEntry de ;         

     // armo un diccionary, requerido por acad
    cObject = Dictionary;
     // InsertCode(0, "DICTIONARY", cObject)
     // InsertCode(5, drw.Dictionary.id, cObject)
     // InsertCode(330, "0", cObject)
     // For Each de In drw.Dictionary.Definitions
     //     InsertCode(3, de.name, cObject)
     //     InsertCode(350, de.id, cObject)
     // Next
     //
     // cObjects.Add(cObject, "DICTIONARY")

    SaveCode(0, "SECTION");
    SaveCode(2, "OBJECTS");
    SaveCode(0, "DICTIONARY"); // Definiciones del diccionario
    SaveCode(5, drw.Dictionary.id);
    SaveCode(330, 0);
    SaveCode(100, "AcDbDictionary");
    SaveCode(280, 0);
    SaveCode(281, 1);
    foreach ( de in drw.Dictionary.Definitions)
    {
        SaveCode(3, de.name);
        SaveCode(350, de.id);
    }

    foreach ( de in drw.Dictionary.Definitions)
    {

        SaveCode(0, "DICTIONARY");
        SaveCode(5, de.id);
        SaveCode(330, drw.Dictionary.id);
        SaveCode(100, "AcDbDictionary");
         //SaveCode(280, 0)
        SaveCode(281, 1);
        foreach ( dl As DictList in de.items)
        {
            SaveCode(3, dl.name);
            SaveCode(350, dl.idSoftOwner);
        }

    }
     // SaveCode(2, "DICTIONARY")
     // SaveCode(5, "B")
     // SaveCode(330, "A")
     // SaveCode(100, "AcDbDictionary")
     // SaveCode(280, 0)
     // SaveCode(281, 1)

     // FIXME: reparar los papaerspaces
    objLayout.ExportDXF(drw);
     // For Each s In drw.Sheets
     //     cObject = Dictionary
     //     objLayout.ExportDXF(s, cObject)
     //     // pequeña correccion del handle
     //     cObject["330"] = drw.Dictionary.id
     //     cObjects.Add(cObject, s.Name)
     // Next
     // For Each cObject In cObjects
     //     SaveColection(cObject)
     // Next
     // end section code
    Console.WriteLine(#hFIle, "  0" + "\n");
    Console.WriteLine(#hFIle, "ENDSEC" + "\n");

}

private int Save7ThumbNail(Image imgGLArea)
    {


    string sValues ;         
    string lpclave ;         
    int i ;         
    Collection cThumbs ;         

    SaveCode(0, "SECTION");
    SaveCode(2, "THUMBNAILIMAGE");
     // cThumbs = cData["THUMBNAILIMAGE"]
     // If Not IsNull(cThumbs) Then
     //   For Each sValues In cThumbs
     //     lpclave = cThumbs.Key
     //     I = InStr(lpclave, "_")
     //     If i > 0 Then lpclave = Left(lpclave, i - 1)
     //     Print #hFile, lpclave
     //     Print #hFIle, sValues
     //   Next
     // End If
     // // end section code
    SaveCode(0, "ENDSEC");

     // end file code
    SaveCode(0, "EOF");

}

 // Inserta un codigo en una coleccion considerando repeticiones de la Key
public void InsertCode(int iCode, Variant sData, Collection cAcumulator)
    {


    string NewKey ;         
    int iCodeAux ;         

    NewKey = Str(iCode);
    if ( cAcumulator.Exist(NewKey) )
    {
        do {
            iCodeAux += 1;
            NewKey = Str(iCode) + "_" + CStr(iCodeAux);

            if ( ! cAcumulator.exist(NewKey) ) Break;
        }
    }
    cAcumulator.Add(CStr(sData), NewKey);

}

 //   Helper para leer DXF: retorna la posicion en la que encontro la clave o -1 si no la encontro
 //   iCode = el codigo DXF
 //   stxClaves = array de claves DXF
 //   stxValues = array de valores DXF
 //   RetValue = el valor a retornar, pasado por referencia
 //   iStartPos = la posivion inicial en los array para la busqueda (def = 0)
 //   ExactPos = si se busca solo en la posicion inicial (def = false)
public int ReadCode(int iCode, string[] stxClaves, string[] stxValues, Variant ByRef RetValue, int iStartPos= 0, bool ExactPos= False)
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
    if ( iStartCode >= 0 ) StartOK = False;
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
public int GoToCodeFromCol(Collection cDxfEntityData, int iCode, string sValue)
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
public string ReadCodeFromCol(Collection cDxfEntityData, int iCode, bool ReadNext= False, bool IgnoreAcadData= True, Variant vDefaultValue= "")
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
                valid = False;
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
            OpenedSection = False;

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

    Console.WriteLine(#hFIle, Format(sCode, "###0") + "\n");
     // If IsFloat(sValue) Then
     //   sToPrint = CStr(svalue)
     // Else
     //   sToPrint = svalue
     // Endif
    Console.WriteLine(#hFIle, svalue + "\n");

}

public void SaveCodeInv(string sValue, string sCode)
    {


    SaveCode(scode, svalue);

}

private void SaveColection(Collection cData)
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

 // // Adds a new object to object by handle collection
 // Private Sub NewObject(drw As Drawing, oNew As Variant, sHandle As String)
 //
 //     If sHandle = "" Then Return
 //
 //     If drw.Handles.Exist(sHandle) Then
 //         gcd.debugInfo("WARNING: Handle repedida " & sHandle)
 //     Else
 //
 //         drw.handles.Add(oNew, sHandle)
 //     End If
 //
 // End

 // Reads layers collection and puts data in oLayers
public void ReadViewports(Collection cVptData, Drawing drw)
    {


    Viewport vNew ;         

     // // primero eliminamos lo q haya
    if ( ! cVptData["TABLES"].Exist("VPORT") ) return;
    Drw.Viewports.Clear;
    foreach ( cViewp As Collection in cVptData["TABLES"]["VPORT"])
    {
        vNew = new Viewport;
         // hLay.Name = cLay[dxf.codName]
         // hLay.Visible = CInt(cLay[dxf.codColor]) >= 0
         // hLay.Colour = Abs(CInt(cLay[dxf.codColor]))
         // hLay.handle = cLay[dxf.codHandle]
         // If hLay.handle = "" Then hLay.handle = gcd.NewHandle()()
         // Drw.oLayers.Add(hLay, hLay.handle)
    }

     // // es inaceptable no tener al menos un layrr
     // If drw.oLayers.Count = 0 Then
     //     hLay = New Layer
     //     hLay.Name = "0"
     //     hLay.Visible = True
     //     hLay.Colour = 0
     //     hLay.handle = gcd.NewHandle()()
     //     Drw.oLayers.Add(hLay, hLay.handle)
     // Endif
     //
     // // aprovecho para setear el layer actual
     // Drw.CurrLayer = Drw.oLayers[Drw.oLayers.First]

}

 // Reads layers collection and puts data in oLayers
public void ReadLayers(Collection cLaydata, Drawing drw)
    {


    Layer hLay ;         

     // // primero eliminamos lo q haya
    Drw.Layers.Clear;
    foreach ( cLay As Collection in cLayData["TABLES"]["LAYER"])
    {
        hLay = new Layer;
        hLay.Name = cLay[dxf.codName];
        hLay.id = cLay[dxf.codid];
        hLay.Visible = CInt(cLay[dxf.codColor]) >= 0;
        hLay.Colour = Abs(CInt(cLay[dxf.codColor]));
        hLay.LineType = drw.LineTypes[cLay[Me.codLType]];

        Try hLay.LineWt = cLay["370"]; // algunos dxf no traen esta info
        if ( hLay.LineWt == 0 ) hLay.LineWt = 1;

        if ( hLay.id == "" ) hLay.id = gcd.NewId();
        Drw.Layers.Add(hLay, hLay.Name);
    }

     // es inaceptable no tener al menos un layrr
    if ( drw.Layers.Count == 0 )
    {
        hLay = new Layer;
        hLay.Name = "0";
        hLay.Visible = True;
        hLay.Colour = 0;
        hLay.LineType = drw.LineTypes[drw.LineTypes.First];
        hLay.id = gcd.NewId();
        Drw.Layers.Add(hLay, hLay.Name);
    }

     // aprovecho para setear el layer actual
    Drw.CurrLayer = Drw.Layers[Drw.Layers.First];

     // o mejor este, pero puede fallar
    Try drw.CurrLayer = drw.Layers[drw.Headers.CLAYER];

}

 // Reads Styles and DimStyles collection and puts data in arrStyles
public void ReadStyles(Collection cData, Drawing drw)
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
    if ( cData["TABLES"].Exist("STYLE") )
    {
        foreach ( c As Collection in cData["TABLES"]["STYLE"])
        {
            hlty = new TextStyle;

            hlty.Name = Lower(c[dxf.codName]);
            hlty.Id = c[dxf.codid];
            if ( hLty.id == "" ) hLty.id = gcd.NewId();
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

            Drw.TextStyles.Add(hlty, n);

        }

        drw.CurrTextStyle = Drw.TextStyles[Drw.TextStyles.First];
    }

     // Leo lo styles de dimensiones
    if ( cData["TABLES"].Exist("DIMSTYLE") )
    {
        foreach ( c As Collection in cData["TABLES"]["DIMSTYLE"])
        {
            hdim = new DimStyle;

            hdim.Name = c[dxf.codName];

            if ( c.Exist("105") )
            {
                hdim.id = c["105"];
            }
            else
            {
                hdim.id = gcd.NewId();
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

            if ( hdim.name != "" ) Drw.DimStyles.Add(hdim, hdim.name);

        }

        drw.CurrDimStyle = Drw.DimStyles[Drw.DimStyles.First];
    }

}

 // Reads LineTypes collection and puts data in arrLTypes
public void ReadLTypes(Collection cData, Drawing drw)
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
    foreach ( c As Collection in cData["TABLES"]["LTYPE"])
    {
        hlty = new LineType;
        hlty.Name = UCase(c[dxf.codName]);
        hlty.Description = c["3"];
        if ( c.Exist("5") ) hlty.id = c["5"];
        if ( hLty.id == "" ) hLty.id = gcd.NewId();
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
                    if ( (ri && 1) == 1 ) AbsoluteRotation = True else absoluterotation = False;
                    if ( (ri && 2) == 2 ) IsText = True else IsText = False;
                    if ( (ri && 4) == 4 ) IsShape = True else IsShape = False;
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
        hlty = new LineType;
        hlty.Name = "CONTINUOUS";
        hlty.Description = "";
        hlty.id = gcd.Newid();
        hlty.nTrames = 0;
        drw.LineTypes.Add(hlty, hlty.Name);

    }

    Drw.CurrLineType = Drw.LineTypes[Drw.LineTypes.First];

}

public void ImportBlocksFromDXF(Collection colData, Drawing drw) //, obxEntities As Entity[]) As Integer
    {


    int iTotalEntities ;         
    Collection colent ;         
    Collection colBlk ;         
    New Float[] flxPoints ;         
    double[] P ;         
    string hBlock ;         
    Collection cParent ;         
    Dictionary cEntyList ;         
    Variant hEnty ;         
    int iEnty ;         
    Variant[] cEnty ;         
    int i ;         
    New Sheet S ;         
    Block newBlock ;         
    bool hasBlocks ;         
    bool hasTables ;         
    bool hasBlockRecord ;         
    bool hasEntities ;         
    Collection cBlocks ;         
    Collection cTables ;         
    Collection cBlockRecord ;         
    Collection cEntities ;         
    New Block b ;         

    if ( colData.Exist("BLOCKS") ) cBlocks = colData["BLOCKS"];
    if ( colData.Exist("TABLES") )
    {
        cTables = colData["TABLES"];
        if ( colData["TABLES"].Exist("BLOCK_RECORD") ) cBlockRecord = cTables["BLOCK_RECORD"];
    }
    if ( ! cBlocks ) return;
     // For Each colBlk In colData["TABLES"]["BLOCK_RECORD"]
     //     Dim newBlock As New Block
     //     newBlock.entities = Dictionary
     //     newBlock.name = colBlk[dxf.codName]
     //     newBlock.handle = colBlk[dxf.codHandle]
     //     newBlock.HandleOwnerParent = colBlk[dxf.codHandleOwner]
     //     newBlock.HandleAsociatedLayout = colBlk["340"]
     //     Try newBlock.InsertUnits = colBlk["70"]
     //     Try newBlock.Explotability = colBlk["280"]
     //     Try newBlock.Scalability = colBlk["281"]
     //     drw.oBlocks.Add(newBlock, newBlock.handle)
     //
     // Next
     // hay DXF sin TAbles
    if ( cTables )
    {

        foreach ( colBlk in cBlocks)
        {
            newBlock = new Block;
            newBlock.id = colBlk[dxf.codid];
            newBlock.entities = Dictionary;
            newBlock.name = colBlk[dxf.codName];
            newBlock.layer = colBlk[dxf.codLayer];
            Try newBlock.x0 = colBlk[dxf.codX0];
            Try newBlock.y0 = colBlk[dxf.codY0];
            Try newBlock.z0 = colBlk[dxf.codZ0];
            Try newBlock.flags = colBlk["70"];

            if ( cBlockRecord )
            {
                if ( cBlockRecord.Exist(colBlk["330"]) )
                {
                    newBlock.idContainer = cBlockRecord[colBlk["330"]]["5"];
                    newBlock.idAsociatedLayout = cBlockRecord[colBlk["330"]]["340"];
                     // If newBlock.idAsociatedLayout <> "0" Then
                     //     hContainers.Add(newBlock, newBlock.idAsociatedLayout)
                     // Else
                    hContainers.Add(newBlock, newBlock.idContainer);
                     // End If
                }
            }
             //If Left(newBlock.name, 1) = "*" Then    // puede ser una Dim o una Sheet
             // If InStr(newBlock.name, "_Space") > 0 Then
             //     // es una sheet, que ya fue creada por ReadObjectsFromDXF
             // Else

            Drw.Blocks.Add(newBlock, newBlock.name);

             //Endif

             //Endif

        }
    }
    if ( (Drw.Blocks.Count == 0) || ! Drw.Blocks.Exist("*Model_Space") ) // agrego el Model
    {

         // lo agrego a los bloques

        b.name = "*Model_Space";
        b.entities = Dictionary;
        b.idContainer = gcd.NewId();
        b.id = gcd.NewId();
        b.idAsociatedLayout = gcd.NewId();
        b.IsAuxiliar = True;
        b.IsReciclable = False;
        Drw.Blocks.Add(b, b.name);
    }

     // voy a hacer un chequeo final, porque algunos DXF vienen sin el bloque Model
    if ( ! Drw.Sheets.Exist("Model") )
    {
         // creo la Sheet
        S = new Sheet;
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

        if ( colBlk.Exist("entities") )
        {

            DXFtoEntity(colBlk["entities"], drw, drw.Blocks[colBlk["2"]]);

        }
        Inc i;

    }

}

public void SetViewports(Collection cDxfData, Drawing drw)
    {


    Entity e ;         
    Viewport v ;         
    string n ;         
    string hLayout ;         
    Sheet s ;         
    Block b ;         

    foreach ( b in drw.Blocks)
    {
        if ( drw.Sheets.Exist(b.idAsociatedLayout) )
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
public void DXFtoEntity(Collection cDxfEntities, Drawing drw, Block bContainer)
    {


    Collection e ;         
    Collection obx ;         
    Collection cLastParent ;         
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
        if ( e.Exist(dxf.codEntity) ) // es una entidad?
        {
             // entonces, creamos una nueva
             // poner en minuscula para anular la entidad
            if ( InStr("VIEWPORT LEADER HATCH POLYLINE ENDBLK SEQEND VERTEX POINT ATTDEF ATTRIB LINE LWPOLYLINE CIRCLE ELLIPSE ARC TEXT MTEXT SPLINE SOLID INSERT DIMENSION DIMENSION_LINEAR DIMENSION_DIAMETEr DIMENSION_RADIUs DIMENSION_ANG3Pt DIMENSION_ALIGNED DIMENSION_ORDINATE LARGE_RADIAL_DIMENSION ARC_DIMENSION MLINE", UCase(e[dxf.codEntity])) == 0 ) IsDummy = True else IsDummy = False;

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
                if ( ! ReadTimes.Exist(entNueva.Gender) ) ReadTimes.Add(fTime, entNueva.Gender);
                if ( ! ReadEntities.Exist(entNueva.Gender) ) ReadEntities.Add(1, entNueva.Gender);

                 //Debug "Contenedor", hContainer
                 // -hBlock-Record
                 // -hInsert
                 // -hHatch               OTRA ENTIDAD que tenga .pBlock.enities as Collection
                 // -Polyline

                if ( bContainer ) entNueva.Container = bContainer;
                if ( entNueva.Container )
                {
                    obx = entNueva.Container.entities;
                }
                else if ( bContainer )
                {
                    obx = bContainer.entities; //drw.Blocks[entNueva.IdContainer]
                     // Else If hContainers.Exist(entNueva.idContainer) Then
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

                 // //gcd.debugInfo("Leida entidad tipo" & entNueva.Gender & " id " & entNueva.id,,, True)
                 // If e[dxf.codEntity] = "POLYLINE" Then //Stop
                 //     flgIsPolyline = True
                 //     //obx = entNueva.pBlock
                 //     // pBlockPolyline = New Block
                 //     // pBlockPolyline.entities = Dictionary
                 //     //
                 //     // entNueva.pBlock = pBlockPolyline
                 // End If
                 // If (e[dxf.codEntity] = "SEQEND") And (flgIsPolyline = True) Then
                 //     flgIsPolyline = False
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
public void DigestColeccion(Collection c, string[] ByRef sClaves, string[] ByRef sValues)
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

public void ReadObjectsFromDXF(Collection cData, Drawing drw)
    {


    Collection cObject ;         
    Entity entNueva ;         
    New String[] sClaves ;         
    New String[] sValues ;         
    Sheet s ;         
    MLineStyle m ;         
    DictEntry entry ;         
    int i ;         
    int flags ;         
    int i2 ;         

    gcd.debugInfo("Importing DXF object data",,, True);
     //Handle_Layout = Dictionary
    if ( ! cData.Exist("OBJECTS") ) return;
    foreach ( cObject in cData["OBJECTS"])
    {

        if ( cObject["0"] == "DICTIONARY" )
        {

            this.DigestColeccion(cObject, ByRef sClaves, ByRef sValues);
            entry = new DictEntry;
             //entry.idSoftOwner = dxf.ReadCodeFromCol(cObject, 330, True)
            entry.name = dxf.ReadCodeFromCol(cObject, 3, True); // name
            entry.idSoftOwner = dxf.ReadCodeFromCol(cObject, 330, True);

        }

        if ( cObject["0"] == "LAYOUT" )
        {

            this.DigestColeccion(cObject, ByRef sClaves, ByRef sValues);
            s = new Sheet;
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
            m = new MLineStyle;
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
private  Block GetBlock(Collection cBlocks, string hBlockRecord)
    {


    Block b ;         

    foreach ( b in cBlocks)
    {
        if ( b.IdContainer == hBlockRecord ) return b;

    }

    return Null;

}

 // Devuelve una nueva handle que acumula en la var publica hexHandle
Private string NewHandle( int iStart  = "-1") ;

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


    New Single[] slx ;         
    New Integer[] inx ;         
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
            foreach ( v in vValue)
            {
                slx.Add(v);
            }
            Object.SetProperty(Me, sVarName2, slx);
        }
        else
        {
            foreach ( v in vValue)
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
    return False;

}

 // Devuelve una string apta DXF
public string[] ExportDXF()
    {


    New String[] stx ;         
    int i ;         
    int tipo ;         
    int iVal ;         
    Object  obj = Me;
    Class  MyClass = Object.Class(obj);
    string Var ;         

    New Single[] slx ;         
    New Integer[] inx ;         

    float sl ;         

    foreach ( Var in myClass.Symbols)
    {
        if ( InStr(var, "_") > 0 ) Continue;
        stx.Add("9");
        stx.Add("$" + var);

         // Verifying that it is a property or a variable.
        if ( (MyClass[var].kind == Class.Variable) || (MyClass[var].kind == Class.Property) )
        {
            tipo = Object.GetProperty(obj, "_" + var);
            if ( MyClass[var].Type == "Single[]" || MyClass[var].Type == "Integer[]" ) //is an array
            {

                if ( tipo == 10 ) // es un array
                {
                    slx = Object.GetProperty(Me, var);
                    stx.Add("10");
                    if ( slx.Count >= 1 ) stx.Add(slx[0]) else stx.Add(0);
                    stx.Add("20");
                    if ( slx.Count >= 2 ) stx.Add(slx[1]) else stx.Add(0);
                    if ( slx.Count == 3 )
                    {
                        stx.Add("30");
                        if ( slx.Count >= 3 ) stx.Add(slx[2]) else stx.Add(0);
                    }
                }
                else if ( tipo == 70 )
                {
                    inx = Object.GetProperty(Me, var);
                    foreach ( iVal in inx)
                    {
                        stx.Add("70");
                        stx.Add(i);
                    }

                }
                else if ( tipo == 40 )
                {
                    slx = Object.GetProperty(Me, var);
                    foreach ( sl in slx)
                    {
                        stx.Add("40");
                        stx.Add(sl);
                    }

                }
            }
            else
            {
                stx.Add(CStr(tipo));
                stx.Add(Object.GetProperty(Me, var));
            }
        }

    }

    return stx;

}

}