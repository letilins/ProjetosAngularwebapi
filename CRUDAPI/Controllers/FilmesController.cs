using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using CRUDAPI.models;
using Microsoft.AspNetCore.Mvc;

namespace CRUDAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmesController : ControllerBase
    {
        private const string ArquivoJson = "Data/filmes.json";

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Filmes>>> PegarTodos()
        {
            var filmes = await ObterFilmesDoArquivo();
            return Ok(filmes);
        }

        [HttpGet("{FilmesId}")]
        public async Task<ActionResult<Filmes>> PegarFilmesPeloId(int FilmesId)
        {
            var filmes = await ObterFilmesDoArquivo();
            var filme = filmes.Find(f => f.FilmesId == FilmesId);

            if (filme == null)
                return NotFound();

            return Ok(filme);
        }

        [HttpPost]
        public async Task<ActionResult<Filmes>> SalvarFilmes(Filmes filme)
        {
            var filmes = await ObterFilmesDoArquivo();
            filme.FilmesId = ObterProximoId(filmes);
            filmes.Add(filme);

            await SalvarFilmesNoArquivo(filmes);

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> AtualizarFilmes(Filmes filme)
        {
            var filmes = await ObterFilmesDoArquivo();
            var filmeExistente = filmes.Find(f => f.FilmesId == filme.FilmesId);

            if (filmeExistente == null)
                return NotFound();

            // Atualize os dados do filme existente
            filmeExistente.Nome = filme.Nome;
            filmeExistente.Ano = filme.Ano;
            filmeExistente.Diretor = filme.Diretor;
            filmeExistente.Atores = filme.Atores;
            filmeExistente.Genero = filme.Genero;

            await SalvarFilmesNoArquivo(filmes);

            return Ok();
        }

        [HttpDelete("{FilmesId}")]
        public async Task<ActionResult> ExcluirFilmes(int FilmesId)
        {
            var filmes = await ObterFilmesDoArquivo();
            var filmeExistente = filmes.Find(f => f.FilmesId == FilmesId);

            if (filmeExistente == null)
                return NotFound();

            filmes.Remove(filmeExistente);

            await SalvarFilmesNoArquivo(filmes);

            return Ok();
        }

        private async Task<List<Filmes>> ObterFilmesDoArquivo()
        {
            if (!System.IO.File.Exists(ArquivoJson))
                return new List<Filmes>();

            var json = await System.IO.File.ReadAllTextAsync(ArquivoJson);
            return JsonSerializer.Deserialize<List<Filmes>>(json);
        }

        private async Task SalvarFilmesNoArquivo(List<Filmes> filmes)
        {
            var json = JsonSerializer.Serialize(filmes, new JsonSerializerOptions { WriteIndented = true });
            await System.IO.File.WriteAllTextAsync(ArquivoJson, json);
        }

        private int ObterProximoId(List<Filmes> filmes)
        {
            return filmes.Count + 1;
        }
    }
}
