using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace TheLinq
{
    class LinquableProvidable<T> : IOrderedQueryable<T>, IQueryProvider
    {
        public LinquableProvidable()
        {
            Expression = Expression.Constant(this);
        }

        public LinquableProvidable(Expression expression)
        {
            Expression = expression;
        }

        #region IOrderedQueryable
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get { return typeof (T); }}
        public IQueryProvider Provider { get { return this; } }
        #endregion

        #region IQueryProvider
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new LinquableProvidable<TElement>(expression);
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

            if (isCollection)
            {
                var list = typeof(List<>).MakeGenericType(itemType);
                return (TResult)Activator.CreateInstance(list);
            }

            return (TResult)Activator.CreateInstance(itemType);
        }
        #endregion
    }
}
