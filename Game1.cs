using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Pac3D
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        private Model mapa; // armazenaremos o mapa aqui
        private Model bloco; // armazenaremos o bloco aqui
        private Model pastilha; //armazenaremos a pastilha

        private Vector3 Eye = new Vector3(-165, 10, -235); // posicao da camera 
        private Vector3 At = new Vector3(0, 0, 0); // alvo da camera 
        private Vector2 Angle = new Vector2(180, 0); // angulo da camera
        private Vector3 LastPos; //ultima posicao do jogador
        private Map mapManager; //gerencia os mapas
        private Ghost ink;
        private Ghost pink;
        private Ghost blink;
        private Ghost clyde;
        private BoundingBox[] blocks;
        private BoundingBox[] dots;
        private BoundingBox player;
        private int remainDots = 5;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            ContentManager content = new ContentManager(Services, "Content");
            Content.RootDirectory = "Content";
            this.blocks = new BoundingBox[50];
            this.dots = new BoundingBox[60];

            this.ink = new Ghost();
            this.pink = new Ghost();
            this.blink = new Ghost();
            this.clyde = new Ghost();

            this.ink.posicao = new Vector3(-25, 0, -10);
            this.pink.posicao = new Vector3(-320, 0, -10);
            this.blink.posicao = new Vector3(-25, 0, -305);
            this.clyde.posicao = new Vector3(-320, 0, -305);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferHeight = 600; // 480 de altura
            graphics.PreferredBackBufferWidth = 800; // 640 de largura
            graphics.IsFullScreen = false; // desabilita o modo tela cheia
            graphics.ApplyChanges(); // aplica as mudanças
            Window.Title = "PacMan3D"; // define um título à janela
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mapa = Content.Load<Model>("mapa"); //carrega o mapa
            bloco = Content.Load<Model>("bloco"); //carrega o bloco
            pastilha = Content.Load<Model>("pastilha"); //carrega a pastilha

            ink.ghost = Content.Load<Model>("ink"); //carrega o ink
            pink.ghost = Content.Load<Model>("pink"); //carrega a pink
            blink.ghost = Content.Load<Model>("blink"); //carrega o blink
            clyde.ghost = Content.Load<Model>("clyde"); //carrega o clyde
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            this.KeypadManage();
            this.ColisionManager();
            this.ink.move();
            this.pink.move();
            this.blink.move();
            this.clyde.move();
            base.Update(gameTime);
        }

        private void KeypadManage()
        {
            KeyboardState teclado = Keyboard.GetState();
            this.LastPos = this.Eye;

            if (teclado.IsKeyDown(Keys.Up))
            {
                this.Eye = this.At;
            }

            if (teclado.IsKeyDown(Keys.Right))
            {
                Angle.X += 1;
                if ((Angle.X + 1) == 360) Angle.X = 0;
            }

            if (teclado.IsKeyDown(Keys.Left))
            {
                Angle.X -= 1;
                if ((Angle.X - 1) == -1) Angle.X = 359;
            }

            At.X = Eye.X + (Single)(Math.Cos((Double)(Angle.X) * Math.PI / 180) *
               Math.Cos((Double)(Angle.Y) * Math.PI / 180));
            At.Z = Eye.Z + (Single)(Math.Sin((Double)(Angle.X) * Math.PI / 180) *
               Math.Cos((Double)(Angle.Y) * Math.PI / 180));
            At.Y = Eye.Y + (Single)(Math.Sin((Double)(Angle.Y) * Math.PI / 180));
            this.player = new BoundingBox(new Vector3(Eye.X - 1, Eye.Y - 1, Eye.Z - 1), new Vector3(Eye.X + 1, Eye.Y + 1, Eye.Z + 1));
        }

        private void ColisionManager()
        {
            if (Eye.X >= -10) this.Eye = this.LastPos;
            if (Eye.Z >= -10) this.Eye = this.LastPos;
            if (Eye.X <= -320) this.Eye = this.LastPos;
            if (Eye.Z <= -320) this.Eye = this.LastPos;

            if (ink.posicao.X <= -320 && ink.posicao.Z <= -300) ink.direction = 4;
            if (ink.posicao.X <= -320 && ink.posicao.Z >= -10) ink.direction = 3;
            if (ink.posicao.X >= -25 && ink.posicao.Z >= -10) ink.direction = 2;
            if (ink.posicao.X >= -25 && ink.posicao.Z <= -305) ink.direction = 1;

            if (pink.posicao.X <= -320 && pink.posicao.Z <= -300) pink.direction = 4;
            if (pink.posicao.X <= -320 && pink.posicao.Z >= -10) pink.direction = 3;
            if (pink.posicao.X >= -25 && pink.posicao.Z >= -10) pink.direction = 2;
            if (pink.posicao.X >= -25 && pink.posicao.Z <= -305) pink.direction = 1;

            if (blink.posicao.X <= -320 && blink.posicao.Z <= -300) blink.direction = 4;
            if (blink.posicao.X <= -320 && blink.posicao.Z >= -10) blink.direction = 3;
            if (blink.posicao.X >= -25 && blink.posicao.Z >= -10) blink.direction = 2;
            if (blink.posicao.X >= -25 && blink.posicao.Z <= -305) blink.direction = 1;

            if (clyde.posicao.X <= -320 && clyde.posicao.Z <= -300) clyde.direction = 4;
            if (clyde.posicao.X <= -320 && clyde.posicao.Z >= -10) clyde.direction = 3;
            if (clyde.posicao.X >= -25 && clyde.posicao.Z >= -10) clyde.direction = 2;
            if (clyde.posicao.X >= -25 && clyde.posicao.Z <= -305) clyde.direction = 1;


            foreach(BoundingBox box in blocks)
            {
                if (this.player.Intersects(box))
                {
                    Eye = LastPos;
                }
            }

            foreach (BoundingBox box in dots)
            {
                if (this.player.Intersects(box))
                {
                    this.Eye = this.At;
                    //remainDots--;
                    if (remainDots == 0)
                    {
                        this.Eye = new Vector3(-165, 10, -235);
                        remainDots = 500;
                    }
                }
            }

            if (ink.box.Intersects(player))
            {
                Eye = new Vector3(-165, 10, -235);
            }

            if (pink.box.Intersects(player))
            {
                Eye = new Vector3(-165, 10, -235);
            }

            if (blink.box.Intersects(player))
            {
                Eye = new Vector3(-165, 10, -235);
            }

            if (clyde.box.Intersects(player))
            {
                Eye = new Vector3(-165, 10, -235);
            }

            At.X = Eye.X + (Single)(Math.Cos((Double)(Angle.X) * Math.PI / 180) *
                Math.Cos((Double)(Angle.Y) * Math.PI / 180));
            At.Z = Eye.Z + (Single)(Math.Sin((Double)(Angle.X) * Math.PI / 180) *
               Math.Cos((Double)(Angle.Y) * Math.PI / 180));
            At.Y = Eye.Y + (Single)(Math.Sin((Double)(Angle.Y) * Math.PI / 180));
            this.player = new BoundingBox(new Vector3(Eye.X - 5, Eye.Y - 10, Eye.Z - 5), new Vector3(Eye.X + 5, Eye.Y + 10, Eye.Z + 5));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            this.mapManager = new Map(Eye, At, graphics);
            

            blocks[1] = this.mapManager.DrawBlock(bloco, -60, -30);
            blocks[2] = this.mapManager.DrawBlock(bloco, -90, -30);
            blocks[3] = this.mapManager.DrawBlock(bloco, -120, -30);
            blocks[4] = this.mapManager.DrawBlock(bloco, -150, -30);
            blocks[5] = this.mapManager.DrawBlock(bloco, -180, -30);
            blocks[6] = this.mapManager.DrawBlock(bloco, -210, -30);
            blocks[7] = this.mapManager.DrawBlock(bloco, -240, -30);
            blocks[8] = this.mapManager.DrawBlock(bloco, -270, -30);
            blocks[9] = this.mapManager.DrawBlock(bloco, -300, -30);

            blocks[10] = this.mapManager.DrawBlock(bloco, -60, -60);
            blocks[12] = this.mapManager.DrawBlock(bloco, -60, -90);
            blocks[13] = this.mapManager.DrawBlock(bloco, -60, -120);
            blocks[14] = this.mapManager.DrawBlock(bloco, -60, -150);
            blocks[15] = this.mapManager.DrawBlock(bloco, -60, -180);

            blocks[16] = this.mapManager.DrawBlock(bloco, -300, -60);
            blocks[17] = this.mapManager.DrawBlock(bloco, -300, -90);
            blocks[18] = this.mapManager.DrawBlock(bloco, -300, -120);
            blocks[19] = this.mapManager.DrawBlock(bloco, -300, -150);
            blocks[20] = this.mapManager.DrawBlock(bloco, -300, -180);

            blocks[21] = this.mapManager.DrawBlock(bloco, -120, -90);
            blocks[22] = this.mapManager.DrawBlock(bloco, -150, -90);
            blocks[23] = this.mapManager.DrawBlock(bloco, -180, -90);
            blocks[24] = this.mapManager.DrawBlock(bloco, -210, -90);
            blocks[25] = this.mapManager.DrawBlock(bloco, -240, -90);

            blocks[27] = this.mapManager.DrawBlock(bloco, -120, -150);
            blocks[28] = this.mapManager.DrawBlock(bloco, -120, -180);
            blocks[29] = this.mapManager.DrawBlock(bloco, -120, -210);
            blocks[30] = this.mapManager.DrawBlock(bloco, -120, -240);
            blocks[31] = this.mapManager.DrawBlock(bloco, -120, -270);
            blocks[32] = this.mapManager.DrawBlock(bloco, -90, -270);
            blocks[33] = this.mapManager.DrawBlock(bloco, -60, -270);

            blocks[34] = this.mapManager.DrawBlock(bloco, -240, -150);
            blocks[35] = this.mapManager.DrawBlock(bloco, -240, -180);
            blocks[36] = this.mapManager.DrawBlock(bloco, -240, -210);
            blocks[37] = this.mapManager.DrawBlock(bloco, -240, -240);
            blocks[38] = this.mapManager.DrawBlock(bloco, -240, -270);
            blocks[39] = this.mapManager.DrawBlock(bloco, -270, -270);
            blocks[40] = this.mapManager.DrawBlock(bloco, -300, -270);

            blocks[41] = this.mapManager.DrawBlock(bloco, -180, -180);

            blocks[42] = this.mapManager.DrawBlock(bloco, -180, -240);

            dots[0] = this.mapManager.DrawDots(pastilha, -20, -15);
            dots[1] = this.mapManager.DrawDots(pastilha, -45, -15);
            dots[2] = this.mapManager.DrawDots(pastilha, -75, -15);
            dots[3] = this.mapManager.DrawDots(pastilha, -105, -15);
            dots[4] = this.mapManager.DrawDots(pastilha, -135, -15);
            dots[5] = this.mapManager.DrawDots(pastilha, -165, -15);
            dots[6] = this.mapManager.DrawDots(pastilha, -195, -15);
            dots[7] = this.mapManager.DrawDots(pastilha, -245, -15);
            dots[8] = this.mapManager.DrawDots(pastilha, -275, -15);
            dots[9] = this.mapManager.DrawDots(pastilha, -315, -15);

            dots[10] = this.mapManager.DrawDots(pastilha, -315, -45);
            dots[11] = this.mapManager.DrawDots(pastilha, -315, -75);
            dots[12] = this.mapManager.DrawDots(pastilha, -315, -105);
            dots[13] = this.mapManager.DrawDots(pastilha, -315, -135);
            dots[14] = this.mapManager.DrawDots(pastilha, -315, -165);
            dots[15] = this.mapManager.DrawDots(pastilha, -315, -195);
            dots[16] = this.mapManager.DrawDots(pastilha, -315, -225);
            dots[17] = this.mapManager.DrawDots(pastilha, -315, -255);
            dots[18] = this.mapManager.DrawDots(pastilha, -315, -285);
            dots[19] = this.mapManager.DrawDots(pastilha, -315, -310);

            dots[20] = this.mapManager.DrawDots(pastilha, -20, -310);
            dots[21] = this.mapManager.DrawDots(pastilha, -45, -310);
            dots[22] = this.mapManager.DrawDots(pastilha, -75, -310);
            dots[23] = this.mapManager.DrawDots(pastilha, -105, -310);
            dots[24] = this.mapManager.DrawDots(pastilha, -135, -310);
            dots[25] = this.mapManager.DrawDots(pastilha, -165, -310);
            dots[26] = this.mapManager.DrawDots(pastilha, -195, -310);
            dots[27] = this.mapManager.DrawDots(pastilha, -215, -310);
            dots[28] = this.mapManager.DrawDots(pastilha, -245, -310);
            dots[29] = this.mapManager.DrawDots(pastilha, -275, -310);
            dots[30] = this.mapManager.DrawDots(pastilha, -315, -310);

            dots[31] = this.mapManager.DrawDots(pastilha, -20, -45);
            dots[32] = this.mapManager.DrawDots(pastilha, -20, -75);
            dots[33] = this.mapManager.DrawDots(pastilha, -20, -105);
            dots[34] = this.mapManager.DrawDots(pastilha, -20, -135);
            dots[35] = this.mapManager.DrawDots(pastilha, -20, -165);
            dots[36] = this.mapManager.DrawDots(pastilha, -20, -195);
            dots[37] = this.mapManager.DrawDots(pastilha, -20, -225);
            dots[38] = this.mapManager.DrawDots(pastilha, -20, -255);
            dots[39] = this.mapManager.DrawDots(pastilha, -20, -285);
            dots[40] = this.mapManager.DrawDots(pastilha, -20, -310);

            dots[41] = this.mapManager.DrawDots(pastilha, -195, -310);
            dots[42] = this.mapManager.DrawDots(pastilha, -195, -285);
            dots[43] = this.mapManager.DrawDots(pastilha, -195, -255);
            dots[44] = this.mapManager.DrawDots(pastilha, -195, -225);
            dots[45] = this.mapManager.DrawDots(pastilha, -195, -195);

            dots[46] = this.mapManager.DrawDots(pastilha, -135, -310);
            dots[47] = this.mapManager.DrawDots(pastilha, -135, -285);
            dots[48] = this.mapManager.DrawDots(pastilha, -135, -255);
            dots[49] = this.mapManager.DrawDots(pastilha, -135, -225);
            dots[50] = this.mapManager.DrawDots(pastilha, -135, -195);

            ink.box = ink.Draw(Eye, At, graphics);
            pink.box = pink.Draw(Eye, At, graphics);
            blink.box = blink.Draw(Eye, At, graphics);
            clyde.box = clyde.Draw(Eye, At, graphics);

            this.mapManager.DrawMap(mapa);
        }
    }
}
