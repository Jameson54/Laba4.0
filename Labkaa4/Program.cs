
using System;

namespace TextEditorApp
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.OutputEncoding = System.Text.Encoding.UTF8;

      try
      {
        ConsoleTextEditor editor = new ConsoleTextEditor();
        editor.Run();
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Критическая ошибка: {ex.Message}");
      }

      Console.WriteLine("\nПрограмма завершена. Нажмите любую клавишу...");
      Console.ReadKey();
    }
  }
}