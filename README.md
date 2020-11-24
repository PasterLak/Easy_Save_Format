# Easy_Save_Format
 Format .esf

       How to use guide:
       

  		    string path = ".../save2.esf";


        // create file or load it
        EsfFile file = new EsfFile(path);
        
        // get an array from file
        char[] chars = file.GetArray<char>("currency");
        
        // get a text from file
        string text = file.GetValue<string>("text");
        
        // set or add value (key, value)
        file.SetValue("health", 75);
        file.SetValue("time", 15.57f);
        
        // remove value (key)
        file.RemoveValue("money");
        
        //save changes to file
        file.Write(path);
        
        
        // delete save file
        file.Delete(path);
        
        // to save original file structure (comments, spaces and so on) or not
        // if false - it's faster
        file.SaveOriginalStructure = false;
