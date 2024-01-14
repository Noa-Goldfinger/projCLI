--Code Packaging Tool in C# (CLI) - Bundle Command:

As part of my programming curriculum, I identified the need for an efficient solution to simplify the process of submitting coding exercises for evaluation. When exercises involve multiple code files, the evaluation process becomes cumbersome for both the student and the instructor. To address this challenge, I developed a Command-Line Interface (CLI) tool in C# to package code from various files into a single, easily assessable document.

Bundle Command Options:

--output, -o:

Specify the desired output filename for the consolidated code document.
--language, -l:

Choose specific programming languages to include in the document.
--note, -n:

Add a note or comment to the document for additional context.
--sort, -s:

Sort files alphabetically or by file type for better organization.
--remove-empty-lines, -r:

Exclude empty lines from the consolidated document for cleaner presentation.
--author, -a:

Include the name of the file creator in the document for reference.
Additional Command:

create-rsp:
Generate a response file (.rsp) to provide users with feedback and facilitate ease of use.
