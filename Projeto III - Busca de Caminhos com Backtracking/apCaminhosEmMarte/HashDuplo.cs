/*
Keven Richard da Rocha Barreiros - 23143
Victor Yuji Mimura               - 23158
*/

using System;
using System.Collections.Generic;

namespace apCaminhosEmMarte
{
    public class HashDuplo<Tipo> : ITabelaDeHash<Tipo> where Tipo : IRegistro<Tipo>, IComparable<Tipo>
    {
        private const int SIZE = 2;      // cria constante para o tamanho de um vetor dados
        private int quantosDados = 0;    // variável de constrole sobre o número de informações em dados
        private int quantasColisoes = 0;
        Tipo[] dados;                    // vetor dados genérico

        public HashDuplo()
        {
            dados = new Tipo[SIZE];
        }

        public int Hash(string chave)
        {
            long tot = 0;
            for (int i = 0; i < chave.Length; i++)
                tot += 37 * tot + (char)chave[i];

            tot = tot % dados.Length;
            if (tot < 0)
                tot += dados.Length;

            return (int)tot;
        }

        private int Hash2(int hash)
        {
            return dados.Length - (hash % dados.Length);
        }

        public void Inserir(Tipo item)
        {
            int valorDeHash = Hash(item.Chave);          // pega a chave do item por meio do GET
            int valorDeHash2 = Hash2(valorDeHash); // pega a segunda chave

            // garantias para inserir os dados
            bool inseriu = false;
            int valorDeHashAtual = (valorDeHash + 1) % dados.Length;
            int valorDeHashAtual2 = (valorDeHash2 + 1) % dados.Length;

            if (dados[valorDeHash] == null)
            {
                quantosDados++;
                inseriu = true;
                dados[valorDeHash] = item;
            }

            else if (dados[valorDeHash2] == null)
            {
                quantosDados++;
                inseriu = true;
                dados[valorDeHash2] = item;
            }

            // tentamos inserir o item em alguma das chaves definidas anteriormente

            while (!inseriu)
            {
                if (dados[valorDeHashAtual] == null)
                {
                    quantosDados++;
                    inseriu = true;
                    dados[valorDeHashAtual] = item;
                }

                else if (dados[valorDeHashAtual2] == null)
                {
                    quantosDados++;
                    inseriu = true;
                    dados[valorDeHashAtual2] = item;
                }

                valorDeHashAtual = (valorDeHashAtual + 1) % dados.Length;
                valorDeHashAtual2 = (valorDeHashAtual2 + 1) % dados.Length;
            }

            if (quantosDados == dados.Length / 2) // se dados está cheio pela metade
                ReHash();
        }

        public bool Remover(Tipo item)
        {
            int onde;

            // se o valor existe em "dados", liberaremos memória
            if (Existe(item, out onde))
            {
                dados[onde] = default(Tipo);
                return true;
            }
                
            return false;
        }

        public bool Existe(Tipo item, out int onde)
        {
            onde = Hash(item.Chave); // recebe a posição do vetor que teoricamente será inserido

            // se dados[onde] está em uso e item é igual à informação de dados[onde]
            if (dados[onde] != null && dados[onde].CompareTo(item) == 0)
                return true;

            // se não encontramos o elemento onde ele deveria estar, então procuramos sequencialmente,
            // visto que o ReHash interfere no método Existir ao mudar dados.Length
            int ondeAtual = (onde + 1) % dados.Length;
            while (onde != ondeAtual)
            {
                if (dados[ondeAtual] != null && dados[ondeAtual].CompareTo(item) == 0)
                {
                    onde = ondeAtual;
                    return true;
                }

                ondeAtual = (ondeAtual + 1) % dados.Length;
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

            Tipo[] aux = new Tipo[dobroTamanho]; // vetor de itens genéricos
            int a = 0;                           // variável de controle de inserção de dados em aux

            // move as informações de dados para outroArray
            for (int j = 0; j < dados.Length; j++)
                if (dados[j] != null)
                {
                    aux[a] = dados[j];
                    a++;
                }

            dados = new Tipo[dobroTamanho]; // faz dados aumentar de tamanho e consequentemente atualiza dados.Length
            quantosDados = 0;               // reseta o número de dados para que a inserção dos dados ocorra normalmente

            // reconstrói o vetor dados a partir do método inserir
            for (int i = 0; i < aux.Length; i++)
                if (aux[i] != null)
                    this.Inserir(aux[i]);
        }
    }
}