using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TheLinq
{
    class Linquable<T> : IQueryable<T>
    {
        public Linquable()
        {
            Provider = new Linqauider();
            Expression = System.Linq.Expressions.Expression.Constant(this);
        }

        public Linquable(Expression expr)
        {
            Provider = new Linqauider();
            Expression = expr;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Provider.Execute<IEnumerable>(Expression).GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType {
            get { return typeof (T); }
        }
        public IQueryProvider Provider { get; private set; }
    }
}
