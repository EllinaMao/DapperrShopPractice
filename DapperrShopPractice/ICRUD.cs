using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperrShopPractice
{
    public interface Icrud<T>
    {
        public List<T> SelectWithCategoryList();//r?
        public IEnumerable<dynamic> SelectWithCategory();//r?
        public bool Delete(T entity);//d
        public T? Update(T entity);//u
        public int Insert(T entity);//c

    }
}
