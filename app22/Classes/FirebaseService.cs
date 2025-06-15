using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;

namespace app22.Classes
{
    public class FirebaseService
    {
        private readonly HttpClient _httpClient;
        private const string FirebaseUrl = "https://agendaluiz-default-rtdb.firebaseio.com"; // sem a barra final

        public FirebaseService()
        {
            _httpClient = new HttpClient();
        }

        public async Task GravarPessoaAsync(DadosUsuario pessoa)
        {
            // Gera ID aleatório no Firebase
            string json = JsonSerializer.Serialize(pessoa);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{FirebaseUrl}/pessoas.json", content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao gravar no Firebase: " + response.StatusCode);
        }
        public async Task<DadosUsuario?> BuscarPessoaPorNomeAsync(string nomeProcurado)
        {
            var response = await _httpClient.GetAsync($"{FirebaseUrl}/pessoas.json");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao buscar dados: " + response.StatusCode);

            string json = await response.Content.ReadAsStringAsync();

            // Dicionário com ID do Firebase e o objeto Pessoa
            var pessoasDict = JsonSerializer.Deserialize<Dictionary<string, DadosUsuario>>(json);

            if (pessoasDict == null) return null;

            // Busca pelo nome (ignora maiúsculas/minúsculas)
            foreach (var pessoa in pessoasDict.Values)
            {
                if (pessoa.usuario?.Trim().ToLower() == nomeProcurado.Trim().ToLower())
                {
                    return pessoa;
                }
            }

            return null; // não encontrado
        }
    }
}
