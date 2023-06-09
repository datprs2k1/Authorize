using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TEST.Data;

namespace TEST.Core
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected APIEntities _context;
        protected IMapper _mapper;
        protected ILogger _logger;
        protected IConfiguration _configuration;
        internal DbSet<T> _dbset;

        public GenericRepository(APIEntities context, ILogger logger, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
            _dbset = _context.Set<T>();
        }
        public virtual async Task<bool> Add(T entity)
        {
            await _dbset.AddAsync(entity);
            return true;
        }

        public virtual async Task<IEnumerable<T>> All()
        {
            return await _dbset.AsNoTracking().ToListAsync();
        }

        public virtual async Task<bool> Delete(T entity)
        {
            _dbset.Remove(entity);

            return true;
        }

        public virtual async Task<T?> GetById(int id)
        {
            return await _dbset.FindAsync(id);
        }

        public virtual async Task<bool> Update(T entity)
        {
            _dbset.Update(entity);
            return true;
        }


    }
}
