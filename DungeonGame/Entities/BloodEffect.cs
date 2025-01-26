using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonGame.Entities;

public class BloodEffect : Effect
{
    private static readonly List<Texture2D> Textures = new();
    
    public BloodEffect(Texture2D texture) : base(texture)
    {
    }


    public static Texture2D GetRandomTexture2D(Game game)
    {
        if (Textures.Count == 0)
        {
            const float minTransparentAmount = 0.8f;
            var baseRect = new Rectangle(0, 0, 32, 32);

            for (var i = 1; i <= 4; i++)
            {
                var texture = new Texture2D(game.GraphicsDevice, baseRect.Width, baseRect.Height);
                var data = new Color[baseRect.Width * baseRect.Height];
                var currentTransparentAmount = 0.0f;
                var bloodColorRange = new List<Color>
                {
                    Color.Red,
                    Color.DarkRed,
                    Color.PaleVioletRed
                };


                for (var j = 0; j < data.Length; ++j)
                {
                    if (currentTransparentAmount >= minTransparentAmount)
                    {
                        data[j] = Color.Transparent;
                        continue;
                    }

                    var chancePerRow = (j % baseRect.Width) / (float) baseRect.Width;
                    var shouldPaint = Random.Shared.NextDouble() > chancePerRow;

                    if (shouldPaint)
                    {
                        data[j] = bloodColorRange[Random.Shared.Next(0, bloodColorRange.Count)];
                        currentTransparentAmount = 0.0f;
                    }
                    else
                    {
                        currentTransparentAmount += 0.1f;
                        data[j] = Color.Transparent * currentTransparentAmount;
                    }
                }


                texture.SetData(data);
                Textures.Add(texture);
            }
        }

        var pickedTexture = Textures[Random.Shared.Next(0, Textures.Count)];
        return pickedTexture;
    }
    
    public void SetPosition(Vector2 position)
    {
        Position = position;
    }
}