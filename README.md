## Usage:  
```
HomemadeChocolateChip.exe <debugport> <killchrome> 
         debugurl: The port which chrome debugger will utilize (eg 9444)  
         killchrome: Terminate current chrome processes and restart them with debug enabled. [yes/no] 
```
To dos:  
- Add a command to list running chrome tabs/sites, so we can monitor for a specific site and then dump
- Add 'continuous parsing' and writing to an output file
- Add a command to specify which Chrome profile of the user to launch. Currently it launches the default profile.
- Explore interaction with Chrome extensions through the debug port
