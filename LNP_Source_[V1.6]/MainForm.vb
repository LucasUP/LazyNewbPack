'Lazy Newb Pack. Unnoficial GUI for the game Dwarf Fortress>
'Copyright (C) 2012 Lucas Paquette (LucasUP)

'This program is free software: you can redistribute it and/or modify
'it under the terms of the GNU General Public License as published by
'the Free Software Foundation, either version 3 of the License, or
'(at your option) any later version.

'This program is distributed in the hope that it will be useful,
'but WITHOUT ANY WARRANTY; without even the implied warranty of
'MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
'GNU General Public License for more details.

'You should have received a copy of the GNU General Public License
'along with this program.  If not, see <http://www.gnu.org/licenses/>.

Public Class MainForm

    'gets version number from ap
    Dim version = My.Application.Info.Version.Major.ToString + "." + My.Application.Info.Version.Minor.ToString

    'file created when extras installed
    Dim installFile = "LNP" + version + ".txt"

    'Store directories
    Const CInitDir = "\data\init"
    Const CRawObjectsDir = "\raw\objects"
    Const CSaveDir = "\data\save"

    Const lnpD = "LNP"
    Const graphicsD = lnpD + "\Graphics"
    Const extrasD = lnpD + "\Extras"
    Const keybindsD = lnpD + "\Keybinds"
    Const settingsD = lnpD + "\Defaults"

    Const utilityD = lnpD + "\Utilities"

    'dfDir + Constant will be concatonated in these once dfDir is found
    Dim dfDir 'to be found later
    Dim initDir 'dfDir + CInitDir
    Dim rawObjectsDir 'dfDir + CRawObjectsDir
    Dim saveDir

    'Array holding all program paths
    Dim UtilityList

    'Store contents of text files
    Dim init As String
    Dim d_init As String

    'Will store loaded tag/value pairs in 2D array form
    Dim tagArray(200, 1) As String

    'INPUT FORM DATA
    Dim inStuff As String
    Dim inStuff2 As String

    'BASIC OPTIONS
    Dim popCap = "200"
    Dim childCap = "100:1000"

    Dim invaders As Boolean = True
    Dim temperature As Boolean = True
    Dim weather As Boolean = True
    Dim caveins As Boolean = True
    Dim liquidDepths As Boolean = True
    Dim variedGround As Boolean = True
    Dim laborLists As String = "SKILLS"
    Dim aquifers As Boolean = True
    'Dim exotics As Boolean = True
    'Dim economy As Boolean = False

    'GRAPHICS OPTIONS
    Dim trueType As Boolean = False

    'ADVANCED OPTIONS
    'init
    Dim sound As Boolean = True
    Dim volume = "255"
    Dim introMovie As Boolean = True
    Dim startWindowed As String = "YES" 'Can be YES, NO or PROMPT.
    Dim fpsCounter As Boolean = False
    Dim fpsCap = "100"
    Dim gpsCap = "50"
    Dim procPriority As String = "NORMAL" 'Can be REALTIME, HIGH, ABOVE_NORMAL, NORMAL, BELOW_NORMAL and IDLE.
    Dim compressSaves As Boolean = True
    'd_init
    Dim autoSave As String = "SEASONAL" 'can be NONE, SEASONAL or YEARLY.
    Dim autoBackup As Boolean = True
    Dim autoSavePause As Boolean = True
    Dim initialSave As Boolean = True
    Dim pauseOnLoad As Boolean = True
    ''TO ADD?
    '[TRUETYPE] 'boolean
    '[PRINT_MODE] 'CAN BE MANY THINGS

    'Is set true once form has loaded properly
    Dim FormLoaded As Boolean = False


    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        UpdateDirectories()
        If dfDir <> "" Then
            LoadAll()
            SetToolText()
            FormLoaded = True
            InstallExtras() 'install extras if first run
            Keybindings.FindAllKeybindings(keybindsD) 'find keybinds
            GraphicsSets.FindPossibleGraphhics(graphicsD) 'and graphics
            Try
                FileOpen(1, utilityD & "\utilitylist.txt", OpenMode.Input)
                Dim i As Integer = 0
                Do Until EOF(1)
                    UtilityListBox.Items.Add(LineInput(1))
                    If LineInput(1) = 1 Then
                        UtilityListBox.SetItemChecked(i, True)
                    End If
                    i = +1
                Loop
            Catch ex As Exception
                UtilityList = Utilities.FindAllUtilities(utilityD) 'and utilities
            End Try
            FileClose(1)
        End If

    End Sub

    Private Sub UpdateDirectories()
        dfDir = findDFDir() 'attempt to find root DF Directory
        initDir = dfDir + CInitDir
        rawObjectsDir = dfDir + CRawObjectsDir
        saveDir = dfDir + CSaveDir
    End Sub

    Public Sub LoadAll()
        'load variables with contents of init files
        init = FileWorking.ReadFile("init.txt", initDir)
        d_init = FileWorking.ReadFile("d_init.txt", initDir)

        If init <> "" And d_init <> "" Then
            tagArray = LoadingOptions.parseTagsToArray(init + d_init) 'concatonate both files and parse their tags into an array

            'economy = LoadingOptions.loadTagAsBool("ECONOMY", tagArray)
            invaders = LoadingOptions.loadTagAsBool("INVADERS", tagArray)
            temperature = LoadingOptions.loadTagAsBool("TEMPERATURE", tagArray)
            weather = LoadingOptions.loadTagAsBool("WEATHER", tagArray)
            caveins = LoadingOptions.loadTagAsBool("CAVEINS", tagArray)
            liquidDepths = LoadingOptions.loadTagAsBool("SHOW_FLOW_AMOUNTS", tagArray)
            popCap = LoadingOptions.loadTag("POPULATION_CAP", tagArray)
            childCap = LoadingOptions.loadTag("BABY_CHILD_CAP", tagArray)
            variedGround = LoadingOptions.loadTagAsBool("VARIED_GROUND_TILES", tagArray)
            laborLists = LoadingOptions.loadTag("SET_LABOR_LISTS", tagArray)

            aquifers = LoadingOptions.loadAquifer(rawObjectsDir)
            'exotics = LoadingOptions.loadExotics(rawObjectsDir)

            trueType = LoadingOptions.loadTagAsBool("TRUETYPE", tagArray)

            sound = LoadingOptions.loadTagAsBool("SOUND", tagArray)
            volume = LoadingOptions.loadTag("VOLUME", tagArray)

            introMovie = LoadingOptions.loadTagAsBool("INTRO", tagArray)
            startWindowed = LoadingOptions.loadTag("WINDOWED", tagArray)
            fpsCounter = LoadingOptions.loadTagAsBool("FPS", tagArray)
            fpsCap = LoadingOptions.loadTag("FPS_CAP", tagArray)
            gpsCap = LoadingOptions.loadTag("G_FPS_CAP", tagArray)
            procPriority = LoadingOptions.loadTag("PRIORITY", tagArray)
            compressSaves = LoadingOptions.loadTagAsBool("COMPRESSED_SAVES", tagArray)

            autoSave = LoadingOptions.loadTag("AUTOSAVE", tagArray)
            autoBackup = LoadingOptions.loadTagAsBool("AUTOBACKUP", tagArray)
            autoSavePause = LoadingOptions.loadTagAsBool("AUTOSAVE_PAUSE", tagArray)
            initialSave = LoadingOptions.loadTagAsBool("INITIAL_SAVE", tagArray)
            pauseOnLoad = LoadingOptions.loadTagAsBool("PAUSE_ON_LOAD", tagArray)

            UpdateButtonText()
        Else
            Me.Close()
        End If
    End Sub

    Private Sub UpdateButtonText()
        'EconomyButton.Text = "Economy: " + booleanToYesNo(economy)
        InvadersButton.Text = "Invaders: " + booleanToYesNo(invaders)
        TemperatureButton.Text = "Temperature: " + booleanToYesNo(temperature)
        LiquidDepthButton.Text = "Liquid Depth: " + booleanToYesNo(liquidDepths)
        WeatherButton.Text = "Weather: " + booleanToYesNo(weather)
        CaveInButton.Text = "Cave-ins: " + booleanToYesNo(caveins)
        PopCapButton.Text = "Population Cap: " + popCap
        ChildCapButton.Text = "Child Cap: " + childCap
        VariedGroundButton.Text = "Varied Ground: " + booleanToYesNo(variedGround)
        LaborButton.Text = "Starting Labors: " + laborLists

        AquiferButton.Text = "Aquifers: " + booleanToYesNo(aquifers)
        'ExoticButton.Text = "Exotic Animals: " + booleanToYesNo(exotics)

        TrueTypeButton.Text = "TrueType Fonts: " + booleanToYesNo(trueType)

        SoundButton.Text = "Sound: " + booleanToYesNo(sound)
        VolumeBox.Text = volume

        IntroMovieButton.Text = "Intro Movie: " + booleanToYesNo(introMovie)
        StartWindowedButton.Text = "Windowed: " + startWindowed
        FPSCounterButton.Text = "FPS Counter: " + booleanToYesNo(fpsCounter)
        FPS_Capper.Text = fpsCap
        GFPS_Capper.Text = gpsCap
        ProcessPriorityButton.Text = "Processor Priority: " + procPriority

        AutoSaveButton.Text = "Autosave: " + autoSave
        AutosavePauseButton.Text = "Pause on Save: " + booleanToYesNo(autoSavePause)
        InitialSaveButton.Text = "Initial Save: " + booleanToYesNo(initialSave)
        PauseOnLoadButton.Text = "Pause on Load: " + booleanToYesNo(pauseOnLoad)
        CompressSavesButton.Text = "Compress Saves: " + booleanToYesNo(compressSaves)
        AutoBackupButton.Text = "Backup Saves: " + booleanToYesNo(autoBackup)

    End Sub

    Private Sub SetToolText()
    End Sub

    Public Sub SaveAll()
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        FileWorking.SaveFile("init.txt", initDir, init)
    End Sub

    Sub InstallExtras()
        Dim fsp = My.Computer.FileSystem
        'check if installFile (lnpX.X.txt) already exists
        If fsp.FileExists(dfDir + "\" + installFile) Then
            'MessageBox.Show("Already installed")
        Else
            Try
                'copy files from extras directory to DF directory
                fsp.CopyDirectory(extrasD, dfDir, True)
                Dim Day = My.Computer.Clock.LocalTime.Day.ToString
                Dim Month = My.Computer.Clock.LocalTime.Month.ToString
                Dim Year = My.Computer.Clock.LocalTime.Year.ToString
                Dim text = "Lazy Newb Pack V" + version + " extras installed!" + vbCrLf _
                + "Date (D/M/Y): " + Day + "/" + Month + "/" + Year
                MessageBox.Show(text)
                'create installFile with above text
                FileWorking.SaveFile(installFile, dfDir, text)
            Catch ex As Exception
            End Try
        End If
    End Sub


    Public Function GetDFDir()
        Return dfDir
    End Function


    'BASIC OPTIONS FUNCTIONS

    Private Sub PopCapButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PopCapButton.Click
        'ask user for input
        inStuff = InputBox("Population cap:", "Settings", popCap)
        If (inStuff <> "") Then popCap = inStuff
        ChangingOptions.stringTagSet("POPULATION_CAP", popCap, d_init)
        'Save d_init file
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub ChildCapButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChildCapButton.Click
        'ask user for input for childCapMax and childCapPercent
        Dim tSplit = Split(childCap, ":")
        inStuff = InputBox("Absolute cap on babies + children:", "Settings", tSplit(0))
        inStuff2 = InputBox("Max percentage of children in fort:" + vbCrLf _
                            + "(lowest of the two values will be used as the cap)", "Settings", tSplit(1))
        If (inStuff <> "") And (inStuff2 <> "") Then
            childCap = inStuff + ":" + inStuff2
        End If
        'Change Child Cap
        ChangingOptions.stringTagSet("BABY_CHILD_CAP", childCap, d_init)
        'Save d_init file
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub InvadersButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InvadersButton.Click
        invaders = Not invaders

        'send tag we want to change ("INVADERS"), invaders value (true/false)
        ' and the variable (d_init) with the text we want to change
        ChangingOptions.booleanTagSet("INVADERS", invaders, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub TemperatureButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TemperatureButton.Click
        temperature = Not temperature
        ChangingOptions.booleanTagSet("TEMPERATURE", temperature, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub WeatherButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles WeatherButton.Click
        weather = Not weather
        ChangingOptions.booleanTagSet("WEATHER", weather, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub CaveinsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CaveInButton.Click
        caveins = Not caveins
        ChangingOptions.booleanTagSet("CAVEINS", caveins, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub LiquidDepthButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LiquidDepthButton.Click
        liquidDepths = Not liquidDepths
        ChangingOptions.booleanTagSet("SHOW_FLOW_AMOUNTS", liquidDepths, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub


    Private Sub VariedGroundButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VariedGroundButton.Click
        variedGround = Not variedGround
        ChangingOptions.booleanTagSet("VARIED_GROUND_TILES", variedGround, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub LaborButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LaborButton.Click
        Dim values() As String = {"SKILLS", "BY_UNIT_TYPE", "NO"}
        ChangingOptions.toggle(laborLists, values)
        ChangingOptions.stringTagSet("SET_LABOR_LISTS", laborLists, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub


    Private Sub AquiferButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AquiferButton.Click
        aquifers = Not aquifers
        ChangingOptions.aquifers(aquifers, rawObjectsDir)
        UpdateButtonText()
    End Sub




    'ADVANCED OPTIONS FUNCTIONS

    Private Sub SoundButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SoundButton.Click
        sound = Not sound
        ChangingOptions.booleanTagSet("SOUND", sound, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub

    Private Sub VolumeBox_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VolumeBox.TextChanged
        If FormLoaded Then
            volume = VolumeBox.Text
            ChangingOptions.stringTagSet("VOLUME", volume, init)
            FileWorking.SaveFile("init.txt", initDir, init)
        End If
    End Sub

    Private Sub IntroMovieButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles IntroMovieButton.Click
        introMovie = Not introMovie
        ChangingOptions.booleanTagSet("INTRO", introMovie, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub

    'Dim startWindowed As String = "YES" 'Can be YES, NO or PROMPT.
    Private Sub StartWindowedButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartWindowedButton.Click
        Dim values() As String = {"YES", "NO", "PROMPT"}
        ChangingOptions.toggle(startWindowed, values)
        ChangingOptions.stringTagSet("WINDOWED", startWindowed, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub

    'Dim fpsCounter
    Private Sub FPSCounterButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FPSCounterButton.Click
        fpsCounter = Not fpsCounter
        ChangingOptions.booleanTagSet("FPS", fpsCounter, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub

    Private Sub GFPS_Capper_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GFPS_Capper.TextChanged
        If FormLoaded Then
            gpsCap = GFPS_Capper.Text
            ChangingOptions.stringTagSet("G_FPS_CAP", gpsCap, init)
            FileWorking.SaveFile("init.txt", initDir, init)
        End If
    End Sub

    Private Sub FPS_Capper_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FPS_Capper.TextChanged
        If FormLoaded Then
            fpsCap = FPS_Capper.Text
            ChangingOptions.stringTagSet("FPS_CAP", fpsCap, init)
            FileWorking.SaveFile("init.txt", initDir, init)
        End If
    End Sub

    'Can be REALTIME, HIGH, ABOVE_NORMAL, NORMAL, BELOW_NORMAL and IDLE.
    Private Sub ProcessPriorityButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ProcessPriorityButton.Click
        Dim values() As String = _
        {"REALTIME", "HIGH", "ABOVE_NORMAL", "NORMAL", "BELOW_NORMAL", "IDLE"}
        ChangingOptions.toggle(procPriority, values)
        ChangingOptions.stringTagSet("PRIORITY", procPriority, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub

    'can be NONE, SEASONAL or YEARLY.
    Private Sub AutoSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoSaveButton.Click
        Dim values() As String = _
        {"NONE", "SEASONAL", "YEARLY"}
        ChangingOptions.toggle(autoSave, values)
        ChangingOptions.stringTagSet("AUTOSAVE", autoSave, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub AutosavePauseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutosavePauseButton.Click
        autoSavePause = Not autoSavePause
        ChangingOptions.booleanTagSet("AUTOSAVE_PAUSE", autoSavePause, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub CompressSavesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CompressSavesButton.Click
        compressSaves = Not compressSaves
        ChangingOptions.booleanTagSet("COMPRESSED_SAVES", compressSaves, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub

    Private Sub AutoBackupButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoBackupButton.Click
        autoBackup = Not autoBackup
        ChangingOptions.booleanTagSet("AUTOBACKUP", autoBackup, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub InitialSaveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InitialSaveButton.Click
        initialSave = Not initialSave
        ChangingOptions.booleanTagSet("INITIAL_SAVE", initialSave, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub

    Private Sub PauseOnLoadButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PauseOnLoadButton.Click
        pauseOnLoad = Not pauseOnLoad
        ChangingOptions.booleanTagSet("PAUSE_ON_LOAD", pauseOnLoad, d_init)
        FileWorking.SaveFile("d_init.txt", initDir, d_init)
        UpdateButtonText()
    End Sub


    'GRAPHICS

    Private Sub ChangeGraphicsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChangeGraphicsButton.Click
        If GraphicsListBox.SelectedItem <> "" Then
            Dim gfxDir = graphicsD + "\" + GraphicsListBox.SelectedItem
            GraphicsSets.SwitchGraphics(gfxDir, dfDir)
            LoadAll()
        Else
            MsgBox("Select a graphics set from the list first!", MsgBoxStyle.Information, "Oops!")
        End If
    End Sub

    Private Sub UpdateSaveGamesButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateSaveGamesButton.Click
        GraphicsSets.UpdateSaveGames(dfDir)
    End Sub

    Private Sub UpdateGraphicsListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateGraphicsListButton.Click
        GraphicsSets.FindPossibleGraphhics(graphicsD)
    End Sub

    Private Sub SimplifyGraphicsButtons_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SimplifyGraphicsButtons.Click
        SimplifyAllGraphics(dfDir, graphicsD)
    End Sub


    '[TRUETYPE]
    Private Sub TrueTypeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TrueTypeButton.Click
        trueType = Not trueType
        ChangingOptions.booleanTagSet("TRUETYPE", trueType, init)
        FileWorking.SaveFile("init.txt", initDir, init)
        UpdateButtonText()
    End Sub



    'UTILITIES

    Private Sub UpdateUtilsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UpdateUtilsButton.Click
        UtilityList = Utilities.FindAllUtilities(utilityD)
    End Sub

    Private Sub RunProgramButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RunProgramButton.Click
        RunSelectedUtility()
    End Sub

    Private Sub UtilityListBox_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        RunSelectedUtility()
    End Sub

    Private Sub RunSelectedUtility()
        Dim pItem = UtilityListBox.SelectedIndex
        If pItem >= 0 Then
            'FileWorking.RunFile(UtilityList(pItem))
            FileWorking.RunFileByBatch(UtilityList(pItem))
        End If
    End Sub



    'MENU ITEMS

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AboutToolStripMenuItem.Click
        AboutBox1.Show()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub ReloadParamSetToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReloadParamSetToolStripMenuItem.Click
        LoadAll()
    End Sub

    Private Sub ResaveParamSetToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ResaveParamSetToolStripMenuItem.Click
        SaveAll()
    End Sub

    Private Sub HowToUseToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HowToUseToolStripMenuItem.Click
        MsgBox("Press Button. Obtain Lazy.", MsgBoxStyle.Information, "How to Use")
    End Sub

    Private Sub PlayDFButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PlayDFButton.Click

        RunFileByBatch("Dwarf Fortress.exe", dfDir)


        'Run utilities on launch - daveralph1234
        For i = 0 To UtilityListBox.Items.Count - 1
            System.Threading.Thread.Sleep(1000) 'wait 1 second between programs
            If CheckState.Checked Then
                Try
                    Shell(UtilityListBox.Items(i).ToString)
                Catch ex As Exception
                End Try
            End If
        Next



        'FileWorking.RunFile("runDF.bat", lnpD)
        'FileWorking.RunFile("Dwarf Fortress.exe", dfDir)
    End Sub

    Private Sub DwarfFortressToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DwarfFortressToolStripMenuItem.Click
        RunFileByBatch("Dwarf Fortress.exe", dfDir)
    End Sub



    'OPENING FOLDERS

    Private Sub OpenSaveGameButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenSaveGameButton.Click
        FileWorking.RunFile("", saveDir)
    End Sub
    Private Sub SavegameFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SavegameFolder.Click
        FileWorking.RunFile("", saveDir)
    End Sub
    Private Sub OpenUtilityFolderButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenUtilityFolderButton.Click
        FileWorking.RunFile("", utilityD)
    End Sub
    Private Sub UtilitiesFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UtilitiesFolder.Click
        FileWorking.RunFile("", utilityD)
    End Sub

    Private Sub OpenGraphicsFolderButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenGraphicsFolderButton.Click
        FileWorking.RunFile("", graphicsD)
    End Sub
    Private Sub GraphicsFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GraphicsFolder.Click
        FileWorking.RunFile("", graphicsD)
    End Sub

    Private Sub DwarfFortressFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DwarfFortressFolder.Click
        FileWorking.RunFile("", dfDir)
    End Sub

    Private Sub MainFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MainFolder.Click
        FileWorking.RunFile("", My.Application.Info.DirectoryPath)
    End Sub

    Private Sub LNPFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LNPFolder.Click
        FileWorking.RunFile("", lnpD)
    End Sub

    Private Sub InitFolder_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InitFolder.Click
        FileWorking.RunFile("", initDir)
    End Sub



    'KEYBINDING BUTTONS

    Private Sub RefreshKBButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshKBButton.Click
        Keybindings.FindAllKeybindings(keybindsD)
    End Sub

    Private Sub SaveKeyBindingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveKeyBindingButton.Click
        Keybindings.SaveKeybinding(keybindsD, dfDir)
    End Sub

    Private Sub LoadKeyBindingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadKeyBindingButton.Click
        Keybindings.LoadKeybinding(KeyBindingList.SelectedItem, _
                                   keybindsD, dfDir)
    End Sub

    Private Sub DeleteKeyBindingButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DeleteKeyBindingButton.Click
        Keybindings.DeleteKeybinding(KeyBindingList.SelectedItem, _
                                     keybindsD)
    End Sub


    Private Sub InitEditorButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InitEditorButton.Click
        InitEditor.Show()
    End Sub

    Private Sub InitTextEditorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles InitTextEditorToolStripMenuItem.Click
        InitEditor.Show()
    End Sub

    Private Sub DefaultsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DefaultsButton.Click
        Dim a = MsgBox("Are you sure? ALL SETTINGS will be reset to game defaults." + vbCrLf + _
                       "You may need to re-install graphics afterwards.", MsgBoxStyle.OkCancel, "Reset all settings to Defaults?")
        If a = MsgBoxResult.Ok Then
            init = FileWorking.ReadFile("init.txt", settingsD)
            d_init = FileWorking.ReadFile("d_init.txt", settingsD)
            SaveAll()
            LoadAll()
            MessageBox.Show("All settings reset to defaults!")
        End If
    End Sub



    'LINKS

    Private Sub DFHomepageToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DFHomepageToolStripMenuItem.Click
        RunFile("http://www.bay12games.com/dwarves/")
    End Sub

    Private Sub DFWikiToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DFWikiToolStripMenuItem.Click
        RunFile("http://df.magmawiki.com/")
    End Sub

    Private Sub DFForumsToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DFForumsToolStripMenuItem.Click
        RunFile("http://www.bay12forums.com/smf/")
    End Sub

    Private Sub LNPForumThreadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LNPForumThreadToolStripMenuItem.Click
        RunFile("http://www.bay12forums.com/smf/index.php?topic=59026.0")
    End Sub




    'DEPRECATED

    'Private Sub ExoticButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    exotics = Not exotics
    '    ChangingOptions.exotics(exotics, rawObjectsDir)
    '    UpdateButtonText()
    'End Sub

    'Private Sub EconomyButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
    '    economy = Not economy
    '    ChangingOptions.booleanTagSet("ECONOMY", economy, d_init)
    '    FileWorking.SaveFile("d_init.txt", initDir, d_init)
    '    UpdateButtonText()
    'End Sub


    Private Sub UtilityListBox_doubleclick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UtilityListBox.DoubleClick
        RunSelectedUtility()
    End Sub

    Private Sub BrowseUtilities_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles BrowseUtilities.Click
        'ability to manually add utilities from other directories - daveralph1234
        OpenFileDialog1.ShowDialog()
        If OpenFileDialog1.FileName <> "OpenFileDialog1" Then
            UtilityListBox.Items.Add(OpenFileDialog1.FileName)
        End If
    End Sub

    Sub updateutilitylistfile() Handles Me.Closing
        FileOpen(1, utilityD & "\utilitylist.txt", OpenMode.Output)
        For i = 0 To UtilityListBox.Items.Count - 1
            PrintLine(1, UtilityListBox.Items(i).ToString)
            If UtilityListBox.CheckedIndices.Contains(i) Then
                PrintLine(1, "1")
            Else
                PrintLine(1, "0")
            End If
        Next
        FileClose(1)
    End Sub
End Class
