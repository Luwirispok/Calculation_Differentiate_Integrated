using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Calculation_Differentiate_Integrated
{
    public partial class Form1 : Form
    {
        public Form1()
        { //31 36 41 46 51
            InitializeComponent();
            Width = 359 / 2;
            Height = 635 / 2;
            textBox_entered_formula.Focus();
            textBox_entered_formula.Select();
            tabControl1.SelectedIndexChanged += new EventHandler(TabControl1_SelectedIndexChanged);
        }
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                Width = 359 / 2;
                Height = 635 / 2;
                textBox_entered_formula.Focus();
                textBox_entered_formula.Select();
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                Width = (int)(699 / 1.3);
                Height = (int)(658 / 1.25);
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                Width = (int)(658 / 1.3);
                Height = (int)(658 / 1.25);
            }

        }
        ////////////////// Ввод чисел и операндов в textBox_entered_formula \\\\\\\\\\\\\\\\\
        private void button_nums_and_operands_Click(object sender, EventArgs e) //ввод чисел и операндов
        {
            Button button = (Button)sender; //присваивание переменной текущей кнопке
            OutputInTextBoxEnteredFormula(button.Text, false);
        }

        private void button_operands_with_brackets_Click(object sender, EventArgs e) //ввод операндов со скобками
        {
            Button button = (Button)sender; //присваивание переменной текущей кнопке
            OutputInTextBoxEnteredFormula($"{button.Text}()", true);
        }

        private void button_square_root_Click(object sender, EventArgs e) //ввод корня
        {
            OutputInTextBoxEnteredFormula("()^(0,5)", false);
        }

        private void button_power_Click(object sender, EventArgs e) //ввод степени
        {
            OutputInTextBoxEnteredFormula("^", false);
        }

        bool activate = true;
        private void button_erygonometry_Click(object sender, EventArgs e) //открытие панельки с тригонометрией
        {
            panel_erygonometry.Location = new Point(3, 250); //переместить панель
            if (activate)
            {
                button_erygonometry.BackColor = Color.Gray;
                panel_erygonometry.Show(); //показать
                panel_erygonometry.Enabled = true; //активировать
                activate = false;
            }
            else
            {
                button_erygonometry.BackColor = Color.Silver;
                panel_erygonometry.Hide(); //скрыть
                panel_erygonometry.Enabled = false;
                activate = true;
            }
        }

        int current_cursor_position = 0;
        private void OutputInTextBoxEnteredFormula(string element, bool is_it_trigonometry)
        {
            current_cursor_position = textBox_entered_formula.SelectionStart; //текущая позиция курсора сохраняется в переменную
            if (textBox_entered_formula.TextLength <= 45)
            {  //вставка текста с нужной позиции
                textBox_entered_formula.Text = textBox_entered_formula.Text.Insert(current_cursor_position, element);
                if (is_it_trigonometry)
                {
                    current_cursor_position += element.Length - 2;
                }
                if (element == "^(2)") { current_cursor_position++; }
                current_cursor_position++;
            }
            textBox_entered_formula.SelectionStart = current_cursor_position;

            textBox_entered_formula.SelectionLength = 0; //длина выделенного текста
            textBox_entered_formula.Focus(); //сфокусировать курсор в нужном textbox
        } //вставляет число или операнд в нужное место

        private void textBox_entered_formula_TextChanged(object sender, EventArgs e)
        { //срабатывает при каждом изменении textbox
            float text_size = textBox_entered_formula.Font.Size;
            int text_length = textBox_entered_formula.TextLength;
            if (text_length <= 11)
            {
                text_size = 20;
            }
            else if (text_length <= 15 && text_length > 10)
            {
                text_size = 15;
            }
            else if (text_length > 16 && text_length <= 25)
            {
                text_size = 10;
            }

            textBox_entered_formula.Font = new Font("Microsoft Sans Serif", text_size, FontStyle.Bold);
        } //уменьшает размер шрифта в зависимости от длины
        ///////////////////////////////___________________________\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

        ////////////////// Очистка полей \\\\\\\\\\\\\\\\\
        private void button_C_Click(object sender, EventArgs e) //очищает все textbox
        {
            textBox_entered_formula.Clear();
            textBox_answer.Clear();
            textBox_equation.Clear();
        }
        private void button_CE_Click(object sender, EventArgs e) //очищает только поле ввода
        {
            textBox_entered_formula.Clear();
        }

        private void button_erase_Click(object sender, EventArgs e)
        {
            try { textBox_entered_formula.Text = textBox_entered_formula.Text.Remove(textBox_entered_formula.TextLength - 1); }
            catch { }
        }
        ///////////////////////////////___________________________\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        bool activate_history = true;
        private void button_history_Click(object sender, EventArgs e) //открытие вкладки с историей
        {
            if (activate_history)
            {
                Width = (int)(615 / 1.3);
                Height = 547 / 2;
                button_history.BackColor = Color.CadetBlue;
                activate_history = false;
                panel2.Visible = true;
            }
            else
            {
                Width = 332 / 2;
                Height = 602 / 2;
                button_history.BackColor = Color.LightCyan;
                activate_history = true;
                panel2.Visible = false;
            }
        }

        private void button_history_formula_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string[] formula_and_answer = button.Text.Replace(" ", "").Split('=');
            try
            {
                textBox_equation.Text = formula_and_answer[0];
                textBox_answer.Text = formula_and_answer[1];
            }
            catch { }
        } //кнопки истории, которые возвращают вычисления из истории
        List<string> list_history = new List<string> { "_", "_", "_", "_", "_", "_", "_", "_" };
        private void button_execute_Click(object sender, EventArgs e)
        {
            textBox_entered_formula.Focus();
            Button[] arr_history_button = { button_history_1, button_history_2, button_history_3, button_history_4, button_history_5, button_history_6, button_history_7, button_history_8 };
            ParsingAndComputation call_parse = new ParsingAndComputation();
            string calculated_formula = textBox_entered_formula.Text;
            double computed_answer = call_parse.Parsed_formula_calculation(calculated_formula);
            textBox_equation.Text = calculated_formula;
            textBox_answer.Text = computed_answer.ToString();

            list_history.Insert(0, $"{calculated_formula} = {computed_answer}");
            list_history.RemoveAt(list_history.Count - 1);
            for (int i = 0; i < list_history.Count; i++)
            {
                arr_history_button[i].Text = list_history[i].ToString();
            }
        } //кнопка выполнить и все, все, все


        /// <summary>
        /// Интегрирование
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_clear_integration_Click(object sender, EventArgs e)
        {
            textBox_a.Clear();
            textBox_b.Clear();
            textBox_h.Clear();
            textBox_f.Clear();
            textBox_f2.Clear();
        }

        private void Button_computation_integration_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            ParsingAndComputation call_parse = new ParsingAndComputation(); //класс парсера
            try
            {
                double a = Convert.ToDouble(textBox_a.Text);
                double b = Convert.ToDouble(textBox_b.Text);
                double h = Convert.ToDouble(textBox_h.Text);
                string f = textBox_f.Text;
                string f2 = textBox_f2.Text;
                double alpha = a + h / 2;
                double n = (b - a) / h;

                double result_f = 0;
                double max_f2 = 0;

                for (double k = 0; k < n; k++)
                {
                    call_parse.AddOrChangeAVariable("x1", alpha + k * h); //добавление переменной
                    double func = call_parse.Parsed_formula_calculation(f); //вычисление
                    result_f += func;

                    double func2 = Math.Abs(call_parse.Parsed_formula_calculation(f2));
                    max_f2 = Math.Max(max_f2, func2);

                    dataGridView1.Rows.Add("", "", k, alpha + k * h, Math.Round(func, 4), Math.Round(func2, 4));
                    if (Math.Abs(func) < 0.1 * Math.Pow(10, 1 - k)) break;
                }
                dataGridView1.Rows[0].Cells[0].Value = n;
                dataGridView1.Rows[0].Cells[1].Value = alpha;
                double error = Math.Pow(b - a, 3) * Math.Abs(max_f2) / (24 * Math.Pow(n, 2));
                textBox_error_value.Text = Math.Round(error, 8).ToString();
                result_f *= h;
                textBox_integral_value.Text = Math.Round(result_f, 8).ToString();
            }
            catch { }
        }

        private void comboBox_function_examples_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedFunction = comboBox_function_examples.SelectedIndex;
            switch (selectedFunction)
            {
                //8 * exp(-x1) * sin(2 * x1)
                //8 * sin(2 * x1) - x1
                //sin(exp(x1)) - exp(-x1) + 1
                //4 * sin(x1) - x1 ^ (0.5)
                //x1* sin(x1) +cos(x1) + 5
                case 0:
                    FunExample(1.2, 3.2, 0.5, "8 * exp(-x1) * sin(2 * x1)", "24 * exp(-x1) * sin(2 * x1) + 32 * exp(-x1) * cos(2 * x1)");
                    return;
                case 1:
                    FunExample(0.2, 1.2, 0.25, "8 * sin(2 * x1) - x1", "-32 * sin(2 * x1)");
                    return;
                case 2:
                    FunExample(0, 1, 0.25, "sin(exp(x1)) - exp(-x1) + 1", "-sin(exp(x1)) * exp(2 * x1) + cos(exp(x1)) * exp(x1) - exp(x1)");
                    return;
                case 3:
                    FunExample(1, 2, 0.25, "4 * sin(x1) - x1 ^ (0,5)", "-4 * sin(x1) + 1 / (4 * x1 * (x1 ^ 0.5) )");
                    return;
                case 4:
                    FunExample(0, 2, 0.5, "x1 * sin(x1) + cos(x1) + 5", "cos(x1) - x1 * sin(x1)");
                    return;
            }
        }
        private void FunExample(double a, double b, double h, string f, string f2)
        {
            textBox_a.Text = a.ToString();
            textBox_b.Text = b.ToString();
            textBox_h.Text = h.ToString();
            textBox_f.Text = f;
            textBox_f2.Text = f2;
        }

        /// <summary>
        /// Диффиринцирование
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void comboBox_dif_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedFunction = comboBox_dif.SelectedIndex;
            switch (selectedFunction)
            {
                case 0:
                    FunExample_dif(0.04, 0, 1, 1, "3 * sin(x1) * y1");
                    return;
                case 1:
                    FunExample_dif(0.04, 1, 1, 2, "y1 ^ 2 / 4");
                    return;
                case 2:
                    FunExample_dif(0.04, 0, 1, 1, "y1 * (x1 - 1) / 2");
                    return;
                case 3:
                    FunExample_dif(0.04, 0, 1, 1, "(4 - y1 ^ 2) ^ (0.5) * x1");
                    return;
                case 4:
                    FunExample_dif(0.04, 0, 1, 1, "y1* exp(x1)");
                    return;
            } 

        }
        private void FunExample_dif(double h, double x, double y, double b, string func)
        {
            textBox_h_dif.Text = h.ToString();
            textBox_x_dif.Text = x.ToString();
            textBox_y_dif.Text = y.ToString();
            textBox_b_dif.Text = b.ToString();
            textBox_dif.Text = func;
        }
        public double Diffirentiation(double x, double y, double h, string func)
        {
            ParsingAndComputation call_parse = new ParsingAndComputation();
            call_parse.AddOrChangeAVariable("x1", x);
            call_parse.AddOrChangeAVariable("y1", y);
            double func_cul = call_parse.Parsed_formula_calculation(func);
            return (h * func_cul);
        }

        private void button_calculating_differentiation_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridView2.Rows.Clear();
                double h = double.Parse(textBox_h_dif.Text);
                double x = double.Parse(textBox_x_dif.Text);
                double y = double.Parse(textBox_y_dif.Text);
                double b = double.Parse(textBox_b_dif.Text);
                string func = textBox_dif.Text;
                dataGridView2.Rows.Add(x, "", "", "", "", y);
                chart1.Series[0].Points.Clear();
                List<double> x_arr = new List<double>();
                List<double> y_arr = new List<double>();

                for (double i = x; i < b + h; i += h)
                {
                    i = Math.Round(i, 3);
                    x_arr.Add(i);
                    y_arr.Add(Math.Round(y, 3));
                    double k0 = Diffirentiation(i, y, h, func);
                    double k1 = Diffirentiation(i + (h / 2), y + (k0 / 2), h, func);
                    double k2 = Diffirentiation(i + (h / 2), y + (k1 / 2), h, func);
                    double k3 = Diffirentiation(i + h, y + k2, h, func);
                    y += (k0 + 2.0 * k1 + 2.0 * k2 + k3) / 6.0;
                    dataGridView2.Rows.Add(i, Math.Round(k0, 5), Math.Round(k1, 5), Math.Round(k2, 5), Math.Round(k3, 5), Math.Round(y, 5));
                }
                textBox_answer_dif.Text = y.ToString();
                for (int i = 0; i < x_arr.Count; i++)
                {
                    chart1.Series["F(x,y)"].Points.AddXY(x_arr[i], y_arr[i]);
                }
            }
            catch { }
        }

        private void button_differentiate_clear_Click(object sender, EventArgs e)
        {
            textBox_h_dif.Clear();
            textBox_x_dif.Clear();
            textBox_y_dif.Clear();
            textBox_b_dif.Clear();
            textBox_dif.Clear();
        }
        bool activate_graphics = true;
        private void button_graphics_Click(object sender, EventArgs e)
        {
            if (activate_graphics)
            {
                Width = (int)(1380 / 1.3);
                Height = (int)(658 / 1.25);
                button_graphics.BackColor = Color.Gray;
                activate_graphics = false;
                chart1.Visible = true;
            }
            else
            {
                Width = (int)(699 / 1.3);
                Height = (int)(658 / 1.25);
                button_graphics.BackColor = Color.WhiteSmoke;
                activate_graphics = true;
                chart1.Visible = false;
            }
        }
    }

}
