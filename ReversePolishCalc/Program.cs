using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversePolishCalc
{
    class Program
    {
        public static void Main(string[] args)
        {
            String expression = null;
            while (true)
            {
                Console.WriteLine("Введите выражение. Разрешается использовать: '(',')','+','-','*','/'.");
                Console.WriteLine("Либо введите 'end' для завершения.");
                expression = Console.ReadLine();
                if (expression.Equals("end")) break;
                RevertPolishCalculator rpa = new RevertPolishCalculator(expression);
                Console.WriteLine("Ответ: {0} \r\n", rpa.GetResult());
                if (double.IsNaN(rpa.GetResult()))
                {
                    Console.WriteLine("Проверьте правильность введенного выражения и попробуйте еще раз.");
                }

            }


        }
    }

    public class RevertPolishCalculator
    {

        //лист операторов
        private List<string> operators = new List<string>(new string[] { "(", ")", "+", "-", "*", "/" });
        //лист преобразованного входного выражения в обратную польскую запись
        private List<string> polishList = new List<string>();
        //стек операторов
        private Stack<string> stackOperations = new Stack<string>();
        //для хранения разбитой по операторам строки
        List<string> stock;

        public RevertPolishCalculator(string input)
        {
            String innerInput = input;
            //добавляем к оператору символ | для удобства разбиения. p.s. возможно стоит придумать способ получше =)
            innerInput = innerInput.Replace("+", "|+|").Replace("-", "|-|").Replace("*", "|*|").Replace("/", "|/|").Replace("(", "|(|").Replace(")", "|)|");
            //разбиваем по | и добавляем в лист
            stock = innerInput.Split('|').ToList<string>();
            TransformToPolish();
        }

        //приоритет оператора
        private byte GetPriority(string operation)
        {
            switch (operation)
            {
                case "(":
                case ")":
                    return 0;
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                default:
                    return 3;
            }
        }
        //функция преобразовывает входное выражение в обратную польскую запись
        private void TransformToPolish()
        {
            //удалим пустые значения, которые могли появится в результате разбиения
            for (int i = 0; i < stock.Count(); i++)
            {
                if (stock[i] == "")
                {
                    stock.RemoveAt(i);
                }
            }
            //обработка значений и формирование массива выхода, который соотвествует обратной польской нотации
            for (int i = 0; i < stock.Count(); i++)
            {
                //если это оператор
                if (operators.Contains(stock[i]))
                {
                    //logic for operatons
                    //если встретили закрывающую скобку то из стека достаем все до открывающей скобки, и перекладываем в лист. После скобки удаляем.
                    if (stock[i] == ")")
                    {
                        while (stackOperations.Peek() != "(")
                        {
                            string lastInStack = stackOperations.Pop();
                            InsertInList(lastInStack.ToString());
                        }
                        string openBrace = stackOperations.Pop();
                        continue;
                    }
                    //стек не пустой
                    if (stackOperations.Count() != 0)
                    {
                        //приоритет текущего оператора
                        byte curentPriority = GetPriority(stock[i]);
                        //приоритет последнего в стеке
                        byte priorityLastItem = GetPriority(stackOperations.Peek());
                        //если текущий приоритет меньше или равен последнему в стеке и если он не равен нулю, то добавляем в стек текущий,а последний добавляем в лист                        
                        if (curentPriority <= priorityLastItem && curentPriority != 0)
                        {
                            string lastInStack = stackOperations.Pop();
                            InsertInList(lastInStack.ToString());
                            PushInStack(stock[i]);
                        }
                        // иначе добавляем в стек
                        else
                        {
                            PushInStack(stock[i]);
                        }
                    }
                    //стек пустой
                    else
                    {
                        PushInStack(stock[i]);
                    }
                }
                //иначе это операнд
                else
                {
                    // logic for operand
                    InsertInList(stock[i].ToString());

                }
            }
            //если стек не пустой,то перебарсываем его элементы в лист
            if (stackOperations.Count() != 0)
            {
                foreach (string s in stackOperations)
                {
                    InsertInList(s);

                }
                stackOperations.Clear();
            }
        }
        //подсчет сформированного массива обратной польской нотации
        public double GetResult()
        {
            double result = 0;
            //стек в который будем последовательно добавлять результаты вычислений
            Stack<string> stackResult = new Stack<string>();
            try
            {
                for (int i = 0; i < polishList.Count(); i++)
                {

                    if (operators.Contains(polishList[i]))
                    {
                        double last = Convert.ToDouble(stackResult.Pop());
                        double prelast = Convert.ToDouble(stackResult.Pop());
                        double resultOperation = DoOperation(polishList[i], prelast, last);
                        stackResult.Push(resultOperation.ToString());
                        continue;
                    }
                    stackResult.Push(polishList[i]);

                }
            }
            catch (Exception e)
            {
                return double.NaN;
            }
            try
            {
                return result = Convert.ToDouble(stackResult.Pop());

            }
            catch (System.FormatException e)
            {
                return double.NaN;
            }
        }
        //возвращает результат операции
        public double DoOperation(string logic, double x, double y)
        {
            switch (logic)
            {
                case "*": return x * y;
                case "/": return x / y;
                case "+": return x + y;
                case "-": return x - y;
                default: throw new Exception("invalid logic");
            }
        }
        //добаляет в лист польской записи
        private void InsertInList(String str)
        {
            polishList.Add(str);
        }
        //добавляет в стек оперторов
        private void PushInStack(string str)
        {
            stackOperations.Push(str);
        }
    }
}
