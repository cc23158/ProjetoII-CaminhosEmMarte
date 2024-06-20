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
        int qtasCidades;
        int[,] matriz;
        string[] nomeCidades;

        public GrafoBacktracking(string nomeArquivo, Cidade[] asCidades)
        {
            qtasCidades = 0;
            nomeCidades = new string[asCidades.Length];

            // preenchemos o vetor de cidades com seus nomes correspondentes
            foreach (Cidade cidade in asCidades)
                if (cidade != null)
                {
                    nomeCidades[qtasCidades] = cidade.NomeCidade;
                    qtasCidades++;
                }

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
                dgv.Columns[coluna].HeaderText = nomeCidades[coluna];
                dgv.Rows[coluna].HeaderCell.Value = coluna.ToString();
                dgv.Columns[coluna].Width = 80;
            }

            for (int linha = 0; linha < qtasCidades; linha++)
            {
                dgv.Rows[linha].HeaderCell.Value = nomeCidades[linha];
                dgv.RowHeadersWidth = 120;

                for (int coluna = 0; coluna < qtasCidades; coluna++)
                    if (matriz[linha, coluna] != 0)
                        dgv[coluna, linha].Value = matriz[linha, coluna];
            }
        }

        public List<PilhaVetor<Movimento>> BuscarTodosOsCaminhos(int origem, int destino)
        {
            // variáveis que armazenarão os caminhos
            List<PilhaVetor<Movimento>> listaDeCaminhos = new List<PilhaVetor<Movimento>>();
            PilhaVetor<Movimento> pilha = new PilhaVetor<Movimento>();

            // variáveis para a navegação pela matriz
            bool[] passou = new bool[qtasCidades];
            int cidadeAtual, saidaAtual;

            cidadeAtual = origem;
            saidaAtual = 0;

            // enquanto não lermos todas as possíveis rotas entre a linha e a coluna
            while (saidaAtual <= qtasCidades)
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
                        listaDeCaminhos.Add(ClonarPilha(pilha));
                        break;
                    }

                    // se não tem nada ou já passou na cidade
                    else if (matriz[cidadeAtual, saidaAtual] == 0 || passou[saidaAtual])
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

                // caso não haja mais possíveis caminhos a serem tentados, voltamos uma cidade
                if (pilha.Tamanho > 0)
                {
                    // volta uma cidadeAtual para verificar outros caminhos
                    Movimento movimento = pilha.Desempilhar();
                    cidadeAtual = movimento.Origem;
                    saidaAtual = movimento.Destino + 1;
                    passou[cidadeAtual] = false; // marca a cidade atual como não visitada
                }

                // se terminamos de ler todas as possibilidades possíveis, retornamos a lista de caminhos
                else if (saidaAtual == qtasCidades)
                    break;
            }

            return listaDeCaminhos;
        }

        public PilhaVetor<Movimento> ClonarPilha(PilhaVetor<Movimento> pilha)
        {
            // se a pilha não existe, não podemos cloná-la
            if (pilha.EstaVazia)
                throw new Exception("Não é possível clonar uma pilha sem elementos");

            PilhaVetor<Movimento> novaPilha = new PilhaVetor<Movimento>(pilha.Tamanho);
            List<Movimento> elementos = pilha.Conteudo();

            // cria uma nova pilha e empilha os elementos na mesma ordem
            foreach (Movimento mov in elementos)
                novaPilha.Empilhar(mov);

            return novaPilha;
        }

        public string[,] LerDados(string arquivoDeLeitura)
        {
            if (arquivoDeLeitura == "")
                throw new Exception("Não foi possível ler os dados do arquivo");

            
            // pegamos todas as linhas do arquivo de leitura
            // fatiamos a cidade de origem do conteúdo da linha
            // preenchemos a matriz bidimensional para saber
            // como os conteúdos das linhas devem ser retornados
            string[] matrizDados  =  File.ReadAllLines(arquivoDeLeitura);
            string[,] matrizOrigemConteudo = new string[matrizDados.Length, 3];

            for (int linha = 0; linha < matrizDados.Length; linha++)
            {
                // guardamos a origem na primeira posição da matriz
                // guardamos o destino da na segunda posição da matriz
                // guardamos o conteúdo da linha na terceira posição da matriz
                matrizOrigemConteudo[linha, 0] = matrizDados[linha].Substring(0, 15);
                matrizOrigemConteudo[linha, 1] = matrizDados[linha].Substring(15, 15);
                matrizOrigemConteudo[linha, 2] = matrizDados[linha].Substring(matrizDados[linha].Length - tamanhoDistancia, tamanhoDistancia);
            }

            OrdenarDados(matrizOrigemConteudo);

            return matrizOrigemConteudo;
        }

        public void OrdenarDados(string[,] matrizDados)
        {
            // ordenação alfabética de acordo com o nome
            // da cidade de origem do caminho
            // (busca binária)
            for (int index = 1; index < matrizDados.Length / 3; index++)
            {
                string cidadeDeOrigem = matrizDados[index, 0];
                string cidadeDeDestino = matrizDados[index, 1];
                string conteudo = matrizDados[index, 2];

                int esquerda = 0;
                int direita = index;

                while (esquerda < direita)
                {
                    int mid = (esquerda + direita) / 2;
                    if (string.Compare(cidadeDeOrigem, matrizDados[mid, 0]) >= 0)
                        esquerda = mid + 1;
                    
                    else
                        direita = mid;
                }

                for (int j = index; j > esquerda; j--)
                {
                    matrizDados[j, 0] = matrizDados[j - 1, 0];
                    matrizDados[j, 1] = matrizDados[j - 1, 1];
                    matrizDados[j, 2] = matrizDados[j - 1, 2];
                }

                matrizDados[esquerda, 0] = cidadeDeOrigem;
                matrizDados[esquerda, 1] = cidadeDeDestino;
                matrizDados[esquerda, 2] = conteudo;
            }
        }

        public int ProcurarIndex(Cidade[] asCidades, string cidadeProcurada)
        {
            if (cidadeProcurada == "")
                throw new Exception("Não é possível procurar uma cidade nula");

            // procura o índice da cidadeProcurada no array de cidades
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
/*
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

            // enquanto não lermos todas as possíveis rotas entre a linha e a coluna
            while (saidaAtual <= qtasCidades)
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

                // caso não haja mais possíveis caminhos a serem tentados, voltamos uma cidade
                if (pilha.Tamanho > 0)
                {
                    // volta uma cidadeAtual para verificar outros caminhos
                    Movimento movimento = pilha.Desempilhar();
                    cidadeAtual = movimento.Origem;
                    saidaAtual = movimento.Destino + 1;
                    passou[cidadeAtual] = false; // marca a cidade atual como não visitada
                }

                // se terminamos de ler todas as possibilidades possíveis, retornamos a lista de caminhos
                else if (saidaAtual == qtasCidades)
                    break;
            }

            return listaDeCaminhos; 
*/