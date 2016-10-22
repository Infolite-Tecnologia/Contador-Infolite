
Imports dn_Library
Imports System.IO.Ports

Public Class Publicos

    Const SOH = Chr(1)
    Const STX = Chr(2)
    Const ETX = Chr(3)

    '// não testada ainda com essa estrutura

    'Tipos de paineis diferentes podem ser utilizados...
    'Se o valor for de 1,2, ou 3 dígitos, exibir no campo da senha no painel
    'Se o valor for maios que 3 digitos, exibir o valor de milhar e dezena de milhar nos campos de guichê do painel
    Shared trabalhando As Boolean

    Public Shared Function enviarAoPainel(ByVal ender As String, ByVal senha As String, ByVal caixa As String, ByVal tipo As String, ByVal porta As String)
        Try

            While (trabalhando)
                pause(10)
            End While

            trabalhando = True

            stringXZeros(caixa, 2)
            stringXZeros(senha, 3)

            If serial.IsOpen = False Then
                serial.PortName = porta
                serial.BaudRate = 9600
                serial.Open()
            End If

            ' tipo == 1 = rechamada
            ' tipo == 0 = chamada
            Dim valor(9) As String
            valor(0) = ender '	 // Endereço

            valor(1) = senha(0) '// Centena  de senha
            valor(2) = senha(1) '// Dezena de senha
            valor(3) = senha(2) '// Unidade de senha

            valor(4) = caixa(0) '// Dezena de mesa
            valor(5) = caixa(1) '// Unidade de mesa

            valor(6) = stringXZeros(converteStringParaInteiro(valor(0)) +
                        converteStringParaInteiro(valor(1)) +
                        converteStringParaInteiro(valor(2)) +
                        converteStringParaInteiro(valor(3)) +
                        converteStringParaInteiro(valor(4)) +
                        converteStringParaInteiro(valor(5)), 2) 'Checa byte

            valor(7) = valor(6)(1) ' Menor
            valor(8) = valor(6)(0)


            If (tipo = "1") Then
                enviar_serial(SOH) ' 			// Rechamada
            Else
                enviar_serial(STX) ' 			// Chamada
            End If

            enviar_serial(valor(0)) ' Endereço
            enviar_serial(valor(1)) ' Centena  de senha
            enviar_serial(valor(2)) ' Dezena de senha
            enviar_serial(valor(3)) ' Unidade de senha
            enviar_serial(valor(4)) ' Dezena de mesa
            enviar_serial(valor(5)) ' Unidade de mesa
            enviar_serial(valor(6)) ' check maior
            enviar_serial(valor(7)) ' check menosr
            enviar_serial(ETX)

            serial.Close()
            trabalhando = False
            Return True

        Catch ex As Exception
            If serial.IsOpen Then : serial.Close() : End If
            trabalhando = False
            MsgBox(ex.Message)
            Return False
        End Try

    End Function

    Shared serial As New SerialPort()

    Shared Sub enviar_serial(ByVal valor As String)
        serial.Write(valor)
    End Sub

End Class
