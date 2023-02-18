using System;
using Newtonsoft.Json;
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

        private readonly char[] operatorsArray = {
            NumRand.OP_RANGE,
            NumRand.OP_ADD,
            NumRand.OP_SUB,
            NumRand.OP_MULT,
            NumRand.OP_INCLUDE,
            NumRand.OP_EXCLUDE
        };

        private readonly HashSet<char> operatorsSet = new() {
            NumRand.OP_RANGE,
            NumRand.OP_ADD,
            NumRand.OP_SUB,
            NumRand.OP_MULT,
            NumRand.OP_INCLUDE,
            NumRand.OP_EXCLUDE
        };

        [JsonProperty] private readonly string expression = string.Empty;
        [JsonProperty] private readonly int value = 0;

        private class Operation
        {
            public Operation(string raw)
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

            public int Calculate()
            {
                if (this.arg0 is null || this.arg1 is null)
                {
                    //INTERNAL ERROR
                }

                return Calculate((float) this.arg0, (float) this.arg1);
            }

            private int Calculate(float x, float y)
            {
                if (this.raw.Length < 1)
                {
                    //INTERNAL ERROR
                }

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
                    int value = UnityEngine.Random.Range((int)x, (int)y);

                    if (IsRangeInclusive(this.raw))
                        value = (int) UnityEngine.Random.Range(x, y);

                    Terminal.IO.Log($"Random Value Generated: {value}");

                    return value;
                }
                else
                {
                    //SYNTAX ERROR
                    throw new Exception();
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
                    //SYNTAX ERROR
                }

                if (full.Length < 2)
                {
                    if (full[0] != NumRand.OP_RANGE)
                    {
                        //SYNTAX ERROR
                    }
                    full += NumRand.OP_EXCLUDE;
                }

                return full[1] == NumRand.OP_INCLUDE;
            }
        };

        private readonly Stack<Operation> opStack = new();
        private Operation currentOperation { get { return this.opStack.Peek(); } }

        public int Calculate()
        {
            if (string.IsNullOrEmpty(this.expression))
                return this.value;

            return Parser(this.expression) + this.value;
        }

        private int Parser(string expression)
        {
            string[] operations = expression.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (operations.Length < 3)
            {
                //SYNTAX ERROR
            }

            int i = 1;

            if (!IsAnOperator(operations[0]))
            {
                //SYNTAX ERROR
            }

            this.opStack.Push(new Operation(operations[0]));

            float? stagedValue = null;
            while (this.opStack.Count > 0)
            {
                if (i < operations.Length && IsAnOperator(operations[i]))
                {
                    this.opStack.Push(new Operation(operations[i]));
                    i++;
                    continue;
                }

                float arg = 0f;
                if (stagedValue is not null)
                {
                    arg = (float) stagedValue;
                    stagedValue = null;
                }
                else
                {
                    try
                    {
                        arg = float.Parse(operations[i]);
                    }
                    catch
                    {
                        //SYNTAX ERROR
                    }
                }

                if (!this.currentOperation.AnyArgumentsSet)
                {
                    this.currentOperation.Arg0 = arg;
                    i++;
                    continue;
                }

                if (this.currentOperation.FirstArgumentSet)
                {

                    this.currentOperation.Arg1 = arg;
                    Operation op = this.opStack.Pop();
                    int value = op.Calculate();
                    stagedValue = value;
                    continue;
                }
            }

            return (int) stagedValue;
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