using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CategoriaController : ControllerBase
    {

        private readonly AppDbContext _context;

        public CategoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("produtos")]
        public ActionResult<IEnumerable<Categoria>> GetCategoriaProduto()
        {
            //return _context.Categorias.Include(p => p.Produtos).ToList();
            return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).ToList();
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> GetCategorias()
        {
            try
            {
                return _context.Categorias.AsNoTracking().Include(p => p.Produtos).ToList();
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
            Categoria categoria = _context.Categorias.AsNoTracking().Include(p => p.Produtos).FirstOrDefault(c => c.CategoriaId == id);

            if (categoria == null) NotFound();

            return Ok(categoria);
        }

        [HttpPost("{id:int}")]
        public ActionResult RegisterCategoria(Categoria categoria)
        {
            if (categoria == null) return BadRequest();

            _context.Categorias.Add(categoria);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId, categoria });
        }

        [HttpPut("{id:int}")]
        public ActionResult UpdateCategoria(int id, Categoria categoria)
        {
            if(id != categoria.CategoriaId) return BadRequest();

            if(categoria is null ) return BadRequest();

            _context.Entry(categoria).State = EntityState.Modified;
            _context.SaveChanges();   

            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult DeleteCategoria(int id)
        {
            Categoria categoria = _context.Categorias.FirstOrDefault(c => c.CategoriaId.Equals(id));

            _context.Categorias.Remove(categoria);
            _context.SaveChanges();

            return Ok(categoria);

        }

    }


}
