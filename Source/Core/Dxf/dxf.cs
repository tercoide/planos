using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Gaucho;
using HarfBuzz;


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
private StreamReader? fp ;



        private StreamWriter? hFile ;         

private string lpCode = "";         
private string lpValue = "";         

private int LastCodeReadIndex = 0;

// Additional properties for DXF processing
public int VerboseLevel = 0;
public bool IgnoreHeader = false;
public bool IgnoreTables = false;
public bool IgnoreBlocks = false;         

public Dictionary<string, Dictionary<string, Entity>> hContainers = new Dictionary<string, Dictionary<string, Entity>>();
private Dictionary<string, Dictionary<string, string>> ReadTimes = new Dictionary<string, Dictionary<string, string>>();
private Dictionary<string, Dictionary<string, string>> ReadEntities = new Dictionary<string, Dictionary<string, string>>();

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
        string tmpfile;
        // elimino el archivo temporal que hubiese creado
        tmpfile = sDwgFile + ".tmp";
        if (File.Exists(tmpfile)) File.Delete(tmpfile);
        // convierte DWG a DXF version 20
        // Shell "/usr/local/bin/dwgread; //" & sDwgFile & "// -O DXF -a r2010 -o //" & tmpfile & "//" Wait To str
        // Gcd.debugInfo("Resultados de la conversion DWG a DXF " + str);

        return tmpfile;

    }

 // Carga el DXF y lo mete en cModel del dibujo actual
 // Verbose=0 nada, 1=minimo, 2=grupos, 3=todo
public bool LoadFile(string sfile, Drawing drw, bool ignoretables= false, bool ignoreblocks= false, bool ignoreheader= false, int verboselevel= 0, bool updategraphics= true, bool readobjects= true)
{
    bool IgnoreTables = ignoretables;
    bool IgnoreBlocks = ignoreblocks;
    bool IgnoreHeader = ignoreheader;
    int VerboseLevel = verboselevel;
    bool UpdateGraphics = updategraphics;
    bool ReadObjects = readobjects;
    Dictionary<string, Dictionary<string, string>>? cLlaveActual = null;         
    Dictionary<string, Dictionary<string, Dictionary<string, string>>>? cToFill = null;         

    fp = new StreamReader(sfile);

    if ( fp == null ) throw new Exception("Error opening file!");

    LoadedBytes = 0;
    LoadTotalBytes = (int)new FileInfo(sfile).Length;
    
    // cEntitiesUnread = Dictionary<string, Dictionary>;
    // nEntitiesUnread = 0;
    // nEntitiesRead = 0;
    hContainers = new Dictionary<string, Dictionary<string, Entity>>(); // Clave = Handle , Dato = Colection
    cToFill = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        while  (!fp.EndOfStream)
        {
            //Wait 0.0001
            ReadData();
            if (lpCode == "0" && lpValue == "SECTION")
            {

                // vemos que seccion es
                ReadData();
                if (lpCode == "2" && lpValue == "HEADER" && !IgnoreHeader)
                {
                    // // creo la llave, pero solo si es necesario
                    // if (!cToFill.ContainsKey("HEADER"))
                    // {
                    //     cLlaveActual = Dictionary<string, Dictionary>;
                    //     cToFill.Append("HEADER", cLlaveActual);
                    // }
                    // else
                    // {
                    //     cLlaveActual = cToFill["HEADER"];
                    // }

                    Load1HeadersDirect(drw.Headers);
                    if (VerboseLevel > 2) {} // Gcd.debugInfo("Leidos Headers", false, false, true);


                }

                if (lpCode == "2" && lpValue == "CLASSES")
                {

                    Load2Classes(drw);
                    if (VerboseLevel > 2) {} // Gcd.debugInfo("Leidas Classes", false, false, true);

                }

                if (lpCode == "2" && lpValue == "TABLES" && !IgnoreTables)
                {
                    if (!cToFill.ContainsKey("TABLES"))
                    {
                        cLlaveActual = new Dictionary<string, Dictionary<string, string>>();
                        cToFill.Add("TABLES", cLlaveActual);
                    }
                    else
                    {
                        cLlaveActual = cToFill["TABLES"];
                    }
                    Load3Tables(drw);
                    if (VerboseLevel > 2) Gcd.debugInfo("Leidos Tables", false, false, true);

// ahora las leo directamente
                    // // con las tablas cargadas, llenamoslas colecciones de objetos
                    // ReadViewports(cToFill, drw);
                    // ReadLTypes(cToFill, drw);
                    // ReadStyles(cToFill, drw);
                    // ReadLayers(cToFill, drw);
                    if (VerboseLevel > 2) Gcd.debugInfo("Tables al Drawing", false, false, true);

                }

                //
                if (lpCode == "2" && lpValue == "BLOCKS" && !IgnoreBlocks)
                {
                    // creo la llave
                    cLlaveActual = new Dictionary<string, Dictionary<string, string>>();
                    cToFill.Add("BLOCKS", cLlaveActual);
                    Load4Blocks(cLlaveActual,drw);
                    if (VerboseLevel > 2) Gcd.debugInfo("Leidos Blocks", false, false, true);

                }

                if (lpCode == "2" && lpValue == "ENTITIES")
                {
                    // creo la llave
                    cLlaveActual = new Dictionary<string, Dictionary<string, string>>();
                    cToFill.Add("ENTITIES", cLlaveActual);

                    Load5Entities(cLlaveActual,drw);
                    if (VerboseLevel > 2) Gcd.debugInfo("Leidas Entidades", false, false, true);

                }
                //
                if (lpCode == "2" && lpValue == "OBJECTS")
                {

                    if (!cToFill.ContainsKey("OBJECTS"))
                    {
                        cLlaveActual = new Dictionary<string, Dictionary<string, string>>();
                        cToFill.Add("OBJECTS", cLlaveActual);
                    }
                    else
                    {
                        cLlaveActual = cToFill["OBJECTS"];
                    }

                    Load6Objects(cLlaveActual,drw);
                    if (VerboseLevel > 2) Gcd.debugInfo("Leidos Objetos", false, false, true);

                }

                if (lpCode == "2" && lpValue == "THUMBNAILIMAGE")
                {

                    if (!cToFill.ContainsKey("THUMBNAILIMAGE"))
                    {
                        cLlaveActual = new Dictionary<string, Dictionary<string, string>>();
                        cToFill.Add("THUMBNAILIMAGE", cLlaveActual);
                    }
                    else
                    {
                        cLlaveActual = cToFill["THUMBNAILIMAGE"];
                    }

                    // Load7Thumbnail(cLlaveActual); // TODO: Fix parameter type mismatch

                }

            }
        }
        ;
    Gcd.debugInfo("DXF a Dictionary<string, Dictionary>",false,false,true, true);

    if ( ReadObjects && cToFill.ContainsKey("OBJECTS") ) ReadObjectsFromDXF(cToFill["OBJECTS"], drw);
    if ( UpdateGraphics )
    {
        if (cToFill.ContainsKey("BLOCKS")) ImportBlocksFromDXF(cToFill["BLOCKS"], drw);
        
         //depre clsEntities.BuildPoi()
        if (cToFill.ContainsKey("ENTITIES")) DXFtoEntity(drw, cToFill["ENTITIES"], drw.Sheet.Entities);
        
        Gcd.debugInfo("Drawing generated",false,false,true, true);
         //clsEntities.DeExtrude(drw)
         //clsEntities.BuildGeometry();
        
         //Gcd.DigestInserts()
        if (cToFill.ContainsKey("TABLES")) SetViewports(cToFill["TABLES"], drw);
        Gcd.debugInfo("Geometry generated",false,false,true, true);
        
    }

     // For Each ft As Float In ReadTimes
     //     Gcd.debugInfo(ReadEntities[ReadTimes.Key] & " " & gb.Tab & ReadTimes.Key & gb.Tab & gb.Tab & gb.Tab & " total time: " & Format(fT, "0.0000"))
     // Next
     // Wait
     // If VerboseLevel > 1 Then
     //     If VerboseLevel > 2 Then
     //         Gcd.debuginfo("DXF: Leidas " & nEntitiesread & " entidades")
     //         If cEntitiesUnread.Count > 0 Then
     //             Gcd.debuginfo("DXF: Un total de" & nEntitiesUnread & " entidades no pudieron ser leidas:")
     //             For Each unread As String In cEntitiesUnread
     //                 Print unread
     //             Next
     //         Endif
     //         Print
     //     End If
     //     Gcd.debuginfo("DXF: fin lectura en " & Str(Timer - t))
     //
     // Else
     //     Gcd.debuginfo("DXF: fin lectura en " & Str(Timer - t))
     // End If
     // Wait
    return false;

}

private void DiscardBlocks(Drawing drw)
    {


            

    foreach ( var kvp in drw.Blocks)
    {
        Block block = kvp.Value;
        if ( (block.Name.StartsWith("*")) && (block.idAsociatedLayout != "0") ) 
        {
            drw.Blocks.Remove(kvp.Key);
        }
    }

}

