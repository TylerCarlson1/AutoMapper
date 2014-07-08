using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AutoMapper
{
    public static class IDBRepositoryExtensions
    {
        public static IEnumerable<TObject> GetAll<TObject, TDBObject>(this IDBRepository self)
            where TObject : class
            where TDBObject : class
        {
            return self.GetMany<TObject, TDBObject>(objs => objs);
        }

        public static IEnumerable<TObject> GetWhere<TObject, TDBObject>(this IDBRepository self, Expression<Func<TDBObject, bool>> function)
            where TObject : class
            where TDBObject : class
        {
            return self.GetMany<TObject,TDBObject>(objs => objs.Where(function));
        }

        public static TObject GetFirst<TObject, TDBObject>(this IDBRepository self)
            where TObject : class
            where TDBObject : class
        {
            return self.GetSingle<TObject, TDBObject>(objs => objs.FirstOrDefault());
        }
        
        public static void UpdateObject<TObject, TDBObject>(this IDBRepository self, Func<IQueryable<TDBObject>, TDBObject> func, Action<TObject> updateAction)
            where TObject : class
            where TDBObject : class
        {
            var obj = self.GetSingle<TObject,TDBObject>(func);
            updateAction(obj);
            self.Save<TDBObject>(obj);
        }

        public static void Save<TDBObject>(this IDBRepository self, object obj)
            where TDBObject : class
        {
            self.Save<TDBObject, TDBObject>(obj);
        }

        public static void UpdateFirst<TObject, TDBObject>(this IDBRepository self, Action<TObject> updateAction)
            where TObject : class
            where TDBObject : class
        {
            self.UpdateObject<TObject, TDBObject>(objs => objs.FirstOrDefault(), updateAction);
        }
    }
}