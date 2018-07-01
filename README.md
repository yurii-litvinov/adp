# Automatic Diploma Processor

A tool for processing various kinds of quailification works for Software Engineering chair of St. Petersburg State University. 
It collects available information from files with diploma text, slides, reviews and then generates index page for SE site. Advanced version of
https://github.com/yurii-litvinov/Tools/blob/master/generateBachelorsThesisLinks.py. It supports bachelor qualification works, 3rd and 2nd course
semester works.

See http://se.math.spbu.ru/SE/diploma/2018/index for example of output and https://www.dropbox.com/sh/3u006fymy3if5sw/AACLMwZ0HTis7PGCvbLMNGCoa?dl=0
for example of possible input.

## Usage

* Build this project using .NET Core (>= 2.0) using `dotnet publish -r win10-x64`.
* Add `bin\netcoreapp2.0\win10-x64` to your `PATH` variable.
* Collect your qualification work files in one folder, make surethat they are named as 
  `<group>-<student name>-<report|slides|review|advisor-review|reviewer-review>.pdf`
  For example, `444-Ololoev-advisor-review.pdf`.
* Run `adp` in that folder. Four files will be generated:
  * `advisors.json` contains editable info about scientific advisors. Each record consists of pattern against which each advisor name from .pdf files will be
     matched and a string with correct name and title of the advisor. Advisors matched against pattern will be replaced by advisor name from this .json in output 
     file, so it is a place where you can manually specify advisor name and title if needed, overriding info extracted from .pdf documents, which in maby cases
     is incorrect since students frequently don't have any ideas about scientific degrees.
  * `config.json` contains generation configuration settings. Right now it contains only target folder on a server where diploma documents will be stored, to 
     correctly generate relative links from index page to diploma files.
  * `works.json` contains metainformation about qualification works extracted from files. It needs to be reviewed, corrected and extended with additional info 
     that is not provided within the files, like source code link and GitHub account name of a student.
  * `out.html` is a preliminary version of generated index file (or a part of it), to be copypasted into a chair server.
* Review and manually edit `works.json` and `advisors.json` if needed.
* Run `adp` again so that it will regenerate `out.html` using provided info from `works.json` and `advisors.json`.
* Upload all diploma files into the correct folder of SE chair server (using BitKinex, for example).
* Copypaste results from `out.html` into the relevant page on SE chair server, creating new page if needed.