private void ReadData()
    {


    lpCode = fp?.ReadLine() ?? "";
    lpValue = fp?.ReadLine() ?? "";

    LoadedBytes += lpCode != null ? lpCode.Length : 0;
    LoadedBytes += lpValue != null ? lpValue.Length : 0;

    if (lpCode != null && lpCode.Length > 0 && lpCode.EndsWith("\r")) lpCode = lpCode.Substring(0, lpCode.Length - 1);
    if (lpValue != null && lpValue.Length > 0 && lpValue.EndsWith("\r")) lpValue = lpValue.Substring(0, lpValue.Length - 1);

    lpCode = Gb.Trim(lpCode);
    lpValue = Gb.Trim(lpValue);

     // updating percentage

    LoadingPercent = LoadedBytes / LoadTotalBytes;

    if ( LoadingPercent - LoadLastPercent > 0.01 )
    {
        Gcd.debugInfo("Loging file " + (int)(LoadingPercent * 100) + "%", true, true);
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
    List<string> cVariable ;         
    int i = 0;         

    ReadData();
        do {

            if (lpCode == "0" && lpValue == "ENDSEC") break;

            if (lpCode == "9") // nueva variable
            {
                cVariable = new List<string>();
                sVarName = lpValue.Substring(1);

                do { // este bucle es por si la variable es un array
                    ReadData();
                    if (lpCode == "0" || lpCode == "9") break;
                    cVariable.Append(lpValue);
                } while  (!(lpCode == "0" || lpCode == "9"));

                // TODO: reparar
                // ya tengo la variable, la seteo
        //        if (!SetValues(sVarName, cVariable)) Gcd.debugInfo("Var " + sVarName + " not found.");
                i++;

            }

        } while (!fp.EndOfStream);
    

    Gcd.debugInfo("DXF: Leidas " + i + " variables de ambiente");

}

private void Load2Classes(Drawing drw)
    {

        // No uso estas clases, pero las leo para no perder informacion

    

        while  (!fp.EndOfStream)
        {

            if (lpValue == "CLASSES") ReadData();
            if (lpValue == "ENDSEC") return;

            // var cClass = new CadClass();
            // drwLoading.CadClasses.Append(cClass);

            ReadData();

                while ((lpCode != "0") && !fp.EndOfStream)
                {
                    // if (lpCode == "0") cClass.recordtype = lpValue;
                    // if (lpCode == "1") cClass.recordName = lpValue;
                    // if (lpCode == "2") cClass.CPPName = lpValue;
                    // if (lpCode == "3") cClass.AppName = lpValue;
                    // if (lpCode == "90") cClass.ProxyCapp = int.Parse(lpValue);
                    // if (lpCode == "91") cClass.InstanceCount = int.Parse(lpValue);
                    // if (lpCode == "280") cClass.ProxyFlag = int.Parse(lpValue);
                    // if (lpCode == "281") cClass.EntityFlag = int.Parse(lpValue);

                    ReadData();
                }

        }

    }



private void Load3Tables(Drawing drw)
    {


    string sTableName ;         
    string sTableid ;          // in hex
    string sTableContainer ;          // in hex , 0 = nobody
    int iTableEntries ;

        // creamos una table inicial con los handles de las tables
        // cTable = Dictionary<string, Dictionary>;


    ReadData();
    while  (!fp.EndOfStream)
    {
       

        if ( lpCode == "0" && lpValue == "ENDSEC" ) break;

        if ( lpCode == "0" && lpValue == "TABLE" )
        {

             // OBTENGO DATOS DE LA TABLA
             // -1 APP: entity Name(changes Each Time a drawing Is Opened)
             // 0 Object type(TABLE)
             // 2 Table Name
             // 5 Handle
             // 330 Soft - pointer ID / handle To owner object
             // 100 Subclass marker(AcDbSymbolTable)
             // 70 Maximum number Of entries In table
            ReadData();
            while ( lpCode != "0")
            {

                if ( lpCode == "5" ) sTableid = lpValue;

                if ( lpCode == "2" ) sTableName = lpValue;
                if ( lpCode == "330" ) sTableContainer = lpValue;

                 //If sTableName = "VIEW" Then Stop

                 // WARNING: este dato no es valido para todas las versiones de DXF
                 // en algunos archivos hay mas tablas que lo que indica este numero
                 // No hay que darle importancia a este numero!!!
                if ( lpCode == "70" ) iTableEntries = int.Parse(lpValue);

                ReadData();
            }

         

            // cTable = Dictionary<string, Dictionary>;

            // cTables.Append(sTableName, cTable);

             // verifico que la tabla no tenga entradas, lo que me altera la carga
            if ( lpValue != "ENDTAB" )
            {
                 //Object(cTable, sTableHandle)
                Load31Table(drw);
            }
        }
        ReadData();
    }

}
 // Lee todas las tables de esta table

private void Load31Table(Drawing drw)
    {


     // Yo usare dos colecciones

    string sTableName ;
    string sid ;
    Dictionary<string, string> cTable = new Dictionary<string, string>();
    Dictionary<string, Dictionary<string, string>> cTables = new Dictionary<string, Dictionary<string, string>>();
    int i =0;

    int iCode ;         
    string Key ;

        // Tengo q leer iEntries
        //For i = 1 To iEntries
        while (true)
        {

            i++;
            while (true)
            {

                // leo la entrada
                // cada entrada empieza con un 0
                // leo hasta encontrar otro 0 que indica otra entrada o el ENDTAB

                // creo la tabla
                cTable = new Dictionary<string, string>();
                sTableName = "";
                iCode = 0;

                ReadData();

                // esto lee todas las tables en la table

                //If lpCode = "0" Then break

                // Leo la entrada en la table
                while (lpCode != "0")
                {
                    Key = lpCode;
                    if (cTable.ContainsKey(Key))
                    {
                        do
                        {
                            iCode += 1;
                            Key = lpCode + "_" + iCode.ToString();

                            if (!cTable.ContainsKey(Key)) break;
                        } while (true);
                    }
                    cTable.Add(Key, lpValue);

                    if (lpCode == codid) sTableName = lpValue;
                    ReadData();

                }
                //If cTable.Count = 1 Then Stop
                if (cTable.Count > 0)
                {
                    if (sTableName == "") sTableName = i.ToString();
                    // no acumulo mas tablas en colecciones
                    cTables.Add(sTableName, cTable);

                }


                if (cTable.ContainsKey("5"))
                {
                    sid = cTable["5"];
                }
                else if (cTable.ContainsKey("105"))
                {
                    sid = cTable["105"];
                }
                else if (cTable.ContainsKey("2"))
                {
                    sid = cTable["2"];
                }
                else
                {
                    sid = Gcd.NewId();

                }
                if (lpCode == "0" && lpValue == "ENDTAB") break;

            } // Leo la tabla completa

            switch (cTable["0"])
            {
                case "LAYER":
                    ReadLayers(cTables, drw);

                    break;
                case "LTYPE":
                    ReadLTypes(cTables, drw);

                    break;
                case "VPORT":
                    ReadViewports(cTables, drw);
                    break;
                case "STYLE":
                    ReadStyles(cTables, drw);
                    break;
                case "BLOCK_RECORD":
                    ReadBlockRecords(cTables, drw);
                    break;
            }
            if ( lpCode == "0" && lpValue == "ENDSECTION" ) break;

        }
    
     

    


    Gcd.debugInfo("DXF: Leidas" + cTable.Values.Count.ToString() + " tablas");

}

private void Load4Blocks(Dictionary<string, Dictionary<string, string>> cBlocks, Drawing drw)
    {
    Block mBlock;         
    string sTableName = "";         
    Dictionary<string, string> cTable = new Dictionary<string, string>();
    Dictionary<string, Dictionary<string, string>> cEntities = new Dictionary<string, Dictionary<string, string>>();
    int iCode = 0;         
    string Key = "";
    int i = 0;

    ReadData();
    do {

      
            mBlock = new Block();

        if ( lpCode == "0" && lpValue == "ENDSEC" ) break;

        if ( (lpCode == "0") && (lpValue == "BLOCK") )
        {
            i++;
            cTable = new Dictionary<string, string>();

            ReadData();

            if ( lpCode == "" ) break;

            while ( lpCode != "0") {

                // Como ya tengo los bloques cargados en el drw, cargo directamente lo que necesito
                switch (lpCode)
                {
                    case "5":
                        mBlock.id = lpValue;
                        break;
                    case "2":
                        mBlock.Name = lpValue;
                        break;
                    case "70":
                        mBlock.Flags = int.Parse(lpValue);
                        break;
                    case "10":
                        mBlock.x0 = float.Parse(lpValue);
                        break;
                    case "20":
                        mBlock.y0 = float.Parse(lpValue);
                        break;
                    case "30":
                        mBlock.z0 = float.Parse(lpValue);
                        break;
                    case "330":
                        mBlock.idContainer = lpValue;
                        break;
                }
                
                Key = lpCode;

                if ( lpCode == codid ) sTableName = lpValue;
                cTable.Add(Key, lpValue);
                ReadData();

            } // fin del encabezado del Block, siguen sus entidades
             //Object(cTable, cTable["5"])

            // Note: Cannot directly add cEntities to cTable as they have incompatible types
            // This might need architectural changes - storing entities reference separately

            Load5Entities(cEntities,drw);

                if (sTableName == "") sTableName = i.ToString();

                drw.Blocks.Add(mBlock.Name, mBlock);
            DXFtoEntity(drw, cEntities, mBlock.entities);

             // DEPRE agrego la tabla a la coleccion

            cBlocks.Add(sTableName, cTable);

        }
    } while  (!fp.EndOfStream); 

     //Object(cTable, sHandle)

     //Gcd.JSONtoLayers

    Gcd.debugInfo("DXF: Leidos " + cBlocks.Count + " bloques");

}


// Lleno la coleccion de entidades
    private void Load5Entities(Dictionary<string, Dictionary<string, string>> cEntities, Drawing drw)
    {
        string sEntidad = "";
        string sKey = "";
        Dictionary<string, string> cEntity = new Dictionary<string, string>();
        int iEntity = 0;
        int iCode = 0;
        string Key = "";

        while (true)
        {
            //Debug lpCode, lpValue

            if (lpValue == "ENTITIES") ReadData();
            if (lpValue == "ENDSEC") return;

            sEntidad = lpValue;
            iEntity++;
            cEntity = new Dictionary<string, string>();

            cEntity.Add("0", sEntidad);
            iCode = 0;

            // Leo descentralizadamente las entidades
            ReadData();

            while ((lpCode != "0") && !fp.EndOfStream)
            {

                Key = lpCode;
                if (cEntity.ContainsKey(Key))
                {
                    do
                    {
                        iCode += 1;
                        Key = lpCode + "_" + iCode.ToString();


                    } while (cEntity.ContainsKey(Key));
                }

                if (sEntidad != "ENDSEC") cEntity.Add(Key, lpValue);
                ReadData();

            }

            //Object(cEntity, cEntity[codHandle])

            if (cEntity.ContainsKey(codid))
            {
                sKey = cEntity[codid];
            }
            if (sKey == "")
            {

                sKey = Gcd.NewId();

            }

            if (sEntidad != "ENDBLK") cEntities.Add(sKey, cEntity);

            if (sEntidad == "ENDBLK" || sEntidad == "ENDSEC") return;

        }

    }

private void Load6Objects(Dictionary<string, Dictionary<string, string>> cObjects, Drawing drw)
    {
    string h = "";         
    Dictionary<string, string> cObject = new Dictionary<string, string>();         
    int iObject = 0;         
    int iCode = 0;         
    string Key = "";
    string sEntidad = "";

    do {
     //Debug lpCode, lpValue

    if ( lpValue == "OBJECTS" ) ReadData();
        if ( lpValue == "ENDSEC" ) return;

        sEntidad = lpValue;
        iObject++;
        cObject = new Dictionary<string, string>();

        cObject.Add("0", sEntidad);
        iCode = 0;

         // Leo descentralizadamente las entidades
        ReadData();

         //If sEntidad = "HATCH" Then Stop
        while ( (lpCode != "0") && !fp.EndOfStream) {

            Key = lpCode;
            if ( cObject.ContainsKey(Key) )
            {
                do {
                    iCode += 1;
                    Key = lpCode + "_" + iCode.ToString();

                    
                } while ( !cObject.ContainsKey(Key) );
            }
            cObject.Add(Key, lpValue);
            ReadData();

        }
         //Object(cObject, cObject["5"])
        if ( cObject.ContainsKey("5") )
        {
            h = cObject["5"];
        }
        else
        {
            h = iObject.ToString();
        }
        cObjects.Add(h, cObject);

        if ( sEntidad == "ENDBLK" || sEntidad == "" ) return;

    } while  (!fp.EndOfStream);

}

private void Load7Thumbnail(Dictionary<string, string> cThumbnail)
    {
    int iCode = 0;         
    string Key = "";         

    do {

        if ( lpValue == "ENDSEC" ) return;

         // Leo descentralizadamente las entidades
        ReadData();

        while ( (lpCode != "0") && !fp.EndOfStream) {

            Key = lpCode;
            if ( cThumbnail.ContainsKey(Key) )
            {
                do {
                    iCode += 1;
                    Key = lpCode + "_" + iCode.ToString();

                   
                } while ( !cThumbnail.ContainsKey(Key) );    
            }
            cThumbnail.Add(Key, lpValue);
            ReadData();

        }

    } while ( !fp.EndOfStream );

}

public void ReconstructHandles(Drawing drw)
    {
    // TODO: Fix missing type definitions for DictEntry, DictList
    // Commented out code due to missing type definitions
    
    // Empiezo por los Bloques importantes
    foreach (var b2 in drw.Blocks)
    {
        Block b = b2.Value;
        if ( b.Name == "*Model_Space" )
        {
            b.idContainer = Handle(2);
            b.id = Handle();
        }
        else if ( b.Name.Length >= 6 && b.Name.Substring(0, 6) == "*Paper" )
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
     
    // TODO: The following code needs proper type definitions:
    // - DictEntry type is not defined
    // - DictList type is not defined  
    // - Drawing.Dictionary property is not defined
    // This code should be restored once the required types are available
    
    /*
    drw.Dictionary.id = Handle();
    var di = new DictEntry();
    di.Name = "ACAD_LAYOUT";
    di.id = Handle();
    drw.Dictionary.Definitions.Add(di.Name, di);

    foreach ( var b in drw.Blocks)
    {
        if ( b.Value.Sheet != null )
        {
            b.Value.Sheet.id = Handle();
            var item = new DictList();
            item.Name = b.Value.Sheet.Name;
            item.idSoftOwner = b.Value.Sheet.id;
            drw.Dictionary.Definitions["ACAD_LAYOUT"].items.Add(item);
        }
    }
    */

     // asigno handles a las Tables

}

public int SaveFile(string sName, Drawing drwToSave, bool LoadMinimal= false, bool SaveHeader= true, bool SaveTables= true, bool SaveBlocks= true, bool SaveThumbnail= true)
    {


    hFile = new StreamWriter(File.Open(sName, FileMode.Create));

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
    // Gcd.ResetChronograph(); // TODO: Fix this method call
    ReconstructHandles(drwToSave);
    // Inc Application.Busy;
    if ( SaveHeader )
    {
        Save1HeadersAndVarsDirect(drwToSave);
        Save2Classes(drwToSave);
    }
    if ( SaveTables )
    {
        Save3TablesDirect(drwToSave);
    }
    if ( SaveBlocks )
    {
        Save4BlocksDirect(drwToSave);
    }

    Save5EntitiesDirect(drwToSave);
    Save6Objects(drwToSave);
    if ( SaveThumbnail )
    {
        Save7ThumbNail(null);
    }

    hFile.Close();
    // Dec Application.Busy;
    Gcd.debugInfo(("Saved to ") + sName,false,false, true, true);
    return 0;
}

private int Save1HeadersAndVarsDirect(Drawing drwToSave)
    {
    hFile.WriteLine( "999" + "\n");
    hFile.WriteLine( "GambasCAD" + "\n");
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "SECTION" + "\n");
    hFile.WriteLine( "2" + "\n");
    hFile.WriteLine( "HEADER" + "\n");

     //Intento guardar algunas cosas utiles para cuando abra de nuevo este archivo
    // drw.Headers.CLAYER = drw.CurrLayer.Name; // Current LAYER

    // TODO: Fix ExportDXF method - not available on Headers
    // string[] stxHeaders = drw.Headers.ExportDXF();
    // foreach ( var sValue in stxHeaders)
    // {
    //     hFile.WriteLine( sValue + "\n");
    // }
    
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");
    
    return 0;
}

 // Las classes de cad no las usamos. En teoria, no tienen ninguna utilidad fuera de AutoCAD.
 // Abriendo un DXF, se guadaran todas las classes a efectos de recosntruir el 
private int Save2Classes(Drawing drwSaving)
    {


    // TODO: Implement CadClass functionality when type is available
    
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
    
    return 0;

}

private int Save3TablesDirect(Drawing drwToSave)
    {         

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
     //             e.pBlock.Name = "*D" & Str(iDimCounter)
     //             e.pBlock.idContainer = Handle()
     //             e.pBlock.id = Handle()
     //             For Each e2 In e.pBlock.entities
     //                 e2.IdContainer = e.pBlock.idContainer
     //                 e2.id = Handle()
     //             Next
     //             // Handle_Block_Record.Add(Handle(), e.pBlock.Name)
     //             // Handle_Block.Add(Handle(), e.pBlock.Name)
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
     //             // Handle_Block_Record.Add(Handle(), e.pBlock.Name)
     //             // Handle_Block.Add(Handle(), e.pBlock.Name)
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

    Save3TableViewPorts(drwToSave);
    Save3TableLineTypes(drwToSave);
    Save3TableLayers(drwToSave);
    Save3TableTextStyles(drwToSave);
    Save3TableViews(drwToSave);
    Save3TableAppID(drwToSave);
    Save3TableUCSs(drwToSave);
    Save3TableDimStyles(drwToSave);
    Save3TableBlockRecords(drwToSave);
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");
    
    return 0;
}

private int Save3TableAppID(Drawing drw)
    {
    string hTableHandle = Handle();

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
    // TODO: Add AppIDs collection to Drawing class
    hFile.WriteLine( "0" + "\n");

     // APPID - commented out until AppIDs is implemented
    // foreach ( var oneAppid in drw.AppIDs)
    // {
    //     hFile.WriteLine( "  0" + "\n");
    //     hFile.WriteLine( "APPID" + "\n");
    //     hFile.WriteLine( "  5" + "\n"); // handle propio
    //     hFile.WriteLine( Handle() + "\n"); //oneAppid.id
    //     hFile.WriteLine( "  330" + "\n"); // handle del padre
    //     hFile.WriteLine( hTableHandle + "\n");
    //     hFile.WriteLine( "  100" + "\n");
    //     hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
    //     hFile.WriteLine( "  100" + "\n");
    //     hFile.WriteLine( "AcDbRegAppTableRecord" + "\n");
    //     hFile.WriteLine( "  2" + "\n");
    //     hFile.WriteLine( oneAppid.APPName_2 + "\n");
    //     hFile.WriteLine( " 70" + "\n");
    //     hFile.WriteLine( oneAppid.Flags_70.ToString() + "\n");
    // }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");
    return 0;

}

private int Save3TableLayers(Drawing drw)
    {
    string hTableHandle = Handle();

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
    hFile.WriteLine( drw.Layers.Count.ToString() + "\n");         
    foreach (var layerPair in drw.Layers)
    {
        Layer oneLayer = layerPair.Value;

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
        // TODO: Fix boolean to int conversion for Frozen and Locked
        hFile.WriteLine( "0" + "\n"); // (-oneLayer.Frozen - oneLayer.Locked * 4).ToString()
        hFile.WriteLine( " 62" + "\n");
        hFile.WriteLine( oneLayer.Colour * (oneLayer.Visible ? 1 : -1) + "\n");
        hFile.WriteLine( "  6" + "\n");
        hFile.WriteLine( oneLayer.LineType?.Name ?? "CONTINUOUS" + "\n");
        hFile.WriteLine( "290" + "\n"); // plotting flag
        hFile.WriteLine( (oneLayer.Printable ? "1" : "0") + "\n");
        hFile.WriteLine( "370" + "\n"); // lit
        // TODO: Add Lit property to Layer class
        hFile.WriteLine( "0" + "\n"); // (oneLayer.Lit).ToString()
        hFile.WriteLine( "390" + "\n"); // plotstyle object
        hFile.WriteLine( " " + "\n");
        hFile.WriteLine( "347" + "\n"); // material
        hFile.WriteLine( " " + "\n");

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");
    
    return 0;
}

private int Save3TableTextStyles(Drawing drw)
    {
    string hTableHandle = Handle();

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
    hFile.WriteLine( (drw.TextStyles.Count).ToString() + "\n");         
    foreach (var textStylePair in drw.TextStyles)
    {
        TextStyle oneTextStyle = textStylePair.Value;

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
        hFile.WriteLine( oneTextStyle.Name + "\n");
        hFile.WriteLine( " 70" + "\n"); // flags, bit coded
        hFile.WriteLine( oneTextStyle.Flags + "\n");
        hFile.WriteLine( " 40" + "\n");
        hFile.WriteLine( (oneTextStyle.FixedH_40).ToString() + "\n");
        hFile.WriteLine( " 41" + "\n");
        hFile.WriteLine( (oneTextStyle.WidthFactor).ToString() + "\n");
        hFile.WriteLine( " 50" + "\n");
        hFile.WriteLine( (oneTextStyle.ObliqueAngle).ToString() + "\n");
        hFile.WriteLine( " 71" + "\n");
        hFile.WriteLine( (oneTextStyle.iDirection).ToString() + "\n");
        hFile.WriteLine( " 42" + "\n");
        hFile.WriteLine( (oneTextStyle.fLastHeightUsed_42).ToString() + "\n");
        hFile.WriteLine( "  3" + "\n");
        hFile.WriteLine( oneTextStyle.sFont_3 + "\n");
        hFile.WriteLine( "  4" + "\n");
        hFile.WriteLine( oneTextStyle.sBigFont_4 + "\n");

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");
    
    return 0;

}

private int Save3TableDimStyles(Drawing drw)
    {
    string hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.DimStyles)

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
    hFile.WriteLine( (drw.DimStyles.Count).ToString() + "\n");         
    foreach (var dimStylePair in drw.DimStyles)
    {
        DimStyle oneDimtStyle = dimStylePair.Value;

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
        hFile.WriteLine( oneDimtStyle.Name + "\n");
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
    
    return 0;
}

private int Save3TableLineTypes(Drawing drw)
    {
    string hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.LineTypes)

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
    hFile.WriteLine( (drw.LineTypes.Count).ToString() + "\n");         
    foreach (var lineTypePair in drw.LineTypes)
    {
        LineType oneLtype = lineTypePair.Value;

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
        SaveCode(70, oneLtype.Flags.ToString());
        SaveCode(3, oneLtype.Description);
        SaveCode(72, "65"); // para compatibilidad
        SaveCode(73, oneLtype.nTrames.ToString());
        SaveCode(40, oneLtype.Length.ToString());
        foreach (var fLength in oneLtype.TrameLength)
        {
            SaveCode(49, fLength.ToString());
        }

         // Hay tipos de linea mas complejos, que se generan con codigos que GambasCAD no maneja de momento

    }
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");
    
    return 0;
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
    // TODO: Implement UCSs collection on Drawing class and UCS type
    hFile.WriteLine( "0" + "\n");

    // UCS oneUCS ;
    // foreach (var oneUCS in drw.UCSs)
    // {
    //     hFile.WriteLine( "  0" + "\n");
    //     hFile.WriteLine( "UCS" + "\n");
    //     hFile.WriteLine( "  5" + "\n"); // handle propio
    //     hFile.WriteLine( Handle() + "\n");
    //     hFile.WriteLine( "  330" + "\n"); // handle del padre
    //     hFile.WriteLine( hTableHandle + "\n");
    //     hFile.WriteLine( "  100" + "\n");
    //     hFile.WriteLine( "AcDbSymbolTableRecord" + "\n");
    //     hFile.WriteLine( "  100" + "\n");
    //     hFile.WriteLine( "AcDbUCSTableRecord" + "\n");
    //     SaveCode(2, oneUCS.Name_2);
    //     SaveCode(70, oneUCS.Flags_70.ToString());
    //     SaveCode(10, oneUCS.OriginX_10.ToString());
    //     SaveCode(20, oneUCS.OriginY_20.ToString());
    //     SaveCode(30, oneUCS.OriginZ_30.ToString());
    //     SaveCode(11, oneUCS.XAxisX_11.ToString());
    //     SaveCode(21, oneUCS.XAxisY_21.ToString());
    //     SaveCode(31, oneUCS.XAxisZ_31.ToString());
    //     SaveCode(12, oneUCS.YAxisX_12.ToString());
    //     SaveCode(22, oneUCS.YAxisY_22.ToString());
    //     SaveCode(32, oneUCS.YAxisZ_32.ToString());
    //     SaveCode(79, "0");
    //     SaveCode(146, oneUCS.Elevation_146.ToString());
    //     SaveCode(346, oneUCS.BaseUCS_346);
    //     SaveCode(13, oneUCS.OriginForThisOrthographicTypeX_13.ToString());
    //     SaveCode(23, oneUCS.OriginForThisOrthographicTypeY_23.ToString());
    //     SaveCode(33, oneUCS.OriginForThisOrthographicTypeZ_33.ToString());
    // }
    
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDTAB" + "\n");
    
    return 0;

}

private int Save3TableViews(Drawing drw)
    {
    string hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.Views)

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
    hFile.WriteLine( drw.Views.Count.ToString() + "\n");         
    foreach (var oneView in drw.Views)
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
        foreach (var sData in oneView.Value.SData)
        {
            hFile.WriteLine( sData + "\n");
        }

    }
    hFile.WriteLine( "  0" + "\n");
            hFile.WriteLine("ENDTAB" + "\n");
    
    return 0;   

}

