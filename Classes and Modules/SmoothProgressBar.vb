Public Class SmoothProgressBar
    Private min As Integer = 0                     ' Minimum value for progress range
    Private max As Integer = 100                   ' Maximum value for progress range
    Private val As Integer = 0                     ' Current progress
    Private barColor As Color = Color.LightGreen   ' Color of progress meter

    Protected Overrides Sub OnResize(ByVal e As EventArgs)
        ' Invalidate the control to get a repaint.
        Me.Invalidate()
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim g As Graphics = e.Graphics
        Dim brush As SolidBrush = New SolidBrush(barColor)
        Dim percent As Decimal = (val - min) / (max - min)
        Dim rect As Rectangle = Me.ClientRectangle

        ' Calculate area for drawing the progress.
        rect.Width = rect.Width * percent

        ' Draw the progress meter.
        g.FillRectangle(brush, rect)

        ' Draw a three-dimensional border around the control.
        Draw3DBorder(g)

        ' Clean up.
        brush.Dispose()
        g.Dispose()
    End Sub

    Public Property Minimum() As Integer
        Get
            Return min
        End Get

        Set(ByVal Value As Integer)
            ' Prevent a negative value.
            If (Value < 0) Then
                min = 0
            End If

            ' Make sure that the minimum value is never set higher than the maximum value.
            If (Value > max) Then
                min = Value
                min = Value
            End If

            ' Make sure that the value is still in range.
            If (val < min) Then
                val = min
            End If

            ' Invalidate the control to get a repaint.
            Me.Invalidate()
        End Set
    End Property

    Public Property Maximum() As Integer
        Get
            Return max
        End Get

        Set(ByVal Value As Integer)
            ' Make sure that the maximum value is never set lower than the minimum value.
            If (Value < min) Then
                min = Value
            End If

            max = Value

            ' Make sure that the value is still in range.
            If (val > max) Then
                val = max
            End If

            ' Invalidate the control to get a repaint.
            Me.Invalidate()
        End Set
    End Property

    Public Property Value() As Integer
        Get
            Return val
        End Get

        Set(ByVal Value As Integer)
            Dim oldValue As Integer = val

            ' Make sure that the value does not stray outside the valid range.
            If (Value < min) Then
                val = min
            ElseIf (Value > max) Then
                val = max
            Else
                val = Value
            End If

            ' Invalidate only the changed area.
            Dim percent As Decimal

            Dim newValueRect As Rectangle = Me.ClientRectangle
            Dim oldValueRect As Rectangle = Me.ClientRectangle

            ' Use a new value to calculate the rectangle for progress.
            percent = (val - min) / (max - min)
            newValueRect.Width = newValueRect.Width * percent

            ' Use an old value to calculate the rectangle for progress.
            percent = (oldValue - min) / (max - min)
            oldValueRect.Width = oldValueRect.Width * percent

            Dim updateRect As Rectangle = New Rectangle()

            ' Find only the part of the screen that must be updated.
            If (newValueRect.Width > oldValueRect.Width) Then
                updateRect.X = oldValueRect.Size.Width
                updateRect.Width = newValueRect.Width - oldValueRect.Width
            Else
                updateRect.X = newValueRect.Size.Width
                updateRect.Width = oldValueRect.Width - newValueRect.Width
            End If

            updateRect.Height = Me.Height
            ' Invalidate only the intersection region.
            Me.Invalidate(updateRect)
        End Set
    End Property

    Public Property ProgressBarColor() As Color
        Get
            Return barColor
        End Get

        Set(ByVal Value As Color)
            barColor = Value

            ' Invalidate the control to get a repaint.
            Me.Invalidate()
        End Set
    End Property

    Private Sub Draw3DBorder(ByVal g As Graphics)
        Dim PenWidth As Integer = Pens.White.Width

        g.DrawLine(Pens.DarkGray, New Point(Me.ClientRectangle.Left, Me.ClientRectangle.Top), New Point(Me.ClientRectangle.Width - PenWidth, Me.ClientRectangle.Top))
        g.DrawLine(Pens.DarkGray, New Point(Me.ClientRectangle.Left, Me.ClientRectangle.Top), New Point(Me.ClientRectangle.Left, Me.ClientRectangle.Height - PenWidth))
        g.DrawLine(Pens.White, New Point(Me.ClientRectangle.Left, Me.ClientRectangle.Height - PenWidth), New Point(Me.ClientRectangle.Width - PenWidth, Me.ClientRectangle.Height - PenWidth))
        g.DrawLine(Pens.White, New Point(Me.ClientRectangle.Width - PenWidth, Me.ClientRectangle.Top), New Point(Me.ClientRectangle.Width - PenWidth, Me.ClientRectangle.Height - PenWidth))
    End Sub
End Class
