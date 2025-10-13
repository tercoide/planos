using Gaucho;
public class main
{
 // Gambas module file

 // GambasCAD
 // Software para diseño CAD
 //
 // Copyright (C) Ing Martin P Cristia
 //
 // This program is free software; you can redistribute it and/or modify
 // it under the terms of the GNU General Public License as published by
 // the Free Software Foundation; either version 2 of the License, or
 // (at your option) any later version.
 //
 // This program is distributed in the hope that it will be useful,
 // but WITHOUT ANY WARRANTY; without even the implied warranty of
 // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 // GNU General Public License for more details.
 //
 // You should have received a copy of the GNU General Public License
 // along with this program; if not, write to the Free Software
 // Foundation, Inc., 51 Franklin St, Fifth Floor,
 // Boston, MA  02110-1301  USA

 //TINCHO 2023.05.22 > Config.class implementation
 // Eviado a la clase Config.class
 // Public dirResources As String = "/usr/share/gambascad"
 // Public dirDwgIn As String = User.Home &/ ".config/GambasCAD/DwgIn"
 // Public dirDxfIn As String = User.Home &/ ".config/GambasCAD/DxfIn"
 // Public dirDwgOut As String = User.Home &/ ".config/GambasCAD/DwgOut"
 // Public dirDxfOut As String = User.Home &/ ".config/GambasCAD/DxfOut"
 // Public dirTemplates As String = User.Home &/ ".config/GambasCAD/templates"
 // Public dirBlocks As String      // path to Blocks
 //TINCHO 2023.05.22 > Config.class implementation

