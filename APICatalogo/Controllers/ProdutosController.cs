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
    [Route("api/[controller]")]
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
            List<Produto> produtos = _unitofwork.ProdutoRepository.GetProdutosPorPreco().ToList();
            List<ProdutoDTO> produtosDTO = _mapper.Map<List<ProdutoDTO>>(produtos);

            return produtosDTO;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutos() {
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

            var produtosDTO = _mapper.Map<ProdutoDTO>(produtos);

            if (produtosDTO is null)
            {
                return NotFound();
            }
            return Ok(produtosDTO);
        }

        [HttpGet("{id:int}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> GetProduto(int id)
        {
            Produto produto = _unitofwork.ProdutoRepository.GetById(p => p.ProdutoId == id);

            if (produto is null)
            {
                return NotFound();
            }

            ProdutoDTO produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return produtoDTO;

        }

        [HttpPost]
        public ActionResult RegisterProduto([FromBody]ProdutoDTO produtoDTO)
        {
            if (produtoDTO == null)
            {
                return BadRequest();
            }

            Produto produto = _mapper.Map<Produto>(produtoDTO);
            _unitofwork.ProdutoRepository.Add(produto);
            _unitofwork.Commit();

            produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId, produtoDTO });
        }

        [HttpPut("{id:int}")]
        public ActionResult<Produto> UpdateProduto(int id, ProdutoDTO produtoDTO)
        {
            if(id != produtoDTO.ProdutoId || produtoDTO == null) return BadRequest();

            Produto produto = _mapper.Map<Produto>(produtoDTO);

            _unitofwork.ProdutoRepository.Update(produto);
            _unitofwork.Commit();

            produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteProduto(int id)
        {
            Produto produto = _unitofwork.ProdutoRepository.GetById(p => p.ProdutoId == id);


            if(produto is null) return NotFound();

            _unitofwork.ProdutoRepository.Delete(produto);
            _unitofwork.Commit();

            ProdutoDTO produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }


    }
}
