using RetroGameFramework;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Media;

namespace RetroGameDemo
{
    internal class MyRetroGame : GameLogic
    {
        public MyRetroGame(GameConfig GameConfig) : base(GameConfig) { }

        // GameConfig is a variable already accessible in methods to retrieve the game configs
        // bool IsPaused() is a function already accessible in methods to check if the game is paused
        // void SetPaused(bool) is a function already accessible in methods to set the game in pause and to resume it

        // GAME DATA
        // Declare here game-specific data that should survive the frame
        float[] ballPosition; // ball position in screen pixels (float to consider also half pixels)
        float[] ballSpeed; // ball speed in pixels per frame (float to consider also half pixels)
        float[] melapox= {10,10};
        int ballColor = 1;
        int melacolor = 2;
        int avvelenata = -1;
        float[,] coda = new float[2, 20];
        int lunghezza = 5;
        Random fede = new Random();
        PaintStyle ballStyle = PaintStyle.Default;


        GameImage hearthImage = GameImage.CreateFromResource("hearth", AnchorType.Center);
        PaintStyle hearthStyle = PaintStyle.Default;

        // Initialization call, used to customize GameConfig data (used to customize the engine behaviour)
        protected override void OnInitGameConfig(GameConfig GameConfig)
        {
            GameConfig.Title = "Bouncing Ball";

            GameConfig.PixelsMatrixWidth = 64;
            GameConfig.PixelsMatrixHeight = 48;
            GameConfig.PixelSize = 10;

            GameConfig.FrameRate = 12;

            GameConfig.BackgroundColor = System.Drawing.Color.Black;
            //GameForm.Initializer.ForegroundColor = System.Drawing.Color.White;
            GameConfig.ForegroundColor = System.Drawing.Color.FromArgb(255, 255, 255);

            GameConfig.AdditionalColors = new System.Drawing.Color[] {
                System.Drawing.Color.Red,
                System.Drawing.Color.Orange,
                System.Drawing.Color.Yellow,
                System.Drawing.Color.Green,
                System.Drawing.Color.Cyan,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Violet,
           };
        }

        // Called at the start of the first frame of the game.
        // It's main purpose it's to setup the scene.
        private void FirstFrameLoop ()
        {
            // set the ball in the center of the screen
           
            ballPosition = new float[] { GameConfig.PixelsMatrixWidth / 2, GameConfig.PixelsMatrixHeight / 2 };

            // give the fall a speed
            ballSpeed = new float[] { 1, 0 };

            ballStyle.SetColorRemap(1, 2); // start from first additional color;

            hearthStyle.SetColorRemap(1, 2);
            hearthStyle.SetColorRemap(2, 8);
        }

        // Called once per frame, BEFORE the OnLoopGame event.
        protected override void OnClear(int[,] pixels)
        {
            GameUtils.ClearScreen(pixels);
        }

        // Called once per frame.
        // Here the actual logic happens.
        protected override void OnLoopGame(float deltaTime)
        {
            if (FrameCount == 0)
            {
                FirstFrameLoop();
            }
            else
            {
                UpdateBallPosition();
                melaposizione();
                sistemacoda();
            }
        }


        // Called once per frame, AFTER the OnLoopGame event.
        protected override void OnDraw(int[,] pixels)
        {
            int screenWidth = pixels.GetLength(0);
            int screenHeight = pixels.GetLength(1);
            DrawBall(pixels, ballColor);
            Drawmela(pixels, melacolor);
            Drawcoda(pixels, ballColor);
                
        }
        private void Drawcoda(int[,] pixels, int color)
        {
            for(int i=0; i<lunghezza; i++)
            {
                DrawPixel(pixels, coda[0,i], coda[1,i], color);
            }
        }
        private void sistemacoda()
        {
            int rows= coda.GetLength(0); 
            int cols = coda.GetLength(1);   

            for (int i = 0; i < rows; i++)
            {
                for (int j = cols - 1; j > 0; j--)
                {
                    coda[i, j] = coda[i, j - 1];
                }

                coda[i, 0] = 0;
            }

        }

