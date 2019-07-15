using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Libs.Misc
{
    public class FieldReference<TSource, TValue>
    {
        private delegate TValue GetHandler(TSource instance);
        private delegate void SetHandler(TSource instance, TValue value);

        private TSource m_Instance;
        private GetHandler m_GetHandler;
        private SetHandler m_SetHandler;

        public TValue Value
        {
            get { return m_GetHandler(m_Instance); }
            set { m_SetHandler(m_Instance, value); }
        }

        private static GetHandler GenerateGetDelegate(FieldInfo fieldInfo)
        {
            ParameterExpression instanceExpression = Expression.Parameter(typeof(TSource));
            MemberExpression fieldExpression = Expression.Field(instanceExpression, fieldInfo);

            Expression<GetHandler> expression = Expression.Lambda<GetHandler>(fieldExpression, instanceExpression);
            return expression.Compile();
        }

        private static SetHandler GenerateSetDelegate(FieldInfo fieldInfo)
        {
            ParameterExpression instanceExpression = Expression.Parameter(typeof(TSource));
            ParameterExpression valueExpression = Expression.Parameter(typeof(TValue));

            Expression<SetHandler> expression = Expression.Lambda<SetHandler>(Expression.Assign(Expression.Field(instanceExpression, fieldInfo), Expression.Convert(valueExpression, fieldInfo.FieldType)), instanceExpression, valueExpression);
            return expression.Compile();
        }

        private static FieldInfo GetFieldInfo(string name)
        {
            return typeof(TSource).GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public FieldReference(TSource instance, string name) : this(instance, GetFieldInfo(name)) { }

        public FieldReference(TSource instance, FieldInfo fieldInfo)
        {
            m_Instance = instance;
            m_GetHandler = GenerateGetDelegate(fieldInfo);
            m_SetHandler = GenerateSetDelegate(fieldInfo);
        }
    }
}
