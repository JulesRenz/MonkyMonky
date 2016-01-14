# MonkyMonky    
    Monitors a set of folders for added files. If added files are detected, the user is given the opportunity 
    to replace the filename with a chosen one and all occurences of the old filename within the file are replaced
    with the new one

    Changelog:
    v0.1.0  initial release
    v0.2.0  - The dialog can now be submitted by hitting the enter key
            - the Config file can now contain comments and empty lines
            - Now uses ANSI Encoding to read the file to allow special (german) characters.
            - Error Handling, when the file name was not changed

	Bugs ToDo:
	- Error handling, when the desired output file alraedy exists
	
	Features ToDo:
	- Replace label in main window with textbox where error messages can be displayed
	- zip exe after build so it can be committed

    