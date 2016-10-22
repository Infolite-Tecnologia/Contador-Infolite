
Imports dn_Library
Imports System.Data
Imports System.Threading
Imports System.Windows.Threading

Public Class MainWindow

#Region "Objetos de Verificação de Instancia única"

    Declare Function ShowWindow Lib "user32" (ByVal hWnd As System.IntPtr, ByVal nCmdShow As Integer) As Boolean
    Declare Function SetForegroundWindow Lib "user32" (ByVal hwnd As System.IntPtr) As Integer

    Public Enum exibir
        SW_HIDE = 0
        SW_SHOWNORMAL = 1
        SW_NORMAL = 1
        SW_SHOWMINIMIZED = 2
        SW_SHOWMAXIMIZED = 3
        SW_MAXIMIZE = 3
        SW_SHOWNOACTIVATE = 4
        SW_SHOW = 5
        SW_MINIMIZE = 6
        SW_SHOWMINNOACTIVE = 7
        SW_SHOWNA = 8
        SW_RESTORE = 9
        SW_SHOWDEFAULT = 10
        SW_FORCEMINIMIZE = 11
        SW_MAX = 11
    End Enum

    Sub Verificar_Instancia_Unica()
        Dim esse_processo As Process = Process.GetCurrentProcess
        For Each processo As Process In Process.GetProcesses()
            If processo.ProcessName = esse_processo.ProcessName And processo.Id <> esse_processo.Id Then
                ShowWindow(processo.MainWindowHandle, exibir.SW_NORMAL)
                SetForegroundWindow(processo.MainWindowHandle)
                End
            End If
        Next processo
    End Sub

#End Region

#Region "Variáveis"

    Public Shared tabela As DataTable = Nothing
    Dim resultado As String = Nothing
    Public Shared credenciaisAccess 'As Dados.Credenciais()
    Dim listaPostasSeriais As New ArrayList
    Dim listastatusPortasSeriais As New ArrayList
    Private timer1 As DispatcherTimer
    Private timer2 As DispatcherTimer
    Public Shared atualizarLista As Boolean

