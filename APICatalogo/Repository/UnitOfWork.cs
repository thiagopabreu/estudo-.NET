using APICatalogo.Context;

namespace APICatalogo.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private IProdutoRepository _ProdutoRepository;

        private ICategoriaRepository _CategoriaRepository;

        public AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public IProdutoRepository ProdutoRepository => _ProdutoRepository ??= new ProdutoRepository(_context);
        public ICategoriaRepository CategoriaRepository => _CategoriaRepository ??= new CategoriaRepository(_context);



        public void Commit()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
