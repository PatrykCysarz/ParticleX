using System;
using System.Numerics;

namespace particlex.Core.Simulation;

// public enum CellType
// {
//     Empty = 0,
//     Sand = 1,
//     Wall = 2,
//     Water = 3,
//     Fire = 4,
// }

public enum CellType
{
    Solid = 0,
    Liquid = 1,
    Gas = 2,
    Particles = 3,
}

public class Cell(CellType type)
{
    public CellType Type = type;
    public bool IsProcessed = false;
    public DateTime CreatedDateTime = DateTime.Now;
}