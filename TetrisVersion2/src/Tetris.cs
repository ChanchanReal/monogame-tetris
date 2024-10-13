using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace TetrisVersion2.src
{
    public class Tetris
    {
        private Random random = new Random();
        private Board board;
        private Tetromino currentTetromino;
        private Tetromino nextTetromino;
        private Tetromino holdTetromino;

        private Texture2D tetrominoTexture;
        private float delay = 0.8f;
        private float fallDelay = 3f;
        private float fallTime = 0f;
        private float buttonPressed = 0f;

        private Texture2D texture;
        public Tetris() 
        {

            texture = new Texture2D(GameHelper.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.DarkGray });
            board = new Board(texture);
            tetrominoTexture = new Texture2D(GameHelper.GraphicsDevice, 1, 1);
            tetrominoTexture.SetData(new Color[] {Color.Blue});
            currentTetromino = GenerateTetromino();
            nextTetromino = GenerateTetromino();
        }

        public Tetromino GenerateTetromino()
        {
            int[,] i = { 
                { 1},
                { 1},
                { 1},
                { 1},
            };

            int[,] o = {
                { 2, 2},
                { 2, 2},
            };

            int[,] t = {
                { 3, 3, 3 },
                { 0, 3, 0 },
            };

            int[,] j = {
                { 0, 0, 4 },
                { 4, 4, 4 },
            };

            int[,] l = {
                { 5, 5, 5 },
                { 0, 0, 5 },
            };

            int[,] s = {
                { 0, 6, 6},
                { 6, 6, 0 },
            };

            int[,] z = {
                { 7, 7, 0},
                { 0, 7, 7},
            };

            List<int[,]> tetrominoes = new List<int[,]>();
            tetrominoes.Add(i);
            tetrominoes.Add(o);
            tetrominoes.Add(t);
            tetrominoes.Add(j);
            tetrominoes.Add(l);
            tetrominoes.Add(s);
            tetrominoes.Add(z);

            int index = random.Next(0, tetrominoes.Count);

            return new Tetromino(tetrominoes[index], 4, 0, tetrominoTexture);

        }
        public void Update()
        {
            AutoFallPiece();
            Controls();
            InstantPlacedTetromino();
        }

        public void Controls()
        {
            if (buttonPressed <= 0)
            {
                if (InputManager.TapInput(Keys.T))
                {
                    buttonPressed = delay;

                    if (holdTetromino == null)
                    {
                        holdTetromino = currentTetromino;
                        currentTetromino = nextTetromino;
                        holdTetromino.Column = 0;
                        holdTetromino.Row = 4;
                        nextTetromino = GenerateTetromino();
                    }
                    else
                    {
                        Tetromino temp = currentTetromino;
                        currentTetromino = holdTetromino;
                        holdTetromino = temp;
                        holdTetromino.Column = 0;
                        holdTetromino.Row = 4;
                    }
                }
                if (InputManager.input(Keys.R))
                {
                    buttonPressed = delay;
                    currentTetromino.Rotate();
                    if (!board.IsPositionValid(currentTetromino.Shape,currentTetromino.Row, currentTetromino.Column))
                        currentTetromino.RotateBack();
                }

                if (InputManager.input(Keys.Right))
                {
                    buttonPressed = delay;

                    if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row + 1, currentTetromino.Column))
                    currentTetromino.Right();
                }
                if (InputManager.input(Keys.Left))
                {
                    buttonPressed = delay;
                    if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row - 1, currentTetromino.Column))
                        currentTetromino.Left();
                }
                if (InputManager.input(Keys.Down))
                {
                    buttonPressed = delay;

                    int current = 1 + currentTetromino.Column;
                    if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row, current))
                    {
                        currentTetromino.Down();
                    }
                    else
                    {
                        board.AddPiece(currentTetromino.Shape, currentTetromino.Row, currentTetromino.Column + 1);
                        currentTetromino = nextTetromino;
                        nextTetromino = GenerateTetromino();
                    }

                }
                board.DisplayDebug();

            }

            board.ClearFullLine();
            buttonPressed -= 0.1f;
        }

        public void AutoFallPiece()
        {
            if (fallTime <= 0)
            {

                if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row, currentTetromino.Column + 1))
                {
                    currentTetromino.Down();

                    fallTime = fallDelay;
                }
                else
                {
                    board.AddPiece(currentTetromino.Shape, currentTetromino.Row, currentTetromino.Column + 1);
                    currentTetromino = nextTetromino;
                    nextTetromino = GenerateTetromino();
                    board.ClearFullLine();
                }
            }

            fallTime -= 0.1f;
        }

        public void InstantPlacedTetromino()
        {
            bool isPlaced = false;
            if (InputManager.TapInput(Keys.Space))
            {

                while (!isPlaced)
                {
                    int current = 1 + currentTetromino.Column;
                    if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row, current))
                    {
                        currentTetromino.Down();
                    }
                    else
                    {
                        isPlaced = true;
                    }
                }

                board.AddPiece(currentTetromino.Shape, currentTetromino.Row, currentTetromino.Column + 1);
                currentTetromino = nextTetromino;
                nextTetromino = GenerateTetromino();

                board.ClearFullLine();
            }
        }

        private void DisplayNextPiece(SpriteBatch sprite)
        {
            DisplayStaticTetromino(nextTetromino,sprite, 0, 0);
        }

        private void DisplayStaticTetromino(Tetromino tetromino, SpriteBatch sprite, int width, int height)
        {
            if (tetromino == null)
                return;

            for(int i = 0; i < tetromino.Shape.GetLength(0); i++)
            {
                for (int j = 0; j < tetromino.Shape.GetLength(1); j++)
                {
                    // center box tetromino
                    if (tetromino.Shape[i, j] == 2)
                    {
                        sprite.Draw(texture, new Rectangle((i * 35) + 500 + width, j * 35 + 65 + height, 34, 34), tetromino.Color);
                        continue;
                    }

                    // center line tetromino
                    if (tetromino.Shape[i, j] == 1)
                    {
                        sprite.Draw(texture, new Rectangle((i * 35) + 500 + width, j * 35 + 80 + height, 34, 34), tetromino.Color);
                        continue;
                    }
                    if (tetromino.Shape[i, j] != 0)
                    {
                        sprite.Draw(texture, new Rectangle((i * 35) + 500 + width, j * 35 + 50 + height, 34, 34), tetromino.Color);
                    }

                }
            }
        }
        private void DisplayHoldPiece(SpriteBatch sprite)
        {
            DisplayStaticTetromino(holdTetromino,sprite, 0, 200);
        }
        private void PanelBackground(Rectangle rectangle, Color color,SpriteBatch spriteBatch)
        {
           spriteBatch.Draw(texture, rectangle, color);
        }

        private void BackgroundLayer(SpriteBatch spriteBatch)
        {
            // background right panel
            PanelBackground(new Rectangle((int)(GameHelper.GameSize.X / 2) - 22, 0, GameHelper.GameSize.X, GameHelper.GameSize.Y), Color.Beige, spriteBatch);
            // background next piece
            PanelBackground(new Rectangle(465, 35, 210, 130), Color.Gray, spriteBatch);
            PanelBackground(new Rectangle(470, 40, 200, 120), Color.Black, spriteBatch);

            // holdpeice background
            PanelBackground(new Rectangle(465, 235, 210, 130), Color.Gray, spriteBatch);
            PanelBackground(new Rectangle(470, 240, 200, 120), Color.Black, spriteBatch);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            BackgroundLayer(spriteBatch);
            board.RenderBoard(spriteBatch);
            board.DisplayPiece(currentTetromino, spriteBatch);
            DisplayNextPiece(spriteBatch);
            DisplayHoldPiece(spriteBatch);
        }
    }
}
