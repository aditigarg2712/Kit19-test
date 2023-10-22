using Kit19_test.DataLayer;
using Kit19_test.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Kit19_test.Controllers
{

    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ProductSearch()
        {
            var allProducts = _context.Products.ToList();
            var model = new ProductSearchView()
            {
                SearchResults = allProducts
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult Search(ProductSearchView model)
        {
            // Get all products from the database
            if (ModelState.IsValid)
            {
                IQueryable<Product> query = _context.Products;
                List<Expression<Func<Product, bool>>> conditions = new();
                if (!string.IsNullOrEmpty(model.ProductName))
                {
                    conditions.Add(p => p.ProductName.Contains(model.ProductName));
                }

                if (!string.IsNullOrEmpty(model.Size))
                {
                    conditions.Add(p => p.Size == model.Size);
                }

                if (model.Price.HasValue)
                {
                    conditions.Add(p => p.Price == model.Price);
                }

                if (model.MfgDate.HasValue)
                {
                    conditions.Add(p => p.MfgDate == model.MfgDate);
                }

                if (!string.IsNullOrEmpty(model.Category))
                {
                    conditions.Add(p => p.Category == model.Category);
                }

                if (conditions.Any())
                {
                    Expression<Func<Product, bool>> combinedCondition = conditions.First();
                    if (model.SelectedConjunction == "OR")
                    {
                        foreach (var condition in conditions.Skip(1))
                        {
                            combinedCondition = CombineWithOr(combinedCondition, condition);
                        }

                        query = query.Where(combinedCondition);
                    }
                    else 
                    {
                        foreach (var condition in conditions.Skip(1))
                        {
                            combinedCondition = CombineWithAnd(combinedCondition, condition);
                        }

                        query = query.Where(combinedCondition);

                    }
                }

                List<Product> searchResults = query.ToList();
                model.SearchResults = searchResults;

                return View("ProductSearch", model);
            }

            return View("ProductSearch", model);
        }

        private Expression<Func<Product, bool>> CombineWithOr(Expression<Func<Product, bool>> left, Expression<Func<Product, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Product));
            var leftBody = RebindParameter(left.Body, left.Parameters[0], parameter);
            var rightBody = RebindParameter(right.Body, right.Parameters[0], parameter);

            var orExpression = Expression.OrElse(leftBody, rightBody);

            return Expression.Lambda<Func<Product, bool>>(orExpression, parameter);
        }
        private Expression<Func<Product, bool>> CombineWithAnd(Expression<Func<Product, bool>> left, Expression<Func<Product, bool>> right)
        {
            var parameter = Expression.Parameter(typeof(Product));
            var leftBody = RebindParameter(left.Body, left.Parameters[0], parameter);
            var rightBody = RebindParameter(right.Body, right.Parameters[0], parameter);

            var andExpression = Expression.AndAlso(leftBody, rightBody);

            return Expression.Lambda<Func<Product, bool>>(andExpression, parameter);
        }

        private Expression RebindParameter(Expression expression, ParameterExpression oldParameter, Expression newParameter)
        {
            return new ParameterRebinder(newParameter, oldParameter).Visit(expression);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Expression _newParameter;
            private readonly ParameterExpression _oldParameter;

            public ParameterRebinder(Expression newParameter, ParameterExpression oldParameter)
            {
                _newParameter = newParameter;
                _oldParameter = oldParameter;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _oldParameter ? _newParameter : base.VisitParameter(node);
            }
        }
    }
}

