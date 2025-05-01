using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace particlex.Core.Simulation;

public class Grid()
{
    private int Width { get; set; }
    private int Height { get; set; }
    public List<Row> Rows;

    public Grid(int width, int height) : this()
    {
        Width = width;
        Height = height;

        Rows = new List<Row>();
        for (int y = 0; y < height; y++)
        {
            Rows.Add(new Row(width));
        }
    }

    public int GetWidth()
    {
        return Width;
    }
    
    public int GetHeight()
    {
        return Height;
    }

    public Cell? GetCell(int x, int y)
    {
        var row = Rows.ElementAtOrDefault(y);

        var cell = row?.Cells.ElementAtOrDefault(x);

        return cell;
    }

    public void RemoveCell(int x, int y)
    {
        var row = Rows.ElementAtOrDefault(y);

        row?.Cells.RemoveAt(x);
    }
    
    public void SetCellType(int x, int y, CellType type)
    {
        if (x < 0 || y < 0 || x > Width - 1 || y > Height - 1)
        {
            return;
        }
        
        var cell = GetCell(x, y);
        if (cell != null)
        {
            return;
        }
        
        Rows[y].Cells[x] = new Cell(type);
        Rows[y].Cells[x].IsProcessed = true;
        Rows[y].Cells[x].CreatedDateTime = DateTime.Now;
    }

    public bool SwapCells(int x, int y, int swapX, int swapY)
    {
        var cell = GetCell(x, y);
        var swapCell = GetCell(swapX, swapY);
        var cellTmp = cell;
        
        if (swapCell == null)
        {
            try
            {
                Rows[y].Cells[x] = null;

                // skip if destination is out of bounds
                if (swapX > 0 && swapX <= Width-1 && swapY > 0 && swapY <= Height-1)
                {
                    Rows[swapY].Cells[swapX] = cell;
                    Rows[swapY].Cells[swapX].IsProcessed = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }
        
        var row = Rows.ElementAtOrDefault(y);
        row.Cells[x] = swapCell;
        
        row = Rows.ElementAtOrDefault(swapY);
        row.Cells[swapX] = cellTmp;

        cell.IsProcessed = true;
        swapCell.IsProcessed = true;

        return true;
    }

    public bool MoveCellToSides(int x, int y)
    {
        var leftCell = GetCell(x - 1, y);
        var rightCell = GetCell(x + 1, y);

        if (leftCell is not null && rightCell is null)
        {
            return SwapCells(x, y, x + 1, y);
        }

        if (rightCell is not null && leftCell is null)
        {
            return SwapCells(x, y, x - 1, y);
        }
        
        if (leftCell is null && rightCell is null)
        {
            var rnd = new Random();
            int move = rnd.Next(0, 2);
            if (move == 0)
            {
                move = -1;
            }
        
            return SwapCells(x, y, x + move, y);
        }

        return false;
    }
    
    public bool MoveCellToBottomSides(int x, int y)
    {
        var leftCell = GetCell(x - 1, y + 1);
        var rightCell = GetCell(x + 1, y + 1);

        if (leftCell is not null && rightCell is null)
        {
            return SwapCells(x, y, x + 1, y + 1);
        }

        if (rightCell is not null && leftCell is null)
        {
            return SwapCells(x, y, x - 1, y + 1);
        }

        
        if (leftCell is null && rightCell is null)
        {
            var rnd = new Random();
            int move = rnd.Next(0, 2);
            if (move == 0)
            {
                move = -1;
            }
        
            return SwapCells(x, y, x + move, y + 1);
        }

        return false;
    }

}
