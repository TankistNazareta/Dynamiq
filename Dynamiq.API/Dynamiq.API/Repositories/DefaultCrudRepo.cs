using AutoMapper;
using Dynamiq.API.DAL.Context;
using Dynamiq.API.Extension.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dynamiq.API.Repositories
{
    public class DefaultCrudRepo<TDto, TEntity> : ICrudRepo<TDto>
        where TDto : class
        where TEntity : class
    {
        protected readonly AppDbContext _db;
        protected readonly IMapper _mapper;
        protected readonly DbSet<TEntity> _set;

        public DefaultCrudRepo(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _set = _db.Set<TEntity>();
        }

        protected virtual IQueryable<TEntity> Query()
        {
            return _set.AsQueryable();
        }

        public virtual async Task<List<TDto>> GetAll()
        {
            var entities = await Query().ToListAsync();
            return _mapper.Map<List<TDto>>(entities);
        }

        public virtual async Task<TDto> GetById(Guid id)
        {
            var entity = await Query().FirstOrDefaultAsync(x => EF.Property<Guid>(x, "Id") == id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found.");
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> Insert(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _set.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task<TDto> Update(TDto dto)
        {
            var entity = _mapper.Map<TEntity>(dto);
            _set.Update(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<TDto>(entity);
        }

        public virtual async Task Delete(Guid id)
        {
            var entity = await _set.FindAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with ID {id} not found.");
            _set.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
