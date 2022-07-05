/**
  Autor: Dalton Solano dos Reis
**/

using OpenTK.Graphics.OpenGL;
using CG_Biblioteca;
using System;
using System.Threading;

namespace gcgcg
{
  internal class Bola : ObjetoGeometria
  {
    public int direcaoX;
    private int direcaoY;
    private int deslocamentoX;
    private int deslocamentoY;

    public Bola(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
    {
      Reseta();

      ObjetoCor.CorR = 0;
      ObjetoCor.CorG = 0;
      ObjetoCor.CorB = 255;
      PrimitivaTamanho = 3;
    }

    public void Mover()
    {
      if (direcaoY > 0 && BBox.obterMaiorY + 5 >= 600)
        direcaoY = -5;
      else if (direcaoY < 0 && BBox.obterMenorY - 5 <= 0)
        direcaoY = 5;

      //if (direcaoX > 0 && BBox.obterMaiorX + 10 >= 600)
      //  direcaoX = -10;
      //else if (direcaoX < 0 && BBox.obterMenorX - 10 <= 0)
      //  direcaoX = 10;

      deslocamentoX += direcaoX;
      deslocamentoY += direcaoY;

      var posX = direcaoX > 0 ? deslocamentoX + 5 : deslocamentoX - 5;
      var posY = direcaoY > 0 ? deslocamentoY + 5 : deslocamentoY - 5;
      BBox.Atribuir(new Ponto4D(posX, posY));

      posX = direcaoX > 0 ? deslocamentoX - 5 : deslocamentoX + 5;
      posY = direcaoY > 0 ? deslocamentoY - 5 : deslocamentoY + 5;
      BBox.Atualizar(new Ponto4D(posX, posY));

      Thread.Sleep(30);
      Translacao(direcaoX, direcaoY, 0);
    }

    public void Reseta(bool novoJogo = true)
    {
      pontosLista.Clear();
      AtribuirIdentidade();

      PontosAdicionar(new Ponto4D(290, 290));
      PontosAdicionar(new Ponto4D(310, 290));
      PontosAdicionar(new Ponto4D(310, 310));
      PontosAdicionar(new Ponto4D(290, 310));

      BBox.Atribuir(PontosLista[0]);
      BBox.Atualizar(PontosLista[2]);

      deslocamentoX = 300;
      deslocamentoY = 300;

      var random = new Random();
      direcaoY = random.Next() % 2 == 0 ? 5 : -5;

      if (novoJogo)
        direcaoX = random.Next() % 2 == 0 ? 10 : -10;
      else
        direcaoX = direcaoX > 0 ? -10 : 10;
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