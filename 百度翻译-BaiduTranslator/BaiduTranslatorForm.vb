Imports System.Text.RegularExpressions

Public Class BaiduTranslatorForm
    Dim LauguageType() As String = {"auto", "zh", "en", "yue", "wyw", "jp", "kor", "fra", "spa", "th", "ara", "ru", "pt", "de", "it", "el", "nl", "pl", "bul", "est", "dan", "fin", "cs", "rom", "slo", "swe", "hu", "cht"}
    '语音朗读相关
    Dim Wscript As Object = CreateObject("Wscript.shell")
    Dim Speaker As Object = CreateObject("SAPI.SPVoice")

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        TextBox2.Text = Translate(TextBox1.Text, LauguageType(FromBox.SelectedIndex), LauguageType(ToBox.SelectedIndex))

        '使用正则判断语言，后来发现没什么卵用
        'Dim regChina As Regex = New Regex("^[^\x00-\xFF]") '中文正则
        'Dim regEnglish As Regex = New Regex("^[a-zA-Z]") '英文正则
        'If (regChina.IsMatch(原文)) Then TextBox2.Text = (Translate(原文, "zh", "en")) '汉译英
        'If (regEnglish.IsMatch(原文)) Then TextBox2.Text = (Translate(原文, "en", "zh")) '英译汉
    End Sub

    Private Sub BaiduTranslatorForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        FromBox.SelectedIndex = 0
        ToBox.SelectedIndex = 0
        If My.Computer.Clipboard.ContainsText Then
            TextBox1.Text = My.Computer.Clipboard.GetText
            TextBox2.Text = Translate(TextBox1.Text, "auto", "auto")
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Speaker.SPEAK(TextBox1.Text)
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Speaker.SPEAK(TextBox2.Text)
    End Sub
End Class
