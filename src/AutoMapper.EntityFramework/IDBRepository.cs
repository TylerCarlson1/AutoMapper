using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper.EquivilencyExpression;
using AutoMapper.Mappers;
using AutoMapper.QueryableExtensions;

namespace AutoMapper
{
    public interface IDBRepository
    {
        void Save<T,TI>(object value) 
            where T : class
            where TI : class, T;

        void Delete<T>(object value)
            where T : class;

        TDTO GetSingle<TDTO, T>(Func<IQueryable<T>, T> func)
            where T : class
            where TDTO : class;

        IEnumerable<TDTO> GetMany<TDTO, T>(Func<IQueryable<T>, IQueryable<T>> func)
            where T : class
            where TDTO : class;
    }

    public class DBRepository<TDatabase> : IDBRepository
        where TDatabase:DbContext, new()
    {
        static DBRepository()
        {
            EquivilentExpressions.GenerateEquality.Add(new GenerateEntityFrameworkPrimaryKeyEquivilentExpressions<TDatabase>());
        }

        public void Save<T,TI>(object value) 
            where T : class
            where TI : class, T
        {
            using (var db = new TDatabase())
            {
                var equivExpr = Mapper.Map(value, value.GetNonDynamicProxyType(), typeof (Expression<Func<T, bool>>)) as Expression<Func<T,bool>>;
                if(equivExpr == null)
                    return;

                var set = db.Set<T>();
                var equivilent = set.FirstOrDefault(equivExpr);

                if (equivilent == null)
                {
                    equivilent = set.Create<TI>();
                    set.Add(equivilent);
                }
                Mapper.Map(value, equivilent, value.GetNonDynamicProxyType(), typeof(T));
                db.SaveChanges();
            }
        }

        public void Delete<T>(object value)
            where T : class
        {
            using (var db = new TDatabase())
            {
                var equivExpr = Mapper.Map(value, value.GetNonDynamicProxyType(), typeof(Expression<Func<T, bool>>)) as Expression<Func<T, bool>>;
                if (equivExpr == null)
                    return;
                var equivilent = db.Set<T>().FirstOrDefault(equivExpr);

                if (equivilent == null)
                    db.Set<T>().Remove(equivilent);
                db.SaveChanges();
            }
        }

        public TDTO GetSingle<TDTO, T>(Func<IQueryable<T>, T> func)
            where T : class
            where TDTO : class
        {
            using (var db = new TDatabase())
            {
                var equivilent = func(db.Set<T>());

                if (equivilent == null)
                    return null;
                return Mapper.Map<T,TDTO>(equivilent);
            }
        }

        public IEnumerable<TDTO> GetMany<TDTO, T>(Func<IQueryable<T>, IQueryable<T>> func)
            where T : class
            where TDTO : class
        {
            using (var db = new TDatabase())
            {
                var equivilent = func(db.Set<T>());
                return equivilent.Project().To<TDTO>().ToList();
            }
        }
    }

    public static class TypeExtensions
    {
        public static Type GetNonDynamicProxyType(this object item)
        {
            return item.GetType().GetNonDynamicProxyType();
        }

        public static Type GetNonDynamicProxyType(this Type item)
        {
            var type = item;
            if (IsDynamicProxy(type))
                type = type.BaseType;
            return type;
        }

        private static bool IsDynamicProxy(Type type)
        {
            return type.Namespace == "System.Data.Entity.DynamicProxies";
        }
    }
}