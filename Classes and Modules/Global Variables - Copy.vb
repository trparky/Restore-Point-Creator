Imports System.Text.RegularExpressions

Namespace windowInstances
    Module windowInstances
        Public frmDiskSpaceUsageWindow As Disk_Space_Usage
        Public frmChangeLog As Change_Log
        Public frmCreateRestorePointAtUserLogon As Create_Restore_Point_at_User_Logon
        Public frmTaskScheduler As frmTaskScheduler
        Public frmManageSystemRestoreStorageSpace As frmManageSystemRestoreStorageSpace
        Public frmPleaseWait As Please_Wait
        Public eventLogForm As eventLogForm
        Public mountVolumeShadowCopy As Mount_Volume_Shadow_Copy
        Public officialContactForm As Official_Contact_Form
        Public configureProxy As Configure_Proxy
        Public frmManuallySubmitCrashDataInstance As frmManuallySubmitCrashData
    End Module
End Namespace

Namespace globalVariables
    Module globalVariables
        Public Function transformURL(strURLInput As String) As String
            If strURLInput.Trim.ToLower.StartsWith("http") = False Then
                If My.Settings.useSSL = True Then
                    Return "https://" & strURLInput
                Else
                    Return "http://" & strURLInput
                End If
            Else
                Return strURLInput
            End If
        End Function

        Public Enum restorePointListSubItems
            restorePointID = 0
            restorePointName = 1
            restorePointDate = 2
            restorePointType = 3
            restorePointAge = 4
        End Enum

        Public Enum versionPieces As Short
            major = 0
            minor = 1
            build = 2
            revision = 3
        End Enum

        Public Const updaterFileName As String = "updater.exe"

        Public Const betaDetailsURL As String = "www.toms-world.org/betadetails"

        Public Const programFileURL As String = "www.toms-world.org/download/Restore Point Creator.exe"
        Public Const programFileURLSHA1 As String = "www.toms-world.org/download/Restore Point Creator.exe.sha1"

        Public Const programFileURLBeta As String = "www.toms-world.org/beta/Restore Point Creator.exe"
        Public Const programFileURLBetaSHA1 As String = "www.toms-world.org/beta/Restore Point Creator.exe.sha1"

        Public Const programDebugFileURL As String = "www.toms-world.org/beta/Restore Point Creator.pdb"
        Public Const programDebugFileSHA1URL As String = "www.toms-world.org/beta/Restore Point Creator.pdb.sha1"

        Public Const programFileURLDebugBuild As String = "www.toms-world.org/debug/Restore Point Creator.exe"
        Public Const programFileURLDebugBuildSHA1 As String = "www.toms-world.org/debug/Restore Point Creator.exe.sha1"

        Public Const programFileURLDebugBuildDebugSymbols As String = "www.toms-world.org/debug/Restore Point Creator.pdb"
        Public Const programFileURLDebugBuildDebugSymbolsURL As String = "www.toms-world.org/debug/Restore Point Creator.pdb.sha1"

        Public Const updaterURL As String = "www.toms-world.org/download/updater.exe"
        Public Const updaterURLSHA1 As String = "www.toms-world.org/download/updater.exe.sha1"

        Public Const webSiteURL As String = "www.toms-world.org/blog/restore_point_creator"

        Public Const programCodeName As String = "restorepointcreator"
        Public Const programCodeNameWinXP As String = "restorepointcreatorxp"
        Public Const programCodeNameBeta As String = "restorepointcreatorbeta"

        Public Const programUpdateCheckerURLPost As String = "www.toms-world.org/programupdatechecker"
        Public Const programName As String = "Restore Point Creator"

        Public Const changeLogURL As String = "www.toms-world.org/download/restorepointcreatorchangelog.rtf"
        Public Const contactFormURL As String = "www.toms-world.org/programcontact"
        Public Const crashFromURL As String = "www.toms-world.org/crashdatareporter"
        Public Const readFirstEntryFromChangeLogURL As String = "www.toms-world.org/recentRPCChange.txt"
        Public Const helpVideosURL As String = "www.toms-world.org/blog/restore_point_creator/help-videos"
        Public Const comExceptionCrashURL As String = "http://www.toms-world.org/blog/restore_point_creator/comexception-crash"
        Public Const providerFailureMicrosoftKnowledgebaseURL As String = "http://support.microsoft.com/kb/2738812"
        Public Const faqURL As String = "http://www.toms-world.org/blog/restore_point_creator/faq"

        Public shadowCopyMountFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.Windows).Substring(0, 3) & "shadowcopy"

        Public Const payPalURL As String = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=HQL3AC96XKM42&lc=US&no_note=1&no_shipping=1&rm=1&return=http%3a%2f%2fwww%2etoms%2dworld%2eorg%2fblog%2fthank%2dyou%2dfor%2dyour%2ddonation&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"

        Public Const updateChannelBeta As String = "beta"
        Public Const updateChannelStable As String = "stable"

        Public Const notificationTypeMessageBox As String = "msg"
        Public Const notificationTypeBalloon As String = "bal"
        Public systemDriveLetter As String = Environment.SystemDirectory.Substring(0, 2)

        'Public boolWinXP As Boolean = False ' Default value is False.

        Public Const strProgramRegistryKey As String = "SOFTWARE\Restore Point Creator"
        Public Const strProgramRegistryKeyInsideSoftwareKey As String = "Restore Point Creator"
        Public Const strSafeModeBootRegistryValue As String = "Safe Mode Boot Set"

