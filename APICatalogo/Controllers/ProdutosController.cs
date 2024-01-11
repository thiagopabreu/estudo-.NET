using APICatalogo.Context;
using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitofwork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet("menor_preco")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPrecos()
        {
            var produtos = _unitofwork.ProdutoRepository.GetProdutosPorPreco().ToList();
            var produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDTO;
        }

        [HttpGet]
        public ActionResult GetProdutos() {
            var produtos = _unitofwork.ProdutoRepository.Get().Include(c => c.Categoria)
                .Select(p => new
                {
                    p.ProdutoId,
                    p.Nome,
                    p.ImagemUrl,
                    p.Descricao,
                    p.Preco,
                    p.Estoque,
                    p.DataCadastro,
                }).ToList();

            if (produtos is null)
            {
                return NotFound();
            }
            return Ok(produtos);
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<Produto> GetProduto(int id)
        {
            Produto produto = _unitofwork.ProdutoRepository.GetById( p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound();
            }
            return produto;

        }

        [HttpPost]
        public ActionResult RegisterProduto(Produto produto)
        {
            if (produto == null)
            {
                return BadRequest();
            }

            _unitofwork.ProdutoRepository.Add(produto);
            _unitofwork.Commit();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId, produto });
        }

        [HttpPut("{id:int}")]
        public ActionResult UpdateProduto(int id, Produto produto)
        {
            if(id != produto.ProdutoId) return BadRequest();
            if(produto == null) return BadRequest();

            _unitofwork.ProdutoRepository.Update(produto);
            _unitofwork.Commit();

            return Ok(produto);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteProduto(int id)
        {
            Produto produto = _unitofwork.ProdutoRepository.GetById(p => p.ProdutoId == id);


            if(produto is null) return NotFound();

            _unitofwork.ProdutoRepository.Delete(produto);
            _unitofwork.Commit();

            return Ok(produto);
        }


    }
}
