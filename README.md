# SWUtils
Auto file/folder creating utilities for SolidWorks.

I'm not a professional developer and this my first public project. So please consider it when you come across each dummy mistake (I think they are everywhere).

This code utilises the great libraries created bu Angelsix:
https://github.com/angelsix
And I've managed to pull this off by watching his wonderful videos here:
https://www.youtube.com/watch?v=7DlG6OQeJP0&list=PLrW43fNmjaQVMN1-lsB29ECnHRlA4ebYn

It is a WPF class library. Compiled SW_Utils.DLL is an AddIn which is registered to SolidWorks by this tool:
https://github.com/angelsix/solidworks-api/tree/develop/Tools/Addin%20Install
After the registration, an additional tab appears in the right side menu of SolidWorks.

Functions for Now:
Can create a properly numbered sub folder in one of the specified working directories with a single click.
Can create a properly numbered Assembly File, acordingly numbered Part File, insert the part into the assembly, save everything and open up the Part File to draw.
Can properly rename and Pack&Go a draft assembly into the next draft folder.
Can Save an .STL file everytime the file is saved.
