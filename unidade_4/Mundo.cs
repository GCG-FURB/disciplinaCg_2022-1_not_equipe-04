/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Privado // código do professor.
#define CG_Gizmo  // debugar gráfico.
#define CG_Debug // debugar texto.
#define CG_OpenGL // render OpenGL.
//#define CG_DirectX // render DirectX.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;
using System.Drawing;

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
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif
    private bool jogoIniciado = false;
    private Player player1 = null;
    private Player player2 = null;
    private Bola bola = null;
    private bool movendoCima1 = false;
    private bool movendoCima2 = false;
    private bool movendoBaixo1 = false;
    private bool movendoBaixo2 = false;

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      camera.xmin = 0; camera.xmax = 600; camera.ymin = 0; camera.ymax = 600;

      GL.Enable(EnableCap.Texture2D);

      objetoId = Utilitario.charProximo(objetoId);

      player1 = new Player(objetoId, null, new Ponto4D(15, 300));
      objetosLista.Add(player1);

      player2 = new Player(objetoId, null, new Ponto4D(585, 300));
      objetosLista.Add(player2);

      bola = new Bola(objetoId, null);
      objetosLista.Add(bola);

      //System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(10, 10, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      //Graphics g = Graphics.FromImage(bmp);
      //g.DrawString("TEXTO", new Font(FontFamily.GenericSerif, 20), Brushes.Black, new System.Drawing.PointF(100, 100));
      //g.Dispose();


#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      obj_Circulo.PrimitivaTipo = PrimitiveType.Points;
      obj_Circulo.PrimitivaTamanho = 5;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
#if CG_OpenGL
      GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
#endif
    }
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);
#if CG_OpenGL
      GL.MatrixMode(MatrixMode.Projection);
      GL.LoadIdentity();
      GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
#endif

      if (player1.pontuacao == 3 || player2.pontuacao == 3)
        IniciarJogo();

      if (!jogoIniciado)
        return;

      if (movendoCima1)
        player1.MoverCima();

      if (movendoCima2)
        player2.MoverCima();

      if (movendoBaixo1)
        player1.MoverBaixo();

      if (movendoBaixo2)
        player2.MoverBaixo();

      bola.Mover();

      if (bola.direcaoX > 0 && bola.BBox.obterMaiorX + 5 >= player2.BBox.obterMenorX && bola.BBox.obterMaiorY >= player2.BBox.obterMenorY && bola.BBox.obterMenorY <= player2.BBox.obterMaiorY)
        bola.direcaoX = -10;
      else if (bola.direcaoX < 0 && bola.BBox.obterMenorX - 5 <= player1.BBox.obterMaiorX && bola.BBox.obterMaiorY >= player1.BBox.obterMenorY && bola.BBox.obterMenorY <= player1.BBox.obterMaiorY)
        bola.direcaoX = 10;

      if (bola.direcaoX > 0 && bola.BBox.obterMaiorX > player2.BBox.obterMaiorX)
      {
        player1.MarcaPonto();
        IniciarJogo(false);
      }
      else if (bola.direcaoX < 0 && bola.BBox.obterMenorX < player1.BBox.obterMenorX)
      {
        player2.MarcaPonto();
        IniciarJogo(false);
      }
    }
    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);
#if CG_OpenGL
      GL.Clear(ClearBufferMask.ColorBufferBit);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.LoadIdentity();
#endif

      DesenhaSeparador();

      for (var i = 0; i < objetosLista.Count; i++)
        objetosLista[i].Desenhar();

      if (bBoxDesenhar && (objetoSelecionado != null))
        objetoSelecionado.BBox.Desenhar();
      this.SwapBuffers();
    }

    protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (!jogoIniciado && e.Key == Key.Unknown)
        return;

      jogoIniciado = true;

      if (e.Key == Key.W)
        movendoCima1 = true;

      if (e.Key == Key.S)
        movendoBaixo1 = true;

      if (e.Key == Key.Up)
        movendoCima2 = true;

      if (e.Key == Key.Down)
        movendoBaixo2 = true;

      //if (e.Key == Key.O)
      //  bBoxDesenhar = !bBoxDesenhar;

      //if (e.Key == Key.Space)
      //  TrocarCamera();
    }

    protected override void OnKeyUp(OpenTK.Input.KeyboardKeyEventArgs e)
    {
      if (e.Key == Key.W)
        movendoCima1 = false;

      if (e.Key == Key.S)
        movendoBaixo1 = false;

      if (e.Key == Key.Up)
        movendoCima2 = false;

      if (e.Key == Key.Down)
        movendoBaixo2 = false;
    }

    private void DesenhaSeparador()
    {
      Ponto4D pontoA = new Ponto4D(300, 10);
      Ponto4D pontoB = new Ponto4D(300, 30);

      GL.Begin(PrimitiveType.Lines);
        for (int i = 0; i < 15; i++)
        {
          GL.Color3(Convert.ToByte(255), Convert.ToByte(255), Convert.ToByte(255));
          GL.Vertex2(pontoA.X, pontoA.Y);
          GL.Vertex2(pontoB.X, pontoB.Y);

          pontoA.Y += 40;
          pontoB.Y += 40;
        }
      GL.End();
    }

    //TODO: não está considerando o NDC
    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
    }

    private void IniciarJogo(bool resetarTudo = true)
    {
      jogoIniciado = false;

      movendoBaixo1 = false;
      movendoBaixo2 = false;
      movendoCima1 = false;
      movendoCima2 = false;

      bola.Reseta(resetarTudo);
      player1.Reseta(resetarTudo);
      player2.Reseta(resetarTudo);

      if (resetarTudo)
      {
        player1.pontuacao = 0;
        player2.pontuacao = 0;
      }
    }
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