using System.CommandLine;

var bundleCommand = new Command("bundle", "Bundle code files to a single file");
var rspCommand = new Command("create-rsp", "Create response file");
var outputOption = new Option<FileInfo>(new[] { "--output", "-o" }, "File pass or name"){ IsRequired = true };
var languageOption = new Option<string>(new[] { "--language", "-l" }, "Programming languages to include in the bundle") { IsRequired = true };
var noteOption = new Option<bool>(new[] { "--note", "-n" }, "Include source code origin as a comment");
var sortOption = new Option<string>(new[] { "--sort", "-s" }, "Sort order for code files by Alphabetical order or type");
var removeEmptyLinesOption = new Option<bool>(new[] { "--remove-empty-lines" ,"-r"}, "Remove empty lines from code files");
var authorOption = new Option<string>(new[] { "--author","-a" }, "Author's name");

noteOption.SetDefaultValue(false);
removeEmptyLinesOption.SetDefaultValue(false);
sortOption.SetDefaultValue("Alphabetical order");

bundleCommand.AddOption(outputOption);
bundleCommand.AddOption(languageOption);
bundleCommand.AddOption(noteOption);
bundleCommand.AddOption(sortOption);
bundleCommand.AddOption(removeEmptyLinesOption);
bundleCommand.AddOption(authorOption);

string currentPath = Directory.GetCurrentDirectory();
List<string> allFolders = Directory.GetFiles(currentPath, "", SearchOption.AllDirectories).Where(file => !file.Contains("bin") && !file.Contains("Debug")).ToList();

string[] LanguagesArray = { "c", "c++", "java", "c#", "javascript", "html", "css", "pyton" };
string[] extensionsArray = { ".c", ".cpp", ".java", ".cs", ".js", ".html", ".css", ".py" };

bundleCommand.SetHandler((output, language, note,sort, removeEmptyLines, author) =>
{
    try
    {
        string[] languages = GetExtentionsOfLanguages(language, extensionsArray, LanguagesArray);
        string[] files = allFolders.Where(file => languages.Contains(Path.GetExtension(file))).ToArray();

        if (files.Any())
        {
            using (StreamWriter writer = new StreamWriter(output.FullName))
            {
                if (!string.IsNullOrEmpty(author))
                {
                    writer.WriteLine($"# Author: {author}");
                    writer.WriteLine();
                }
                if (note)
                {
                    writer.WriteLine($"# Source code origin: {Directory.GetCurrentDirectory()}");
                    writer.WriteLine();
                }

                if (sort.ToLower() == "type")
                {
                    Array.Sort(files, (a, b) => Path.GetExtension(a).CompareTo(Path.GetExtension(b)));
                }
                else
                {
                    Array.Sort(files);
                }

                foreach (var file in files)
                {
                    string content = File.ReadAllText(file);

                    if (removeEmptyLines)
                    {
                        content = string.Join(Environment.NewLine, content.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)));
                    }

                    writer.WriteLine($"### {Path.GetFileName(file)}");
                    writer.WriteLine(content);
                    writer.WriteLine();
                }
               Console.WriteLine($"File {output.FullName} was created successfully");
            }
        }
        else Console.WriteLine($"There are no match files");
    }
    catch (DirectoryNotFoundException ex)
    {
        Console.WriteLine("Error:File path is invalid");
    }
    catch (IOException ex1)
    {
        Console.WriteLine("Error:IOException");
    }

}, outputOption, languageOption, noteOption, sortOption, removeEmptyLinesOption,authorOption);

rspCommand.SetHandler(() =>
{
    var responseFile = new FileInfo("responseFile.rsp");
    try
    {
        using (StreamWriter rspWriter = new StreamWriter(responseFile.FullName))
        {
            Console.WriteLine("Enter File output name");
            string output;
            do
            {
                output = Console.ReadLine();
            } while (String.IsNullOrEmpty(output));
            rspWriter.Write($"--output {output} ");
            Console.WriteLine("Enter programming languages to include: c, c++, java, c#, javascript, html, css, pyton enter or all");
            string lang;
            do
            {
             lang=Console.ReadLine();
            } while (String.IsNullOrEmpty(lang));
            rspWriter.Write($"--language {lang} ");
            
            Console.WriteLine("Include source code origin as a comment? (y/n)");
            var noteInput = Console.ReadLine();
            rspWriter.Write(noteInput.Trim().ToLower() == "y" ? "--note " : "");

            Console.Write("Enter the sort order for code files ('alphabet' or 'type'): ");
            rspWriter.Write($"--sort {Console.ReadLine()} ");

            Console.WriteLine("Remove empty lines from code files? (y/n)");
            var removeEmptyLinesInput = Console.ReadLine();
            rspWriter.Write(removeEmptyLinesInput.Trim().ToLower() == "y" ? "--remove-empty-lines " : "");

            Console.Write("Enter the author`s name: ");
            rspWriter.Write($"--author {Console.ReadLine()}");
        }
        Console.WriteLine($"Response file created successfully: {responseFile.FullName}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error creating response file: {ex.Message}");
    }
});

static string[] GetExtentionsOfLanguages(string language, string[] extensions, string[] allLanguages)
{
    if (language.Equals("all"))
        return extensions;
    string[] languages = language.Split(' ');
    for (int i = 0; i < languages.Length; i++)
    {
        for (int j = 0; j < allLanguages.Length; j++)
        {
            if (languages[i].Equals(allLanguages[j]))
            {
                languages[i] = extensions[j];
                break;
            }
        }
    }
    if (languages.Length == 0)
        return extensions;
    return languages;
}

var RootCommand = new RootCommand("Root command for file bundler for CLI");
RootCommand.AddCommand(bundleCommand);
RootCommand.AddCommand(rspCommand);

RootCommand.InvokeAsync(args);