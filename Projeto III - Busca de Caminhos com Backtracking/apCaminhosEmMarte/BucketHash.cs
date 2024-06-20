/*
Keven Richard da Rocha Barreiros - 23143
Victor Yuji Mimura               - 23158
*/

using apCaminhosEmMarte;
using System;
using System.Collections;
using System.Collections.Generic;

class BucketHash<Tipo> : ITabelaDeHash<Tipo> where Tipo : IRegistro<Tipo>, IComparable<Tipo>
{
    private const int SIZE = 2; // para gerar mais colisões; o ideal é primo > 100
    ArrayList[] dados;
    private int qtsComDados; // para saber quando o ReHash será feito (qtsComDados == dados.Length / 2)

    public BucketHash()
    {
        dados = new ArrayList[SIZE];
        qtsComDados = 0;

        // coloca em cada posição do vetor, um arrayList vazio
        for (int i = 0; i < SIZE; i++)
            dados[i] = new ArrayList(1);
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

    public void Inserir(Tipo item)
    {
        int valorDeHash = Hash(item.Chave);
        if (!dados[valorDeHash].Contains(item))
        {
            // se esse ArrayList não tinha dados, incrementa o qtsComDados           
            if (dados[valorDeHash].Count == 0)
                qtsComDados++;

            dados[valorDeHash].Add(item);

            // se precisamos de um ReHash
            if (qtsComDados == dados.Length / 2)
                ReHash();
        }
    }

    public bool Remover(Tipo item)
    {
        int onde = 0;  // posição que o item deve se encontrar
        int index = 0; // index da remoção do item da lista

        if (!Existe(item, out onde)) // verifica se o item existe e caso positivo, retorna sua posição do vetor
            return false;

        // procura o índice do item a ser excluído na lista
        foreach (Tipo objeto in dados[onde])
        {
            if (objeto.CompareTo(item) == 0)
            {
                dados[onde].RemoveAt(index); // remove o item
                break;
            }

            else // incrementa o index, já que não foi encontrado na posição anterior
                index++;
        }

        return true;
    }

    public bool Existe(Tipo item, out int onde)
    {
        onde = Hash(item.Chave);           // pega a posição do vetor que o item se encontra

        foreach (Tipo objeto in dados[onde]) // verifica se o item está na lista da posição do vetor
            if (objeto.CompareTo(item) == 0)
                return true;

        return false;
    }

    public List<Tipo> Conteudo()
    {
        List<Tipo> saida = new List<Tipo>();

        for (int i = 0; i < dados.Length; i++)
            if (dados[i].Count > 0)
                foreach (Tipo item in dados[i])
                    saida.Add(item);

        return saida;
    }

    public void ReHash()
    {
        // variáveis para aumentar o tamanho de dados
        int dobroTamanho = dados.Length * 2;
        bool primo = false;
        int quantosDiv = 2;

        // procura o valor primo mais próximo do dobro de dados.Length
        while (!primo)
        {
            for (int i = 2; i < (dobroTamanho / 2); i++)
                if (dobroTamanho % i == 0)
                    quantosDiv++;

            if (quantosDiv == 2)
                primo = true;

            dobroTamanho++;
            quantosDiv = 2;
        }

        Tipo[] aux = new Tipo[dobroTamanho];                  // vetor dos itens genéricos
        ArrayList[] outroArray = new ArrayList[dobroTamanho]; // ArrayList auxiliar
        int a = 0;                                            // variável de controle para inserção de dados em aux

        for (int j = 0; j < outroArray.Length; j++)
            outroArray[j] = new ArrayList(1);

        // para cada posição do ArrayList dados, se a lista possui ao menos 1 item,
        // então os itens da lista são inseridos no vetor de itens genéricos
        for (int i = 0; i < dados.Length; i++)
            if (dados[i].Count > 0)
                foreach (Tipo item in dados[i])
                {
                    aux[a] = item;
                    a++;
                }

        dados = new ArrayList[dobroTamanho]; // faz dados ter o tamanho aumentado e consequentemente atualiza dados.Length

        // para cada valor no vetor de itens genéricos não nulos
        for (int i = 0; i < aux.Length; i++)
            if (aux[i] != null)
            {
                Tipo objeto = aux[i];            // define-se o objeto genérico
                string chave = objeto.Chave;     // define-se a chave do objeto
                int posicao = Hash(chave);       // define-se a posição do vetor do objeto
                outroArray[posicao].Add(objeto); // adiciona o objeto em sua respectiva posição em outroArray

            }
        dados = outroArray;
    }
}
