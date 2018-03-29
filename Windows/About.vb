Public Class About
    Public taskScheduledAssemblyVersion As Version

    Private Sub About_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        PictureBox1.Image = SystemIcons.Information.ToBitmap()
        Media.SystemSounds.Exclamation.Play()

        Dim strTaskLibraryVersion As String

        If taskScheduledAssemblyVersion.Build = 0 Then
            strTaskLibraryVersion = String.Format("{0}.{1}", taskScheduledAssemblyVersion.Major, taskScheduledAssemblyVersion.Minor)
        Else
            strTaskLibraryVersion = String.Format("{0}.{1} Build {2}", taskScheduledAssemblyVersion.Major, taskScheduledAssemblyVersion.Minor, taskScheduledAssemblyVersion.Build)
        End If

        Dim strLocalVersionString As String = globalVariables.version.strFullVersionString
        If globalVariables.version.boolDebugBuild Then strLocalVersionString &= " (Debug Build)"

        lblAbout.Text = String.Format(lblAbout.Text, strLocalVersionString, strTaskLibraryVersion)
    End Sub

    Private Sub OpenTaskSchedulerProjectSite_Click(sender As Object, e As EventArgs) Handles OpenTaskSchedulerProjectSite.Click
        Functions.support.launchURLInWebBrowser("https://github.com/dahall/taskscheduler")
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
End Class