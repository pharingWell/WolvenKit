using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace WolvenKit.App.Models;

public class FileCollection : ObservableCollection<FileModel>
{
    public void AddEntry(string entry, string projectDir, int startIndex = 0)
    {
        if (startIndex >= entry.Length)
        {
            return;
        }

        var endIndex = entry.IndexOf("\\", startIndex, StringComparison.Ordinal);
        if (endIndex == -1)
        {
            endIndex = entry.Length;
        }

        
        var key = entry.Substring(startIndex, endIndex - startIndex);
        if (string.IsNullOrEmpty(key))
        {
            return;
        }
        
        var item = this.FirstOrDefault(n => n != null && n.Name == key);
        if (item == null)
        {
            var relPath = entry.Substring(0, endIndex);

            bool isDirectory;
            if (endIndex + 1 < entry.Length)
            {
                isDirectory = true;
            }
            else
            {
                var attr = File.GetAttributes(Path.Combine(projectDir, entry));
                isDirectory = attr.HasFlag(FileAttributes.Directory);
            }

            item = new FileModel(key, relPath, projectDir, isDirectory);
            Add(item);
        }
        item.Children.AddEntry(entry, projectDir, endIndex + 1);
    }

    public void RemoveEntry(string entry, int startIndex = 0)
    {
        if (startIndex >= entry.Length)
        {
            return;
        }

        var endIndex = entry.IndexOf("\\", startIndex, StringComparison.Ordinal);
        if (endIndex == -1)
        {
            endIndex = entry.Length;
        }
        var key = entry.Substring(startIndex, endIndex - startIndex);
        if (string.IsNullOrEmpty(key))
        {
            return;
        }

        var item = this.FirstOrDefault(n => n.Name == key);
        if (item == null)
        {
            return;
        }

        if (endIndex + 1 >= entry.Length)
        {
            Remove(item);
        }
        else
        {
            item.Children.RemoveEntry(entry, endIndex + 1);
        }
    }
}