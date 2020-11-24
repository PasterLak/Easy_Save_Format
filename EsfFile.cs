using System;
using System.Collections.Generic;
using System.IO;

public sealed class EsfFile
{
    private Dictionary<string, string> _dictionary = new Dictionary<string, string>();
    private List<string> _addedKeys = new List<string>();

    public bool SaveOriginalStructure { get; set; }

    public EsfFile()
    {

    }

    public EsfFile(Dictionary<string, string> dictionary)
    {
        _dictionary = dictionary;
    }

    public EsfFile(string path)
    {
        Read(path);
    }

    public void SetValue<T>(string key, T value) where T : IConvertible
    {

        if (_dictionary.Keys.Count == 0) return;

        if (_dictionary.ContainsKey(key))
        {
            if (!value.GetType().IsArray)
            {
                _dictionary[key] = value.ToString();
            }
            else
            {
                _dictionary[key] = MassiveToText((IConvertible[])(object)value);
            }
        }
        else
        {
            AddValue(key, value);
        }

    }

    public void SetValue<T>(string key, T[] value)
    {

        if (_dictionary.Keys.Count == 0) return;

        if (_dictionary.ContainsKey(key))
        {
            _dictionary[key] = MassiveToText(value);
        }
        else
        {
            AddValue(key, value);
        }

    }

    private void AddValue<T>(string key, T value) where T : IConvertible
    {

        if (!_dictionary.ContainsKey(key))
        {
            _dictionary.Add(key, value.ToString());
            _addedKeys.Add(key);
        }
        else
        {
            Console.WriteLine(key + " already exist!");
        }

    }

    private void AddValue<T>(string key, T[] value)
    {

        if (!_dictionary.ContainsKey(key))
        {
            _dictionary.Add(key, MassiveToText(value));
            _addedKeys.Add(key);
        }
        else
        {
            Console.WriteLine(key + " already exist!");
        }

    }

    public void RemoveValue(string key)
    {

        if (_dictionary.ContainsKey(key))
        {
            _dictionary.Remove(key);
        }

    }

    public T GetValue<T>(string key) where T : IConvertible
    {

        if (_dictionary.Keys.Count == 0 & !_dictionary.ContainsKey(key))
        {
            return (T)(object)null;
        }

        return (T)Convert.ChangeType(_dictionary[key], typeof(T));

    }

    public T[] GetArray<T>(string key) where T : IConvertible
    {

        if (_dictionary.Keys.Count == 0 && !_dictionary.ContainsKey(key))
        {
            return new T[0];
        }

        return ReadMassive<T>(_dictionary[key]);

    }

    public void GetValue<T>(string key, out T value)
    {

        value = (T)(object)null;

        if (_dictionary.Keys.Count == 0 && !_dictionary.ContainsKey(key))
            return;

        value = (T)Convert.ChangeType(_dictionary[key], typeof(T));

    }

    public void GetValue<T>(string key, out T[] array) where T : IConvertible
    {

        array = null;

        if (_dictionary.Keys.Count == 0) return;

        if (_dictionary.ContainsKey(key))
        {
            array = ReadMassive<T>(_dictionary[key]);
        }

    }

    public void Write(string path)
    {

        if (SaveOriginalStructure == false)
        {
            List<string> line = new List<string>();

            foreach (string key in _dictionary.Keys)
            {
                line.Add(key + " = " + _dictionary[key]);
            }

            File.WriteAllLines(path, line.ToArray());

            _addedKeys.Clear();

            return;
        }


        string[] lines = new string[0];
        List<string> newLines = new List<string>();
        List<string> keys = new List<string>();

        if (File.Exists(path))
        {
            lines = File.ReadAllLines(path);

        }


        for (int i = 0; i < lines.Length; i++)
        {
            if (TryReadLine(lines[i], out string key, out string _))
            {
                keys.Add(key);

                if (_dictionary.ContainsKey(key) == true)
                {

                    string value = _dictionary[key];

                    lines[i] = key + " = " + value;
                }

            }
        }

        if (File.Exists(path))
            File.WriteAllLines(path, lines);
        else
            File.Create(path).Close();

        if (_addedKeys.Count > 0)
        {

            for (int i = 0; i < _addedKeys.Count; i++)
            {
                if (!keys.Contains(_addedKeys[i]))
                {

                    newLines.Add(_addedKeys[i] + " = " + _dictionary[_addedKeys[i]]);

                }
            }

            _addedKeys.Clear();

        }

        if (newLines.Count > 0)
        {
            File.AppendAllLines(path, newLines.ToArray());
        }
    }

    public void Delete(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    private void Read(string path)
    {
        if (File.Exists(path))
        {

            string[] lines = File.ReadAllLines(path);

            foreach (string line in lines)
            {
                if (TryReadLine(line, out string key, out string value))
                {

                    if (_dictionary.ContainsKey(key) == false)
                    {
                        _dictionary.Add(key, value);
                    }

                }
            }
        }

    }

    private string MassiveToText<T>(T[] array)
    {

        string value = "[";

        if (array == null) return "ERROR";

        for (int i = 0; i < array.Length; i++)
        {
            value += array[i].ToString();
            if (i < array.Length - 1) value += ",";
        }

        value += "]";

        return value;

    }

    private bool TryReadLine(string line, out string key, out string value)
    {

        key = "";
        value = "";

        if (String.IsNullOrWhiteSpace(line)) return false;
        if (line.Contains("=") == false) return false;
        if (line[0] == '/' & line[1] == '/') return false;
        if (line[0] == '#') return false;

        string[] parts = line.Split('=');

        if (parts.Length == 2)
        {
            key = parts[0].Trim(' ');
            key = key.Replace(" ", string.Empty);

            parts[1] = parts[1].Trim(' ');

            if (parts[1][0] == '"' & parts[1][parts[1].Length - 1] == '"')
                parts[1] = parts[1].Substring(1, parts[1].Length - 2);

            value = parts[1];

            return true;

        }

        return false;

    }

    private T[] ReadMassive<T>(string value) where T : IConvertible
    {

        string[] data;
        T[] num;

        if (typeof(T) != typeof(string))
        {
            value = value.Replace(" ", string.Empty);
        }

        value = value.Replace("[", string.Empty);
        value = value.Replace(",]", string.Empty);
        value = value.Replace("]", string.Empty);

        data = value.Split(',');
        num = new T[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] != string.Empty)
                num[i] = (T)Convert.ChangeType(data[i], typeof(T));
            else
                Console.WriteLine("Empty string!");
        }

        return num;

    }


}