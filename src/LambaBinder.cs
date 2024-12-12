using System;
using System.Linq.Expressions;
using System.Windows.Forms;

namespace  kasthack.binding.wf {
    /// <summary>
    /// Lambda binding helper
    /// </summary>
    public static class LambaBinder
    {
        /// <summary>
        /// Bind Control property to object property. Usage: control.Bind(a=>a.ControlProperty, model, a=>a.ModelProperty.ModelSubProperty)
        /// </summary>
        /// <typeparam name="TControl">Control type(don't specify manually)</typeparam>
        /// <typeparam name="TProperty">Control target property type (don't specify manually)</typeparam>
        /// <typeparam name="TModel">Model type (don't specify manually)</typeparam>
        /// <typeparam name="TBind">Model property type  (don't specify manually)</typeparam>
        /// <param name="control">Control to apply the binding to</param>
        /// <param name="property">Control property expression</param>
        /// <param name="model">Binding model object</param>
        /// <param name="bind">Binding propery expression</param>
        /// <param name="formattingEnabled">Enable formatting</param>
        /// <param name="updateMode">Update mode</param>
        /// <returns></returns>
        public static Binding Bind<TControl, TProperty, TModel, TBind>(this TControl control, Expression<Func<TControl, TProperty>> property, TModel model, Expression<Func<TModel, TBind>> bind, bool formattingEnabled = false, DataSourceUpdateMode updateMode = DataSourceUpdateMode.OnPropertyChanged)
            where TControl : IBindableComponent
            => BindInternal(control, property, model, bind, formattingEnabled, updateMode);

        private static Binding BindInternal<TControl, TModel>(TControl control, LambdaExpression property, TModel model, LambdaExpression bind, bool formattingEnabled, DataSourceUpdateMode updateMode) where TControl : IBindableComponent
        {
            //validate
            CheckPropertyLambda(property, nameof(property));
            CheckPropertyLambda(bind, nameof(bind));
            return control.DataBindings.Add(
                                     GetExpressionPath(property.Body as MemberExpression),
                                     model,
                                     GetExpressionPath(bind.Body as MemberExpression),
                                     formattingEnabled,
                                     updateMode
                );
        }

        private static string GetExpressionPath(Expression property, bool hasChildren = false)
        {
            switch (property)
            {
                case MemberExpression m:
                    return GetExpressionPath(m.Expression, true) + m.Member.Name + (hasChildren ? "." : string.Empty);
                case ParameterExpression p:
                    return string.Empty;
                default:
                    throw new InvalidOperationException("Trying to get expression path for some bullshit");
            }
        }

        private static void CheckPropertyLambda(LambdaExpression expression, string cn)
        {
            if (expression == null || expression.NodeType != ExpressionType.Lambda || !IsValidMemberLambda(expression.Body))
                throw new ArgumentException("Expression must be a property lambda", cn);
        }

        private static bool IsValidMemberLambda(Expression expression, bool hasChildren = false)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return IsValidMemberLambda((expression as MemberExpression).Expression, true);
                case ExpressionType.Parameter:
                    return hasChildren;
                default:
                    return false;
            }
        }
    }
}
