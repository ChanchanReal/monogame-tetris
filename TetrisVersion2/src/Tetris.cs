using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TetrisVersion2.src
{
    public class Tetris
    {
        private Random random = new Random();
        private Board board;
        private List<Tetromino> tetrominos;
        private Tetromino currentTetromino;
        private Tetromino nextTetromino;
        private Tetromino holdTetromino;

        private Texture2D tetrominoTexture;
        private const float ButtonPressedDelay = 0.8f;
        private float fallDelay = 3f;
        private float fallTime = 0f;
        private float buttonPressed = 0f;
        private float InstaplacedDelay = 0f;

        private const int levelScoreThreshold = 10000;
        private int lastUpdateScore = 0;
        private const float speedUpFall = 0.3f;
        private const float increasePointPerLevel = 200;

        private int score = 0;
        private int level = 0;

        private Texture2D texture;
        private SpriteFont spriteFont;
        public Tetris() 
        {
            GenerateTetrominos();
            spriteFont = GameHelper.ContentManager.Load<SpriteFont>("font");
            texture = new Texture2D(GameHelper.GraphicsDevice, 1, 1);
            texture.SetData(new Color[] { Color.DarkGray });
            board = new Board(texture, this);
            tetrominoTexture = new Texture2D(GameHelper.GraphicsDevice, 1, 1);
            tetrominoTexture.SetData(new Color[] {Color.Blue});
            currentTetromino = GenerateTetromino();
            nextTetromino = GenerateTetromino();
        }

        public Tetromino GenerateTetromino()
        {
            if (tetrominos == null || tetrominos.Count <= 0)
            {
                GenerateTetrominos();
            }

            Tetromino tetromino = tetrominos.FirstOrDefault();
            tetrominos.Remove(tetromino);
            return tetromino;

        }
        public void Update()
        {
            AutoFallPiece();
            Controls();
            InstantPlacedTetromino();
            GameLevelSpeedUp();
            ComboScore(ref totalIncrementScore); // apply the combo multiplier

            if (comboDelay > 0)
            {
                comboDelay -= (float)GameHelper.GameTime.ElapsedGameTime.TotalMilliseconds; // decrease the delay timer over time
            }
        }

        private void GenerateTetrominos()
        {
            // using shuffle algorithm
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

            List<int[,]> tetrominoShape = new List<int[,]>();
            tetrominoShape.Add(i);
            tetrominoShape.Add(o);
            tetrominoShape.Add(t);
            tetrominoShape.Add(j);
            tetrominoShape.Add(l);
            tetrominoShape.Add(s);
            tetrominoShape.Add(z);

            List<Tetromino> tetrominoes = new List<Tetromino>();
            tetrominoShape.ForEach(shape => {
                tetrominoes.Add(new Tetromino(shape, 4, 0, texture));
            });

            ShuffleTetrominos(ref tetrominoes);

            tetrominos = tetrominoes;
        }

        private void ShuffleTetrominos(ref List<Tetromino> tetrominoList)
        {
            int n = tetrominoList.Count;

            while (n > 0)
            {
                n--;
                int k = random.Next(n + 1);
                Tetromino tetromino = tetrominoList[k];
                tetrominoList[k] = tetrominoList[n];
                tetrominoList[n] = tetromino;
            }
        }
        public void Controls()
        {
            if (buttonPressed <= 0)
            {
                if (InputManager.TapInput(Keys.T))
                {
                    buttonPressed = ButtonPressedDelay;
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
                    InstaplacedDelay = 1f;
                }
                if (InputManager.input(Keys.R))
                {
                    buttonPressed = ButtonPressedDelay;
                    currentTetromino.Rotate();
                    if (!board.IsPositionValid(currentTetromino.Shape,currentTetromino.Row, currentTetromino.Column))
                        currentTetromino.RotateBack();
                }

                if (InputManager.input(Keys.Right))
                {
                    buttonPressed = ButtonPressedDelay;

                    if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row + 1, currentTetromino.Column))
                    currentTetromino.Right();
                    InstaplacedDelay = 1f;
                }
                if (InputManager.input(Keys.Left))
                {
                    buttonPressed = ButtonPressedDelay;
                    if (board.IsPositionValid(currentTetromino.Shape, currentTetromino.Row - 1, currentTetromino.Column))
                        currentTetromino.Left();
                    InstaplacedDelay = 1f;
                }
                if (InputManager.input(Keys.Down))
                {
                    buttonPressed = ButtonPressedDelay;

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

                    InstaplacedDelay = 1f;
                }
                board.DisplayDebug();

            }

            board.ClearFullLine();
            buttonPressed -= 0.1f;
            InstaplacedDelay -= 0.1f;
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
            if (InstaplacedDelay > 0)
                return;

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
        private void GameLevelSpeedUp()
        {
            if (score >= lastUpdateScore + levelScoreThreshold)
            {
                lastUpdateScore += levelScoreThreshold; // Update the last score threshold
                fallDelay -= speedUpFall;               // Speed up the game by reducing fall delay
                level += 1;                             // Increase the game level
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
            spriteBatch.DrawString(spriteFont, "[Next]", new Vector2(465, 10), Color.AliceBlue);

            // holdpeice background
            PanelBackground(new Rectangle(465, 235, 210, 130), Color.Gray, spriteBatch);
            PanelBackground(new Rectangle(470, 240, 200, 120), Color.Black, spriteBatch);
            spriteBatch.DrawString(spriteFont, "[Hold]", new Vector2(465, 210), Color.AliceBlue);
            spriteBatch.DrawString(spriteFont, "[Score] : " + score, new Vector2(465, 510), Color.AliceBlue);
            spriteBatch.DrawString(spriteFont, "[Level] " + level, new Vector2(465, 530), Color.AliceBlue);
            spriteBatch.DrawString(spriteFont, "[Time]" + GameHelper.GameTime.TotalGameTime, new Vector2(465, 550), Color.AliceBlue);
        }
        private const float comboDuration = 1f;
        private float comboDelay = 0;
        private int currentCombo = 0;
        private int totalIncrementScore;
        // if this called multipleTimes we add add multiplier to the score
        public void ComboScore(ref int addedScore)
        {
            // If comboDelay has expired, reset the combo
            if (currentCombo > 1 && comboDelay <= 0)
            {
                addedScore *= currentCombo;
               
            }
        }
        public void AddScore(int addScore)
        {
            comboDelay = comboDuration; // reset the combo delay each time a new score is added
            int timeScore = (int)GameHelper.GameTime.TotalGameTime.Minutes;
            int timeBonus = 100 * timeScore;
            totalIncrementScore = addScore + timeBonus;
            score += totalIncrementScore; // increase the score

            if (comboDelay > 0)
            {
                currentCombo++;
            }
            else
            {
                currentCombo = 0; // reset the combo
            }
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
