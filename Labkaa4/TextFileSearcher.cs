using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TextEditorApp
{
  public class TextFileSearcher
  {
    private readonly StringComparison _comparison = StringComparison.OrdinalIgnoreCase;

    public List<string> SearchByKeyword(string directory, string keyword, bool searchSubdirectories = false)
    {
      var results = new List<string>();

      if (!Directory.Exists(directory))
        throw new DirectoryNotFoundException($"Директория не найдена: {directory}");

      var searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
      string[] files = Directory.GetFiles(directory, "*.txt", searchOption);

      foreach (string filePath in files)
      {
        try
        {
          string content = File.ReadAllText(filePath, Encoding.UTF8);
          if (content.IndexOf(keyword, _comparison) >= 0)
          {
            results.Add(filePath);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
        }
      }

      return results;
    }

    public List<string> SearchByKeywords(string directory, string[] keywords, bool matchAll, bool searchSubdirectories = false)
    {
      var results = new List<string>();

      if (!Directory.Exists(directory))
        throw new DirectoryNotFoundException($"Директория не найдена: {directory}");

      if (keywords == null || keywords.Length == 0)
        throw new ArgumentException("Не указаны ключевые слова для поиска");

      var searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
      string[] files = Directory.GetFiles(directory, "*.txt", searchOption);

      foreach (string filePath in files)
      {
        try
        {
          string content = File.ReadAllText(filePath, Encoding.UTF8);
          bool fileMatches;

          if (matchAll)
          {
            fileMatches = KeywordsMatchAll(content, keywords);
          }
          else
          {
            fileMatches = KeywordsMatchAny(content, keywords);
          }

          if (fileMatches)
          {
            results.Add(filePath);
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Ошибка при чтении файла {filePath}: {ex.Message}");
        }
      }

      return results;
    }

    public bool KeywordsMatchAll(string content, string[] keywords)
    {
      foreach (string keyword in keywords)
      {
        if (content.IndexOf(keyword, _comparison) < 0)
          return false;
      }
      return true;
    }

    public bool KeywordsMatchAny(string content, string[] keywords)
    {
      foreach (string keyword in keywords)
      {
        if (content.IndexOf(keyword, _comparison) >= 0)
          return true;
      }
      return false;
    }

    public Dictionary<string, List<string>> CreateIndex(string directory, string[] keywords, bool searchSubdirectories = false)
    {
      var fileIndex = new Dictionary<string, List<string>>();

      if (!Directory.Exists(directory))
        throw new DirectoryNotFoundException($"Директория не найдена: {directory}");

      if (keywords == null || keywords.Length == 0)
        throw new ArgumentException("Не указаны ключевые слова для индексации");

      var searchOption = searchSubdirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
      string[] files = Directory.GetFiles(directory, "*.txt", searchOption);

      foreach (string filePath in files)
      {
        try
        {
          string content = File.ReadAllText(filePath, Encoding.UTF8);
          var foundKeywords = new List<string>();

          foreach (string keyword in keywords)
          {
            if (content.IndexOf(keyword, _comparison) >= 0)
            {
              foundKeywords.Add(keyword);
            }
          }

          if (foundKeywords.Count > 0)
          {
            fileIndex[filePath] = foundKeywords;
          }
        }
        catch (Exception ex)
        {
          Console.WriteLine($"Ошибка при индексации файла {filePath}: {ex.Message}");
        }
      }

      return fileIndex;
    }

    public List<string> GetFileIndex(Dictionary<string, List<string>> index, string filePath)
    {
      if (index.TryGetValue(filePath, out List<string> keywords))
      {
        return keywords;
      }
      return new List<string>();
    }
  }
}