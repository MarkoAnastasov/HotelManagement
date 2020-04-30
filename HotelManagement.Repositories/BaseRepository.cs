using HotelManagement.Interfaces;
using HotelManagement.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace HotelManagement.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private HotelManagementContext _context = new HotelManagementContext();
     
        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public List<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public List<T> GetAllInclude(Expression<Func<T, object>> predicate)
        {
            return _context.Set<T>().Include(predicate).ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void SaveEntities()
        {
            _context.SaveChanges();
        }

    }
}
