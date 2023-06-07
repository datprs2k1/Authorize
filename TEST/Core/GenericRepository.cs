using AutoMapper;
using TEST.Data;

namespace TEST.Core
{
    public class GenericRepository
    {
        protected APIEntities _context;
        protected IMapper _mapper;
        protected IConfiguration _configuration;
        protected ILogger _logger;
        public GenericRepository(APIEntities context, ILogger logger, IMapper mapper, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
            _configuration = configuration;
        }


    }
}
