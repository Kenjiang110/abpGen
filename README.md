Using GitHub to maintain an open source project, I am a beginner, so any advice is welcome. 
This project is a tool I developed myself while using the abp vNext framework to build applications, and it serves as a plugin for VS2022.
To get started, please download the source code and build it. To build it, you will need to add the VS Extension and WPF development support for VS. 
Then, locate the Bob.Abp.AppGen.vsix file and double-click to install the plugin.
Next, create an Entity class in the Domain project. Currently, I can only support AggregateRoot<Guid> and Entity<Guid>, but you can easily modify the generated source code.

After creating the class file (e.g. Country.cs), right-click it and find the "Bob Abp Assistant" menu:
- Extract Information File: This will create a file with the name *.json (e.g. Country.cs.json)
- Edit Information File: Use the UI to modify the JSON file. You can also directly modify this file as it is easy to understand.
- Generate Codes: This will generate multiple code files for the entity, but you can choose which files to generate.
