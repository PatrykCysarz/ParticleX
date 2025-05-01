using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using particlex.Core.Simulation;

namespace particlex.Core.Ui;

public class Ui(Configuration.Model.Configuration configuration)
{
    public CellType SelectedCellType = CellType.Particles;
    private Configuration.Model.Configuration Configuration = configuration;

    public void Draw(SpriteBatch spriteBatch)
    {
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] {Color.White});

        var i = 0;
        foreach (Configuration.Model.Cell cellConfiguration in Configuration.Cells)
        {
            var type = (CellType) cellConfiguration.Type;
            var color = new Color(cellConfiguration.ColorR, cellConfiguration.ColorG, cellConfiguration.ColorB);
            
            if (SelectedCellType == type)
            {
                spriteBatch.Draw(texture, new Rectangle(i * 25, 25, 20, 20), Color.White);
            }
            spriteBatch.Draw(texture, new Rectangle(i * 25 + 4, 29, 11, 11), color);

            i++;
        }
    }
}