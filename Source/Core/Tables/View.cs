
using Gaucho;


    /// <summary>
    /// Represents a viewport/layout (converted from Gambas Viewport.class).
    /// The original Gambas file contained DXF documentation; this class exposes the key viewport parameters.
    /// </summary>
public class Viewport
{
    /// <summary>Original Gambas constant: "VIEWPORT"</summary>
    public const string Gender = "VIEWPORT";

    /// <summary>
    /// Id of the viewport/layout (not the entity id).
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Id of the entity whose block is this viewport (if any).
    /// </summary>
    public string IdEntity { get; set; }

    /// <summary>
    /// Pan offsets (pixels) applied inside the viewport (translation of the 0,0 corner).
    /// </summary>
    public double PanX { get; set; } = 0.0;

    public double PanY { get; set; } = 0.0;

    /// <summary>
    /// Scale (zoom factor). Default 1.0
    /// </summary>
    public double ScaleZoom { get; set; } = 1.0;

    /// <summary>
    /// Viewport name (DXF code 2).
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Lower-left corner of viewport (DCS / device coordinates).
    /// X0 = DXF code 10, Y0 = 20
    /// </summary>
    public double X0 { get; set; }

    public double Y0 { get; set; }

    /// <summary>
    /// Upper-right corner of viewport (DXF code 11 / 21).
    /// </summary>
    public double X1 { get; set; }

    public double Y1 { get; set; }

    /// <summary>
    /// List of frozen layer handles/ids (originally a Gambas Collection of handles).
    /// This models the DXF 331/441 list of frozen layers.
    /// </summary>
    public List<string> Layers { get; } = new List<string>();

    /// <summary>
    /// Whether the viewport is visible (default true).
    /// </summary>
    public bool Visible { get; set; } = true;

    /// <summary>
    /// Additional DXF/View/3D-related fields (placeholders for future use).
    /// Many DXF codes were present in the Gambas source; include some commonly useful ones.
    /// </summary>
    public double ViewHeight { get; set; } = 0.0;
    public double ViewCenterX { get; set; } = 0.0;
    public double ViewCenterY { get; set; } = 0.0;
    public double ViewTargetX { get; set; } = 0.0;
    public double ViewTargetY { get; set; } = 0.0;
    public double ViewTargetZ { get; set; } = 0.0;

    public Viewport() { }

    public override string ToString()
    {
        return $"Viewport(Id={Id}, Name={Name}, Bounds=[{X0},{Y0}]-[{X1},{Y1}], Pan=[{PanX},{PanY}], Scale={ScaleZoom}, Layers={Layers.Count})";
    }
}

/// <summary>
/// Represents a saved view (converted from Gambas View.class).
/// The original Gambas file contained DXF documentation; this class exposes the key view parameters.
/// </summary>
public class View
{
    /// <summary>Original Gambas constant: "VIEW"</summary>
    public const string Gender = "VIEW";

