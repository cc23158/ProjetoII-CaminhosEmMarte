/*
Keven Richard da Rocha Barreiros - 23143
Victor Yuji Mimura               - 23158
*/

using System;
using System.Collections.Generic;

namespace apCaminhosEmMarte
{
    public interface ITabelaDeHash<Tipo> where Tipo : IRegistro<Tipo>, IComparable<Tipo>
    {
        int Hash(string chave);
        void Inserir(Tipo item);
        bool Remover(Tipo item);
        bool Existe(Tipo item, out int onde);
        List<Tipo> Conteudo();
        void ReHash();
    }
}
