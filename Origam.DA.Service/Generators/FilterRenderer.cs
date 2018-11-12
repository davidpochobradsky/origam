using System;
using System.Collections.Generic;
using System.Text;
using CSharpFunctionalExtensions;

namespace Origam.DA.Service
{
    class FilterRenderer
    {
        public string In(string leftOperand, IEnumerable<string> options)
        {
            return leftOperand+" IN (" + string.Join(", ",options) + ")";
        }

        public string LogicalAndOr(string functionName, IList<string> arguments)
        {
            if (arguments.Count < 2)
            {
                throw new ArgumentOutOfRangeException("At least 2 arguments must be present fpr AND/OR.");
            }
            int i = 0;
            StringBuilder logicalBuilder = new StringBuilder();
            logicalBuilder.Append("(");
            foreach (string arg in arguments)
            {
                if (arg != string.Empty)
                {
                    if (i > 0)
                    {
                        logicalBuilder.Append(" " + GetOperator(functionName) + " ");
                    }
                    logicalBuilder.Append(arg);
                }
                i++;
            }
            logicalBuilder.Append(")");
            return logicalBuilder.ToString();
        }

        public string BinaryOperator(string leftValue,
            string rightValue, string operatorName)
        {
            switch (operatorName)
            {
                case "Equal":
                    return Equal(leftValue, rightValue);
                case "NotEqual":
                    return NotEqual(leftValue, rightValue);
                default:
                    CheckArgumentEmpty("leftValue", leftValue);
                    CheckArgumentEmpty("rightValue", rightValue);
                    return string.Format("({0} {1} {2})",
                        leftValue, GetOperator(operatorName), rightValue);
            }
        }

        public string NotEqual(string leftValue, string rightValue)
        {
            CheckArgumentEmpty("leftValue", leftValue);
            if (rightValue == null)
            {
                return string.Format("{0} IS NOT NULL", leftValue);
            }
            else
            {
                return string.Format("{0} <> {1}", leftValue, rightValue);
            }
        }

        public string Equal(string leftValue, string rightValue)
        {
            CheckArgumentEmpty("leftValue", leftValue);
            if (rightValue == null)
            {
                return string.Format("{0} IS NULL", leftValue);
            }
            else
            {
                return string.Format("{0} = {1}", leftValue, rightValue);
            }
        }

        public string Not(string argument)
        {
            CheckArgumentEmpty("argument", argument);
            return string.Format("NOT({0})", argument);
        }

        private string GetOperator(string functionName)
        {
            switch (functionName)
            {
                case "NotEqual":
                    return "<>";
                case "Equal":
                    return "=";
                case "Like":
                    return "LIKE";
                case "Add":
                    return "+";
                case "Deduct":
                    return "-";
                case "Multiply":
                    return "*";
                case "Divide":
                    return "/";
                case "LessThan":
                    return "<";
                case "LessThanOrEqual":
                    return "<=";
                case "GreaterThan":
                    return ">";
                case "GreaterThanOrEqual":
                    return ">=";
                case "LogicalOr":
                    return "OR";
                case "LogicalAnd":
                    return "AND";
                default:
                    throw new ArgumentOutOfRangeException("functionName", functionName, ResourceUtils.GetString("UnsupportedOperator"));
            }
        }

        private static void CheckArgumentEmpty(string name, string argument)
        {
            if (argument == null)
            {
                throw new ArgumentOutOfRangeException("name", name, "Argument cannot be empty.");
            }
        }
    }
}