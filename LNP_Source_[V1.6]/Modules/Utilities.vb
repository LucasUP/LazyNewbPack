Module Utilities
    Function FindAllUtilities(ByVal dir As String)

        'Load file-exlusion list
        Dim exclude = FileWorking.ReadFile("exclude.txt", dir)
        Dim excludeArray() As String = LoadingOptions.parseTagsToArray1D(exclude)

        MainForm.UtilityListBox.Items.Clear()

        Dim fsp = My.Computer.FileSystem
        'Find any .exe files in every sub-directory
        Dim exeCollection = fsp.GetFiles( _
            dir, FileIO.SearchOption.SearchAllSubDirectories, "*.exe")
        Dim jarCollection = fsp.GetFiles( _
            dir, FileIO.SearchOption.SearchAllSubDirectories, "*.jar")
        Dim fileCollection = exeCollection.Concat(jarCollection)

        Dim Utilities(fileCollection.Count - 1) As String 'Create array with size of number of files found

        'Adds each item to utility list
        Dim j = 0
        For Each item In fileCollection

            'create an array out of directory structure
            Dim dSplit = Split(item, "\")
            Dim fileName = dSplit(dSplit.Length - 1)

            'Test if filename is in exlusion list
            Dim skip = False
            For k = 0 To (excludeArray.Length - 1)
                If excludeArray(k) = fileName Then
                    skip = True
                    Exit For
                End If
            Next

            If Not skip Then
                Utilities(j) = item 'add the FULL PATH to the array (to be returned to caller)
                'Dim text = dSplit(dSplit.Length - 2) + "\" + fileName 'return parent directory and filename
                MainForm.UtilityListBox.Items.Add(item) 'add them to the list
                j = j + 1
            End If

        Next
        'Next
        Return Utilities
    End Function

End Module
