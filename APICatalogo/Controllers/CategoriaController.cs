using APICatalogo.Context;
using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        public CategoriaController(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategoriaProduto([FromBody] CategoriasParameters categoriasParameters)
        {
            //return _context.Categorias.Include(p => p.Produtos).ToList();
            var categorias = _unitofwork.CategoriaRepository.GetCategoriaByProdutos(categoriasParameters);

            var metadata = new {
                categorias.TotalCount,
                categorias.PageSize,
                categorias.CurrentPage,
                categorias.TotalPages,
                categorias.HasNext,
                categorias.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            List<CategoriaDTO> categoriaDTOs = _mapper.Map<List<CategoriaDTO>>(categorias);

            return Ok(categoriaDTOs);
        }

        [HttpGet]
        public ActionResult<IEnumerable<CategoriaDTO>> GetCategorias()
        {
            try
            {
                List<Categoria> categorias = _unitofwork.CategoriaRepository.Get().ToList();
                List<CategoriaDTO> categoriaDTOs = _mapper.Map<List<CategoriaDTO>>(categorias);

                return Ok(categoriaDTOs);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Ocorreu um problema ao tratar a sua solicitação");
            }
        }

        [HttpGet("{id:int}", Name = "ObterCategoria")]
        public ActionResult<Categoria> GetCategoria(int id) 
        {
            Categoria categoria = _unitofwork.CategoriaRepository.GetById(c => c.CategoriaId == id);
            CategoriaDTO categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            if (categoria == null) NotFound();

            return Ok(categoria);
        }

        [HttpPost("{id:int}")]
        public ActionResult RegisterCategoria(CategoriaDTO categoriaDTO)
        {
            if (categoriaDTO == null) return BadRequest();

            Categoria categoria = _mapper.Map<Categoria>(categoriaDTO);

            _unitofwork.CategoriaRepository.Add(categoria);
            _unitofwork.Commit();

            categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoriaDTO.CategoriaId, categoriaDTO });
        }

        [HttpPut("{id:int}")]
        public ActionResult UpdateCategoria(int id, CategoriaDTO categoriaDTO)
        {

            if (categoriaDTO is null) return BadRequest();

            if (id != categoriaDTO.CategoriaId) return BadRequest();

            Categoria categoria = _mapper.Map<Categoria>(categoriaDTO);

            _unitofwork.CategoriaRepository.Update(categoria);
            _unitofwork.Commit();

            categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDTO);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteCategoria(int id)
        {
            Categoria categoria = _unitofwork.CategoriaRepository.GetById(c => c.CategoriaId == id);

            if (categoria is null) return NotFound();

            _unitofwork.CategoriaRepository.Delete(categoria);
            _unitofwork.Commit();

            CategoriaDTO categoriaDTO = _mapper.Map<CategoriaDTO>(categoria);

            return Ok(categoriaDTO);

        }

    }


}
