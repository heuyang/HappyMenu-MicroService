﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HappyMenu.OrderApi.Data.Repository
{
    public interface IRepository<TEntity> where TEntity : class, new()
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task UpdateRangeAsync(List<TEntity> entities);
    }
}
