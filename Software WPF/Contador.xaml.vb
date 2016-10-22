Imports dn_Library
Imports System.Data
Imports System.Threading
Imports System.IO.Ports

Public Class Contador
    Public codigo As Integer
    Public ultimaAtualizacao As Int64
    Dim resultado As String

    Sub aplicarClick() Handles btnAplicar.Click
        Try
            Dim comando As String = Nothing
            Dim operacao As String = Nothing
            Dim timeHoje As New TimeSpan(Now.Ticks)
            Dim diasHoje As Int64 = timeHoje.TotalDays

            If txtValor.Text = "" Then
                MsgBox("Preencha o campo Valor!")
                Return
            End If

            If rdIncrementa.IsChecked Then
                operacao = "I"
            Else
                operacao = "D"
            End If

            comando = "UPDATE contador SET nome = '" & txtNome.Text &
                    "',porta = '" & cmbPorta.SelectedItem.content &
                    "',endereco  =" & cmbEndereco.SelectedIndex &
                    ",operacao = '" & operacao &
                    "',valor = " & txtValor.Text &
                    ",ultimaAtualizacao = '" & diasHoje &
                    "' WHERE codigo = " & codigo

            If Dados.executar(comando, MainWindow.credenciaisAccess, resultado) = False Then
                MsgBox(resultado)
            Else
                Dim valor As String = txtValor.Text

                valor = stringXZeros(valor, 5)

                If Publicos.enviarAoPainel(cmbEndereco.SelectedItem.content, Mid(valor, 3, 3), Mid(valor, 1, 2), "0", cmbPorta.SelectedItem.content) Then
                    MsgBox("Atualizado com Sucesso")
                    MainWindow.atualizarLista = True
                End If
            End If

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