        // Called at the end of the last frame of the game.
        // Its main purpose it's to dispose resources, as the game will end immediately after this call.
        protected override void OnEndGame()
        {
            Environment.Exit(0);
        }
        private void avvelenamento()
        {
            if (avvelenata == 1)
            {
                melacolor = 2;
            }
            else
            {
                if (fede.Next(1, 2) == 1)
                {
                    avvelenata = 2;
                    melacolor = 6;
                }
                else
                {
                    melacolor = 2;
                }
                
            }
            avvelenata -= 1;
        }
        private void melaposizione()
        {
            if (Math.Abs(ballPosition[0] - melapox[0]) < 2 && Math.Abs(ballPosition[1] - melapox[1]) < 2)
            {
                lunghezza+=2 ;
                avvelenamento();
                bool conferma = true;
                while (conferma)
                {
                    int controllo = 0;
                    melapox[0] = fede.Next(GameConfig.PixelsMatrixWidth - 4);
                    melapox[1] = fede.Next(GameConfig.PixelsMatrixHeight - 4);
                    if (Math.Abs(ballPosition[0] - melapox[0]) > 2 || Math.Abs(ballPosition[1] - melapox[1]) > 2)
                    {
                        for (int i = 0; i < lunghezza; i++)
                        {
                            if (Math.Abs(ballPosition[0] - melapox[0]) <= 2 || Math.Abs(ballPosition[1] - melapox[1]) <= 2)
                            {
                                controllo++;
                            }
                        }
                        if (controllo == 0)
                        {
                            conferma= false;
                        }
                    }
                }


            }

        }
        private void UpdateBallPosition()
        {

            coda[0, 0] = ballPosition[0];
            coda[1,0] = ballPosition[1];
            ballPosition[0] += ballSpeed[0];
            ballPosition[1] += ballSpeed[1];
            
            

            float ballRadius = 0f;

            if (ballSpeed[0] < 0 && ballPosition[0] - (ballRadius - 0.5f) <= 0) // horizontal check to the left
            {
                IsPaused();
                OnEndGame();
            }
            else if (ballSpeed[0] > 0 && ballPosition[0] + (ballRadius - 0.5) >= GameConfig.PixelsMatrixWidth - 1) // horizontal check to the right
            {
                IsPaused();
                OnEndGame();
            }

            if (ballSpeed[1] < 0 && ballPosition[1] - (ballRadius - 0.5f) <= 0) // vertical check to the top
            {
                IsPaused();
                OnEndGame();
            }
            else if (ballSpeed[1] > 0 && ballPosition[1] + (ballRadius - 0.5f) >= GameConfig.PixelsMatrixHeight - 1) // vertical check to the bottom
            {
                IsPaused();
                OnEndGame();
            }
            for(int i =0; i < lunghezza; i++)
            {
                if (ballPosition[0] == coda[0, i] && ballPosition[1] == coda[1, i])
                {
                    IsPaused();
                    OnEndGame();
                }
            }
        }
        private void DrawBall(int[,] pixels, int color)
        {
            DrawPixel(pixels, ballPosition[0],ballPosition[1],color);


        }
        private void Drawmela(int[,] pixels, int color)
        {
            DrawPixel(pixels, melapox[0], melapox[1]+1, color);
            DrawPixel(pixels, melapox[0], melapox[1]-1, 3);
            DrawPixel(pixels, melapox[0]+1, melapox[1], color);
            DrawPixel(pixels, melapox[0]-1, melapox[1], color);
            DrawPixel(pixels, melapox[0], melapox[1], color);
            DrawPixel(pixels, melapox[0] - 1, melapox[1]+1, color);
            DrawPixel(pixels, melapox[0] +1 , melapox[1]+1, color);
        }

        private static void DrawPixel(int[,] pixels, float x, float y, int color)
        {
            int posX = (int)x;
            int posY = (int)y;

            if (posX >= 0 && posX < pixels.GetLength(0)
                && posY >= 0 && posY < pixels.GetLength(1))
            {
                // X coordinate is the column index, while Y coordinate is the row index
                pixels[posX, posY] = color;
            }
        }

        // Called the first frame a key is pressed, and not called anymore unless the key is released
        protected override void OnKeyDown(Keys KeyCode)
        {
            if (!IsPaused())
            {
                float[] ballSpeedAbs = new float[] { Math.Abs(ballSpeed[0]), Math.Abs(ballSpeed[1]) };
                if(avvelenata!=0){
                    if((KeyCode == Keys.Up || KeyCode == Keys.W) && ballSpeed[0] != 0)
                    {
                        ballSpeed[0] = 0;
                        ballSpeed[1] = -1;
                    }
                    else if ((KeyCode == Keys.Down || KeyCode == Keys.S) && ballSpeed[0] != 0)
                    {
                        ballSpeed[0] = 0;
                        ballSpeed[1] = 1;
                    }
                    else if ((KeyCode == Keys.Right || KeyCode == Keys.D) && ballSpeed[1] != 0)
                    {
                        ballSpeed[0] = 1;
                        ballSpeed[1] = 0;
                    }
                    else if ((KeyCode == Keys.Left || KeyCode == Keys.A) && ballSpeed[1] != 0)
                    {
                        ballSpeed[0] = -1;
                        ballSpeed[1] = 0;
                    }
                }
                else
                {
                    if((KeyCode == Keys.Up || KeyCode == Keys.W) && ballSpeed[0] != 0)
                    {
                        ballSpeed[0] = 0;
                        ballSpeed[1] = 1;
                    }
                    else if ((KeyCode == Keys.Down || KeyCode == Keys.S) && ballSpeed[0] != 0)
                    {
                        ballSpeed[0] = 0;
                        ballSpeed[1] = -1;
                    }
                    else if ((KeyCode == Keys.Right || KeyCode == Keys.D) && ballSpeed[1] != 0)
                    {
                        ballSpeed[0] = -1;
                        ballSpeed[1] = 0;
                    }
                    else if ((KeyCode == Keys.Left || KeyCode == Keys.A) && ballSpeed[1] != 0)
                    {
                        ballSpeed[0] = 1;
                        ballSpeed[1] = 0;
                    }
                }
                if (KeyCode == Keys.P)
                {
                    SetPaused(true);
                }
                else if (KeyCode == Keys.C)
                {
                    int tmpColor = ballStyle.GetRemappedColor(PaintStyle.FOREGROUND_COLOR_INDEX);
                    tmpColor++;
                    if (tmpColor >= GameConfig.AdditionalColors.Length + 2)
                        tmpColor = 2;
                    ballStyle.SetColorRemap(PaintStyle.FOREGROUND_COLOR_INDEX, tmpColor);

                    ballColor++;
                    if (ballColor >= GameConfig.AdditionalColors.Length + 2)
                        ballColor = 2;
                }
            }
            else
            {
                if (KeyCode == Keys.P)
                {
                    SetPaused(false);
                }
            }
        }

        // Called if a key has been released (even in the same frame it has been released)
        protected override void OnKeyUp(Keys KeyCode)
        {
        
        }

        // Called during the frame a key is pressed and in all the following frames until it's released (excluding the frame it's released)
        protected override void OnKeyPress(Keys KeyCode)
        {
        
        }

    }
}
