using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Reflection//Вариант 14
{/*№ 12 Рефлексия*/
    class Program
    {/*Задание
1. Для изучения .NET Reflection API допишите класс Рефлектор, который
будет содержать методы выполняющие следующие действия:
a. выводит всё содержимое класса в текстовый файл (принимает
в качестве параметра имя класса);
b. извлекает все общедоступные публичные методы класса
(принимает в качестве параметра имя класса);
c. получает информацию о полях и свойствах класса;
d. получает все реализованные классом интерфейсы;
e. выводит по имени класса имена методов, которые содержат
заданный (пользователем) тип параметра (имя класса
передается в качестве аргумента);
f. вызывает некоторый метод класса, при этом значения для его
параметров необходимо прочитать из текстового файла (имя
класса и имя метода передаются в качестве аргументов).
Продемонстрируйте работу «Рефлектора» для исследования типов на
созданных вами (предыдущие лабораторные работы) и классах .Net.*/
        static async Task Main(string[] args)
        {
            var reflector = new Reflector("Reflection.Crocodile");

            Console.WriteLine("a. Вывод всего содержимого класса в текстовый файл... ; \n");
            await reflector.PrintContentToFileAsync("Reflection.Crocodile");
            Console.WriteLine(new string('-',60));

            Console.WriteLine("b. Извлечение всех общедоступных публичных методов класса; \n");
            reflector.ExtractAllPublicMethods("Reflection.Crocodile");
            Console.WriteLine(new string('-', 60));

            Console.WriteLine("c. Получение информации о полях и свойствах класса; \n");
            reflector.GetInfoFieldsAndProperties();
            Console.WriteLine(new string('-', 60));

            Console.WriteLine("d. Получение всех реализованных классом интерфейсов; \n");
            reflector.GetAllInterfaces();
            Console.WriteLine(new string('-', 60));

            string Par = "public";
            Console.WriteLine($"e. Вывод по имени класса имён методов, которые содержат тип параметра '{Par}'; \n");
            reflector.DisplayNamesOfMethods("Reflection.Crocodile", Par);
            Console.WriteLine(new string('-', 60));

            string Met = "ToString();";           
            Console.WriteLine($"f. Чтение из текстового файла значения для параметров метода класса: '{Met}'... \nВызов метода класса '{Met}': \n");
            reflector.CallClassMethodAsync("Reflection.Crocodile", Met);
           

            Console.ReadKey();
        }
    }
    #region Класс Рефлектор
    public class Reflector
    {
        private static string FilePath { get; set; }
        private string ClassName { get; set; }
        private string MethodName { get; set; }
        private string ParamType { get; set; }
        public Reflector()
        {
        }
        public Reflector(string ClassName)
        {
            this.ClassName = ClassName;
        }

        //a. выводит всё содержимое класса в текстовый файл (принимает в качестве параметра имя класса);
        #region Метод PrintContentToFileAsync
        /// <summary>
        /// Выводит всё содержимое класса в текстовый файл (принимает в качестве параметра имя класса);      
        /// </summary>           
        #endregion
        public async Task PrintContentToFileAsync(string ClassName)
        {
            this.ClassName = ClassName;       
            FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            FilePath = Path.Combine(FilePath, "Всё содержимое класса");
            DirectoryInfo dirInf = new DirectoryInfo(FilePath);
            if (!dirInf.Exists)
            {
                dirInf.Create();
            }

            await Task.Run(async () =>
            {
                string Path = FilePath + @"\Всё содержимое класса.txt";
                try
                {
                    using (StreamWriter sw = new StreamWriter(Path, true, System.Text.Encoding.Default))
                    {
                        sw.WriteLine("Методы:");
                        Type type = Type.GetType(this.ClassName, true, true);
                        foreach (System.Reflection.MethodInfo method in type.GetMethods())
                        {
                            string modificator = "";
                            if (method.IsConstructor)
                                modificator += "constructor";
                            if (method.IsConstructedGenericMethod)
                                modificator += "constructedGenericMethod";
                            if (method.IsPrivate)
                                modificator += "private";
                            if (method.IsStatic)
                                modificator += "static";
                            if (method.IsPublic)
                                modificator += "public";
                            if (method.IsAbstract)
                                modificator += "abstract";
                            if (method.IsAssembly)
                                modificator += "assembly";
                            if (method.IsCollectible)
                                modificator += "collectible";
                            if (method.IsGenericMethod)
                                modificator += "genericMethod";
                            if (method.IsGenericMethodDefinition)
                                modificator += "genericMethodDefinition";
                            if (method.IsFamily)
                                modificator += "family";
                            if (method.IsFamilyAndAssembly)
                                modificator += "familyAndAssembly";
                            if (method.IsFamilyOrAssembly)
                                modificator += "familyOrAssembly";
                            if (method.IsSecuritySafeCritical)
                                modificator += "securitySafeCritical";
                            if (method.IsSecurityTransparent)
                                modificator += "securityTransparent";

                            if (method.IsSpecialName)
                            {
                                foreach (System.Reflection.PropertyInfo prop in type.GetProperties())
                                {
                                    modificator += $" {prop.Name} ";
                                }
                            }                         

                            await sw.WriteAsync($"{modificator} {method.ReturnType.Name} {method.Name}(");
                            //получаем все параметры
                            System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                            for (int i = 0; i < parameters.Length; i++)
                            {                               
                                await sw.WriteAsync($"{parameters[i].ParameterType.Name} {parameters[i].Name}");                              
                                if (i + 1 < parameters.Length) await sw.WriteLineAsync(", ");
                            }
                            await sw.WriteLineAsync(");");                                                 
                        }
                        Console.WriteLine("Запись прошла успешно!");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }    
                
            });
        }

        //b. извлекает все общедоступные публичные методы класса (принимает в качестве параметра имя класса);
        #region Метод ExtractAllPublicMethods
        /// <summary>
        /// Извлекает все общедоступные публичные методы класса (принимает в качестве параметра имя класса);    
        /// </summary>           
        #endregion
        public void ExtractAllPublicMethods(string ClassName)
        {
            this.ClassName = ClassName;

            Type type = Type.GetType(this.ClassName, true, true);
            Console.WriteLine("Методы:");
            foreach (System.Reflection.MethodInfo method in type.GetMethods())
            {
                string modificator = "";
                if (method.IsPublic)
                    modificator += "public";
                if (method.IsCollectible)
                    modificator += "collectible";
                if (method.IsGenericMethod)
                    modificator += "genericMethod";
                if (method.IsGenericMethodDefinition)
                    modificator += "genericMethodDefinition";

                if (!method.Name.Contains("get") && !method.Name.Contains("set"))
                {
                    Console.Write($"{modificator} {method.ReturnType.Name} {method.Name}(");

                    //получаем все параметры
                    System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                        if (i + 1 < parameters.Length) Console.Write(", ");
                    }
                    Console.WriteLine(");");
                }

            }
            Console.WriteLine();
        }

        //c. получает информацию о полях и свойствах класса;
        #region Метод GetInfoFieldsAndProperties
        /// <summary>
        /// Получает информацию о полях и свойствах класса;    
        /// </summary>           
        #endregion
        public void GetInfoFieldsAndProperties()
        {
            Type type = Type.GetType(this.ClassName, true, true);

            Console.WriteLine("Поля:");
            foreach (FieldInfo field in type.GetFields())
            {
                Console.WriteLine($"{field.FieldType} {field.Name};");
            }
            Console.WriteLine();

            Console.WriteLine("Свойства:");
            foreach (PropertyInfo prop in type.GetProperties())
            {
                Console.WriteLine($"{prop.PropertyType} {prop.Name} {(char)123} get; set; {(char)125}");
            }
            Console.WriteLine();
        }

        //d. получает все реализованные классом интерфейсы;
        #region Метод GetAllInterfaces
        /// <summary>
        /// Получает все реализованные классом интерфейсы;
        /// </summary>           
        #endregion
        public void GetAllInterfaces()
        {
            Type type = Type.GetType(this.ClassName, true, true);

            Console.WriteLine("Реализованные интерфейсы:");
            foreach (Type i in type.GetInterfaces())
            {
                Console.WriteLine(i.Name);
            }
            Console.WriteLine();
        }

        //e. выводит по имени класса имена методов, которые содержат заданный(пользователем) тип параметра(имя класса передается в качестве аргумента);
        #region Метод DisplayNamesOfMethods
        /// <summary>
        /// Выводит по имени класса имена методов, которые содержат заданный(пользователем) тип параметра(имя класса передается в качестве аргумента);  
        /// </summary>           
        #endregion
        public void DisplayNamesOfMethods(string ClassName, string ParamType)
        {
            this.ClassName = ClassName;
            this.ParamType = ParamType;

            Type type = Type.GetType(this.ClassName, true, true);
            foreach (System.Reflection.MethodInfo method in type.GetMethods())
            {
                string modificator = "";
                if (method.IsConstructor)
                    modificator += "constructor";
                if (method.IsConstructedGenericMethod)
                    modificator += "constructedGenericMethod";
                if (method.IsPrivate)
                    modificator += "private";
                if (method.IsStatic)
                    modificator += "static";
                if (method.IsPublic)
                    modificator += "public";
                if (method.IsAbstract)
                    modificator += "abstract";
                if (method.IsAssembly)
                    modificator += "assembly";
                if (method.IsCollectible)
                    modificator += "collectible";
                if (method.IsGenericMethod)
                    modificator += "genericMethod";
                if (method.IsGenericMethodDefinition)
                    modificator += "genericMethodDefinition";
                if (method.IsFamily)
                    modificator += "family";
                if (method.IsFamilyAndAssembly)
                    modificator += "familyAndAssembly";
                if (method.IsFamilyOrAssembly)
                    modificator += "familyOrAssembly";
                if (method.IsSecuritySafeCritical)
                    modificator += "securitySafeCritical";
                if (method.IsSecurityTransparent)
                    modificator += "securityTransparent";              

                if (modificator == this.ParamType)
                {
                    if (!method.Name.Contains("get") && !method.Name.Contains("set"))
                    {
                        Console.Write($"{modificator} {method.ReturnType.Name} {method.Name}(");
                        //получаем все параметры
                        System.Reflection.ParameterInfo[] parameters = method.GetParameters();
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            Console.Write($"{parameters[i].ParameterType.Name} {parameters[i].Name}");
                            if (i + 1 < parameters.Length) Console.Write(", ");
                        }
                        Console.WriteLine(");");
                    }                       
                }                
            }
            Console.WriteLine();            
        }

        //f. вызывает некоторый метод класса, при этом значения для его параметров необходимо прочитать из текстового файла
        //(имя класса и имя метода передаются в качестве аргументов) 
        #region Метод CallClassMethodAsync
        /// <summary>
        /// Вызывает некоторый метод класса, при этом значения для его параметров необходимо прочитать из текстового файла(имя класса и имя метода передаются в качестве аргументов);    
        /// </summary>           
        #endregion
        public async Task CallClassMethodAsync(string ClassName, string MethodName)
        {
            this.ClassName = ClassName;
            this.MethodName = MethodName;
      
            FilePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            FilePath = System.IO.Path.Combine(FilePath, "Всё содержимое класса");        
            DirectoryInfo dirInf = new DirectoryInfo(FilePath);
            if (!dirInf.Exists)
            {
                dirInf.Create();
            }

            string Path = FilePath + @"\Всё содержимое класса.txt";
            if (File.Exists(Path))
            {
                try
                {         
                    await Task.Run(async () =>
                    {
                        using (StreamReader sr = new StreamReader(Path, System.Text.Encoding.Default))
                        {
                            string line;                         
                            while ((line = await sr.ReadLineAsync()) != null)
                            {
                                if (line.Contains(this.MethodName))
                                {
                                    Console.WriteLine($" {line}");
                                }                               
                            }
                        }
                    });                
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

        }

    }
    #endregion

    public class Crocodile //Крокодил
    {
        public string Name;
        public float BodyLength;
        public int Weight;
        public string crocodile { get; set; }//крокодил

        public Crocodile(string name, float bodyLength, int weight)
        {
            Name = name;
            BodyLength = bodyLength;
            Weight = weight;
        }

        public override string ToString()
        {
            return string.Format("Рептилия: {0} \t Длина тела = {1}; Вес = {2}.", Name, BodyLength, Weight);
        }
        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Crocodile temp = (Crocodile)obj;
            return base.Equals(temp);
            //return Name == temp.Name &&
            //BodyLength == temp.BodyLength &&
            //Weight == temp.Weight;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();        
        }
    }

}
