Public Class Configure_Proxy
    Private Sub Configure_Proxy_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        globalVariables.windows.configureProxy.Dispose()
        globalVariables.windows.configureProxy = Nothing
    End Sub

    Sub doTheDisablingOfStuff()
        chkUseSystemProxy.Enabled = False
        txtProxyAddress.Enabled = False
        txtPort.Enabled = False
        txtUser.Enabled = False
        txtPass.Enabled = False

        txtProxyAddress.Text = Nothing
        txtPort.Text = Nothing
    End Sub

    Sub doTheEnablingOfStuff()
        chkUseSystemProxy.Enabled = True

        If My.Settings.useSystemProxyConfig = True Then
            txtPass.Enabled = False
            txtPort.Enabled = False
            txtProxyAddress.Enabled = False
            txtUser.Enabled = False
        Else
            txtPass.Enabled = True
            txtPort.Enabled = True
            txtProxyAddress.Enabled = True
            txtUser.Enabled = True
        End If
    End Sub

    Private Sub Configure_Proxy_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        chkUseProxy.Checked = My.Settings.useHTTPProxy
        chkUseSystemProxy.Checked = My.Settings.useSystemProxyConfig

        If My.Settings.useHTTPProxy = False Then
            doTheDisablingOfStuff()
        Else
            doTheEnablingOfStuff()
        End If
    End Sub

    Private Sub chkUseProxy_Click(sender As Object, e As EventArgs) Handles chkUseProxy.Click
        My.Settings.useHTTPProxy = chkUseProxy.Checked

        If My.Settings.useHTTPProxy = False Then
            doTheDisablingOfStuff()
        Else
            doTheEnablingOfStuff()
        End If
    End Sub

    Private Sub chkUseSystemProxy_Click(sender As Object, e As EventArgs) Handles chkUseSystemProxy.Click
        My.Settings.useSystemProxyConfig = chkUseSystemProxy.Checked

        If My.Settings.useSystemProxyConfig = True Then
            txtPass.Enabled = False
            txtPort.Enabled = False
            txtProxyAddress.Enabled = False
            txtUser.Enabled = False
        Else
            txtPass.Enabled = True
            txtPort.Enabled = True
            txtProxyAddress.Enabled = True
            txtUser.Enabled = True

            My.Settings.proxyAddress = Nothing
            My.Settings.proxyPort = Nothing
            My.Settings.proxyUser = Nothing
            My.Settings.proxyPass = Nothing

            txtUser.Text = Nothing
            txtPort.Text = Nothing
            txtProxyAddress.Text = Nothing
            txtUser.Text = Nothing
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Dim port As Integer

        If Integer.TryParse(txtPort.Text.Trim, port) = True Then
            My.Settings.proxyAddress = txtProxyAddress.Text
            My.Settings.proxyPort = port
            My.Settings.proxyUser = txtUser.Text
            My.Settings.proxyPass = Functions.support.convertToBase64(txtPass.Text)

            MsgBox("Proxy Settings Saved.", MsgBoxStyle.Information, Me.Text)
        Else
            MsgBox("Invalid proxy port.", MsgBoxStyle.Critical, Me.Text)
        End If
    End Sub
End Class