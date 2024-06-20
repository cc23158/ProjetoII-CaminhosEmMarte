using apCidadesBacktracking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apCaminhosEmMarte
{
    public partial class FrmCaminhos : Form
    {
        public FrmCaminhos()
        {
            InitializeComponent();
        }

        ITabelaDeHash<Cidade> tabela;

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnLerArquivo_Click(object sender, EventArgs e)
        {
            if (dlgAbrir.ShowDialog() == DialogResult.OK)
            {
                if (rbBucketHash.Checked)
                    tabela = new BucketHash<Cidade>();
                else
                  if (rbHashLinear.Checked)
                    tabela = new HashLinear<Cidade>();
                else
                    if (rbHashQuadratico.Checked)
                    tabela = new HashQuadratico<Cidade>();
                else
                      if (rbHashDuplo.Checked)
                    tabela = new HashDuplo<Cidade>();

                var arquivo = new StreamReader(dlgAbrir.FileName);
                while (!arquivo.EndOfStream)
                {
                    Cidade umaCidade = new Cidade();
                    umaCidade.LerRegistro(arquivo);
                    tabela.Inserir(umaCidade);
                }
                lsbCidades.Items.Clear();  // limpa o listBox
                var asCidades = tabela.Conteudo();
                foreach (Cidade cid in asCidades)
                    lsbCidades.Items.Add(cid);
                arquivo.Close();
            }
        }

        private void FrmCaminhos_FormClosing(object sender, FormClosingEventArgs e)
        {
            // abrir o arquivo para saida, se houver um arquivo selecionado
            // obter todo o conteúdo da tabela de hash
            // percorrer o conteúdo da tabela de hash, acessando
            // cada cidade individualmente e usar esse objeto Cidade
            // para gravar seus próprios dados no arquivo
            // fechar o arquivo ao final do percurso
            if (dlgFecharCaminho.ShowDialog() == DialogResult.OK)
            {
                StreamWriter arquivo = new StreamWriter(dlgFecharCaminho.FileName);

                // percorremos todas as informações do grafo e gravamos no arquivo .txt
                int[,] matriz = grafo.Matriz;


                for (int linha = 0; linha < quantasCidades; linha++)
                    for (int coluna = 0; coluna < quantasCidades; coluna++)
                        if (matriz[linha, coluna] != 0)
                        {
                            string origem = asCidades[linha].NomeCidade;
                            string destino = asCidades[coluna].NomeCidade;
                            string distancia = matriz[linha, coluna].ToString();

                            arquivo.WriteLine($"{origem}{destino}{distancia}");
                        }

                arquivo.Close();
            }
        }

        private void FrmCaminhos_Load(object sender, EventArgs e)
        {

        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {

        }

        /////////////// Projeto Caminhos em Marte ///////////////
        
        Cidade[] asCidades;
        int quantasCidades;   // tamanho lógico
        GrafoBacktracking grafo;

        // criamos variável para saber a menor distância e consequentemente, menor caminho
        // criamos variável para saber todos os possíveis caminhos
        // criamos variável para armazenar o caminho a ser desenhado no mapa
        int menorDistancia = Int32.MaxValue;
        List<PilhaVetor<Movimento>> caminhos = null;
        List<(int origem, int destino)> linhaAerea = null;

        private void tpCaminhos_Enter(object sender, EventArgs e)
        {
            if (dlgAbrirCidades.ShowDialog() == DialogResult.OK && dlgAbrirCaminhos.ShowDialog() == DialogResult.OK)
            {
                asCidades = new Cidade[25];
                quantasCidades = 0;

                // abrir o arquivo de cidades
                // enquanto o arquivo de cidades não acabar
                //    instancie um objeto da classe cidade
                //    faça esse objeto ler um registro de cidade
                //    adicione esse registro de cidade após a última
                //    posição usada do vetor de cidades
                //    incremente quantasCidades
                StreamReader arquivoDeCidades = new StreamReader(dlgAbrirCidades.FileName);
                while (!arquivoDeCidades.EndOfStream)
                {
                    Cidade umaCidade = new Cidade();
                    umaCidade.LerRegistro(arquivoDeCidades);
                    asCidades[quantasCidades] = umaCidade;
                    quantasCidades++;
                }

                // fechar o arquivo de cidades
                // ordenar o vetor de cidades pelo atributo nome
                // e criamos a matriz de adjacência
                arquivoDeCidades.Close();
                OrdenarCidades();
                grafo = new GrafoBacktracking(dlgAbrirCaminhos.FileName, asCidades);

                // copiar os nomes de cada cidade nos cbxOrigem e cbxDestino
                CopiarNomes(cbxOrigem);
                CopiarNomes(cbxDestino);
                AtualizarTela(dgvCaminhos);
            }
        }

        private void btnIncluirCaminho_Click(object sender, EventArgs e)
        {
            if (grafo != null && cbxOrigem.Text != null && cbxDestino.Text != null && udDistancia.Value != 0)
            {
                // pegamos os dados necessários para a inclusão de uma ligação
                // inserimos a ligação entre a cidade de origem e a cidade de destino
                int indexLinha = cbxOrigem.SelectedIndex;
                int indexColuna = cbxDestino.SelectedIndex;
                decimal distancia = udDistancia.Value;

                grafo.CriarLigacao(indexLinha, indexColuna, distancia);
                AtualizarTela(dgvCaminhos);
            }

            else
                MessageBox.Show("Preencha os dados");
        }

        private void btnExcluirCaminho_Click(object sender, EventArgs e)
        {
            if (grafo != null && cbxOrigem.Text != null && cbxDestino.Text != null)
            {
                // pegamos os dados necessários para a exclusão de uma ligação
                // excluimos a ligação entre a cidade de origem e a cidade de destino
                int indexLinha = cbxOrigem.SelectedIndex;
                int indexColuna = cbxDestino.SelectedIndex;

                grafo.ExcluirLigacao(indexLinha, indexColuna);
                AtualizarTela(dgvCaminhos);
            }

            else
                MessageBox.Show("Preencha os dados");
        }

        private void btnAlterarCaminho_Click(object sender, EventArgs e)
        {
            if (grafo != null && cbxOrigem.Text != null && cbxDestino.Text != null && udDistancia.Value != 0)
            {
                // pegamos os dados necessários para a alteração de uma ligação
                // alteramos a ligação entre a cidade de origem e a cidade de destino
                int indexLinha = cbxOrigem.SelectedIndex;
                int indexColuna = cbxDestino.SelectedIndex;
                decimal distancia = udDistancia.Value;

                grafo.AlterarLigacao(indexLinha, indexColuna, distancia);
                AtualizarTela(dgvCaminhos);
            }

            else
                MessageBox.Show("Preencha os dados");
        }

        private void btnBuscarCaminho_Click(object sender, EventArgs e)
        {
            // pegamos as cidades de origem e destino e buscamos os caminhos e
            // buscamos todos os caminhos possíveis entre as cidades selecionadas
            // e depois verificamos qual é o menor caminho dentre os achados
            caminhos = grafo.BuscarTodosOsCaminhos(cbxOrigem.SelectedIndex, cbxDestino.SelectedIndex);
            PilhaVetor<Movimento> menorRota = CalcularMenorRota(caminhos);

            // exibimos a menorRota também em um DataGridView separado
            AtualizarTela(dgvMelhorCaminho, menorRota);
            AtualizarTela(dgvCaminhos, caminhos);
            DesenharMelhorCaminho();
        }

        private void dgvCaminhos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // se estamos com o DataGridView configurado para
            // a exibição dos caminhos encontrados
            if (dgvCaminhos.Columns.Count == 1)
            {
                // limpamos os dados da linha aérea
                // selecionamos o índice da rota clicada
                // identificamos a rota na lista de caminhos
                // preenchemos a linha aérea com as cidades
                // chamamos a função de desenhar a linha no mapa

                linhaAerea = new List<(int origem, int destino)>();
                int indexRota = e.RowIndex;
                List<Movimento> caminhoSelecionado = caminhos[indexRota].Conteudo();

                foreach (Movimento movim in caminhoSelecionado)
                    linhaAerea.Add((movim.Origem, movim.Destino));

                pbMapa2.Invalidate();
            }

            // se estamos com o DataGridView configurado para
            // a exibição dos dados do arquivo .txt
            else
            {
                // configura os comboBoxes de acordo com a célula clicada
                cbxOrigem.SelectedIndex = e.RowIndex;
                cbxDestino.SelectedIndex = e.ColumnIndex;
            }
        }

        // essa sobrecarga atualiza o DataGridView de caminhos
        // com os dados do grafo
        private void AtualizarTela(DataGridView dgv)
        {
            grafo.Exibir(dgvCaminhos);
        }

        // essa sobrecarga atualiza o DataGridView de caminhos
        // com as rotas encontradas entre duas cidades
        private void AtualizarTela(DataGridView dgv, PilhaVetor<Movimento> rota)
        {
            string caminho = "";

            // se há alguma rota, então a exibimos no DataGridView
            if (!rota.EstaVazia)
            {
                // configuramos o DataGridView para os novosDados
                dgv.Rows.Clear();
                dgv.Refresh();
                dgv.RowCount = dgv.ColumnCount = 1;

                dgv.Columns[0].HeaderText = "Menor Rota";
                dgv.Columns[0].Width = 300;

                // a cadeia de strings receberá o nome da cidade de origem e em seguida
                // receberá a cidade subsequente
                List<Movimento> movim = rota.Conteudo();
                caminho = asCidades[movim[0].Origem].NomeCidade.Trim();

                foreach (Movimento movimento in movim)
                    caminho += " -> " + asCidades[movimento.Destino].NomeCidade.Trim();

                dgv[0, 0].Value = caminho;
            }
        }

        // essa sobrecarga atualiza o DataGridView da melhor rota
        private void AtualizarTela(DataGridView dgv, List<PilhaVetor<Movimento>> caminhos)
        {
            // se tem um caminho para exibir
            if (caminhos.Count > 0)
            {
                // configuramos novamente o DataGridView para os novos dados
                dgv.Rows.Clear();
                dgv.Refresh();
                dgv.RowCount = caminhos.Count;
                dgv.ColumnCount = 1;

                dgv.Columns[0].HeaderText = "Caminho";
                dgv.Columns[0].Width = 300;
                dgv.RowHeadersWidth = 50;

                // inserimos todas as rotas no DataGridView de acordo com o index dela na lista
                // então no índice 0 terá a rota de índex 0
                for (int index = 0; index < caminhos.Count; index++)
                {
                    dgv.Rows[index].HeaderCell.Value = index.ToString();

                    List<Movimento> caminhoAtual = caminhos[index].Conteudo();
                    string caminho = asCidades[caminhoAtual[0].Origem].NomeCidade.Trim();

                    foreach (Movimento movim in caminhoAtual)
                        caminho += " -> " + asCidades[movim.Destino].NomeCidade.Trim();

                    // adicionamos o caminho ao DataGridView
                    dgv[0, index].Value = caminho;
                }
            }

            else
                MessageBox.Show("Não foi encontrado nenhum caminho entre as cidades correspondentes");
        }

        private void pbMapa2_Paint(object sender, PaintEventArgs e)
        {
            // se o mapa ainda não foi criado e portanto linhaAerea
            // é nula, então não podemos desenhar nada
            if (linhaAerea == null)
                return;

            // objetos de desenho
            SolidBrush brush = new SolidBrush(Color.Black);
            Pen pen = new Pen(brush, 2);

            // desenhamos a linha aérea
            foreach (var linha in linhaAerea)
            {
                int origem = linha.origem;
                int destino = linha.destino;

                float XOrigem = (float)Math.Round(asCidades[origem].X * pbMapa2.Width);
                float YOrigem = (float)Math.Round(asCidades[origem].Y * pbMapa2.Height);
                float XDestino = (float)Math.Round(asCidades[destino].X * pbMapa2.Width);
                float YDestino = (float)Math.Round(asCidades[destino].Y * pbMapa2.Height);

                e.Graphics.DrawLine(pen, XOrigem, YOrigem, XDestino, YDestino);
            }
        }

        private void OrdenarCidades()
        {
            //asCidades[0] = new Cidade("Campinas", 0, 0);
            //asCidades[1] = new Cidade("Americana", 0, 0); 
            //asCidades[2] = new Cidade("Sumaré", 0, 0);
            //asCidades[3] = new Cidade("Estiva Gerbi", 0, 0);
            //asCidades[4] = new Cidade("Rafard", 0, 0); 
            //asCidades[5] = new Cidade("Rifaina", 0, 0);
            //asCidades[6] = new Cidade("Hortolândia", 0, 0);
            //quantasCidades = 7;

            // Ordenação por seleção direta ou
            // Selection Sort

            for (int lento = 0; lento < quantasCidades; lento++)
            {
                int indiceMenorCidade = lento;
                for (int rapido = lento + 1; rapido < quantasCidades; rapido++)
                    if (asCidades[rapido].NomeCidade.CompareTo(
                          asCidades[indiceMenorCidade].NomeCidade) < 0)
                        indiceMenorCidade = rapido;

                if (indiceMenorCidade != lento)
                {
                    Cidade auxiliar = asCidades[indiceMenorCidade];
                    asCidades[indiceMenorCidade] = asCidades[lento];
                    asCidades[lento] = auxiliar;
                }
            }
        }

        private void CopiarNomes(ComboBox comboBox)
        {
            // adicionamos as cidades nos ComboBoxes
            foreach (Cidade cidade in asCidades)
                if (cidade != null)
                    comboBox.Items.Add(cidade.NomeCidade);
        }

        private PilhaVetor<Movimento> CalcularMenorRota(List<PilhaVetor<Movimento>> caminhos)
        {
            PilhaVetor<Movimento> menorRota = null;

            foreach (var caminho in caminhos)
            {
                int distancia = 0;
                var caminhoAtual = caminho.Conteudo();

                // calcula a disância total da rota em análise
                foreach (Movimento movim in caminhoAtual)
                {
                    int origem = movim.Origem;
                    int destino = movim.Destino;
                    distancia += grafo.Matriz[origem, destino];
                }

                // verificamos se essa é a menor distância encontrada até agora
                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    menorRota = grafo.ClonarPilha(caminho);
                }
            }

            return menorRota;
        }

        private void DesenharMelhorCaminho()
        {
            // se não estamos com o DataGridView configurado para
            // a exibição dos caminhos encontrados, ignoramos
            if (dgvCaminhos.Columns.Count != 1)
                return;


            // limpamos os dados da linha aérea
            // selecionamos o índice da rota clicada
            // identificamos a rota na lista de caminhos
            // preenchemos a linha aérea com as cidades
            // chamamos a função de desenhar a linha no mapa

            linhaAerea = new List<(int origem, int destino)>();
            List<Movimento> caminhoSelecionado = caminhos[0].Conteudo();

            foreach (Movimento movim in caminhoSelecionado)
                linhaAerea.Add((movim.Origem, movim.Destino));

            pbMapa2.Invalidate();
        }
    }
}