using System;
using System.Collections.Generic;
using System.Text;

namespace HotelManagement.Interfaces
{
    public interface IBaseRepository<T>
    {
        public void Add(T entity);

        public List<T> GetAll();

        public T GetById(int id);

        public void Update(T entity);

        void Remove(T entity);

        void SaveEntities();
        
    }
}
