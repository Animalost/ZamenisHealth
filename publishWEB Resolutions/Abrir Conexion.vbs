On Error Resume Next
Set objShell = CreateObject("WScript.Shell")
objShell.Run "cmd /c net use X: /delete", 0, True
If Err.Number <> 0 Then
    MsgBox "Error: " & Err.Description
End If
On Error GoTo 0

Dim objNetwork
Set objNetwork = CreateObject("WScript.Network")
objNetwork.MapNetworkDrive "X:", "https://slsoft.net:5009/Zamenis/", False, "Tecnologia", "Caihcron*103"
Set objNetwork = Nothing