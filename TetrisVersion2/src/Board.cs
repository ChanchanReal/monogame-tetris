using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TetrisVersion2.src
{
    internal class Board
    {
        public int[,] GameBoard;
        private Texture2D _gridTexture;
        private Tetris tetris;
        public Board(Texture2D gridTexture,Tetris tetris)
        {
            GameBoard = new int[20, 10];
            _gridTexture = gridTexture;
            this.tetris = tetris;
        }

        public void RenderBoard(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < GameBoard.GetLength(1); i++) 
            {
                for (int j = 0; j < GameBoard.GetLength(0); j++) 
                {
                    spriteBatch.Draw(_gridTexture, new Rectangle(i * 35, j * 35, 34, 34), TetrominoColor(GameBoard[j,i]));
                }
            }
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
        public void DisplayDebug()
        {
            Console.Clear();
            for (int i = 0; i < GameBoard.GetLength(0); i++)
            {
                for (int j = 0; j < GameBoard.GetLength(1); j++)
                {
                    Console.Write(GameBoard[i, j] + " ");
                }
                Console.WriteLine();
            }

        }

        public void DisplayPiece(Tetromino tetromino, SpriteBatch spriteBatch)
        {
            for (int i = 0; i < tetromino.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetromino.Shape.GetLength(1); j++)
                {
                    if (tetromino.Shape[i,j] != 0)
                    {
                        int updateX = tetromino.Row + i;
                        int updateY = tetromino.Column + j;
                        spriteBatch.Draw(_gridTexture, new Rectangle(updateX * 35, updateY * 35, 34, 34), tetromino.Color);
                    }
                    
                }
            }

            Console.SetCursorPosition(30, 0);
            Console.WriteLine(tetromino.Row + " " + tetromino.Column);
        }

        public void ClearFullLine()
        {

           for (int col = GameBoard.GetLength(0) - 1; col > 0; col--)
            {
                bool filled = true;

                for (int row = 0; row <= GameBoard.GetLength(1) - 1; row++)
                {
                    if (GameBoard[col, row] == 0)
                        filled = false;
                }

                if (filled)
                {
                    ShiftRowDown(col);
                    col += 1;
                    col = Math.Clamp(col, 0, GameBoard.GetLength(0) - 1);
                    tetris.AddScore(100);
                }
            }

        }
        public void ShiftRowDown(int column)
        {
            for (int col = column; col > 0; col--)
            for (int row = 0; row < GameBoard.GetLength(1); row++)
            {
                GameBoard[col, row] = GameBoard[col - 1, row];
            }
        }
        public void AddPiece(int[,] tetromino, int row, int col)
        {
            for (int x = 0; x < tetromino.GetLength(0); x++)
            {
                for (int y = 0; y < tetromino.GetLength(1); y++)
                {
                    if (tetromino[x, y] != 0)
                    {
                        int updateX = row + x;
                        int updateY = col + y;

                        GameBoard[updateY - 1, updateX] = tetromino[x, y];
                    }
                }
            }
        }

        public bool IsPositionValid(int[,] tetromino, int row, int col)
        {
            for (int y = 0; y < tetromino.GetLength(0); y++)
            {
                for (int x = 0; x < tetromino.GetLength(1); x++)
                {
                    if (tetromino[y, x] != 0)
                    {
                        int updateX = row + y;
                        int updateY = col + x;

                        // Check if the position is out of bounds (both upper and lower bounds)
                        if (updateX < 0 || updateX >= GameBoard.GetLength(1) ||
                            updateY < 0 || updateY >= GameBoard.GetLength(0))
                        {
                            return false;
                        }

                        // Check if the current cell in the game board is already occupied
                        if (GameBoard[updateY, updateX] != 0)
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
