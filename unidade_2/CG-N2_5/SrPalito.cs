using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;

namespace gcgcg
{
  internal class SrPalito : ObjetoGeometria
  {
    private int angulo = 45;
    private int raio = 100;
    private int deslocamentoHori = 0;

    public SrPalito(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
    {
      ObjetoCor.CorR = 0;
      ObjetoCor.CorG = 0;
      ObjetoCor.CorB = 0;
      PrimitivaTamanho = 5;

      base.PontosAdicionar(new Ponto4D(0, 0));
      base.PontosAdicionar(Matematica.GerarPtosCirculo(angulo, raio));
    }

    public void MoveEsquerda()
    {
      base.pontosLista[0].X -= 2;
      base.pontosLista[1].X -= 2;

      deslocamentoHori -= 2;
    }

    public void MoveDireita()
    {
      base.pontosLista[0].X += 2;
      base.pontosLista[1].X += 2;

      deslocamentoHori += 2;
    }

    public void DiminuirRaio()
    {
      raio -= 2;

      Ponto4D pto = Matematica.GerarPtosCirculo(angulo, raio);
      base.pontosLista[1] = new Ponto4D(pto.X + deslocamentoHori, pto.Y);
    }

    public void AumentaRaio()
    {
      raio += 2;

      Ponto4D pto = Matematica.GerarPtosCirculo(angulo, raio);
      base.pontosLista[1] = new Ponto4D(pto.X + deslocamentoHori, pto.Y);
    }

    public void GirarEsquerda()
    {
      angulo -= 5;

      Ponto4D pto = Matematica.GerarPtosCirculo(angulo, raio);
      base.pontosLista[1] = new Ponto4D(pto.X + deslocamentoHori, pto.Y);
    }

    public void GirarDireita()
    {
      angulo += 5;
      
      Ponto4D pto = Matematica.GerarPtosCirculo(angulo, raio);
      base.pontosLista[1] = new Ponto4D(pto.X + deslocamentoHori, pto.Y);
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