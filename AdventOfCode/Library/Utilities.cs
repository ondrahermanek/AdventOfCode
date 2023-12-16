﻿using System.ComponentModel.DataAnnotations;

namespace Library
{
    public static class Utilities
    {
        public static async Task<List<string>> ReadFileAsync(string fileName)
        {
            string consoleProjectPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "..", "Inputs", "2023");
            string fullPath = Path.Combine(consoleProjectPath, fileName);

            string fileContent = await File.ReadAllTextAsync(fullPath);
            return fileContent.Split("\r\n").ToList();
        }
    }
}
