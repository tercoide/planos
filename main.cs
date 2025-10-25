
namespace Gaucho
{
    public static class main
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
        public static bool convLibreDWG;
        public static bool convODA;
        public static bool convOdaAppImage;
        public static bool DebugMode = true;
        public static FileStream? MyLog;

        public static void main()
        {
            Config.Home = Environment.GetEnvironmentVariable("HOME") ??
                      Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string sFile;

            //TINCHO 2023.05.22 > Config.class implementation
            // Estableciendo los parametros de configuración

            Config.Load();

            Config.Root = System.IO.Path.Combine(Config.Home, ".config", "gambascad");
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

            if (Config.SplitterH.Length == 0)
            {
                Config.SplitterH = [144, 500, 144];
            }

            if (Config.SplitterV.Length == 0)
            {
                Config.SplitterV = [400, 80];
            }

            //LoadPatterns2

            Config.Save();

            // Public dirResources As String = "/usr/share/gambascad"
            // Public dirDwgIn As String = User.Home &/ ".config/GambasCAD/DwgIn"
            // Public dirDxfIn As String = User.Home &/ ".config/GambasCAD/DxfIn"
            // Public dirDwgOut As String = User.Home &/ ".config/GambasCAD/DwgOut"
            // Public dirDxfOut As String = User.Home &/ ".config/GambasCAD/DxfOut"
            // Public dirTemplates As String = User.Home &/ ".config/GambasCAD/templates"
            // Public dirBlocks As String      // path to Blocks

            Application.MainWindow = fMain;

            Wait;

            fSplash.Visible = true;

            fSplash.Show;

            Wait;

            //TINCHO 2023.05.22 > Config.class implementation
            // Anualdo: Los recursos se copiaran una unica vez, en la primera ejecucion, en el directorio del usuario.
            // necesitamo saber desde donde estamos corriendo
            //If Application.Path Like "/usr/bin*" Then
            //    DebugMode = false
            //    Gcd.dirResources = "/usr/share/gambascad"
            //Else
            //    DebugMode = true
            //    Gcd.dirResources = Application.Path
            //Endif
            //TINCHO 2023.05.22 > Config.class implementation

            // Inicializo el programa
            Initialize; // general init

            //MyLog = Open User.Home &/ ".config/gambascad/log.txt" For Write Create
            //TINCHO 2023.05.22 > Config.class implementation
            MyLog = new FileStream(Config.Log, FileMode.Create, FileAccess.Write);

            Gcd.debugInfo("Init program - Version " + Application.Version, false, false, true);
            Gcd.debugInfo("Debug mode = " + Str(DebugMode), false, false, true);

            //fMain.tabFile.Index = 0

            // leo la configuracion inicial
            //Utils.LoadClass(Config, Config.ConfigFile) // Deshabilitado, con la nueva configuracion no es necesario.
            InitColors; // CAD color init
            InitClasses;
            InitMenus; // fMain menus
            loadPrintStyles();
            LoadPatterns();
            LoadCursors();

            // TODO: DATO INTERESANTE SI LLAMO A LA SIGUIENTE LINEA DESPUES DE Fmain.Run , los graficos se ven opacos
            Gcd.Main(); // drawing specific init

            // armo el combo de colores
            // fLayersOnScreen.Run  // FIXME:
            Gcd.debuginfo("LayersOnScreen initialized OK", false, false, true);

            fMain.Run;
            //Wait
            fmain.Refresh;
            //Gcd.debuginfo("FMain initialized OK",,, true)

            fSplash.HIde;

            Wait;

            if (Application.Args.Count > 1)
            {
                sFile = Args[1];
                actions.FileOpen(sfile);
            }
            else
            {
                actions.FileNew;
            }
            Gcd.clsJob.start();
            // bloques common
            Gcd.LoadCommon;

            // glx.Resize(fmain.glarea1)
            // fmain.glarea1.Refresh
            Gcd.redraw;

        }

