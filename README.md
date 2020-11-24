# How to use guide:
       

<b>string path = ".../save2.esf";</b>

-------------------------------------------------------
<i>// create file or load it</i>

<b>EsfFile file = new EsfFile(path);</b>

-------------------------------------------------------
<i>// get an array from file</i>

<b>char[] chars = file.GetArray<char>("currency");</b>
 
 -------------------------------------------------------
        
<i>// get a value from file</i>

<b>string text = file.GetValue<string>("text");</b>
 
<b>int id = file.GetValue<int>("id");</b>
 
<b>bool state = file.GetValue<bool>("state");</b>
 
-------------------------------------------------------
        
<i>// set or add value (key, value)</i>

<b>file.SetValue("health", 75);</b>
<b>file.SetValue("time", 15.57f);</b>

-------------------------------------------------------
        
<i>// remove value (key)</i>

<b>file.RemoveValue("money");</b>

-------------------------------------------------------
        
<i>//save changes to file</i>

<b>file.Write(path);</b>

-------------------------------------------------------
        
        
<i>// delete save file</i>

<b>file.Delete(path);</b>

-------------------------------------------------------
        
<i>// to save original file structure (comments, spaces and so on) or not</i>

<i>// if false - it's faster</i>

<b>file.SaveOriginalStructure = false;</b>
