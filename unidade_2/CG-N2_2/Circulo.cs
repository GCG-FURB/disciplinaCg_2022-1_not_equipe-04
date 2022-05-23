using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class Circulo : ObjetoGeometria
  {
    public Circulo(char rotulo, Objeto paiRef, int raio, int quantidadePontos) : base(rotulo, paiRef)
    {
        var anguloEntrePontos = (double)360 / quantidadePontos;

        for (int i = 0; i < quantidadePontos; i++)
        {
            var angulo = i * anguloEntrePontos;
            var ponto = Matematica.GerarPtosCirculo(angulo, raio);
            
            base.PontosAdicionar(ponto);
        }

        ObjetoCor.CorR = 255;
        ObjetoCor.CorG = 216;
        ObjetoCor.CorB = 0;
        PrimitivaTamanho = 5;
    }

    protected override void DesenharObjeto()
    {
      GL.Begin(PrimitiveType.Points);
      foreach (var pto in pontosLista)
      {
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
    }
  }
}