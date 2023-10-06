

public sealed class EasySave
{

    private Data[]? _data;
    private readonly Dictionary<string, Data> _dictionary = new Dictionary<string, Data>();
    private readonly Dictionary<string, Data> _addedData = new Dictionary<string, Data>();
    
    private string _path;

    public EasySave(string path)
    {
        _path = path;
        if (!File.Exists(path)) Console.WriteLine("The file doesnt exist!");

        Load(path);
    }

    public bool Exists(string key)
    {
        return _dictionary.ContainsKey(key);
    }

    public void SetValue<T>(string key, T value) where T : IComparable
    {

        if(value == null) return;
        
        if (_dictionary.ContainsKey(key))
        {

            if (typeof(T) == typeof(string))
            {
                _dictionary[key].IsString = true;
            }
            
            _dictionary[key].Value = value.ToString();
 
            foreach (var da in _data)
            {
                if (da.Key != key) continue;
                
                da.Value = value.ToString();
                return;
            }
        }
        else
        {
            
            _dictionary.Add(key, new Data(key,value.ToString(), false));
            
            Data data = new Data(key, value.ToString(), false);
            if (typeof(T) == typeof(string))
            {
                data.IsString = true;
            }
            _addedData.Add(key,data);
        }
    }

    public void SetArray<T>(string key, T[] value) where T : IComparable
    {
        if(value == null) return;

        string m = MassiveToText<T>(value);
        
        if (_dictionary.ContainsKey(key))
        {
            
            _dictionary[key].Value = m;
            
            foreach (var da in _data)
            {
                if (da.Key != key) continue;
                
                da.Value = m;
                return;
            }
        }
        else
        {
            Data data = new Data(key, m, false);
            if (typeof(T) == typeof(string[]))
            {
                //data.IsString = true;
            }
            
            _dictionary.Add(key, data);
            
            _addedData.Add(key,data);
        }
    }

    public T GetValue<T>(string key) where T : IComparable
    {
        key = key.ToLower();
        
        if (!_dictionary.ContainsKey(key)) return default(T);

        return TryParse<T>(_dictionary[key].Value, out T value) ? value : value;
    }
    
    public T[] GetArray<T>(string key) where T : IComparable
    {

        if (!_dictionary.ContainsKey(key)) return new T[0];
        
        return ReadMassive<T>(_dictionary[key].Value);

    }


    private void Load(string path)
    {
        if (!File.Exists(path)) return;
        
        var lines = File.ReadAllLines(path);
        
        _data = new Data[lines.Length];

        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            
            if (IsComment(line))
            {
                _data[i] = new Data(string.Empty, line, true);
                continue;
            }
            
            
            if (!TryReadLine(line, out string key, out string value, out bool isString)) continue;

            _data[i] = new Data(key, value, false);

            if (isString) _data[i].IsString = true;

            if (_dictionary.ContainsKey(key) == false)
            {
                _dictionary.Add(key, _data[i]);
            }
        }
    }

    public void RemoveValue(string key)
    {
        if (!_dictionary.ContainsKey(key)) return;

        _dictionary.Remove(key);

        if (_addedData.ContainsKey(key))
        {
            _addedData.Remove(key);
        }

        for (var index = 0; index < _data.Length; index++)
        {
   
            if (_data[index].Key != key) continue;

            _data[index].Delete = true;
            return;
        }
    }

    public void Reload()
    {
        Load(_path);
    }

    public void SetPath(string path)
    {
        _path = path;
    }

    public void PrintValue(string key)
    {
        if (_dictionary.ContainsKey(key))
        {
            Console.WriteLine(_dictionary[key].Value);
        }
    }

    public void Save()
    {
        var lines = new List<string>();

        if(_data != null)
        for (var i = 0; i < _data.Length; i++)
        {
            if(_data[i].Delete) continue;

            if (_data[i].IsString) _data[i].Value = "\"" + _data[i].Value + "\"";
            
            lines.Add(_data[i].KeyToLower().GetAsString());
        }

        foreach (var pair in _addedData)
        {
            if (pair.Value.IsString) pair.Value.Value = "\"" + pair.Value.Value + "\"";
            
            var line = $"{pair.Key.ToLower()} = {pair.Value.Value}";
            lines.Add(line);
        }

        if(lines.Count != 0)
        File.WriteAllLines(_path, lines.ToArray());
    }

    private static bool TryReadLine(string line, out string key, out string value, out bool isString)
    {
        key = string.Empty;
        value = string.Empty;
        isString = false;

        line = line.Trim();

        if (!CheckLineSyntax(line)) return false;

        string[] parts = line.Split('=');

        if (parts.Length != 2) return false;

        key = parts[0].ToLower().Trim();

        if (parts[1].Trim().StartsWith("\""))
        {
            isString = true;
            parts[1] = parts[1].Trim('"');
        }

        value = IfIsString(parts[1], isString);

        return true;
    }

    private static string IfIsString(string line, bool isString)
    {
        return !isString ? line.Trim() : line;
    }

    private static bool IsComment(string line)
    {
        return string.IsNullOrWhiteSpace(line)
            || line.StartsWith("//") || line.StartsWith("#");
    }

    private static bool CheckLineSyntax(string line)
    {
        return !string.IsNullOrWhiteSpace(line) && line.Contains("=");
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
    
    private T[] ReadMassive<T>(string value) where T : IComparable
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

    private static bool TryParse<T>(string data, out T value) where T : IComparable
    {
        value = default(T);

        try
        {
            value = (T)Convert.ChangeType(data, typeof(T));
            return true;
        }
        catch
        {
            return false;
        }
    }


    private class Data
    {
        public string Key;
        public string Value;
        public bool IsComment;
        public bool Delete = false;
        public bool IsString = false;

        public Data(string key, string value, bool isComment)
        {
            Key = key;
            Value = value;
            IsComment = isComment;
        }

        public Data KeyToLower()
        {
            Key = Key.ToLower();
            return this;
        }

        public string GetAsString() => !IsComment ? $"{Key} = {Value}" : Value;
    }


}
