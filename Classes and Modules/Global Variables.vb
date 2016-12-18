Imports System.Text.RegularExpressions

Namespace globalVariables
    Namespace commandLineSwitches
        Module commandLineSwitches
            Public Const scheduledRestorePoint As String = "-createscheduledrestorepoint"
            Public Const createCustomRestorePoint As String = "-createrestorepointcustomname"
            Public Const createRestorePoint As String = "-createrestorepoint"
            Public Const viewChangeLog As String = "-viewchangelog"
            Public Const viewEventLog As String = "-eventlog"
            Public Const forceUAC As String = "-forceuac"
            Public Const deleteOldRestorePoints As String = "-deleteoldrestorepoints"
            Public Const keepXNumberOfRestorePoints As String = "-keepxnumberofrestorepoints"
        End Module
    End Namespace

    Namespace booleans
        Module booleans
            Public Const strTrue As String = "True"
            Public Const strFalse As String = "False"
        End Module
    End Namespace

    Namespace windows
        Module windows
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

    Namespace eventLog
        Module eventLog
            Public Const strSystemRestorePointCreator As String = "System Restore Point Creator"
            Public Const strApplication As String = "Application"
            Public Const strRestorePointCreator As String = "Restore Point Creator"
        End Module
    End Namespace

    Namespace programCodeNames
        Module programCodeNames
            Public Const main As String = "restorepointcreator"
            Public Const beta As String = "restorepointcreatorbeta"
            Public Const tom As String = "restorepointcreatortom" ' This is a private update channel.
            Public Const xp As String = "restorepointcreatorxp"
        End Module
    End Namespace

    Namespace updateChannels
        Module updateChannels
            Public Const stable As String = "stable"
            Public Const beta As String = "beta"
            Public Const tom As String = "tom" ' This is a private update channel.
        End Module
    End Namespace

    Namespace version
        Module version
            Public versionInfo As String() = Application.ProductVersion.Split(".")

            Public strFullVersionString As String = String.Format("{0}.{1} Build {2}", versionInfo(enums.versionPieces.major), versionInfo(enums.versionPieces.minor), versionInfo(enums.versionPieces.build))

            Public shortMajor As Short = Short.Parse(versionInfo(enums.versionPieces.major).Trim)
            Public shortMinor As Short = Short.Parse(versionInfo(enums.versionPieces.minor).Trim)
            Public shortBuild As Short = Short.Parse(versionInfo(enums.versionPieces.build).Trim)

            Public versionStringWithoutBuild As String = String.Format("{0}.{1}", versionInfo(enums.versionPieces.major), versionInfo(enums.versionPieces.minor))

#If DEBUG Then
            Public Const boolDebugBuild As Boolean = True

            Public Const boolBeta As Boolean = True
            Public Const shortBetaVersion As Short = 5

            Public Const boolReleaseCandidate As Boolean = False
            Public Const shortReleaseCandidateVersion As Short = 0
#Else
            Public Const boolDebugBuild As Boolean = False

            Public Const boolBeta As Boolean = False
            Public Const shortBetaVersion As Short = 0

            Public Const boolReleaseCandidate As Boolean = False
            Public Const shortReleaseCandidateVersion As Short = 0
