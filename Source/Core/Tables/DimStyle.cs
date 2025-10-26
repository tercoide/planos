// Gambas class file
public class DimStyle
{
    public const string Gender = "DIMSTYLE";
    public string Name="";
    public int Flags;
    public string id="";
    public bool Avalilable;
    // ver
    // https://ezdxf.readthedocs.io/en/stable/tables/dimstyle_table_entry.html
    public string Thick1 = "_thick1_arrow";
    public string Thick2 = "_thick2_arrow";
    public string FormatLong = "0.00";    //// Formato para longitud
    public string FormatAng = "0.00";     //// Formato para angulos
                                          // DXF
    public string DIMPOST = "";           //// el sufijo de la dimension lineal
    public string DIMAPOST = "";          //// idem angulos
    public string DIMBLK = "";            //// nombre del bloque para las flechas o thicks
    public string DIMBLK1 = "";           //// idem 1
    public string DIMBLK2 = "";           //// idem 2
    public float DIMSCALE = 1;      //// el factor que multiplica la escala medida
    public float DIMASZ = 1;        //// el tamanio o escala de las flechas o bloques
    public float DIMEXO = 0.625F;           //// Desfase de línea de referencia
    public float DIMDLI;            //// Incremento de línea de cota
    public float DIMEXE = 1.25F;           //// Extensión de línea de referencia
    public float DIMRND;            //// Valor de redondeo para las distancias de cota
    public float DIMDLE = 1;           //// extension de la linea de cota
    public float DIMTP;             //// Tolerancia más
    public float DIMTM;             //// Tolerancia menos.

    public float DIMTXT = 1;          //// Dimensioning text height
    public float DIMCEN;            //// Tamaño de marcas y líneas centrales
    public float DIMTSZ;            //// Tamaño del trazo de acotación:
    public float DIMALTF;           //// Factor de escala de unidades alternativas
    public float DIMLFAC;           //// Factor de escala de medidas lineales
    public float DIMTVP;           //// Posición vertical del texto
    public float DIMTFAC;           //// Factor de escala de visualización de tolerancia de cota
    public float DIMGAP;            //// Espacio de la línea de cota
    public float DIMALTRND;         //// Determina el redondeo de las unidades alternativas
    public float DIMTOL;            //// Genera tolerancias de cota si su valor es distinto de cero
    public float DIMLIM;            //// Genera límites de cota si su valor es distinto de cero
    public float DIMTIH;            //// Tolerancia interior horizontal
    public float DIMTOH;            //// Tolerancia exterior horizontal

    public float DIMTAD;            //// Tolerancia adicional
    public float DIMZIN;            //// Z-incremento
    public float DIMAZIN;            //// Z-incremento
    public float DIMALT;            //// Alternativa
    public float DIMALTD;           //// Alternativa
    public float DIMTOFL;           //// Tolerancia de línea de referencia
    public float DIMSAH;           //// Tolerancia de línea de referencia
    public float DIMTIX;           //// Tolerancia de línea de referencia
    public float DIMSOXD;         //// Tolerancia de línea de referencia
    public float DIMCLRD;         //// Tolerancia de línea de referencia
    public float DIMCLRE;         //// Tolerancia de línea de referencia
    public float DIMCLRT;         //// Tolerancia de línea de referencia
    public float DIMADEC;         //// Número de decimales para los valores de tolerancia de una cota de unidades principales
    public float DIMUNIT;        //// Unidad de medida
    public float DIMDEC;         //// Número de decimales para los valores de tolerancia de una cota de unidades principales
    public float DIMTDEC;       //// Número de decimales para los valores de tolerancia de una cota de tolerancia
    public float DIMALTU;      //// Unidad de medida alternativa
    public float DIMALTTD;     //// Unidad de medida alternativa
    public float DIMAUNIT;     //// Unidad de medida alternativa
    public float DIMFRAC;      //// Unidad de medida alternativa
    public float DIMLUNIT;     //// Define las unidades para todos los tipos de cota excepto el angular:1 = Cient íficas; 2 = Decimales; 3 = Pies y pulgadas I;4 = Pies y pulgadas II; 5 = Fraccionarias; 6 = Escritorio de Windows
    public float DIMDSEP;     //// Unidad de medida alternativa
    public float DIMTMOVE;    //// Movimiento de texto de cota
    public float DIMJUST;     //// Posición horizontal del texto de cota:

    // 0 = Sobre la l ínea de cota y centrado entre las líneas de referencia
    // 1 = Sobre la l ínea de cota y a continuación de la primera línea de referencia
    // 2 = Sobre la l ínea de cota y a continuación de la segunda línea de referencia
    // 3 = Sobre la primera l ínea de referencia y centrado con respecto a ella
    // 4 = Sobre la segunda l ínea de referencia y centrado con respecto a ella
    public float DIMSD1;            //// Desplazamiento de la línea de referencia 1
    public float DIMSD2;            //// Desplazamiento de la línea de referencia 2
    public float DIMTOLJ;          //// Justificación de la tolerancia
    public float DIMTZIN;          //// Z-incremento
    public float DIMALTZ;         //// Z-incremento
    public float DIMALTTZ;      //// Z-incremento
    public float DIMFIT;         //// Ajuste de dimensión
    public float DIMUPT;        //// Actualización de dimensión
    public float DIMATFIT;     //// Ajuste automático de dimensión
    public string DIMTXSTY="";     //// Estilo de texto de dimensión
    public float DIMLDRBLK;    //// Nombre del bloque de flecha para las directrices

    public float DIMLWD;       //// Grosor de línea de dimensión
    public float DIMLWE;       //// Grosor de línea de extensión

    public float DIMSE1;      //// Suprimir línea de extensión 1
    public float DIMSE2;      //// Suprimir línea de extensión 2
}