    /// <summary>
    /// Raw serialized extra data from the Gambas object (keeps original semantics).
    /// </summary>
    public string[] SData { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Identifier / handle of the view record.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Human-readable name (DXF group code 2).
    /// </summary>
    public string Name { get; set; }

    // VIEW-related properties (DXF codes documented in the original file)
    /// <summary>View height in DCS (code 40)</summary>
    public double ViewHeight { get; set; }

    /// <summary>View center point in DCS (code 10/20)</summary>
    public double CenterX { get; set; }
    public double CenterY { get; set; }

    /// <summary>View width in DCS (code 41)</summary>
    public double ViewWidth { get; set; }

    /// <summary>View direction vector from target (code 11/21/31)</summary>
    public double DirectionX { get; set; }
    public double DirectionY { get; set; }
    public double DirectionZ { get; set; }

    /// <summary>Target point (code 12/22/32)</summary>
    public double TargetX { get; set; }
    public double TargetY { get; set; }
    public double TargetZ { get; set; }

    /// <summary>Lens length (code 42)</summary>
    public double LensLength { get; set; }

    /// <summary>Front clipping plane offset from target point (code 43)</summary>
    public double FrontClip { get; set; }

    /// <summary>Back clipping plane offset from target point (code 44)</summary>
    public double BackClip { get; set; }

    /// <summary>Twist angle (code 50)</summary>
    public double TwistAngle { get; set; }

    /// <summary>View mode (code 71)</summary>
    public int ViewMode { get; set; }

    /// <summary>Render mode (code 281)</summary>
    public int RenderMode { get; set; }

    /// <summary>
    /// If true, this view has an associated UCS (code 72). When true the UCS fields below are valid.
    /// </summary>
    public bool HasUcs { get; set; }

    /// <summary>UCS origin (code 110/120/130)</summary>
    public double[] UcsOrigin { get; set; } = new double[3];

    /// <summary>UCS X axis (code 111/121/131)</summary>
    public double[] UcsXAxis { get; set; } = new double[3];

    /// <summary>UCS Y axis (code 112/122/132)</summary>
    public double[] UcsYAxis { get; set; } = new double[3];

    /// <summary>Orthographic type of UCS (code 79) - 0 = not orthographic, 1..6 = top/bottom/front/back/left/right</summary>
    public int OrthographicType { get; set; } = 0;

    /// <summary>UCS elevation (code 146)</summary>
    public double UcsElevation { get; set; }

    /// <summary>Optional background object handle (code 332)</summary>
    public string BackgroundObjectId { get; set; }

    /// <summary>Optional live section object handle (code 334)</summary>
    public string LiveSectionObjectId { get; set; }

    /// <summary>Optional visual style handle (code 348)</summary>
    public string VisualStyleId { get; set; }

    public View() { }

    public override string ToString()
    {
        return $"View(Id={Id}, Name={Name}, Center=[{CenterX},{CenterY}], Height={ViewHeight}, Width={ViewWidth}, Mode={ViewMode}, Render={RenderMode})";
    }
}

     public class UCS
    {
        public const string Gender = "UCS";

        /// <summary>DXF group code 2: name</summary>
        public string Name { get; set; }

        /// <summary>Handle / id</summary>
        public string Id { get; set; }

        /// <summary>DXF group code 70: flags (bit-coded)</summary>
        public int Flags70 { get; set; }

        /// <summary>DXF codes 10/20/30: origin (WCS)</summary>
        public Vector3 Origin { get; set; } = new Vector3();

        /// <summary>DXF codes 11/21/31: X axis direction (WCS)</summary>
        public Vector3 XAxis { get; set; } = new Vector3(1, 0, 0);

        /// <summary>DXF codes 12/22/32: Y axis direction (WCS)</summary>
        public Vector3 YAxis { get; set; } = new Vector3(0, 1, 0);

        /// <summary>DXF code 79: Valor (often 0)</summary>
        public int Valor79 { get; set; }

        /// <summary>DXF code 146: elevation</summary>
        public double Elevation { get; set; }

        /// <summary>
        /// DXF code 346: base UCS handle when this is an orthographic UCS.
        /// If not present and orthographic type is non-zero, base UCS is assumed WORLD.
        /// </summary>
        public string BaseUcs { get; set; }

        /// <summary>
        /// DXF code 71: orthographic type (0 = not orthographic, 1 = Top, 2 = Bottom, 3 = Front, ...)
        /// </summary>
        public int OrthographicType { get; set; }

        /// <summary>
        /// DXF codes 13/23/33: origin relative to this UCS for the specific orthographic type
        /// </summary>
        public Vector3 OriginForOrthographicType { get; set; } = new Vector3();

        public UCS() { }

        public UCS(string name, string id = null)
        {
            Name = name;
            Id = id;
        }

        public override string ToString()
        {
            return $"UCS(Name={Name}, Id={Id}, Origin={Origin}, XAxis={XAxis}, YAxis={YAxis}, OrthType={OrthographicType})";
        }
    }