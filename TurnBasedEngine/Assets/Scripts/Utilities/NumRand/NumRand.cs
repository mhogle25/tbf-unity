using System;
using System.Collections.Generic;
using UnityEngine;

namespace BF2D.Utilities
{
    [Serializable]
    public class NumRand
    {
        private const char OP_RANGE = '|';
        private const char OP_ADD = '+';
        private const char OP_SUB = '-';
        private const char OP_MULT = '*';
        private const char OP_INCLUDE = '!';
        private const char OP_EXCLUDE = '_';

        private readonly HashSet<char> operatorsSet = new() {
            NumRand.OP_RANGE,
            NumRand.OP_ADD,
            NumRand.OP_SUB,
            NumRand.OP_MULT,
            NumRand.OP_INCLUDE,
            NumRand.OP_EXCLUDE
        };

        public class TextSpecs
        {
            public string modifyEveryRandOp = null;
            public Dictionary<string, string> termRegistry = new();
            public Color randModifierColor = Color.white;
        }

        public class CalcSpecs
        {
            public int? modifyEveryRandOp = null;
            public Dictionary<string, int> termRegistry = new();
            public bool canExceedMax = false;
            public bool canExceedMin = false;
            public bool showLogs = false;
        }

        private class FloatOp
        {
            public FloatOp(string raw)
            {
                this.raw = raw;
            }

            public float Arg0 { set { this.arg0 = value; } }
            private float? arg0 = null;
            public float Arg1 { set { this.arg1 = value; } }
            private float? arg1 = null;

            private readonly string raw = string.Empty;

            public bool AnyArgumentsSet { get { return this.arg0 is not null || this.arg1 is not null; } }

            public bool FirstArgumentSet { get { return this.arg0 is not null && this.arg1 is null; } }

            public int Calculate(CalcSpecs specs)
            {
                if (this.arg0 is null || this.arg1 is null)
                    throw new Exception("[NumRand:FloatOp:Calculate] Tried to calculate but one or both of the arguments were null");

                return Calculate((float) this.arg0, (float) this.arg1, specs);
            }

            private int Calculate(float x, float y, CalcSpecs specs)
            {
                if (this.raw.Length < 1)
                    throw new Exception($"[NumRand:FloatOp:Calculate] Tried to calculate but the operation was invalid -> {this.raw}");

                if (this.raw[0] == NumRand.OP_ADD)
                {
                    return (int) (x + y);
                }
                else if (this.raw[0] == NumRand.OP_SUB)
                {
                    return (int) (x - y);
                }
                else if (this.raw[0] == NumRand.OP_MULT)
                {
                    return (int) (x * y);
                }
                else if (RangeOperatorVerify(this.raw))
                {
                    float max = x > y ? x : y;
                    float min = x < y ? x : y;

                    int value = UnityEngine.Random.Range((int) min, (int) max);

                    if (IsRangeInclusive(this.raw))
                        value = (int) UnityEngine.Random.Range(min, max);

                    if (specs.showLogs)
                        Terminal.IO.Log($"Random Value: {value}");

                    value += specs.modifyEveryRandOp is null ? 0 : (int) specs.modifyEveryRandOp;

                    if (!specs.canExceedMax && value > max)
                        value = (int) max;

                    if (!specs.canExceedMin && value < min)
                        value = (int) min;

                    if (specs.showLogs)
                        Terminal.IO.Log($"After Specs: {value}");

                    return value;
                }
                else
                {
                    throw new Exception($"[NumRand:FloatOp:Calculate] Syntax Error: The operation '{this.raw}' was invalid");
                }
            }

            private bool RangeOperatorVerify(string op)
            {
                for (int i = 0; i < op.Length; i++)
                {
                    if (op[i] != NumRand.OP_RANGE && op[i] != NumRand.OP_INCLUDE && op[i] != NumRand.OP_EXCLUDE)
                        return false;
                }
                return true;
            }

            private bool IsRangeInclusive(string op)
            {
                string full = op;

                if (full.Length > 2)
                {
                    throw new Exception($"[NumRand:FloatOp:IsRangeInclusive] Syntax Error: The operation '{full}' was invalid");
                }

                if (full.Length < 2)
                {
                    if (full[0] != NumRand.OP_RANGE)
                    {
                        throw new Exception($"[NumRand:FloatOp:IsRangeInclusive] Syntax Error: Range operator was incorrectly formatted -> full");
                    }
                    full += NumRand.OP_EXCLUDE;
                }

                return full[1] == NumRand.OP_INCLUDE;
            }
        }

        private class StringOp
        {
            public StringOp(string op)
            {
                this.op = op;
            }

            public string Arg0 { set { this.arg0 = value; } }
            private string arg0 = null;
            public string Arg1 { set { this.arg1 = value; } }
            private string arg1 = null;

            private readonly string op = string.Empty;

            public bool AnyArgumentsSet { get { return this.arg0 is not null || this.arg1 is not null; } }

            public bool FirstArgumentSet { get { return this.arg0 is not null && this.arg1 is null; } }

            public string TextBreakdown(TextSpecs specs)
            {
                if (this.arg0 is null || this.arg1 is null)
                    throw new Exception("[NumRand:StringOp:TextBreakdown] Tried to create a text breakdown but one or both of the arguments were null");

                return Calculate(this.arg0, this.arg1, specs);
            }

