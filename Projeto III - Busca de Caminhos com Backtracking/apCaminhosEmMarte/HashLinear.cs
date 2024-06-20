/*
Keven Richard da Rocha Barreiros - 23143
Victor Yuji Mimura               - 23158
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosEmMarte
{
    public class HashLinear<Tipo> : ITabelaDeHash<Tipo> where Tipo : IRegistro<Tipo>, IComparable<Tipo>
    {
        private const int SIZE = 2;      // cria constante para o tamanho de um vetor dados
        private int quantosDados = 0;    // variável de constrole sobre o número de informações em dados
        private int quantasColisoes = 0;
        Tipo[] dados;                    // vetor dados genérico

        public HashLinear()
        {
            dados = new Tipo[SIZE];
        }

        public int Hash(string chave)
        {
            long tot = 0;

            // calcula tot de acordo com os caracteres da string
            for (int i = 0; i < chave.Length; i++)
                tot += 37 * tot + (char)chave[i];

            tot %= dados.Length;

            if (tot < 0) // para que não dê erro de posição inacessível no vetor dados
                tot += dados.Length;

            return (int)tot;
        }

        public void Inserir(Tipo item)
        {
            bool inseriu = false;
            int valorDeHash = Hash(item.Chave); // índice calculado do registro

            while (!inseriu)
            {
                if (dados[valorDeHash] != null)                     // já em uso!!
                {
                    valorDeHash = (valorDeHash + 1) % dados.Length; // calcula nova posição
                    quantasColisoes++;
                }


                else
                {
                    dados[valorDeHash] = item;
                    quantosDados++;
                    inseriu = true;
                }
            }

            if (quantosDados == dados.Length / 2) // se dados está cheio pela metade
                this.ReHash();
        }

        public bool Remover(Tipo item)
        {
            int onde = 0;

            // se o valor existe em "dados", liberamos memória
            if (Existe(item, out onde))
            {
                dados[onde] = default(Tipo);
                return true;
            }


            return false;
        }

        public bool Existe(Tipo item, out int onde)
        {
            onde = Hash(item.Chave); // calcula a posição inicial.

            if (dados[onde] != null && dados[onde].CompareTo(item) == 0)
                return true;

            // se não foi encontrado na posição esperada, procura sequencialmente no resto do vetor

            int ondeAtual = (onde + 1) % dados.Length; // próxima posição a ser verificada.
            int tentativa = 1;

            while (tentativa < quantasColisoes + 1) // enquanto não voltar à posição inicial.
            {
                if (dados[ondeAtual] != null && dados[ondeAtual].CompareTo(item) == 0)
                {
                    onde = ondeAtual;
                    return true;
                }

                ondeAtual = (ondeAtual + 1) % dados.Length; // re-calcula a posição a ser verificada
                tentativa++;
            }

            return false;
        }

        public List<Tipo> Conteudo()
        {
            List<Tipo> saida = new List<Tipo>(); // cria a lista de retorno

            // se o dado lido é diferente de nulo, adiciona-o à lista de retorno
            for (int i = 0; i < dados.Length; i++)
                if (dados[i] != null)
                    saida.Add(dados[i]);

            return saida;
        }

        public void ReHash()
        {
            int dobroTamanho = dados.Length * 2; // calcula o novo tamanho de dados
            bool primo = false;
            int quantosDiv = 2;

            while (!primo)
            {
                // verificamos a quantidade de divisores do novo tamanho
                for (int i = 2; i <= (dobroTamanho / 2); i++)
                    if (dobroTamanho % i == 0)
                        quantosDiv++;

                // se é primo
                if (quantosDiv == 2)
                    primo = true;

                else
                {
                    dobroTamanho++;
                    quantosDiv = 2;
                }
            }

            Tipo[] aux = new Tipo[dobroTamanho]; // vetor de itens genéricos auxiliar
            int a = 0;                           // variável de controle de inserção de itens em aux


            // move as informações de dados para aux
            for (int j = 0; j < dados.Length; j++)
                if (dados[j] != null)
                {
                    aux[a] = dados[j];
                    a++;
                }

            dados = new Tipo[dobroTamanho]; // faz dados aumentar de tamanho e consequentemente atualiza dados.Length
            quantosDados = 0;            // reseta o número de dados para que a inserção dos dados ocorra normalmente

            // reconstrói o vetor dados a partir do método inserir
            for (int i = 0; i < aux.Length; i++)
                if (aux[i] != null)
                    this.Inserir(aux[i]);
        }
    }
}
