Imports System.Reflection

Public Class Sobre


    Sub inicializar() Handles Me.Loaded

        Dim ApplicationTitle As String
        If My.Application.Info.Title <> "" Then
            ApplicationTitle = My.Application.Info.Title
        Else
            ApplicationTitle = System.IO.Path.GetFileNameWithoutExtension(My.Application.Info.AssemblyName)
        End If
        Dim texto As String = Nothing

        'texto = String.Format("Título: ", ApplicationTitle) & vbLf


        texto &= "Sistema: " & My.Application.Info.ProductName & vbLf & vbLf
        texto &= "Versão: " & My.Application.Info.Version.ToString & vbLf & vbLf
        texto &= "Copyright: " & My.Application.Info.Copyright & vbLf & vbLf
        texto &= "Empresa: " & My.Application.Info.CompanyName & vbLf & vbLf
        texto &= "Descrição: " & My.Application.Info.Description & vbLf & vbLf

        Dim myAss As [Assembly]
        myAss = [Assembly].GetEntryAssembly
        Dim myAssGUID As System.Guid = myAss.ManifestModule.ModuleVersionId
        Dim myStringGUID As String = myAssGUID.ToString
        texto &= "GUID: " & myStringGUID & vbLf & vbLf

        txtSobre.Text = texto

    End Sub

End Class
