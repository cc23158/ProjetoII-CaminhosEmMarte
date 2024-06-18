// https://learn.microsoft.com/en-us/dotnet/api/system.io.file?view=netframework-4.8 -> Linha 164

using apCaminhosEmMarte;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCidadesBacktracking
{
    class GrafoBacktracking
    {
        const int tamanhoDistancia = 4;
        char tipoGrafo;
        int qtasCidades;
        int[,] matriz;

        public GrafoBacktracking(string nomeArquivo, Cidade[] asCidades)
        {
            qtasCidades = 0;
            foreach (Cidade cidade in asCidades)
                if (cidade != null)
                    qtasCidades++;


            var arquivo = new StreamReader(nomeArquivo);
            tipoGrafo = arquivo.ReadLine()[0]; // acessa primeiro caracter com tipo do grafo
            matriz = new int[qtasCidades, qtasCidades];

            string[,] matrizOrdenada = LerDados(nomeArquivo);

            // inserimos as distâncias em suas respectivas posições
            for(int index = 0; index < matrizOrdenada.Length / 3; index++)
            {
                string cidadeDeOrigem = matrizOrdenada[index, 0];
                string cidadeDeDestino = matrizOrdenada[index, 1];
                string distancia = matrizOrdenada[index, 2];

                int indexOrigem = ProcurarIndex(asCidades, cidadeDeOrigem);
                int indexDestino = ProcurarIndex(asCidades, cidadeDeDestino);

                matriz[indexOrigem, indexDestino] = Convert.ToInt32(distancia);
            }
        }

        public char TipoGrafo { get => tipoGrafo; set => tipoGrafo = value; }
        public int QtasCidades { get => qtasCidades; set => qtasCidades = value; }
        public int[,] Matriz { get => matriz; set => matriz = value; }

        // exibe apenas os dados dos arquivos .txt
        public void Exibir(DataGridView dgv)
        {
            dgv.Rows.Clear();
            dgv.Refresh();
            dgv.RowCount = dgv.ColumnCount = qtasCidades;

            for (int coluna = 0; coluna < qtasCidades; coluna++)
            {
                dgv.Columns[coluna].HeaderText = coluna.ToString();
                dgv.Rows[coluna].HeaderCell.Value = coluna.ToString();
                dgv.Columns[coluna].Width = 50;
            }

            for (int linha = 0; linha < qtasCidades; linha++)
                for (int coluna = 0; coluna < qtasCidades; coluna++)
                    if (matriz[linha, coluna] != 0)
                        dgv[coluna, linha].Value = matriz[linha, coluna];
        }

        public List<PilhaVetor<Movimento>> BuscarTodosOsCaminhos(int origem, int destino)
        {
            // variáveis que armazenarão os caminhos
            List<PilhaVetor<Movimento>> listaDeCaminhos = new List<PilhaVetor<Movimento>>();
            PilhaVetor<Movimento> pilha = new PilhaVetor<Movimento>();

            bool[] passou = new bool[qtasCidades];
            int cidadeAtual, saidaAtual;

            // inicia os valores de "passou" pois ainda não foi em nenhuma cidade
            for (int indice = 0; indice < qtasCidades; indice++)
                passou[indice] = false;

            cidadeAtual = origem;
            saidaAtual = 0;

            while (true)
            {
                while (saidaAtual < qtasCidades)
                {


                    // se chegamos ao destino
                    if (saidaAtual == destino && matriz[cidadeAtual, saidaAtual] != 0)
                    {
                        // criamos o movimento
                        // empilhamos o movimento
                        // inserimos o caminho na lista
                        Movimento movim = new Movimento(cidadeAtual, saidaAtual);
                        pilha.Empilhar(movim);

                        // adiciona uma cópia da pilha atual à lista de caminhos para que sejam retornadas as rotas corretamente
                        PilhaVetor<Movimento> caminhoEncontrado = ClonarPilha(pilha);
                        listaDeCaminhos.Add(caminhoEncontrado);                        

                        break;
                    }

                    // se não tem nada
                    else if (matriz[cidadeAtual, saidaAtual] == 0)
                        saidaAtual++;

                    // se já passou na próxima cidade
                    else if (passou[saidaAtual])
                        saidaAtual++;

                    else
                    {
                        Movimento movim = new Movimento(cidadeAtual, saidaAtual);
                        pilha.Empilhar(movim);
                        passou[cidadeAtual] = true;
                        cidadeAtual = saidaAtual; // muda para a nova cidade
                        saidaAtual = 0; // reinicia busca de saídas da nova
                                        // cidade a partir da primeira cidade
                    }
                }

                if (pilha.Tamanho > 0)
                {
                    // volta uma cidadeAtual para verificar outros caminhos
                    Movimento movimento = pilha.Desempilhar();                    
                    cidadeAtual = movimento.Origem;
                    saidaAtual = movimento.Destino + 1;
                    passou[cidadeAtual] = false; // marca a cidade atual como não visitada
                }
            }

            return listaDeCaminhos;
        }

        public PilhaVetor<Movimento> ClonarPilha(PilhaVetor<Movimento> pilha)
        {
            PilhaVetor<Movimento> novaPilha = new PilhaVetor<Movimento>(pilha.Tamanho);
            List<Movimento> elementos = pilha.Conteudo();

            // cria uma nova pilha e empilha os elementos na mesma ordem
            foreach (Movimento mov in elementos)
                novaPilha.Empilhar(mov);

            return novaPilha;
        }

        public string[,] LerDados(string arquivoDeLeitura)
        {
            // pegamos todas as linhas do arquivo de leitura
            // fatiamos a cidade de origem do conteúdo da linha
            // preenchemos a matriz bidimensional para saber
            // como os conteúdos das linhas devem ser retornados
            string[] matrizDados  =  File.ReadAllLines(arquivoDeLeitura);
            string[,] matrizOrigemConteudo = new string[matrizDados.Length, 3];

            for (int linha = 0; linha < matrizDados.Length; linha++)
            {
                string cidadeDeOrigem = matrizDados[linha].Substring(0, 15);
                string cidadeDeDestino  = matrizDados[linha].Substring(15, 15);
                string distancia = matrizDados[linha].Substring(matrizDados[linha].Length - tamanhoDistancia, tamanhoDistancia);

                // guardamos a origem na primeira posição da matriz bidimensional
                // e o conteúdo da linha na segunda posição
                matrizOrigemConteudo[linha, 0] = cidadeDeOrigem;
                matrizOrigemConteudo[linha, 1] = cidadeDeDestino;
                matrizOrigemConteudo[linha, 2] = distancia;
            }

            OrdenarDados(matrizOrigemConteudo);

            return matrizOrigemConteudo;
        }

        public void OrdenarDados(string[,] matrizDados)
        {
            // ordenação alfabética de acordo com o nome
            // da cidade de origem do caminho
            for (int index = 1; index < matrizDados.Length / 3; index++)
            {
                string cidadeDeOrigem = matrizDados[index, 0];
                string cidadeDeDestino = matrizDados[index, 1];
                string conteudo = matrizDados[index, 2];

                int esquerda = 0;
                int direita = index;

                // Busca binária para encontrar a posição correta do elemento
                while (esquerda < direita)
                {
                    int mid = (esquerda + direita) / 2;
                    if (string.Compare(cidadeDeOrigem, matrizDados[mid, 0]) >= 0)
                        esquerda = mid + 1;
                    
                    else
                        direita = mid;
                }

                // Move todos os elementos para abrir espaço para o elemento chave
                for (int j = index; j > esquerda; j--)
                {
                    matrizDados[j, 0] = matrizDados[j - 1, 0];
                    matrizDados[j, 1] = matrizDados[j - 1, 1];
                    matrizDados[j, 2] = matrizDados[j - 1, 2];
                }

                // Insere o elemento chave na posição encontrada
                matrizDados[esquerda, 0] = cidadeDeOrigem;
                matrizDados[esquerda, 1] = cidadeDeDestino;
                matrizDados[esquerda, 2] = conteudo;
            }
        }

        public int ProcurarIndex(Cidade[] asCidades, string cidadeProcurada)
        {
            for (int index = 0; index < asCidades.Length; index++)
                if (asCidades[index] != null && asCidades[index].NomeCidade == cidadeProcurada)
                    return index;

            return -1;
        }

        public void CriarLigacao(int linha, int coluna, decimal distancia)
        {
            // não se pode criar ligações cuja cidade de origem é a cidade de destino
            if (linha == coluna)
                throw new Exception("Não foi possível criar o registro");

            // se tiver um registro nessa posição da matriz
            // não será criado a ligação
            if (matriz[linha, coluna] != 0)
                throw new Exception("Não foi possível criar o registro");

            matriz[linha, coluna] = Convert.ToInt32(distancia);
        }

        public void ExcluirLigacao(int linha, int coluna)
        {            
            // não se pode excluir ligações cuja cidade de origem é a cidade de destino
            if (linha == coluna)
                throw new Exception("Não foi possível excluir o registro");

            // se não tiver um registro nessa posição da matriz
            // não será alterado nenhum dado
            if (matriz[linha, coluna] == 0)
                throw new Exception("Não é possível excluir registros inexistentes");

            matriz[linha, coluna] = 0;
        }

        public void AlterarLigacao(int linha, int coluna, decimal distancia)
        {
            // não se pode atualizar ligações cuja cidade de origem é a cidade de destino
            if (linha == coluna)
                throw new Exception("Não foi possível atualizar o registro");

            // se não tiver um registro nessa posição da matriz
            // não será alterado nenhum dado
            if (matriz[linha, coluna] == 0)
                throw new Exception("Não é possível alterar registros inexistentes");

            matriz[linha, coluna] = Convert.ToInt32(distancia);
        }
    }
}
