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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtRestorePointDescription = New System.Windows.Forms.TextBox()
        Me.btnCreate = New System.Windows.Forms.Button()
        Me.systemRestorePointsList = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
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
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.btnRestoreToRestorePointSafeMode = New System.Windows.Forms.Button()
        Me.btnDeleteRestorePoint = New System.Windows.Forms.Button()
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.toolUtilities = New System.Windows.Forms.ToolStripMenuItem()
        Me.MountVolumeShadowCopyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckWindowsPowerPlanSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDeleteRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDeleteAllRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDeleteOldRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripViewDiskSpaceUsage = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripManageSystemRestoreStorageSize = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripScheduleRestorePoints = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProgramEventLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckForUpdates = New System.Windows.Forms.ToolStripMenuItem()
        Me.OptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetBarColorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripMyComputer = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateRestorePointAtUserLogonToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigurationBackupRestoreToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackupToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RestoreToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigureProxyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripSeparator()
        Me.toolStripAutomaticallyCheckForUpdates = New System.Windows.Forms.ToolStripMenuItem()
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ConfigureAutomaticUpdatesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckEveryWeek = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckEveryTwoWeeks = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCheckCustom = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateChannelToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripStableChannel = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripBetaChannel = New System.Windows.Forms.ToolStripMenuItem()
        Me.NotificationOptionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.NotificationTypeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MessageBoxToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BalloonToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EnableSystemEventLoggingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripConfirmDeletions = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripLogRestorePointDeletions = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripCloseAfterRestorePointIsCreated = New System.Windows.Forms.ToolStripMenuItem()
        Me.KeepXAmountOfRestorePointsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DefaultCustomRestorePointNameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BypassNoUACLauncherToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SwitchToDebugBuildToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UseSSLToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripAbout = New System.Windows.Forms.ToolStripMenuItem()
        Me.AboutThisProgramToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ProductWebSiteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContactTheDeveloperToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewOfficialChangeLogToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.toolStripDonate = New System.Windows.Forms.ToolStripMenuItem()
        Me.HelpToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.FfgdToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.FgdfgdToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ColorDialog = New System.Windows.Forms.ColorDialog()
        Me.NotifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.deleteProgressBar = New Tom.SmoothProgressBar()
        Me.importBackupDialog = New System.Windows.Forms.OpenFileDialog()
        Me.exportBackupDialog = New System.Windows.Forms.SaveFileDialog()
        Me.AdditionalOptionsAndSettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.restorePointListContextMenu.SuspendLayout()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.MenuStrip1.SuspendLayout()
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
        Me.txtRestorePointDescription.Size = New System.Drawing.Size(469, 20)
        Me.txtRestorePointDescription.TabIndex = 1
        Me.txtRestorePointDescription.TabStop = False
        Me.txtRestorePointDescription.Text = "Type in a name for your custom-named System Restore Point..."
        '
        'btnCreate
        '
        Me.btnCreate.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCreate.Enabled = False
        Me.btnCreate.Image = CType(resources.GetObject("btnCreate.Image"), System.Drawing.Image)
        Me.btnCreate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreate.Location = New System.Drawing.Point(482, 33)
        Me.btnCreate.Name = "btnCreate"
        Me.btnCreate.Size = New System.Drawing.Size(205, 22)
        Me.btnCreate.TabIndex = 2
        Me.btnCreate.Text = "Create Custom Named Restore Point"
        Me.btnCreate.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnCreate.UseVisualStyleBackColor = True
        '
        'systemRestorePointsList
        '
        Me.systemRestorePointsList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.systemRestorePointsList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader4})
        Me.systemRestorePointsList.ContextMenuStrip = Me.restorePointListContextMenu
        Me.systemRestorePointsList.FullRowSelect = True
        Me.systemRestorePointsList.Location = New System.Drawing.Point(7, 119)
        Me.systemRestorePointsList.Name = "systemRestorePointsList"
        Me.systemRestorePointsList.Size = New System.Drawing.Size(688, 317)
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
        Me.stripRefresh.Image = CType(resources.GetObject("stripRefresh.Image"), System.Drawing.Image)
        Me.stripRefresh.Name = "stripRefresh"
        Me.stripRefresh.Size = New System.Drawing.Size(319, 22)
        Me.stripRefresh.Text = "&Refresh List"
        '
        'stripDelete
        '
        Me.stripDelete.Enabled = False
        Me.stripDelete.Image = CType(resources.GetObject("stripDelete.Image"), System.Drawing.Image)
        Me.stripDelete.Name = "stripDelete"
        Me.stripDelete.Size = New System.Drawing.Size(319, 22)
        Me.stripDelete.Text = "&Delete Selected Restore Point"
        '
        'stripRestore
        '
        Me.stripRestore.Enabled = False
        Me.stripRestore.Image = CType(resources.GetObject("stripRestore.Image"), System.Drawing.Image)
        Me.stripRestore.Name = "stripRestore"
        Me.stripRestore.Size = New System.Drawing.Size(319, 22)
        Me.stripRestore.Text = "Restore to &Selected Restore Point"
        '
        'stripRestoreSafeMode
        '
        Me.stripRestoreSafeMode.Enabled = False
        Me.stripRestoreSafeMode.Image = Global.Restore_Point_Creator.My.Resources.Resources.restore
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
        Me.btnCreateSystemCheckpoint.Image = CType(resources.GetObject("btnCreateSystemCheckpoint.Image"), System.Drawing.Image)
        Me.btnCreateSystemCheckpoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnCreateSystemCheckpoint.Location = New System.Drawing.Point(482, 61)
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
        Me.btnCreateRestorePointNameWithDefaultName.Image = CType(resources.GetObject("btnCreateRestorePointNameWithDefaultName.Image"), System.Drawing.Image)
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
        Me.btnRestoreToRestorePoint.Image = CType(resources.GetObject("btnRestoreToRestorePoint.Image"), System.Drawing.Image)
        Me.btnRestoreToRestorePoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRestoreToRestorePoint.Location = New System.Drawing.Point(3, 37)
        Me.btnRestoreToRestorePoint.Name = "btnRestoreToRestorePoint"
        Me.btnRestoreToRestorePoint.Size = New System.Drawing.Size(339, 22)
        Me.btnRestoreToRestorePoint.TabIndex = 18
        Me.btnRestoreToRestorePoint.Text = "Restore to Selected System Restore Point"
        Me.btnRestoreToRestorePoint.UseVisualStyleBackColor = True
        '
        'btnRefreshRestorePoints
        '
        Me.btnRefreshRestorePoints.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRefreshRestorePoints.Image = CType(resources.GetObject("btnRefreshRestorePoints.Image"), System.Drawing.Image)
        Me.btnRefreshRestorePoints.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRefreshRestorePoints.Location = New System.Drawing.Point(3, 5)
        Me.btnRefreshRestorePoints.Name = "btnRefreshRestorePoints"
        Me.btnRefreshRestorePoints.Size = New System.Drawing.Size(339, 23)
        Me.btnRefreshRestorePoints.TabIndex = 14
        Me.btnRefreshRestorePoints.Text = "Refresh List of System Restore Points"
        Me.btnRefreshRestorePoints.UseVisualStyleBackColor = True
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.Controls.Add(Me.btnRestoreToRestorePointSafeMode, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.btnRefreshRestorePoints, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnDeleteRestorePoint, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.btnRestoreToRestorePoint, 0, 1)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(5, 442)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 2
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(690, 62)
        Me.TableLayoutPanel2.TabIndex = 31
        '
        'btnRestoreToRestorePointSafeMode
        '
        Me.btnRestoreToRestorePointSafeMode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRestoreToRestorePointSafeMode.Enabled = False
        Me.btnRestoreToRestorePointSafeMode.Image = CType(resources.GetObject("btnRestoreToRestorePointSafeMode.Image"), System.Drawing.Image)
        Me.btnRestoreToRestorePointSafeMode.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnRestoreToRestorePointSafeMode.Location = New System.Drawing.Point(348, 37)
        Me.btnRestoreToRestorePointSafeMode.Name = "btnRestoreToRestorePointSafeMode"
        Me.btnRestoreToRestorePointSafeMode.Size = New System.Drawing.Size(339, 22)
        Me.btnRestoreToRestorePointSafeMode.TabIndex = 19
        Me.btnRestoreToRestorePointSafeMode.Text = "Restore to Selected System Restore Point in Safe Mode"
        Me.btnRestoreToRestorePointSafeMode.UseVisualStyleBackColor = True
        '
        'btnDeleteRestorePoint
        '
        Me.btnDeleteRestorePoint.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDeleteRestorePoint.Enabled = False
        Me.btnDeleteRestorePoint.Image = CType(resources.GetObject("btnDeleteRestorePoint.Image"), System.Drawing.Image)
        Me.btnDeleteRestorePoint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteRestorePoint.Location = New System.Drawing.Point(348, 5)
        Me.btnDeleteRestorePoint.Name = "btnDeleteRestorePoint"
        Me.btnDeleteRestorePoint.Size = New System.Drawing.Size(339, 23)
        Me.btnDeleteRestorePoint.TabIndex = 13
        Me.btnDeleteRestorePoint.Text = "Delete Selected Restore Point"
        Me.btnDeleteRestorePoint.UseVisualStyleBackColor = True
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolUtilities, Me.OptionsToolStripMenuItem, Me.toolStripAbout, Me.toolStripDonate, Me.HelpToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(707, 24)
        Me.MenuStrip1.TabIndex = 36
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'toolUtilities
        '
        Me.toolUtilities.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.MountVolumeShadowCopyToolStripMenuItem, Me.CheckWindowsPowerPlanSettingsToolStripMenuItem, Me.toolStripDeleteRestorePoints, Me.toolStripViewDiskSpaceUsage, Me.toolStripManageSystemRestoreStorageSize, Me.toolStripScheduleRestorePoints, Me.ProgramEventLogToolStripMenuItem, Me.toolStripCheckForUpdates})
        Me.toolUtilities.Name = "toolUtilities"
        Me.toolUtilities.Size = New System.Drawing.Size(172, 20)
        Me.toolUtilities.Text = "System Restore Point &Utilities"
        '
        'MountVolumeShadowCopyToolStripMenuItem
        '
        Me.MountVolumeShadowCopyToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.explore
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
        Me.toolStripDeleteRestorePoints.Image = CType(resources.GetObject("toolStripDeleteRestorePoints.Image"), System.Drawing.Image)
        Me.toolStripDeleteRestorePoints.Name = "toolStripDeleteRestorePoints"
        Me.toolStripDeleteRestorePoints.Size = New System.Drawing.Size(308, 22)
        Me.toolStripDeleteRestorePoints.Text = "Delete Restore Points"
        '
        'toolStripDeleteAllRestorePoints
        '
        Me.toolStripDeleteAllRestorePoints.Image = CType(resources.GetObject("toolStripDeleteAllRestorePoints.Image"), System.Drawing.Image)
        Me.toolStripDeleteAllRestorePoints.Name = "toolStripDeleteAllRestorePoints"
        Me.toolStripDeleteAllRestorePoints.Size = New System.Drawing.Size(202, 22)
        Me.toolStripDeleteAllRestorePoints.Text = "Delete All Restore Points"
        '
        'toolStripDeleteOldRestorePoints
        '
        Me.toolStripDeleteOldRestorePoints.Image = CType(resources.GetObject("toolStripDeleteOldRestorePoints.Image"), System.Drawing.Image)
        Me.toolStripDeleteOldRestorePoints.Name = "toolStripDeleteOldRestorePoints"
        Me.toolStripDeleteOldRestorePoints.Size = New System.Drawing.Size(202, 22)
        Me.toolStripDeleteOldRestorePoints.Text = "Old Restore Points"
        '
        'toolStripViewDiskSpaceUsage
        '
        Me.toolStripViewDiskSpaceUsage.Image = CType(resources.GetObject("toolStripViewDiskSpaceUsage.Image"), System.Drawing.Image)
        Me.toolStripViewDiskSpaceUsage.Name = "toolStripViewDiskSpaceUsage"
        Me.toolStripViewDiskSpaceUsage.Size = New System.Drawing.Size(308, 22)
        Me.toolStripViewDiskSpaceUsage.Text = "&View System Disk Space Usage"
        '
        'toolStripManageSystemRestoreStorageSize
        '
        Me.toolStripManageSystemRestoreStorageSize.Image = CType(resources.GetObject("toolStripManageSystemRestoreStorageSize.Image"), System.Drawing.Image)
        Me.toolStripManageSystemRestoreStorageSize.Name = "toolStripManageSystemRestoreStorageSize"
        Me.toolStripManageSystemRestoreStorageSize.Size = New System.Drawing.Size(308, 22)
        Me.toolStripManageSystemRestoreStorageSize.Text = "&Manage System Restore Point Storage Space"
        '
        'toolStripScheduleRestorePoints
        '
        Me.toolStripScheduleRestorePoints.Image = CType(resources.GetObject("toolStripScheduleRestorePoints.Image"), System.Drawing.Image)
        Me.toolStripScheduleRestorePoints.Name = "toolStripScheduleRestorePoints"
        Me.toolStripScheduleRestorePoints.Size = New System.Drawing.Size(308, 22)
        Me.toolStripScheduleRestorePoints.Text = "Schedule creation of System Restore Points"
        '
        'ProgramEventLogToolStripMenuItem
        '
        Me.ProgramEventLogToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.fileIcon
        Me.ProgramEventLogToolStripMenuItem.Name = "ProgramEventLogToolStripMenuItem"
        Me.ProgramEventLogToolStripMenuItem.Size = New System.Drawing.Size(308, 22)
        Me.ProgramEventLogToolStripMenuItem.Text = "System Event Log"
        '
        'toolStripCheckForUpdates
        '
        Me.toolStripCheckForUpdates.Image = CType(resources.GetObject("toolStripCheckForUpdates.Image"), System.Drawing.Image)
        Me.toolStripCheckForUpdates.Name = "toolStripCheckForUpdates"
        Me.toolStripCheckForUpdates.Size = New System.Drawing.Size(308, 22)
        Me.toolStripCheckForUpdates.Text = "Check for Updates"
        '
        'OptionsToolStripMenuItem
        '
        Me.OptionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SetBarColorToolStripMenuItem, Me.toolStripClear, Me.toolStripMyComputer, Me.CreateRestorePointAtUserLogonToolStripMenuItem, Me.ConfigurationBackupRestoreToolStripMenuItem, Me.ConfigureProxyToolStripMenuItem, Me.ToolStripMenuItem3, Me.toolStripAutomaticallyCheckForUpdates, Me.AskBeforeUpgradingUpdatingToolStripMenuItem, Me.ConfigureAutomaticUpdatesToolStripMenuItem, Me.UpdateChannelToolStripMenuItem, Me.NotificationOptionsToolStripMenuItem, Me.ToolStripMenuItem1, Me.EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem, Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem, Me.EnableSystemEventLoggingToolStripMenuItem, Me.toolStripConfirmDeletions, Me.toolStripLogRestorePointDeletions, Me.toolStripCloseAfterRestorePointIsCreated, Me.KeepXAmountOfRestorePointsToolStripMenuItem, Me.DefaultCustomRestorePointNameToolStripMenuItem, Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem, Me.BypassNoUACLauncherToolStripMenuItem, Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem, Me.SwitchToDebugBuildToolStripMenuItem, Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem, Me.UseSSLToolStripMenuItem, Me.AdditionalOptionsAndSettingsToolStripMenuItem})
        Me.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem"
        Me.OptionsToolStripMenuItem.Overflow = System.Windows.Forms.ToolStripItemOverflow.AsNeeded
        Me.OptionsToolStripMenuItem.Size = New System.Drawing.Size(157, 20)
        Me.OptionsToolStripMenuItem.Text = "Program &Options/Settings"
        '
        'SetBarColorToolStripMenuItem
        '
        Me.SetBarColorToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.color_wheel
        Me.SetBarColorToolStripMenuItem.Name = "SetBarColorToolStripMenuItem"
        Me.SetBarColorToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SetBarColorToolStripMenuItem.Text = "Set Progress Bar Color"
        '
        'toolStripClear
        '
        Me.toolStripClear.Image = Global.Restore_Point_Creator.My.Resources.Resources.clear
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
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Name = "CreateRestorePointAtUserLogonToolStripMenuItem"
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.CreateRestorePointAtUserLogonToolStripMenuItem.Text = "Create Restore Point at User Logon"
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
        Me.BackupToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.BackupToolStripMenuItem.Text = "Backup"
        '
        'RestoreToolStripMenuItem
        '
        Me.RestoreToolStripMenuItem.Name = "RestoreToolStripMenuItem"
        Me.RestoreToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.RestoreToolStripMenuItem.Text = "Restore"
        '
        'ConfigureProxyToolStripMenuItem
        '
        Me.ConfigureProxyToolStripMenuItem.Name = "ConfigureProxyToolStripMenuItem"
        Me.ConfigureProxyToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.ConfigureProxyToolStripMenuItem.Text = "Configure Proxy"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(428, 6)
        '
        'toolStripAutomaticallyCheckForUpdates
        '
        Me.toolStripAutomaticallyCheckForUpdates.CheckOnClick = True
        Me.toolStripAutomaticallyCheckForUpdates.Name = "toolStripAutomaticallyCheckForUpdates"
        Me.toolStripAutomaticallyCheckForUpdates.Size = New System.Drawing.Size(431, 22)
        Me.toolStripAutomaticallyCheckForUpdates.Text = "Automatically &Check for Updates at Program Launch"
        '
        'AskBeforeUpgradingUpdatingToolStripMenuItem
        '
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.CheckOnClick = True
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.Name = "AskBeforeUpgradingUpdatingToolStripMenuItem"
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.AskBeforeUpgradingUpdatingToolStripMenuItem.Text = "Ask Before Upgrading/Updating"
        '
        'ConfigureAutomaticUpdatesToolStripMenuItem
        '
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripCheckEveryWeek, Me.toolStripCheckEveryTwoWeeks, Me.toolStripCheckCustom})
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.chronometer
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Name = "ConfigureAutomaticUpdatesToolStripMenuItem"
        Me.ConfigureAutomaticUpdatesToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
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
        Me.UpdateChannelToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.toolStripStableChannel, Me.toolStripBetaChannel})
        Me.UpdateChannelToolStripMenuItem.Image = Global.Restore_Point_Creator.My.Resources.Resources.arrow_down
        Me.UpdateChannelToolStripMenuItem.Name = "UpdateChannelToolStripMenuItem"
        Me.UpdateChannelToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.UpdateChannelToolStripMenuItem.Text = "Program Updates Channel"
        '
        'toolStripStableChannel
        '
        Me.toolStripStableChannel.CheckOnClick = True
        Me.toolStripStableChannel.Name = "toolStripStableChannel"
        Me.toolStripStableChannel.Size = New System.Drawing.Size(138, 22)
        Me.toolStripStableChannel.Text = "Stable"
        '
        'toolStripBetaChannel
        '
        Me.toolStripBetaChannel.CheckOnClick = True
        Me.toolStripBetaChannel.Name = "toolStripBetaChannel"
        Me.toolStripBetaChannel.Size = New System.Drawing.Size(138, 22)
        Me.toolStripBetaChannel.Text = "Public Betas"
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
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(428, 6)
        '
        'EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem
        '
        Me.EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem.CheckOnClick = True
        Me.EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem.Name = "EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuIt" &
    "em"
        Me.EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem.Text = "Interface too small? Enable this option"
        '
        'CheckSystemDrivesForFullShadowStorageToolStripMenuItem
        '
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.CheckOnClick = True
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Name = "CheckSystemDrivesForFullShadowStorageToolStripMenuItem"
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.CheckSystemDrivesForFullShadowStorageToolStripMenuItem.Text = "Check Windows Drive for Full Shadow Storage at Program Launch"
        '
        'EnableSystemEventLoggingToolStripMenuItem
        '
        Me.EnableSystemEventLoggingToolStripMenuItem.CheckOnClick = True
        Me.EnableSystemEventLoggingToolStripMenuItem.Name = "EnableSystemEventLoggingToolStripMenuItem"
        Me.EnableSystemEventLoggingToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.EnableSystemEventLoggingToolStripMenuItem.Text = "Enable System Event Logging"
        '
        'toolStripConfirmDeletions
        '
        Me.toolStripConfirmDeletions.CheckOnClick = True
        Me.toolStripConfirmDeletions.Name = "toolStripConfirmDeletions"
        Me.toolStripConfirmDeletions.Size = New System.Drawing.Size(431, 22)
        Me.toolStripConfirmDeletions.Text = "Confirm Restore Point &Deletions"
        '
        'toolStripLogRestorePointDeletions
        '
        Me.toolStripLogRestorePointDeletions.CheckOnClick = True
        Me.toolStripLogRestorePointDeletions.Name = "toolStripLogRestorePointDeletions"
        Me.toolStripLogRestorePointDeletions.Size = New System.Drawing.Size(431, 22)
        Me.toolStripLogRestorePointDeletions.Text = "&Log System Restore Point Deletions"
        '
        'toolStripCloseAfterRestorePointIsCreated
        '
        Me.toolStripCloseAfterRestorePointIsCreated.CheckOnClick = True
        Me.toolStripCloseAfterRestorePointIsCreated.Name = "toolStripCloseAfterRestorePointIsCreated"
        Me.toolStripCloseAfterRestorePointIsCreated.Size = New System.Drawing.Size(431, 22)
        Me.toolStripCloseAfterRestorePointIsCreated.Text = "Close Program After Restore Point is Created"
        '
        'KeepXAmountOfRestorePointsToolStripMenuItem
        '
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.CheckOnClick = True
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Name = "KeepXAmountOfRestorePointsToolStripMenuItem"
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.KeepXAmountOfRestorePointsToolStripMenuItem.Text = "Keep X Amount of Restore Points"
        '
        'DefaultCustomRestorePointNameToolStripMenuItem
        '
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Name = "DefaultCustomRestorePointNameToolStripMenuItem"
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.DefaultCustomRestorePointNameToolStripMenuItem.Text = "Set Default Custom Restore Point Name"
        '
        'AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem
        '
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Name = "AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem"
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem.Text = "Allow for deletion of all System Restore Points"
        '
        'BypassNoUACLauncherToolStripMenuItem
        '
        Me.BypassNoUACLauncherToolStripMenuItem.CheckOnClick = True
        Me.BypassNoUACLauncherToolStripMenuItem.Name = "BypassNoUACLauncherToolStripMenuItem"
        Me.BypassNoUACLauncherToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.BypassNoUACLauncherToolStripMenuItem.Text = "Bypass No UAC Launcher"
        '
        'LogProgramLoadsAndExitsToEventLogToolStripMenuItem
        '
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.CheckOnClick = True
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Name = "LogProgramLoadsAndExitsToEventLogToolStripMenuItem"
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.LogProgramLoadsAndExitsToEventLogToolStripMenuItem.Text = "Log Program Loads and Exits to Event Log"
        '
        'SwitchToDebugBuildToolStripMenuItem
        '
        Me.SwitchToDebugBuildToolStripMenuItem.Name = "SwitchToDebugBuildToolStripMenuItem"
        Me.SwitchToDebugBuildToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SwitchToDebugBuildToolStripMenuItem.Text = "Switch to Debug Build (Helpful in debugging program crashes)"
        '
        'SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem
        '
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Name = "SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem"
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem.Text = "Set Windows Power Plan for Wake Timers back to default (Disabled)"
        '
        'UseSSLToolStripMenuItem
        '
        Me.UseSSLToolStripMenuItem.CheckOnClick = True
        Me.UseSSLToolStripMenuItem.Name = "UseSSLToolStripMenuItem"
        Me.UseSSLToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.UseSSLToolStripMenuItem.Text = "Use SSL"
        '
        'toolStripAbout
        '
        Me.toolStripAbout.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.AboutThisProgramToolStripMenuItem, Me.ProductWebSiteToolStripMenuItem, Me.ContactTheDeveloperToolStripMenuItem, Me.ViewOfficialChangeLogToolStripMenuItem1})
        Me.toolStripAbout.Name = "toolStripAbout"
        Me.toolStripAbout.Size = New System.Drawing.Size(52, 20)
        Me.toolStripAbout.Text = "&About"
        '
        'AboutThisProgramToolStripMenuItem
        '
        Me.AboutThisProgramToolStripMenuItem.Image = CType(resources.GetObject("AboutThisProgramToolStripMenuItem.Image"), System.Drawing.Image)
        Me.AboutThisProgramToolStripMenuItem.Name = "AboutThisProgramToolStripMenuItem"
        Me.AboutThisProgramToolStripMenuItem.Size = New System.Drawing.Size(256, 22)
        Me.AboutThisProgramToolStripMenuItem.Text = "About this Program"
        '
        'ProductWebSiteToolStripMenuItem
        '
        Me.ProductWebSiteToolStripMenuItem.Image = CType(resources.GetObject("ProductWebSiteToolStripMenuItem.Image"), System.Drawing.Image)
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
        Me.ViewOfficialChangeLogToolStripMenuItem1.Image = Global.Restore_Point_Creator.My.Resources.Resources.fileIcon
        Me.ViewOfficialChangeLogToolStripMenuItem1.Name = "ViewOfficialChangeLogToolStripMenuItem1"
        Me.ViewOfficialChangeLogToolStripMenuItem1.Size = New System.Drawing.Size(256, 22)
        Me.ViewOfficialChangeLogToolStripMenuItem1.Text = "View Official Program Change Log"
        '
        'toolStripDonate
        '
        Me.toolStripDonate.Image = CType(resources.GetObject("toolStripDonate.Image"), System.Drawing.Image)
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
        Me.Label5.Size = New System.Drawing.Size(710, 1)
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
        Me.Label2.Size = New System.Drawing.Size(710, 1)
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
        'deleteProgressBar
        '
        Me.deleteProgressBar.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.deleteProgressBar.Location = New System.Drawing.Point(148, 100)
        Me.deleteProgressBar.Maximum = 100
        Me.deleteProgressBar.Minimum = 0
        Me.deleteProgressBar.Name = "deleteProgressBar"
        Me.deleteProgressBar.ProgressBarColor = System.Drawing.Color.Blue
        Me.deleteProgressBar.Size = New System.Drawing.Size(547, 13)
        Me.deleteProgressBar.TabIndex = 20
        Me.deleteProgressBar.Value = 0
        Me.deleteProgressBar.Visible = False
        '
        'importBackupDialog
        '
        Me.importBackupDialog.FileName = "OpenFileDialog1"
        '
        'AdditionalOptionsAndSettingsToolStripMenuItem
        '
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Name = "AdditionalOptionsAndSettingsToolStripMenuItem"
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Size = New System.Drawing.Size(431, 22)
        Me.AdditionalOptionsAndSettingsToolStripMenuItem.Text = "Additional Options and Settings"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(707, 516)
        Me.Controls.Add(Me.btnCreateRestorePointNameWithDefaultName)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Controls.Add(Me.btnCreateSystemCheckpoint)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.deleteProgressBar)
        Me.Controls.Add(Me.lblCurrentRestorePointsLabel)
        Me.Controls.Add(Me.systemRestorePointsList)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.btnCreate)
        Me.Controls.Add(Me.txtRestorePointDescription)
        Me.KeyPreview = True
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(715, 543)
        Me.Name = "Form1"
        Me.Text = "Restore Point Creator"
        Me.restorePointListContextMenu.ResumeLayout(False)
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
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
    Friend WithEvents TableLayoutPanel2 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents btnDeleteRestorePoint As System.Windows.Forms.Button
    Friend WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents toolUtilities As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripViewDiskSpaceUsage As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripManageSystemRestoreStorageSize As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripScheduleRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents OptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripMyComputer As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripLogRestorePointDeletions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripAutomaticallyCheckForUpdates As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripConfirmDeletions As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents toolStripCheckForUpdates As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripAbout As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ProductWebSiteToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents AboutThisProgramToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDonate As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripCloseAfterRestorePointIsCreated As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FfgdToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents FgdfgdToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDeleteRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDeleteOldRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripDeleteAllRestorePoints As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents UpdateChannelToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripStableChannel As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripBetaChannel As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents KeepXAmountOfRestorePointsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ConfigureAutomaticUpdatesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripCheckEveryWeek As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripCheckEveryTwoWeeks As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripCheckCustom As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents HelpToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SetBarColorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents ColorDialog As System.Windows.Forms.ColorDialog
    Friend WithEvents DefaultCustomRestorePointNameToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents CheckSystemDrivesForFullShadowStorageToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NotificationOptionsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowMessageBoxAfterSuccessfulCreationOfRestorePointToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ShowMessageBoxAfterSuccessfulDeletionOfRestorePointsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents toolStripClear As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents NotificationTypeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents MessageBoxToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents BalloonToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripSeparator
    Friend WithEvents NotifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents AllowForDeletionOfAllSystemRestorePointsToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnCreateRestorePointNameWithDefaultName As System.Windows.Forms.Button
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents BypassNoUACLauncherToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ViewOfficialChangeLogToolStripMenuItem1 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents stripRestoreSafeMode As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents btnRestoreToRestorePointSafeMode As System.Windows.Forms.Button
    Friend WithEvents CreateRestorePointAtUserLogonToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents EnableSystemEventLoggingToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents deleteProgressBar As Tom.SmoothProgressBar
    Friend WithEvents ProgramEventLogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LogProgramLoadsAndExitsToEventLogToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContactTheDeveloperToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents SwitchToDebugBuildToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EnableThisIfTheUserInterfaceIsTooSmallOnSystemsRunningWindows8Or10ToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CheckWindowsPowerPlanSettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigurationBackupRestoreToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents BackupToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents RestoreToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents importBackupDialog As OpenFileDialog
    Friend WithEvents exportBackupDialog As SaveFileDialog
    Friend WithEvents MountVolumeShadowCopyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SetWindowsActivePowerPlanSettingsForWakeTimersBackToDefaultToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents UseSSLToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ConfigureProxyToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AskBeforeUpgradingUpdatingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AdditionalOptionsAndSettingsToolStripMenuItem As ToolStripMenuItem
End Class
