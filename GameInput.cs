using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GraphicsLab5
{
   public partial class Game
   {
      protected override void OnKeyDown(KeyboardKeyEventArgs e)
      {
         switch (e.Key)
         {
            case Key.Enter:
            {
               translationVector.X = 0f;
               translationVector.Y = 0f;
               scalingFactor = 1f;

               GL.LoadIdentity();
               break;
            }
            case Key.M:
            {
               spline.splineType = (SplineType)(((int)(spline.splineType) + 1) % 3);
               break;
            }
         }
         base.OnKeyDown(e);
      }

      protected override void OnMouseDown(MouseButtonEventArgs e)
      {
         switch (e.Button)
         {
            case MouseButton.Left:
            {
               spline.points.Add(new Vector2(InNewCoordsX(mouseX), InNewCoordsY(mouseY)));
               break;
            }
            case MouseButton.Right:
            {
               int closest = spline.FindClosest(new Vector2(InNewCoordsX(mouseX), InNewCoordsY(mouseY)));
               if (closest != -1)
               {
                  spline.isAnyActive = true;
                  spline.active = closest;
               }
               break;
            }
         }

         base.OnMouseDown(e);
      }

      protected override void OnMouseMove(MouseMoveEventArgs e)
      {
         if (e.Mouse.IsButtonDown(MouseButton.Middle))
         {
            //GL.Translate(e.XDelta, e.YDelta, 0);
            translationVector -= new Vector2(e.XDelta, e.YDelta) / scalingFactor;
         }
         else
         {
            if (e.Mouse.IsButtonDown(MouseButton.Right))
            {
               if (spline.isAnyActive)
               {
                  spline.points[spline.active] = new Vector2(InNewCoordsX(mouseX), InNewCoordsY(mouseY));
               }
            }
         }

         mouseX = e.X;
         mouseY = e.Y;

         base.OnMouseMove(e);
      }

      protected override void OnMouseWheel(MouseWheelEventArgs e)
      {
         scalingFactor += e.Delta * 0.1f;
         //float fac = 1 + e.Delta * 0.05f;

         //GL.Translate(Width / 2f + translationVector.X, Height / 2f + translationVector.Y, 0);
         //GL.Translate(-Width / 2f - translationVector.X, -Height / 2f - translationVector.Y, 0);

         base.OnMouseWheel(e);
      }
   }
}
