using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace app22.Classes
{
    public class GravaDadosUsuario
    {
        private readonly HttpClient _httpClient;
        private const string FirebaseUrl = "https://agendaluiz-default-rtdb.firebaseio.com"; // sem a barra final

        public GravaDadosUsuario()
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
    }
}
