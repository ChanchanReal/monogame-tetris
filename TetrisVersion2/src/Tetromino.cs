using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TetrisVersion2.src
{
    public class Tetromino
    {
        public Texture2D Texture { get; private set; }
        public int Row { get; set; }
        public int Column { get; set; }
        public Color Color { get; private set; }
        

        public int[,] Shape;
        private int[,] previousRotatedShape;

        public Tetromino(int[,] shape, int row, int col , Texture2D texture) 
        {
            Row = row;
            Column = col;
            Shape = shape;
            Texture = texture;

            for (int i = 0; i < shape.GetLength(0); i++)
            {
                if (shape[i,0] != 0)
                {
                    Color = TetrominoColor(shape[i, 0]);
                    break;
                }
            }
        }

        public void Down()
        {
            Column++;
        }

        public void Left()
        {
            Row--;
            Row = Math.Clamp(Row, 0, 9);
        }
        public void Right()
        {
            Row++;
            Row = Math.Clamp(Row, 0, 9);
        }

        public void Rotate()
        {
            // hold previous location so can rotate back.
            previousRotatedShape = Shape;
            int[,] rotatedShape = new int[Shape.GetLength(1), Shape.GetLength(0)];

            // Transpose the matrix (swap rows and columns)
            for (int i = 0; i < Shape.GetLength(0); i++)
            {
                for (int j = 0; j < Shape.GetLength(1); j++)
                {
                    rotatedShape[j, Shape.GetLength(0) - 1 - i] = Shape[i, j];
                }
            }

            // Replace the original shape with the rotated shape
            Shape = rotatedShape;
        }
        private Color TetrominoColor(int num)
        {
            return num switch
            {
                1 => Color.LightCyan,
                2 => Color.Gold,
                3 => Color.Magenta,
                4 => Color.Turquoise,
                5 => Color.GreenYellow,
                6 => Color.Red,
                7 => Color.Orange,
                _ => Color.Black,
            };
        }

        public void RotateBack()
        {
            if (previousRotatedShape != null)
            {
                Shape = previousRotatedShape;
                previousRotatedShape = null;
            }
        }
    }

    public enum Shape
    {
        I = 0,
        O = 1,
        T = 2,
        J = 3,
        L = 4,
        S = 5,
        Z = 6,
    }
}
