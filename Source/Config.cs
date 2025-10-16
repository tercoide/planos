
using System.Text.Json;
using System.Reflection;

namespace Gaucho
{
    public static class Config
    {
        // Gambas class file


        //TINCHO 2023.05.22 > Config.class implementation
        // STANDAR VARIABLES
        public static string Root="/usr/share/gambascad";
        public static string Depot="/usr/share/gambascad";
        public static string Log = "/usr/share/gambascad/log.txt";
        public static string ConfigFile = "/usr/share/gambascad/config.json";

        // GambasCAD Variables
        // INTERFACE
        public static bool ShowConsoleTab;          //Show console tab
        public static int ModelBackgroundColor;          //Model background color
        public static int ButtonSize = 32;
        public static bool ShowEntityInspector;          //Show entity inspector
        public static int DecimalDigitsCoords;          //Decimal digits for coordinates
        public static int DecimalDigitsInquiries;          //Decimal digits for inquiries
        public static string IconFamily="";          //Icon family

        // GRID
        public static int GrIdSize = 50;                  // pixels
        public static bool GrIdActive = true;              //
        public static int GrIdStyle = 0;                   // 0 = dots, 1 = lines
        public static bool GrIdBorder = false;     // TODO: borde no listo

        // Voy a agregar una variable
        //================================SNAP=========================================
        public static int SnapDistancePix = 32;  // Minimal distance to point
        public static int SnapModeSaved;
        public static int GripSize = 8;
        public static int GripProximityDistance = 16;
        public static double GripTextOnScreenSize = 10;
        public static int GripTextOnScreenColor = 0x808080; // Color.Gray equivalent
        public static string GripTextOnScreenFont = "romand";
        public static int GripLineColor = 0x808080; // Color.Gray equivalent

        // FORMATS
        public static int DigitsCoord = 2;
        public static int DigitsInquiries = 2;
        public static string FormatCoord = "0.00";
        public static string FormatInquiries = "0.00";
        public static bool DrawingAreaDarkMode = true;
        public static string FileConversion = "ODA";

        //NEW: Estas variables provienen del formulario main. En dicho formulario se estableceran pero sean guardadas siempre en esta clase y no se alterraran manualmente (de momento)
        //TINCHO 2023.05.22 > Config.class implementation
        public static string dirResources = Environment.CurrentDirectory;
        public static string dirDwgIn;
        public static string dirDxfIn;
        public static string dirDwgOut;
        public static string dirDxfOut;
        public static string dirTemplates;
        public static string dirBlocks;          //= gcd.dirResources &/ "library"
        public static string dirPrintStyles;
        public static string dirPatterns;
        public static string Home;

        //TODO: Estas faltan definir como seran editadas
        // ==============Variables de configuracion a guardar, agregar las necesarias=======================
        public static Byte CurrentColor = 1;
        public static double DefLineWt = 1;  //0.25 //mm  // en alguno sistemas 0.25 trae problemas y las lineas salen con huecos
        public static Byte CurrentLType = 1;
        public static double NuevoParametro = 1.2299;
        public static string otroparametro = "Gaucho pampa";

        public static int WindowBackColor;          //= Color.Blac  // Window backgrount color
        public static int WindowTextColor;          // Window helper text color
        public static int WindowInfoColor;          // Window helper text colorPublic flgWindowCursorColor As Integer // Window cursor color
        public static int WindowCursorColor;          // Window helper text color
        public static int WindowAIdsColor = 8;    // CAD color, not Gb

        public static int WhiteAndBlack;          // El color blanco/negro siempre sera distinto al
        public static int OnScreenHelpColor = 0xD3D3D3; // Color.LightGray equivalent
        public static int ColorForSelected = 0x70E000;
        public static int ColorForRemark = 0x0000FF00;
        public static double ArcDensity = 10;              // Pixels que tiene que tramo de un circulo
        public static bool AutoRegen = false;
        public static int AutoRegenFactor = 3;

        //===========================LAYOUT=========================================
        public static int[] SplitterH = [144];
        public static int[] SplitterV = [400];
        public static int[] SplRigth = [400];

        //===========================DETECCION=========================================
        public static bool TrackEntityLines = true;        // line polyline lwpolyline
        public static bool TrackEntityTexts = true;        // text mtext attrib attdef
        public static bool TrackEntityCurves = true;       // circle arc ellipse spline
        public static bool TrackEntityHatches = true;      // hatch
        public static bool TrackEntityDim = true;          // dimXXXX
        public static bool TrackEntityInserts = true;      // inserts
        public static bool ShowInspector = true;
        public static int TrackingIntervalMilisec = 50;
        public static int TrackMaxEntitiesNumber = 100;

        //===========================FILES AND DIRS=========================================
        //Public BlocksLibraryPath As String //TINCHO 2023.05.22 > Config.class implementation
        //Public ConfigPath As String //= User.Home &/ ".config/GambasCAD" // Reemplazada por Config.Depot
        public static string OdaPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        public static string OdaCommandLine = "ODAFileConverter.AppImage";
        public static int AutoSaveDelayMin = 15;
        public static string FilesLastPath = "";
        public static string FilesLastOpen1 = "";
        public static string FilesLastOpen2 = "";
        public static string FilesLastOpen3 = "";
        public static string FilesLastOpen4 = "";
        public static string FilesLastOpen5 = "";
        public static string FilesLastOpen6 = "";
        public static string FilesLastOpen7 = "";
        public static string FilesLastOpen8 = "";
        public static string FilesLastOpen9 = "";
        public static string FilesLastOpen10 = "";

