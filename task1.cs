using System;
using System.Diagnostics;
using System.Windows.Forms;

class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        // Завдання 3 - Дочірній процес
        if (args.Length == 3)
        {
            double a = double.Parse(args[0]);
            double b = double.Parse(args[1]);
            string op = args[2];
            double result = 0;

            switch (op)
            {
                case "+": result = a + b; break;
                case "-": result = a - b; break;
                case "*": result = a * b; break;
                case "/": result = b != 0 ? a / b : 0; break;
            }

            Console.WriteLine($"Аргументи: {a} {op} {b}");
            Console.WriteLine($"Результат: {result}");
            Console.ReadKey();
            return;
        }

        Application.Run(new Form1());
    }
}

public class Form1 : Form
{
    Button btnTask1 = new Button() { Text = "Завдання 1", Left = 10, Top = 10, Width = 150 };
    Button btnTask2Wait = new Button() { Text = "Завдання 2 - Чекати", Left = 10, Top = 50, Width = 150 };
    Button btnTask2Kill = new Button() { Text = "Завдання 2 - Завершити", Left = 10, Top = 90, Width = 150 };
    Button btnTask3 = new Button() { Text = "Завдання 3", Left = 10, Top = 130, Width = 150 };
    TextBox txtNum1 = new TextBox() { Left = 170, Top = 130, Width = 60, Text = "7" };
    TextBox txtNum2 = new TextBox() { Left = 240, Top = 130, Width = 60, Text = "3" };
    ComboBox cmbOp = new ComboBox() { Left = 310, Top = 130, Width = 60, DropDownStyle = ComboBoxStyle.DropDownList };
    Label lblResult = new Label() { Left = 10, Top = 200, Width = 400, AutoSize = false, Height = 60 };

    Process childProcess;

    public Form1()
    {
        this.Text = "Батьківський процес";
        this.Width = 450;
        this.Height = 320;

        cmbOp.Items.AddRange(new string[] { "+", "-", "*", "/" });
        cmbOp.SelectedIndex = 0;

        this.Controls.Add(btnTask1);
        this.Controls.Add(btnTask2Wait);
        this.Controls.Add(btnTask2Kill);
        this.Controls.Add(btnTask3);
        this.Controls.Add(txtNum1);
        this.Controls.Add(txtNum2);
        this.Controls.Add(cmbOp);
        this.Controls.Add(lblResult);

        btnTask1.Click += (s, e) =>
        {
            Process proc = new Process();
            proc.StartInfo.FileName = "notepad.exe";
            proc.Start();

            Console.WriteLine("Процес розпочато: " + proc.ProcessName);

            proc.WaitForExit();

            Console.WriteLine("Процес завершився з кодом: " + proc.ExitCode);
            lblResult.Text = $"Завдання 1\nПроцес завершився з кодом: {proc.ExitCode}";
        };

        btnTask2Wait.Click += (s, e) =>
        {
            childProcess = new Process();
            childProcess.StartInfo.FileName = "notepad.exe";
            childProcess.Start();

            lblResult.Text = "Завдання 2\nЧекаємо завершення процесу...";

            childProcess.WaitForExit();

            lblResult.Text = $"Завдання 2\nПроцес завершився з кодом: {childProcess.ExitCode}";
        };

        btnTask2Kill.Click += (s, e) =>
        {
            if (childProcess == null || childProcess.HasExited)
            {
                childProcess = new Process();
                childProcess.StartInfo.FileName = "notepad.exe";
                childProcess.Start();
                lblResult.Text = "Завдання 2\nПроцес запущено.";
                return;
            }

            childProcess.CloseMainWindow();

            if (!childProcess.WaitForExit(2000))
            {
                childProcess.Kill();
            }

            childProcess.Dispose();
            lblResult.Text = "Завдання 2\nПроцес примусово завершено.";
        };

        btnTask3.Click += (s, e) =>
        {
            string num1 = txtNum1.Text;
            string num2 = txtNum2.Text;
            string op = cmbOp.SelectedItem.ToString();

            Process proc = new Process();
            proc.StartInfo.FileName = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            proc.StartInfo.Arguments = $"{num1} {num2} {op}";
            proc.StartInfo.UseShellExecute = true;
            proc.Start();
        };
    }
}