#End If
        End Module
    End Namespace

    Namespace webURLs
        Namespace updateBranch
            Namespace beta
                Module beta
                    Public Const strProgramZIP As String = "www.toms-world.org/beta/Restore Point Creator.zip"
                    Public Const strProgramZIPSHA2 As String = "www.toms-world.org/beta/Restore Point Creator.zip.sha2"
                End Module
            End Namespace

            Namespace debug
                Module debug
                    Public Const strProgramZIP As String = "www.toms-world.org/debug/Restore Point Creator.zip"
                    Public Const strProgramZIPSHA2 As String = "www.toms-world.org/debug/Restore Point Creator.zip.sha2"
                End Module
            End Namespace

            Namespace main
                Module main
                    Public Const strProgramZIP As String = "www.toms-world.org/release/Restore Point Creator.zip"
                    Public Const strProgramZIPSHA2 As String = "www.toms-world.org/release/Restore Point Creator.zip.sha2"
                End Module
            End Namespace
        End Namespace

        Namespace webPages
            Module webPages
                Public Const strCOMExceptionCrash As String = "http://www.toms-world.org/blog/restore_point_creator/comexception-crash"
                Public Const strProviderFailureMicrosoftKB As String = "http://support.microsoft.com/kb/2738812"
                Public Const strFAQ As String = "http://www.toms-world.org/blog/restore_point_creator/faq"
                Public Const strHelpVideos As String = "www.toms-world.org/blog/restore_point_creator/help-videos"

                Public Const strPayPal As String = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=HQL3AC96XKM42&lc=US&no_note=1&no_shipping=1&rm=1&return=http%3a%2f%2fwww%2etoms%2dworld%2eorg%2fblog%2fthank%2dyou%2dfor%2dyour%2ddonation&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donateCC_LG%2egif%3aNonHosted"
            End Module
        End Namespace

        Namespace dataProcessors
            Module dataProcessors
                Public Const strContactForm As String = "www.toms-world.org/programcontact"
                Public Const strCrashReporter As String = "www.toms-world.org/crashdatareporter"
            End Module
        End Namespace

        Namespace core
            Module core
                Public Const strWebSite As String = "www.toms-world.org/blog/restore_point_creator"
                Public Const strBetaDetails As String = "www.toms-world.org/betadetails"
                Public Const strProgramUpdateChecker As String = "www.toms-world.org/programupdatechecker"

                Public Const strFullChangeLog As String = "www.toms-world.org/download/restorepointcreatorchangelog.rtf"

                Public Const strRecentChangesLog As String = "www.toms-world.org/recentRPCChange.txt"
            End Module
        End Namespace
    End Namespace

    Namespace registryValues
        Module registryValues
            Public Const strKey As String = "SOFTWARE\Restore Point Creator"
            Public Const strKeyInsideSoftware As String = "Restore Point Creator"
        End Module
    End Namespace

    Module globalVariables
        Public Const programFileNameInZIP As String = "Restore Point Creator.exe"
        Public Const pdbFileNameInZIP As String = "Restore Point Creator.pdb"

        Public strDumpFilePath As String = IO.Path.Combine(IO.Path.GetTempPath(), "restorePointCreator.dmp")
        Public Const updaterFileName As String = "updater.exe"

        Public Const programName As String = "Restore Point Creator"

        Public shadowCopyMountFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.Windows).Substring(0, 3) & "shadowcopy"

        Public Const notificationTypeMessageBox As String = "msg"
        Public Const notificationTypeBalloon As String = "bal"

        Public strPathToSystemFolder As String = Environment.GetFolderPath(Environment.SpecialFolder.System)
        Public systemDriveLetter As String = Environment.GetFolderPath(Environment.SpecialFolder.System).Substring(0, 2)
        Public boolExtendedLoggingDuringUpdating As Boolean

        Public Const taskFolder As String = "Restore Point Creator" ' DON'T TOUCH THIS!

        Public Const invalidGUID As String = "00000000-0000-0000-0000-000000000000"

        Public KeepXAmountOfRestorePoints As Boolean = False
        Public KeepXAmountofRestorePointsValue As Short = -10
        Public boolLogLoadsAndExits As Boolean = False
        Public pleaseWaitWindowThread As Threading.Thread = Nothing

        Public boolLogToSystemLog As Boolean = True

        ' Creates the Regular Expression Parser that's used to parse the System Restore Point Creation Date
        Public regexRestorePointCreationTimeParser As Regex = New Regex("(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})(?<hour>\d{2})(?<minute>\d{2})(?<second>\d{2})\.", RegexOptions.IgnoreCase + RegexOptions.Compiled)

        Public Const strDefaultNameForScheduledTasks As String = "Scheduled System Checkpoint made by System Restore Point Creator"

        Public Const gdiHandles As Short = 0
        Public Const userHandles As Short = 1
        Public Const warningPercentage As Short = 90
        Public boolPortableMode As Boolean = False

        Public Const constStringRoot As String = "root"

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