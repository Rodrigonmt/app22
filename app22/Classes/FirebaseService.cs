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

        public async Task AtualizarPessoaAsync(string usuarioAntigo, DadosUsuario pessoaAtualizada)
        {
            var response = await _httpClient.GetAsync($"{FirebaseUrl}/pessoas.json");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao buscar usuários: " + response.StatusCode);

            string json = await response.Content.ReadAsStringAsync();
            var pessoasDict = JsonSerializer.Deserialize<Dictionary<string, DadosUsuario>>(json);

            if (pessoasDict == null) throw new Exception("Nenhum usuário encontrado.");

            // Procurar o ID da pessoa com o nome antigo
            var pessoaEntry = pessoasDict.FirstOrDefault(p =>
                p.Value.usuario?.Trim().ToLower() == usuarioAntigo.Trim().ToLower());

            if (pessoaEntry.Key == null)
                throw new Exception("Usuário não encontrado para atualização.");

            // Atualizar os dados
            var updatedJson = JsonSerializer.Serialize(pessoaAtualizada);
            var content = new StringContent(updatedJson, Encoding.UTF8, "application/json");

            var putResponse = await _httpClient.PutAsync($"{FirebaseUrl}/pessoas/{pessoaEntry.Key}.json", content);

            if (!putResponse.IsSuccessStatusCode)
                throw new Exception("Erro ao atualizar o usuário no Firebase: " + putResponse.StatusCode);
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

        public async Task SalvarAgendamentoAsync(string userId, AgendamentoModel agendamento)
        {
            var agendamentoId = Guid.NewGuid().ToString();

            var json = JsonSerializer.Serialize(agendamento);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var url = $"https://agendaluiz-default-rtdb.firebaseio.com/{"Agendamentos"}/{userId}/{agendamentoId}.json";
            var response = await _httpClient.PutAsync(url, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Erro ao salvar agendamento");
        }

        public async Task<List<AgendamentoModel>> ObterAgendamentosAsync(string nomeUsuario)
        {
            var client = new HttpClient();
            var url = $"https://agendaluiz-default-rtdb.firebaseio.com/{nomeUsuario}.json";

            var response = await client.GetStringAsync(url);
            var resultado = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, AgendamentoModel>>(response);

            return resultado?.Values.ToList() ?? new List<AgendamentoModel>();
        }
    }
}
