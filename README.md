<details>
<summary><strong>Русский</strong></summary>

# ⚙️ MyStemSharpness: Высокопроизводительная библиотека для работы с MyStem на C#

Добро пожаловать в MyStemSharpness! Этот проект представляет собой тщательно разработанную .NET-библиотеку на языке C#, предназначенную для обеспечения эффективного и удобного взаимодействия с мощным лингвистическим инструментом MyStem. Мы понимаем, насколько важно иметь надежный и быстрый способ интеграции возможностей MyStem в ваши приложения, и именно поэтому создали эту библиотеку.

## 🚀 Начало работы

Если вы готовы погрузиться в мир лингвистического анализа с MyStemSharpness, вот первые шаги:

1.  **Установка:** На данный момент библиотека находится на стадии разработки и может быть установлена непосредственно из исходного кода. После публикации на NuGet, вы сможете добавить ее в свой проект Visual Studio Code или другой .NET-проект с помощью следующей команды в консоли NuGet Package Manager:

    ```bash
    dotnet add package MyStemSharpness

    ```

2.  **Добавление пространства имен:** В файлах, где вы планируете использовать MyStemSharpness, добавьте следующую строку в начало:

    ```csharp
    using MyStem;
    ```

3.  **Зависимость от MyStem:** Для работы библиотеки необходимо, чтобы на целевой машине был установлен исполняемый файл [`mystem.exe`](https://yandex.ru/dev/mystem/). Убедитесь, что он доступен по указанному пути.

## 🛠️ Примеры использования

Чтобы вы могли быстро оценить возможности MyStemSharpness, мы подготовили несколько примеров использования для различных сценариев.

### 💡 Однопоточный анализ

Для задач, где не требуется высокая степень параллелизма или обработка выполняется последовательно, вы можете использовать класс `SingleThreadedMyStem`.

```csharp
using MyStem;
using System;
using System.IO;

public class SingleThreadedExample
{
    public static void Main(string[] args)
    {
        // Укажите путь к исполняемому файлу MyStem, если он отличается от "mystem.exe"
        SingleThreadedMyStem.PathToMyStem = "path/to/mystem.exe";

        // Создайте экземпляр класса
        using var myStem = new SingleThreadedMyStem();
        myStem.Initialize(); // Инициализация процесса MyStem

        string textToAnalyze = "Это простой пример текста для анализа.";
        try
        {
            string result = myStem.Analysis(textToAnalyze);
            Console.WriteLine($"Результат анализа: {result}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Ошибка во время анализа: {ex.Message}");
        }
    }
}
```

### 🧵 Многопоточный анализ

Для приложений, где важна высокая производительность и параллельная обработка больших объемов текста, используйте класс `MultiThreadedMyStem`.

```csharp
using MyStem;
using System;
using System.IO;
using System.Threading.Tasks;

public class MultiThreadedExample
{
    public static async Task Main(string[] args)
    {
        // Укажите путь к исполняемому файлу MyStem, если он отличается от "mystem.exe"
        MultiThreadedMyStem.PathToMyStem = "path/to/mystem.exe";

        // Создайте экземпляр класса
        using var myStem = new MultiThreadedMyStem();
        myStem.Initialize(); // Инициализация процесса MyStem

        string textToAnalyze1 = "Первый текст для параллельного анализа.";
        string textToAnalyze2 = "Второй текст, который будет обработан одновременно.";

        var task1 = Task.Run(() => myStem.MultiAnalysis(textToAnalyze1));
        var task2 = Task.Run(() => myStem.MultiAnalysis(textToAnalyze2));

        try
        {
            string result1 = await task1;
            string result2 = await task2;
            Console.WriteLine($"Результат анализа 1: {result1}");
            Console.WriteLine($"Результат анализа 2: {result2}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Ошибка во время анализа: {ex.Message}");
        }
    }
}
```

### ⚙️ Настройка параметров MyStem

Библиотека предоставляет класс `MyStemOptions` для гибкой настройки параметров запуска MyStem.

```csharp
using MyStem;
using System;
using System.IO;

public class OptionsExample
{
    public static void Main(string[] args)
    {
        // Создайте объект с нужными параметрами
        var options = new MyStemOptions
        {
            LineByLine = true,
            PrintOnlyLemmasAndGrammemes = true,
            Encoding = "utf-8"
        };

        // Укажите путь к исполняемому файлу MyStem
        SingleThreadedMyStem.PathToMyStem = "path/to/mystem.exe";

        // Создайте экземпляр класса, передав опции
        using var myStem = new SingleThreadedMyStem(options);
        myStem.Initialize(); // Инициализация процесса MyStem

        string textToAnalyze = "Пример текста с настройками.";
        try
        {
            string result = myStem.Analysis(textToAnalyze);
            Console.WriteLine($"Результат анализа с опциями: {result}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Ошибка во время анализа: {ex.Message}");
        }
    }
}
```

## ⚠️ Возможные проблемы на стадии разработки

В процессе разработки и использования библиотеки MyStemSharpness могут возникнуть некоторые трудности:

- **Кодировка:** Неправильная настройка кодировки может привести к некорректному анализу текста. По умолчанию используется UTF-8, но при необходимости вы можете настроить другие кодировки через `MyStemOptions`.
- **Обработка ошибок MyStem:** Хотя библиотека старается обрабатывать исключения, ошибки, возникающие непосредственно в процессе MyStem, могут потребовать дополнительной диагностики.
- **Ресурсы системы:** Запуск внешнего процесса MyStem может потреблять системные ресурсы. При интенсивном использовании в многопоточной среде следите за загрузкой процессора и памяти.

## ⚡ Гарантия высокой производительности

Несмотря на возможные трудности, MyStemSharpness разработан с акцентом на высокую производительность:

- **Эффективное управление процессами:** Библиотека переиспользует экземпляры процесса MyStem, минимизируя накладные расходы на запуск новых процессов для каждого запроса.
- **Асинхронное чтение (многопоточный режим):** В классе `MultiThreadedMyStem` используется асинхронное чтение выходных данных MyStem, что позволяет избежать блокировки вызывающего потока и повышает общую пропускную способность.
- **Оптимизированные буферы:** Для чтения выходных данных используются буферы, размер которых динамически оценивается на основе размера входного текста, что снижает количество операций выделения памяти.
- **Разделение на однопоточную и многопоточную версии:** Предоставляя отдельные классы для разных сценариев использования, мы позволяем вам выбирать оптимальный подход для ваших конкретных потребностей, избегая ненужной синхронизации в однопоточных сценариях.

## 🙏 Благодарности и вклад

Мы надеемся, что MyStemSharpness станет ценным инструментом в вашем арсенале. Будем рады вашим отзывам, предложениям и вкладу в развитие проекта. Следите за обновлениями и новыми возможностями!

</details>

<details>
<summary><strong>English</strong></summary>

# ⚙️ MyStemSharpness: High-Performance Library for MyStem Interaction in C#

Welcome to MyStemSharpness! This project is a carefully crafted .NET library in C# designed to provide efficient and convenient interaction with the powerful MyStem linguistic tool. We understand the importance of having a reliable and fast way to integrate MyStem's capabilities into your applications, and that's why we created this library.

## 🚀 Getting Started

If you're ready to dive into the world of linguistic analysis with MyStemSharpness, here are the first steps:

1.  **Installation:** Currently, the library is under development and can be installed directly from the source code. Once published on NuGet, you will be able to add it to your Visual Studio Code or other .NET project using the following command in the NuGet Package Manager console:

    ```bash
    dotnet add package MyStemSharpness

    ```

2.  **Adding Namespace:** In the files where you plan to use MyStemSharpness, add the following line at the beginning:

    ```csharp
    using MyStem;
    ```

3.  **MyStem Dependency:** The library requires the [`mystem.exe`](https://yandex.ru/dev/mystem/) executable to be installed on the target machine. Ensure it is accessible at the specified path.

## 🛠️ Usage Examples

To help you quickly appreciate the capabilities of MyStemSharpness, we have prepared several usage examples for different scenarios.

### 💡 Single-Threaded Analysis

For tasks where a high degree of parallelism is not required or processing is performed sequentially, you can use the `SingleThreadedMyStem` class.

```csharp
using MyStem;
using System;
using System.IO;

public class SingleThreadedExample
{
    public static void Main(string[] args)
    {
        // Specify the path to the MyStem executable if it's different from "mystem.exe"
        SingleThreadedMyStem.PathToMyStem = "path/to/mystem.exe";

        // Create an instance of the class
        using var myStem = new SingleThreadedMyStem();
        myStem.Initialize(); // Initialize the MyStem process

        string textToAnalyze = "Это простой пример текста для анализа.";
        try
        {
            string result = myStem.Analysis(textToAnalyze);
            Console.WriteLine($"Analysis Result: {result}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error during analysis: {ex.Message}");
        }
    }
}
```

### 🧵 Multi-Threaded Analysis

For applications where high performance and parallel processing of large volumes of text are important, use the `MultiThreadedMyStem` class.

```csharp
using MyStem;
using System;
using System.IO;
using System.Threading.Tasks;

public class MultiThreadedExample
{
    public static async Task Main(string[] args)
    {
        // Specify the path to the MyStem executable if it's different from "mystem.exe"
        MultiThreadedMyStem.PathToMyStem = "path/to/mystem.exe";

        // Create an instance of the class
        using var myStem = new MultiThreadedMyStem();
        myStem.Initialize(); // Initialize the MyStem process

        string textToAnalyze1 = "Первый текст для параллельного анализа.";
        string textToAnalyze2 = "Второй текст, который будет обработан одновременно.";

        var task1 = Task.Run(() => myStem.MultiAnalysis(textToAnalyze1));
        var task2 = Task.Run(() => myStem.MultiAnalysis(textToAnalyze2));

        try
        {
            string result1 = await task1;
            string result2 = await task2;
            Console.WriteLine($"Analysis Result 1: {result1}");
            Console.WriteLine($"Analysis Result 2: {result2}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error during analysis: {ex.Message}");
        }
    }
}
```

### ⚙️ Configuring MyStem Parameters

The library provides the `MyStemOptions` class for flexible configuration of MyStem's launch parameters.

```csharp
using MyStem;
using System;
using System.IO;

public class OptionsExample
{
    public static void Main(string[] args)
    {
        // Create an object with the desired options
        var options = new MyStemOptions
        {
            LineByLine = true,
            PrintOnlyLemmasAndGrammemes = true,
            Encoding = "utf-8"
        };

        // Specify the path to the MyStem executable
        SingleThreadedMyStem.PathToMyStem = "path/to/mystem.exe";

        // Create an instance of the class, passing the options
        using var myStem = new SingleThreadedMyStem(options);
        myStem.Initialize(); // Initialize the MyStem process

        string textToAnalyze = "Пример текста с настройками.";
        try
        {
            string result = myStem.Analysis(textToAnalyze);
            Console.WriteLine($"Analysis Result with Options: {result}");
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Error during analysis: {ex.Message}");
        }
    }
}
```

## ⚠️ Potential Development Issues

During the development and use of the MyStemSharpness library, some difficulties may arise:

- **Encoding:** Incorrect encoding settings can lead to incorrect text analysis. UTF-8 is used by default, but you can configure other encodings via `MyStemOptions` if needed.
- **MyStem Error Handling:** While the library tries to handle exceptions, errors originating directly from the MyStem process might require additional diagnostics.
- **System Resources:** Running the external MyStem process can consume system resources. With intensive use in a multi-threaded environment, monitor CPU and memory usage.

## ⚡ High-Performance Guarantee

Despite potential difficulties, MyStemSharpness is designed with a focus on high performance:

- **Efficient Process Management:** The library reuses MyStem process instances, minimizing the overhead of starting new processes for each request.
- **Asynchronous Reading (Multi-Threaded Mode):** The `MultiThreadedMyStem` class uses asynchronous reading of MyStem's output, which prevents blocking the calling thread and increases overall throughput.
- **Optimized Buffers:** Buffers are used for reading output data, and their size is dynamically estimated based on the input text size, reducing the number of memory allocation operations.
- **Separation into Single-Threaded and Multi-Threaded Versions:** By providing separate classes for different usage scenarios, we allow you to choose the optimal approach for your specific needs, avoiding unnecessary synchronization in single-threaded scenarios.

## 🙏 Acknowledgments and Contributions

We hope that MyStemSharpness will become a valuable tool in your arsenal. We welcome your feedback, suggestions, and contributions to the project's development. Stay tuned for updates and new features!

```
</details>
```
