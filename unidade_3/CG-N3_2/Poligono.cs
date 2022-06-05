using CG_Biblioteca;
using OpenTK.Graphics.OpenGL;

namespace gcgcg
{
    internal class Poligono : ObjetoGeometria
    {
        private bool emPrevisao;
        private bool emPrevisaoMovendo;

        public int QuantidadePontos => pontosLista.Count;
        public Ponto4D[] Pontos => pontosLista.ToArray();

        public Poligono(char rotulo, Objeto paiRef, Ponto4D pontoInicial) : base(rotulo, paiRef)
        {
            PontosAdicionar(pontoInicial);
            PontosAdicionar(pontoInicial);
            
            ObjetoCor.CorR = 255;
            ObjetoCor.CorG = 255;
            ObjetoCor.CorB = 255;
            PrimitivaTamanho = 3;
            PrimitivaTipo = PrimitiveType.LineLoop;
        }

        public void PreverPonto(Ponto4D ponto)
        {
            PontosRemoverUltimo();
            PontosAdicionar(ponto);
            emPrevisao = true;
        }
        
        public void PreverPonto(Ponto4D ponto, int indice)
        {
            PontosAlterar(ponto, indice);
            emPrevisaoMovendo = true;
        }

        public void FinalizarPrevisao()
        {
            if (emPrevisao)
                PontosRemoverUltimo();
        }
        
        public void FinalizarPrevisao(int indice)
        {
            if (emPrevisaoMovendo)
            {
                pontosLista.RemoveAt(indice);
                emPrevisaoMovendo = false;    
            }
        }
        
        public void AdicionarPonto(Ponto4D ponto)
        {
            PontosAdicionar(ponto);
            emPrevisao = false;
        }

        public void AlterarPonto(Ponto4D ponto, int indice)
        {
            PontosAlterar(ponto, indice);
        }
        
        public int RemoverPonto(Ponto4D ponto)
        {
            var indice = pontosLista.IndexOf(ponto);
            pontosLista.RemoveAt(indice);

            return indice;
        }
        
        protected override void DesenharObjeto()
        {
            GL.Begin(PrimitivaTipo);
            foreach (Ponto4D pto in pontosLista)
            {
                GL.Vertex2(pto.X, pto.Y);
            }
            GL.End();
        }
    
        //TODO: melhorar para exibir não só a lista de pontos (geometria), mas também a topologia ... poderia ser listado estilo OBJ da Wavefrom
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Retangulo: " + rotulo + "\n";
            for (var i = 0; i < pontosLista.Count; i++)
            {
                retorno += "P" + i + "[" + pontosLista[i].X + "," + pontosLista[i].Y + "," + pontosLista[i].Z + "," + pontosLista[i].W + "]" + "\n";
            }
            return (retorno);
        }
    }    
}