            private string Calculate(string x, string y, TextSpecs specs)
            {
                if (this.op.Length < 1)
                    throw new Exception($"[NumRand:StringOp:TextBreakdown] Tried to create a text breakdown but the operation was invalid -> {this.op}");

                if (this.op == $"{NumRand.OP_ADD}")
                {
                    return $"{x}{NumRand.OP_ADD}{y}";
                }
                else if (this.op == $"{NumRand.OP_SUB}")
                {
                    return $"{x}{NumRand.OP_SUB}{y}";
                }
                else if (this.op == $"{NumRand.OP_MULT}")
                {
                    return $"{x}{NumRand.OP_MULT}{y}";
                }
                else if (RangeOperatorVerify(this.op))
                {
                    if (string.IsNullOrEmpty(specs.modifyEveryRandOp))
                        return $"<color=#{ColorUtility.ToHtmlStringRGBA(specs.randModifierColor)}>{x} to {y}</color>";

                    return $"<color=#{ColorUtility.ToHtmlStringRGBA(specs.randModifierColor)}>{x} to {y} {specs.modifyEveryRandOp}</color>";
                }
                else
                {
                    throw new Exception($"[NumRand:StringOp:TextBreakdown] Syntax Error: The operation '{this.op}' was invalid");
                }
            }

            private bool RangeOperatorVerify(string op)
            {
                for (int i = 0; i < op.Length; i++)
                {
                    if (op[i] != NumRand.OP_RANGE && op[i] != NumRand.OP_INCLUDE && op[i] != NumRand.OP_EXCLUDE)
                        return false;
                }
                return true;
            }
        }

        private readonly Stack<FloatOp> opStackF = new();
        private FloatOp CurrentOpF { get { return this.opStackF.Peek(); } }

        private readonly Stack<StringOp> opStackS = new();
        private StringOp CurrentOpS { get { return this.opStackS.Peek(); } }

        public int Calculate(string expression, CalcSpecs specs)
        {
            if (string.IsNullOrEmpty(expression))
                return 0;

            return CalculateParser(expression, specs);
        }

        public string TextBreakdown(string expression, TextSpecs specs)
        {
            if (string.IsNullOrEmpty(expression))
                return string.Empty;

            return TextBreakdownParser(expression, specs);
        }

        private int CalculateParser(string expression, CalcSpecs specs)
        {
            string[] operations = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (operations.Length < 3)
                throw new Exception($"[NumRand:CalculateParser] Syntax Error: The expression supplied was smaller than a single operation -> {expression}");

            int i = 1;

            if (!IsAnOperator(operations[0]))
                throw new Exception($"[NumRand:CalculateParser] Syntax Error: The first term of the expression must be an operator");

            this.opStackF.Push(new FloatOp(operations[0]));

            float? stagedValue = null;
            while (this.opStackF.Count > 0)
            {
                if (i >= operations.Length)
                    throw new Exception($"[NumRand:TextBreakdownParser] Syntax Error: Incomplete operation -> {expression}");

                if (i < operations.Length && IsAnOperator(operations[i]))
                {
                    this.opStackF.Push(new FloatOp(operations[i]));
                    i++;
                    continue;
                }

                float arg = 0f;
                if (stagedValue is not null)
                {
                    arg = (float) stagedValue;
                    stagedValue = null;
                }
                else if (!specs.termRegistry.ContainsKey(operations[i]))
                {
                    try
                    {
                        arg = float.Parse(operations[i]);
                    }
                    catch
                    {
                        throw new Exception($"[NumRand:CalculateParser] Syntax Error: The provided term could not be converted into a float -> {operations[i]}");
                    }
                }

                arg = specs.termRegistry.ContainsKey(operations[i]) ? specs.termRegistry[operations[i]] : arg;

                if (!this.CurrentOpF.AnyArgumentsSet)
                {
                    this.CurrentOpF.Arg0 = arg;
                    i++;
                    continue;
                }

                if (this.CurrentOpF.FirstArgumentSet)
                {
                    this.CurrentOpF.Arg1 = arg;
                    FloatOp op = this.opStackF.Pop();
                    int value = op.Calculate(specs);
                    stagedValue = value;
                    continue;
                }
            }

            if (specs.showLogs)
                Terminal.IO.Log($"Final Value: {stagedValue}");
            return (int) stagedValue;
        }

        private string TextBreakdownParser(string expression, TextSpecs specs)
        {
            string[] operations = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (operations.Length < 3)
                throw new Exception($"[NumRand:TextBreakdownParser] Syntax Error: The expression supplied was smaller than a single operation -> {expression}");


            if (!IsAnOperator(operations[0]))
                throw new Exception($"[NumRand:TextBreakdownParser] Syntax Error: The first term of the expression must be an operator");


            int i = 1;

            this.opStackS.Push(new StringOp(operations[0]));

            string stagedValue = null;
            while (this.opStackS.Count > 0)
            {
                if (i >= operations.Length)
                    throw new Exception($"[NumRand:TextBreakdownParser] Syntax Error: Incomplete operation -> {expression}");

                if (IsAnOperator(operations[i]))
                {
                    this.opStackS.Push(new StringOp(operations[i]));
                    i++;
                    continue;
                }

                string arg = null;
                if (stagedValue is not null)
                {
                    arg = $"({stagedValue})";
                    stagedValue = null;
                }
                else
                {
                    arg = operations[i];
                }

                arg = specs.termRegistry.ContainsKey(operations[i]) ? specs.termRegistry[operations[i]] : arg;

                if (!this.CurrentOpS.AnyArgumentsSet)
                {
                    this.CurrentOpS.Arg0 = arg;
                    i++;
                    continue;
                }

                if (this.CurrentOpS.FirstArgumentSet)
                {
                    this.CurrentOpS.Arg1 = arg;
                    StringOp op = this.opStackS.Pop();
                    string value = op.TextBreakdown(specs);
                    stagedValue = value;
                    continue;
                }
            }

            return stagedValue;

        }

        private bool IsAnOperator(string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (!this.operatorsSet.Contains(value[i]))
                    return false;
            }
            return true;
        }
    }
}