 // file conversion
public bool convLibreDWG ;         
public bool convODA ;         
public bool convOdaAppImage ;         
public bool DebugMode = True;
public File MyLog ;         

public void main()
    {


    string sFile ;         

     //TINCHO 2023.05.22 > Config.class implementation
     // Estableciendo los parametros de configuración
    if (Exist(System.IO.File.Exists(System.IO.Path.Combine(User.Home, ".config", "gambascad", "config.json"))))
    {
        Config.Load(System.IO.Path.Combine(User.Home, ".config", "gambascad", "config.json"));
    }

    Config.Root = System.IO.Path.Combine(User.Home, ".config", "gambascad");
    Config.Depot = System.IO.Path.Combine(Config.Root, "config.json");
    Config.Log = System.IO.Path.Combine(Config.Root, "log.txt");
    Config.dirDwgIn = System.IO.Path.Combine(Config.Root, "dwgin");
    Config.dirDxfIn = System.IO.Path.Combine(Config.Root, "dxfin");
    Config.dirDwgOut = System.IO.Path.Combine(Config.Root, "dwgout");
    Config.dirDxfOut = System.IO.Path.Combine(Config.Root, "dxfout");
    Config.dirTemplates = System.IO.Path.Combine(Config.Root, "templates");
     //Config.dirBlocks = System.IO.Path.Combine(Config.Root, "blocks");
    Config.dirPrintStyles = System.IO.Path.Combine(Config.Root, "printstyles");
    Config.dirPatterns = System.IO.Path.Combine(Config.Root, "patterns");
    Config.dirResources = System.IO.Path.Combine(Config.Root, "resources");

    if ( Config.SplitterH.Count == 0 )
    {
        Config.SplitterH = [144, 500, 144];
    }

    if ( Config.SplitterV.Count == 0 )
    {
        Config.SplitterV = [400, 80];
    }

     //LoadPatterns2

    Config.Save(Config.Depot);

     // Public dirResources As String = "/usr/share/gambascad"
     // Public dirDwgIn As String = User.Home &/ ".config/GambasCAD/DwgIn"
     // Public dirDxfIn As String = User.Home &/ ".config/GambasCAD/DxfIn"
     // Public dirDwgOut As String = User.Home &/ ".config/GambasCAD/DwgOut"
     // Public dirDxfOut As String = User.Home &/ ".config/GambasCAD/DxfOut"
     // Public dirTemplates As String = User.Home &/ ".config/GambasCAD/templates"
     // Public dirBlocks As String      // path to Blocks

    Application.MainWindow = fMain;

    Wait;

    fSplash.Visible = True;

    fSplash.Show;

    Wait;

     //TINCHO 2023.05.22 > Config.class implementation
     // Anualdo: Los recursos se copiaran una unica vez, en la primera ejecucion, en el directorio del usuario.
     // necesitamo saber desde donde estamos corriendo
     //If Application.Path Like "/usr/bin*" Then
     //    DebugMode = False
     //    gcd.dirResources = "/usr/share/gambascad"
     //Else
     //    DebugMode = True
     //    gcd.dirResources = Application.Path
     //Endif
     //TINCHO 2023.05.22 > Config.class implementation

     // Inicializo el programa
    Initialize; // general init

     //MyLog = Open User.Home &/ ".config/gambascad/log.txt" For Write Create
     //TINCHO 2023.05.22 > Config.class implementation
    MyLog = File.Open(Config.Log, FileMode.Create, FileAccess.Write);

    gcd.debugInfo("Init program - Version " + Application.Version,false,false, True);
    gcd.debugInfo("Debug mode = " + Str(DebugMode),false,false, True);

     //fMain.tabFile.Index = 0

     // leo la configuracion inicial
     //Utils.LoadClass(Config, Config.ConfigFile) // Deshabilitado, con la nueva configuracion no es necesario.
    InitColors; // CAD color init
    InitClasses;
    InitMenus; // fMain menus
    loadPrintStyles;
    LoadPatterns;
    LoadCursors;

     // TODO: DATO INTERESANTE SI LLAMO A LA SIGUIENTE LINEA DESPUES DE Fmain.Run , los graficos se ven opacos
    gcd.Main; // drawing specific init

     // armo el combo de colores
     // fLayersOnScreen.Run  // FIXME:
    gcd.debuginfo("LayersOnScreen initialized OK",false,false, True);

    fMain.Run;
     //Wait
    fmain.Refresh;
     //gcd.debuginfo("FMain initialized OK",,, True)

    fSplash.HIde;

    Wait;

    if ( Application.Args.Count > 1 )
    {
        sFile = Args[1];
        actions.FileOpen(sfile);
    }
    else
    {
        actions.FileNew;
    }
    gcd.clsJob.start();
     // bloques common
    gcd.LoadCommon;

     // glx.Resize(fmain.glarea1)
     // fmain.glarea1.Refresh
    gcd.redraw;

}

public void Initialize()
    {


    string aFile ;         
    string[] aDirs = { System.IO.Path.Combine(Config.dirResources, "minimal") }; // , Config.dirPatterns
    string[] aTemp = [Config.dirDwgIn]; //, Config.dirPatterns]
    string sDir ;         

     // TERCO lo siguiente me borra los patrones de Hatch, elimino esa parte
     //TINCHO 2023.05.22 > Config.class implementation
     // Checking that the necessary directories exist
    foreach ( var sDir in aDirs)
    {
        if ( ! Exist(sDir) )
        {
            Utils.Shell("mkdir -p " + sDir);
        }
        else
        {
            Utils.Shell("rm -R " + sDir + "/*");
        }
    }
     // Copio lo patterns
    if ( ! Exist(Config.dirPatterns) )
    {
        Utils.Shell("mkdir -p " + Config.dirPatterns);
    }
    foreach ( string aFile in Directory.GetFiles("./patterns").ToArray())
    {
         //TINCHO 2023.05.22 > Config.class implementation
        if ( ! File.Exists(System.IO.Path.Combine(Config.dirPatterns, aFile)) )
        {
            File.Copy(System.IO.Path.Combine("./patterns", aFile), System.IO.Path.Combine(Config.dirPatterns, aFile));
        }

    }
     // Copio lo templates
    foreach ( string aFile in Directory.GetFiles("./minimal").ToArray())
    {
         //TINCHO 2023.05.22 > Config.class implementation
        File.Copy(System.IO.Path.Combine("./minimal", aFile), System.IO.Path.Combine(Config.dirTemplates, aFile));
    }

     // External programs availability
     // libredwg
    if ( File.Exists("/usr/local/lib/libredwg.so") ) convLibreDWG = True;

     // ODA
    if ( File.Exists("/usr/bin/ODAFileConverter") ) convODA = True;

    if ( Config.FileConversion == "ODA" )
    {
        if ( convODA )
        {
             //nothing
        }
        else if ( convLibreDWG )
        {
            Config.FileConversion = "LibreDWG";
        }
        else
        {
             // none available
            Config.FileConversion = "";
        }
    }
    else if ( Config.FileConversion == "LibreDWG" )
    {
        if ( convLibreDWG )
        {
             //nothing
        }
        else if ( convODA )
        {
            Config.FileConversion = "ODA";
        }
        else
        {
             // none available
            Config.FileConversion = "";
        }
    } // none selected
    else
    {
        if ( convODA )
        {
            Config.FileConversion = "ODA";
        }
        else if ( convLibreDWG )
        {
            Config.FileConversion = "LibreDWG";
        }
    }

     //TINCHO 2023.05.22 > Config.class implementation
     //dirBlocks = Config.BlocksLibraryPath

}

public void LoadPatterns()
    {


    string[] s ;         
    string sp ;         
    string spd ;         
    HatchPattern p ;         

    spd = Config.dirPatterns;
    s = Dir(spd, "*.pat");
    if ( S.Count == 0 )
    {

        foreach ( string sp in Directory.GetFiles(System.IO.Path.Combine(Application.path, "patterns"), "*.pat"))
        {
             // p = New HatchPattern
             // Utils.LoadClass2(p, spd &/ sp)
             // gcd.HatchPatterns.Add(p, p.Name)
            try
            {
                File.Copy(System.IO.Path.Combine(Application.path + "/patterns", sp), System.IO.Path.Combine(spd, sp));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error copying pattern file: " + ex.Message);
            }
        }

    }
    s = Directory.GetFiles(spd,  "*.pat");
    foreach ( string sp in s.ToList())
    {
         // p = New HatchPattern
         // Utils.LoadClass2(p, spd &/ sp)
         // gcd.HatchPatterns.Add(p, p.Name)
        gcd.ImportPAT(System.IO.Path.Combine(spd, sp));
    }

}

public void LoadPatterns2()
    {


    string[] s ;         
    string sp ;         
    string spd ;         
    HatchPattern p ;         

    spd = Config.dirPatterns;
    s = Directory.GetFiles(spd).ToList();
    foreach ( string sp in s)
    {
        p = new HatchPattern();
        Utils.LoadClass2(p, System.IO.Path.Combine(spd, sp));
        gcd.HatchPatterns.Add(p, p.Name);
    }

}

public void SavePatterns()
    {


    string[] s ;         
    string sp ;         
    HatchPattern p ;         

    sp = Config.dirPatterns;
    foreach ( p in gcd.HatchPatterns)
    {
        Utils.SaveClass2(p, System.IO.Path.Combine(sp, p.Name));
    }

}

public void LoadPrintStyles()
    {


    string[] s ;         
    string sp ;         
    string spd ;         
    PrintStyle p ;         

    spd = Config.dirPrintStyles;
    s = Dir(spd);
    foreach ( sp in s)
    {
        p = new PrintStyle;
        Utils.LoadClass(p, spd &/ sp);
        gcd.PrintStyles.Add(p, p.Name);
    }

}

public void SavePrintStyles()
    {


    string[] s ;         
    string sp ;         
    PrintStyle p ;         

    sp = Config.dirPrintStyles;
    foreach ( p in gcd.PrintStyles)
    {
        Utils.SaveClass(p, sp &/ p.Name);
    }

}

public void InitClasses()
    {


    Class cClass ;         
    string s ;         
    string sFinishedClasses ;         
    New String[] sSplit ;         

    sFinishedClasses = "LEADER HATCH POLYLINE ENDBLK SEQEND VERTEX POINT RECTANGLE POLYGON ATTDEF ATTRIB LINE LWPOLYLINE CIRCLE ELLIPSE ARC TEXT MTEXT SPLINE SOLID INSERT DIMENSION DIMENSION_LINEAR DIMENSION_DIAMETER DIMENSION_RADIUS DIMENSION_ANG3PT DIMENSION_ALIGNED DIMENSION_ORDINATE LARGE_RADIAL_DIMENSION ARC_DIMENSION VIEWPORT ARC3POINT";

    sFinishedClasses &= " AREA ARRAY BLOCKS BREAK CHAMFER COPY DIVIDE EDIT ERASE EXPLODE FILLET HATCHBUILDER ENTITYBUILDER LAYERS MIRROR MOVE MTEXTBUILDER OFFSET PAN PROTRACTOR ROTATE RULER SCALE SELECTION STRETCH TRIM ZOOME ZOOMW";
    sFinishedClasses &= " MLINE";
     // smart
    sFinishedClasses &= " SLAB BIMENTITYBUILDER";

    sSplit = Split(sFinishedClasses, " ");

    foreach ( s in sSplit)
    {
         // intento crearla
        cClass = Null;
        cClass = Class.Load("cad" + s);
        if ( cClass )
        {
            gcd.CCC.add(cClass.AutoCreate(), s);

            Debug s;
        }
        else
        {
            Console.WriteLine("WARNING: the Class " + s + " it + "\n"); //s not implemented."
        }

    }

}

private void InitColors()
    {

     // Load CAD color into Gambas colors
     // array index is CAD color, returning Gambas int color

    gcd.gcolor.add(color.rgb(0, 0, 0));
    gcd.gcolor.add(color.rgb(255, 0, 0));
    gcd.gcolor.add(color.rgb(255, 255, 0));
    gcd.gcolor.add(color.rgb(0, 255, 0));
    gcd.gcolor.add(color.rgb(0, 255, 255));
    gcd.gcolor.add(color.rgb(0, 0, 255));
    gcd.gcolor.add(color.rgb(255, 0, 255));
    gcd.gcolor.add(color.rgb(255, 255, 255));
    gcd.gcolor.add(color.rgb(128, 128, 128));
    gcd.gcolor.add(color.rgb(192, 192, 192));
    gcd.gcolor.add(color.rgb(255, 0, 0));
    gcd.gcolor.add(color.rgb(255, 127, 127));
    gcd.gcolor.add(color.rgb(165, 0, 0));
    gcd.gcolor.add(color.rgb(165, 82, 82));
    gcd.gcolor.add(color.rgb(127, 0, 0));
    gcd.gcolor.add(color.rgb(127, 63, 63));
    gcd.gcolor.add(color.rgb(76, 0, 0));
    gcd.gcolor.add(color.rgb(76, 38, 38));
    gcd.gcolor.add(color.rgb(38, 0, 0));
    gcd.gcolor.add(color.rgb(38, 19, 19));
    gcd.gcolor.add(color.rgb(255, 63, 0));
    gcd.gcolor.add(color.rgb(255, 159, 127));
    gcd.gcolor.add(color.rgb(165, 41, 0));
    gcd.gcolor.add(color.rgb(165, 103, 82));
    gcd.gcolor.add(color.rgb(127, 31, 0));
    gcd.gcolor.add(color.rgb(127, 79, 63));
    gcd.gcolor.add(color.rgb(76, 19, 0));
    gcd.gcolor.add(color.rgb(76, 47, 38));
    gcd.gcolor.add(color.rgb(38, 9, 0));
    gcd.gcolor.add(color.rgb(38, 23, 19));
    gcd.gcolor.add(color.rgb(255, 127, 0));
    gcd.gcolor.add(color.rgb(255, 191, 127));
    gcd.gcolor.add(color.rgb(165, 82, 0));
    gcd.gcolor.add(color.rgb(165, 124, 82));
    gcd.gcolor.add(color.rgb(127, 63, 0));
    gcd.gcolor.add(color.rgb(127, 95, 63));
    gcd.gcolor.add(color.rgb(76, 38, 0));
    gcd.gcolor.add(color.rgb(76, 57, 38));
    gcd.gcolor.add(color.rgb(38, 19, 0));
    gcd.gcolor.add(color.rgb(38, 28, 19));
    gcd.gcolor.add(color.rgb(255, 191, 0));
    gcd.gcolor.add(color.rgb(255, 223, 127));
    gcd.gcolor.add(color.rgb(165, 124, 0));
    gcd.gcolor.add(color.rgb(165, 145, 82));
    gcd.gcolor.add(color.rgb(127, 95, 0));
    gcd.gcolor.add(color.rgb(127, 111, 63));
    gcd.gcolor.add(color.rgb(76, 57, 0));
    gcd.gcolor.add(color.rgb(76, 66, 38));
    gcd.gcolor.add(color.rgb(38, 28, 0));
    gcd.gcolor.add(color.rgb(38, 33, 19));
    gcd.gcolor.add(color.rgb(255, 255, 0));
    gcd.gcolor.add(color.rgb(255, 255, 127));
    gcd.gcolor.add(color.rgb(165, 165, 0));
    gcd.gcolor.add(color.rgb(165, 165, 82));
    gcd.gcolor.add(color.rgb(127, 127, 0));
    gcd.gcolor.add(color.rgb(127, 127, 63));
    gcd.gcolor.add(color.rgb(76, 76, 0));
    gcd.gcolor.add(color.rgb(76, 76, 38));
    gcd.gcolor.add(color.rgb(38, 38, 0));
    gcd.gcolor.add(color.rgb(38, 38, 19));
    gcd.gcolor.add(color.rgb(191, 255, 0));
    gcd.gcolor.add(color.rgb(223, 255, 127));
    gcd.gcolor.add(color.rgb(124, 165, 0));
    gcd.gcolor.add(color.rgb(145, 165, 82));
    gcd.gcolor.add(color.rgb(95, 127, 0));
    gcd.gcolor.add(color.rgb(111, 127, 63));
    gcd.gcolor.add(color.rgb(57, 76, 0));
    gcd.gcolor.add(color.rgb(66, 76, 38));
    gcd.gcolor.add(color.rgb(28, 38, 0));
    gcd.gcolor.add(color.rgb(33, 38, 19));
    gcd.gcolor.add(color.rgb(127, 255, 0));
    gcd.gcolor.add(color.rgb(191, 255, 127));
    gcd.gcolor.add(color.rgb(82, 165, 0));
    gcd.gcolor.add(color.rgb(124, 165, 82));
    gcd.gcolor.add(color.rgb(63, 127, 0));
    gcd.gcolor.add(color.rgb(95, 127, 63));
    gcd.gcolor.add(color.rgb(38, 76, 0));
    gcd.gcolor.add(color.rgb(57, 76, 38));
    gcd.gcolor.add(color.rgb(19, 38, 0));
    gcd.gcolor.add(color.rgb(28, 38, 19));
    gcd.gcolor.add(color.rgb(63, 255, 0));
    gcd.gcolor.add(color.rgb(159, 255, 127));
    gcd.gcolor.add(color.rgb(41, 165, 0));
    gcd.gcolor.add(color.rgb(103, 165, 82));
    gcd.gcolor.add(color.rgb(31, 127, 0));
    gcd.gcolor.add(color.rgb(79, 127, 63));
    gcd.gcolor.add(color.rgb(19, 76, 0));
    gcd.gcolor.add(color.rgb(47, 76, 38));
    gcd.gcolor.add(color.rgb(9, 38, 0));
    gcd.gcolor.add(color.rgb(23, 38, 19));
    gcd.gcolor.add(color.rgb(0, 255, 0));
    gcd.gcolor.add(color.rgb(127, 255, 127));
    gcd.gcolor.add(color.rgb(0, 165, 0));
    gcd.gcolor.add(color.rgb(82, 165, 82));
    gcd.gcolor.add(color.rgb(0, 127, 0));
    gcd.gcolor.add(color.rgb(63, 127, 63));
    gcd.gcolor.add(color.rgb(0, 76, 0));
    gcd.gcolor.add(color.rgb(38, 76, 38));
    gcd.gcolor.add(color.rgb(0, 38, 0));
    gcd.gcolor.add(color.rgb(19, 38, 19));
    gcd.gcolor.add(color.rgb(0, 255, 63));
    gcd.gcolor.add(color.rgb(127, 255, 159));
    gcd.gcolor.add(color.rgb(0, 165, 41));
    gcd.gcolor.add(color.rgb(82, 165, 103));
    gcd.gcolor.add(color.rgb(0, 127, 31));
    gcd.gcolor.add(color.rgb(63, 127, 79));
    gcd.gcolor.add(color.rgb(0, 76, 19));
    gcd.gcolor.add(color.rgb(38, 76, 47));
    gcd.gcolor.add(color.rgb(0, 38, 9));
    gcd.gcolor.add(color.rgb(19, 38, 23));
    gcd.gcolor.add(color.rgb(0, 255, 127));
    gcd.gcolor.add(color.rgb(127, 255, 191));
    gcd.gcolor.add(color.rgb(0, 165, 82));
    gcd.gcolor.add(color.rgb(82, 165, 124));
    gcd.gcolor.add(color.rgb(0, 127, 63));
    gcd.gcolor.add(color.rgb(63, 127, 95));
    gcd.gcolor.add(color.rgb(0, 76, 38));
    gcd.gcolor.add(color.rgb(38, 76, 57));
    gcd.gcolor.add(color.rgb(0, 38, 19));
    gcd.gcolor.add(color.rgb(19, 38, 28));
    gcd.gcolor.add(color.rgb(0, 255, 191));
    gcd.gcolor.add(color.rgb(127, 255, 223));
    gcd.gcolor.add(color.rgb(0, 165, 124));
    gcd.gcolor.add(color.rgb(82, 165, 145));
    gcd.gcolor.add(color.rgb(0, 127, 95));
    gcd.gcolor.add(color.rgb(63, 127, 111));
    gcd.gcolor.add(color.rgb(0, 76, 57));
    gcd.gcolor.add(color.rgb(38, 76, 66));
    gcd.gcolor.add(color.rgb(0, 38, 28));
    gcd.gcolor.add(color.rgb(19, 38, 33));
    gcd.gcolor.add(color.rgb(0, 255, 255));
    gcd.gcolor.add(color.rgb(127, 255, 255));
    gcd.gcolor.add(color.rgb(0, 165, 165));
    gcd.gcolor.add(color.rgb(82, 165, 165));
    gcd.gcolor.add(color.rgb(0, 127, 127));
    gcd.gcolor.add(color.rgb(63, 127, 127));
    gcd.gcolor.add(color.rgb(0, 76, 76));
    gcd.gcolor.add(color.rgb(38, 76, 76));
    gcd.gcolor.add(color.rgb(0, 38, 38));
    gcd.gcolor.add(color.rgb(19, 38, 38));
    gcd.gcolor.add(color.rgb(0, 191, 255));
    gcd.gcolor.add(color.rgb(127, 223, 255));
    gcd.gcolor.add(color.rgb(0, 124, 165));
    gcd.gcolor.add(color.rgb(82, 145, 165));
    gcd.gcolor.add(color.rgb(0, 95, 127));
    gcd.gcolor.add(color.rgb(63, 111, 127));
    gcd.gcolor.add(color.rgb(0, 57, 76));
    gcd.gcolor.add(color.rgb(38, 66, 76));
    gcd.gcolor.add(color.rgb(0, 28, 38));
    gcd.gcolor.add(color.rgb(19, 33, 38));
    gcd.gcolor.add(color.rgb(0, 127, 255));
    gcd.gcolor.add(color.rgb(127, 191, 255));
    gcd.gcolor.add(color.rgb(0, 82, 165));
    gcd.gcolor.add(color.rgb(82, 124, 165));
    gcd.gcolor.add(color.rgb(0, 63, 127));
    gcd.gcolor.add(color.rgb(63, 95, 127));
    gcd.gcolor.add(color.rgb(0, 38, 76));
    gcd.gcolor.add(color.rgb(38, 57, 76));
    gcd.gcolor.add(color.rgb(0, 19, 38));
    gcd.gcolor.add(color.rgb(19, 28, 38));
    gcd.gcolor.add(color.rgb(0, 63, 255));
    gcd.gcolor.add(color.rgb(127, 159, 255));
    gcd.gcolor.add(color.rgb(0, 41, 165));
    gcd.gcolor.add(color.rgb(82, 103, 165));
    gcd.gcolor.add(color.rgb(0, 31, 127));
    gcd.gcolor.add(color.rgb(63, 79, 127));
    gcd.gcolor.add(color.rgb(0, 19, 76));
    gcd.gcolor.add(color.rgb(38, 47, 76));
    gcd.gcolor.add(color.rgb(0, 9, 38));
    gcd.gcolor.add(color.rgb(19, 23, 38));
    gcd.gcolor.add(color.rgb(0, 0, 255));
    gcd.gcolor.add(color.rgb(127, 127, 255));
    gcd.gcolor.add(color.rgb(0, 0, 165));
    gcd.gcolor.add(color.rgb(82, 82, 165));
    gcd.gcolor.add(color.rgb(0, 0, 127));
    gcd.gcolor.add(color.rgb(63, 63, 127));
    gcd.gcolor.add(color.rgb(0, 0, 76));
    gcd.gcolor.add(color.rgb(38, 38, 76));
    gcd.gcolor.add(color.rgb(0, 0, 38));
    gcd.gcolor.add(color.rgb(19, 19, 38));
    gcd.gcolor.add(color.rgb(63, 0, 255));
    gcd.gcolor.add(color.rgb(159, 127, 255));
    gcd.gcolor.add(color.rgb(41, 0, 165));
    gcd.gcolor.add(color.rgb(103, 82, 165));
    gcd.gcolor.add(color.rgb(31, 0, 127));
    gcd.gcolor.add(color.rgb(79, 63, 127));
    gcd.gcolor.add(color.rgb(19, 0, 76));
    gcd.gcolor.add(color.rgb(47, 38, 76));
    gcd.gcolor.add(color.rgb(9, 0, 38));
    gcd.gcolor.add(color.rgb(23, 19, 38));
    gcd.gcolor.add(color.rgb(127, 0, 255));
    gcd.gcolor.add(color.rgb(191, 127, 255));
    gcd.gcolor.add(color.rgb(82, 0, 165));
    gcd.gcolor.add(color.rgb(124, 82, 165));
    gcd.gcolor.add(color.rgb(63, 0, 127));
    gcd.gcolor.add(color.rgb(95, 63, 127));
    gcd.gcolor.add(color.rgb(38, 0, 76));
    gcd.gcolor.add(color.rgb(57, 38, 76));
    gcd.gcolor.add(color.rgb(19, 0, 38));
    gcd.gcolor.add(color.rgb(28, 19, 38));
    gcd.gcolor.add(color.rgb(191, 0, 255));
    gcd.gcolor.add(color.rgb(223, 127, 255));
    gcd.gcolor.add(color.rgb(124, 0, 165));
    gcd.gcolor.add(color.rgb(145, 82, 165));
    gcd.gcolor.add(color.rgb(95, 0, 127));
    gcd.gcolor.add(color.rgb(111, 63, 127));
    gcd.gcolor.add(color.rgb(57, 0, 76));
    gcd.gcolor.add(color.rgb(66, 38, 76));
    gcd.gcolor.add(color.rgb(28, 0, 38));
    gcd.gcolor.add(color.rgb(33, 19, 38));
    gcd.gcolor.add(color.rgb(255, 0, 255));
    gcd.gcolor.add(color.rgb(255, 127, 255));
    gcd.gcolor.add(color.rgb(165, 0, 165));
    gcd.gcolor.add(color.rgb(165, 82, 165));
    gcd.gcolor.add(color.rgb(127, 0, 127));
    gcd.gcolor.add(color.rgb(127, 63, 127));
    gcd.gcolor.add(color.rgb(76, 0, 76));
    gcd.gcolor.add(color.rgb(76, 38, 76));
    gcd.gcolor.add(color.rgb(38, 0, 38));
    gcd.gcolor.add(color.rgb(38, 19, 38));
    gcd.gcolor.add(color.rgb(255, 0, 191));
    gcd.gcolor.add(color.rgb(255, 127, 223));
    gcd.gcolor.add(color.rgb(165, 0, 124));
    gcd.gcolor.add(color.rgb(165, 82, 145));
    gcd.gcolor.add(color.rgb(127, 0, 95));
    gcd.gcolor.add(color.rgb(127, 63, 111));
    gcd.gcolor.add(color.rgb(76, 0, 57));
    gcd.gcolor.add(color.rgb(76, 38, 66));
    gcd.gcolor.add(color.rgb(38, 0, 28));
    gcd.gcolor.add(color.rgb(38, 19, 33));
    gcd.gcolor.add(color.rgb(255, 0, 127));
    gcd.gcolor.add(color.rgb(255, 127, 191));
    gcd.gcolor.add(color.rgb(165, 0, 82));
    gcd.gcolor.add(color.rgb(165, 82, 124));
    gcd.gcolor.add(color.rgb(127, 0, 63));
    gcd.gcolor.add(color.rgb(127, 63, 95));
    gcd.gcolor.add(color.rgb(76, 0, 38));
    gcd.gcolor.add(color.rgb(76, 38, 57));
    gcd.gcolor.add(color.rgb(38, 0, 19));
    gcd.gcolor.add(color.rgb(38, 19, 28));
    gcd.gcolor.add(color.rgb(255, 0, 63));
    gcd.gcolor.add(color.rgb(255, 127, 159));
    gcd.gcolor.add(color.rgb(165, 0, 41));
    gcd.gcolor.add(color.rgb(165, 82, 103));
    gcd.gcolor.add(color.rgb(127, 0, 31));
    gcd.gcolor.add(color.rgb(127, 63, 79));
    gcd.gcolor.add(color.rgb(76, 0, 19));
    gcd.gcolor.add(color.rgb(76, 38, 47));
    gcd.gcolor.add(color.rgb(38, 0, 9));
    gcd.gcolor.add(color.rgb(38, 19, 23));
    gcd.gcolor.add(color.rgb(0, 0, 0));
    gcd.gcolor.add(color.rgb(51, 51, 51));
    gcd.gcolor.add(color.rgb(102, 102, 102));
    gcd.gcolor.add(color.rgb(153, 153, 153));
    gcd.gcolor.add(color.rgb(204, 204, 204));
    gcd.gcolor.add(color.rgb(255, 255, 255));
    gcd.gcolor.add(color.rgb(255, 255, 255)); //By Layer
    gcd.gcolor.add(color.rgb(255, 255, 255)); //By Block
    gcd.gcolor.add(color.rgb(255, 255, 255)); //By Object?

     // corrijo los colores que no se ven contra el fondo
    gcd.gcolor[0] = config.WhiteAndBlack;
    gcd.gcolor[7] = config.WhiteAndBlack;
    gcd.gcolor[250] = config.WhiteAndBlack;
    gcd.gcolor[255] = config.WhiteAndBlack;
    gcd.gcolor[256] = config.WhiteAndBlack;
    gcd.gcolor[257] = config.WhiteAndBlack;
    gcd.gcolor[258] = config.WhiteAndBlack;

}

public void InitMenus()
    {


     // colores
    void Menu( fMai)
        {

    Menu mItem ;         

    int i ;         
    int iColor ;         

    string sMenuPre ;         
    string sMenuPost ;         
    string sMenuSnap ;         

    mColors.Name = "mColores";

     //============ByLayer======================
    i = 256;
    iColor = gcd.gColor[i];
    mItem = new Menu(mColors);
    mItem.Text = "ByLayer";
    mItem.Action = "Color_256"; // & CStr(i)
    mItem.Picture = paintPlus.picCirculito(8, gcd.gColor[256], Color.ButtonForeground);

     //============Byblock======================
    i = 257;
    mItem = new Menu(mColors);
    mItem.Text = "ByBlock";
    mItem.Action = "Color_257"; // & CStr(i)
    mItem.Picture = paintPlus.picCirculito(8, gcd.gColor[i], Color.ButtonForeground);

     //============separator======================
    mItem = new Menu(mColors);
    mItem.Text = "";
    mItem.Action = ""; // & CStr(i)

    for ( i = 0; i <= 10; i + 1) //gcd.gColor.Max
    {
        mItem = new Menu(mColors);
        mItem.Text = "Color " + CStr(i);
        mItem.Action = "Color_" + CStr(i);
        mItem.Picture = paintPlus.picCirculito(8, gcd.gColor[i], Color.ButtonForeground);

    }
     //============separator======================
    mItem = new Menu(mColors);
    mItem.Text = "";
    mItem.Action = ""; // & CStr(i)

     //============more colors======================
    mItem = new Menu(mColors);
    mItem.Text = ("more colors...");
    mItem.Action = "more_colors"; // & CStr(i)

    fMain.mbtcolors.text = mColors.Children[0].Text;
    fMain.mbtColors.Picture = mColors.Children[0].Picture;
    fMain.mbtColors.Menu = mColors.Name;
    fMain.mbtColors.Tag = mColors.Children[0].Tag;
    fMain.mbtColors.Action = "Color_256";

     // Menu contextual de entidades
     // colores
    void Menu( fMai)
        {


    mEntities.Name = "mEntities";
     //fMain.PopupMenu = "mEntities"
     // Cortar/Copiar/Pegar/Propiedades/Apagar sus layers/Hacer actual ese layer
     //============Cut======================
    mItem = new Menu(mEntities);
    mItem.Text = ("Cut");
    mItem.Action = "mEntities-Cut";
    mItem.Picture = Picture["icon:/32/cut"];
     //============Copy======================
    mItem = new Menu(mEntities);
    mItem.Text = ("Copy");
    mItem.Action = "mEntities-Copy";
    mItem.Picture = Picture["icon:/32/copy"];
     //============Paste======================
    mItem = new Menu(mEntities);
    mItem.Text = ("Paste");
    mItem.Action = "mEntities-Paste";
    mItem.Picture = Picture["icon:/32/paste"];
     //============separator======================
    mItem = new Menu(mColors);
    mItem.Text = "";
    mItem.Action = "";
     //============Agrupar======================
    mItem = new Menu(mEntities);
    mItem.Text = ("Make group");
    mItem.Action = "mEntities-Group";
    mItem.Picture = Picture["icon:/32/paste"];
     //============Desagrupar======================
    mItem = new Menu(mEntities);
    mItem.Text = ("Break group");
    mItem.Action = "mEntities-DeGroup";
    mItem.Picture = Picture["icon:/32/paste"];
     //============separator======================
    mItem = new Menu(mColors);
    mItem.Text = "";
    mItem.Action = ""; // & CStr(i)
     //============Ocultar layers de todo=====
    mItem = new Menu(mEntities);
    mItem.Text = ("HIde these Layers");
    mItem.Action = "mEntities-HIdeLayers";
    mItem.Picture = Picture["icon:/32/paste"];
     //============Desagrupar======================
    mItem = new Menu(mEntities);
    mItem.Text = ("Paste");
    mItem.Action = "mEntities-Paste";
    mItem.Picture = Picture["icon:/32/paste"];
     //fMain.PopupMenu = "mEntities"

     //===================================================================================================================================
     // Armo el menu contextual de cada ENTIDAD
    gcd.SnapMode = Config.SnapModeSaved;
    sMenuSnap = ";Snap to...;;<nothing>;_nothing;4;;4;End point;_end;end_point;4;MId point;_mId;mId_point;4;Perpendicular;_per;perpendicular;4";
    sMenuSnap &= ";Quadrant;_qua;quadrant;4";
    sMenuSnap &= ";Center;_cen;center_point;4";
    sMenuSnap &= ";Intersection;_int;intersection;4";
    sMenuSnap &= ";Tangent;_tan;tangent;4";
    sMenuSnap &= ";Nearest;_nea;nearest;4";
    sMenuSnap &= ";Base point;_bas;base_point;4";

    sMenuPost = (";;;Cancel;_CANCEL;");
    Variant vClass ;         
     String[] stxMenus =[]     ;         

    foreach ( vClass in gcd.CCC)
    {
        Utils.MenuMakerPlus(fmain, vClass.gender, vClass.contextmenu + sMenuSnap + sMenuPost, gcd.dirResources &/ "svg" &/ Config.IconFamily);
    }

}

 // carga los cursores desde SVG y los coloca en GCD
public void LoadCursors()
    {


    int c ;         
    string sCursor ;         

    sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor1.svg"), "#0066b3", Config.ModelBackgroundColor);
    gcd.CursorCross = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

    sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor2.svg"), "#0066b3", Config.ModelBackgroundColor);
    gcd.CursorSelect = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

    sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor3.svg"), "#0066b3", Config.ModelBackgroundColor);
    gcd.CursorSelectAdd = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

    sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor4.svg"), "#0066b3", Config.ModelBackgroundColor);
    gcd.CursorSelectRem = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

    sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor5.svg"), "#0066b3", Config.ModelBackgroundColor);
    gcd.CursorSelectXchange = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

}

}