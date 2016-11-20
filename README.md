# CommandLineParser 
A library for parsing command line arguments.

## Features
- Unnamed values
- Single or multi-valued options
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
