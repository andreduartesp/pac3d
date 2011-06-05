using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Pac3D
{
    class Map
    {
        GraphicsDeviceManager graphics;
        private Vector3 Eye = new Vector3(-20, 10, -20); // posicao da camera 
        private Vector3 At = new Vector3(0, 0, 0); // alvo da camera 
        private Vector2 Angle = new Vector2(180, 0); // angulo da camera

        public Map(Vector3 Eye, Vector3 At, GraphicsDeviceManager graphics)
        {
            this.Eye = Eye;
            this.At = At;
            this.graphics = graphics;
        }

        public void DrawMap(Model mapa)
        {
            Matrix[] transforms = new Matrix[mapa.Bones.Count];
            mapa.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in mapa.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = Matrix.CreateLookAt(Eye, At, Vector3.Up); 
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        graphics.GraphicsDevice.Viewport.Width/graphics.GraphicsDevice.Viewport.Height,
                        1.0f, 10000.0f);
                }
                mesh.Draw();
            }
        }

        public BoundingBox DrawBlock(Model bloco, float posX, float posZ)
        {
            Matrix[] transforms = new Matrix[bloco.Bones.Count];
            
            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            BoundingBox boundingBox;
            Matrix matTransform;

            
            foreach (ModelMesh mesh in bloco.Meshes)
            {
                bloco.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(new Vector3(posX, 0, posZ));
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
            boundingBox.Min.X += posX;
            boundingBox.Max.X += posX;

            boundingBox.Min.Z += posZ;
            boundingBox.Max.Z += posZ;
            return boundingBox;
        }

        public BoundingBox DrawDots(Model pastilha, float posX, float posZ)
        {
            Matrix[] transforms = new Matrix[pastilha.Bones.Count];

            Vector3 Max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 Min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            BoundingBox boundingBox;
            Matrix matTransform;


            foreach (ModelMesh mesh in pastilha.Meshes)
            {
                pastilha.CopyAbsoluteBoneTransformsTo(transforms);
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = Matrix.CreateTranslation(new Vector3(posX, 5, posZ));
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
            boundingBox.Min.X += posX;
            boundingBox.Max.X += posX;

            boundingBox.Min.Z += posZ;
            boundingBox.Max.Z += posZ;

            boundingBox.Min.Y += 5;
            boundingBox.Max.Y += 5;
            return boundingBox;
        }
    }
}
