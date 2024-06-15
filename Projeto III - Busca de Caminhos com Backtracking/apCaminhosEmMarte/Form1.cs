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
        }

        private void FrmCaminhos_Load(object sender, EventArgs e)
        {

        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {

        }

        Cidade[] asCidades;
        int quantasCidades;   // tamanho lógico
        GrafoBacktracking grafo;

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
            foreach (Cidade cidade in asCidades)
                if (cidade != null)
                    comboBox.Items.Add(cidade.NomeCidade);
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
        }

        private void AtualizarTela(DataGridView dgv)
        {
            grafo.Exibir(dgv);
        }

        private void btnBuscarCaminho_Click(object sender, EventArgs e)
        {
            // pegamos as cidades de origem e destino e buscamos os caminhos
            int cidadeOrigem  = cbxOrigem.SelectedIndex;
            int cidadeDestino = cbxDestino.SelectedIndex;

            int origem, destino, distancia, menorDistancia = Int32.MaxValue;
            PilhaVetor<Movimento> menorRota = new PilhaVetor<Movimento>();
            PilhaVetor<Movimento> cloneRota = new PilhaVetor<Movimento>();
            PilhaVetor<Movimento> rotaAux = new PilhaVetor<Movimento>();

            List<PilhaVetor<Movimento>> caminhos = grafo.BuscarTodosOsCaminhos(cidadeOrigem, cidadeDestino);

            // buscamos o menr caminho dentre todos os encontrados
            foreach(PilhaVetor<Movimento> rota in caminhos)
            {
                distancia = 0;
                cloneRota = grafo.ClonarPilha(rota);
                while (!rota.EstaVazia)
                {
                    Movimento movim = rota.Desempilhar();
                    origem = movim.Origem;
                    destino = movim.Destino;

                    distancia += grafo.Matriz[origem, destino];
                }

                if (distancia < menorDistancia)
                {
                    menorDistancia = distancia;
                    rotaAux = cloneRota;
                }
            }

            // preparamos a rota para inserí-la no listBox
            while(!rotaAux.EstaVazia)
                menorRota.Empilhar(rotaAux.Desempilhar());

            AtualizarTela(dgvMelhorCaminho);
        }
    }
}
