Module Keybindings
    Sub FindAllKeybindings(ByVal dir As String)
        Try
            Dim fsp = My.Computer.FileSystem
            MainForm.KeyBindingList.Items.Clear()
            Dim fileCollection = fsp.GetFiles( _
                    dir, FileIO.SearchOption.SearchTopLevelOnly, "*.txt")
            'Adds keybinding to list
            For Each item In fileCollection
                Dim dSplit = Split(item, "\")
                Dim text = dSplit(dSplit.Length - 1)
                MainForm.KeyBindingList.Items.Add(text)
            Next
        Catch ex As Exception
        End Try
    End Sub

    Sub LoadKeybinding(ByVal fileName As String, ByVal kDir As String, ByVal dfDir As String)
        Dim fsp = My.Computer.FileSystem
        If fileName <> "" Then
            Try
                fsp.CopyFile(kDir + "\" + fileName, _
                             dfDir + "\data\init\interface.txt", True)
                MessageBox.Show(fileName + " loaded!")
            Catch ex As Exception
                MessageBox.Show(ex.Message)
            End Try
        Else
            MessageBox.Show("No file selected")
        End If

    End Sub

    Sub SaveKeybinding(ByVal keyDir As String, ByVal dfDir As String)
        Dim fsp = My.Computer.FileSystem

        Dim input = ""
        Dim matching = "" 'if theres a matching file
        input = InputBox("Save current keybindings as:", "Save Keybindings")

        'checks if input is empty,
        If input <> "" Then
            'Checks if same as another item.
            For i = 0 To (MainForm.KeyBindingList.Items.Count - 1)
                Dim toMatch = MainForm.KeyBindingList.Items(i).ToString
                If toMatch.Equals(input + ".txt", StringComparison.OrdinalIgnoreCase) Then
                    matching = toMatch
                End If
            Next

            If matching <> "" Then
                Dim a = MsgBox("Overwrite " + matching + "?", MsgBoxStyle.YesNo, "Overwrite file?")
                If a = MsgBoxResult.Yes Then
                    fsp.CopyFile(dfDir + "\data\init\interface.txt", _
                                 keyDir + "\" + input + ".txt", True)
                End If
            Else
                fsp.CopyFile(dfDir + "\data\init\interface.txt", _
                                 keyDir + "\" + input + ".txt", True)
            End If
        End If
        FindAllKeybindings(keyDir) 'refresh list
    End Sub

    Sub DeleteKeybinding(ByVal filename As String, ByVal dir As String)
        If filename <> "" Then
            Dim fsp = My.Computer.FileSystem
            Dim file = dir + "\" + filename
            Dim a = MsgBox("Are you sure you want to delete " + filename + "?", MsgBoxStyle.YesNo, "Delete file?")
            If a = MsgBoxResult.Yes Then
                If fsp.FileExists(file) Then fsp.DeleteFile(file)
            End If
            FindAllKeybindings(dir) 'refresh list
        Else
            MessageBox.Show("No file selected")
        End If
    End Sub

End Module