private int Save3TableViewPorts(Drawing drw)
    {
    string hTableHandle = Handle(); //Utils.FindItem(drw.Tables, drw.Viewports)

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
    hFile.WriteLine((drw.Viewports.Count).ToString() + "\n");         
    foreach ( var oneViewport in drw.Viewports)
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
        foreach (var sData in oneViewport.Value.ToString().Split('\n'))
        {
            hFile.WriteLine( sData + "\n");
        }

    }
    hFile.WriteLine( "  0" + "\n");
            hFile.WriteLine("ENDTAB" + "\n");
    return 0;   

}

private int Save3TableBlockRecords(Drawing drw)
    {


    // Block eBlock ;         
    // Sheet s ;         
    // Dictionary<string, Dictionary> cTable ;         
    // Dictionary<string, Dictionary> cTableEntry ;         
    // Dictionary<string, Dictionary> cVar ;         
    // Dictionary<string, Dictionary> cVars ;         
    // string sValues ;         
    // string lpclave ;         
    // string sHandle1 ;         
    // string sTableid ;         
    // string sBlockName ;         
    // string sBlockHandle ;         
    // int i ;         
    int iDimCounter =0;         
    // int iPaperSpaceCounter ;         
    // Block b ;         
    // Entity e ;         

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
    hFile.WriteLine( (drw.Sheets.Count + drw.Blocks.Count + iDimCounter).ToString() + "\n");

    foreach (var b2 in drw.Blocks)
            {
        var b = b2.Value;
        if ( b.idContainer == "" )
        {
            b.idContainer = Handle();
            b.id = Handle();
        }

        if ( b.Sheet != null)
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
        hFile.WriteLine( b.Name + "\n");
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
            hFile.WriteLine("ENDTAB" + "\n");
    return 0;

}

private int Save4BlocksDirect(Drawing drw)
    {


    // int i ;         
    // int iPaperSpaceCounter ;         
    // bool bCan ;         
    // int iii ;         
     Block eBlock ;         
    //  String[] stxEnty ;         
     Entity eEnty ;         
    // string sValues ;         
    // string lpclave ;         
    // string sBlockName ;         
    // Sheet s ;         
    // Dictionary<string, Dictionary> eBlocks ;         
    // Entity e ;         
    // Entity e2 ;         

    SaveCode(0, "SECTION");
    SaveCode(2, "BLOCKS");

    foreach (var eBlock2 in drw.Blocks)
            {
        eBlock = eBlock2.Value;

        SaveCode(0, "BLOCK");
        SaveCode(5, eBlock.id);
        SaveCode(330, eBlock.idContainer);
        SaveCode(100, "AcDbEntity");
        SaveCode(8, eBlock.Layer);
        SaveCode(100, "AcDbBlockBegin");
        SaveCode(2, eBlock.Name);
        SaveCode(70, eBlock.Flags);
        SaveCode(10, eBlock.x0.ToString());
        SaveCode(20, eBlock.y0.ToString());
        SaveCode(30, eBlock.z0.ToString());
        SaveCode(3, eBlock.Name);
        SaveCode(1, ""); // X ref path
        if ( (eBlock.entities.Count > 0) && (eBlock.Name != "*Model_Space") )
        {

            foreach (var eEnty2 in eBlock.entities)
                    {
                eEnty = eEnty2.Value;
                eEnty.id = Handle();
                DXFSaveCommonEntityData(eEnty);
                Gcd.CCC[eEnty.Gender].SaveDxfData(eEnty);

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
    return 0;   

}

private void DXFSaveCommonEntityData(Entity eEnty)
    {


    string sGender ;         

    sGender = eEnty.Gender;
    if ( sGender.IndexOf("DIMENSION_") > -1 ) sGender = "DIMENSION";
    SaveCode(0, sGender);
    SaveCode(5, eEnty.id);
    SaveCode(330, eEnty.Container.idContainer);
    SaveCode(100, "AcDbEntity");
    SaveCode(8, eEnty.pLayer.Name);
    SaveCode(6, eEnty.LType.Name);
    SaveCode(62, eEnty.Colour);
    SaveCode(370, eEnty.LineWidth.ToString());

}

private int Save5EntitiesDirect(Drawing drwToSAve)
    {


    Entity eEnty ;         
    Entity eBlock ;         
       
    bool bAttribPresent ;         
    Sheet s ;         
        

    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "SECTION" + "\n");
    hFile.WriteLine( "  2" + "\n");
    hFile.WriteLine( "ENTITIES" + "\n");
    foreach ( var s2 in drwToSAve.Sheets)
    {
                s = s2.Value;
         // here go all entities
        foreach ( var eEnty2 in s.Entities)
        {
            eEnty = eEnty2.Value;
            eEnty.id = Handle(); //FIXME: el problema es que ahora el Key en la Colection <> Id

            DXFSaveCommonEntityData(eEnty);
            Gcd.CCC[eEnty.Gender].SaveDxfData(eEnty);

            if ( eEnty.Gender == "INSERT" )
            {

                bAttribPresent = false;
                if ( (eEnty.pBlock.entities.Count > 0) )
                {
                    eEnty.pBlock.idContainer = eEnty.id;

                    foreach (var eBlock2 in eEnty.pBlock.entities)
                    {

eBlock = eBlock2.Value;
                                if (eBlock.Gender == "ATTRIB")
                                {
                                    bAttribPresent = true;

                                    eBlock.id = Handle();
                                    DXFSaveCommonEntityData(eBlock);
                                    Gcd.CCC[eBlock.Gender].SaveDxfData(eBlock);

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
                    foreach ( var eBlock2 in eEnty.pBlock.entities)
                            {
                        eBlock = eBlock2.Value;
                        eBlock.id = Handle();
                        DXFSaveCommonEntityData(eBlock);
                        Gcd.CCC[eBlock.Gender].SaveDxfData(eBlock);

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
             //     Gcd.debugInfo(("No puedo guardar este tipo de entidades ") & eEnty.Gender,false,false,true)
             // End If
        }
    }

     // end section code
    hFile.WriteLine( "  0" + "\n");
    hFile.WriteLine( "ENDSEC" + "\n");

    return 0;
 
}

private bool Save6Objects(Drawing drw)
    {
   

   

     // armo un diccionary, requerido por acad
    // var cObject = Dictionary<string, Dictionary>;
     // InsertCode(0, "Dictionary<string, Dictionary>", cObject)
     // InsertCode(5, drw.Dictionary<string, Dictionary>.id, cObject)
     // InsertCode(330, "0", cObject)
     // For Each de In drw.Dictionary<string, Dictionary>.Definitions
     //     InsertCode(3, de.Name, cObject)
     //     InsertCode(350, de.id, cObject)
     // Next
     //
     // cObjects.Add(cObject, "Dictionary<string, Dictionary>")
    var DictID = Handle(); 
    SaveCode(0, "SECTION");
    SaveCode(2, "OBJECTS");
    SaveCode(0, "DICTIONARY"); // Definiciones del diccionario
    SaveCode(5, DictID);
    SaveCode(330, 0);
    SaveCode(100, "AcDbDictionary>");
    SaveCode(280, 0);
    SaveCode(281, 1);
    foreach (var de2 in drw.Dictionaries)
    {
        var de = de2.Value;
        SaveCode(3, de.Name);
        SaveCode(350, de.Id);
    }

    foreach (var de2 in drw.Dictionaries)
    {
var de = de2.Value;
         // ahora las definiciones de cada diccionario
        SaveCode(0, "DICTIONARY");
        SaveCode(5, de.Id);
        SaveCode(330, DictID);
        SaveCode(100, "AcDbDictionary");
         //SaveCode(280, 0)
        SaveCode(281, 1);
        foreach (var dl in de.Items)
        {
            SaveCode(3, dl.Name);
            SaveCode(350, dl.IdSoftOwner);
        }

    }
     // SaveCode(2, "Dictionary<string, Dictionary>")
     // SaveCode(5, "B")
     // SaveCode(330, "A")
     // SaveCode(100, "AcDbDictionary<string, Dictionary>")
     // SaveCode(280, 0)
     // SaveCode(281, 1)

     // FIXME: reparar los papaerspaces
    // objLayout.ExportDXF(drw);
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
            hFile.WriteLine("ENDSEC" + "\n");
    return true;

}

private int Save7ThumbNail(Gtk.Image imgGLArea)
    {


   

    SaveCode(0, "SECTION");
    SaveCode(2, "THUMBNAILIMAGE");
     // cThumbs = cData["THUMBNAILIMAGE"]
     // If Not !(cThumbs) Then
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
    return 0;

}

 // Inserta un codigo en una coleccion considerando repeticiones de la Key
public void InsertCode(int iCode, string sData, Dictionary<string, string> cAcumulator)
    {


    string Key ;         
    int iCodeAux =0;         

    Key = iCode.ToString();
        if (cAcumulator.ContainsKey(Key))
        {
            do
            {
                iCodeAux += 1;
                Key = iCode.ToString() + "_" + iCodeAux.ToString();

                if (!cAcumulator.ContainsKey(Key)) break;

            } while (true);
        }
    cAcumulator.Add(sData.ToString(), Key);

}

 //   Helper para leer DXF: retorna la posicion en la que encontro la clave o -1 si no la encontro
 //   iCode = el codigo DXF
 //   stxClaves = array de claves DXF
 //   stxValues = array de valores DXF
 //   RetValue = el valor a retornar, pasado por referencia
 //   iStartPos = la posivion inicial en los array para la busqueda (def = 0)
 //   ExactPos = si se busca solo en la posicion inicial (def = false)
public int ReadCode(int iCode, string[] stxClaves, string[] stxValues,  ref string RetValue, int iStartPos= 0, bool ExactPos= false)
    {


    int i ;         
    int iMax ;         

    if ( iStartPos < 0 ) return iStartPos;
    // if ( stxClaves.Length != stxValues.Length )
    // {
    //     Debug "ReadCode: error, bad lists";
    //     return -1;
    // }
    if ( ExactPos ) { iMax = iStartPos; } else { iMax = stxClaves.Length; }
    for ( i = iStartPos; i <= iMax; i++)
    {
        if ( Gb.CInt(stxClaves[i]) == iCode )
        {
            // switch ( TypeOf(RetValue))
            // {
            //     case gb.Integer:
            //         RetValue = Gb.CInt(stxValues[i]);
            //         return i;
            //     case gb.Float:
            //         RetValue = float.Parse(stxValues[i]);
            //         return i;
            //     case gb.String:
                    RetValue = stxValues[i];
                    return i;
            //}

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

    public int ReadCodePlus(int iExpectedCode, List<string> stxClaves, List<string> stxValues, ref string RetValue, int iStartPos = 0, int iEscapeCode = -1, int iStartCode = -1)
    {


        int i;
        int iMax;
        bool StartOK = true;

        // if ( stxClaves.Length != stxValues.Length )
        // {
        //     Debug "ReadCode: error, bad lists";
        //     return -1;
        // }
        //If ExactPos Then iMax = iStartPos Else imax = stxClaves.Length
        if (iStartCode >= 0) StartOK = false;
        if (iStartPos < 0) return -1;
        for (i = iStartPos; i <= stxClaves.Count; i++)
        {
            // veo si proveyo un codigo inicial
            if (iStartCode >= 0)
            {

                if (Gb.CInt(stxClaves[i]) == iStartCode)
                {
                    StartOK = true;
                }
            }

            if (!StartOK) continue;

            if (Gb.CInt(stxClaves[i]) == iExpectedCode)
            {
                // switch ( TypeOf(RetValue))
                // {
                //     case gb.Integer:
                //         RetValue = CInt(stxValues[i]);

                //     case gb.Float:
                //         RetValue = float.Parse(stxValues[i]);
                //     case gb.Single:
                //         RetValue = float.Parse(stxValues[i]);

                //     case gb.String:
                RetValue = stxValues[i];

                // }

                return i + 1;
            }
            else if (Gb.CInt(stxClaves[i]) == iEscapeCode)
            {
                // switch ( TypeOf(RetValue))
                // {
                //     case gb.Integer:
                //         RetValue = 0;

                //     case gb.Float:
                //         RetValue = 0;
                //     case gb.Single:
                //         RetValue = 0;

                //     case gb.String:
                RetValue = "";

                //}

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

public int ReadCodePlusI(int iExpectedCode, List<string> stxClaves, List<string> stxValues, ref int RetValue, int iStartPos= 0, int iEscapeCode= -1, int iStartCode= -1)
    {
        string sRetValue = "";
        int res = ReadCodePlus(iExpectedCode, stxClaves, stxValues, ref sRetValue, iStartPos, iEscapeCode, iStartCode);
        if (res > 0)
        {
            RetValue = Gb.CInt(sRetValue);
        }
        return res;
    }   
public int ReadCodePlusD(int iExpectedCode, List<string> stxClaves, List<string> stxValues, ref double RetValue, int iStartPos= 0, int iEscapeCode= -1, int iStartCode= -1)
    {
        string sRetValue = "";
        int res = ReadCodePlus(iExpectedCode, stxClaves, stxValues, ref sRetValue, iStartPos, iEscapeCode, iStartCode);
        if (res > 0)
        {
            RetValue = Gb.CDbl(sRetValue);
        }
        return res;
    }   




    // Lee el codigo de la coleccion que se importa del DXF, puede ignorar lo que esta entre llaves {} que es info de ACAD privativa y puede empezar desde el ultimo leido antes
    public int GoToCodeFromCol(Dictionary<string, string> cDxfEntityData, int iCode, string sValue)
    {


        string s;
        string sKey;
        int i = 0;


        foreach (var s2 in cDxfEntityData)
        {
            i++;
            s = s2.Value;
            sKey = s2.Key;

            if (s == sValue)
            {
                LastCodeReadIndex = i;
                return i;
            }
        }
        return 0;

    }

 // Lee el codigo de la coleccion que se importa del DXF, puede ignorar lo que esta entre llaves {} que es info de ACAD privativa y puede empezar desde el ultimo leido antes
public string ReadCodeFromCol(Dictionary<string, string> cDxfEntityData, int iCode, bool ReadNext= false, bool IgnoreAcadData= true, string vDefaultValue= "")
    {


    string sKey = "";
    int i = 0;
    int p = 0;
    bool valid = false;
    bool OpenedSection = false;

    foreach (var s2 in cDxfEntityData)
    {
                i++;
               var s = s2.Value;

        if ( ReadNext )
        {
            if ( i < LastCodeReadIndex )
            {
                valid = false;
            }
            else
            {
                valid = true;
            }
        }
        else
        {
            valid = true;

        }

        if ( Gb.Left(s, 1) == "{" )
        {
            OpenedSection = true;

        }

        if ( Gb.Left(s, 1) == "}" && OpenedSection )
        {
            OpenedSection = false;

        }

        if ( valid && ! OpenedSection )
        {
            sKey = s2.Key;
             // elimino el posible _
            p = Gb.InStr(sKey, "_");
            if ( p > 0 )
            {
                sKey = Gb.Left(sKey, p - 1);
            }
            if (Gb.CInt(sKey) == iCode)
            {
                LastCodeReadIndex = i;
                return s;
            }
        }

    }

    return vDefaultValue.ToString();

}

public void SaveCode(int iCode, object arg)
        {
        string sValue = "" ;
if (arg is string s)
        {
            sValue = s;
        }
        else if (arg is int i)
        {
            sValue= i.ToString();
        }
        else if (arg is double d)
        {
            sValue = d.ToString();
        }

   

    hFile.WriteLine( iCode.ToString() + "\n");
  
    hFile.WriteLine(sValue + "\n");

}

public void SaveCodeInv(string sValue, int sCode)
    {


    SaveCode(sCode, sValue);

}

private void SaveColection(Dictionary<string, string> cData)
    {
     
         var lpclave = "" ;
    int i ;         

    foreach (var s2 in cData)
            {
        var s = s2;
         // elimino el posible _
       
        i = Gb.InStr(s.Key, "_");
        if ( i > 0 )  lpclave = Gb.Left(s.Key, i - 1);
        SaveCode(Gb.CInt(lpclave), s.Value);
    }

}

    // // Adds a  object to object by handle Dictionary<string, Dictionary>
    // Private Sub Object(drw As Drawing, o As string, sHandle As String)
    //
    //     If sHandle = "" Then Return
    //
    //     If drw.Handles.ContainsKey(sHandle) Then
    //         Gcd.debugInfo("WARNING: Handle repedida " & sHandle)
    //     Else
    //
    //         drw.handles.Add(o, sHandle)
    //     End If
    //
    // End

    // Reads layers Dictionary<string, Dictionary> and puts data in oLayers
    public void ReadViewports(Dictionary<string, Dictionary<string, string>> cVptData, Drawing drw)
    {


        // Viewport v ;         

        //  // // primero eliminamos lo q haya
        // if ( ! cVptData["TABLES"].ContainsKey("VPORT") ) return;
        // drw.Viewports.Clear;
        // foreach (var cViewp  in cVptData["TABLES"]["VPORT"])
        // {
        //     v =  Viewport;
        //      // hLay.Name = cLay[codName]
        //      // hLay.Visible = CInt(cLay[codColor]) >= 0
        //      // hLay.Colour = Abs(CInt(cLay[codColor]))
        //      // hLay.handle = cLay[codHandle]
        //      // If hLay.handle = "" Then hLay.handle = Gcd.Handle()()
        //      // drw.oLayers.Add(hLay, hLay.handle)
        // }

        //  // // es inaceptable no tener al menos un layrr
        //  // If drw.oLayers.Count = 0 Then
        //  //     hLay =  Layer
        //  //     hLay.Name = "0"
        //  //     hLay.Visible = true
        //  //     hLay.Colour = 0
        //  //     hLay.handle = Gcd.Handle()()
        //  //     drw.oLayers.Add(hLay, hLay.handle)
        //  // Endif
        //  //
        //  // // aprovecho para setear el layer actual
        //  // drw.CurrLayer = drw.oLayers[drw.oLayers.First]

    }

public void ReadBlockRecords(Dictionary<string, Dictionary<string, string>> cBlockRecs, Drawing drw)
    {
        Block b;

        // primero eliminamos lo q haya
        drw.Blocks.Clear();

        foreach (var cBlockRec2 in cBlockRecs)
        {
            var cBlockRec = cBlockRec2.Value;
            b = new Block();
            b.Name = cBlockRec[codName];
            b.idContainer = cBlockRec[codid];
            if (b.idContainer == "") b.idContainer = Gcd.NewId();
            b.InsertUnits = Gb.CInt(cBlockRec["70"]);
            b.Explotability = Gb.CInt(cBlockRec["280"]);
            b.Scalability = Gb.CInt(cBlockRec["281"]);
            if (cBlockRec.ContainsKey("340")) b.idAsociatedLayout = cBlockRec["340"];
            // If b.idAsociatedLayout <> "0" Then
            //     hContainers.Add(b, b.idAsociatedLayout)
            // Else
            hContainers.Add(b.idContainer, b.entities);
            // End If

            drw.Blocks.Add(b.Name, b);
        }

    }

    // Reads layers Dictionary<string, Dictionary> and puts data in oLayers
    public void ReadLayers(Dictionary<string, Dictionary<string, string>> cLays, Drawing drw)
    {


        Layer hLay;

        // // primero eliminamos lo q haya
        drw.Layers.Clear();

        foreach (var cLay2 in cLays)
        {
            var cLay = cLay2.Value;

            hLay = new Layer();
            hLay.Name = cLay[codName];
            hLay.id = cLay[codid];
            hLay.Visible = Gb.CInt(cLay[codColor]) >= 0;
            hLay.Colour = Gb.Abs(Gb.CInt(cLay[codColor]));
            hLay.LineType = drw.LineTypes[cLay[codLType]];

            try { hLay.LineWt = Gb.CInt(cLay["370"]); } catch { hLay.LineWt = 1; }  // algunos dxf no traen esta info


            if (hLay.id == "") hLay.id = Gcd.NewId();
            drw.Layers.Add(hLay.Name, hLay);
        }
        // TODO: poner lo que sigue en otro lado

        // es inaceptable no tener al menos un layer
        if (drw.Layers.Count == 0)
        {
            hLay = new Layer();
            hLay.Name = "0";
            hLay.Visible = true;
            hLay.Colour = 0;
            hLay.LineType = drw.LineTypes.First().Value;
            hLay.id = Gcd.NewId();
            drw.Layers.Add(hLay.Name, hLay);
        }

        // o mejor este, pero puede fallar
        try
        {
            drw.CurrLayer = drw.Layers[drw.Headers.CLAYER];
        }
        catch
        {
            drw.CurrLayer = drw.Layers.First().Value;
        }

    }

    // Reads Styles and DimStyles Dictionary<string, Dictionary> and puts data in arrStyles
    public void ReadStyles(Dictionary<string, Dictionary<string, string>> cStyles, Drawing drw)
    {

        TextStyle hlty;
        int t;
        int i;
        double fTrameLength;
        string sH2;
        string sNextKey;
        TextStyle RefStyle;
        DimStyle hdim;
        string n = "";

        // no mas porque leo las tables de a una----> primero eliminamos lo q haya
        drw.TextStyles.Clear();
        // Leo los styles de texto
        foreach (var c2 in cStyles)
        {
            var c = c2.Value;

            hlty = new TextStyle();

            hlty.Name = (c[codName].ToLower());
            hlty.Id = c[codid];
            if (hlty.Id == "") hlty.Id = Gcd.NewId();
            hlty.sFont_3 = (c["3"].ToLower());

            hlty.FixedH_40 = float.Parse(c["40"]);

            // Esto no puede usarse asi, LastHeightUsed_2 es solo un dato de historial
            // If hlty.FixedH_40 = 0 Then hlty.FixedH_40 = CFloat(c["42"])

            if (hlty.Name != "")
            {
                n = hlty.Name.ToLower();
            }
            else if (hlty.sFont_3 != "")
            {
                n = Gb.FileWithoutExtension(hlty.sFont_3);

            }

            drw.TextStyles.Add(n, hlty);

        }

            drw.CurrTextStyle = drw.TextStyles.First().Value;
        
    }
    public void ReadDimStyles(Dictionary<string, Dictionary<string, string>> cDims, Drawing drw)
    {

        // Leo lo styles de dimensiones

        var hdim = new DimStyle();

        foreach (var c2 in cDims)
        {
            var c = c2.Value;

            hdim.Name = c[codName];

            if (c.ContainsKey("105"))
            {
                hdim.id = c["105"];
            }
            else
            {
                hdim.id = Gcd.NewId();
            } // depre

            try { hdim.DIMSCALE = float.Parse(c["40"]); } catch { hdim.DIMSCALE = 1; }
            if (hdim.DIMSCALE == 0) hdim.DIMSCALE = 1;

            try { hdim.DIMASZ = float.Parse(c["41"]); } catch { hdim.DIMASZ = 1; }
            if (hdim.DIMASZ == 0) hdim.DIMASZ = 1;

            try { hdim.DIMTXT = float.Parse(c["140"]); } catch { hdim.DIMTXT = 1; }
            if (hdim.DIMTXT == 0) hdim.DIMTXT = 1;

            try { hdim.DIMTXSTY = c["340"]; } catch { hdim.DIMTXSTY = ""; }

            // try { hdim.DIMBLK = cData["TABLES"]["BLOCK_RECORD"][c["341"]]["2"]; } catch { hdim.DIMBLK = ""; }
            if (hdim.DIMBLK == "") hdim.DIMBLK = "_" + Gcd.Drawing.Headers.DIMBLK;
            // try { hdim.DIMBLK1 = cData["TABLES"]["BLOCK_RECORD"][c["343"]]["2"]; } catch { hdim.DIMBLK1 = ""; }
            if (hdim.DIMBLK1 == "") hdim.DIMBLK1 = "_" + Gcd.Drawing.Headers.DIMBLK1;
            // try { hdim.DIMBLK2 = cData["TABLES"]["BLOCK_RECORD"][c["344"]]["2"]; } catch { hdim.DIMBLK2 = ""; }
            if (hdim.DIMBLK2 == "") hdim.DIMBLK2 = "_" + Gcd.Drawing.Headers.DIMBLK2;

            if (hdim.DIMTXSTY != "")
            {
                var RefStyle = Gcd.FindStyleById(hdim.DIMTXSTY);
                if (!(RefStyle == null))
                {
                    if (RefStyle.FixedH_40 > 0) hdim.DIMTXT = RefStyle.FixedH_40;
                    hdim.DIMTXSTY = RefStyle.sFont_3;
                }
            }

            if (hdim.Name != "") drw.DimStyles.Add(hdim.Name, hdim);
        }
        

        drw.CurrDimStyle = drw.DimStyles.First().Value;
    }



 // Reads LineTypes Dictionary<string, Dictionary> and puts data in arrLTypes
public void ReadLTypes(Dictionary<string, Dictionary<string, string>> cLtypes, Drawing drw)
    {

    LineType hlty;
    int t;
    int i;
    int ri;
    double fTrameLength;
    string sNextKey;
    string r;
    bool AbsoluteRotation ;         
    bool IsText ;         
    bool IsShape ;

        // primero eliminamos lo q haya
        drw.LineTypes.Clear();
        foreach (var c2 in cLtypes)
        {
            var c = c2.Value;

            hlty = new LineType();
            hlty.Name = c[codName].ToUpper();
            hlty.Description = c["3"];
            if (c.ContainsKey("5")) hlty.id = c["5"];
            if (hlty.id == "") hlty.id = Gcd.NewId();
            try { hlty.nTrames = Gb.CInt(c["73"]); } catch { hlty.nTrames = 0; }
            if (hlty.nTrames > 0) hlty.Length = float.Parse(ReadCodeFromCol(c, 40));
            i = 0;
            for (t = 1; t <= hlty.nTrames; t++)
            {

                r = ReadCodeFromCol(c, 49, true);
                hlty.TrameLength.Add(float.Parse(r));
                ri = Gb.CInt(ReadCodeFromCol(c, 74, true, true, "0"));
                hlty.TrameType.Add(ri);
                switch (ri)
                {
                    case 0:
                    // nada
                    default:
                        if ((ri & 1) == 1) { AbsoluteRotation = true; } else { AbsoluteRotation = false; }
                        if ((ri & 2) == 2) { IsText = true; } else { IsText = false; }
                        if ((ri & 4) == 4) { IsShape = true; } else { IsShape = false; }
                        hlty.TrameData.Add(ReadCodeFromCol(c, 75, true));
                        hlty.TrameStyle.Add(ReadCodeFromCol(c, 340, true));
                        hlty.TrameScale.Add((float)Gb.CDbl(ReadCodeFromCol(c, 46, true)));
                        hlty.TrameRotation.Add((float)Gb.CDbl(ReadCodeFromCol(c, 50, true)));
                        hlty.TrameOffX.Add((float)Gb.CDbl(ReadCodeFromCol(c, 44, true)));
                        hlty.TrameStyle.Add(ReadCodeFromCol(c, 45, true));
                        hlty.TrameData.Add(ReadCodeFromCol(c, 9, true));
                        break;
                }

            }

            drw.LineTypes.Add(hlty.Name, hlty);

        }
        if (drw.LineTypes.Count == 0)
        {
            hlty = new LineType();
            hlty.Name = "CONTINUOUS";
            hlty.Description = "";
            hlty.id = Gcd.NewId();
            hlty.nTrames = 0;
            drw.LineTypes.Add(hlty.Name, hlty);

        }

    drw.CurrLineType = drw.LineTypes.First().Value;

}

    public void ImportBlocksFromDXF(Dictionary<string, Dictionary<string, string>> cBlocks, Drawing drw) //, obxEntities As Entity[]) As Integer
    {



        Dictionary<string, Dictionary<string, string>> colBlk;

        int i = 0;


        Dictionary<string, Dictionary<string, string>> cTables;
        Dictionary<string, Dictionary<string, string>> cBlockRecord;



        // if (colData.ContainsKey("BLOCKS")) cBlocks = colData["BLOCKS"];
        // if (colData.ContainsKey("TABLES"))
        // {
        //     cTables = colData["TABLES"];
        //     if (colData["TABLES"].ContainsKey("BLOCK_RECORD")) cBlockRecord = cTables["BLOCK_RECORD"];
        // }
        // if (!cBlocks) return;
        // For Each colBlk In colData["TABLES"]["BLOCK_RECORD"]
        //     Dim Block As  Block
        //     Block.entities = Dictionary<string, Dictionary>
        //     Block.Name = colBlk[codName]
        //     Block.handle = colBlk[codHandle]
        //     Block.HandleOwnerParent = colBlk[codHandleOwner]
        //     Block.HandleAsociatedLayout = colBlk["340"]
        //     try Block.InsertUnits = colBlk["70"]
        //     try Block.Explotability = colBlk["280"]
        //     try Block.Scalability = colBlk["281"]
        //     drw.oBlocks.Add(Block, Block.handle)
        //
        // Next
        // hay DXF sin TAbles
        // if (cTables)
        // {

        //     foreach (var colBlk in cBlocks)
        //     {
        //         Block = new Block();
        //         Block.id = colBlk[codid];
        //         Block.entities = new Dictionary<string, Entity>();
        //         Block.Name = colBlk[codName];
        //         Block.layer = colBlk[codLayer];
        //         try { Block.x0 = colBlk[codX0]; } catch { Block.x0 = 0; }
        //         try { Block.y0 = colBlk[codY0]; } catch { Block.y0 = 0; }
        //         try { Block.z0 = colBlk[codZ]; } catch { Block.z0 = 0; }
        //         try { Block.flags = colBlk["70"]; } catch { Block.flags = 0; }


        //         if (cBlockRecord)
        //         {
        //             if (cBlockRecord.ContainsKey(colBlk["330"]))
        //             {
        //                 Block.idContainer = cBlockRecord[colBlk["330"]]["5"];
        //                 Block.idAsociatedLayout = cBlockRecord[colBlk["330"]]["340"];
        //                 // If Block.idAsociatedLayout <> "0" Then
        //                 //     hContainers.Add(Block, Block.idAsociatedLayout)
        //                 // Else
        //                 hContainers.Add(Block, Block.idContainer);
        //                 // End If
        //             }
        //         }
        //         //If Left(Block.Name, 1) = "*" Then    // puede ser una Dim o una Sheet
        //         // If InStr(Block.Name, "_Space") > 0 Then
        //         //     // es una sheet, que ya fue creada por ReadObjectsFromDXF
        //         // Else

        //         drw.Blocks.Add(Block, Block.Name);

        //         //Endif

        //         //Endif

        //     }
        // }


        // foreach (var colBlk2 in cBlocks)
        // {
        //     colBlk = colBlk2.Value;

        //     if (colBlk.ContainsKey("entities"))
        //     {

        //         DXFtoEntity(colBlk["entities"], drw, drw.Blocks[colBlk["2"]]);

        //     }
        //     i++;

        // }

    }
    

    

public static void SecondPass(Drawing drw)
    {
        // me aseguro que exista el bloque Model_Space
         if ((drw.Blocks.Count == 0) || !drw.Blocks.ContainsKey("*Model_Space")) // agrego el Model
        {

            // lo agrego a los bloques
            Block b = new Block();
            b.Name = "*Model_Space";
            b.entities = new Dictionary<string, Entity>();
            b.idContainer = Gcd.NewId();
            b.id = Gcd.NewId();
            b.idAsociatedLayout = Gcd.NewId();
            b.IsAuxiliar = true;
            b.IsReciclable = false;
            drw.Blocks.Add(b.Name, b);
        }

        // voy a hacer un chequeo final, porque algunos DXF vienen sin el bloque Model
        if (!drw.Sheets.ContainsKey("Model"))
        {
            // creo la Sheet
            Sheet S = new Sheet();
            S.Name = "Model";
            S.IsModel = true;
            S.Block = drw.Blocks["*Model_Space"];
            S.id = drw.Blocks["*Model_Space"].idAsociatedLayout;
            drw.Sheets.Add("Model", S);
            drw.Sheet = S;
            drw.Model = S;

        }

        // // ahora vinculo las Sheets con su bloque
        foreach (var s2 in drw.Sheets)
        {
            Sheet s = s2.Value;
            foreach (var b2 in drw.Blocks)
            {
                Block b = b2.Value;
                if (b.idAsociatedLayout == s.id)
                {
                    s.Block = b;
                    s.Entities = b.entities;
                    b.Sheet = s;
                    break;
                }
            }
        }
    }

    public void SetViewports(Dictionary<string, Dictionary<string, string>> cDxfData, Drawing drw)
    {


        Entity e;
        Viewport v;
        string n;
        string hLayout;
        Sheet s;
        Block b;

        foreach (var b2 in drw.Blocks)

        {
            b = b2.Value;
            if (drw.Sheets.ContainsKey(b.idAsociatedLayout))
            {
                foreach (var e2 in b.entities)
                {
                    e = e2.Value;
                    drw.Sheets[b.idAsociatedLayout].Entities.Add(e.id, e);
                    // try n = cDxfData["BLOCKS"][b.Name]["2"]                      // nombre del bloque
                    // try hLayout = cDxfData["TABLES"]["BLOCK_RECORD"][n]["340"]
                    // If hLayout <> "" Then
                    //     s.Entities = b.entities
                    //
                    //
                }
            }

        }
        foreach (var s2 in drw.Sheets)
        {
            s = s2.Value;
            foreach (var e2 in s.Entities)
            {
                e = e2.Value;
                if (e.Gender == "VIEWPORT")
                {
                    // s.Viewports.Add(e.pBlock, e.id); TODO ver q onda
                    //cadViewport.SetViewport(e, s)
                }
            }
            //Gcd.Drawing.Sheet = s
            //cadZoomE.Start()
        }

    }

 // Importa las cosas de manera descentralizada
public static void DXFtoEntity(Drawing drw, Dictionary<string, Dictionary<string, string>> cDxfEntities, Dictionary <string,Entity> entis )
    {


    Dictionary<string, string> e ;         
    Dictionary<string, Entity> obx =new Dictionary<string, Entity>() ;         
     
    Entity entNueva ;         
    bool flgIsPolyline ;         
    bool IsDummy ;         
        

    double fTime ;         
            
  

    foreach (var e2 in cDxfEntities) // Para cada Coleccion de datos de vrx
        {
        e = e2.Value;
        if ( e.ContainsKey(codEntity) ) // es una entidad?
        {
                // entonces, creamos una nueva
                // poner en minuscula para anular la entidad
                if (Gb.InStr("VIEWPORT LEADER HATCH POLYLINE ENDBLK SEQEND VERTEX POINT ATTDEF ATTRIB LINE LWPOLYLINE CIRCLE ELLIPSE ARC TEXT MTEXT SPLINE SOLID INSERT DIMENSION DIMENSION_LINEAR DIMENSION_DIAMETEr DIMENSION_RADIUs DIMENSION_ANG3Pt DIMENSION_ALIGNED DIMENSION_ORDINATE LARGE_RADIAL_DIMENSION ARC_DIMENSION MLINE", e[codEntity].ToUpper())>0 ) { IsDummy = true; } else { IsDummy = false; }

            if ( e[codEntity].ToUpper() == "ENDBLK" ) continue;

            if ( IsDummy )
            {
                 // no esta implementada
                Gcd.debugInfo("Entidad no implementada o con errores: " + e[codid] + "," + e[codEntity]);

            }
            else
            {

               // var t = System.Threading.Timer;

                entNueva = clsEntities.DXFImportToEntity(drw, e, IsDummy);

                if ( !(entNueva==null) ) continue; // si esta implementada, llenamos los datos
                if ( entNueva.Gender == "ENDBLK" ) continue;

                    //stats
                    // if ( ! ReadTimes.ContainsKey(entNueva.Gender) ) ReadTimes.Add(fTime, entNueva.Gender);
                    // if ( ! ReadEntities.ContainsKey(entNueva.Gender) ) ReadEntities.Add(1, entNueva.Gender);

                    //Debug "Contenedor", hContainer
                    // -hBlock-Record
                    // -hInsert
                    // -hHatch               OTRA ENTIDAD que tenga .pBlock.enities as Dictionary<string, Dictionary>
                    // -Polyline

                    // if ( bContainer ) entNueva.Container = bContainer;
                    // if ( entNueva.Container )
                    // {
                    //     obx = entNueva.Container.entities;
                    // }
                    // else if ( bContainer )
                    // {
                    //     obx = bContainer.entities; //drw.Blocks[entNueva.IdContainer]
                    //      // Else If hContainers.ContainsKey(entNueva.idContainer) Then
                    //      //     obx = hContainers[entNueva.idContainer].entities
                    // }
                    // else
                    // {
                    //      //If Not obx Then
                    //     obx = drw.Sheet.Entities;
                    //     entNueva.Container = drw.Sheet.Block;
                    // }

                    if ("HATCH INSERT POLYLINE POLYLINE_2D".Contains(entNueva.Gender))
                    {

                        if (!(entNueva.pBlock == null))
                        {
                            obx = entNueva.pBlock.entities;
                        }
                        // if ( entNueva.Gender == "POLYLINE" ) // TODO, ver 3D
                        // {
                        //     if ( (entNueva.iParam[cadPolyline.iiiPolylineType] && 64) == 64 )
                        //     {
                        //         drw.Has3dEntities = true;
                        //     }
                        // }
                    }
                    else
                    {
                        obx = entis;
                       
                    }
                    //End If

                    if (e[codEntity] != "SEQEND")
                    {
                        if (!(obx == null)) obx.Add(entNueva.id, entNueva);
                    }
                    else
                    {
                        // es el fin de una polilinea
                    }   

                 // //Gcd.debugInfo("Leida entidad tipo" & entNueva.Gender & " id " & entNueva.id,false,false,true)
                 // If e[codEntity] = "POLYLINE" Then //Stop
                 //     flgIsPolyline = true
                 //     //obx = entNueva.pBlock
                 //     // pBlockPolyline =  Block
                 //     // pBlockPolyline.entities = Dictionary<string, Dictionary>
                 //     //
                 //     // entNueva.pBlock = pBlockPolyline
                 // End If
                 // If (e[codEntity] = "SEQEND") And (flgIsPolyline = true) Then
                 //     flgIsPolyline = false
                 //     obx.Remove(obx.Last)
                 //     obx = null
                 //
                 // End If

                // fTime = (Timer - t);
                // ReadTimes[entNueva.Gender] += fTime;

                // ReadEntities[entNueva.Gender] = ReadEntities[entNueva.Gender] + 1;

                 // If entNueva.Gender = "HATCH" Then
                 //     ReadTimes.Add(fTime, entNueva.Handle)
                 // Endif
                 // If entNueva.HandleOwner = "640" Then Debug "640", entNueva.Gender

                entNueva = null; // limpiamos

            }
        }
    }

}

 // Transforma una coleccion en dos array de strings
public void DigestColeccion(Dictionary<string, string> c,  ref List<string> sClaves, ref List<string> sValues)
    {


    string lpValue ;         
    string lpclave ;         
    int i ;         

    sClaves.Clear();
    sValues.Clear();

    foreach (var lpValue2 in c)
        {
        lpValue = lpValue2.Value;
         // elimino el posible _
        lpclave = lpValue2.Key;
        i = Gb.InStr(lpclave, "_");
        if ( i > 0 ) lpclave = Gb.Left(lpclave, i - 1);
        sClaves.Append(lpclave); // el codigo es el tipo de variable
        sValues.Append(lpValue);

    }

}

public void ReadObjectsFromDXF(Dictionary<string, Dictionary<string, string>> cData, Drawing drw)
    {


    Dictionary<string, string> cObject ;         
    Entity entNueva ;         
    List <string> sClaves=[] ;         
    List <string> sValues =[];         
    Sheet s ;         
    MLineStyle m ;         
    DictEntry entry ;         
    int i ;         
    int flags =0;         
    int i2 ;
        string sFlags = "";
    Gcd.debugInfo("Importing DXF object data",false,false,true);
     //Handle_Layout = Dictionary<string, Dictionary>
    if ( ! cData.ContainsKey("OBJECTS") ) return;
    foreach (var cObject2 in cData)
    {
        cObject = cObject2.Value;
        if ( cObject["0"] == "Dictionary<string, Dictionary>" )
        {

            this.DigestColeccion(cObject, ref sClaves, ref sValues);
            entry =  new DictEntry();
             //entry.idSoftOwner = ReadCodeFromCol(cObject, 330, true)
            entry.Name = ReadCodeFromCol(cObject, 3, true); // Name
            entry.IdSoftOwner = ReadCodeFromCol(cObject, 330, true);

        }

        if ( cObject["0"] == "LAYOUT" )
        {

            DigestColeccion(cObject, ref sClaves, ref sValues);
            s =  new Sheet();
            // objLayout.importDXF(s, cObject); TODO 
            if ( s.Name == "" ) s.Name = s.pPrintStyle.ViewName;
            if ( s.Name == "" ) s.Name = "Sheet " + drw.Sheets.Count.ToString();
            drw.Sheets.Add(s.Name,s);
             //Handle_Layout.Add(s.Name, s.id)
             //hContainers.Add(s.Entities, s.id)
            if ( s.Name.ToLower() == "model" )
            {
                s.IsModel = true;
                drw.Sheet = s;
                drw.Model = s;

            }

        }

        if ( cObject["0"] == "MLINESTYLE" )
        {

            DigestColeccion(cObject, ref sClaves, ref sValues);
            m = new MLineStyle();
            i = ReadCodePlus(2, sClaves, sValues, ref m.Name, 0);
            i = ReadCodePlusI(70, sClaves, sValues, ref flags, i);
            m.FillOn = flags.IsBitSet(1);
            m.ShowMiters = flags.IsBitSet(2);
            m.StartSquareCap = flags.IsBitSet(5);
            m.StartInnerArc = flags.IsBitSet(6);
            m.StartRound = flags.IsBitSet(7);
            m.EndSquareCap = flags.IsBitSet(9);
            m.EndInnerArc = flags.IsBitSet(10);
            m.EndRound = flags.IsBitSet(11);

            i = ReadCodePlus(3, sClaves, sValues, ref m.Description, i);

            i = ReadCodePlusI(62, sClaves, sValues, ref m.FillColor, i);

            i = ReadCodePlusD(51, sClaves, sValues, ref m.StartAngle, i);

            i = ReadCodePlusD(52, sClaves, sValues, ref m.EndAngle, i);
            i = ReadCodePlusI(71, sClaves, sValues, ref m.Elements, i);

            Array.Resize(ref m.ElemOffset, m.Elements);
            Array.Resize(ref m.ElemColor, m.Elements);
            Array.Resize(ref m.ElemLinetype, m.Elements);

            for ( i2 = 1; i2 <= m.Elements; i2++)
            {
                i = ReadCodePlusD(49, sClaves, sValues, ref m.ElemOffset[i2 - 1], i);
                i = ReadCodePlusI(62, sClaves, sValues, ref m.ElemColor[i2 - 1], i);
                i = ReadCodePlus(6, sClaves, sValues, ref m.ElemLinetype[i2 - 1], i);
                if ( (m.ElemOffset[i2 - 1] > 0) && (m.JustificationTop < m.ElemOffset[i2 - 1]) ) m.JustificationTop = m.ElemOffset[i2 - 1];
                if ( (m.ElemOffset[i2 - 1] < 0) && (m.JustificationBottom > m.ElemOffset[i2 - 1]) ) m.JustificationBottom = m.ElemOffset[i2 - 1];
            }
            drw.MLineStyles.Add(m.Name, m);

        }

    }

}

 // busca un bloque con ese block record
private  Block GetBlock(Dictionary<string, Block> cBlocks, string hBlockRecord)
    {


            

    foreach ( var b in cBlocks)
    {
        if ( b.Value.idContainer == hBlockRecord ) return b.Value;

    }

    return null;

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
    return iHandle.ToString("X");

    }
// TODO: reparar
// public bool SetValues(string sVarName, string[] vValue)
//     {


//      float[] slx =[];         
//      int[] inx =[];         
//     int Tipo ;         
//     string v ;         
//     string sVarName2 ;

//         if (IsNumber(Left(sVarName, 1))) { sVarName2 = "__" + sVarName; } else { sVarName2 = sVarName; }  
    

//     if ( vValue.Count > 1 )
//     {
//          // determino el tipo de array
//         tipo = Object.GetProperty(Me, "_" + sVarName);
//         if ( (tipo > 9) && (tipo < 70) )
//         {
//             foreach ( var v in vValue)
//             {
//                 slx.Add(v);
//             }
//             Object.SetProperty(Me, sVarName2, slx);
//         }
//         else
//         {
//             foreach (var  v in vValue)
//             {
//                 inx.Add(v);
//             }
//             Object.SetProperty(Me, sVarName2, inx);
//         }

//     }
//     else
//     {

//         Object.SetProperty(Me, sVarName2, vValue[0]);

//     }
//     return true;
// Catch;
//     return false;

// }

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
//         stx.Append("9");
//         stx.Append("$" + var);

//          // Verifying that it is a property or a variable.
//         if ( (MyClass[var].kind == Class.Variable) || (MyClass[var].kind == Class.Property) )
//         {
//             tipo = Object.GetProperty(obj, "_" + var);
//             if ( MyClass[var].Type == "Single[]" || MyClass[var].Type == "Integer[]" ) //is an array
//             {

//                 if ( tipo == 10 ) // es un array
//                 {
//                     slx = Object.GetProperty(Me, var);
//                     stx.Append("10");
//                     if ( slx.Count >= 1 ) stx.Append(slx[0]) else stx.Append(0);
//                     stx.Append("20");
//                     if ( slx.Count >= 2 ) stx.Append(slx[1]) else stx.Append(0);
//                     if ( slx.Count == 3 )
//                     {
//                         stx.Append("30");
//                         if ( slx.Count >= 3 ) stx.Append(slx[2]) else stx.Append(0);
//                     }
//                 }
//                 else if ( tipo == 70 )
//                 {
//                     inx = Object.GetProperty(Me, var);
//                     foreach (var iVal in inx)
//                     {
//                         stx.Append("70");
//                         stx.Append(i);
//                     }

//                 }
//                 else if ( tipo == 40 )
//                 {
//                     slx = Object.GetProperty(Me, var);
//                     foreach ( var sl in slx)
//                     {
//                         stx.Append("40");
//                         stx.Append(sl);
//                     }

//                 }
//             }
//             else
//             {
//                 stx.Append(CStr(tipo));
//                 stx.Append(Object.GetProperty(Me, var));
//             }
//         }

//     }

//     return stx;

// }

}

