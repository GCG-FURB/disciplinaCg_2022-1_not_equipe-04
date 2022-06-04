/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Gizmo
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

    private Mundo(int width, int height) : base(width, height)
    {
    }

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
    private Ponto4D coordenada = new ();
    private Cor corRetangulo = new Cor(178, 0, 255);
    
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
      switch (e.Key)
      {
        case Key.Space:
          if (objetoSelecionado == null)
          {
            NovoPoligono(coordenada);
          }
          else
          {
            AtualizarPoligono(coordenada);
          }
          break;
        
        case Key.Enter:
          SalvarPoligono();
          break;
      }
    }

    private void NovoPoligono(Ponto4D pontoInicial)
    {
      var poligono = new Poligono(Utilitario.charProximo(objetoId), null, pontoInicial);
      objetoSelecionado = poligono;
      objetosLista.Add(poligono);
    }

    private void PreverPoligono(Ponto4D novoPonto)
    {
      var poligono = (Poligono)objetoSelecionado;
      poligono.PreverPonto(novoPonto);
    }
    
    private void AtualizarPoligono(Ponto4D novoPonto)
    {
      var poligono = (Poligono)objetoSelecionado;
      poligono.AdicionarPonto(novoPonto);
    }

    private void SalvarPoligono()
    {
      if (objetoSelecionado == null)
        return;
      
      var poligono = (Poligono)objetoSelecionado;
      poligono.FinalizarPrevisao();

      if (poligono.QuantidadePontos < 2)
        objetosLista.Remove(poligono);
      
      objetoSelecionado = null;
    }

    private Ponto4D ObterPonto(int x, int y) => new ((x - 300) * 2, (y - 300) * -2);

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      coordenada = ObterPonto(e.X, e.Y);
      
      if (objetoSelecionado != null)
      {
        PreverPoligono(coordenada);
      }

      base.OnMouseMove(e);
    }
    
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.IsPressed)
      {
        if (objetoSelecionado == null)
        {
          NovoPoligono(coordenada);
        }
        else
        {
          AtualizarPoligono(coordenada);
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
      window.Title = "CG_N3";
      window.Run(1.0 / 60.0);
    }
  }
}
