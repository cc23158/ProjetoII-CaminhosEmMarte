using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IRegistro<Tipo>
{
  void LerRegistro(StreamReader arquivo);
  void GravarDados(StreamWriter arquivo);
  string Chave { get; }
}

