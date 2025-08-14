# delbinobj
Command-line tool that scans a specified directory for .NET
project files (.csproj) and deletes the associated bin, obj
and optionally .vs directories.

## Run arguments
### How to use?
  delbinobj [options]

### Run arguments:
1.  -?, -h, --help        Show help and usage information
1.  --version             Wywietl informacje o wersji
1.  -p, --startup-path    Directory where command will look for *.csproj file to locate bin and obj folders to delete [default: .]
1.  -r, --recursive       If present, command will scan startup directory and all directories below for *.csproj files
1.  -vs, --include-vs     If present, command will delete .vs directory from solution directory
1.  -sd, --softdelete     If present, the command will move (instead of delete) found directories to the directory specified in this argument, maintaining the relationship to the start-up path
1.  -dr, --dryrun         If present, command will not delete or move anything, just print what would processed
1.  -v, --verbose         If present, command will print verbose output to console
1.  -ui, --userinterface  If present, progress bar will be presented
1.  -l, --log-file        If present, command will log output to specified file
1.  -q, --query           If present, command will prompt for all other options