#If DEBUG Then
        Public Const boolDebugBuild As Boolean = True

        Public Const boolBeta As Boolean = False
        Public Const shortBetaVersion As Short = 0

        Public Const boolReleaseCandidate As Boolean = False
        Public Const shortReleaseCandidateVersion As Short = 0
#Else
        Public Const boolDebugBuild As Boolean = False

        Public Const boolBeta As Boolean = False
        Public Const shortBetaVersion As Short = 0

        Public Const boolReleaseCandidate As Boolean = False
        Public Const shortReleaseCandidateVersion As Short = 0
#End If

        Public Const taskFolder As String = "Restore Point Creator" ' DON'T TOUCH THIS!

        Public Const invalidGUID As String = "00000000-0000-0000-0000-000000000000"

        Public KeepXAmountOfRestorePoints As Boolean = False
        Public KeepXAmountofRestorePointsValue As Short = -10
        Public boolLogLoadsAndExitsToEventLog As Boolean = False
        Public pleaseWaitWindowThread As Threading.Thread = Nothing
        Public versionInfo As String() = Application.ProductVersion.Split(".")
        Public versionString As String = String.Format("{0}.{1} Build {2}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor), versionInfo(versionPieces.build))
        Public versionStringWithoutBuild As String = String.Format("{0}.{1}", versionInfo(versionPieces.major), versionInfo(versionPieces.minor))

        Public versionMajor As Short = Short.Parse(versionInfo(versionPieces.major).Trim)
        Public versionMinor As Short = Short.Parse(versionInfo(versionPieces.minor).Trim)
        Public versionBuild As Short = Short.Parse(versionInfo(versionPieces.build).Trim)

        Public boolLogToSystemLog As Boolean = True

        ' Creates the Regular Expression Parser that's used to parse the System Restore Point Creation Date
        Public regexRestorePointCreationTimeParser As Regex = New Regex("(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})\.", RegexOptions.IgnoreCase + RegexOptions.Compiled)
        Public Const strDefaultNameForScheduledRestorePoint As String = "Scheduled System Checkpoint made by System Restore Point Creator"

        Public Const gdiHandles As Short = 0
        Public Const userHandles As Short = 1
        Public Const warningPercentage As Short = 90
        Public boolPortableMode As Boolean = False

        ' We don't use this anymore but is kept here for reference to the datatypes in the systemRestorePoint Object.
        'Public Enum systemRestorePointDataType
        '    <Description("CreationTime")> CreationTime
        '    <Description("Description")> Description
        '    <Description("EventType")> EventType
        '    <Description("RestorePointType")> RestorePointType
        '    <Description("SequenceNumber")> SequenceNumber
        'End Enum
        '
        ' The systemRestorePoint Object has the following attributes...
        ' CreationTime : The encoded time of when the restore point was created.  This must be parsed to get a proper date from it.
        ' Description : This is the name of the restore point.
        ' RestorePointType : The type of restore point it is.
        ' SequenceNumber : The unique numerical ID that's assigned to a restore point on Windows Vista and newer.  This counts
        '                  up over time on Vista and newer.  On Windows XP it always starts off at 0 no matter how many restore
        '                  points have been made since the system was installed.
        ' EventType : We don't use this data type but it is a data type in the systemRestorePoint Object so it's documented here.
    End Module
End Namespace