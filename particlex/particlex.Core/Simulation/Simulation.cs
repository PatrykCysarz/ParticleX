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
    }

    private void RefreshProcessedStatus()
    {
        for (int x = 0; x < Grid.GetWidth(); x++)
        {
            for (int y = 0; y < Grid.GetHeight(); y++)
            {
                var cell = Grid.GetCell(x, y);
                if (cell == null)
                {
                    continue;
                }

                cell.IsProcessed = false;
            }
        }
    }
}