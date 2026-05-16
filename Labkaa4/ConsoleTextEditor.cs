using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TextEditorApp
{
  public class ConsoleTextEditor
  {
    private TextFile _currentFile;
    private readonly TextFileOriginator _originator;
    private readonly TextEditorHistory _history;
    private readonly TextFileSearcher _searcher;
    private bool _isFileOpen;

    public ConsoleTextEditor()
    {
      _originator = new TextFileOriginator();
      _history = new TextEditorHistory(_originator);
      _searcher = new TextFileSearcher();
      _isFileOpen = false;
    }

    public void Run()
    {
      bool exit = false;

      while (!exit)
      {
        Console.Clear();
        DisplayMainMenu();
        string choice = Console.ReadLine();

        switch (choice)
        {
          case "1":
            CreateNewFile();
            break;
          case "2":
            OpenFile();
            break;
          case "3":
            if (_isFileOpen)
              EditFile();
            else
              Console.WriteLine("Сначала откройте или создайте файл!");
            break;
          case "4":
            SearchFiles();
            break;
          case "5":
            IndexFiles();
            break;
          case "6":
            exit = true;
            break;
          default:
            Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
            Console.ReadKey();
            break;
        }
      }
    }

    private void DisplayMainMenu()
    {
      Console.WriteLine("=== Текстовый редактор ===");
      Console.WriteLine("1. Создать новый файл");
      Console.WriteLine("2. Открыть существующий файл");
      Console.WriteLine("3. Редактировать текущий файл");
      Console.WriteLine("4. Поиск файлов по ключевым словам");
      Console.WriteLine("5. Индексация файлов");
      Console.WriteLine("6. Выход");
      Console.Write("Выберите действие: ");
    }

    private void CreateNewFile()
    {
      Console.Write("Введите путь для нового файла: ");
      string path = Console.ReadLine();

      try
      {
        _currentFile = new TextFile
        {
          FilePath = path,
          Content = string.Empty,
          IsModified = true
        };

        _originator.SetContent(_currentFile.Content);
        _history.Clear();
        _history.Backup();
        _isFileOpen = true;

        Console.WriteLine($"Создан новый файл: {path}");
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
        Console.ReadKey();
      }
    }

    private void OpenFile()
    {
      Console.Write("Введите путь к файлу: ");
      string path = Console.ReadLine();

      try
      {
        _currentFile = new TextFile(path);
        _originator.SetContent(_currentFile.Content);
        _history.Clear();
        _history.Backup();
        _isFileOpen = true;

        Console.WriteLine($"Файл загружен. Содержимое:\n{_currentFile.Content}");
        Console.WriteLine("Нажмите любую клавишу для продолжения...");
        Console.ReadKey();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при открытии файла: {ex.Message}");
        Console.ReadKey();
      }
    }

    private void EditFile()
    {
      bool exitEdit = false;

      while (!exitEdit)
      {
        Console.Clear();
        Console.WriteLine("=== Редактирование файла ===");
        Console.WriteLine($"Текущий файл: {_currentFile.GetFileName()}");
        Console.WriteLine($"Содержимое:\n{_originator.GetContent()}");
        Console.WriteLine("\n--- Действия ---");
        Console.WriteLine("1. Редактировать текст");
        Console.WriteLine("2. Отменить последнее изменение");
        Console.WriteLine("3. Сохранить файл");
        Console.WriteLine("4. Сохранить как (бинарная сериализация)");
        Console.WriteLine("5. Сохранить как (XML сериализация)");
        Console.WriteLine("6. Выход без сохранения");
        Console.Write("Выберите действие: ");

        string choice = Console.ReadLine();

        switch (choice)
        {
          case "1":
            EditText();
            break;
          case "2":
            if (_history.CanUndo())
            {
              _history.Undo();
              _currentFile.Content = _originator.GetContent();
              _currentFile.IsModified = true;
              Console.WriteLine("Изменение отменено.");
            }
            else
            {
              Console.WriteLine("Нет действий для отмены.");
            }
            Console.ReadKey();
            break;
          case "3":
            SaveFile();
            break;
          case "4":
            SaveAsBinary();
            break;
          case "5":
            SaveAsXml();
            break;
          case "6":
            if (_currentFile.IsModified)
            {
              Console.Write("Есть несохранённые изменения. Выйти без сохранения? (y/n): ");
              string confirm = Console.ReadLine();
              if (confirm?.ToLower() == "y")
                exitEdit = true;
            }
            else
            {
              exitEdit = true;
            }
            break;
          default:
            Console.WriteLine("Неверный выбор.");
            Console.ReadKey();
            break;
        }
      }
    }

    private void EditText()
    {
      Console.WriteLine("Введите новый текст (для завершения введите пустую строку):");
      var newContent = new System.Text.StringBuilder();
      string line;

      while ((line = Console.ReadLine()) != "")
      {
        newContent.AppendLine(line);
      }

      if (newContent.Length > 0)
      {
        _history.Backup();
        string content = newContent.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        _originator.SetContent(content);
        _currentFile.Content = _originator.GetContent();
        _currentFile.IsModified = true;
        Console.WriteLine("Текст обновлён.");
      }
      else
      {
        Console.WriteLine("Текст не изменён.");
      }
      Console.ReadKey();
    }

    private void SaveFile()
    {
      try
      {
        _currentFile.Content = _originator.GetContent();
        _currentFile.SaveToFile();
        Console.WriteLine("Файл сохранён.");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при сохранении: {ex.Message}");
      }
      Console.ReadKey();
    }

    private void SaveAsBinary()
    {
      Console.Write("Введите путь для бинарного сохранения: ");
      string path = Console.ReadLine();

      try
      {
        _currentFile.Content = _originator.GetContent();
        _currentFile.BinarySerialize(path);
        Console.WriteLine($"Файл сохранён в бинарном формате: {path}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при бинарном сохранении: {ex.Message}");
      }
      Console.ReadKey();
    }

    private void SaveAsXml()
    {
      Console.Write("Введите путь для XML сохранения: ");
      string path = Console.ReadLine();

      try
      {
        _currentFile.Content = _originator.GetContent();
        _currentFile.XmlSerialize(path);
        Console.WriteLine($"Файл сохранён в XML формате: {path}");
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при XML сохранении: {ex.Message}");
      }
      Console.ReadKey();
    }

    private void SearchFiles()
    {
      Console.Clear();
      Console.WriteLine("=== Поиск файлов ===");
      Console.Write("Введите путь к директории для поиска: ");
      string directory = Console.ReadLine();

      Console.Write("Искать в поддиректориях? (y/n): ");
      bool searchSubdirs = Console.ReadLine()?.ToLower() == "y";

      Console.WriteLine("Выберите тип поиска:");
      Console.WriteLine("1. По одному ключевому слову");
      Console.WriteLine("2. По нескольким ключевым словам");
      Console.Write("Выбор: ");
      string searchType = Console.ReadLine();

      try
      {
        List<string> results;

        if (searchType == "1")
        {
          Console.Write("Введите ключевое слово: ");
          string keyword = Console.ReadLine();
          results = _searcher.SearchByKeyword(directory, keyword, searchSubdirs);
        }
        else if (searchType == "2")
        {
          Console.Write("Введите ключевые слова через запятую: ");
          string keywordsInput = Console.ReadLine();
          string[] keywords = keywordsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                           .Select(k => k.Trim())
                                           .ToArray();

          if (keywords.Length == 0)
          {
            Console.WriteLine("Не введено ни одного ключевого слова.");
            Console.ReadKey();
            return;
          }

          Console.WriteLine("Выберите условие поиска:");
          Console.WriteLine("1. Файл должен содержать ВСЕ ключевые слова");
          Console.WriteLine("2. Файл должен содержать ЛЮБОЕ из ключевых слов");
          Console.Write("Выбор: ");
          string matchType = Console.ReadLine();

          bool matchAll = matchType == "1";
          results = _searcher.SearchByKeywords(directory, keywords, matchAll, searchSubdirs);
        }
        else
        {
          Console.WriteLine("Неверный тип поиска.");
          Console.ReadKey();
          return;
        }

        DisplaySearchResults(results);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при поиске: {ex.Message}");
        Console.ReadKey();
      }
    }

    private void DisplaySearchResults(List<string> results)
    {
      Console.WriteLine($"\n=== Результаты поиска (найдено: {results.Count}) ===");

      if (results.Count == 0)
      {
        Console.WriteLine("Файлы не найдены.");
      }
      else
      {
        for (int i = 0; i < results.Count; i++)
        {
          Console.WriteLine($"{i + 1}. {results[i]}");
        }
      }

      Console.WriteLine("\nНажмите любую клавишу для продолжения...");
      Console.ReadKey();
    }

    private void IndexFiles()
    {
      Console.Clear();
      Console.WriteLine("=== Индексация файлов ===");
      Console.Write("Введите путь к директории для индексации: ");
      string directory = Console.ReadLine();

      Console.Write("Введите ключевые слова для индексации (через запятую): ");
      string keywordsInput = Console.ReadLine();
      string[] keywords = keywordsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(k => k.Trim())
                                       .ToArray();

      if (keywords.Length == 0)
      {
        Console.WriteLine("Не введено ни одного ключевого слова.");
        Console.ReadKey();
        return;
      }

      Console.Write("Искать в поддиректориях? (y/n): ");
      bool searchSubdirs = Console.ReadLine()?.ToLower() == "y";

      try
      {
        var index = _searcher.CreateIndex(directory, keywords, searchSubdirs);

        Console.WriteLine($"\n=== Результаты индексации ===");
        Console.WriteLine($"Найдено файлов с ключевыми словами: {index.Count}");

        foreach (var kvp in index)
        {
          Console.WriteLine($"\nФайл: {kvp.Key}");
          Console.WriteLine($"Найденные ключевые слова: {string.Join(", ", kvp.Value)}");
        }

        if (index.Count == 0)
        {
          Console.WriteLine("\nФайлы, содержащие указанные ключевые слова, не найдены.");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Ошибка при индексации: {ex.Message}");
      }

      Console.WriteLine("\nНажмите любую клавишу для продолжения...");
      Console.ReadKey();
    }
  }
}