#End Region

    Sub inicializar() Handles Me.Loaded
        Verificar_Instancia_Unica()

        Dim senhaAcessoLibrary As String = "8QIc/rfGX4HjtzOhuHOq8fiQKcGIb8cN0mPxZhESvjA="

        If dn_Library.inicializar(System.Reflection.Assembly.GetExecutingAssembly, senhaAcessoLibrary, senhaAcessoLibrary, True, True, True, True) = False Then
            End
        End If


        credenciaisAccess = New Dados.Credenciais(IO.Directory.GetCurrentDirectory & "/dados2003.mdb", 3)

        Dim t1 As New Thread(AddressOf listarItens)
        t1.Start()

        ' ******* Timer 1 *************
        timer1 = New DispatcherTimer
        timer1.Interval = TimeSpan.FromMilliseconds(100)
        AddHandler timer1.Tick, AddressOf timer1Tick
        timer1.Start()
        '******************************

        ' ******* Timer 2 *************
        timer2 = New DispatcherTimer
        timer2.Interval = TimeSpan.FromMilliseconds(100) 'Pelomenos 1 seg p dar tempo de listar itens No proprio timer ele se seta para 1 minuto
        AddHandler timer2.Tick, AddressOf timer2Tick
        timer2.Start()
        '******************************

    End Sub

    Sub timer1Tick()
        If atualizarLista Then
            atualizarLista = False
            Dim t1 As New Thread(AddressOf listarItens)
            t1.Start()
        End If
    End Sub

    Sub timer2Tick()
        Try
            Dim timeHoje As New TimeSpan(Now.Ticks)
            Dim diasHoje As Int64 = timeHoje.TotalDays

            For i = 0 To pnPrincipal.Children.Count - 1
                Dim contadorLocal As Contador
                Dim codigo As Integer = 0
                contadorLocal = pnPrincipal.Children.Item(i)
                codigo = contadorLocal.codigo
                If contadorLocal.txtValor.Text.somenteNumeros() = False Then
                    contadorLocal.txtValor.Text = "0"
                End If

                If contadorLocal.ultimaAtualizacao = diasHoje Then

                Else


                    Dim diasPassados As Int64 = diasHoje - contadorLocal.ultimaAtualizacao
                    Dim valorFinal As Int64

                    If diasPassados < 0 Then Continue For

                    If contadorLocal.rdIncrementa.IsChecked Then
                        valorFinal = converteStringParaInteiro(contadorLocal.txtValor.Text) + diasPassados
                        If valorFinal > 99999 Then valorFinal = 99999
                    Else
                        valorFinal = converteStringParaInteiro(contadorLocal.txtValor.Text) - diasPassados
                        If valorFinal < 0 Then valorFinal = 0
                    End If

                    contadorLocal.txtValor.Text = valorFinal

                    Dim comando As String = Nothing
                    comando = "UPDATE contador SET ultimaAtualizacao = '" & diasHoje & "', valor = " & valorFinal & " WHERE codigo = " & codigo
                    atualizarLista = True

                    If Dados.executar(comando, credenciaisAccess, resultado) = False Then
                        MsgBox(resultado)
                    Else
                        Dim t1 As New Thread(AddressOf listarItens)
                        t1.Start()
                    End If

                End If
                Dim valor As String = contadorLocal.txtValor.Text
                stringXZeros(valor, 5)
                Dim senha As String = Mid(valor, 3, 3)
                Dim guiche As String = Mid(valor, 1, 2)

                If Publicos.enviarAoPainel(contadorLocal.cmbEndereco.SelectedItem.content, senha, guiche, "0", contadorLocal.cmbPorta.SelectedItem.content) Then
                    'MsgBox("Atualizado com Sucesso")

                End If

            Next

        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        If pnPrincipal.Children.Count = 0 Then
            timer2.Interval = TimeSpan.FromMilliseconds(500)
        Else
            timer2.Interval = TimeSpan.FromMilliseconds(60000)
        End If

    End Sub

    Sub listarItens()
        Try
            If Dados.select_("Select * from contador", credenciaisAccess, tabela, resultado) = False Then
                MsgBox(resultado)
                Return
            End If

            listaPostasSeriais = listarPortasSeriais(True, listastatusPortasSeriais, resultado)

            If listaPostasSeriais Is Nothing Then
                MsgBox(resultado)
            End If

            Me.Dispatcher.BeginInvoke(New Action(AddressOf carregarValores))
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub carregarValores()
        Try
            For i = 0 To pnPrincipal.Children.Count - 1
                pnPrincipal.Children.RemoveAt(0)
            Next

            For i = 0 To tabela.Rows.Count - 1
                Dim it As New Contador
                Dim portaASelecionarstr As String = tabela.Rows(i).Item(2)
                Dim portaASelecionarint As Integer = -1
                For y = 0 To listaPostasSeriais.Count - 1
                    Dim txtPorta As New ListBoxItem
                    txtPorta.Content = listaPostasSeriais(y)
                    If listaPostasSeriais(y) = portaASelecionarstr Then
                        portaASelecionarint = y
                    End If
                    If listastatusPortasSeriais(y) = "True" Then
                        txtPorta.Foreground = Brushes.Blue
                    Else
                        txtPorta.Foreground = Brushes.Red
                    End If

                    it.cmbPorta.Items.Add(txtPorta)
                Next

                it.codigo = tabela.Rows(i).Item(0)
                it.txtNome.Text = tabela.Rows(i).Item(1)
                it.cmbPorta.SelectedIndex = portaASelecionarint
                it.cmbEndereco.SelectedIndex = tabela.Rows(i).Item(3)
                Dim operacao As String = tabela.Rows(i).Item(4) 'Operacao
                If operacao = "I" Then : it.rdIncrementa.IsChecked = True : Else : it.rdDecrementa.IsChecked = True : End If
                it.txtValor.Text = tabela.Rows(i).Item(5)
                it.ultimaAtualizacao = tabela.Rows(i).Item(6)

                pnPrincipal.Children.Add(it)
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub novo() Handles imgAdicionar.MouseDown
        Try
            Dim comando As String = Nothing
            Dim timeHoje As New TimeSpan(Now.Ticks)
            Dim diasHoje As String = timeHoje.TotalDays

            comando = "insert into contador (nome, porta, endereco, operacao, valor,ultimaAtualizacao) values ('','COM1',0,'D',9999,'" & diasHoje & "')"

            If Dados.executar(comando, credenciaisAccess, resultado) = False Then
                MsgBox(resultado)
            Else
                Dim t1 As New Thread(AddressOf listarItens)
                t1.Start()
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Sub remover() Handles imgRemover.MouseDown

        Dim rm As New Remover
        rm.ShowDialog()
        Dim t1 As New Thread(AddressOf listarItens)
        t1.Start()

    End Sub

    Sub fecharDefinitivamente() Handles imgFechar.MouseDown
        If MsgBox("Com o contador fechado não será possível atualizar os paineis. Deseja realmente fechar?", MsgBoxStyle.YesNoCancel) = MsgBoxResult.Yes Then
            End
        End If
    End Sub

    Sub abrirHelp() Handles imgManuel.MouseDown
        abrirProcesso(IO.Directory.GetCurrentDirectory & "/Contador Infolite Manual.chm")
    End Sub

    Sub sobreClick() Handles imgSobre.MouseDown
        Dim a As New Sobre
        a.Show()
    End Sub

    Private Sub Window_Closing(ByVal sender As System.Object, ByVal e As System.ComponentModel.CancelEventArgs) Handles MyBase.Closing


        e.Cancel = True
        'Me.Hide()
        Me.WindowState = Windows.WindowState.Minimized

    End Sub
End Class
