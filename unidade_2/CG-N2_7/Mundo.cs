/**
  Autor: Dalton Solano dos Reis
**/

#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;

namespace gcgcg
{
  class Mundo : GameWindow
  {
    private static Mundo instanciaMundo = null;

    private Mundo(int width, int height) : base(width, height) { }

    public static Mundo GetInstance(int width, int height)
    {
      if (instanciaMundo == null)
        instanciaMundo = new Mundo(width, height);
      return instanciaMundo;
    }

    private CameraOrtho camera = new CameraOrtho();
    protected List<Objeto> objetosLista = new List<Objeto>();
    private ObjetoGeometria objetoSelecionado = null;
    private char objetoId = '@';
    private bool bBoxDesenhar = false;
    int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
    private bool mouseMoverPto = false;
    private Retangulo obj_Retangulo;
    public static Ponto4D pontoCentral = new Ponto4D(0, 0);
    public static Cor corRetangulo = new Cor(178, 0, 255);
    public static bool pontoSelecionado = false;

#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = -600; camera.xmax = 600; camera.ymin = -600; camera.ymax = 600;

      Console.WriteLine(" --- Ajuda / Teclas: ");
      Console.WriteLine(" [  H     ] mostra teclas usadas. ");

      objetoId = Utilitario.charProximo(objetoId);

      DesenharBbox();

#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
      GL.Clear(ClearBufferMask.ColorBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
#if CG_Gizmo      
      Sru3D();
#endif
      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();
      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
    }

    private void DesenharBbox()
    {
      Retangulo retangulo = new Retangulo(Utilitario.charProximo(objetoId), null, Matematica.GerarPtosCirculo(225, 300), Matematica.GerarPtosCirculo(45, 300));
      retangulo.ObjetoCor = corRetangulo;
      objetosLista.Add(retangulo);

      Ponto pontoCentral = new Ponto(Utilitario.charProximo(objetoId), null, Mundo.pontoCentral);
      objetosLista.Add(pontoCentral);

      Circulo circuloMaior = new Circulo(Utilitario.charProximo(objetoId), null, 300, 720, new Ponto4D(0, 0));
      objetosLista.Add(circuloMaior);

      Circulo circuloMenor = new Circulo(Utilitario.charProximo(objetoId), null, 100, 72, Mundo.pontoCentral);
      objetosLista.Add(circuloMenor);
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      if (!e.Mouse.IsButtonDown(MouseButton.Left) || !pontoSelecionado)
      {
        pontoCentral.X = 0;
        pontoCentral.Y = 0;

        return;
      }

      int xPos = (e.X - 300) * 2;
      int yPos = (e.Y - 300) * -2;

      if (Matematica.GerarPtosCirculo(315, 300).X > xPos && Matematica.GerarPtosCirculo(315, 300).Y < yPos && Matematica.GerarPtosCirculo(135, 300).X < xPos && Matematica.GerarPtosCirculo(135, 300).Y > yPos)
      {
        pontoCentral.X = xPos;
        pontoCentral.Y = yPos;

        corRetangulo.CorR = 178;
        corRetangulo.CorG = 0;
        corRetangulo.CorB = 255;
      }
      else
      {
        corRetangulo.CorR = 255;
        corRetangulo.CorG = 255;
        corRetangulo.CorB = 0;

        var distancia = (xPos * xPos) + (yPos * yPos);
        var raioQuadrado = 300 * 300;

        if (distancia < raioQuadrado)
        {
          pontoCentral.X = xPos;
          pontoCentral.Y = yPos;
        }
      }
    }

    protected override void OnMouseUp(MouseButtonEventArgs e)
    {
      pontoSelecionado = false;
      base.OnMouseUp(e);
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.IsPressed)
      {
        int xPos = (e.X - 300) * 2;
        int yPos = (e.Y - 300) * -2;

        pontoSelecionado = false;

        if (xPos > (pontoCentral.X - 50) && xPos < (pontoCentral.X + 50) && yPos < (pontoCentral.Y + 50) && yPos > (pontoCentral.Y - 50))
        {
          pontoSelecionado = true;
        }
      }

      base.OnMouseDown(e);
    }

#if CG_Gizmo
    private void Sru3D()
    {
      GL.LineWidth(5);
      GL.Begin(PrimitiveType.Lines);
      // GL.Color3(1.0f,0.0f,0.0f);
      GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
      // GL.Color3(0.0f,1.0f,0.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
      // GL.Color3(0.0f,0.0f,1.0f);
      GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
      GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
      GL.End();
    }
#endif    
  }
  class Program
  {
    static void Main(string[] args)
    {
      Mundo window = Mundo.GetInstance(600, 600);
      window.Title = "CG_N2";
      window.Run(1.0 / 60.0);
    }
  }
}
