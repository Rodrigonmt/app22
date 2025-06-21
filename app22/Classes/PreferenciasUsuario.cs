using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app22.Classes
{
    public static class PreferenciasUsuario
    {
        public static void DefinirUsuario(string nome) =>
            Preferences.Set("UsuarioLogado", nome);

        public static string ObterUsuario() =>
            Preferences.Get("UsuarioLogado", string.Empty);
    }

}