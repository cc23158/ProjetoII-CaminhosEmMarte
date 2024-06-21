/*
Keven Richard da Rocha Barreiros - 23143
Victor Yuji Mimura               - 23158
*/

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
                pbMapa.Invalidate();
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
            if (dlgFecharCidade.ShowDialog() == DialogResult.OK) // abre a caixa de diálogos
            {
                // o arquivo escolhido pelo usuário terá os dados de tabelaDeHash gravados
                var arquivo = new StreamWriter(dlgFecharCidade.FileName);

                // a tabela deve ser percorrida e os dados atualizados
                foreach (Cidade cidade in tabela.Conteudo())
                    cidade.GravarDados(arquivo);

                arquivo.Close();
            }

            // abrimos o arquivo para saída, se houver um arquivo selecionado
            // chamamos a função para gravar os dados da matriz no arquivo
            if (dlgFecharCaminho.ShowDialog() == DialogResult.OK)
            {
                StreamWriter arquivo = new StreamWriter(dlgFecharCaminho.FileName);
                grafo.GravarDados(arquivo);
            }
        }

        private void btnInserir_Click(object sender, EventArgs e)
        {
            if (txtCidade.Text != "")
            {
                string nomeCidade = txtCidade.Text.Trim().PadRight(15); // nome da cidade digitado (formatado)
                double x = (double)udX.Value;                        // coordenada X digitada
                double y = (double)udY.Value;                        // coordenada Y digitada

                // formatamos as coordenadas para 7.5f
                string strX = x.ToString("0.00000");
                string strY = y.ToString("0.00000");

                Cidade cidade = new Cidade(nomeCidade, double.Parse(strX), double.Parse(strY));     // cria objeto Cidade com os dados digitados

                tabela.Inserir(cidade); // insere o objeto Cidade na tabelaDeHash
                ListarNoListBox();
            }

            else
                MessageBox.Show("Preencha os campos");
        }

        private void btnRemover_Click(object sender, EventArgs e)
        {
            if (txtCidade.Text != "")
            {
                string nomeCidade = txtCidade.Text.Trim().PadRight(15); // nome da cidade digitado (formatado)
                Cidade cidade = new Cidade(nomeCidade, 0, 0);

                tabela.Remover(cidade);
                ListarNoListBox();
            }

            else
                MessageBox.Show("Preencha o campos de nome da cidade");
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            if (txtCidade.Text != "")
            {
                string nomeCidade = txtCidade.Text.Trim().PadRight(15);
                Cidade cidade = new Cidade(nomeCidade, 0, 0);
                int index = -1;

                // se a cidade procurada existe na tabelaDeHash
                // exibimos as informações dela na tela
                if (tabela.Existe(cidade, out index))
                {
                    List<Cidade> conteudo = tabela.Conteudo();

                    foreach (Cidade cid in conteudo)
                        if(cid.CompareTo(cidade) == 0)
                        {
                            udX.Value = (decimal)cid.X;
                            udY.Value = (decimal)cid.Y;
                            break;
                        }
                }
            }

            else
                MessageBox.Show("Preencha o campo de nome da cidade");
        }

        private void btnListar_Click(object sender, EventArgs e)
        {
            // se a tabelaDeHash existe, exibimos as informações
            if (tabela != null)
            {
                lsbCidades.Items.Clear();

                foreach (Cidade cid in tabela.Conteudo()) // para cada objeto Cidade na tabelaDeHash
                {
                    string nomeCidade = cid.Chave;        // define o nome da cidade
                    string x = cid.X.ToString("0.00000"); // coordenada X
                    string y = cid.Y.ToString("0.00000"); // coordenada Y

                    lsbCidades.Items.Add($"{nomeCidade} - ({x} , {y})");
                }
            }

            else
                MessageBox.Show("Não foi possível carregar os dados do ListBox");
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            if (tabela != null)
            {
                // variável que recebe uma lista dos objetos inseridos na tabelaDeHash
                var conteudo = tabela.Conteudo();

                // variável do tamanho da fonte da cidade
                const float tamanhoDaFonte = 8.25f;

                if (conteudo.Count != 0)                // se há elementos na lista retornada
                    foreach (Cidade cidade in conteudo) // para cada objeto Cidade na lista retornada
                    {
                        // calculamos as coordenadas da cidade no mapa
                        float x = (float)Math.Round(pbMapa.Width * cidade.X);
                        float y = (float)Math.Round(pbMapa.Height * cidade.Y);

                        // define o nome da cidade para colocá-la no mapa
                        string nomeCidade = cidade.Chave;

                        // define a fonte da cidade
                        Font fonte = new Font("Sans Serif", tamanhoDaFonte);

                        // define objeto para "pintar" as figuras que serão desenhados no mapa
                        SolidBrush brush = new SolidBrush(Color.Black);

                        // desenha uma ellipse nas coordenadas (x,y) de largura 5px e altura 5px
                        e.Graphics.FillEllipse(brush, x, y, 5, 5);

                        // escreve o nome da cidade no mapa nas coordenadas (x,y) usando a fonte e a cor definidas anteriormente
                        e.Graphics.DrawString(nomeCidade, fonte, brush, x, y);
                    }
            }
        }

        // método que percorre o conteúdo da tabela e lista no ListBox
        private void ListarNoListBox()
        {
            lsbCidades.Items.Clear();
            List<Cidade> conteudo = tabela.Conteudo();

            foreach (Cidade cid in conteudo)
                lsbCidades.Items.Add(cid);
        }

        /////////////// Projeto Caminhos em Marte ///////////////
        
        Cidade[] asCidades;
        int quantasCidades;   // tamanho lógico
        GrafoBacktracking grafo;

        // criamos variável para saber a menor distância e consequentemente, menor caminho
        // criamos variável para saber todos os possíveis caminhos
        // criamos variável para armazenar o caminho a ser desenhado no mapa
        // criamos variável para armazenar o caminho mais curto
        int menorDistancia = Int32.MaxValue;
        List<PilhaVetor<Movimento>> caminhos = null;
        List<(int origem, int destino)> linhaAerea = null;
        PilhaVetor<Movimento> menorRota = null;

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
                pbMapa2.Invalidate();
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
            //se as cidades de origem e destino são iguais, não será
            //produzida uma rota
            if(cbxOrigem.SelectedIndex == cbxDestino.SelectedIndex)
            {
                MessageBox.Show("Não é possível criar uma rota entre a cidade e si. Selecione cidades diferentes para origem e destino");
            }
            else
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

        private void dgvMelhorCaminho_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // se estamos com o DataGridView configurado para
            // a exibição dos caminhos encontrados
            if (dgvMelhorCaminho.Columns.Count == 1 && menorRota != null)
            {
                // limpamos os dados da linha aérea
                // selecionamos o índice da rota clicada
                // identificamos a rota na lista de caminhos
                // preenchemos a linha aérea com as cidades
                // chamamos a função de desenhar a linha no mapa

                linhaAerea = new List<(int origem, int destino)>();
                

                foreach (Movimento movim in menorRota.Conteudo())
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
            // se estamos criando o mapa, desenhamos as cidades
            if (asCidades.Length > 0)
            {
                // variável do tamanho da fonte da cidade
                const float tamanhoDaFonte = 8.25f;

                foreach (Cidade cidade in asCidades) // para cada objeto Cidade na lista retornada
                    if (cidade != null)
                    {
                        // calculamos as coordenadas da cidade no mapa
                        float x = (float)Math.Round(pbMapa2.Width * cidade.X);
                        float y = (float)Math.Round(pbMapa2.Height * cidade.Y);

                        // define o nome da cidade para colocá-la no mapa
                        string nomeCidade = cidade.Chave;

                        // define a fonte da cidade
                        Font fonte = new Font("Sans Serif", tamanhoDaFonte);

                        // define objeto para "pintar" as figuras que serão desenhados no mapa
                        SolidBrush brush2 = new SolidBrush(Color.Black);

                        // desenha uma ellipse nas coordenadas (x,y) de largura 5px e altura 5px
                        e.Graphics.FillEllipse(brush2, x, y, 5, 5);

                        // escreve o nome da cidade no mapa nas coordenadas (x,y) usando a fonte e a cor definidas anteriormente
                        e.Graphics.DrawString(nomeCidade, fonte, brush2, x, y);
                    }
            }


            // se o mapa ainda não foi criado e portanto linhaAerea
            // é nula, então não podemos desenhar nada
            if (linhaAerea == null)
                return;

            // objetos de desenho
            SolidBrush brush = new SolidBrush(Color.Blue);
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
