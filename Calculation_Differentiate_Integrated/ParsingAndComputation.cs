using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculation_Differentiate_Integrated
{
    /*
     * Вызов:
     * ParsingAndComputation call_parse = new ParsingAndComputation();
     * double answer = call_parse.Parsed_formula_calculation(parsing_text);
     * 
     * Добавление или изменение переменных переменный
     * call_parse.AddOrChangeAVariable(string переменная, double значение переменной);
     */
    class ParsingAndComputation
    {
        /*
         * ------------------ Задания для изменений: -------------------
         * - Добавить Экспоненту, число pi и другие константы
         * - Переписать Parsing_text на основе сравнения "число ли":
         *      + через цикл for
         *      + если подряд идут числа "1234567890.", то добавляем их во временную переменную
         * 
         * 
         */
        public double Parsed_formula_calculation(string text)
        {
            List<string> list_of_text = ParsingText(text);
            Console.WriteLine("\nCписок элементов: ");
            foreach (string el in list_of_text)
            {
                Console.Write(el + " | ");
            }
            List<string> formula_parse_RPN = Shunting_yard(list_of_text);
            Console.WriteLine("\nФормула в ОПН: ");
            foreach (string el in formula_parse_RPN)
            {
                Console.Write(el + " ");
            }
            Console.WriteLine("");
            string output = Calculation_of_a_formula(formula_parse_RPN);
            Console.WriteLine($"Ответ: {output}");
            return Convert.ToDouble(output);
        } //Пульт управления

        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// </summary>

        Dictionary<string, double> set_of_variables = new Dictionary<string, double>(); //хранит переменные и их значения
        public void AddOrChangeAVariable(string variable, double value)
        {
            //variable += "_1"; //костыль, через который ParsingTextVersion2 находит переменную корректно
            if (set_of_variables.ContainsKey(variable))
            {
                set_of_variables[variable] = value;
            }
            else
            {
                set_of_variables.Add(variable, value);
            }
        } //добавляет в set_of_variables новую пару ключ-значение
        //protected string ChangeOfVariables(string text)
        //{ // x + 4
        //    // x_1 + 4
        //    // si + 24,3 * sin ( si )
        //    foreach (string key_var in set_of_variables.Keys)
        //    {
        //        text = text.Replace(key_var.Replace("_1", ""), key_var);
        //    }
        //    Console.WriteLine(text);
        //    return text;
        //} // (не работает) заменяет переменные на переменные с "_1", чтобы корректно делилось на элементы в ParsingText
        protected List<string> ParsingText(string text = "0")
        {
            text = text.ToLower().Replace(".", ",").Replace(" ", "");
            //text = ChangeOfVariables(text);
            List<string> parse_text = new List<string> { };
            char[] text_char = text.ToCharArray();
            try
            {
                string collection = text_char[0].ToString();
                // 34 + 48,9 * 2 / (1 - 5)^2  <--->  34 48,9 2 * 1 5 − 2 ^ / +
                // x12 * sin( 0,7 ) + 4
                // "+-*/^ sin cos tan ln exp"
                for (int i = 1; i < text_char.Length; i++)
                {

                    if (IsThisANumber(text_char[i - 1]) && !IsThisANumber(text_char[i]))
                    {
                        parse_text.Add(collection);
                        collection = "";
                    }
                    else if (IsThisAnOperation(collection))
                    {
                        parse_text.Add(collection);
                        collection = "";
                    }
                    collection += text_char[i];
                }
                parse_text.Add(collection);
            }
            catch { parse_text.Add("0"); }
            return parse_text;
        }//Парсит строку и разделяет её на список с числами и операциями
        private bool IsThisANumber(char num)
        {
            return "1234567890,".Contains(num.ToString());
        } //Число ли?
        private bool IsThisAnOperation(string operation)
        {
            string operation_str = operation.ToString(); // || -> или !!!!! && -> и
            return "+-*/^()".Contains(operation_str) || operation_str.Equals("sin") || operation_str.Equals("cos") ||
                operation_str.Equals("tan") || operation_str.Equals("ln") || operation_str.Equals("exp");
        } //Операция ли?

        /// <summary>
        /// ////////////////////////////////////////////////////////////////
        /// </summary>
        protected List<string> Shunting_yard(List<string> parse_formula)
        {

            for (int i = 0; i < parse_formula.Count; i++)
            {
                if (i != 0 && parse_formula[i] == "-" && parse_formula[i - 1] == "(" || i == 0 && parse_formula[i] == "-")
                {
                    parse_formula.Insert(i, "0");
                }

            }

            List<string> output = Maneuver(parse_formula);
            return output;
        } //Убирает унарный минус и отправляет в алгоритм ОПП
        static private List<string> Maneuver(List<string> formula_parse_RPN)
        {
            //  31 + 42,4 * 21.5 / ( 1 - 5 ) ^ 2 ^ 3
            //  31 + 42,4 * 21.5 / ( - 5 ) ^ 2 ^ 3
            // 3 + 4 * 2 / (1 * 5 * (5 - 3) + 2) * 2^2 + 2 - 3   <--->  3 4 2 * 1 5 * 5 * 3 - 2 + 2 ^ / +
            // 3 + 4 * 2 / (1 - 5)^2  <--->  3 4 2 * 1 5 − 2 ^ / +
            // sin(2) ^ sin(1)

            List<string> operator_stack = new List<string> { };
            List<string> output = new List<string> { };
            int num = -1;
            for (int i = 0; i < formula_parse_RPN.Count; i++)
            {
                num++;
                if (formula_parse_RPN[i] == "(")
                {
                    int open_bracket = 1, closed_bracket = 0, ij = 0;
                    List<string> formula_transfer = new List<string> { };
                    for (int j = i + 1; j < formula_parse_RPN.Count; j++)
                    {
                        if (formula_parse_RPN[j] == "(") { open_bracket++; }
                        if (formula_parse_RPN[j] == ")") { closed_bracket++; }
                        if (formula_parse_RPN[j] == ")" && open_bracket == closed_bracket)
                        {
                            ij = j + 1;
                            break;
                        }
                        formula_transfer.Add(formula_parse_RPN[j]);
                    }
                    formula_transfer = Maneuver(formula_transfer);
                    foreach (string token in formula_transfer)
                    {
                        output.Add(token);
                    }
                    num = num == i ? ij : ij - 1;
                    i = ij;
                    if (i == formula_parse_RPN.Count) break;
                }

                if ("+-*/^ sin cos tan ln exp".Contains(formula_parse_RPN[i]))
                {
                    if (operator_stack.Count > 0 && i == num + 1 && ("*/".Contains(operator_stack[operator_stack.Count - 1])) && !"^sin cos tan ln exp".Contains(formula_parse_RPN[i]))
                    {
                        output.Add(operator_stack[operator_stack.Count - 1]);
                        operator_stack.RemoveAt(operator_stack.Count - 1);
                        num++;
                    }
                    if (formula_parse_RPN[i] == "^" || "sin cos tan ln exp".Contains(formula_parse_RPN[i]))
                    {
                        operator_stack.Add(formula_parse_RPN[i]);
                    }
                    else if ("*/".Contains(formula_parse_RPN[i]))
                    {
                        operator_stack.Add(formula_parse_RPN[i]);
                        num--;
                    }
                    else if ("+-".Contains(formula_parse_RPN[i]))
                    {
                        operator_stack.Add(formula_parse_RPN[i]); // && - "и" || - "или" 
                    }
                }
                else
                {
                    output.Add(formula_parse_RPN[i]);
                }
                if (operator_stack.Count > 1 && ("^ sin cos tan ln exp".Contains(operator_stack[operator_stack.Count - 2])) && !"sin cos tan ln exp".Contains(operator_stack[operator_stack.Count - 1]))
                {
                    output.Add(operator_stack[operator_stack.Count - 2]); // operator_stack.Count - 2
                    operator_stack.RemoveAt(operator_stack.Count - 2);
                    if (operator_stack.Count > 1 && ("*/".Contains(operator_stack[operator_stack.Count - 2])))
                    {
                        output.Add(operator_stack[operator_stack.Count - 2]);
                        operator_stack.RemoveAt(operator_stack.Count - 2);
                        num++;
                    }
                }
            }

            operator_stack.Reverse();
            foreach (string i in operator_stack)
            {
                output.Add(i);
            }

            return output;
        }//Преобразует список инфиксной записи в постфиксную через обратную польскую последовательность

        /// <summary>
        /// //////////////////////////////////////////////////////////////\
        /// </summary>
        delegate double Formula_2(double x, double y);
        delegate double Formula_1(double x);
        readonly Dictionary<string, Formula_2> dic_formul_2 = new Dictionary<string, Formula_2>
        {
            ["+"] = (x, y) => x + y,
            ["-"] = (x, y) => x - y,
            ["*"] = (x, y) => x * y,
            ["/"] = (x, y) => x / y,
            ["^"] = (x, y) => Math.Pow(x, y),
        }; //для операций с двумя операндами
        readonly Dictionary<string, Formula_1> dic_formul_1 = new Dictionary<string, Formula_1>
        {
            ["sin"] = (x) => Math.Sin(x),
            ["cos"] = (x) => Math.Cos(x),
            ["tan"] = (x) => Math.Tan(x),
            ["ln"] = (x) => Math.Log(x),
            ["exp"] = (x) => Math.Exp(x)
        }; //для операций с одним операндом

        // dic_for.Keys.Contains(formula_parse_RPN[i]) <- для проверки, что это операнд

        protected string Calculation_of_a_formula(List<string> parse_formula)
        {
            //  3 + 4 * 2 / (1 - 5)^2        -->     3 4 2 * 1 5 − 2 ^ / +
            int i = 0;
            while (parse_formula.Count != 1)
            {
                if (!"+-*/^ sin cos tan ln exp".Contains(parse_formula[i]))
                {
                    i++;
                    continue;
                }
                if ("+-*/^".Contains(parse_formula[i]))
                {
                    foreach (KeyValuePair<string, Formula_2> keyValue in dic_formul_2)
                    {
                        if (parse_formula[i] == keyValue.Key)
                        {
                            if (set_of_variables.ContainsKey(parse_formula[i - 1]))
                            {
                                parse_formula[i - 1] = set_of_variables[parse_formula[i - 1]].ToString();
                            }
                            double y = Double.Parse(parse_formula[i - 1]);
                            parse_formula.RemoveAt(i - 1);
                            if (set_of_variables.ContainsKey(parse_formula[i - 2]))
                            {
                                parse_formula[i - 2] = set_of_variables[parse_formula[i - 2]].ToString();
                            }
                            double x = Convert.ToDouble(parse_formula[i - 2]);
                            parse_formula.RemoveAt(i - 1);
                            i -= 2;
                            parse_formula[i] = keyValue.Value(x, y).ToString();
                        }
                    }
                    i++;
                }
                else if ("sin cos tan ln exp".Contains(parse_formula[i]))
                {
                    foreach (KeyValuePair<string, Formula_1> keyValue in dic_formul_1)
                    {
                        if (parse_formula[i] == keyValue.Key)
                        {
                            if (set_of_variables.ContainsKey(parse_formula[i - 1]))
                            {
                                parse_formula[i - 1] = set_of_variables[parse_formula[i - 1]].ToString();
                            }
                            double x = Double.Parse(parse_formula[i - 1]);

                            parse_formula.RemoveAt(i - 1);
                            i -= 1;
                            parse_formula[i] = keyValue.Value(x).ToString();
                        }
                    }
                    i++;
                }
            }

            return parse_formula[0];
        } // Производит сопоставление операций и вычисление

    }
}
