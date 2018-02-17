<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtRestorePointDescription = New System.Windows.Forms.TextBox()
        Me.btnCreate = New System.Windows.Forms.Button()
        Me.systemRestorePointsList = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.restorePointListContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.toolStripHeader = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem2 = New System.Windows.Forms.ToolStripSeparator()
        Me.stripRefresh = New System.Windows.Forms.ToolStripMenuItem()
        Me.stripDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.stripRestore = New System.Windows.Forms.ToolStripMenuItem()
        Me.stripRestoreSafeMode = New System.Windows.Forms.ToolStripMenuItem()
        Me.lblCurrentRestorePointsLabel = New System.Windows.Forms.Label()
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnCreateSystemCheckpoint = New System.Windows.Forms.Button()
        Me.btnCreateRestorePointNameWithDefaultName = New System.Windows.Forms.Button()
        Me.btnRestoreToRestorePoint = New System.Windows.Forms.Button()
        Me.btnRefreshRestorePoints = New System.Windows.Forms.Button()
        Me.buttonTableLayout = New System.Windows.Forms.TableLayoutPanel()
        Me.btnDeleteRestorePoint = New System.Windows.Forms.Button()
        Me.btnRestoreToRestorePointSafeMode = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.toolUtilities = New System.Windows.Forms.ToolStripMenuItem()
        Me.ManuallyFixSystemRestoreToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MountVolumeShadowCopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckWindowsPowerPlanSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDeleteRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDeleteAllRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDeleteOldRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripViewDiskSpaceUsage = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripManageSystemRestoreStorageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripScheduleRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.FixRuntimeTasksToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProgramEventLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RebootSystemToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckForUpdates = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveSafeModeBootOptionToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.InterfaceTooBigToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AdditionalOptionsAndSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripConfirmDeletions = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfirmRestorePointDeletionsInBatchesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripLogRestorePointDeletions = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCloseAfterRestorePointIsCreated = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BypassNoUACLauncherToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExtendedLoggingForScheduledTasks = New System.Windows.Forms.ToolStripMenuItem()
        Me.AskBeforeCreatingRestorePointToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.chkShowVersionInTitleBarToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigurationBackupRestoreToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RestoreToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripSeparator()
        Me.SetBarColorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetPleaseWaitBorderColorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.barBelowColorSettings = New System.Windows.Forms.ToolStripSeparator()
        Me.ExtendedDebugToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripMyComputer = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateRestorePointAtUserLogonToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigureProxyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.KeepXAmountOfRestorePointsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DefaultCustomRestorePointNameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigureHTTPTimeoutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.SoftwareUpdateSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripAutomaticallyCheckForUpdates = New System.Windows.Forms.ToolStripMenuItem()
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnableExtendedLoggingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigureAutomaticUpdatesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckEveryWeek = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckEveryTwoWeeks = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckCustom = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateChannelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OnlyGiveMeReleaseCandidates = New System.Windows.Forms.ToolStripMenuItem()
        Me.lineUnderRC = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripStableChannel = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripBetaChannel = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItemPrivateForTom = New System.Windows.Forms.ToolStripMenuItem()
        Me.NotificationOptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NotificationTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MessageBoxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BalloonToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UseSSLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.lineBeforeDebugMenuItem = New System.Windows.Forms.ToolStripSeparator()
        Me.SwitchToDebugBuildToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutThisProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FrequentlyAskedQuestionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProductWebSiteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContactTheDeveloperToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewOfficialChangeLogToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.CommandLineArgumentsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDonate = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.FfgdToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FgdfgdToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ColorDialog = New System.Windows.Forms.ColorDialog()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.importBackupDialog = New System.Windows.Forms.OpenFileDialog()
        Me.exportBackupDialog = New System.Windows.Forms.SaveFileDialog()
        Me.pleaseWaitPanel = New System.Windows.Forms.Panel()
        Me.pleaseWaitBorderText = New System.Windows.Forms.Label()
        Me.pleaseWaitlblLabel = New System.Windows.Forms.Label()
        Me.pleaseWaitProgressBar = New SmoothProgressBar()
        Me.pleaseWaitProgressBarChanger = New System.Windows.Forms.Timer(Me.components)
        Me.pleaseWaitMessageChanger = New System.Windows.Forms.Timer(Me.components)
        Me.EnableAdvancedDebugModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.restorePointListContextMenu.SuspendLayout()
        Me.buttonTableLayout.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
        Me.pleaseWaitPanel.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(202, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Enter a Description for your Restore Point"
        '
        'txtRestorePointDescription
        '
        Me.txtRestorePointDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtRestorePointDescription.ForeColor = System.Drawing.Color.DimGray
        Me.txtRestorePointDescription.Location = New System.Drawing.Point(7, 35)
        Me.txtRestorePointDescription.Name = "txtRestorePointDescription"
        Me.txtRestorePointDescription.Size = New System.Drawing.Size(484, 20)
        Me.txtRestorePointDescription.TabIndex = 1
        Me.txtRestorePointDescription.TabStop = False
        Me.txtRestorePointDescription.Text = "Type in a name for your custom-named System Restore Point and press Enter..."
        '
        'btnCreate
        '
        Me.btnCreate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreate.Enabled = False
        Me.btnCreate.Image = Global.Restore_Point_Creator.My.Resources.Resources.create
        Me.btnCreate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreate.Location = New System.Drawing.Point(497, 33)
        Me.btnCreate.Name = "btnCreate"
        Me.btnCreate.Size = New System.Drawing.Size(205, 22)
        Me.btnCreate.TabIndex = 2
        Me.btnCreate.Text = "Create Custom Named Restore Point"
        Me.btnCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnCreate.UseVisualStyleBackColor = True
        '
        'systemRestorePointsList
        '
        Me.systemRestorePointsList.AllowColumnReorder = True
        Me.systemRestorePointsList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.systemRestorePointsList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader5})
        Me.systemRestorePointsList.ContextMenuStrip = Me.restorePointListContextMenu
        Me.systemRestorePointsList.FullRowSelect = True
        Me.systemRestorePointsList.Location = New System.Drawing.Point(7, 119)
        Me.systemRestorePointsList.Name = "systemRestorePointsList"
        Me.systemRestorePointsList.Size = New System.Drawing.Size(703, 224)
        Me.systemRestorePointsList.TabIndex = 0
        Me.systemRestorePointsList.UseCompatibleStateImageBehavior = False
        Me.systemRestorePointsList.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "ID"
        Me.ColumnHeader1.Width = 27
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Restore Point Description"
        Me.ColumnHeader2.Width = 191
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Created On"
        Me.ColumnHeader3.Width = 140
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Restore Point Type"
        Me.ColumnHeader4.Width = 125
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Age in Days"
        Me.ColumnHeader5.Width = 71
        '
        'restorePointListContextMenu
        '
        Me.restorePointListContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripHeader, Me.ToolStripMenuItem2, Me.stripRefresh, Me.stripDelete, Me.stripRestore, Me.stripRestoreSafeMode})
        Me.restorePointListContextMenu.Name = "restorePointListContentMenu"
        Me.restorePointListContextMenu.Size = New System.Drawing.Size(320, 120)
        Me.restorePointListContextMenu.Text = "Restore Point Command Menu"
        '
        'toolStripHeader
        '
        Me.toolStripHeader.Image = Global.Restore_Point_Creator.My.Resources.Resources.gear
        Me.toolStripHeader.Name = "toolStripHeader"
        Me.toolStripHeader.Size = New System.Drawing.Size(319, 22)
        Me.toolStripHeader.Text = "System Restore Point Command Menu"
        '
        'ToolStripMenuItem2
        '
        Me.ToolStripMenuItem2.Name = "ToolStripMenuItem2"
        Me.ToolStripMenuItem2.Size = New System.Drawing.Size(316, 6)
        '
        'stripRefresh
        '
        Me.stripRefresh.Name = "stripRefresh"
        Me.stripRefresh.Size = New System.Drawing.Size(319, 22)
        Me.stripRefresh.Text = "&Refresh List"
        '
        'stripDelete
        '
        Me.stripDelete.Enabled = False
        Me.stripDelete.Name = "stripDelete"
        Me.stripDelete.Size = New System.Drawing.Size(319, 22)
        Me.stripDelete.Text = "&Delete Selected Restore Point"
        '
        'stripRestore
        '
        Me.stripRestore.Enabled = False
        Me.stripRestore.Name = "stripRestore"
        Me.stripRestore.Size = New System.Drawing.Size(319, 22)
        Me.stripRestore.Text = "Restore to &Selected Restore Point"
        '
        'stripRestoreSafeMode
        '
        Me.stripRestoreSafeMode.Enabled = False
        Me.stripRestoreSafeMode.Name = "stripRestoreSafeMode"
        Me.stripRestoreSafeMode.Size = New System.Drawing.Size(319, 22)
        Me.stripRestoreSafeMode.Text = "Restore to Selected Restore Point in Safe Mode"
        '
        'lblCurrentRestorePointsLabel
        '
        Me.lblCurrentRestorePointsLabel.AutoSize = True
        Me.lblCurrentRestorePointsLabel.Location = New System.Drawing.Point(5, 100)
        Me.lblCurrentRestorePointsLabel.Name = "lblCurrentRestorePointsLabel"
        Me.lblCurrentRestorePointsLabel.Size = New System.Drawing.Size(113, 13)
        Me.lblCurrentRestorePointsLabel.TabIndex = 11
        Me.lblCurrentRestorePointsLabel.Text = "Current Restore Points"
        '
        'btnCreateSystemCheckpoint
        '
        Me.btnCreateSystemCheckpoint.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreateSystemCheckpoint.Image = Global.Restore_Point_Creator.My.Resources.Resources.create
        Me.btnCreateSystemCheckpoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreateSystemCheckpoint.Location = New System.Drawing.Point(497, 61)
        Me.btnCreateSystemCheckpoint.Name = "btnCreateSystemCheckpoint"
        Me.btnCreateSystemCheckpoint.Size = New System.Drawing.Size(157, 23)
        Me.btnCreateSystemCheckpoint.TabIndex = 19
        Me.btnCreateSystemCheckpoint.Text = "Create System Checkpoint"
        Me.btnCreateSystemCheckpoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ToolTip.SetToolTip(Me.btnCreateSystemCheckpoint, "Creates a System Restore Point with a description of ""System Checkpoint made by S" &
        "ystem Restore Point Creator"".")
        Me.btnCreateSystemCheckpoint.UseVisualStyleBackColor = True
        '
        'btnCreateRestorePointNameWithDefaultName
        '
        Me.btnCreateRestorePointNameWithDefaultName.Image = Global.Restore_Point_Creator.My.Resources.Resources.create
        Me.btnCreateRestorePointNameWithDefaultName.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreateRestorePointNameWithDefaultName.Location = New System.Drawing.Point(7, 61)
        Me.btnCreateRestorePointNameWithDefaultName.Name = "btnCreateRestorePointNameWithDefaultName"
        Me.btnCreateRestorePointNameWithDefaultName.Size = New System.Drawing.Size(385, 23)
        Me.btnCreateRestorePointNameWithDefaultName.TabIndex = 39
        Me.btnCreateRestorePointNameWithDefaultName.Text = "Create System Restore Point with your Default Custom Restore Point Name"
        Me.btnCreateRestorePointNameWithDefaultName.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ToolTip.SetToolTip(Me.btnCreateRestorePointNameWithDefaultName, "Creates a System Restore Point with the name of your Default Custom Restore Point" &
        " Name.")
        Me.btnCreateRestorePointNameWithDefaultName.UseVisualStyleBackColor = True
        Me.btnCreateRestorePointNameWithDefaultName.Visible = False
        '
        'btnRestoreToRestorePoint
        '
        Me.btnRestoreToRestorePoint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRestoreToRestorePoint.Enabled = False
        Me.btnRestoreToRestorePoint.Image = Global.Restore_Point_Creator.My.Resources.Resources.restore
        Me.btnRestoreToRestorePoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRestoreToRestorePoint.Location = New System.Drawing.Point(3, 37)
        Me.btnRestoreToRestorePoint.Name = "btnRestoreToRestorePoint"
        Me.btnRestoreToRestorePoint.Size = New System.Drawing.Size(346, 22)
        Me.btnRestoreToRestorePoint.TabIndex = 18
        Me.btnRestoreToRestorePoint.Text = "Restore to Selected System Restore Point"
        Me.btnRestoreToRestorePoint.UseVisualStyleBackColor = True
        '
        'btnRefreshRestorePoints
        '
        Me.btnRefreshRestorePoints.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRefreshRestorePoints.Image = Global.Restore_Point_Creator.My.Resources.Resources.refresh
        Me.btnRefreshRestorePoints.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRefreshRestorePoints.Location = New System.Drawing.Point(3, 5)
        Me.btnRefreshRestorePoints.Name = "btnRefreshRestorePoints"
        Me.btnRefreshRestorePoints.Size = New System.Drawing.Size(346, 23)
        Me.btnRefreshRestorePoints.TabIndex = 14
        Me.btnRefreshRestorePoints.Text = "Refresh List of System Restore Points"
        Me.btnRefreshRestorePoints.UseVisualStyleBackColor = True
        '
        'buttonTableLayout
        '
        Me.buttonTableLayout.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.buttonTableLayout.ColumnCount = 2
        Me.buttonTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.buttonTableLayout.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.buttonTableLayout.Controls.Add(Me.btnRestoreToRestorePoint, 0, 1)
        Me.buttonTableLayout.Controls.Add(Me.btnRefreshRestorePoints, 0, 0)
        Me.buttonTableLayout.Controls.Add(Me.btnDeleteRestorePoint, 1, 0)
        Me.buttonTableLayout.Controls.Add(Me.btnRestoreToRestorePointSafeMode, 1, 1)
        Me.buttonTableLayout.Location = New System.Drawing.Point(5, 349)
        Me.buttonTableLayout.Name = "buttonTableLayout"
        Me.buttonTableLayout.RowCount = 2
        Me.buttonTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.buttonTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.buttonTableLayout.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.buttonTableLayout.Size = New System.Drawing.Size(705, 62)
        Me.buttonTableLayout.TabIndex = 31
        '
        'btnDeleteRestorePoint
        '
        Me.btnDeleteRestorePoint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDeleteRestorePoint.Enabled = False
        Me.btnDeleteRestorePoint.Image = Global.Restore_Point_Creator.My.Resources.Resources.delete
        Me.btnDeleteRestorePoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteRestorePoint.Location = New System.Drawing.Point(355, 5)
        Me.btnDeleteRestorePoint.Name = "btnDeleteRestorePoint"
        Me.btnDeleteRestorePoint.Size = New System.Drawing.Size(347, 23)
        Me.btnDeleteRestorePoint.TabIndex = 13
        Me.btnDeleteRestorePoint.Text = "Delete Selected Restore Point"
        Me.btnDeleteRestorePoint.UseVisualStyleBackColor = True
        '
        'btnRestoreToRestorePointSafeMode
        '
        Me.btnRestoreToRestorePointSafeMode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRestoreToRestorePointSafeMode.Enabled = False
        Me.btnRestoreToRestorePointSafeMode.Image = Global.Restore_Point_Creator.My.Resources.Resources.restore
        Me.btnRestoreToRestorePointSafeMode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRestoreToRestorePointSafeMode.Location = New System.Drawing.Point(355, 37)
        Me.btnRestoreToRestorePointSafeMode.Name = "btnRestoreToRestorePointSafeMode"
        Me.btnRestoreToRestorePointSafeMode.Size = New System.Drawing.Size(347, 22)
        Me.btnRestoreToRestorePointSafeMode.TabIndex = 19
        Me.btnRestoreToRestorePointSafeMode.Text = "Restore to Selected System Restore Point in Safe Mode"
        Me.btnRestoreToRestorePointSafeMode.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolUtilities, Me.OptionsToolStripMenuItem, Me.toolStripAbout, Me.toolStripDonate, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(722, 24)
        Me.MenuStrip1.TabIndex = 36
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'toolUtilities
        '
        Me.toolUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ManuallyFixSystemRestoreToolStripMenuItem, Me.MountVolumeShadowCopyToolStripMenuItem, Me.CheckWindowsPowerPlanSettingsToolStripMenuItem, Me.toolStripDeleteRestorePoints, Me.toolStripViewDiskSpaceUsage, Me.toolStripManageSystemRestoreStorageSize, Me.toolStripScheduleRestorePoints, Me.FixRuntimeTasksToolStripMenuItem, Me.ProgramEventLogToolStripMenuItem, Me.RebootSystemToolStripMenuItem, Me.toolStripCheckForUpdates})
        Me.toolUtilities.Name = "toolUtilities"
        Me.toolUtilities.Size = New System.Drawing.Size(172, 20)
        Me.toolUtilities.Text = "System Restore Point &Utilities"
        '
        'ManuallyFixSystemRestoreToolStripMenuItem
        '
        Me.ManuallyFixSystemRestoreToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.hammer
        Me.ManuallyFixSystemRestoreToolStripMenuItem.Name = "ManuallyFixSystemRestoreToolStripMenuItem"
        Me.ManuallyFixSystemRestoreToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.ManuallyFixSystemRestoreToolStripMenuItem.Text = "Manually Fix System Restore"
        '
        'MountVolumeShadowCopyToolStripMenuItem
        '
        Me.MountVolumeShadowCopyToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.folder_explore
        Me.MountVolumeShadowCopyToolStripMenuItem.Name = "MountVolumeShadowCopyToolStripMenuItem"
        Me.MountVolumeShadowCopyToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.MountVolumeShadowCopyToolStripMenuItem.Text = "Mount Volume Shadow Copy"
        '
        'CheckWindowsPowerPlanSettingsToolStripMenuItem
        '
        Me.CheckWindowsPowerPlanSettingsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.plug
        Me.CheckWindowsPowerPlanSettingsToolStripMenuItem.Name = "CheckWindowsPowerPlanSettingsToolStripMenuItem"
        Me.CheckWindowsPowerPlanSettingsToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.CheckWindowsPowerPlanSettingsToolStripMenuItem.Text = "Check Windows Power Plan Settings"
        '
        'toolStripDeleteRestorePoints
        '
        Me.toolStripDeleteRestorePoints.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripDeleteAllRestorePoints, Me.toolStripDeleteOldRestorePoints})
        Me.toolStripDeleteRestorePoints.Image = Global.Restore_Point_Creator.My.Resources.Resources.delete
        Me.toolStripDeleteRestorePoints.Name = "toolStripDeleteRestorePoints"
        Me.toolStripDeleteRestorePoints.Size = New System.Drawing.Size(308, 22)
        Me.toolStripDeleteRestorePoints.Text = "Delete Restore Points"
        '
        'toolStripDeleteAllRestorePoints
        '
        Me.toolStripDeleteAllRestorePoints.Image = Global.Restore_Point_Creator.My.Resources.Resources.errorIcon
        Me.toolStripDeleteAllRestorePoints.Name = "toolStripDeleteAllRestorePoints"
        Me.toolStripDeleteAllRestorePoints.Size = New System.Drawing.Size(202, 22)
        Me.toolStripDeleteAllRestorePoints.Text = "Delete All Restore Points"
        '
        'toolStripDeleteOldRestorePoints
        '
        Me.toolStripDeleteOldRestorePoints.Image = Global.Restore_Point_Creator.My.Resources.Resources.chronometer
        Me.toolStripDeleteOldRestorePoints.Name = "toolStripDeleteOldRestorePoints"
        Me.toolStripDeleteOldRestorePoints.Size = New System.Drawing.Size(202, 22)
        Me.toolStripDeleteOldRestorePoints.Text = "Old Restore Points"
        '
        'toolStripViewDiskSpaceUsage
        '
        Me.toolStripViewDiskSpaceUsage.Image = Global.Restore_Point_Creator.My.Resources.Resources.disk_space
        Me.toolStripViewDiskSpaceUsage.Name = "toolStripViewDiskSpaceUsage"
        Me.toolStripViewDiskSpaceUsage.Size = New System.Drawing.Size(308, 22)
        Me.toolStripViewDiskSpaceUsage.Text = "&View System Disk Space Usage"
        '
        'toolStripManageSystemRestoreStorageSize
        '
        Me.toolStripManageSystemRestoreStorageSize.Image = Global.Restore_Point_Creator.My.Resources.Resources.settings
        Me.toolStripManageSystemRestoreStorageSize.Name = "toolStripManageSystemRestoreStorageSize"
        Me.toolStripManageSystemRestoreStorageSize.Size = New System.Drawing.Size(308, 22)
        Me.toolStripManageSystemRestoreStorageSize.Text = "&Manage System Restore Point Storage Space"
        '
        'toolStripScheduleRestorePoints
        '
        Me.toolStripScheduleRestorePoints.Image = Global.Restore_Point_Creator.My.Resources.Resources.chronometer
        Me.toolStripScheduleRestorePoints.Name = "toolStripScheduleRestorePoints"
        Me.toolStripScheduleRestorePoints.Size = New System.Drawing.Size(308, 22)
        Me.toolStripScheduleRestorePoints.Text = "Schedule creation of System Restore Points"
        '
        'FixRuntimeTasksToolStripMenuItem
        '
        Me.FixRuntimeTasksToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.tools
        Me.FixRuntimeTasksToolStripMenuItem.Name = "FixRuntimeTasksToolStripMenuItem"
        Me.FixRuntimeTasksToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.FixRuntimeTasksToolStripMenuItem.Text = "Fix Runtime Tasks"
        '
        'ProgramEventLogToolStripMenuItem
        '
        Me.ProgramEventLogToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.textBlock
        Me.ProgramEventLogToolStripMenuItem.Name = "ProgramEventLogToolStripMenuItem"
        Me.ProgramEventLogToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.ProgramEventLogToolStripMenuItem.Text = "Application Event Log"
        '
        'RebootSystemToolStripMenuItem
        '
        Me.RebootSystemToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.reboot
        Me.RebootSystemToolStripMenuItem.Name = "RebootSystemToolStripMenuItem"
        Me.RebootSystemToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.RebootSystemToolStripMenuItem.Text = "Reboot Computer"
        '
        'toolStripCheckForUpdates
        '
        Me.toolStripCheckForUpdates.Image = Global.Restore_Point_Creator.My.Resources.Resources.refresh
        Me.toolStripCheckForUpdates.Name = "toolStripCheckForUpdates"
        Me.toolStripCheckForUpdates.Size = New System.Drawing.Size(308, 22)
        Me.toolStripCheckForUpdates.Text = "Check for Updates"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RemoveSafeModeBootOptionToolStripMenuItem, Me.InterfaceTooBigToolStripMenuItem, Me.AdditionalOptionsAndSettingsToolStripMenuItem, Me.ConfigurationBackupRestoreToolStripMenuItem, Me.ToolStripMenuItem5, Me.SetBarColorToolStripMenuItem, Me.SetPleaseWaitBorderColorToolStripMenuItem, Me.barBelowColorSettings, Me.ExtendedDebugToolStripMenuItem, Me.RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem, Me.toolStripClear, Me.toolStripMyComputer, Me.CreateRestorePointAtUserLogonToolStripMenuItem, Me.ConfigureProxyToolStripMenuItem, Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem, Me.KeepXAmountOfRestorePointsToolStripMenuItem, Me.DefaultCustomRestorePointNameToolStripMenuItem, Me.ConfigureHTTPTimeoutToolStripMenuItem, Me.ToolStripMenuItem3, Me.SoftwareUpdateSettingsToolStripMenuItem, Me.NotificationOptionsToolStripMenuItem, Me.UseSSLToolStripMenuItem, Me.lineBeforeDebugMenuItem, Me.SwitchToDebugBuildToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.settings
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(173, 20)
        Me.OptionsToolStripMenuItem.Text = "Program &Options/Settings"
        '
        'RemoveSafeModeBootOptionToolStripMenuItem
        '
        Me.RemoveSafeModeBootOptionToolStripMenuItem.Name = "RemoveSafeModeBootOptionToolStripMenuItem"
        Me.RemoveSafeModeBootOptionToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.RemoveSafeModeBootOptionToolStripMenuItem.Text = "Stuck in Safe Mode? Remove Safe Mode Boot Option"
        '
        'InterfaceTooBigToolStripMenuItem
        '
        Me.InterfaceTooBigToolStripMenuItem.CheckOnClick = True
        Me.InterfaceTooBigToolStripMenuItem.Name = "InterfaceTooBigToolStripMenuItem"
        Me.InterfaceTooBigToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.InterfaceTooBigToolStripMenuItem.Text = "Interface too big? Enable this option"
        '
        'AdditionalOptionsAndSettingsToolStripMenuItem
        '
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem, Me.toolStripConfirmDeletions, Me.ConfirmRestorePointDeletionsInBatchesToolStripMenuItem, Me.toolStripLogRestorePointDeletions, Me.toolStripCloseAfterRestorePointIsCreated, Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem, Me.BypassNoUACLauncherToolStripMenuItem, Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem, Me.ExtendedLoggingForScheduledTasks, Me.AskBeforeCreatingRestorePointToolStripMenuItem, Me.chkShowVersionInTitleBarToolStripMenuItem, Me.EnableAdvancedDebugModeToolStripMenuItem})
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.increase
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Name = "AdditionalOptionsAndSettingsToolStripMenuItem"
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Text = "Additional Options and Settings"
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.ToolTipText = "Lots more options and setting are stored here."
        '
        'CheckSystemDrivesForFullShadowStorageToolStripMenuItem
        '
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.CheckOnClick = True
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Name = "CheckSystemDrivesForFullShadowStorageToolStripMenuItem"
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Text = "Check Windows Drive for Full Shadow Storage at Program Launch"
        '
        'toolStripConfirmDeletions
        '
        Me.toolStripConfirmDeletions.CheckOnClick = True
        Me.toolStripConfirmDeletions.Name = "toolStripConfirmDeletions"
        Me.toolStripConfirmDeletions.Size = New System.Drawing.Size(435, 22)
        Me.toolStripConfirmDeletions.Text = "Confirm Restore Point &Deletions"
        '
        'ConfirmRestorePointDeletionsInBatchesToolStripMenuItem
        '
        Me.ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.CheckOnClick = True
        Me.ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.Name = "ConfirmRestorePointDeletionsInBatchesToolStripMenuItem"
        Me.ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.ConfirmRestorePointDeletionsInBatchesToolStripMenuItem.Text = "Confirm Restore Point Deletions in Batches"
        '
        'toolStripLogRestorePointDeletions
        '
        Me.toolStripLogRestorePointDeletions.CheckOnClick = True
        Me.toolStripLogRestorePointDeletions.Name = "toolStripLogRestorePointDeletions"
        Me.toolStripLogRestorePointDeletions.Size = New System.Drawing.Size(435, 22)
        Me.toolStripLogRestorePointDeletions.Text = "&Log System Restore Point Deletions"
        '
        'toolStripCloseAfterRestorePointIsCreated
        '
        Me.toolStripCloseAfterRestorePointIsCreated.CheckOnClick = True
        Me.toolStripCloseAfterRestorePointIsCreated.Name = "toolStripCloseAfterRestorePointIsCreated"
        Me.toolStripCloseAfterRestorePointIsCreated.Size = New System.Drawing.Size(435, 22)
        Me.toolStripCloseAfterRestorePointIsCreated.Text = "Close Program After Restore Point is Created"
        '
        'AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem
        '
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Name = "AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem"
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Text = "Allow for deletion of all System Restore Points"
        '
        'BypassNoUACLauncherToolStripMenuItem
        '
        Me.BypassNoUACLauncherToolStripMenuItem.CheckOnClick = True
        Me.BypassNoUACLauncherToolStripMenuItem.Name = "BypassNoUACLauncherToolStripMenuItem"
        Me.BypassNoUACLauncherToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.BypassNoUACLauncherToolStripMenuItem.Text = "Bypass No UAC Launcher"
        '
        'LogProgramLoadsAndExitsToEventLogToolStripMenuItem
        '
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.CheckOnClick = True
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Name = "LogProgramLoadsAndExitsToEventLogToolStripMenuItem"
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Text = "Log Program Loads and Exits to Event Log"
        '
        'ExtendedLoggingForScheduledTasks
        '
        Me.ExtendedLoggingForScheduledTasks.CheckOnClick = True
        Me.ExtendedLoggingForScheduledTasks.Name = "ExtendedLoggingForScheduledTasks"
        Me.ExtendedLoggingForScheduledTasks.Size = New System.Drawing.Size(435, 22)
        Me.ExtendedLoggingForScheduledTasks.Text = "Enable Extended Logging for Scheduled Restore Point Creation Tasks"
        '
        'AskBeforeCreatingRestorePointToolStripMenuItem
        '
        Me.AskBeforeCreatingRestorePointToolStripMenuItem.CheckOnClick = True
        Me.AskBeforeCreatingRestorePointToolStripMenuItem.Name = "AskBeforeCreatingRestorePointToolStripMenuItem"
        Me.AskBeforeCreatingRestorePointToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.AskBeforeCreatingRestorePointToolStripMenuItem.Text = "Ask Before Creating Restore Point"
        '
        'chkShowVersionInTitleBarToolStripMenuItem
        '
        Me.chkShowVersionInTitleBarToolStripMenuItem.CheckOnClick = True
        Me.chkShowVersionInTitleBarToolStripMenuItem.Name = "chkShowVersionInTitleBarToolStripMenuItem"
        Me.chkShowVersionInTitleBarToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.chkShowVersionInTitleBarToolStripMenuItem.Text = "Show Version in Title Bar"
        '
        'EnableAdvancedDebugModeToolStripMenuItem
        '
        Me.EnableAdvancedDebugModeToolStripMenuItem.CheckOnClick = True
        Me.EnableAdvancedDebugModeToolStripMenuItem.Name = "EnableAdvancedDebugModeToolStripMenuItem"
        Me.EnableAdvancedDebugModeToolStripMenuItem.Size = New System.Drawing.Size(435, 22)
        Me.EnableAdvancedDebugModeToolStripMenuItem.Text = "Enable Advanced Debug Mode"
        Me.EnableAdvancedDebugModeToolStripMenuItem.Visible = False
        '
        'ConfigurationBackupRestoreToolStripMenuItem
        '
        Me.ConfigurationBackupRestoreToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.BackupToolStripMenuItem, Me.RestoreToolStripMenuItem})
        Me.ConfigurationBackupRestoreToolStripMenuItem.Name = "ConfigurationBackupRestoreToolStripMenuItem"
        Me.ConfigurationBackupRestoreToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.ConfigurationBackupRestoreToolStripMenuItem.Text = "Configuration Backup/Restore"
        '
        'BackupToolStripMenuItem
        '
        Me.BackupToolStripMenuItem.Name = "BackupToolStripMenuItem"
        Me.BackupToolStripMenuItem.Size = New System.Drawing.Size(113, 22)
        Me.BackupToolStripMenuItem.Text = "Backup"
        '
        'RestoreToolStripMenuItem
        '
        Me.RestoreToolStripMenuItem.Name = "RestoreToolStripMenuItem"
        Me.RestoreToolStripMenuItem.Size = New System.Drawing.Size(113, 22)
        Me.RestoreToolStripMenuItem.Text = "Restore"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(428, 6)
        '
        'SetBarColorToolStripMenuItem
        '
        Me.SetBarColorToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.color_wheel
        Me.SetBarColorToolStripMenuItem.Name = "SetBarColorToolStripMenuItem"
        Me.SetBarColorToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SetBarColorToolStripMenuItem.Text = "Set Progress Bar Color"
        '
        'SetPleaseWaitBorderColorToolStripMenuItem
        '
        Me.SetPleaseWaitBorderColorToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.color_wheel
        Me.SetPleaseWaitBorderColorToolStripMenuItem.Name = "SetPleaseWaitBorderColorToolStripMenuItem"
        Me.SetPleaseWaitBorderColorToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SetPleaseWaitBorderColorToolStripMenuItem.Text = "Set Please Wait Border Color"
        '
        'barBelowColorSettings
        '
        Me.barBelowColorSettings.Name = "barBelowColorSettings"
        Me.barBelowColorSettings.Size = New System.Drawing.Size(428, 6)
        '
        'ExtendedDebugToolStripMenuItem
        '
        Me.ExtendedDebugToolStripMenuItem.CheckOnClick = True
        Me.ExtendedDebugToolStripMenuItem.Name = "ExtendedDebugToolStripMenuItem"
        Me.ExtendedDebugToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.ExtendedDebugToolStripMenuItem.Text = "Enable Extended Debug Logging"
        '
        'RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem
        '
        Me.RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.calculator
        Me.RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem.Name = "RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem"
        Me.RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem.Text = "Round the age of restore point in days to how many decimals?"
        '
        'toolStripClear
        '
        Me.toolStripClear.Image = Global.Restore_Point_Creator.My.Resources.Resources.edit_clear
        Me.toolStripClear.Name = "toolStripClear"
        Me.toolStripClear.Size = New System.Drawing.Size(431, 22)
        Me.toolStripClear.Text = "&Clear Saved ""Delete Old Restore Points"" Setting"
        '
        'toolStripMyComputer
        '
        Me.toolStripMyComputer.CheckOnClick = True
        Me.toolStripMyComputer.Image = Global.Restore_Point_Creator.My.Resources.Resources.mycomputer
        Me.toolStripMyComputer.Name = "toolStripMyComputer"
        Me.toolStripMyComputer.Size = New System.Drawing.Size(431, 22)
        Me.toolStripMyComputer.Text = "&Enable ""My Computer"" Right-Click Option"
        '
        'CreateRestorePointAtUserLogonToolStripMenuItem
        '
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.login
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Name = "CreateRestorePointAtUserLogonToolStripMenuItem"
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Text = "Create Restore Point at User Logon"
        '
        'ConfigureProxyToolStripMenuItem
        '
        Me.ConfigureProxyToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.network
        Me.ConfigureProxyToolStripMenuItem.Name = "ConfigureProxyToolStripMenuItem"
        Me.ConfigureProxyToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.ConfigureProxyToolStripMenuItem.Text = "Configure Proxy"
        '
        'SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem
        '
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.plug
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Name = "SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem"
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Text = "Set Windows Power Plan for Wake Timers back to default (Disabled)"
        '
        'KeepXAmountOfRestorePointsToolStripMenuItem
        '
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.CheckOnClick = True
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.question
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Name = "KeepXAmountOfRestorePointsToolStripMenuItem"
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Text = "Keep X Amount of Restore Points"
        '
        'DefaultCustomRestorePointNameToolStripMenuItem
        '
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.keyboard
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Name = "DefaultCustomRestorePointNameToolStripMenuItem"
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Text = "Set Default Custom Restore Point Name"
        '
        'ConfigureHTTPTimeoutToolStripMenuItem
        '
        Me.ConfigureHTTPTimeoutToolStripMenuItem.Name = "ConfigureHTTPTimeoutToolStripMenuItem"
        Me.ConfigureHTTPTimeoutToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.ConfigureHTTPTimeoutToolStripMenuItem.Text = "Configure HTTP Timeout"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(428, 6)
        '
        'SoftwareUpdateSettingsToolStripMenuItem
        '
        Me.SoftwareUpdateSettingsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripAutomaticallyCheckForUpdates, Me.AskBeforeUpgradingUpdatingToolStripMenuItem, Me.EnableExtendedLoggingToolStripMenuItem, Me.ConfigureAutomaticUpdatesToolStripMenuItem, Me.UpdateChannelToolStripMenuItem})
        Me.SoftwareUpdateSettingsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.down
        Me.SoftwareUpdateSettingsToolStripMenuItem.Name = "SoftwareUpdateSettingsToolStripMenuItem"
        Me.SoftwareUpdateSettingsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SoftwareUpdateSettingsToolStripMenuItem.Text = "Software Update Settings"
        '
        'toolStripAutomaticallyCheckForUpdates
        '
        Me.toolStripAutomaticallyCheckForUpdates.CheckOnClick = True
        Me.toolStripAutomaticallyCheckForUpdates.Name = "toolStripAutomaticallyCheckForUpdates"
        Me.toolStripAutomaticallyCheckForUpdates.Size = New System.Drawing.Size(352, 22)
        Me.toolStripAutomaticallyCheckForUpdates.Text = "Automatically &Check for Updates at Program Launch"
        '
        'AskBeforeUpgradingUpdatingToolStripMenuItem
        '
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.CheckOnClick = True
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.Name = "AskBeforeUpgradingUpdatingToolStripMenuItem"
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.Size = New System.Drawing.Size(352, 22)
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.Text = "Ask Before Upgrading/Updating"
        '
        'EnableExtendedLoggingToolStripMenuItem
        '
        Me.EnableExtendedLoggingToolStripMenuItem.CheckOnClick = True
        Me.EnableExtendedLoggingToolStripMenuItem.Name = "EnableExtendedLoggingToolStripMenuItem"
        Me.EnableExtendedLoggingToolStripMenuItem.Size = New System.Drawing.Size(352, 22)
        Me.EnableExtendedLoggingToolStripMenuItem.Text = "Enable Extended Logging"
        '
        'ConfigureAutomaticUpdatesToolStripMenuItem
        '
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripCheckEveryWeek, Me.toolStripCheckEveryTwoWeeks, Me.toolStripCheckCustom})
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.chronometer
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Name = "ConfigureAutomaticUpdatesToolStripMenuItem"
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Size = New System.Drawing.Size(352, 22)
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Text = "Update Checking Interval"
        '
        'toolStripCheckEveryWeek
        '
        Me.toolStripCheckEveryWeek.CheckOnClick = True
        Me.toolStripCheckEveryWeek.Name = "toolStripCheckEveryWeek"
        Me.toolStripCheckEveryWeek.Size = New System.Drawing.Size(188, 22)
        Me.toolStripCheckEveryWeek.Text = "Every one week"
        '
        'toolStripCheckEveryTwoWeeks
        '
        Me.toolStripCheckEveryTwoWeeks.CheckOnClick = True
        Me.toolStripCheckEveryTwoWeeks.Name = "toolStripCheckEveryTwoWeeks"
        Me.toolStripCheckEveryTwoWeeks.Size = New System.Drawing.Size(188, 22)
        Me.toolStripCheckEveryTwoWeeks.Text = "Every two weeks"
        '
        'toolStripCheckCustom
        '
        Me.toolStripCheckCustom.CheckOnClick = True
        Me.toolStripCheckCustom.Name = "toolStripCheckCustom"
        Me.toolStripCheckCustom.Size = New System.Drawing.Size(188, 22)
        Me.toolStripCheckCustom.Text = "Custom Time Interval"
        '
        'UpdateChannelToolStripMenuItem
        '
        Me.UpdateChannelToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OnlyGiveMeReleaseCandidates, Me.lineUnderRC, Me.toolStripStableChannel, Me.toolStripBetaChannel, Me.ToolStripMenuItemPrivateForTom})
        Me.UpdateChannelToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.down
        Me.UpdateChannelToolStripMenuItem.Name = "UpdateChannelToolStripMenuItem"
        Me.UpdateChannelToolStripMenuItem.Size = New System.Drawing.Size(352, 22)
        Me.UpdateChannelToolStripMenuItem.Text = "Program Updates Channel"
        '
        'OnlyGiveMeReleaseCandidates
        '
        Me.OnlyGiveMeReleaseCandidates.CheckOnClick = True
        Me.OnlyGiveMeReleaseCandidates.Name = "OnlyGiveMeReleaseCandidates"
        Me.OnlyGiveMeReleaseCandidates.Size = New System.Drawing.Size(248, 22)
        Me.OnlyGiveMeReleaseCandidates.Text = "Only give me Release Candidates"
        '
        'lineUnderRC
        '
        Me.lineUnderRC.Name = "lineUnderRC"
        Me.lineUnderRC.Size = New System.Drawing.Size(245, 6)
        '
        'toolStripStableChannel
        '
        Me.toolStripStableChannel.CheckOnClick = True
        Me.toolStripStableChannel.Name = "toolStripStableChannel"
        Me.toolStripStableChannel.Size = New System.Drawing.Size(248, 22)
        Me.toolStripStableChannel.Text = "Stable"
        '
        'toolStripBetaChannel
        '
        Me.toolStripBetaChannel.CheckOnClick = True
        Me.toolStripBetaChannel.Name = "toolStripBetaChannel"
        Me.toolStripBetaChannel.Size = New System.Drawing.Size(248, 22)
        Me.toolStripBetaChannel.Text = "Public Betas"
        '
        'ToolStripMenuItemPrivateForTom
        '
        Me.ToolStripMenuItemPrivateForTom.CheckOnClick = True
        Me.ToolStripMenuItemPrivateForTom.Name = "ToolStripMenuItemPrivateForTom"
        Me.ToolStripMenuItemPrivateForTom.Size = New System.Drawing.Size(248, 22)
        Me.ToolStripMenuItemPrivateForTom.Text = "Private for Tom"
        Me.ToolStripMenuItemPrivateForTom.Visible = False
        '
        'NotificationOptionsToolStripMenuItem
        '
        Me.NotificationOptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.NotificationTypeToolStripMenuItem, Me.ToolStripMenuItem4, Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem, Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem})
        Me.NotificationOptionsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.notifications
        Me.NotificationOptionsToolStripMenuItem.Name = "NotificationOptionsToolStripMenuItem"
        Me.NotificationOptionsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.NotificationOptionsToolStripMenuItem.Text = "Message Box Notification Options"
        '
        'NotificationTypeToolStripMenuItem
        '
        Me.NotificationTypeToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MessageBoxToolStripMenuItem, Me.BalloonToolStripMenuItem})
        Me.NotificationTypeToolStripMenuItem.Name = "NotificationTypeToolStripMenuItem"
        Me.NotificationTypeToolStripMenuItem.Size = New System.Drawing.Size(374, 22)
        Me.NotificationTypeToolStripMenuItem.Text = "Notification Type"
        '
        'MessageBoxToolStripMenuItem
        '
        Me.MessageBoxToolStripMenuItem.CheckOnClick = True
        Me.MessageBoxToolStripMenuItem.Name = "MessageBoxToolStripMenuItem"
        Me.MessageBoxToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
        Me.MessageBoxToolStripMenuItem.Text = "Message Box"
        '
        'BalloonToolStripMenuItem
        '
        Me.BalloonToolStripMenuItem.CheckOnClick = True
        Me.BalloonToolStripMenuItem.Name = "BalloonToolStripMenuItem"
        Me.BalloonToolStripMenuItem.Size = New System.Drawing.Size(142, 22)
        Me.BalloonToolStripMenuItem.Text = "Balloon"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(371, 6)
        '
        'ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem
        '
        Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.CheckOnClick = True
        Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Name = "ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem"
        Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Size = New System.Drawing.Size(374, 22)
        Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem.Text = "Show message after successful creation of Restore Point"
        '
        'ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem
        '
        Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.CheckOnClick = True
        Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Name = "ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem"
        Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Size = New System.Drawing.Size(374, 22)
        Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem.Text = "Show message after successful deletion of Restore Points"
        '
        'UseSSLToolStripMenuItem
        '
        Me.UseSSLToolStripMenuItem.CheckOnClick = True
        Me.UseSSLToolStripMenuItem.Name = "UseSSLToolStripMenuItem"
        Me.UseSSLToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.UseSSLToolStripMenuItem.Text = "Use SSL (Recommended)"
        Me.UseSSLToolStripMenuItem.ToolTipText = "Enables and disables SSL mode to for many of the things that this program does" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "w" &
    "hen contacting my web site (check for updates, download updates, etc.)."
        '
        'lineBeforeDebugMenuItem
        '
        Me.lineBeforeDebugMenuItem.Name = "lineBeforeDebugMenuItem"
        Me.lineBeforeDebugMenuItem.Size = New System.Drawing.Size(428, 6)
        '
        'SwitchToDebugBuildToolStripMenuItem
        '
        Me.SwitchToDebugBuildToolStripMenuItem.Name = "SwitchToDebugBuildToolStripMenuItem"
        Me.SwitchToDebugBuildToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SwitchToDebugBuildToolStripMenuItem.Text = "Switch to Debug Build (Helpful in debugging program crashes)"
        '
        'toolStripAbout
        '
        Me.toolStripAbout.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutThisProgramToolStripMenuItem, Me.FrequentlyAskedQuestionsToolStripMenuItem, Me.ProductWebSiteToolStripMenuItem, Me.ContactTheDeveloperToolStripMenuItem, Me.ViewOfficialChangeLogToolStripMenuItem1, Me.CommandLineArgumentsToolStripMenuItem})
        Me.toolStripAbout.Name = "toolStripAbout"
        Me.toolStripAbout.Size = New System.Drawing.Size(52, 20)
        Me.toolStripAbout.Text = "&About"
        '
        'AboutThisProgramToolStripMenuItem
        '
        Me.AboutThisProgramToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.info_blue
        Me.AboutThisProgramToolStripMenuItem.Name = "AboutThisProgramToolStripMenuItem"
        Me.AboutThisProgramToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.AboutThisProgramToolStripMenuItem.Text = "About this Program"
        '
        'FrequentlyAskedQuestionsToolStripMenuItem
        '
        Me.FrequentlyAskedQuestionsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.system_question
        Me.FrequentlyAskedQuestionsToolStripMenuItem.Name = "FrequentlyAskedQuestionsToolStripMenuItem"
        Me.FrequentlyAskedQuestionsToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.FrequentlyAskedQuestionsToolStripMenuItem.Text = "Frequently Asked Questions"
        '
        'ProductWebSiteToolStripMenuItem
        '
        Me.ProductWebSiteToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.website
        Me.ProductWebSiteToolStripMenuItem.Name = "ProductWebSiteToolStripMenuItem"
        Me.ProductWebSiteToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.ProductWebSiteToolStripMenuItem.Text = "Open Product Web Site"
        '
        'ContactTheDeveloperToolStripMenuItem
        '
        Me.ContactTheDeveloperToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.contact
        Me.ContactTheDeveloperToolStripMenuItem.Name = "ContactTheDeveloperToolStripMenuItem"
        Me.ContactTheDeveloperToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.ContactTheDeveloperToolStripMenuItem.Text = "Contact the Developer"
        '
        'ViewOfficialChangeLogToolStripMenuItem1
        '
        Me.ViewOfficialChangeLogToolStripMenuItem1.Image = Global.Restore_Point_Creator.My.Resources.Resources.textBlock
        Me.ViewOfficialChangeLogToolStripMenuItem1.Name = "ViewOfficialChangeLogToolStripMenuItem1"
        Me.ViewOfficialChangeLogToolStripMenuItem1.Size = New System.Drawing.Size(256, 22)
        Me.ViewOfficialChangeLogToolStripMenuItem1.Text = "View Official Program Change Log"
        '
        'CommandLineArgumentsToolStripMenuItem
        '
        Me.CommandLineArgumentsToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.terminal
        Me.CommandLineArgumentsToolStripMenuItem.Name = "CommandLineArgumentsToolStripMenuItem"
        Me.CommandLineArgumentsToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.CommandLineArgumentsToolStripMenuItem.Text = "Command Line Arguments"
        '
        'toolStripDonate
        '
        Me.toolStripDonate.Image = Global.Restore_Point_Creator.My.Resources.Resources.green_dollar
        Me.toolStripDonate.Name = "toolStripDonate"
        Me.toolStripDonate.Size = New System.Drawing.Size(163, 20)
        Me.toolStripDonate.Text = "Donate to the Developer"
        '
        'HelpToolStripMenuItem
        '
        Me.HelpToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.movies
        Me.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem"
        Me.HelpToolStripMenuItem.Size = New System.Drawing.Size(139, 20)
        Me.HelpToolStripMenuItem.Text = "Instructional Videos"
        '
        'Label5
        '
        Me.Label5.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label5.BackColor = System.Drawing.Color.Black
        Me.Label5.Location = New System.Drawing.Point(0, 24)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(725, 1)
        Me.Label5.TabIndex = 37
        Me.Label5.Text = "Label5"
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.BackColor = System.Drawing.Color.Black
        Me.Label2.Location = New System.Drawing.Point(-1, 90)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(725, 1)
        Me.Label2.TabIndex = 38
        Me.Label2.Text = "Label2"
        '
        'FfgdToolStripMenuItem
        '
        Me.FfgdToolStripMenuItem.Name = "FfgdToolStripMenuItem"
        Me.FfgdToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.FfgdToolStripMenuItem.Text = "ffgd"
        '
        'FgdfgdToolStripMenuItem
        '
        Me.FgdfgdToolStripMenuItem.Name = "FgdfgdToolStripMenuItem"
        Me.FgdfgdToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.FgdfgdToolStripMenuItem.Text = "fgdfgd"
        '
        'NotifyIcon1
        '
        Me.NotifyIcon1.Text = "Restore Point Creator"
        Me.NotifyIcon1.Visible = True
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 1000
        '
        'importBackupDialog
        '
        Me.importBackupDialog.FileName = "OpenFileDialog1"
        '
        'pleaseWaitPanel
        '
        Me.pleaseWaitPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitBorderText)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitlblLabel)
        Me.pleaseWaitPanel.Controls.Add(Me.pleaseWaitProgressBar)
        Me.pleaseWaitPanel.Location = New System.Drawing.Point(216, 160)
        Me.pleaseWaitPanel.Name = "pleaseWaitPanel"
        Me.pleaseWaitPanel.Size = New System.Drawing.Size(293, 86)
        Me.pleaseWaitPanel.TabIndex = 40
        Me.pleaseWaitPanel.Visible = False
        '
        'pleaseWaitBorderText
        '
        Me.pleaseWaitBorderText.BackColor = System.Drawing.Color.SkyBlue
        Me.pleaseWaitBorderText.Location = New System.Drawing.Point(0, 0)
        Me.pleaseWaitBorderText.Name = "pleaseWaitBorderText"
        Me.pleaseWaitBorderText.Padding = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me.pleaseWaitBorderText.Size = New System.Drawing.Size(292, 23)
        Me.pleaseWaitBorderText.TabIndex = 4
        Me.pleaseWaitBorderText.Text = "Please Wait..."
        Me.pleaseWaitBorderText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pleaseWaitlblLabel
        '
        Me.pleaseWaitlblLabel.AutoSize = True
        Me.pleaseWaitlblLabel.Location = New System.Drawing.Point(3, 31)
        Me.pleaseWaitlblLabel.Name = "pleaseWaitlblLabel"
        Me.pleaseWaitlblLabel.Size = New System.Drawing.Size(39, 13)
        Me.pleaseWaitlblLabel.TabIndex = 3
        Me.pleaseWaitlblLabel.Text = "Label1"
        '
        'pleaseWaitProgressBar
        '
        Me.pleaseWaitProgressBar.Location = New System.Drawing.Point(6, 56)
        Me.pleaseWaitProgressBar.Maximum = 100
        Me.pleaseWaitProgressBar.Minimum = 0
        Me.pleaseWaitProgressBar.Name = "pleaseWaitProgressBar"
        Me.pleaseWaitProgressBar.ProgressBarColor = System.Drawing.Color.Blue
        Me.pleaseWaitProgressBar.Size = New System.Drawing.Size(280, 19)
        Me.pleaseWaitProgressBar.TabIndex = 2
        Me.pleaseWaitProgressBar.Value = 0
        '
        'pleaseWaitProgressBarChanger
        '
        Me.pleaseWaitProgressBarChanger.Interval = 25
        '
        'pleaseWaitMessageChanger
        '
        Me.pleaseWaitMessageChanger.Interval = 250
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(722, 423)
        Me.Controls.Add(Me.pleaseWaitPanel)
        Me.Controls.Add(Me.btnCreateRestorePointNameWithDefaultName)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.btnCreateSystemCheckpoint)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblCurrentRestorePointsLabel)
        Me.Controls.Add(Me.systemRestorePointsList)
        Me.Controls.Add(Me.buttonTableLayout)
        Me.Controls.Add(Me.btnCreate)
        Me.Controls.Add(Me.txtRestorePointDescription)
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(738, 462)
        Me.Name = "Form1"
        Me.Text = "Restore Point Creator"
        Me.restorePointListContextMenu.ResumeLayout(False)
        Me.buttonTableLayout.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.pleaseWaitPanel.ResumeLayout(False)
        Me.pleaseWaitPanel.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtRestorePointDescription As System.Windows.Forms.TextBox
    Friend WithEvents btnCreate As System.Windows.Forms.Button
    Friend WithEvents systemRestorePointsList As System.Windows.Forms.ListView
    Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
    Friend WithEvents lblCurrentRestorePointsLabel As System.Windows.Forms.Label
    Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
    Friend WithEvents ToolTip As System.Windows.Forms.ToolTip
    Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
    Friend WithEvents btnCreateSystemCheckpoint As System.Windows.Forms.Button
    Friend WithEvents restorePointListContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents stripDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents stripRefresh As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents stripRestore As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripHeader As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem2 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents btnRestoreToRestorePoint As System.Windows.Forms.Button
    Friend WithEvents btnRefreshRestorePoints As System.Windows.Forms.Button
    Friend WithEvents buttonTableLayout As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnDeleteRestorePoint As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents toolUtilities As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripViewDiskSpaceUsage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripManageSystemRestoreStorageSize As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripScheduleRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents toolStripCheckForUpdates As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ProductWebSiteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutThisProgramToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDonate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FfgdToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FgdfgdToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDeleteRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDeleteOldRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDeleteAllRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ColorDialog As System.Windows.Forms.ColorDialog
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents btnCreateRestorePointNameWithDefaultName As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents ViewOfficialChangeLogToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents stripRestoreSafeMode As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnRestoreToRestorePointSafeMode As System.Windows.Forms.Button
    Friend WithEvents ProgramEventLogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContactTheDeveloperToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckWindowsPowerPlanSettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents importBackupDialog As OpenFileDialog
    Friend WithEvents exportBackupDialog As SaveFileDialog
    Friend WithEvents MountVolumeShadowCopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents OptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents InterfaceTooBigToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AdditionalOptionsAndSettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CheckSystemDrivesForFullShadowStorageToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripConfirmDeletions As ToolStripMenuItem
    Friend WithEvents toolStripLogRestorePointDeletions As ToolStripMenuItem
    Friend WithEvents toolStripCloseAfterRestorePointIsCreated As ToolStripMenuItem
    Friend WithEvents KeepXAmountOfRestorePointsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DefaultCustomRestorePointNameToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BypassNoUACLauncherToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents LogProgramLoadsAndExitsToEventLogToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SwitchToDebugBuildToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UseSSLToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As ToolStripSeparator
    Friend WithEvents SetBarColorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripClear As ToolStripMenuItem
    Friend WithEvents toolStripMyComputer As ToolStripMenuItem
    Friend WithEvents CreateRestorePointAtUserLogonToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigurationBackupRestoreToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BackupToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RestoreToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigureProxyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As ToolStripSeparator
    Friend WithEvents toolStripAutomaticallyCheckForUpdates As ToolStripMenuItem
    Friend WithEvents AskBeforeUpgradingUpdatingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigureAutomaticUpdatesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripCheckEveryWeek As ToolStripMenuItem
    Friend WithEvents toolStripCheckEveryTwoWeeks As ToolStripMenuItem
    Friend WithEvents toolStripCheckCustom As ToolStripMenuItem
    Friend WithEvents UpdateChannelToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents toolStripStableChannel As ToolStripMenuItem
    Friend WithEvents toolStripBetaChannel As ToolStripMenuItem
    Friend WithEvents NotificationOptionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents NotificationTypeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents MessageBoxToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BalloonToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As ToolStripSeparator
    Friend WithEvents ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SoftwareUpdateSettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RemoveSafeModeBootOptionToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents FrequentlyAskedQuestionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ColumnHeader5 As ColumnHeader
    Friend WithEvents RoundTheAgeOfRestorePointInDaysToHowManyDecimalsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents lineBeforeDebugMenuItem As ToolStripSeparator
    Friend WithEvents CommandLineArgumentsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItemPrivateForTom As ToolStripMenuItem
    Friend WithEvents OnlyGiveMeReleaseCandidates As ToolStripMenuItem
    Friend WithEvents lineUnderRC As ToolStripSeparator
    Friend WithEvents ExtendedLoggingForScheduledTasks As ToolStripMenuItem
    Friend WithEvents FixRuntimeTasksToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AskBeforeCreatingRestorePointToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RebootSystemToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EnableExtendedLoggingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkShowVersionInTitleBarToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigureHTTPTimeoutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfirmRestorePointDeletionsInBatchesToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ManuallyFixSystemRestoreToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents pleaseWaitPanel As Panel
    Friend WithEvents pleaseWaitProgressBarChanger As Timer
    Friend WithEvents pleaseWaitBorderText As Label
    Friend WithEvents pleaseWaitlblLabel As Label
    Friend WithEvents pleaseWaitProgressBar As SmoothProgressBar
    Friend WithEvents pleaseWaitMessageChanger As Timer
    Friend WithEvents SetPleaseWaitBorderColorToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents barBelowColorSettings As ToolStripSeparator
    Friend WithEvents ExtendedDebugToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EnableAdvancedDebugModeToolStripMenuItem As ToolStripMenuItem
End Class