        public static void Initialize()
        {



            string[] aDirs = { System.IO.Path.Combine(Config.dirResources, "minimal") }; // , Config.dirPatterns
            string[] aTemp = [Config.dirDwgIn]; //, Config.dirPatterns]


            // TERCO lo siguiente me borra los patrones de Hatch, elimino esa parte
            //TINCHO 2023.05.22 > Config.class implementation
            // Checking that the necessary directories exist
            foreach (var sDir in aDirs)
            {
                if (!File.Exists(sDir))
                {
                    Utils.Shell("mkdir -p " + sDir);
                }
                else
                {
                    Utils.Shell("rm -R " + sDir + "/*");
                }
            }
            // Copio lo patterns
            if (!File.Exists(Config.dirPatterns))
            {
                Utils.Shell("mkdir -p " + Config.dirPatterns);
            }
            foreach (var aFile in Directory.GetFiles("./patterns").ToArray())
            {
                //TINCHO 2023.05.22 > Config.class implementation
                if (!File.Exists(System.IO.Path.Combine(Config.dirPatterns, aFile)))
                {
                    File.Copy(System.IO.Path.Combine("./patterns", aFile), System.IO.Path.Combine(Config.dirPatterns, aFile));
                }

            }
            // Copio lo templates
            foreach (string aFile in Directory.GetFiles("./minimal").ToArray())
            {
                //TINCHO 2023.05.22 > Config.class implementation
                File.Copy(System.IO.Path.Combine("./minimal", aFile), System.IO.Path.Combine(Config.dirTemplates, aFile));
            }

            // External programs availability
            // libredwg
            if (File.Exists("/usr/local/lib/libredwg.so")) convLibreDWG = true;

            // ODA
            if (File.Exists("/usr/bin/ODAFileConverter")) convODA = true;

            if (Config.FileConversion == "ODA")
            {
                if (convODA)
                {
                    //nothing
                }
                else if (convLibreDWG)
                {
                    Config.FileConversion = "LibreDWG";
                }
                else
                {
                    // none available
                    Config.FileConversion = "";
                }
            }
            else if (Config.FileConversion == "LibreDWG")
            {
                if (convLibreDWG)
                {
                    //nothing
                }
                else if (convODA)
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
                if (convODA)
                {
                    Config.FileConversion = "ODA";
                }
                else if (convLibreDWG)
                {
                    Config.FileConversion = "LibreDWG";
                }
            }

            //TINCHO 2023.05.22 > Config.class implementation
            //dirBlocks = Config.BlocksLibraryPath

        }

