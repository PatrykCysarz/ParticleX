using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using particlex.Core.Simulation;

namespace particlex.Core.Configuration.Model;

public class Configuration
{
    public List<Cell> Cells { get; set; } = [];

    public Color GetColorForCellType(CellType? cellType)
    {
        if (cellType == null)
        {
            return new Color(0, 0, 0);
        }
        
        var cellConfiguration = Cells.Single(c => c.Type == (int) cellType);
        
        return new Color(cellConfiguration.ColorR, cellConfiguration.ColorG, cellConfiguration.ColorB);
    }
}

public class Cell
{
    public string Name { get; set; } = string.Empty;
    public int Type { get; set; }
    public int ColorR { get; set; }
    public int ColorG { get; set; }
    public int ColorB { get; set; }
}