# CommandLineParser (under construction)
A library for parsing command line arguments. Written in C#, based on .NET Core.

## Features
- Unnamed values
- [Single or multi-valued options](#2-options)
- Bool flags
- Commands

##Examples

###1. Registering types and parsing
Define a class which will hold the parsed arguments:
```csharp
class Arguments
{
    [Option('i', "input")]
    public string Input { get; set; }
    [Option('o', "output")]
    public string Output { get; set; }
}
```

In the main method instantiate a Parser object and register the type. After that pass on the args from the Main method to the parser's Parse method:
```csharp
public static void Main(string[] args)
{
    var parser = new Parser();
    parser.Register<Arguments>(arguments =>
    {
        Console.WriteLine(arguments.Input);
        Console.WriteLine(arguments.Output);
    }).Parse(args);
}
```

Call the program:
```
program.exe --input D:\input.txt --output D:\output.txt
```

Result:
```
D:\input.txt
D:\output.txt
```

###2. Options
Options can be placed on properties with different types. 
The current supported types are Int32, String, FileInfo, Boolean and user defined Enums.
IEnumerable of all of those types is also supported.

```csharp
class Arguments
{
    [Option('i', "input")]
    public FileInfo Input { get; set; }
    [Option('o', "output")]
    public FileInfo Output { get; set; }
    [Option('e', "enabled")]
    public bool IsEnabled { get; set; }
    [Option('n', "numbers")]
    public IEnumerable<int> Numbers { get; set; }
}
```
Parsing:

```csharp
public static void Main(string[] args)
{
    var parser = new Parser();
    parser.Register<Arguments>(arguments =>
    {
        Console.WriteLine(arguments.Input);
        Console.WriteLine(arguments.Output);
        Console.WriteLine(arguments.IsEnabled);
        Console.WriteLine(string.Join(", ", arguments.Numbers));
    }).Parse(args);
}
```
Call the program

```
program.exe -i in.txt -o out.txt -e true -n 1 2 3 4 5
```
Result:

```
in.txt
out.txt
True
1, 2, 3, 4, 5
```

###3. ...
