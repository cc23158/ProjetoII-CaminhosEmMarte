using System;
using System.Collections.Generic;

public class PilhaVetor<Tipo> : IStack<Tipo>
{
  int maximoPosicoes; // tamanho físico máximo do vetor
  int topo;           // stack pointer, indica o topo da pilha
  Tipo[] pilha;       // vetor de objetos da classe Tipo

  public PilhaVetor(int posic)
  {
    pilha = new Tipo[posic];
    maximoPosicoes = posic;
    topo = -1;                // ainda nada foi empilhado
  }

  public PilhaVetor() : this(500) { }

  public int Tamanho => topo + 1;

  public bool EstaVazia => Tamanho == 0;

  public void Empilhar(Tipo o)
  {
    if (Tamanho == maximoPosicoes)
       throw new Exception("Pilha cheia: overflow!");

    topo = topo + 1;    // avança o topo
    pilha[topo] = o;    // armazena novo elemento "o" no topo
  }

  public Tipo Desempilhar()
  {
    if (EstaVazia)
       throw new Exception("Pilha vazia: underflow!");

    Tipo dadoEmpilhado = pilha[topo];
    pilha[topo] = default(Tipo);    // para limpar essa posição do vetor
    topo = topo - 1;                // retrocede o topo
    return dadoEmpilhado;
  }

  public Tipo OTopo()
  {
    if (EstaVazia)
      throw new Exception("Pilha vazia: underflow!");

    return pilha[topo]; // não altera o topo nem a pilha
  }

  public List<Tipo> Conteudo()
  {
    List<Tipo> resultado = new List<Tipo> ();
    for (int indice=0; indice <= topo; indice++)
      resultado.Add(pilha[indice]); ;

    return resultado;
  }

  public List<Tipo> Conteudo(bool deFormaPura)
  {
    List<Tipo> resultado = new List<Tipo>();
    var novaPilha = new PilhaVetor<Tipo>(maximoPosicoes);
    while (!this.EstaVazia)
    {
      novaPilha.Empilhar(this.Desempilhar());
      resultado.Add(novaPilha.OTopo());
    }

    while (!novaPilha.EstaVazia)
      this.Empilhar(novaPilha.Desempilhar());

    return resultado;
  }
}

