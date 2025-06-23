using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app22.Classes
{
    public class Produto
    {
        public string NomeProduto { get; set; }
        public string NomeVendedor { get; set; }
        public string Marca { get; set; }
        public string Descricao { get; set; }
        public string Estado { get; set; }
        public string Valor { get; set; }
        public string Telefone { get; set; }
        public List<string> Fotos { get; set; } = new(); // Evita null
        public string DataPublicacao { get; set; }
    }
}
