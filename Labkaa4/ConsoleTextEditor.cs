using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextEditorApp
{
  public class ConsoleTextEditor
  {
    private TextFile _currentFile;
    private TextFileOriginator _originator;
    private TextEditorHistory _history;
    private TextFileSearcher _searcher;
    private bool _isRunning;

    public ConsoleTextEditor()
    {
      _originator = new TextFileOriginator();
      _history = new TextEditorHistory(_originator);
      _searcher = new TextFileSearcher();
      _isRunning = true;
    }

    public void Run()
    {
      Console.WriteLine("=== Текстовый редактор ===");

      while (_isRunning)
      {
        ShowMainMenu();
        string choice = Console.ReadLine();

        switch (choice)
        {
          case "1":
            OpenFile();
            break;
          case "2":
            CreateNewFile();
            break;
          case "3":
            SearchFiles();
            break;
          case "4":
            if (_currentFile != null)
            {
              EditFile();
            }
            else
            {
              Console.WriteLine("Сначала откройте файл!");
            }
            break;
          case "5":
            IndexFiles();
            break;
          case "6":
            TestSerialization();
            break;
          case "0":
            _isRunning = false;
            break;
          default:
            Console.WriteLine("Неверный выбор!");
            break;
        }
      }
    }

    private void ShowMainMenu()
    {
      Console.WriteLine("\n--- Главное меню ---");
      Console.WriteLine("1. Открыть файл");
      Console.WriteLine("2. Создать новый файл");
      Console.WriteLine("3. Поиск файлов по ключевым словам");
      Console.WriteLine("4. Редактировать текущий файл");
      Console.WriteLine("5. Индексация файлов");
      Console.WriteLine("6. Тест сериализации");
      Console.WriteLine("0. Выход");
      Console.Write("Выберите действие: ");
    }

    private void OpenFile()
    {
      Console.Write("Введите путь к файлу: ");
      string path = Console.ReadLine();

      if (File.Exists(path))
      {
        _currentFile = new TextFile(path);
        _originator.SetContent(_currentFile.Content);
        _history.Clear();
        _history.Backup();
        Console.WriteLine($"Файл '{_currentFile.GetFileName()}' открыт");
      }
      else
      {
        Console.WriteLine("Файл не найден!");
      }
    }

    private void CreateNewFile()
    {
      Console.Write("Введите путь для нового файла: ");
      string path = Console.ReadLine();

      _currentFile = new TextFile(path);
      _currentFile.Content = string.Empty;
      _originator.SetContent(string.Empty);
      _history.Clear();
      _history.Backup();
      Console.WriteLine("Новый файл создан");
    }

    private void EditFile()
    {
      bool editing = true;
      while (editing)
      {
        Console.WriteLine("\n--- Редактирование ---");
        Console.WriteLine("Текущее содержимое:");
        Console.WriteLine("---------------------");
        Console.WriteLine(_originator.GetContent());
        Console.WriteLine("---------------------");
        Console.WriteLine("Команды:");
        Console.WriteLine("1. Добавить текст");
        Console.WriteLine("2. Очистить");
        Console.WriteLine("3. Отменить (Undo)");
        Console.WriteLine("4. Сохранить");
        Console.WriteLine("0. Вернуться в меню");
        Console.Write("Выберите действие: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
          case "1":
            Console.Write("Введите текст: ");
            string newText = Console.ReadLine();
            _history.Backup();
            _originator.SetContent(_originator.GetContent() + newText + "\n");
            break;
          case "2":
            _history.Backup();
            _originator.SetContent(string.Empty);
            break;
          case "3":
            if (_history.CanUndo())
            {
              _history.Undo();
              Console.WriteLine("Отмена выполнена");
            }
            else
            {
              Console.WriteLine("Нечего отменять");
            }
            break;
          case "4":
            if (_currentFile != null)
            {
              _currentFile.Content = _originator.GetContent();
              _currentFile.SaveToFile();
              Console.WriteLine("Файл сохранен");
            }
            break;
          case "0":
            editing = false;
            break;
        }
      }
    }

    private void SearchFiles()
    {
      Console.Write("Введите директорию для поиска: ");
      string directory = Console.ReadLine();

      if (!Directory.Exists(directory))
      {
        Console.WriteLine("Директория не существует!");
        return;
      }

      Console.WriteLine("Тип поиска:");
      Console.WriteLine("1. По одному ключевому слову");
      Console.WriteLine("2. По нескольким словам");
      string searchType = Console.ReadLine();

      if (searchType == "1")
      {
        Console.Write("Введите ключевое слово: ");
        string keyword = Console.ReadLine();

        List<string> results = _searcher.SearchByKeyword(directory, keyword);

        Console.WriteLine($"\nНайдено файлов: {results.Count}");
        foreach (string file in results)
        {
          Console.WriteLine($"- {file}");
        }
      }
      else if (searchType == "2")
      {
        Console.Write("Введите ключевые слова (через запятую): ");
        string keywordsInput = Console.ReadLine();
        List<string> keywords = keywordsInput.Split(',').Select(k => k.Trim()).ToList();

        Console.WriteLine("Соответствие:");
        Console.WriteLine("1. Всем словам");
        Console.WriteLine("2. Любому из слов");
        string matchType = Console.ReadLine();

        List<string> results = _searcher.SearchByKeywords(directory, keywords, matchType == "1");

        Console.WriteLine($"\nНайдено файлов: {results.Count}");
        foreach (string file in results)
        {
          Console.WriteLine($"- {file}");
        }
      }
    }

    private void IndexFiles()
    {
      Console.Write("Введите директорию для индексации: ");
      string directory = Console.ReadLine();

      if (!Directory.Exists(directory))
      {
        Console.WriteLine("Директория не существует!");
        return;
      }

      Console.Write("Введите ключевые слова для индексации (через запятую): ");
      string keywordsInput = Console.ReadLine();
      List<string> keywords = keywordsInput.Split(',').Select(k => k.Trim()).ToList();

      _searcher.CreateIndex(directory, keywords);
      var index = _searcher.GetFileIndex();

      Console.WriteLine("\n--- Индекс файлов ---");
      foreach (var entry in index)
      {
        Console.WriteLine($"Файл: {entry.Key}");
        Console.WriteLine($"Ключевые слова: {string.Join(", ", entry.Value)}");
        Console.WriteLine("---");
      }
    }

    private void TestSerialization()
    {
      if (_currentFile == null)
      {
        Console.WriteLine("Сначала откройте файл!");
        return;
      }

      Console.WriteLine("Тест сериализации:");
      Console.WriteLine("1. Бинарная сериализация");
      Console.WriteLine("2. XML сериализация");
      string choice = Console.ReadLine();

      string testFile = "test_serialization";

      if (choice == "1")
      {
        string binaryFile = testFile + ".bin";
        _currentFile.BinarySerialize(binaryFile);
        Console.WriteLine($"Бинарная сериализация в {binaryFile}");

        TextFile deserialized = TextFile.BinaryDeserialize(binaryFile);
        Console.WriteLine($"Десериализован файл: {deserialized.GetFileName()}");
      }
      else if (choice == "2")
      {
        string xmlFile = testFile + ".xml";
        _currentFile.XmlSerialize(xmlFile);
        Console.WriteLine($"XML сериализация в {xmlFile}");

        TextFile deserialized = TextFile.XmlDeserialize(xmlFile);
        Console.WriteLine($"Десериализован файл: {deserialized.GetFileName()}");
      }
    }
  }
}