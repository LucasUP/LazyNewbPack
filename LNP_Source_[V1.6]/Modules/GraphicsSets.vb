Module GraphicsSets
    Function FindCurrentGraphics()
        'Does nothing yet
        Return 0
    End Function

    Sub FindPossibleGraphhics(ByVal directory As String)
        MainForm.GraphicsListBox.Items.Clear()
        'gets all subDirs as an Array
        Dim subDirs = findSubDirs(directory)
        For i = 0 To (subDirs.Length - 1)
            'displays only the last directory in path
            Dim dSplit = Split(subDirs(i), "\")
            MainForm.GraphicsListBox.Items.Add(dSplit(dSplit.Length - 1))
        Next
    End Sub

    Sub SwitchGraphics(ByVal gfxDir As String, ByVal dfDir As String)
        Dim click = MsgBox("Your graphics, settings and raws will be changed.", MsgBoxStyle.OkCancel, "Are you sure?")
        If click = MsgBoxResult.Ok Then
            Dim fsp = My.Computer.FileSystem
            Dim delOpt = FileIO.DeleteDirectoryOption.DeleteAllContents
            Dim err = False
            'MessageBox.Show(rootDir)
            If gfxDir <> "" And _
            fsp.DirectoryExists(gfxDir + "\raw\graphics") And _
            fsp.DirectoryExists(gfxDir + "\data\init") Then
                If fsp.DirectoryExists(dfDir) And dfDir <> "" Then
                    Try
                        'delete old graphics
                        If fsp.DirectoryExists(dfDir + "\raw\graphics") Then _
                        fsp.DeleteDirectory(dfDir + "\raw\graphics", delOpt)
                        'copy new raws
                        fsp.CopyDirectory(gfxDir + "\raw", _
                                          dfDir + "\raw", True)
                        'UpdateSaveGames(rootDir)

                        If fsp.DirectoryExists(dfDir + "\data\art") Then _
                            fsp.DeleteDirectory(dfDir + "\data\art", delOpt)
                        fsp.CopyDirectory(gfxDir + "\data\art", _
                                          dfDir + "\data\art", True)
                        fsp.CopyFile(gfxDir + "\data\init\init.txt", _
                                     dfDir + "\data\init\init.txt", True)
                        fsp.CopyFile(gfxDir + "\data\init\d_init.txt", _
                                     dfDir + "\data\init\d_init.txt", True)
                        fsp.CopyFile(gfxDir + "\data\init\colors.txt", _
                                     dfDir + "\data\init\colors.txt", True)
                    Catch ex As Exception
                        MessageBox.Show(ex.Message + vbCrLf + _
                        "Graphics folder may not have nessesary files. Graphics may not be installed correctly.")
                        Exit Sub
                    End Try
                    click = MsgBox("Graphics and settings installed!" + vbCrLf + "Would you like to update your savegames to properly use the new graphics?", MsgBoxStyle.YesNo, "Update Savegames?")
                    If click = MsgBoxResult.Yes Then UpdateSaveGames(dfDir)
                Else
                    MessageBox.Show("Cannot find: " + dfDir)
                End If
            Else
                MessageBox.Show("Nothing was installed." + vbCrLf + _
                "Folder does not exist or does not have required files or folders: " + vbCrLf + gfxDir)
            End If
        End If
    End Sub


    Sub UpdateSaveGames(ByVal dfDir As String)
        Dim fsp = My.Computer.FileSystem
        Dim delOpt = FileIO.DeleteDirectoryOption.DeleteAllContents
        'find all savegames
        Dim savegames = FileWorking.findSubDirs(dfDir + "\data\save")
        If savegames.Length > 0 Then
            'for every save
            For Each save As String In savegames
                If Not save.EndsWith("current") Then 'dont do anything to the 'current' folder
                    'delete old graphics
                    If fsp.DirectoryExists(save + "\raw\graphics") Then _
                        fsp.DeleteDirectory(save + "\raw\graphics", delOpt)
                    'copy new raws
                    fsp.CopyDirectory(dfDir + "\raw", save + "\raw", True)
                End If
            Next
            MessageBox.Show("Savegames Updated!")
        Else
            MessageBox.Show("No savegames to update.")
        End If
    End Sub

    Sub SimplifyAllGraphics(ByVal dfDir As String, ByVal gfxDir As String)
        Dim subDirs = findSubDirs(gfxDir)
        For i = 0 To (subDirs.Length - 1)
            SimplifyGraphics(dfDir, subDirs(i))
        Next
        MessageBox.Show("Simplification Complete!")
    End Sub

    'Delete all unnesesary files in graphics folders. Used for packaging
    Sub SimplifyGraphics(ByVal dfDir As String, ByVal gfxDir As String)

        Dim fsp = My.Computer.FileSystem
        Dim delOpt = FileIO.DeleteDirectoryOption.DeleteAllContents
        Dim tmp = dfDir + "\simple_tmp"

        Dim filesBefore = fsp.GetFiles(gfxDir, FileIO.SearchOption.SearchAllSubDirectories, "*.*").Count
        Dim filesAfter As Integer
        Dim filesDif As Integer

        If filesBefore = 0 Then
            MessageBox.Show("No files in: " + gfxDir)
            Exit Sub
        End If

        Try
            If fsp.DirectoryExists(tmp) Then fsp.DeleteDirectory(tmp, delOpt)
            fsp.CreateDirectory(tmp)
            fsp.CopyDirectory(gfxDir, tmp, True)

            fsp.DeleteDirectory(gfxDir, delOpt)

            fsp.CreateDirectory(gfxDir)
            fsp.CreateDirectory(gfxDir + "\data\art")
            fsp.CreateDirectory(gfxDir + "\raw\graphics")
            fsp.CreateDirectory(gfxDir + "\raw\objects")
            fsp.CreateDirectory(gfxDir + "\data\init")

            fsp.CopyDirectory(tmp + "\data\art", gfxDir + "\data\art", True)
            fsp.CopyDirectory(tmp + "\raw\graphics\", gfxDir + "\raw\graphics\", True)
            fsp.CopyDirectory(tmp + "\raw\objects\", gfxDir + "\raw\objects\", True)

            fsp.CopyFile(tmp + "\data\init\colors.txt", _
                         gfxDir + "\data\init\colors.txt", True)
            fsp.CopyFile(tmp + "\data\init\init.txt", _
                         gfxDir + "\data\init\init.txt", True)
            fsp.CopyFile(tmp + "\data\init\d_init.txt", _
                         gfxDir + "\data\init\d_init.txt", True)

            If fsp.DirectoryExists(tmp) Then fsp.DeleteDirectory(tmp, delOpt)

        Catch ex As Exception
            If fsp.DirectoryExists(tmp) Then fsp.DeleteDirectory(tmp, delOpt)
            MessageBox.Show("Problem simplifying graphics folder. It may not have the required files. " + _
            gfxDir + vbCrLf + vbCrLf + ex.Message)
            Exit Sub
        End Try
        filesAfter = fsp.GetFiles(gfxDir, FileIO.SearchOption.SearchAllSubDirectories, "*.*").Count
        filesDif = filesBefore - filesAfter
        MessageBox.Show("Deleted " + filesDif.ToString + " unnecessary files in:" + vbCrLf + gfxDir)
    End Sub

End Module
