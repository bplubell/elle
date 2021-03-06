using DynamicExpresso;
using System.Collections.Generic;

namespace Elle.Client.Models
{
    public class Calculator
    {
        public List<Expression> Expressions { get; set; } = new List<Expression>();

        public int Id { get; set; }

        public string Name { get; set; } = "New";

        public void AddExpression() => Expressions.Add(new Expression());

        public void Clear() => Expressions = new List<Expression>();

        public void RemoveExpression(int index) => Expressions.RemoveAt(index);

        public void Solve()
        {
            Interpreter interpreter = new Interpreter()
                .EnableAssignment(AssignmentOperators.None);

            foreach (Expression expression in Expressions)
            {
                // Dependent on the order of the expressions; dependent expressions must come later
                object result = interpreter.Eval(expression.Value);
                expression.Result = result switch
                {
                    double doubleResult => doubleResult,
                    int intResult => System.Convert.ToDouble(intResult),
                    _ => 0
                };
                interpreter.SetVariable(expression.Name, expression.Result);
            }
        }
    }
}