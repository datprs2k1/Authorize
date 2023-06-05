using AutoMapper;
using TEST.Data;
using TEST.Repositories;

namespace TEST.Core
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly APIEntities _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public IUserRepository Users { get; private set; }

        public UnitOfWork(APIEntities context, ILoggerFactory logger, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            var _logger = logger.CreateLogger("logs");
            Users = new UserRepository(_context, _logger, _mapper, _configuration);
        }
        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
