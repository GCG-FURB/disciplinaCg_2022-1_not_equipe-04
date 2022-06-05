/**
  Autor: Dalton Solano dos Reis
**/

//#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Linq;
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

    private bool criandoNovo;
    private bool movendo;
    private int indiceMovendo = -1;
    
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

      var pt0 = new Ponto4D(-332, 392);
      var pt1 = new Ponto4D(172, 398);
      var pt2 = new Ponto4D(298, -196);
      var pt3 = new Ponto4D(-510, -204);
      
      objetoId = Utilitario.charProximo(objetoId);
      var poli = new Poligono(objetoId, null, pt0);
      poli.AdicionarPonto(pt1);
      poli.AdicionarPonto(pt2);
      poli.AdicionarPonto(pt3);

      objetoSelecionado = poli;
      objetosLista.Add(poli);
      
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
          NovoPonto();
          break;
        
        case Key.Enter:
          SalvarPoligono();
          break;
        
        case Key.V:
          MoverPontoMaisProximo();
          break;
        
        case Key.D:
          RemoverPontoMaisProximo();
          break;
      }
      
      base.OnKeyDown(e);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      coordenada = ObterPonto(e.X, e.Y);
      
      if (objetoSelecionado != null && (criandoNovo || movendo))
      {
        PreverPoligono();
      }

      base.OnMouseMove(e);
    }
    
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
      if (e.IsPressed)
      {
      }

      base.OnMouseDown(e);
    }
    
    private Ponto4D ObterPonto(int x, int y) => new ((x - 300) * 2, (y - 300) * -2);

    private void NovoPonto()
    {
      if (objetoSelecionado == null)
      {
        NovoPoligono();
      }
      else
      {
        AtualizarPoligono();
      }
    }
    
    private void NovoPoligono()
    {
      var poligono = new Poligono(Utilitario.charProximo(objetoId), null, coordenada);
      objetoSelecionado = poligono;
      objetosLista.Add(poligono);

      criandoNovo = true;
    }

    private void PreverPoligono()
    {
      var poligono = (Poligono)objetoSelecionado;

      if (movendo)
      {
        poligono.PreverPonto(coordenada, indiceMovendo);
      }
      else
      {
        poligono.PreverPonto(coordenada);  
      }
    }
    
    private void AtualizarPoligono()
    {
      var poligono = (Poligono)objetoSelecionado;

      if (movendo)
      {
        poligono.AlterarPonto(coordenada, indiceMovendo);
      }
      else
      {
        poligono.AdicionarPonto(coordenada);
      }
    }
    
    private void SalvarPoligono()
    {
      if (objetoSelecionado == null)
        return;
      
      var poligono = (Poligono)objetoSelecionado;

      if (movendo)
      {
        poligono.FinalizarPrevisao(indiceMovendo);  
      }
      else
      {
        poligono.FinalizarPrevisao();  
      }

      RemoverPoligonoInvalido(poligono);

      objetoSelecionado = null;
      criandoNovo = false;
      movendo = false;
      indiceMovendo = -1;
    }
    
    private void MoverPontoMaisProximo()
    {
      if (objetoSelecionado == null)
        return;
      
      var maisProximo = ObterPontoMaisProximo();
      
      var poligono = (Poligono)objetoSelecionado;
      indiceMovendo = poligono.RemoverPonto(maisProximo);

      movendo = true;
    }
    
    private void RemoverPontoMaisProximo()
    {
      if (objetoSelecionado == null)
        return;
      
      var maisProximo = ObterPontoMaisProximo();

      var poligono = (Poligono)objetoSelecionado;
      poligono.RemoverPonto(maisProximo);
      
      RemoverPoligonoInvalido(poligono);
    }

    private Ponto4D ObterPontoMaisProximo()
    {
      if (objetoSelecionado == null)
        return null;

      var poligono = (Poligono)objetoSelecionado;
      var pontos = poligono.Pontos;
      
      return pontos.MinBy(p => Math.Pow(p.X - coordenada.X, 2 + Math.Pow(p.Y - coordenada.Y, 2)));
    }

    private void RemoverPoligonoInvalido(Poligono poligono)
    {
      if (poligono.QuantidadePontos < 2)
      {
        objetosLista.Remove(poligono);
        objetoSelecionado = null;
      }
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
