using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class Primitiva : ObjetoGeometria
  {
    private PrimitiveType PrimitivaAtual;

    public Primitiva(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
    {
      ObjetoCor.CorR = 0;
      ObjetoCor.CorG = 0;
      ObjetoCor.CorB = 0;
      PrimitivaTamanho = 5;

      PrimitivaAtual = PrimitiveType.Points;
      DrawPoints();
    }

    private void DrawPoints()
    {
      base.PontosRemoverTodos();

      base.PontosAdicionar(new Ponto4D(200, 200));
      base.PontosAdicionar(new Ponto4D(-200, 200));
      base.PontosAdicionar(new Ponto4D(-200, -200));
      base.PontosAdicionar(new Ponto4D(200, -200));
    }

    private void DrawTriangle()
    {
      base.PontosRemoverTodos();

      base.PontosAdicionar(new Ponto4D(200, 200));
      base.PontosAdicionar(new Ponto4D(-200, 200));
      base.PontosAdicionar(new Ponto4D(-200, -200));
    }

    private void DrawTriangleStrip()
    {
      base.PontosRemoverTodos();

      //Triângulo Superior
      base.PontosAdicionar(new Ponto4D(200, 200));
      base.PontosAdicionar(new Ponto4D(-200, 200));
      base.PontosAdicionar(new Ponto4D(0, 0));

      //Triângulo Lateral
      base.PontosAdicionar(new Ponto4D(-200, 200));
      base.PontosAdicionar(new Ponto4D(-200, -200));
      base.PontosAdicionar(new Ponto4D(200, -200));
    }

    public void ProximaPrimitiva()
    {
      if (PrimitivaAtual == PrimitiveType.Points)
      {
        PrimitivaAtual = PrimitiveType.Lines;
      }
      else if (PrimitivaAtual == PrimitiveType.Lines)
      {
        PrimitivaAtual = PrimitiveType.LineLoop;
      }
      else if (PrimitivaAtual == PrimitiveType.LineLoop)
      {
        PrimitivaAtual = PrimitiveType.LineStrip;
      }
      else if (PrimitivaAtual == PrimitiveType.LineStrip)
      {
        PrimitivaAtual = PrimitiveType.Triangles;
        DrawTriangle();
      }
      else if (PrimitivaAtual == PrimitiveType.Triangles)
      {
        PrimitivaAtual = PrimitiveType.TriangleStrip;
        DrawTriangleStrip();
      }
      else if (PrimitivaAtual == PrimitiveType.TriangleStrip)
      {
        PrimitivaAtual = PrimitiveType.Quads;
        DrawPoints();
      }
    }

    protected override void DesenharObjeto()
    {
      GL.Begin(PrimitivaAtual);
      foreach (var pto in pontosLista)
      {
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
    }
  }
}