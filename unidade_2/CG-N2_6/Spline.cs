using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
  internal class Spline : ObjetoGeometria
  {
    private int ptoControleSelecionado = 1;
    private List<Ponto4D> ptosControle = new List<Ponto4D>();
    private int qntdPtosSpline = 20;

    public int PtoSelecionado { set => ptoControleSelecionado = value; }
    public List<Ponto4D> PtosControle { get => ptosControle; }
    public int QntdPtosSpline { get => qntdPtosSpline; set => qntdPtosSpline = value; }

    public Spline(char rotulo, Objeto paiRef) : base(rotulo, paiRef)
    {
      ObjetoCor.CorR = 0;
      ObjetoCor.CorG = 0;
      ObjetoCor.CorB = 0;
      PrimitivaTamanho = 10;

      Reseta();
    }

    public void MoveCima()
    {
      ptosControle[ptoControleSelecionado - 1].Y += 2;
    }

    public void MoveBaixo()
    {
      ptosControle[ptoControleSelecionado - 1].Y -= 2;
    }

    public void MoveDireita()
    {
      ptosControle[ptoControleSelecionado - 1].X += 2;
    }

    public void MoveEsquerda()
    {
      ptosControle[ptoControleSelecionado - 1].X -= 2;
    }

    public void Reseta()
    {
      ptosControle.Clear();

      ptosControle.Add(new Ponto4D(-100, -100));
      ptosControle.Add(new Ponto4D(-100, 100));
      ptosControle.Add(new Ponto4D(100, 100));
      ptosControle.Add(new Ponto4D(100, -100));
    }

    private void DesenhaPontosControle()
    {
      //Desenha os pontos que não estão selecionados
      GL.Begin(PrimitiveType.Points);
        for (int i = 0; i < 4; i++)
        {
          if (i + 1 == ptoControleSelecionado)
            continue;

          GL.Vertex2(ptosControle[i].X, ptosControle[i].Y);
        }
      GL.End();

      //Desenha o ponto selecionado
      GL.Begin(PrimitiveType.Points);
        GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
        GL.Vertex2(ptosControle[ptoControleSelecionado - 1].X, ptosControle[ptoControleSelecionado - 1].Y);
      GL.End();
    }

    private Ponto4D CalculaPonto(Ponto4D ptoA, Ponto4D ptoB, double t)
    {
      Ponto4D ptoResultante = new Ponto4D();
      ptoResultante.X = ptoA.X + ((ptoB.X - ptoA.X) * t);
      ptoResultante.Y = ptoA.Y + ((ptoB.Y - ptoA.Y) * t);

      return ptoResultante;
    }

    private void DesenhaSpline()
    {
      double tamanhoPartes = 1d / qntdPtosSpline;
      Ponto4D ptoInicial = ptosControle[0];

      GL.LineWidth(3);
      GL.Begin(PrimitiveType.LineStrip);
        GL.Color3(Convert.ToByte(255), Convert.ToByte(216), Convert.ToByte(0));
        for (double t = 0; t <= 1; t += tamanhoPartes)
        {
          Ponto4D P1P2 = CalculaPonto(ptosControle[0], ptosControle[1], t);
          Ponto4D P2P3 = CalculaPonto(ptosControle[1], ptosControle[2], t);
          Ponto4D P3P4 = CalculaPonto(ptosControle[2], ptosControle[3], t);

          Ponto4D P1P2P3 = CalculaPonto(P1P2, P2P3, t);
          Ponto4D P2P3P4 = CalculaPonto(P2P3, P3P4, t);

          Ponto4D ptoFinal = CalculaPonto(P1P2P3, P2P3P4, t);

          GL.Vertex2(ptoInicial.X, ptoInicial.Y);
          GL.Vertex2(ptoFinal.X, ptoFinal.Y);

          ptoInicial = ptoFinal;
        }
      GL.End();
    }

    protected override void DesenharObjeto()
    {
      DesenhaPontosControle();
      DesenhaSpline();
    }
  }
}