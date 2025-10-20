public class HatchPattern  
{
public string Name = "";
public string description = "";
public int type = 0;
public List<Pattern> patterns = [];   


// Gambas class file
// type: 0=User, 1=Predefined
}

public class Pattern
{
    public double AngleDeg;
    public double BaseX;
    public double BaseY;
    public double OffsetX;
    public double OffsetY;
    public double[] DashLength = [];
}