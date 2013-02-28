'imports Regex class, which can do Regular Expressions
' http://msdn.microsoft.com/en-us/library/taz3ak2f(v=VS.90).aspx
' http://msdn.microsoft.com/en-us/library/az24scfc(v=VS.90).aspx
Imports System.Text.RegularExpressions

Module FileWorking

    Function findDFDir()
        Dim search = "Dwarf Fortress*"
        Dim directory = My.Computer.FileSystem.CurrentDirectory() 'Programs current dir
        Dim dirCollection = My.Computer.FileSystem.GetDirectories( _
                            directory, _
                            FileIO.SearchOption.SearchTopLevelOnly, _
                            search) 'Searches directories in current dir
        If dirCollection.Any Then 'if it found one
            Dim dfDir = dirCollection.ElementAt(0) 'return first directory
            'MessageBox.Show(dfDir)
            Return dfDir
        End If
        MessageBox.Show("Cannot find Dwarf Fortress directory")
        MainForm.Close()
        Return ""
    End Function

    'Runs a file/program
    Sub RunFile(ByVal fileName As String, Optional ByVal path As String = "")
        Dim tmpString = ""
        Try 'try/catch errors
            tmpString = fileName
            If path <> "" Then tmpString = path + "\" + fileName
            Process.Start(tmpString)
            'System.Diagnostics.Process.Start(tmpString)
            'Shell(tmpString, AppWinStyle.NormalFocus)
        Catch ex As Exception
            'if theres a error loading the file, tell the user
            MessageBox.Show("RunFile error: " + tmpString + vbCrLf + ex.Message)
        End Try

    End Sub

    'runs a program from its root directory by creating and calling a batch file.
    'Legit programmers are probably going to cringe at this. But it is the only solution i've personally found to stop certain errors from occuring when running Dwarf Fortress and other utlities.
    Sub RunFileByBatch(ByVal fileName As String, Optional ByVal path As String = "")

        'if only passed fileName, assume it is a full path, and seperate the filename from the directory
        If path = "" Then
            path = fileName
            Dim dSplit = Split(path, "\") 'create an array out of directory structure
            fileName = dSplit(dSplit.Length - 1) 'only return last item (will be the file name)
            path = path.Substring(0, path.Length - fileName.Length) 'returns the path (before the filename)
        End If

        'creates a batch file in the same directory as the program we are trying to launch. This batch file will launch our target program and then delete itself.
        Dim bName = "runProgram.bat"
        Dim batch = _
        "@ECHO off" + vbCrLf + _
        "SETLOCAL" + vbCrLf + _
        "CD %~dp0" + vbCrLf + _
        "START """" " + """" + fileName + """" + vbCrLf + _
        "start /min cmd /c del /q %~s0" 'deletes self afterwards

        FileWorking.SaveFile(bName, path, batch) 'create the batch file
        FileWorking.RunFile(bName, path) 'run the batch file

    End Sub


    'Returns string from text file
    Function ReadFile(ByVal filename As String, ByVal place_in_df As String)
        Dim textfile As String = ""
        Dim filepath As String = place_in_df + "\" + filename

        'Makes sure file exists
        If System.IO.File.Exists(filepath) Then
            'objreader will read specified file
            Dim objreader As New System.IO.StreamReader(filepath)
            'read contents of file specified in objreader and store string to textfile variable
            textfile = objreader.ReadToEnd
            objreader.Close()
            'functions can return values where subs cannot
            If textfile = "" Then MessageBox.Show(filepath + " is EMPTY!")
            Return textfile
        Else
            MessageBox.Show("ReadFile error. Cannot find file: " + filepath)
            Return ""
        End If
    End Function


    Sub SaveFile(ByVal filename As String, ByVal place_in_df As String, ByVal text_to_save As String)
        Dim fsp = My.Computer.FileSystem
        Try
            If text_to_save <> "" Then
                Dim filepath As String = place_in_df + "\" + filename
                'objwriter will write to specified file
                Dim objwriter As New System.IO.StreamWriter(filepath)
                'save text_to_save to the file (filepath)
                objwriter.Write(text_to_save)
                objwriter.Close()
            Else
                MsgBox("Tried to save an empty file...")
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

    End Sub


    'pass a tag like "ECONOMY" to find [ECONOMY:*] and change to [ECONOMY:newValue]
    '   where * can be anything
    Sub ReplaceTag(ByRef fileText As String, ByVal tag As String, _
                    ByVal newValue As String)
        'creates regular expression pattern. ".+" finds any number of characters. "\[" just finds a "["
        'ie pattern = "\[ECONOMY:.+\]"
        Dim pattern As String = "\[" + tag + ":.*\]"
        Dim newTag As String = "[" + tag + ":" + newValue + "]"
        fileText = Regex.Replace(fileText, pattern, newTag)
    End Sub



    'standard find-replace. Currently just a re-structure of the Replace function
    Sub ReplaceText(ByRef fileText As String, ByVal findText As String, _
                    ByVal replaceText As String)
        fileText = Replace(fileText, findText, replaceText)
    End Sub


    'find-replace using regular expression patterns
    Sub ReplaceTextPattern(ByRef fileText As String, ByVal pattern As String, _
                    ByVal replaceText As String)
        fileText = Regex.Replace(fileText, pattern, replaceText)
    End Sub


    'does multiple finds-replaces in multiple files and SAVES THEM
    Sub ReplaceInMultipleFiles(ByVal fileNameArray As Array, _
                               ByVal directory As String, _
                               ByVal findList As Array, _
                               ByVal replaceList As Array)

        Dim num = fileNameArray.Length - 1 'number of files in fileNameArray
        Dim fileArray(num) As String 'create array of same length to store loaded files

        'For every file in fileNameArray
        For i = 0 To (num)
            If fileNameArray(i) <> "" Then
                'load file i into fileArray
                fileArray(i) = FileWorking.ReadFile(fileNameArray(i), directory)
                'in current file, find/replace all strings in findList/replaceList
                For j = 0 To (findList.Length - 1)
                    fileArray(i) = Replace(fileArray(i), findList(j), replaceList(j))
                Next
                'save files
                FileWorking.SaveFile(fileNameArray(i), directory, fileArray(i))
            End If
        Next

    End Sub


    'Find sub-directories in a directory
    Public Function findSubDirs(ByVal directory As String)
        If My.Computer.FileSystem.DirectoryExists(directory) Then
            Try
                Dim dirCollection = My.Computer.FileSystem.GetDirectories(directory, _
                                    FileIO.SearchOption.SearchTopLevelOnly)
                Dim dirArray(dirCollection.Count - 1)
                Dim i = 0
                For Each Item In dirCollection
                    dirArray(i) = dirCollection.Item(i)
                    i = i + 1
                Next
                Return dirArray
            Catch ex As Exception
            End Try
        End If
        Dim empty() As String = {}
        Return empty
    End Function


    'Does a regex text search inside of multiple files and returns true if found. Case-insensitive
    Public Function regexSearchInFiles(ByVal search As String, _
                                       ByVal fileNames() As String, _
                                       ByVal directory As String)
        Dim file As String
        Dim regex As Regex = New Regex(search, RegexOptions.IgnoreCase) 'define regular expression search
        For i = 0 To (fileNames.Length - 1)
            file = FileWorking.ReadFile(fileNames(i), directory) 'load a file into a string variable
            If regex.Match(file).Success Then Return True 'return true if found
        Next
        Return False
    End Function

    'Does a case-sensitive simple text search inside of multiple files and returns true if found
    Public Function searchInFiles(ByVal search As String, _
                                 ByVal fileNames() As String, _
                                 ByVal directory As String)
        Dim file As String
        For i = 0 To (fileNames.Length - 1)
            file = FileWorking.ReadFile(fileNames(i), directory) 'load a file into a string variable
            If file.Contains(search) Then Return True 'return true if found
        Next
        Return False
    End Function

End Module

