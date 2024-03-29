﻿Public Class About
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

        ' Replace the placeholders in the label's text with the appropriate data.
        lblAbout.Text = String.Format(lblAbout.Text, If(globalVariables.version.boolDebugBuild, globalVariables.version.strFullVersionString & " (Debug Build)", globalVariables.version.strFullVersionString), strTaskLibraryVersion)
    End Sub

    Private Sub OpenTaskSchedulerProjectSite_Click(sender As Object, e As EventArgs) Handles OpenTaskSchedulerProjectSite.Click
        Functions.support.launchURLInWebBrowser("https://github.com/dahall/taskscheduler")
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub OpenRPCWebSite_Click(sender As Object, e As EventArgs) Handles OpenRPCWebSite.Click
        Functions.support.launchURLInWebBrowser("https://www.toms-world.org/blog/rpc")
    End Sub
End Class