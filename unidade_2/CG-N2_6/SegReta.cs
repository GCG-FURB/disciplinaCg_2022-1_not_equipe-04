using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class SegReta : ObjetoGeometria
  {
    public SegReta(char rotulo, Objeto paiRef, Ponto4D ptoInicio, Ponto4D ptoFim) : base(rotulo, paiRef)
    {
        base.PontosAdicionar(ptoInicio);
        base.PontosAdicionar(ptoFim);

        ObjetoCor.CorR = 0;
        ObjetoCor.CorG = 255;
        ObjetoCor.CorB = 255;
        PrimitivaTamanho = 3;
    }

    protected override void DesenharObjeto()
    {
      GL.Begin(PrimitiveType.Lines);
      foreach (var pto in pontosLista)
      {
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
    }
  }
}