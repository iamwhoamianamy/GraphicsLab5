using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLab5
{
   public partial class Game : GameWindow
   {
      private Color4 backgroundColor;

      float mouseX = 0;
      float mouseY = 0;

      Spline spline;

      float scalingFactor = 1f;
      Vector2 translationVector;

      TextRenderer renderer;
      // Шрифты для вывода текста
      Font mono = new Font(FontFamily.GenericSansSerif, 20);

      public Game(int width, int height, string title) :
         base(width, height, GraphicsMode.Default, title)
      {

      }

      protected override void OnLoad(EventArgs e)
      {
         backgroundColor = Color4.White;
         translationVector = Vector2.Zero;

         spline = new Spline();

         renderer = new TextRenderer((int)(Width * 0.20), (int)(Height * 0.0500));
         base.OnLoad(e);
      }

      protected override void OnRenderFrame(FrameEventArgs e)
      {
         GL.Clear(ClearBufferMask.ColorBufferBit);

         UpdatePhysics();
         Draw();

         //Title = colony.ants.Count().ToString();
         Title = scalingFactor.ToString();

         Context.SwapBuffers();
         base.OnRenderFrame(e);
      }

      private void Draw()
      {
         GL.ClearColor(backgroundColor);
         DrawGrid();
         // Draw objects here

         //GL.PushMatrix();
         GL.Translate(Width / 2, Height / 2, 0);
         GL.Scale(scalingFactor, scalingFactor, scalingFactor);
         GL.Translate(-Width / 2, -Height / 2, 0);
         //GL.PopMatrix();

         GL.Translate(-translationVector.X, -translationVector.Y, 0);

         GL.Color4(Color4.Green);
         spline.DrawOutline();

         GL.Color4(Color4.Blue);
         spline.DrawSpline();
         spline.DrawControlPoints();

         GL.Color4(Color4.Red);
         spline.HighlightActive();

         GL.LoadIdentity();

      }

      private void UpdatePhysics()
      {

      }

      protected override void OnResize(EventArgs e)
      {
         GL.Disable(EnableCap.DepthTest);
         GL.Viewport(0, 0, Width, Height);
         GL.MatrixMode(MatrixMode.Projection);
         GL.LoadIdentity();
         GL.Ortho(0, Width, Height, 0, -1.0, 1.0);
         GL.MatrixMode(MatrixMode.Modelview);
         GL.LoadIdentity();

         base.OnResize(e);
      }

      private void DrawGrid()
      {
         int n = 10;
         float w = Width / 10.0f;
         float h = Height / 10.0f;

         // Отрисовка значений по оси абсцисс
         for (int i = 0; i < n; i++)
         {
            double coordX = (i * w - Width / 2 + translationVector.X * scalingFactor) / scalingFactor / Width * 10f;
            double coordY = translationVector.Y / Height * 10f;
            string str = "(" + coordX.ToString("f2") + ", " + coordY.ToString("f2") + ")";
            //DrawString(str, w * i, Height / 2, 70 / scalingFactor, 17.5f / scalingFactor);
            DrawString(str, w * i, Height / 2, 70, 17.5f);
         }

         // Отрисовка значений по оси ординат
         for (int i = 0; i < n; i++)
         {
            double coordX = translationVector.X / Width * 10f;
            double coordY = -1 * (i * h - Height / 2 + translationVector.Y * scalingFactor) / scalingFactor / Height * 10f;
            string str = "(" + coordX.ToString("f2") + ", " + coordY.ToString("f2") + ")";
            //DrawString(str, Height / 2, h * i, 70 / scalingFactor, 17.5f / scalingFactor);
            DrawString(str, Height / 2, h * i, 70, 17.5f);
         }

         // Отрисовка сетки
         GL.Color4(Color4.Gray);
         GL.LineWidth(1);

         GL.Begin(PrimitiveType.Lines);
         for (int i = 1; i < n; i++)
         {
            GL.Vertex2(0, h * i);
            GL.Vertex2(Width, h * i);

            GL.Vertex2(w * i, 0);
            GL.Vertex2(w * i, Height);
         }
         GL.End();

         // Отрисовка точек
         GL.Color4(Color4.Black);

         GL.Enable(EnableCap.PointSmooth);
         GL.PointSize(4);
         GL.Begin(PrimitiveType.Points);
         for (int i = 1; i < n; i++)
         {
            GL.Vertex2(Width / 2, h * i);

            GL.Vertex2(w * i, Height / 2);
         }
         GL.End();

         // Отрисовка координатных осей
         GL.Color4(Color4.Black);

         GL.LineWidth(2);
         GL.Begin(PrimitiveType.Lines);

         GL.Vertex2(0, Height / 2);
         GL.Vertex2(Width, Height / 2);

         GL.Vertex2(Width / 2, 0);
         GL.Vertex2(Width / 2, Height);

         GL.End();

      }

      private void DrawString(string str, float xPos, float yPos, float width, float height)
      {
         GL.Enable(EnableCap.Texture2D);
         renderer.Clear(Color.FromArgb(255, 255, 255, 255));
         renderer.DrawString(str, mono, Brushes.Black, new PointF(0, 0));

         GL.Color4(1.0f, 1.0f, 1.0f, 0.0f);

         GL.BindTexture(TextureTarget.Texture2D, renderer.Texture);

         GL.Begin(PrimitiveType.Quads);
         GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(xPos, yPos);
         GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(xPos + width, yPos);
         GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(xPos + width, yPos + height);
         GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(xPos, yPos + height);
         GL.End();

         GL.Disable(EnableCap.Texture2D);
      }

      protected override void OnUnload(EventArgs e)
      {
         renderer.Dispose();
      }

      private float InNewCoordsX(float val)
      {
         return (val - Width / 2) / scalingFactor + Width / 2 + translationVector.X;
      }
      private float InNewCoordsY(float val)
      {
         return (val - Height / 2) / scalingFactor + Height / 2 + translationVector.Y;
      }
   }

}
