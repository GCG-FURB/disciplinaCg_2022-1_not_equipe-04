/**
  Autor: Dalton Solano dos Reis
**/

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System.Threading;

namespace gcgcg
{
  internal class Player : ObjetoGeometria
  {
    private Ponto4D coordenadaIni;
    private double deslocamentoY = 300;
    public int pontuacao = 0;

    public Player(char rotulo, Objeto paiRef, Ponto4D coordenada) : base(rotulo, paiRef)
    {
      coordenadaIni = coordenada;
      Reseta();

      PrimitivaTamanho = 3;
    }

    public void MoverCima()
    {
      if (deslocamentoY + 60 >= 600)
        return;

      deslocamentoY += 10;
      MoveBBox();

      Thread.Sleep(10);
      Translacao(0, 10, 0);
    }

    public void MoverBaixo()
    {
      if (deslocamentoY - 60 <= 0)
        return;

      deslocamentoY -= 10;
      MoveBBox();

      Thread.Sleep(10);
      Translacao(0, -10, 0);
    }

    public void MarcaPonto()
    {
      ObjetoCor.CorR = 0;
      ObjetoCor.CorG = 0;
      ObjetoCor.CorB = 0;

      switch (++pontuacao)
      {
        case 1:
        {
          ObjetoCor.CorR = 255;
          ObjetoCor.CorG = 255;
        }
        break;

        case 2:
        {
          ObjetoCor.CorR = 75;
          ObjetoCor.CorG = 255;
        }
        break;
      }
    }

    private void MoveBBox()
    {
      if (PontosLista[0].X == 10)
      {
        BBox.Atribuir(new Ponto4D(20, deslocamentoY + 50));
        BBox.Atualizar(new Ponto4D(10, deslocamentoY - 50));
      }
      else
      {
        BBox.Atribuir(new Ponto4D(590, deslocamentoY + 50));
        BBox.Atualizar(new Ponto4D(580, deslocamentoY - 50));
      }
    }

    public void Reseta(bool resetaCor = false)
    {
      pontosLista.Clear();
      AtribuirIdentidade();

      PontosAdicionar(new Ponto4D(coordenadaIni.X - 5, coordenadaIni.Y - 50));
      PontosAdicionar(new Ponto4D(coordenadaIni.X + 5, coordenadaIni.Y - 50));
      PontosAdicionar(new Ponto4D(coordenadaIni.X + 5, coordenadaIni.Y + 50));
      PontosAdicionar(new Ponto4D(coordenadaIni.X - 5, coordenadaIni.Y + 50));

      BBox.Atribuir(PontosLista[0]);
      BBox.Atualizar(PontosLista[2]);

      deslocamentoY = 300;

      if (resetaCor)
      {
        ObjetoCor.CorR = 255;
        ObjetoCor.CorG = 255;
        ObjetoCor.CorB = 255;
      }
    }

    protected override void DesenharObjeto()
    {
      GL.Begin(PrimitiveType.Quads);
      foreach (Ponto4D pto in pontosLista)
      {
        GL.Vertex2(pto.X, pto.Y);
      }
      GL.End();
    }
  }
}