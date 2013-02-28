Public Class InitEditor
    Dim dfDir
    Dim initDir

    Dim init
    Dim dinit

    Private Sub InitEditor_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        dfDir = MainForm.GetDFDir()
        initDir = dfDir + "\data\init"
        LoadInit()
    End Sub

    Private Sub LoadInitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadInitButton.Click
        LoadInit()
    End Sub

    Private Sub SaveInitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveInitButton.Click
        SaveInit()
    End Sub

    Private Sub LoadInit()
        init = FileWorking.ReadFile("init.txt", initDir)
        dinit = FileWorking.ReadFile("d_init.txt", initDir)
        InitTextBox.Text = init
        DInitTextBox.Text = dinit
    End Sub

    Private Sub SaveInit()
        init = InitTextBox.Text
        dinit = DInitTextBox.Text
        FileWorking.SaveFile("init.txt", initDir, init)
        FileWorking.SaveFile("d_init.txt", initDir, dinit)
        MainForm.LoadAll()
    End Sub


End Class