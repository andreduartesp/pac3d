using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pac3D
{
    class Ghost
    {

        public Model ghost { get; set; }
        public int direction { get; set; }
        public Vector3 posicao { get; set; }
        public BoundingBox box { get; set; }

        public Ghost()
        {
            direction = 1;
        }

        public void move()
        {
            switch (direction)
            {
                case 1:
                    posicao = new Vector3(posicao.X - 0.7f, posicao.Y, posicao.Z);
                    break;
                case 2:
                    posicao = new Vector3(posicao.X, posicao.Y, posicao.Z - 0.7f);
                    break;
                case 3:
                    posicao = new Vector3(posicao.X + 0.7f, posicao.Y, posicao.Z);
                    break;
                case 4:
                    posicao = new Vector3(posicao.X, posicao.Y, posicao.Z + 0.7f);
                    break;
            }
        }

        public BoundingBox Draw(Vector3 Eye, Vector3 At, GraphicsDeviceManager graphics)
        {
            Matrix[] transforms = new Matrix[ghost.Bones.Count];

            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            BoundingBox boundingBox;
            Matrix matTransform;


            foreach (ModelMesh mesh in ghost.Meshes)
            {
                ghost.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(this.posicao);
                    effect.EnableDefaultLighting();
                    effect.View = Matrix.CreateLookAt(Eye, At, Vector3.Up);
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        graphics.GraphicsDevice.Viewport.Width / graphics.GraphicsDevice.Viewport.Height,
                        1.0f, 10000.0f);
                }

                mesh.Draw();

                matTransform = transforms[mesh.ParentBone.Index];
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int stride = part.VertexStride;
                    int numberv = part.NumVertices;
                    byte[] data = new byte[stride * numberv];


                    mesh.VertexBuffer.GetData<byte>(data);

                    for (int ndx = 0; ndx < data.Length; ndx += stride)
                    {
                        float floatvaluex = BitConverter.ToSingle(data, ndx);
                        float floatvaluey = BitConverter.ToSingle(data, ndx + 4);
                        float floatvaluez = BitConverter.ToSingle(data, ndx + 8);
                        Vector3 vectCurrentVertex = new Vector3(floatvaluex, floatvaluey, floatvaluez);
                        Vector3 vectWorldVertex = Vector3.Transform(vectCurrentVertex, matTransform);

                        if (vectWorldVertex.X < Min.X) Min.X = vectWorldVertex.X;
                        if (vectWorldVertex.X > Max.X) Max.X = vectWorldVertex.X;
                        if (vectWorldVertex.Y < Min.Y) Min.Y = vectWorldVertex.Y;
                        if (vectWorldVertex.Y > Max.Y) Max.Y = vectWorldVertex.Y;
                        if (vectWorldVertex.Z < Min.Z) Min.Z = vectWorldVertex.Z;
                        if (vectWorldVertex.Z > Max.Z) Max.Z = vectWorldVertex.Z;
                    }
                }
            }
            boundingBox = new BoundingBox(Min, Max); // Cria a BoundingBox em volta do modelo
            boundingBox.Min.X += this.posicao.X;
            boundingBox.Max.X += this.posicao.X;

            boundingBox.Min.Z += this.posicao.Z;
            boundingBox.Max.Z += this.posicao.Z;

            boundingBox.Min.Y += 0;
            boundingBox.Max.Y += 0;
            return boundingBox;
        }
    }
}