        /// <summary>
        /// Saves all static fields of the Config class to a JSON file
        /// </summary>
        /// <param name="filePath">Path to the JSON file to save</param>
        public static void Save()
        {

            var filePath = ConfigFile;
            try
            {
                var configData = new Dictionary<string, object>();
                
                // Get all static fields using reflection
                var fields = typeof(Config).GetFields(BindingFlags.Public | BindingFlags.Static);
                
                foreach (var field in fields)
                {
                    var value = field.GetValue(null);
                    configData[field.Name] = value ?? "";
                }
                
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Serialize to JSON with indentation for readability
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                
                var jsonString = JsonSerializer.Serialize(configData, options);
                File.WriteAllText(filePath, jsonString);
                
                Console.WriteLine($"Config saved to: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving config: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Loads configuration from a JSON file and updates the static fields
        /// </summary>
        /// <param name="filePath">Path to the JSON file to load</param>
        public static void Load()
        {
            var filePath = ConfigFile;
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"Config file not found: {filePath}");
                    return;
                }
                
                var jsonString = File.ReadAllText(filePath);
                var configData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);
                
                if (configData == null) return;
                
                // Get all static fields using reflection
                var fields = typeof(Config).GetFields(BindingFlags.Public | BindingFlags.Static);
                
                foreach (var field in fields)
                {
                    if (configData.TryGetValue(field.Name, out var jsonValue))
                    {
                        try
                        {
                            object? value = ConvertJsonValue(jsonValue, field.FieldType);
                            if (value != null)
                            {
                                field.SetValue(null, value);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error setting field {field.Name}: {ex.Message}");
                        }
                    }
                }
                
                Console.WriteLine($"Config loaded from: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Converts JsonElement to the appropriate type
        /// </summary>
        private static object? ConvertJsonValue(JsonElement jsonValue, Type targetType)
        {
            if (targetType == typeof(string))
                return jsonValue.GetString();
            else if (targetType == typeof(int))
                return jsonValue.GetInt32();
            else if (targetType == typeof(double))
                return jsonValue.GetDouble();
            else if (targetType == typeof(bool))
                return jsonValue.GetBoolean();
            else if (targetType == typeof(byte))
                return jsonValue.GetByte();
            else if (targetType == typeof(int[]))
                return JsonSerializer.Deserialize<int[]>(jsonValue.GetRawText());
            else if (targetType == typeof(string[]))
                return JsonSerializer.Deserialize<string[]>(jsonValue.GetRawText());
            else
                return JsonSerializer.Deserialize(jsonValue.GetRawText(), targetType);
        }

//         public Config()
// {

// } 
    }

// public void Load(string sFile)
//     {


//     Collection cConfig ;         
//     string sSymbol ;         
//     Object  obj = Me;
//     Class  MyClass = Object.Class(obj);

//     if ( Exist(sFile) )
//     {
//         cConfig = JSON.FromString(File.Load(sFile));
//         foreach ( var sSymbol in myClass.Symbols)
//         {
//             if ( cConfig.Exist(sSymbol) )
//             {
//                 Object.SetProperty(obj, sSymbol, cConfig[sSymbol]);
//             }
//         }
//     }

// }

// static public void Save(string sFile, JSONCollection j)
//     {


//     JSONCollection jConfig ;         
//     Object  obj = Me;
//     Class  MyClass = Object.Class(obj);
//     string sSymbol ;         
//     string Var ;         
//     Variant Valor ;         

//     if ( ! Exist(File.Dir(sFile)) )
//     {
//         Shell "mkdir -p " + File.Dir(sFile) Wait;
//     }

//     if ( j )
//     {
//         jConfig = j.Copy();
//         foreach ( sSymbol in myClass.Symbols)
//         {
//             if ( jConfig.Exist(sSymbol) )
//             {
//                 Object.SetProperty(obj, sSymbol, jConfig[sSymbol]);
//             }
//         }
//     }
//     else
//     {
//         foreach ( Var in myClass.Symbols)
//         {
//             if ( (MyClass[var].kind == Class.Variable) || (MyClass[var].kind == Class.Property) )
//             {
//                 valor = Object.GetProperty(obj, var);
//                 jConfig.Add(Valor, var);
//             }
//         }
//     }

//     File.Save(sFile, JSON.Encode2(jConfig));

// }

// static public  JSONCollection List()
//     {


//     New JSONCollection j ;         
//     Object  obj = Me;
//     Class  MyClass = Object.Class(obj);
//     string sName ;         
//     Variant v1 ;         
//     Variant v2 ;         
//     Collection oBehaviour ;         
//     string sTitle ;         
//     New Variant[] vTemp ;         
//     int i ;         

//     oBehaviour = Behaviour();

//      //For Each sName In myClass.Symbols
//     foreach ( sName in oBehaviour.Keys)
//     {
//         if ( myClass.Symbols.Exist(sName) )
//         {
//             vTemp.Clear;
//             if ( (MyClass[sName].kind == Class.Variable) || (MyClass[sName].kind == Class.Property) )
//             {
//                 v1 = Object.GetProperty(obj, sName);

//                 vTemp.Add(v1);
//                 vTemp.Add(MyClass[sName].Type);
//                 sTitle = sName;
//                  //If oBehaviour.Exist(sName) Then
//                 if ( oBehaviour[sName][0] != "" )
//                 {
//                     sTitle = oBehaviour[sName][0];
//                 }
//                  //Endif
//                 vTemp.Add(sTitle);
//                  //If oBehaviour.Exist(sName) Then
//                 for ( i = 1; i <= oBehaviour[sName].Max; i + 1)
//                 {
//                     v2 = oBehaviour[sName][i];
//                     vTemp.Add(v2);
//                 }
//                  //Endif

//                 j.Add(vTemp.Copy(), sName);
//                 vTemp.Clear;

//             }
//         }
//     }

//     return j;

// }
} // End of Gaucho namespace