        public static void LoadPatterns()
        {


            string[] s;
            string sp;
            string spd;
            HatchPattern p;

            spd = Config.dirPatterns;
            s = Dir(spd, "*.pat");
            if (S.Count == 0)
            {

                foreach (string sp in Directory.GetFiles(System.IO.Path.Combine(Application.path, "patterns"), "*.pat"))
                {
                    // p = New HatchPattern
                    // Utils.LoadClass2(p, spd &/ sp)
                    // Gcd.HatchPatterns.Add(p, p.Name)
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
            s = Directory.GetFiles(spd, "*.pat");
            foreach (string sp in s.ToList())
            {
                // p = New HatchPattern
                // Utils.LoadClass2(p, spd &/ sp)
                // Gcd.HatchPatterns.Add(p, p.Name)
                Gcd.ImportPAT(System.IO.Path.Combine(spd, sp));
            }

        }

        // public void LoadPatterns2()
        //     {


        //     string[] s ;         
        //     string sp ;         
        //     string spd ;         
        //     HatchPattern p ;         

        //     spd = Config.dirPatterns;
        //     s = Directory.GetFiles(spd).ToList();
        //     foreach ( string sp in s)
        //     {
        //         p = new HatchPattern();
        //         Utils.LoadClass2(p, System.IO.Path.Combine(spd, sp));
        //         Gcd.HatchPatterns.Add(p, p.Name);
        //     }

        // }

        // public void SavePatterns()
        //     {


        //     string[] s ;         
        //     string sp ;         
        //     HatchPattern p ;         

        //     sp = Config.dirPatterns;
        //     foreach ( p in Gcd.HatchPatterns)
        //     {
        //         Utils.SaveClass2(p, System.IO.Path.Combine(sp, p.Name));
        //     }

        // }

        // public void LoadPrintStyles()
        //     {


        //     string[] s ;         
        //     string sp ;         
        //     string spd ;         
        //     PrintStyle p ;         

        //     spd = Config.dirPrintStyles;
        //     s = Dir(spd);
        //     foreach ( sp in s)
        //     {
        //         p = new PrintStyle;
        //         Utils.LoadClass(p, spd &/ sp);
        //         Gcd.PrintStyles.Add(p, p.Name);
        //     }

        // }

        // public void SavePrintStyles()
        //     {


        //     string[] s ;         
        //     string sp ;         
        //     PrintStyle p ;         

        //     sp = Config.dirPrintStyles;
        //     foreach ( p in Gcd.PrintStyles)
        //     {
        //         Utils.SaveClass(p, sp &/ p.Name);
        //     }

        // }

        public void InitClasses()
        {


            // Class cClass ;         
            // string s ;         
            // string sFinishedClasses ;         
            // New String[] sSplit ;         

            // sFinishedClasses = "LEADER HATCH POLYLINE ENDBLK SEQEND VERTEX POINT RECTANGLE POLYGON ATTDEF ATTRIB LINE LWPOLYLINE CIRCLE ELLIPSE ARC TEXT MTEXT SPLINE SOLID INSERT DIMENSION DIMENSION_LINEAR DIMENSION_DIAMETER DIMENSION_RADIUS DIMENSION_ANG3PT DIMENSION_ALIGNED DIMENSION_ORDINATE LARGE_RADIAL_DIMENSION ARC_DIMENSION VIEWPORT ARC3POINT";

            // sFinishedClasses &= " AREA ARRAY BLOCKS BREAK CHAMFER COPY DIVIDE EDIT ERASE EXPLODE FILLET HATCHBUILDER ENTITYBUILDER LAYERS MIRROR MOVE MTEXTBUILDER OFFSET PAN PROTRACTOR ROTATE RULER SCALE SELECTION STRETCH TRIM ZOOME ZOOMW";
            // sFinishedClasses &= " MLINE";
            //  // smart
            // sFinishedClasses &= " SLAB BIMENTITYBUILDER";

            // sSplit = Split(sFinishedClasses, " ");

            // foreach ( s in sSplit)
            // {
            //      // intento crearla
            //     cClass = Null;
            //     cClass = Class.Load("cad" + s);
            //     if ( cClass )
            //     {
            //         Gcd.CCC.add(cClass.AutoCreate(), s);

            //         Debug s;
            //     }
            //     else
            //     {
            //         Console.WriteLine("WARNING: the Class " + s + " it + "\n"); //s not implemented."
            //     }

            // }

        }

        public static int RGB(int r, int g, int b)
        {
            // return (r << 16) | (g << 8) | b;
            return (b << 16) | (g << 8) | r;
        }

        private static void InitColors()
        {

            // Load CAD color into Gambas colors
            // array index is CAD color, returning Gambas int color

            Gcd.gColor.add(RGB(0, 0, 0));
            Gcd.gColor.add(RGB(255, 0, 0));
            Gcd.gColor.add(RGB(255, 255, 0));
            Gcd.gColor.add(RGB(0, 255, 0));
            Gcd.gColor.add(RGB(0, 255, 255));
            Gcd.gColor.add(RGB(0, 0, 255));
            Gcd.gColor.add(RGB(255, 0, 255));
            Gcd.gColor.add(RGB(255, 255, 255));
            Gcd.gColor.add(RGB(128, 128, 128));
            Gcd.gColor.add(RGB(192, 192, 192));
            Gcd.gColor.add(RGB(255, 0, 0));
            Gcd.gColor.add(RGB(255, 127, 127));
            Gcd.gColor.add(RGB(165, 0, 0));
            Gcd.gColor.add(RGB(165, 82, 82));
            Gcd.gColor.add(RGB(127, 0, 0));
            Gcd.gColor.add(RGB(127, 63, 63));
            Gcd.gColor.add(RGB(76, 0, 0));
            Gcd.gColor.add(RGB(76, 38, 38));
            Gcd.gColor.add(RGB(38, 0, 0));
            Gcd.gColor.add(RGB(38, 19, 19));
            Gcd.gColor.add(RGB(255, 63, 0));
            Gcd.gColor.add(RGB(255, 159, 127));
            Gcd.gColor.add(RGB(165, 41, 0));
            Gcd.gColor.add(RGB(165, 103, 82));
            Gcd.gColor.add(RGB(127, 31, 0));
            Gcd.gColor.add(RGB(127, 79, 63));
            Gcd.gColor.add(RGB(76, 19, 0));
            Gcd.gColor.add(RGB(76, 47, 38));
            Gcd.gColor.add(RGB(38, 9, 0));
            Gcd.gColor.add(RGB(38, 23, 19));
            Gcd.gColor.add(RGB(255, 127, 0));
            Gcd.gColor.add(RGB(255, 191, 127));
            Gcd.gColor.add(RGB(165, 82, 0));
            Gcd.gColor.add(RGB(165, 124, 82));
            Gcd.gColor.add(RGB(127, 63, 0));
            Gcd.gColor.add(RGB(127, 95, 63));
            Gcd.gColor.add(RGB(76, 38, 0));
            Gcd.gColor.add(RGB(76, 57, 38));
            Gcd.gColor.add(RGB(38, 19, 0));
            Gcd.gColor.add(RGB(38, 28, 19));
            Gcd.gColor.add(RGB(255, 191, 0));
            Gcd.gColor.add(RGB(255, 223, 127));
            Gcd.gColor.add(RGB(165, 124, 0));
            Gcd.gColor.add(RGB(165, 145, 82));
            Gcd.gColor.add(RGB(127, 95, 0));
            Gcd.gColor.add(RGB(127, 111, 63));
            Gcd.gColor.add(RGB(76, 57, 0));
            Gcd.gColor.add(RGB(76, 66, 38));
            Gcd.gColor.add(RGB(38, 28, 0));
            Gcd.gColor.add(RGB(38, 33, 19));
            Gcd.gColor.add(RGB(255, 255, 0));
            Gcd.gColor.add(RGB(255, 255, 127));
            Gcd.gColor.add(RGB(165, 165, 0));
            Gcd.gColor.add(RGB(165, 165, 82));
            Gcd.gColor.add(RGB(127, 127, 0));
            Gcd.gColor.add(RGB(127, 127, 63));
            Gcd.gColor.add(RGB(76, 76, 0));
            Gcd.gColor.add(RGB(76, 76, 38));
            Gcd.gColor.add(RGB(38, 38, 0));
            Gcd.gColor.add(RGB(38, 38, 19));
            Gcd.gColor.add(RGB(191, 255, 0));
            Gcd.gColor.add(RGB(223, 255, 127));
            Gcd.gColor.add(RGB(124, 165, 0));
            Gcd.gColor.add(RGB(145, 165, 82));
            Gcd.gColor.add(RGB(95, 127, 0));
            Gcd.gColor.add(RGB(111, 127, 63));
            Gcd.gColor.add(RGB(57, 76, 0));
            Gcd.gColor.add(RGB(66, 76, 38));
            Gcd.gColor.add(RGB(28, 38, 0));
            Gcd.gColor.add(RGB(33, 38, 19));
            Gcd.gColor.add(RGB(127, 255, 0));
            Gcd.gColor.add(RGB(191, 255, 127));
            Gcd.gColor.add(RGB(82, 165, 0));
            Gcd.gColor.add(RGB(124, 165, 82));
            Gcd.gColor.add(RGB(63, 127, 0));
            Gcd.gColor.add(RGB(95, 127, 63));
            Gcd.gColor.add(RGB(38, 76, 0));
            Gcd.gColor.add(RGB(57, 76, 38));
            Gcd.gColor.add(RGB(19, 38, 0));
            Gcd.gColor.add(RGB(28, 38, 19));
            Gcd.gColor.add(RGB(63, 255, 0));
            Gcd.gColor.add(RGB(159, 255, 127));
            Gcd.gColor.add(RGB(41, 165, 0));
            Gcd.gColor.add(RGB(103, 165, 82));
            Gcd.gColor.add(RGB(31, 127, 0));
            Gcd.gColor.add(RGB(79, 127, 63));
            Gcd.gColor.add(RGB(19, 76, 0));
            Gcd.gColor.add(RGB(47, 76, 38));
            Gcd.gColor.add(RGB(9, 38, 0));
            Gcd.gColor.add(RGB(23, 38, 19));
            Gcd.gColor.add(RGB(0, 255, 0));
            Gcd.gColor.add(RGB(127, 255, 127));
            Gcd.gColor.add(RGB(0, 165, 0));
            Gcd.gColor.add(RGB(82, 165, 82));
            Gcd.gColor.add(RGB(0, 127, 0));
            Gcd.gColor.add(RGB(63, 127, 63));
            Gcd.gColor.add(RGB(0, 76, 0));
            Gcd.gColor.add(RGB(38, 76, 38));
            Gcd.gColor.add(RGB(0, 38, 0));
            Gcd.gColor.add(RGB(19, 38, 19));
            Gcd.gColor.add(RGB(0, 255, 63));
            Gcd.gColor.add(RGB(127, 255, 159));
            Gcd.gColor.add(RGB(0, 165, 41));
            Gcd.gColor.add(RGB(82, 165, 103));
            Gcd.gColor.add(RGB(0, 127, 31));
            Gcd.gColor.add(RGB(63, 127, 79));
            Gcd.gColor.add(RGB(0, 76, 19));
            Gcd.gColor.add(RGB(38, 76, 47));
            Gcd.gColor.add(RGB(0, 38, 9));
            Gcd.gColor.add(RGB(19, 38, 23));
            Gcd.gColor.add(RGB(0, 255, 127));
            Gcd.gColor.add(RGB(127, 255, 191));
            Gcd.gColor.add(RGB(0, 165, 82));
            Gcd.gColor.add(RGB(82, 165, 124));
            Gcd.gColor.add(RGB(0, 127, 63));
            Gcd.gColor.add(RGB(63, 127, 95));
            Gcd.gColor.add(RGB(0, 76, 38));
            Gcd.gColor.add(RGB(38, 76, 57));
            Gcd.gColor.add(RGB(0, 38, 19));
            Gcd.gColor.add(RGB(19, 38, 28));
            Gcd.gColor.add(RGB(0, 255, 191));
            Gcd.gColor.add(RGB(127, 255, 223));
            Gcd.gColor.add(RGB(0, 165, 124));
            Gcd.gColor.add(RGB(82, 165, 145));
            Gcd.gColor.add(RGB(0, 127, 95));
            Gcd.gColor.add(RGB(63, 127, 111));
            Gcd.gColor.add(RGB(0, 76, 57));
            Gcd.gColor.add(RGB(38, 76, 66));
            Gcd.gColor.add(RGB(0, 38, 28));
            Gcd.gColor.add(RGB(19, 38, 33));
            Gcd.gColor.add(RGB(0, 255, 255));
            Gcd.gColor.add(RGB(127, 255, 255));
            Gcd.gColor.add(RGB(0, 165, 165));
            Gcd.gColor.add(RGB(82, 165, 165));
            Gcd.gColor.add(RGB(0, 127, 127));
            Gcd.gColor.add(RGB(63, 127, 127));
            Gcd.gColor.add(RGB(0, 76, 76));
            Gcd.gColor.add(RGB(38, 76, 76));
            Gcd.gColor.add(RGB(0, 38, 38));
            Gcd.gColor.add(RGB(19, 38, 38));
            Gcd.gColor.add(RGB(0, 191, 255));
            Gcd.gColor.add(RGB(127, 223, 255));
            Gcd.gColor.add(RGB(0, 124, 165));
            Gcd.gColor.add(RGB(82, 145, 165));
            Gcd.gColor.add(RGB(0, 95, 127));
            Gcd.gColor.add(RGB(63, 111, 127));
            Gcd.gColor.add(RGB(0, 57, 76));
            Gcd.gColor.add(RGB(38, 66, 76));
            Gcd.gColor.add(RGB(0, 28, 38));
            Gcd.gColor.add(RGB(19, 33, 38));
            Gcd.gColor.add(RGB(0, 127, 255));
            Gcd.gColor.add(RGB(127, 191, 255));
            Gcd.gColor.add(RGB(0, 82, 165));
            Gcd.gColor.add(RGB(82, 124, 165));
            Gcd.gColor.add(RGB(0, 63, 127));
            Gcd.gColor.add(RGB(63, 95, 127));
            Gcd.gColor.add(RGB(0, 38, 76));
            Gcd.gColor.add(RGB(38, 57, 76));
            Gcd.gColor.add(RGB(0, 19, 38));
            Gcd.gColor.add(RGB(19, 28, 38));
            Gcd.gColor.add(RGB(0, 63, 255));
            Gcd.gColor.add(RGB(127, 159, 255));
            Gcd.gColor.add(RGB(0, 41, 165));
            Gcd.gColor.add(RGB(82, 103, 165));
            Gcd.gColor.add(RGB(0, 31, 127));
            Gcd.gColor.add(RGB(63, 79, 127));
            Gcd.gColor.add(RGB(0, 19, 76));
            Gcd.gColor.add(RGB(38, 47, 76));
            Gcd.gColor.add(RGB(0, 9, 38));
            Gcd.gColor.add(RGB(19, 23, 38));
            Gcd.gColor.add(RGB(0, 0, 255));
            Gcd.gColor.add(RGB(127, 127, 255));
            Gcd.gColor.add(RGB(0, 0, 165));
            Gcd.gColor.add(RGB(82, 82, 165));
            Gcd.gColor.add(RGB(0, 0, 127));
            Gcd.gColor.add(RGB(63, 63, 127));
            Gcd.gColor.add(RGB(0, 0, 76));
            Gcd.gColor.add(RGB(38, 38, 76));
            Gcd.gColor.add(RGB(0, 0, 38));
            Gcd.gColor.add(RGB(19, 19, 38));
            Gcd.gColor.add(RGB(63, 0, 255));
            Gcd.gColor.add(RGB(159, 127, 255));
            Gcd.gColor.add(RGB(41, 0, 165));
            Gcd.gColor.add(RGB(103, 82, 165));
            Gcd.gColor.add(RGB(31, 0, 127));
            Gcd.gColor.add(RGB(79, 63, 127));
            Gcd.gColor.add(RGB(19, 0, 76));
            Gcd.gColor.add(RGB(47, 38, 76));
            Gcd.gColor.add(RGB(9, 0, 38));
            Gcd.gColor.add(RGB(23, 19, 38));
            Gcd.gColor.add(RGB(127, 0, 255));
            Gcd.gColor.add(RGB(191, 127, 255));
            Gcd.gColor.add(RGB(82, 0, 165));
            Gcd.gColor.add(RGB(124, 82, 165));
            Gcd.gColor.add(RGB(63, 0, 127));
            Gcd.gColor.add(RGB(95, 63, 127));
            Gcd.gColor.add(RGB(38, 0, 76));
            Gcd.gColor.add(RGB(57, 38, 76));
            Gcd.gColor.add(RGB(19, 0, 38));
            Gcd.gColor.add(RGB(28, 19, 38));
            Gcd.gColor.add(RGB(191, 0, 255));
            Gcd.gColor.add(RGB(223, 127, 255));
            Gcd.gColor.add(RGB(124, 0, 165));
            Gcd.gColor.add(RGB(145, 82, 165));
            Gcd.gColor.add(RGB(95, 0, 127));
            Gcd.gColor.add(RGB(111, 63, 127));
            Gcd.gColor.add(RGB(57, 0, 76));
            Gcd.gColor.add(RGB(66, 38, 76));
            Gcd.gColor.add(RGB(28, 0, 38));
            Gcd.gColor.add(RGB(33, 19, 38));
            Gcd.gColor.add(RGB(255, 0, 255));
            Gcd.gColor.add(RGB(255, 127, 255));
            Gcd.gColor.add(RGB(165, 0, 165));
            Gcd.gColor.add(RGB(165, 82, 165));
            Gcd.gColor.add(RGB(127, 0, 127));
            Gcd.gColor.add(RGB(127, 63, 127));
            Gcd.gColor.add(RGB(76, 0, 76));
            Gcd.gColor.add(RGB(76, 38, 76));
            Gcd.gColor.add(RGB(38, 0, 38));
            Gcd.gColor.add(RGB(38, 19, 38));
            Gcd.gColor.add(RGB(255, 0, 191));
            Gcd.gColor.add(RGB(255, 127, 223));
            Gcd.gColor.add(RGB(165, 0, 124));
            Gcd.gColor.add(RGB(165, 82, 145));
            Gcd.gColor.add(RGB(127, 0, 95));
            Gcd.gColor.add(RGB(127, 63, 111));
            Gcd.gColor.add(RGB(76, 0, 57));
            Gcd.gColor.add(RGB(76, 38, 66));
            Gcd.gColor.add(RGB(38, 0, 28));
            Gcd.gColor.add(RGB(38, 19, 33));
            Gcd.gColor.add(RGB(255, 0, 127));
            Gcd.gColor.add(RGB(255, 127, 191));
            Gcd.gColor.add(RGB(165, 0, 82));
            Gcd.gColor.add(RGB(165, 82, 124));
            Gcd.gColor.add(RGB(127, 0, 63));
            Gcd.gColor.add(RGB(127, 63, 95));
            Gcd.gColor.add(RGB(76, 0, 38));
            Gcd.gColor.add(RGB(76, 38, 57));
            Gcd.gColor.add(RGB(38, 0, 19));
            Gcd.gColor.add(RGB(38, 19, 28));
            Gcd.gColor.add(RGB(255, 0, 63));
            Gcd.gColor.add(RGB(255, 127, 159));
            Gcd.gColor.add(RGB(165, 0, 41));
            Gcd.gColor.add(RGB(165, 82, 103));
            Gcd.gColor.add(RGB(127, 0, 31));
            Gcd.gColor.add(RGB(127, 63, 79));
            Gcd.gColor.add(RGB(76, 0, 19));
            Gcd.gColor.add(RGB(76, 38, 47));
            Gcd.gColor.add(RGB(38, 0, 9));
            Gcd.gColor.add(RGB(38, 19, 23));
            Gcd.gColor.add(RGB(0, 0, 0));
            Gcd.gColor.add(RGB(51, 51, 51));
            Gcd.gColor.add(RGB(102, 102, 102));
            Gcd.gColor.add(RGB(153, 153, 153));
            Gcd.gColor.add(RGB(204, 204, 204));
            Gcd.gColor.add(RGB(255, 255, 255));
            Gcd.gColor.add(RGB(255, 255, 255)); //By Layer
            Gcd.gColor.add(RGB(255, 255, 255)); //By Block
            Gcd.gColor.add(RGB(255, 255, 255)); //By Object?

            // corrijo los colores que no se ven contra el fondo
            Gcd.gColor[0] = config.WhiteAndBlack;
            Gcd.gColor[7] = config.WhiteAndBlack;
            Gcd.gColor[250] = config.WhiteAndBlack;
            Gcd.gColor[255] = config.WhiteAndBlack;
            Gcd.gColor[256] = config.WhiteAndBlack;
            Gcd.gColor[257] = config.WhiteAndBlack;
            Gcd.gColor[258] = config.WhiteAndBlack;

        }

        public static void InitMenus()
        {


            // colores
            void Menu(fMain)
            {

                Menu mItem;

                int i;
                int iColor;

                string sMenuPre;
                string sMenuPost;
                string sMenuSnap;

                mColors.Name = "mColores";

                //============ByLayer======================
                i = 256;
                iColor = Gcd.gColor[i];
                mItem = new Menu(mColors);
                mItem.Text = "ByLayer";
                mItem.Action = "Color_256"; // & CStr(i)
                mItem.Picture = paintPlus.picCirculito(8, Gcd.gColor[256], Color.ButtonForeground);

                //============Byblock======================
                i = 257;
                mItem = new Menu(mColors);
                mItem.Text = "ByBlock";
                mItem.Action = "Color_257"; // & CStr(i)
                mItem.Picture = paintPlus.picCirculito(8, Gcd.gColor[i], Color.ButtonForeground);

                //============separator======================
                mItem = new Menu(mColors);
                mItem.Text = "";
                mItem.Action = ""; // & CStr(i)

                for (i = 0; i <= 10; i + 1) //Gcd.gColor.Max
                {
                    mItem = new Menu(mColors);
                    mItem.Text = "Color " + CStr(i);
                    mItem.Action = "Color_" + CStr(i);
                    mItem.Picture = paintPlus.picCirculito(8, Gcd.gColor[i], Color.ButtonForeground);

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
                void Menu(fMai)
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
                    Gcd.SnapMode = Config.SnapModeSaved;
                    sMenuSnap = ";Snap to...;;<nothing>;_nothing;4;;4;End point;_end;end_point;4;MId point;_mId;mId_point;4;Perpendicular;_per;perpendicular;4";
                    sMenuSnap &= ";Quadrant;_qua;quadrant;4";
                    sMenuSnap &= ";Center;_cen;center_point;4";
                    sMenuSnap &= ";Intersection;_int;intersection;4";
                    sMenuSnap &= ";Tangent;_tan;tangent;4";
                    sMenuSnap &= ";Nearest;_nea;nearest;4";
                    sMenuSnap &= ";Base point;_bas;base_point;4";

                    sMenuPost = (";;;Cancel;_CANCEL;");

                    String[] stxMenus = [];

                    foreach (var vClass in Gcd.CCC)
                    {
                        Utils.MenuMakerPlus(fmain, vClass.gender, vClass.contextmenu + sMenuSnap + sMenuPost, Gcd.dirResources &/ "svg" &/ Config.IconFamily);
                    }

                }

 // carga los cursores desde SVG y los coloca en Gcd
public static void LoadCursors()
        {


            int c;
            string sCursor;

            sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor1.svg"), "#0066b3", Config.ModelBackgroundColor);
            Gcd.CursorCross = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

            sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor2.svg"), "#0066b3", Config.ModelBackgroundColor);
            Gcd.CursorSelect = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

            sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor3.svg"), "#0066b3", Config.ModelBackgroundColor);
            Gcd.CursorSelectAdd = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

            sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor4.svg"), "#0066b3", Config.ModelBackgroundColor);
            Gcd.CursorSelectRem = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

            sCursor = dsk.Contrary(System.IO.Path.Combine(Application.Path, "svg", "Cursors", "cursor5.svg"), "#0066b3", Config.ModelBackgroundColor);
            Gcd.CursorSelectXchange = new Cursor(Image.FromString(sCursor).Stretch(48, 48).Picture, 24, 24);

        }

    }
}