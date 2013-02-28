Imports System.Text.RegularExpressions

Module LoadingOptions

    'Return a tag's value in a 2D array
    Public Function loadTag(ByVal tag As String, ByVal tagArray(,) As String)
        For i = 0 To (tagArray.GetLength(0) - 1) 'GetLength(0) gets the length of the FIRST DIMENSION of array, not total number of elements
            If tagArray(i, 0) = tag Then
                'MsgBox(tagArray(i, 0) + ":" + tagArray(i, 1))
                'Return string
                Return tagArray(i, 1) 'return value of tag
            End If
        Next i
        MsgBox("[" + tag + "] not found :(")
        Return 0
    End Function

    Public Function loadTagAsBool(ByVal tag As String, ByVal tagArray(,) As String)
        For i = 0 To (tagArray.GetLength(0) - 1) 'GetLength(0) gets the length of the FIRST DIMENSION of array, not total number of elements
            If tagArray(i, 0) = tag Then
                'Return boolean true/false statements if value is "YES" or "NO"
                If tagArray(i, 1).Equals("YES", StringComparison.OrdinalIgnoreCase) Then
                    Return True
                ElseIf tagArray(i, 1).Equals("NO", StringComparison.OrdinalIgnoreCase) Then
                    Return False
                Else
                    Return False
                End If
            End If
        Next i
        MsgBox("[" + tag + "] not found :(")
        Return 0
    End Function

    Public Function loadAquifer(ByVal dir As String)
        'searches for a single [AQUIFER] tag. Returns True if it can find one
        Dim pattern = "\[AQUIFER\]"
        'Dim dir = "raw\objects\"
        Dim fileList() As String = {"inorganic_stone_layer.txt", _
                                    "inorganic_stone_mineral.txt", _
                                    "inorganic_stone_soil.txt"}
        Return FileWorking.regexSearchInFiles(pattern, fileList, dir)
    End Function

    'Public Function loadExotics(ByVal dir As String)
    '    'searches for a single [PET_EXOTIC] tag in any one of these files. Returns True if it can find one
    '    Dim pattern = "\[PET_EXOTIC\]"
    '    'Dim dir = "raw\objects\"
    '    'These are only a few of the creature files, Didn't make sense to look through every single one.
    '    Dim fileList() As String = {"creature_reptiles.txt", _
    '                                "creature_standard.txt", _
    '                                "creature_subterranean.txt", _
    '                                "creature_domestic.txt", _
    '                                "creature_small_mammals.txt"}
    '    Return FileWorking.regexSearchInFiles(pattern, fileList, dir)
    'End Function


    'Use Regex Group collection to find and store [data:value] pairs from a string, and return it as 2D array!
    Public Function parseTagsToArray(ByVal fileText As String)
        'http://msdn.microsoft.com/en-us/library/30wbz966(v=VS.90).aspx#GroupCollection
        'http://msdn.microsoft.com/en-us/library/30wbz966(v=VS.90).aspx#the_captured_group
        'http://msdn.microsoft.com/en-us/library/system.text.regularexpressions.match.groups(v=VS.90).aspx

        'Dim tagArray(200, 1) As String 'A 2D array will return our data/value pairs
        Dim pattern As String = "\[(?<tag>\w+):(?<value>.*)\]" 'find anything with a [tag:value] pattern and stores content in "tag" and "value" groups
        Dim matches As MatchCollection = Regex.Matches(fileText, pattern) 'Do the search with above pattern. Creates a collection of matches
        Dim tagArray(matches.Count - 1, 1) As String 'A 2D array will return our data/value pairs

        'Iterates through all matches and stores data from them in a 2d array {{tag, value}, {tag, value}, {tag, value}, ...
        Dim i = 0
        For Each match As Match In matches
            Dim tag As String = match.Groups("tag").Value 'gets the current matches tag/value groups (defined in "pattern") 
            Dim value As String = match.Groups("value").Value
            If tag <> "" Then 'If tag not empty store them into array
                tagArray(i, 0) = tag
                tagArray(i, 1) = value
                i = i + 1 'only increment when pair is added to array
                'MsgBox(tagArray(i, 0) + ":" + tagArray(i, 1)) 'DEBUG outputs tag:value
            End If
        Next
        'MsgBox(i.ToString + " tags parsed and loaded!")
        If (i < 2) Then
            MsgBox("Problem loading options: Few or no tags found in a file!")
        End If
        Return tagArray
    End Function


    'Use Regex Group collection to find and store [data] tags from a string, and return it as 1D array!
    Public Function parseTagsToArray1D(ByVal fileText As String)

        'Dim tagArray(200) As String 'A 2D array will return our data/value pairs
        Dim pattern As String = "\[(?<tag>.*)\]" 'find anything with a [tag:value] pattern and stores content in "tag" and "value" groups
        Dim matches As MatchCollection = Regex.Matches(fileText, pattern) 'Do the search with above pattern. Creates a collection of matches
        Dim tagArray(matches.Count - 1) As String 'A 2D array will return our data/value pairs

        'Iterates through all matches and stores data from them in a 2d array {{tag, value}, {tag, value}, {tag, value}, ...
        Dim i = 0
        For Each match As Match In matches
            Dim tag As String = match.Groups("tag").Value 'gets the current matches tag/value groups (defined in "pattern") 
            If tag <> "" Then 'If tag not empty store them into array
                tagArray(i) = tag
                i = i + 1 'only increment when pair is added to array
                'MsgBox("'" + tag + "'") 'DEBUG outputs tag
            End If
        Next
        'MsgBox(i.ToString + " tags parsed and loaded!")
        If (i < 1) Then
            MsgBox("Problem loading options: Few or no tags found in a file!")
        End If
        Return tagArray
    End Function



End Module
