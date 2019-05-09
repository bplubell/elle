﻿using Microsoft.AspNetCore.Components;
using Blazor.Extensions.Storage;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Elle.Models;
using DynamicExpresso;

namespace Elle.ViewModels
{
    public class CalculatorViewModel : ComponentBase
    {
        private readonly string _key = "expressions";
        
        [Parameter]
        public Calculator Calculator { private get; set; } = new Calculator();

        public CalculatorViewModel()
        {
        }

        public CalculatorViewModel(Calculator calculator)
        {
            Calculator = calculator;
        }

        [Inject]
        protected LocalStorage? LocalStorage { get; private set; }

        protected override async Task OnInitAsync()
        {
            if (LocalStorage != null)
            {
                Expression[] expressions = await LocalStorage.GetItem<Expression[]>(_key);
                if (expressions != null)
                {
                    Expressions = expressions.ToList();
                }
            }
        }

        protected List<Expression> Expressions
        {
            get => Calculator.Expressions;
            set => Calculator.Expressions = value;
        }

        protected void AddExpression() => Calculator.AddExpression();

        protected string Name => Calculator.Name;

        protected void RemoveExpression(int index) => Calculator.RemoveExpression(index);

        protected void Clear() => Calculator.Clear();

        protected void Solve()
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

        protected bool ExpressionNameIsValid(string name)
        {
            return (!string.IsNullOrWhiteSpace(name)
                && Expressions.Count(ex => ex.Name == name) == 1);
        }

        protected async void SaveLocal()
        {
            if (LocalStorage != null)
            {
                await LocalStorage.SetItem<Expression[]>(_key, Expressions.ToArray());
            }
        }

    }
}
