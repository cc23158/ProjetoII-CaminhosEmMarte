/*
Keven Richard da Rocha Barreiros - 23143
Victor Yuji Mimura               - 23158
*/

using System;

namespace apCidadesBacktracking
{
    class Movimento
    {
        // onde estou, para onde vou
        private int origem, destino;

        public Movimento(int or, int dest)
        {
            origem = or;
            destino = dest;
        }

        public int CompareTo(Movimento outro) // compatível com ListaSimples e NoLista
        {
            return 0;
        }

        public int Origem { get => origem; set => origem = value; }

        public int Destino { get => destino; set => destino = value; }

        public override String ToString()
        {
            return origem + " " + destino;
        }
    }
}
