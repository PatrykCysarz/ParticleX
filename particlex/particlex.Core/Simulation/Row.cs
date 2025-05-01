using System.Collections.Generic;
using System.Numerics;

namespace particlex.Core.Simulation;

public class Row()
{
    public List<Cell?> Cells;

    public Row(int width) : this()
    {
        Cells = new List<Cell>();
        for (int i = 0; i < width; i++)
        {
            Cells.Add(null);
        }
    }
}