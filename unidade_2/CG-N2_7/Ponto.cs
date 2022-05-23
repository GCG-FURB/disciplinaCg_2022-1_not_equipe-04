using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class Ponto : ObjetoGeometria
  {
    public Ponto(char rotulo, Objeto paiRef, Ponto4D ponto) : base(rotulo, paiRef)
    {
        ObjetoCor.CorR = 0;
        ObjetoCor.CorG = 0;
        ObjetoCor.CorB = 0;
        PrimitivaTamanho = 5;

        base.PontosAdicionar(ponto);
    }

    protected override void DesenharObjeto()
    {
      GL.Begin(PrimitiveType.Points);
        GL.Vertex2(pontosLista[0].X, pontosLista[0].Y);
      GL.End();
    }
  }
}