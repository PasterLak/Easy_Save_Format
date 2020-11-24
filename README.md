# Easy_Save_Format
 Format .esf



  		path = ".../save2.esf";



        EsfFile file = new EsfFile(path);
        

        char[] chars = file.GetArray<char>("currency");

        string text = file.GetValue<string>("text");

        file.SetValue("health", 75);
        file.SetValue("time", 15.57f);
        file.RemoveValue("money");
        
        file.Write(path);
