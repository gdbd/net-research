using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TheLinq
{
    class Linqauider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Linquable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var isCollection = typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>);
            
            var itemType = isCollection
                ? typeof(TResult).GetGenericArguments().Single()
                : typeof(TResult);

            //var queryable = Activator.CreateInstance(
            //    typeof(Linquable<>).MakeGenericType(itemType), this, expression) as IQueryable;

            if (isCollection)
            {
                var list = typeof (List<>).MakeGenericType(itemType);
                return (TResult) Activator.CreateInstance(list);
            }

            return (TResult)Activator.CreateInstance(itemType);
        }
    }
}
