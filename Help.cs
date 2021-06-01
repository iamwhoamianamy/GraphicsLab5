using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GraphicsLab5
{
   public static class Help
   {
      public static void DrawRect(float centX, float centY, float w, float h)
      {
         GL.Begin(PrimitiveType.LineLoop);

         GL.Vertex2(centX - w / 2, centY - h / 2);
         GL.Vertex2(centX + w / 2, centY - h / 2);
         GL.Vertex2(centX + w / 2, centY + h / 2);
         GL.Vertex2(centX - w / 2, centY + h / 2);

         GL.End();
      }
   }
}
