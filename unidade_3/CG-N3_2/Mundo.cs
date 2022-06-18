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
    private ObjetoGeometria objetoDesenhando = null;
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
      camera.xmin = 0; camera.xmax = 600; camera.ymin = 0; camera.ymax = 600;

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

        case Key.R:
          MudarCor('r');
          break;

        case Key.G:
          MudarCor('g');
          break;

        case Key.B:
          MudarCor('b');
          break;

        case Key.C:
          RemoverPoligono();
          break;

        case Key.S:
          objetoSelecionado.PrimitivaTipo = objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop ? PrimitiveType.LineStrip : PrimitiveType.LineLoop;
          break;

        case Key.O:
            bBoxDesenhar = !bBoxDesenhar;
            break;

        case Key.Up:
          objetoSelecionado?.Translacao(0, 10, 0);
          break;

        case Key.Down:
          objetoSelecionado?.Translacao(0, -10, 0);
          break;

        case Key.Right:
          objetoSelecionado?.Translacao(10, 0, 0);
          break;

        case Key.Left:
          objetoSelecionado?.Translacao(-10, 0, 0);
          break;

        case Key.Number1:
          objetoSelecionado?.RotacaoGlobal(10, 'z');
          break;

        case Key.Number2:
          objetoSelecionado?.RotacaoGlobal(-10, 'z');
          break;

        case Key.Number3:
          objetoSelecionado?.RotacaoLocal(10);
          break;

        case Key.Number4:
          objetoSelecionado?.RotacaoLocal(-10);
          break;

        case Key.PageUp:
          objetoSelecionado?.EscalaGlobal(2, 2, 2);
          break;

        case Key.PageDown:
          objetoSelecionado?.EscalaGlobal(0.5, 0.5, 0.5);
          break;

        case Key.Home:
          objetoSelecionado?.EscalaLocal(2, 2, 2);
          break;

        case Key.End:
          objetoSelecionado?.EscalaLocal(0.5, 0.5, 0.5);
          break;

        case Key.A:
          SelecionaPoligono();
          break;
      }
      
      base.OnKeyDown(e);
    }

    protected override void OnMouseMove(MouseMoveEventArgs e)
    {
      coordenada = new Ponto4D(e.X, 600 - e.Y);
      
      if (objetoDesenhando != null || objetoSelecionado != null && (criandoNovo || movendo))
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
    
    private void NovoPonto()
    {
      if (!criandoNovo)
      {
        indiceMovendo = -1;
        movendo = false;

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
      objetoDesenhando = poligono;
      objetosLista.Add(poligono);

      criandoNovo = true;
    }

    private void PreverPoligono()
    {
      if (movendo)
      {
        var poligono = (Poligono)objetoSelecionado;
        poligono.PreverPonto(coordenada, indiceMovendo);
      }
      else
      {
        var poligono = (Poligono)objetoDesenhando;
        poligono.PreverPonto(coordenada);
      }
    }
    
    private void AtualizarPoligono()
    {
      if (movendo)
      {
        var poligono = (Poligono)objetoSelecionado;
        poligono.AlterarPonto(coordenada, indiceMovendo);
      }
      else
      {
        var poligono = (Poligono)objetoDesenhando;
        poligono.AdicionarPonto(coordenada);
      }
    }
    
    private void SalvarPoligono()
    {
      if (objetoDesenhando == null && objetoSelecionado == null)
        return;
      
      if (!movendo)
      {
        var poligono = (Poligono)objetoDesenhando;
        poligono.FinalizarPrevisao();

        if (!RemoverPoligonoInvalido(poligono))
        {
          if (objetoSelecionado != null)
          {
            objetoSelecionado.FilhoAdicionar(objetoDesenhando);
            objetosLista.Remove(objetoDesenhando);
          }

          objetoSelecionado = objetoDesenhando;
          objetoDesenhando = null;
        }
      }

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
      indiceMovendo = poligono.IndicePonto(maisProximo);

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
      
      return pontos.MinBy(p => Math.Pow(p.X - coordenada.X, 2) + Math.Pow(p.Y - coordenada.Y, 2));
    }

    private bool RemoverPoligonoInvalido(Poligono poligono)
    {
      if (poligono.QuantidadePontos < 2)
      {
        objetosLista.Remove(poligono);
        objetoSelecionado = null;
        return true;
      }

      return false;
    }

    private void MudarCor(char faixa)
    {
      if (objetoSelecionado == null)
        return;

      objetoSelecionado.ObjetoCor.CorR = 0;
      objetoSelecionado.ObjetoCor.CorG = 0;
      objetoSelecionado.ObjetoCor.CorB = 0;

      switch(faixa)
      {
        case 'r':
          objetoSelecionado.ObjetoCor.CorR = 255;
          break;

        case 'g':
          objetoSelecionado.ObjetoCor.CorG = 255;
          break;

        case 'b':
          objetoSelecionado.ObjetoCor.CorB = 255;
          break;
      }
    }

    private void SelecionaPoligono()
    {
      foreach (var poligono in objetosLista)
      {
        var pol = (Poligono)poligono;
        objetoSelecionado = pol.ScanLine(coordenada);

        if (objetoSelecionado != null)
          break;
      }
    }

    private void RemoverPoligono()
    {
      if (!objetosLista.Remove(objetoSelecionado))
      {
        foreach (var poligono in objetosLista)
        {
          if (((Poligono)poligono).RemoveFilho((Poligono)objetoSelecionado))
            break;
        }
      }

      objetoSelecionado = null;
    }
    
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
