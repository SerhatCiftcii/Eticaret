using Eticaret.Core.Entities;
using System.Linq.Expressions; //GetAllın içinde kullandık

namespace Eticaret.Service.Abstract
{
    public interface IService<T> where T : class,IEntity ,new() // T tipinde bir nesne alacak ve bu nesne class olacak
    {
        List<T> GetAll(); // T tipinde bir liste döndürecek
        List<T> GetAll(Expression<Func<T, bool>> expression); // T tipinde bir liste döndürecek ve bu liste filtreli olacak
        IQueryable<T> GetQueryable(); // T tipinde bir sorgu döndürecek

        T Get(Expression<Func<T, bool>> expression); // T tipinde bir nesne döndürecek ve bu nesne filtreli olacak

        T Find(int id); // T tipinde bir nesne döndürecek ve bu nesne id'ye göre bulunacak
        void Add(T entity); // T tipinde bir nesne alacak ve bu nesneyi ekleyecek
        void Update(T entity); // T tipinde bir nesne alacak ve bu nesneyi güncelleyecek

        void Delete(T entity); // T tipinde bir nesne alacak ve bu nesneyi silecek
        int SaveChanges(); // Değişiklikleri kaydedecek

        // Asenkron işlemler

      Task<T> FindAsync(int id);
      Task<T> GetAsync(Expression<Func<T, bool>> expression);
        Task<List<T>> GetAllAsync(); // T tipinde bir liste döndürecek
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> expression);// T tipinde bir liste döndürecek ve bu liste filtreli olacak

        Task AddAsync(T entity); // T tipinde bir nesne alacak ve bu nesneyi ekleyecek
      Task<int> SaveChangesAsync(); // Değişiklikleri kaydedecek

    }
}
