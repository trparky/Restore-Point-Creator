<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmTaskScheduler
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.timePicker = New System.Windows.Forms.DateTimePicker()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.grpDaysOfTheWeek = New System.Windows.Forms.GroupBox()
        Me.chkSaturday = New System.Windows.Forms.CheckBox()
        Me.chkFriday = New System.Windows.Forms.CheckBox()
        Me.chkThursday = New System.Windows.Forms.CheckBox()
        Me.chkWednesday = New System.Windows.Forms.CheckBox()
        Me.chkTuesday = New System.Windows.Forms.CheckBox()
        Me.chkMonday = New System.Windows.Forms.CheckBox()
        Me.chkSunday = New System.Windows.Forms.CheckBox()
        Me.btnSaveTask = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.radEvery = New System.Windows.Forms.RadioButton()
        Me.radWeekly = New System.Windows.Forms.RadioButton()
        Me.radDaily = New System.Windows.Forms.RadioButton()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnDeleteTask = New System.Windows.Forms.Button()
        Me.btnSet = New System.Windows.Forms.Button()
        Me.txtDays = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.chkDeleteOldRestorePoints = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.lblCreateTaskLabel = New System.Windows.Forms.Label()
        Me.txtDaysDelete = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.btnDeleteTaskDelete = New System.Windows.Forms.Button()
        Me.btnSaveTaskDelete = New System.Windows.Forms.Button()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.radWeeklyDelete = New System.Windows.Forms.RadioButton()
        Me.radDailyDelete = New System.Windows.Forms.RadioButton()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.grpDaysOfTheWeekDelete = New System.Windows.Forms.GroupBox()
        Me.chkSaturdayDelete = New System.Windows.Forms.CheckBox()
        Me.chkFridayDelete = New System.Windows.Forms.CheckBox()
        Me.chkThursdayDelete = New System.Windows.Forms.CheckBox()
        Me.chkWednesdayDelete = New System.Windows.Forms.CheckBox()
        Me.chkTuesdayDelete = New System.Windows.Forms.CheckBox()
        Me.chkMondayDelete = New System.Windows.Forms.CheckBox()
        Me.chkSundayDelete = New System.Windows.Forms.CheckBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.timePickerDelete = New System.Windows.Forms.DateTimePicker()
        Me.lblEvery = New System.Windows.Forms.Label()
        Me.txtEveryDay = New System.Windows.Forms.TextBox()
        Me.lblDays = New System.Windows.Forms.Label()
        Me.deleteOldRestorePointsTaskSettings = New System.Windows.Forms.GroupBox()
        Me.lblRunTimesDelete = New System.Windows.Forms.Label()
        Me.chkWakeDelete = New System.Windows.Forms.CheckBox()
        Me.chkRunMissedTaskDelete = New System.Windows.Forms.CheckBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.chkRunMissedTask = New System.Windows.Forms.CheckBox()
        Me.btnSetCustomName = New System.Windows.Forms.Button()
        Me.chkWake = New System.Windows.Forms.CheckBox()
        Me.lblRunTimes = New System.Windows.Forms.Label()
        Me.grpDaysOfTheWeek.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.grpDaysOfTheWeekDelete.SuspendLayout()
        Me.deleteOldRestorePointsTaskSettings.SuspendLayout()
        Me.SuspendLayout()
        '
        'timePicker
        '
        Me.timePicker.CustomFormat = """MM/dd/yyyy hh:mm:ss tt"""
        Me.timePicker.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.timePicker.Location = New System.Drawing.Point(236, 39)
        Me.timePicker.Name = "timePicker"
        Me.timePicker.Size = New System.Drawing.Size(98, 20)
        Me.timePicker.TabIndex = 4
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(155, 41)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "At What Time:"
        '
        'grpDaysOfTheWeek
        '
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkSaturday)
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkFriday)
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkThursday)
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkWednesday)
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkTuesday)
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkMonday)
        Me.grpDaysOfTheWeek.Controls.Add(Me.chkSunday)
        Me.grpDaysOfTheWeek.Enabled = False
        Me.grpDaysOfTheWeek.Location = New System.Drawing.Point(152, 65)
        Me.grpDaysOfTheWeek.Name = "grpDaysOfTheWeek"
        Me.grpDaysOfTheWeek.Size = New System.Drawing.Size(328, 66)
        Me.grpDaysOfTheWeek.TabIndex = 13
        Me.grpDaysOfTheWeek.TabStop = False
        Me.grpDaysOfTheWeek.Text = "Days of the Week"
        '
        'chkSaturday
        '
        Me.chkSaturday.AutoSize = True
        Me.chkSaturday.Location = New System.Drawing.Point(162, 42)
        Me.chkSaturday.Name = "chkSaturday"
        Me.chkSaturday.Size = New System.Drawing.Size(68, 17)
        Me.chkSaturday.TabIndex = 19
        Me.chkSaturday.Text = "Saturday"
        Me.chkSaturday.UseVisualStyleBackColor = True
        '
        'chkFriday
        '
        Me.chkFriday.AutoSize = True
        Me.chkFriday.Location = New System.Drawing.Point(84, 42)
        Me.chkFriday.Name = "chkFriday"
        Me.chkFriday.Size = New System.Drawing.Size(54, 17)
        Me.chkFriday.TabIndex = 18
        Me.chkFriday.Text = "Friday"
        Me.chkFriday.UseVisualStyleBackColor = True
        '
        'chkThursday
        '
        Me.chkThursday.AutoSize = True
        Me.chkThursday.Location = New System.Drawing.Point(6, 42)
        Me.chkThursday.Name = "chkThursday"
        Me.chkThursday.Size = New System.Drawing.Size(70, 17)
        Me.chkThursday.TabIndex = 17
        Me.chkThursday.Text = "Thursday"
        Me.chkThursday.UseVisualStyleBackColor = True
        '
        'chkWednesday
        '
        Me.chkWednesday.AutoSize = True
        Me.chkWednesday.Location = New System.Drawing.Point(240, 19)
        Me.chkWednesday.Name = "chkWednesday"
        Me.chkWednesday.Size = New System.Drawing.Size(83, 17)
        Me.chkWednesday.TabIndex = 16
        Me.chkWednesday.Text = "Wednesday"
        Me.chkWednesday.UseVisualStyleBackColor = True
        '
        'chkTuesday
        '
        Me.chkTuesday.AutoSize = True
        Me.chkTuesday.Location = New System.Drawing.Point(162, 19)
        Me.chkTuesday.Name = "chkTuesday"
        Me.chkTuesday.Size = New System.Drawing.Size(67, 17)
        Me.chkTuesday.TabIndex = 15
        Me.chkTuesday.Text = "Tuesday"
        Me.chkTuesday.UseVisualStyleBackColor = True
        '
        'chkMonday
        '
        Me.chkMonday.AutoSize = True
        Me.chkMonday.Location = New System.Drawing.Point(84, 19)
        Me.chkMonday.Name = "chkMonday"
        Me.chkMonday.Size = New System.Drawing.Size(64, 17)
        Me.chkMonday.TabIndex = 14
        Me.chkMonday.Text = "Monday"
        Me.chkMonday.UseVisualStyleBackColor = True
        '
        'chkSunday
        '
        Me.chkSunday.AutoSize = True
        Me.chkSunday.Location = New System.Drawing.Point(6, 19)
        Me.chkSunday.Name = "chkSunday"
        Me.chkSunday.Size = New System.Drawing.Size(62, 17)
        Me.chkSunday.TabIndex = 13
        Me.chkSunday.Text = "Sunday"
        Me.chkSunday.UseVisualStyleBackColor = True
        '
        'btnSaveTask
        '
        Me.btnSaveTask.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnSaveTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveTask.Location = New System.Drawing.Point(15, 272)
        Me.btnSaveTask.Name = "btnSaveTask"
        Me.btnSaveTask.Size = New System.Drawing.Size(230, 23)
        Me.btnSaveTask.TabIndex = 14
        Me.btnSaveTask.Text = "Save Task"
        Me.btnSaveTask.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.radEvery)
        Me.Panel1.Controls.Add(Me.radWeekly)
        Me.Panel1.Controls.Add(Me.radDaily)
        Me.Panel1.Location = New System.Drawing.Point(63, 35)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(66, 66)
        Me.Panel1.TabIndex = 15
        '
        'radEvery
        '
        Me.radEvery.AutoSize = True
        Me.radEvery.Location = New System.Drawing.Point(3, 48)
        Me.radEvery.Name = "radEvery"
        Me.radEvery.Size = New System.Drawing.Size(52, 17)
        Me.radEvery.TabIndex = 6
        Me.radEvery.TabStop = True
        Me.radEvery.Text = "Every"
        Me.radEvery.UseVisualStyleBackColor = True
        '
        'radWeekly
        '
        Me.radWeekly.AutoSize = True
        Me.radWeekly.Location = New System.Drawing.Point(3, 26)
        Me.radWeekly.Name = "radWeekly"
        Me.radWeekly.Size = New System.Drawing.Size(61, 17)
        Me.radWeekly.TabIndex = 5
        Me.radWeekly.TabStop = True
        Me.radWeekly.Text = "Weekly"
        Me.radWeekly.UseVisualStyleBackColor = True
        '
        'radDaily
        '
        Me.radDaily.AutoSize = True
        Me.radDaily.Location = New System.Drawing.Point(3, 3)
        Me.radDaily.Name = "radDaily"
        Me.radDaily.Size = New System.Drawing.Size(48, 17)
        Me.radDaily.TabIndex = 4
        Me.radDaily.TabStop = True
        Me.radDaily.Text = "Daily"
        Me.radDaily.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(12, 39)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(45, 13)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "Settings"
        '
        'btnDeleteTask
        '
        Me.btnDeleteTask.Enabled = False
        Me.btnDeleteTask.Image = Global.Restore_Point_Creator.My.Resources.Resources.delete
        Me.btnDeleteTask.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteTask.Location = New System.Drawing.Point(249, 272)
        Me.btnDeleteTask.Name = "btnDeleteTask"
        Me.btnDeleteTask.Size = New System.Drawing.Size(230, 23)
        Me.btnDeleteTask.TabIndex = 16
        Me.btnDeleteTask.Text = "Delete Task"
        Me.btnDeleteTask.UseVisualStyleBackColor = True
        '
        'btnSet
        '
        Me.btnSet.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnSet.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSet.Location = New System.Drawing.Point(229, 211)
        Me.btnSet.Name = "btnSet"
        Me.btnSet.Size = New System.Drawing.Size(105, 23)
        Me.btnSet.TabIndex = 20
        Me.btnSet.Text = "Save Max Age"
        Me.btnSet.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.btnSet.UseVisualStyleBackColor = True
        '
        'txtDays
        '
        Me.txtDays.Location = New System.Drawing.Point(188, 213)
        Me.txtDays.Name = "txtDays"
        Me.txtDays.Size = New System.Drawing.Size(35, 20)
        Me.txtDays.TabIndex = 19
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(11, 216)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(174, 13)
        Me.Label3.TabIndex = 18
        Me.Label3.Text = "Max Age of Restore Points in Days:"
        '
        'chkDeleteOldRestorePoints
        '
        Me.chkDeleteOldRestorePoints.AutoSize = True
        Me.chkDeleteOldRestorePoints.Location = New System.Drawing.Point(15, 150)
        Me.chkDeleteOldRestorePoints.Name = "chkDeleteOldRestorePoints"
        Me.chkDeleteOldRestorePoints.Size = New System.Drawing.Size(380, 17)
        Me.chkDeleteOldRestorePoints.TabIndex = 17
        Me.chkDeleteOldRestorePoints.Text = "Delete Old System Restore Points After Scheduled Restore Point is created"
        Me.chkDeleteOldRestorePoints.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(12, 134)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(167, 13)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "Additional Task Preferences"
        '
        'lblCreateTaskLabel
        '
        Me.lblCreateTaskLabel.AutoSize = True
        Me.lblCreateTaskLabel.Location = New System.Drawing.Point(12, 9)
        Me.lblCreateTaskLabel.Name = "lblCreateTaskLabel"
        Me.lblCreateTaskLabel.Size = New System.Drawing.Size(191, 13)
        Me.lblCreateTaskLabel.TabIndex = 29
        Me.lblCreateTaskLabel.Text = "Create Scheduled Restore Points Task"
        '
        'txtDaysDelete
        '
        Me.txtDaysDelete.Location = New System.Drawing.Point(185, 150)
        Me.txtDaysDelete.Name = "txtDaysDelete"
        Me.txtDaysDelete.Size = New System.Drawing.Size(35, 20)
        Me.txtDaysDelete.TabIndex = 40
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(5, 153)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(174, 13)
        Me.Label9.TabIndex = 39
        Me.Label9.Text = "Max Age of Restore Points in Days:"
        '
        'btnDeleteTaskDelete
        '
        Me.btnDeleteTaskDelete.Enabled = False
        Me.btnDeleteTaskDelete.Image = Global.Restore_Point_Creator.My.Resources.Resources.delete
        Me.btnDeleteTaskDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnDeleteTaskDelete.Location = New System.Drawing.Point(239, 238)
        Me.btnDeleteTaskDelete.Name = "btnDeleteTaskDelete"
        Me.btnDeleteTaskDelete.Size = New System.Drawing.Size(230, 23)
        Me.btnDeleteTaskDelete.TabIndex = 37
        Me.btnDeleteTaskDelete.Text = "Delete Task"
        Me.btnDeleteTaskDelete.UseVisualStyleBackColor = True
        '
        'btnSaveTaskDelete
        '
        Me.btnSaveTaskDelete.Image = Global.Restore_Point_Creator.My.Resources.Resources.save
        Me.btnSaveTaskDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btnSaveTaskDelete.Location = New System.Drawing.Point(3, 238)
        Me.btnSaveTaskDelete.Name = "btnSaveTaskDelete"
        Me.btnSaveTaskDelete.Size = New System.Drawing.Size(230, 23)
        Me.btnSaveTaskDelete.TabIndex = 36
        Me.btnSaveTaskDelete.Text = "Save Task"
        Me.btnSaveTaskDelete.UseVisualStyleBackColor = True
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.radWeeklyDelete)
        Me.Panel2.Controls.Add(Me.radDailyDelete)
        Me.Panel2.Location = New System.Drawing.Point(56, 43)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(66, 50)
        Me.Panel2.TabIndex = 35
        '
        'radWeeklyDelete
        '
        Me.radWeeklyDelete.AutoSize = True
        Me.radWeeklyDelete.Location = New System.Drawing.Point(3, 26)
        Me.radWeeklyDelete.Name = "radWeeklyDelete"
        Me.radWeeklyDelete.Size = New System.Drawing.Size(61, 17)
        Me.radWeeklyDelete.TabIndex = 5
        Me.radWeeklyDelete.TabStop = True
        Me.radWeeklyDelete.Text = "Weekly"
        Me.radWeeklyDelete.UseVisualStyleBackColor = True
        '
        'radDailyDelete
        '
        Me.radDailyDelete.AutoSize = True
        Me.radDailyDelete.Location = New System.Drawing.Point(3, 3)
        Me.radDailyDelete.Name = "radDailyDelete"
        Me.radDailyDelete.Size = New System.Drawing.Size(48, 17)
        Me.radDailyDelete.TabIndex = 4
        Me.radDailyDelete.TabStop = True
        Me.radDailyDelete.Text = "Daily"
        Me.radDailyDelete.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(5, 47)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(45, 13)
        Me.Label5.TabIndex = 31
        Me.Label5.Text = "Settings"
        '
        'grpDaysOfTheWeekDelete
        '
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkSaturdayDelete)
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkFridayDelete)
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkThursdayDelete)
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkWednesdayDelete)
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkTuesdayDelete)
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkMondayDelete)
        Me.grpDaysOfTheWeekDelete.Controls.Add(Me.chkSundayDelete)
        Me.grpDaysOfTheWeekDelete.Enabled = False
        Me.grpDaysOfTheWeekDelete.Location = New System.Drawing.Point(141, 73)
        Me.grpDaysOfTheWeekDelete.Name = "grpDaysOfTheWeekDelete"
        Me.grpDaysOfTheWeekDelete.Size = New System.Drawing.Size(328, 66)
        Me.grpDaysOfTheWeekDelete.TabIndex = 34
        Me.grpDaysOfTheWeekDelete.TabStop = False
        Me.grpDaysOfTheWeekDelete.Text = "Days of the Week"
        '
        'chkSaturdayDelete
        '
        Me.chkSaturdayDelete.AutoSize = True
        Me.chkSaturdayDelete.Location = New System.Drawing.Point(162, 42)
        Me.chkSaturdayDelete.Name = "chkSaturdayDelete"
        Me.chkSaturdayDelete.Size = New System.Drawing.Size(68, 17)
        Me.chkSaturdayDelete.TabIndex = 19
        Me.chkSaturdayDelete.Text = "Saturday"
        Me.chkSaturdayDelete.UseVisualStyleBackColor = True
        '
        'chkFridayDelete
        '
        Me.chkFridayDelete.AutoSize = True
        Me.chkFridayDelete.Location = New System.Drawing.Point(84, 42)
        Me.chkFridayDelete.Name = "chkFridayDelete"
        Me.chkFridayDelete.Size = New System.Drawing.Size(54, 17)
        Me.chkFridayDelete.TabIndex = 18
        Me.chkFridayDelete.Text = "Friday"
        Me.chkFridayDelete.UseVisualStyleBackColor = True
        '
        'chkThursdayDelete
        '
        Me.chkThursdayDelete.AutoSize = True
        Me.chkThursdayDelete.Location = New System.Drawing.Point(6, 42)
        Me.chkThursdayDelete.Name = "chkThursdayDelete"
        Me.chkThursdayDelete.Size = New System.Drawing.Size(70, 17)
        Me.chkThursdayDelete.TabIndex = 17
        Me.chkThursdayDelete.Text = "Thursday"
        Me.chkThursdayDelete.UseVisualStyleBackColor = True
        '
        'chkWednesdayDelete
        '
        Me.chkWednesdayDelete.AutoSize = True
        Me.chkWednesdayDelete.Location = New System.Drawing.Point(240, 19)
        Me.chkWednesdayDelete.Name = "chkWednesdayDelete"
        Me.chkWednesdayDelete.Size = New System.Drawing.Size(83, 17)
        Me.chkWednesdayDelete.TabIndex = 16
        Me.chkWednesdayDelete.Text = "Wednesday"
        Me.chkWednesdayDelete.UseVisualStyleBackColor = True
        '
        'chkTuesdayDelete
        '
        Me.chkTuesdayDelete.AutoSize = True
        Me.chkTuesdayDelete.Location = New System.Drawing.Point(162, 19)
        Me.chkTuesdayDelete.Name = "chkTuesdayDelete"
        Me.chkTuesdayDelete.Size = New System.Drawing.Size(67, 17)
        Me.chkTuesdayDelete.TabIndex = 15
        Me.chkTuesdayDelete.Text = "Tuesday"
        Me.chkTuesdayDelete.UseVisualStyleBackColor = True
        '
        'chkMondayDelete
        '
        Me.chkMondayDelete.AutoSize = True
        Me.chkMondayDelete.Location = New System.Drawing.Point(84, 19)
        Me.chkMondayDelete.Name = "chkMondayDelete"
        Me.chkMondayDelete.Size = New System.Drawing.Size(64, 17)
        Me.chkMondayDelete.TabIndex = 14
        Me.chkMondayDelete.Text = "Monday"
        Me.chkMondayDelete.UseVisualStyleBackColor = True
        '
        'chkSundayDelete
        '
        Me.chkSundayDelete.AutoSize = True
        Me.chkSundayDelete.Location = New System.Drawing.Point(6, 19)
        Me.chkSundayDelete.Name = "chkSundayDelete"
        Me.chkSundayDelete.Size = New System.Drawing.Size(62, 17)
        Me.chkSundayDelete.TabIndex = 13
        Me.chkSundayDelete.Text = "Sunday"
        Me.chkSundayDelete.UseVisualStyleBackColor = True
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(148, 49)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(75, 13)
        Me.Label6.TabIndex = 33
        Me.Label6.Text = "At What Time:"
        '
        'timePickerDelete
        '
        Me.timePickerDelete.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.timePickerDelete.Location = New System.Drawing.Point(229, 47)
        Me.timePickerDelete.Name = "timePickerDelete"
        Me.timePickerDelete.Size = New System.Drawing.Size(98, 20)
        Me.timePickerDelete.TabIndex = 32
        '
        'lblEvery
        '
        Me.lblEvery.AutoSize = True
        Me.lblEvery.Location = New System.Drawing.Point(342, 42)
        Me.lblEvery.Name = "lblEvery"
        Me.lblEvery.Size = New System.Drawing.Size(34, 13)
        Me.lblEvery.TabIndex = 32
        Me.lblEvery.Text = "Every"
        '
        'txtEveryDay
        '
        Me.txtEveryDay.Location = New System.Drawing.Point(374, 39)
        Me.txtEveryDay.Name = "txtEveryDay"
        Me.txtEveryDay.Size = New System.Drawing.Size(25, 20)
        Me.txtEveryDay.TabIndex = 33
        '
        'lblDays
        '
        Me.lblDays.AutoSize = True
        Me.lblDays.Location = New System.Drawing.Point(399, 42)
        Me.lblDays.Name = "lblDays"
        Me.lblDays.Size = New System.Drawing.Size(31, 13)
        Me.lblDays.TabIndex = 34
        Me.lblDays.Text = "Days"
        '
        'deleteOldRestorePointsTaskSettings
        '
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.lblRunTimesDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.chkWakeDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.chkRunMissedTaskDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.Label8)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.txtDaysDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.Label5)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.Label9)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.timePickerDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.Label6)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.btnDeleteTaskDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.grpDaysOfTheWeekDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.btnSaveTaskDelete)
        Me.deleteOldRestorePointsTaskSettings.Controls.Add(Me.Panel2)
        Me.deleteOldRestorePointsTaskSettings.Location = New System.Drawing.Point(12, 301)
        Me.deleteOldRestorePointsTaskSettings.Name = "deleteOldRestorePointsTaskSettings"
        Me.deleteOldRestorePointsTaskSettings.Size = New System.Drawing.Size(475, 268)
        Me.deleteOldRestorePointsTaskSettings.TabIndex = 35
        Me.deleteOldRestorePointsTaskSettings.TabStop = False
        Me.deleteOldRestorePointsTaskSettings.Text = "Delete Old Restore Points Task Settings"
        '
        'lblRunTimesDelete
        '
        Me.lblRunTimesDelete.AutoSize = True
        Me.lblRunTimesDelete.Location = New System.Drawing.Point(207, 16)
        Me.lblRunTimesDelete.Name = "lblRunTimesDelete"
        Me.lblRunTimesDelete.Size = New System.Drawing.Size(81, 26)
        Me.lblRunTimesDelete.TabIndex = 44
        Me.lblRunTimesDelete.Text = "Last Run Time:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Next Run Time:"
        '
        'chkWakeDelete
        '
        Me.chkWakeDelete.AutoSize = True
        Me.chkWakeDelete.Location = New System.Drawing.Point(6, 215)
        Me.chkWakeDelete.Name = "chkWakeDelete"
        Me.chkWakeDelete.Size = New System.Drawing.Size(337, 17)
        Me.chkWakeDelete.TabIndex = 43
        Me.chkWakeDelete.Text = "Wake computer if the system is sleeping to run maintenance tasks"
        Me.chkWakeDelete.UseVisualStyleBackColor = True
        '
        'chkRunMissedTaskDelete
        '
        Me.chkRunMissedTaskDelete.AutoSize = True
        Me.chkRunMissedTaskDelete.Location = New System.Drawing.Point(6, 196)
        Me.chkRunMissedTaskDelete.Name = "chkRunMissedTaskDelete"
        Me.chkRunMissedTaskDelete.Size = New System.Drawing.Size(344, 17)
        Me.chkRunMissedTaskDelete.TabIndex = 42
        Me.chkRunMissedTaskDelete.Text = "If task is missed, run when system is next available (Recommended)"
        Me.chkRunMissedTaskDelete.UseVisualStyleBackColor = True
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(6, 180)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(167, 13)
        Me.Label8.TabIndex = 41
        Me.Label8.Text = "Additional Task Preferences"
        '
        'chkRunMissedTask
        '
        Me.chkRunMissedTask.AutoSize = True
        Me.chkRunMissedTask.Location = New System.Drawing.Point(15, 169)
        Me.chkRunMissedTask.Name = "chkRunMissedTask"
        Me.chkRunMissedTask.Size = New System.Drawing.Size(344, 17)
        Me.chkRunMissedTask.TabIndex = 36
        Me.chkRunMissedTask.Text = "If task is missed, run when system is next available (Recommended)"
        Me.chkRunMissedTask.UseVisualStyleBackColor = True
        '
        'btnSetCustomName
        '
        Me.btnSetCustomName.Location = New System.Drawing.Point(16, 243)
        Me.btnSetCustomName.Name = "btnSetCustomName"
        Me.btnSetCustomName.Size = New System.Drawing.Size(463, 23)
        Me.btnSetCustomName.TabIndex = 37
        Me.btnSetCustomName.Text = "Set Custom Restore Point Name for Scheduled Restore Points"
        Me.btnSetCustomName.UseVisualStyleBackColor = True
        '
        'chkWake
        '
        Me.chkWake.AutoSize = True
        Me.chkWake.Location = New System.Drawing.Point(15, 188)
        Me.chkWake.Name = "chkWake"
        Me.chkWake.Size = New System.Drawing.Size(445, 17)
        Me.chkWake.TabIndex = 38
        Me.chkWake.Text = "Wake computer if the system is sleeping to create System Restore Point (Recommend" &
    "ed)"
        Me.chkWake.UseVisualStyleBackColor = True
        '
        'lblRunTimes
        '
        Me.lblRunTimes.AutoSize = True
        Me.lblRunTimes.Location = New System.Drawing.Point(238, 9)
        Me.lblRunTimes.Name = "lblRunTimes"
        Me.lblRunTimes.Size = New System.Drawing.Size(81, 26)
        Me.lblRunTimes.TabIndex = 39
        Me.lblRunTimes.Text = "Last Run Time:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Next Run Time:"
        '
        'frmTaskScheduler
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(96.0!, 96.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi
        Me.ClientSize = New System.Drawing.Size(492, 575)
        Me.Controls.Add(Me.lblRunTimes)
        Me.Controls.Add(Me.chkWake)
        Me.Controls.Add(Me.btnSetCustomName)
        Me.Controls.Add(Me.chkRunMissedTask)
        Me.Controls.Add(Me.deleteOldRestorePointsTaskSettings)
        Me.Controls.Add(Me.lblDays)
        Me.Controls.Add(Me.txtEveryDay)
        Me.Controls.Add(Me.lblEvery)
        Me.Controls.Add(Me.lblCreateTaskLabel)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnSet)
        Me.Controls.Add(Me.txtDays)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.chkDeleteOldRestorePoints)
        Me.Controls.Add(Me.btnDeleteTask)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.btnSaveTask)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.grpDaysOfTheWeek)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.timePicker)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = Global.Restore_Point_Creator.My.Resources.Resources.RestorePoint_noBackground_2
        Me.MaximizeBox = False
        Me.Name = "frmTaskScheduler"
        Me.Text = "Task Scheduler"
        Me.grpDaysOfTheWeek.ResumeLayout(False)
        Me.grpDaysOfTheWeek.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.grpDaysOfTheWeekDelete.ResumeLayout(False)
        Me.grpDaysOfTheWeekDelete.PerformLayout()
        Me.deleteOldRestorePointsTaskSettings.ResumeLayout(False)
        Me.deleteOldRestorePointsTaskSettings.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents timePicker As System.Windows.Forms.DateTimePicker
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents grpDaysOfTheWeek As System.Windows.Forms.GroupBox
    Friend WithEvents chkSaturday As System.Windows.Forms.CheckBox
    Friend WithEvents chkFriday As System.Windows.Forms.CheckBox
    Friend WithEvents chkThursday As System.Windows.Forms.CheckBox
    Friend WithEvents chkWednesday As System.Windows.Forms.CheckBox
    Friend WithEvents chkTuesday As System.Windows.Forms.CheckBox
    Friend WithEvents chkMonday As System.Windows.Forms.CheckBox
    Friend WithEvents chkSunday As System.Windows.Forms.CheckBox
    Friend WithEvents btnSaveTask As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents radWeekly As System.Windows.Forms.RadioButton
    Friend WithEvents radDaily As System.Windows.Forms.RadioButton
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnDeleteTask As System.Windows.Forms.Button
    Friend WithEvents btnSet As System.Windows.Forms.Button
    Friend WithEvents txtDays As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents chkDeleteOldRestorePoints As System.Windows.Forms.CheckBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents lblCreateTaskLabel As System.Windows.Forms.Label
    Friend WithEvents btnDeleteTaskDelete As System.Windows.Forms.Button
    Friend WithEvents btnSaveTaskDelete As System.Windows.Forms.Button
    Friend WithEvents Panel2 As System.Windows.Forms.Panel
    Friend WithEvents radWeeklyDelete As System.Windows.Forms.RadioButton
    Friend WithEvents radDailyDelete As System.Windows.Forms.RadioButton
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents grpDaysOfTheWeekDelete As System.Windows.Forms.GroupBox
    Friend WithEvents chkSaturdayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkFridayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkThursdayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkWednesdayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkTuesdayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkMondayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents chkSundayDelete As System.Windows.Forms.CheckBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents timePickerDelete As System.Windows.Forms.DateTimePicker
    Friend WithEvents txtDaysDelete As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents radEvery As System.Windows.Forms.RadioButton
    Friend WithEvents lblEvery As System.Windows.Forms.Label
    Friend WithEvents txtEveryDay As System.Windows.Forms.TextBox
    Friend WithEvents lblDays As System.Windows.Forms.Label
    Friend WithEvents deleteOldRestorePointsTaskSettings As System.Windows.Forms.GroupBox
    Friend WithEvents chkRunMissedTask As System.Windows.Forms.CheckBox
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents chkRunMissedTaskDelete As System.Windows.Forms.CheckBox
    Friend WithEvents btnSetCustomName As System.Windows.Forms.Button
    Friend WithEvents chkWakeDelete As CheckBox
    Friend WithEvents chkWake As CheckBox
    Friend WithEvents lblRunTimes As Label
    Friend WithEvents lblRunTimesDelete As Label
End Class
