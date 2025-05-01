using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace particlex.Core.Simulation;

public class Simulation(int width, int height)
{
    public Grid Grid = new(width, height);
    private DateTime LastUpdateTime;

    public void Update()
    {
        if (LastUpdateTime > DateTime.Now.AddMilliseconds(-50))
        {
            // slows down simulation for debugging purposes
            // return;
        }
        LastUpdateTime = DateTime.Now;
        
        RefreshProcessedStatus();

        var watch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int y = 0; y < Grid.GetHeight(); y++)
        {
            var row = Grid.Rows.ElementAtOrDefault(y);
            for (int x = 0; x < Grid.GetWidth(); x++)
            {
                var cell = row.Cells.ElementAtOrDefault(x);
                if (cell == null)
                {
                    continue;
                }

                if (cell.IsProcessed)
                {
                    continue;
                }

                if (cell.Type == CellType.Solid)
                {
                    continue;
                }
                
                if (cell.Type == CellType.Particles)
                {
                    var bottomCell = Grid.GetCell(x, y + 1);
                    if (bottomCell == null)
                    {
                        Grid.SwapCells(x, y, x, y + 1);
                        continue;
                    }

                    if (bottomCell.Type == CellType.Liquid)
                    {
                        Grid.SwapCells(x, y, x, y + 1);
                        continue;
                    }
                    
                    Grid.MoveCellToBottomSides(x, y);
                }
                
                if (cell.Type == CellType.Liquid)
                {
                    var bottomCell = Grid.GetCell(x, y + 1);

                    if (bottomCell == null)
                    {
                        Grid.SwapCells(x, y, x, y + 1);
                        continue;
                    }

                    Grid.MoveCellToSides(x, y);
                }
                
                if (cell.Type == CellType.Gas)
                {
                    if (cell.CreatedDateTime < DateTime.Now.AddSeconds(-2))
                    {
                        Grid.RemoveCell(x, y);
                    }
                    
                    var topCell = Grid.GetCell(x, y - 1);
                    var rnd = new Random();
                    int move = rnd.Next(0, 2);

                    if (move == 0)
                    {
                        if (topCell == null)
                        {
                            Grid.SwapCells(x, y, x, y - 1);
                            continue;
                        }
                    }

                    if (move == 1)
                    {
                        Grid.MoveCellToSides(x, y);
                    }
                }
            }
        }
        
        watch.Stop();
        // Console.WriteLine("Grid processing time: " + watch.ElapsedMilliseconds + "ms");
    }

    public void CreateBorder()
    {
        for (int x = 0; x < Grid.GetWidth() - 1; x++)
        {
            Grid.SetCellType(x, 0, CellType.Solid);
            Grid.SetCellType(x, Grid.GetHeight() - 1, CellType.Solid);
        }
        
        for (int y = 0; y < Grid.GetHeight() - 1; y++)
        {
            Grid.SetCellType(0, y, CellType.Solid);
            Grid.SetCellType(Grid.GetWidth() - 1, y, CellType.Solid);
        }
    }

    private void RefreshProcessedStatus()
    {
        for (int y = 0; y < Grid.GetHeight(); y++)
        {
            var row = Grid.Rows.ElementAtOrDefault(y);
            for (int x = 0; x < Grid.GetWidth(); x++)
            {
                var cell = row.Cells.ElementAtOrDefault(x);
                if (cell == null)
                {
                    continue;
                }

                cell.IsProcessed = false;
            }
        }
    }
}