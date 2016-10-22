
Imports dn_Library

Public Class Remover
    Dim resultado As String

    Sub inicializar() Handles Me.Loaded
        lstItens.Items.Clear()

        For i = 0 To MainWindow.tabela.Rows.Count - 1
            Dim it As New Contador
            Dim portaASelecionarstr As String = MainWindow.tabela.Rows(i).Item(2)
            lstItens.Items.Add(stringXZeros(MainWindow.tabela.Rows(i).Item(0), 4) & "-" & MainWindow.tabela.Rows(i).Item(1))
        Next

    End Sub

    Sub remover() Handles btnRemover.Click
        If lstItens.SelectedItems.Count = 0 Then
            MsgBox("Selecione um item para remover!")
        Else
            Dim codItem As Integer
            codItem = Mid(lstItens.SelectedItem, 1, 4)
            Dim comando As String = Nothing

            comando = "Delete from contador where codigo = " & codItem

            If Dados.executar(comando, MainWindow.credenciaisAccess, resultado) = False Then
                MsgBox(resultado)
            Else
                Me.Close()
            End If

        End If
    End Sub

    Sub cancelar() Handles btncancelar.Click
        Me.Close()
    End Sub

End Class
