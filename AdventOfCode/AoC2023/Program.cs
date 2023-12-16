using AoC2023;

var task = new Task3();
var result = await task.Run();

Console.WriteLine($"Result:");
foreach ( var line in result )
{
    Console.WriteLine(line);
}

Console.ReadLine();