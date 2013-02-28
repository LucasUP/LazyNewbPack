
Module ChangingOptions

    'toggles value into next item in valueArray.
    'e.g. if value = "B" and valueArray = {"A","B","C"} then value would equal "C"
    Sub toggle(ByRef value As String, ByVal valueArray() As String)
        'Dim oldVal = value
        Dim num = valueArray.Length - 1
        'find the value in valueArray
        For i = 0 To num
            'if value = valueArray(i)
            If valueArray(i).Equals(value, StringComparison.OrdinalIgnoreCase) Then
                'if value is last in array, set value to first in array
                If i = num Then
                    value = valueArray(0)
                    Exit For
                Else 'set value to next value in array
                    value = valueArray(i + 1)
                    Exit For
                End If
            End If
        Next
        'MessageBox.Show(oldVal + " to " + value)
    End Sub



    'Converts boolean true or false values to "YES" or "NO" strings
    Function booleanToYesNo(ByVal value As Boolean)
        Dim strValue
        If (value) Then strValue = "YES" Else strValue = "NO"
        Return strValue
    End Function


    'CHANGE SPECIFIED TAG in "text_to_change" to "YES" or "NO"  using a boolean "value" 
    'e.g. can change [ECONOMY:NO] to [ECONOMY:YES] by calling booleanTagSet("ECONOMY", true, d_init)
    Sub booleanTagSet(ByVal tag As String, _
                      ByVal value As Boolean, _
                      ByRef text_to_change As String)
        Dim newValue = booleanToYesNo(value) 'Change true or false value to "YES" or "NO"
        FileWorking.ReplaceTag(text_to_change, tag, newValue) 'Changes value of tag to "YES" or "NO"
    End Sub


    'CHANGE SPECIFIED TAG in "text_to_change" to any string value 
    'e.g. can change [POPULATION_CAP:200] to [POPULATION_CAP:123] by calling stringTagSet("POPULATION_CAP", "123", d_init)
    Sub stringTagSet(ByVal tag As String, _
                     ByVal value As String, _
                     ByRef text_to_change As String)
        If value = "" Then Exit Sub
        FileWorking.ReplaceTag(text_to_change, tag, value)
    End Sub


    '' MODS ''

    Sub aquifers(ByVal value As Boolean, ByVal dir As String)
        'Arrays with all strings to find and replace.
        'In () put upperbound of array ("0" for a 1-item array)
        Dim findList(0) As String
        Dim replaceList(0) As String

        'if value is true then TURN ON aquifers. Otherwise disable
        If (value) Then
            findList(0) = "!AQUIFER!"
            replaceList(0) = "[AQUIFER]"
        Else
            findList(0) = "[AQUIFER]"
            replaceList(0) = "!AQUIFER!"
        End If

        'define directory
        'Dim dir = "raw\objects\"
        'list of files to do find/replace in
        Dim fileList() As String = {"inorganic_stone_layer.txt", _
                                    "inorganic_stone_mineral.txt", _
                                    "inorganic_stone_soil.txt"}

        'Do all find and replaces for for all files in fileList
        FileWorking.ReplaceInMultipleFiles(fileList, dir, findList, replaceList)

    End Sub


    'Sub exotics(ByVal value As Boolean, ByVal dir As String)
    '    'Arrays with all strings to find and replace.
    '    'In () put upperbound of array ("1" for a 2-item array)
    '    Dim findList(1) As String
    '    Dim replaceList(1) As String

    '    'if value is TRUE then TURN ON exotic animals. Otherwise disable
    '    If (value) Then
    '        findList(0) = "![PET]!"
    '        replaceList(0) = "[PET_EXOTIC]"
    '        findList(1) = "![MOUNT]!"
    '        replaceList(1) = "[MOUNT_EXOTIC]"
    '    Else
    '        findList(0) = "[PET_EXOTIC]"
    '        replaceList(0) = "![PET]!"
    '        findList(1) = "[MOUNT_EXOTIC]"
    '        replaceList(1) = "![MOUNT]!"
    '    End If


    '    'Search for all "creature_*.txt" files to do find/replace in. 
    '    Dim fileCollection = My.Computer.FileSystem.GetFiles( _
    '        dir, FileIO.SearchOption.SearchTopLevelOnly, "creature_*.txt")

    '    Dim fileList(fileCollection.Count - 1) As String 'Create array with size of number of files found
    '    Dim j = 0
    '    For Each item In fileCollection
    '        Dim dSplit = Split(item, "\") 'create an array out of directory structure
    '        fileList(j) = dSplit(dSplit.Length - 1) 'return only the filename
    '        j = j + 1
    '    Next

    '    'Do all find and replaces for for all files in fileList
    '    FileWorking.ReplaceInMultipleFiles(fileList, dir, findList, replaceList)

    'End Sub


End Module
