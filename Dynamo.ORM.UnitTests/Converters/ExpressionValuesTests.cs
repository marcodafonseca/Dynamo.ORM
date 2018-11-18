using System;
using System.Text;
using Xunit;
using Dynamo.ORM.Converters;
using System.Linq.Expressions;
using Dynamo.ORM.Models;

namespace Dynamo.ORM.UnitTests.Converters
{
    public class ExpressionValuesTests
    {
        /// <summary>
        /// Testing expression "(x) => x.Id == 0"
        /// Expecting string "#id = :val0"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectEqualsExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id == 0;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("#id = :val0", expressionString.ToString());
        }

        /// <summary>
        /// Testing expression "(x) => x.Id > 0"
        /// Expecting string "#id > :val0"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectGreaterThanExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id > 0;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("#id > :val0", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x) => x.Id < 0"
        //// Expecting string "#id < :val0"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectLessThanExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id < 0;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("#id < :val0", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x) => x.Id <= 0"
        //// Expecting string "#id <= :val0"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectLessThanEqualExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id <= 0;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("#id <= :val0", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x) => x.Id <= 0"
        //// Expecting string "#id <= :val0"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectGreaterThanEqualExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id >= 0;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("#id >= :val0", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x) => x.Id <= 0"
        //// Expecting string "#id <= :val0"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectNotEqualExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id != 0;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("#id != :val0", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x) => x.Id != 0 && x.Id > -1 && x.Id < 1"
        //// Expecting string "((#id != :val0 && #id > :val1) && #id < :val2)"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectComplexAndExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id != 0 && x.Id > -1 && x.Id < 1;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("((#id != :val0 && #id > :val1) && #id < :val2)", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x) => x.Id != 0 || x.Id > -1 || x.Id < 1"
        //// Expecting string "((#id != :val0 OR #id > :val1) OR #id < :val2)"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectComplexOrExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => x.Id != 0 || x.Id > -1 || x.Id < 1;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("((#id != :val0 OR #id > :val1) OR #id < :val2)", expressionString.ToString());
        }

        /// <summary>
        //// Testing expression "(x.Id != 0 && x.Id > -1) || x.Id < 1"
        //// Expecting string "((#id != :val0 && #id > :val1) OR #id < :val2)"
        /// </summary>
        [Fact]
        public void TestConvertExpressionValues_ExpectComplexAndOrExpressionString()
        {
            Expression<Func<Base, bool>> expression = (x) => (x.Id != 0 && x.Id > -1) || x.Id < 1;

            var expressionString = new StringBuilder();

            ExpressionValues.ConvertExpressionValues(expression, ref expressionString);

            Assert.Equal("((#id != :val0 && #id > :val1) OR #id < :val2)", expressionString.ToString());
        }
    }
}