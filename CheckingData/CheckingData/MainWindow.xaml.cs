using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using OfficeOpenXml;

namespace CheckingData
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int FIOCounter = 1;
        private int POLCounter = 1;
        private int TELCounter = 1;
        private int EMAILCounter = 1;
        private int DULCounter = 1;
        private int SERIACounter = 1;
        public MainWindow()
        {
            string culture = "ru-RU"; 
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture(culture);

            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void B1_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Excel Files (*.xlsx)|*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                T1.Text = filePath;
            }
        }

        private void B2_Click(object sender, RoutedEventArgs e)
        {
            string path = T2.Text;
            if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
            }
            else
            {
                MessageBox.Show("Указанный путь не существует или не был указан.");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Фамилия или Имя или Отчество имеют пустое значение (null)");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пол имеет пустое значение (null) \n\nПол имеет значение отличное от “Мужской” или “Женский” \n\nПол не соответствует ФИО");
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Номер телефона содержит НЕ 10 цифр или содержит символы отличные от 0-9, (, ), - и символа пробела");
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Электронная почта не соответствует формату <address_name>@<domain>.<tld>");
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Две первые цифры в серии имеют значение из списка:00, 02, 06, 13, 16, 21, 23, 31, 35 \n\nДве последние цифры серии не входят в диапазон от 97 до 99 И в диапазон от 00 до ХХ, где ХХ=две последние цифры текущего года +3.");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Содержит иные символы, кроме числовых или длина не равна 6 символов \n\nНомер паспорта меньше 000101");
        }
        private void BB_Click(object sender, RoutedEventArgs e)
        {
            if (T1.Text != null && T1.Text != "")
            { 
                // Удаление всех файлов в папке ExeleData
                string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\ExeleData");
                if (Directory.Exists(folderPath))
                {
                    DirectoryInfo directory = new DirectoryInfo(folderPath);
                    foreach (FileInfo file in directory.GetFiles())
                    {
                        file.Delete();
                    }
                }
                List<ValidationResult> results = new List<ValidationResult>();

                ExcelDataReader excelDataReader = new ExcelDataReader();
                List<Person> people = excelDataReader.ReadDataFromExcel(T1.Text);

                if (!string.IsNullOrWhiteSpace(T1.Text) && File.Exists(T1.Text))
                {
                    int totalCountFIO = people.Count; 
                    int totalCountPOL = people.Count; 
                    int totalCountTEL = people.Count; 
                    int totalCountEMAIL = people.Count; 
                    int totalCountNUMBER = people.Count; 
                    int totalCountSERIA = people.Count;

                    // ---------------------------------------------------Путь к папке ExeleData внутри проекта-----------------------------------------------------------------------

                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                    string filePath;
                    if (Mesto.Text == "Не обязательно")
                    {
                        if (Name.Text != "Не обязательно")
                        {
                            filePath = Path.Combine(folderPath, $"{Name.Text}_CheckingData.xlsx");
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        else
                        {
                            filePath = Path.Combine(folderPath, "ValidationResults.xlsx");
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                    }
                    else
                    {
                        if (Name.Text != "Не обязательно")
                        {
                            filePath = Path.Combine(Mesto.Text, $"{Name.Text}_CheckingData.xlsx");
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        else
                        {
                            filePath = Path.Combine(Mesto.Text, "ValidationResults.xlsx");
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                    }
                    

                    if (C_FIO.IsChecked == true)
                    {
                        int processedRecords = 0;
                        foreach (Person person in people)
                        {
                            if (!string.IsNullOrWhiteSpace(person.FST_NAME) && !string.IsNullOrWhiteSpace(person.LAST_NAME) && !string.IsNullOrWhiteSpace(person.MID_NAME))
                            {
                                processedRecords++;
                            }
                            else
                            {
                                //MessageBox.Show("Фамилия, Имя или Отчество имеют пустое значение (null) для строки " + person.ROW_ID);
                            }
                        }
                        results.Add(new ValidationResult
                        {
                            ValidationCode = $"R_FL_FIO_{FIOCounter:000}",
                            ValidationName = "Заполненность ФИО",
                            RecordCount = totalCountFIO,
                            SuccessRate = (double)processedRecords / totalCountFIO * 100
                        });

                        FIOCounter++;
                    }

                    if (C_POL.IsChecked == true)
                    {
                        int processedRecords = 0;
                        foreach (Person person in people)
                        {
                            if (!string.IsNullOrWhiteSpace(person.SEX_MF) && (person.SEX_MF == "Мужское" || person.SEX_MF == "Женское"))
                            {
                                processedRecords++;
                            }
                            else
                            {
                                //MessageBox.Show("Пол не соответствует заданным критериям для строки " + person.ROW_ID);
                            }
                        }
                        results.Add(new ValidationResult
                        {
                            ValidationCode = $"R_FL_POL_{POLCounter:000}",
                            ValidationName = "Пол соответствует ФИО",
                            RecordCount = totalCountPOL, 
                            SuccessRate = (double)processedRecords / totalCountPOL * 100
                        });

                        POLCounter++;
                    }

                    if (C_TEL.IsChecked == true)
                    {
                        int processedRecords = 0;
                        foreach (Person person in people)
                        {
                            if (IsValidPhoneNumber(person.COMM_ADDR))
                            {
                                processedRecords++;
                            }
                            else
                            {
                                //MessageBox.Show("Номер телефона не соответствует заданным критериям для строки " + person.ROW_ID);
                            }
                        }
                        results.Add(new ValidationResult
                        {
                            ValidationCode = $"R_FL_TEL_{TELCounter:000}",
                            ValidationName = "Формат телефона",
                            RecordCount = totalCountTEL, 
                            SuccessRate = (double)processedRecords / totalCountTEL * 100
                        });

                        TELCounter++;
                    }

                    if (C_EMAIL.IsChecked == true)
                    {
                        int processedRecords = 0;
                        foreach (Person person in people)
                        {
                            if (IsValidEmail(person.COMM_ADDR))
                            {
                                processedRecords++;
                            }
                        }
                        results.Add(new ValidationResult
                        {
                            ValidationCode = $"R_FL_EMAIL_{EMAILCounter:000}",
                            ValidationName = "Формат электронной почты",
                            RecordCount = totalCountEMAIL, 
                            SuccessRate = (double)processedRecords / totalCountEMAIL * 100
                        });

                        EMAILCounter++;
                    }

                    if (C_NUMBER.IsChecked == true)
                    {
                        int processedRecords = 0;
                        foreach (Person person in people)
                        {
                            if (IsValidPassportNumber(person.CRED_NUM))
                            {
                                processedRecords++;
                            }
                        }
                        results.Add(new ValidationResult
                        {
                            ValidationCode = $"R_FL_DUL_{DULCounter:000}",
                            ValidationName = "Проверка номера паспорта",
                            RecordCount = totalCountNUMBER, 
                            SuccessRate = (double)processedRecords / totalCountNUMBER * 100
                        });
                        DULCounter++;
                    }

                    if (C_SERIA.IsChecked == true)
                    {
                        int processedRecords = 0;
                        foreach (Person person in people)
                        {
                            if (IsValidPassportSeries(person.CRED_SR))
                            {
                                processedRecords++;
                            }
                        }
                        results.Add(new ValidationResult
                        {
                            ValidationCode = $"R_FL_DUL_{DULCounter:000}",
                            ValidationName = "Проверка серии паспорта",
                            RecordCount = totalCountSERIA,
                            SuccessRate = (double)processedRecords / totalCountSERIA * 100
                        });

                        DULCounter++;
                    }

                    CreateExcelFile(results, filePath);

                    T2.Text = Path.GetDirectoryName(filePath);
                }
                else
                {
                    MessageBox.Show("Путь к файлу не указан или файл не существует.");
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали файл или не указали проверки");
            }
            
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^[0-9\(\)\-\s]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, pattern);
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("asdsdas");
                return false;
            }

            string pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return System.Text.RegularExpressions.Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private bool IsValidPassportNumber(string passportNumber)
        {
            return !string.IsNullOrEmpty(passportNumber) && passportNumber.Length == 6 && passportNumber.All(char.IsDigit);
        }
        private bool IsValidPassportSeries(string passportSeries)
        {
            string[] allowedFirstDigits = { "00", "02", "06", "13", "16", "21", "23", "31", "35" };
            string currentYearLastDigits = (DateTime.Now.Year % 100).ToString("00");
            int currentYearLastDigitsPlusThree = (int.Parse(currentYearLastDigits) + 3) % 100;

            if (!allowedFirstDigits.Contains(passportSeries.Substring(0, 2)))
            {
                return false;
            }

            int lastTwoDigits = int.Parse(passportSeries.Substring(2));
            if ((lastTwoDigits < 97 || lastTwoDigits > 99) && (lastTwoDigits < 0 || lastTwoDigits > currentYearLastDigitsPlusThree))
            {
                return false;
            }

            return true;
        }

        private void CreateExcelFile(List<ValidationResult> results, string filePath)
        {
            FileInfo newFile = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(newFile))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Validation Results");

                worksheet.Cells["A1"].Value = "Код проверки";
                worksheet.Cells["B1"].Value = "Название проверки";
                worksheet.Cells["C1"].Value = "Количество записей";
                worksheet.Cells["D1"].Value = "Процент прохождения проверки";

                int row = 2;
                foreach (var result in results)
                {
                    int totalCountForValidation = result.RecordCount;
                    worksheet.Cells[row, 1].Value = result.ValidationCode;
                    worksheet.Cells[row, 2].Value = result.ValidationName;
                    worksheet.Cells[row, 3].Value = totalCountForValidation;
                    worksheet.Cells[row, 4].Value = Math.Round(result.SuccessRate, 2) + "%"; 

                    row++;
                }

                worksheet.Cells.AutoFitColumns(0);

                package.Save();
            }

            MessageBox.Show($"Файл Excel успешно создан: {filePath}");
        }


        public class ValidationResult
        {
            public string ValidationCode { get; set; }
            public string ValidationName { get; set; }
            public int RecordCount { get; set; }
            public double SuccessRate { get; set; }
        }
        private string selectedFolderPath = ""; 

        private void Mesto_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var folderDialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = folderDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    Mesto.Text = folderDialog.SelectedPath;
                    selectedFolderPath = folderDialog.SelectedPath;
                }
            }
        }


        private void textBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Mesto.Text == "Не обязательно")
            {
                Mesto.Text = "";
            }
        }

        private void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Mesto.Text))
            {
                Mesto.Text = "Не обязательно";
            }
        }
        private void textBox_GotFocus1(object sender, RoutedEventArgs e)
        {
            if (Name.Text == "Не обязательно")
            {
                Name.Text = "";
            }
        }

        private void textBox_LostFocus1(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name.Text))
            {
                Name.Text = "Не обязательно"; 
            }
        }
    }

    public class Person
    {
        public string ROW_ID { get; set; }
        public string FST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string MID_NAME { get; set; }
        public string SEX_MF { get; set; }
        public string COMM_ADDR { get; set; }
        public string EMAIL { get; set; }
        public string CRED_NUM { get; set; }
        public string CRED_SR { get; set; }
    }

    public class ExcelDataReader
    {
        public List<Person> ReadDataFromExcel(string filePath)
        {
            List<Person> people = new List<Person>();

            FileInfo file = new FileInfo(filePath);
            using (ExcelPackage package = new ExcelPackage(file))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) 
                    {
                        string rowId = worksheet.Cells[row, 1].Text;
                        if (!string.IsNullOrEmpty(rowId))
                        {
                            Person person = new Person
                            {
                                ROW_ID = rowId,
                                FST_NAME = worksheet.Cells[row, 2].Text ?? string.Empty,
                                LAST_NAME = worksheet.Cells[row, 3].Text ?? string.Empty,
                                MID_NAME = worksheet.Cells[row, 4].Text ?? string.Empty,
                                SEX_MF = worksheet.Cells[row, 5].Text ?? string.Empty,
                                COMM_ADDR = worksheet.Cells[row, 6].Text ?? string.Empty,
                                EMAIL = worksheet.Cells[row, 7].Text ?? string.Empty,
                                CRED_NUM = worksheet.Cells[row, 8].Text ?? string.Empty,
                                CRED_SR = worksheet.Cells[row, 9].Text ?? string.Empty,
                            };

                            people.Add(person);
                        }
                        
                    }
                }           
            }

            return people;
        }
    }
    
}
