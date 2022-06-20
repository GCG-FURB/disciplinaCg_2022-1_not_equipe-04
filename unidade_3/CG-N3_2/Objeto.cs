/**
  Autor: Dalton Solano dos Reis
**/

using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using CG_Biblioteca;

namespace gcgcg
{
  internal abstract class Objeto
  {
    protected char rotulo;
    private Cor objetoCor = new Cor(255, 255, 255, 255);
    public Cor ObjetoCor { get => objetoCor; set => objetoCor = value; }
    private PrimitiveType primitivaTipo = PrimitiveType.LineLoop;
    public PrimitiveType PrimitivaTipo { get => primitivaTipo; set => primitivaTipo = value; }
    private float primitivaTamanho = 1;
    public float PrimitivaTamanho { get => primitivaTamanho; set => primitivaTamanho = value; }
    private BBox bBox = new BBox();
    public BBox BBox { get => bBox; set => bBox = value; }
    private List<Objeto> objetosLista = new List<Objeto>();
    public List<Objeto> ObjetosLista { get => objetosLista; }
    private Transformacao4D matriz = new Transformacao4D();
    private static Transformacao4D matrizRotacao = new Transformacao4D();
    private static Transformacao4D matrizTranslacao = new Transformacao4D();
    private static Transformacao4D matrizTranslacaoInversa = new Transformacao4D();
    private static Transformacao4D matrizEscala = new Transformacao4D();
    private static Transformacao4D matrizGlobal = new Transformacao4D();

    public Objeto(char rotulo, Objeto paiRef)
    {
      this.rotulo = rotulo;
    }

    public void Desenhar()
    {
      GL.PushMatrix();
      GL.MultMatrix(matriz.ObterDados());
      GL.Color3(objetoCor.CorR, objetoCor.CorG, objetoCor.CorB);
      GL.LineWidth(primitivaTamanho);
      GL.PointSize(primitivaTamanho);
      DesenharGeometria();
      for (var i = 0; i < objetosLista.Count; i++)
      {
        objetosLista[i].Desenhar();
      }
      GL.PopMatrix();
    }
    protected abstract void DesenharGeometria();
    public void FilhoAdicionar(Objeto filho)
    {
      this.objetosLista.Add(filho);
    }
    public void FilhoRemover(Objeto filho)
    {
      this.objetosLista.Remove(filho);
    }

    public void Translacao(double x, double y, double z)
    {
      Transformacao4D matrizLocal = new Transformacao4D();
      matrizLocal.AtribuirTranslacao(x, y, z);
      matriz = matrizLocal.MultiplicarMatriz(matriz);
      matrizLocal.AtribuirIdentidade();
    }

    public void RotacaoGlobal(double angulo, char eixo, bool rotacionar = true)
    {
      switch(eixo)
      {
        case 'x':
          matrizRotacao.AtribuirRotacaoX(Transformacao4D.DEG_TO_RAD * angulo);
          break;

        case 'y':
          matrizRotacao.AtribuirRotacaoY(Transformacao4D.DEG_TO_RAD * angulo);
          break;

        case 'z':
          matrizRotacao.AtribuirRotacaoZ(Transformacao4D.DEG_TO_RAD * angulo);
          break;
      }

      if (rotacionar)
        matriz = matriz.MultiplicarMatriz(matrizRotacao);
    }

    public void RotacaoLocal(double angulo)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D centroBBox = bBox.obterCentro;

      matrizTranslacao.AtribuirTranslacao(-centroBBox.X, -centroBBox.Y, -centroBBox.Z);
      matrizGlobal = matrizTranslacao.MultiplicarMatriz(matrizGlobal);

      RotacaoGlobal(angulo, 'z', false);
      matrizGlobal = matrizRotacao.MultiplicarMatriz(matrizGlobal);

      matrizTranslacaoInversa.AtribuirTranslacao(centroBBox.X, centroBBox.Y, centroBBox.Z);
      matrizGlobal = matrizTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);
    }

    public void EscalaGlobal(double x, double y, double z)
    {
      Transformacao4D matrizLocal = new Transformacao4D();
      matrizLocal.AtribuirEscala(x, y, z);
      matriz = matrizLocal.MultiplicarMatriz(matriz);
    }

    public void EscalaLocal(double x, double y, double z)
    {
      matrizGlobal.AtribuirIdentidade();
      Ponto4D centroBBox = bBox.obterCentro;

      matrizTranslacao.AtribuirTranslacao(-centroBBox.X, -centroBBox.Y, -centroBBox.Z);
      matrizGlobal = matrizTranslacao.MultiplicarMatriz(matrizGlobal);

      matrizEscala.AtribuirEscala(x, y, z);
      matrizGlobal = matrizEscala.MultiplicarMatriz(matrizGlobal);

      matrizTranslacao.AtribuirTranslacao(centroBBox.X, centroBBox.Y, centroBBox.Z);
      matrizGlobal = matrizTranslacaoInversa.MultiplicarMatriz(matrizGlobal);

      matriz = matriz.MultiplicarMatriz(matrizGlobal);
    }
  }
}