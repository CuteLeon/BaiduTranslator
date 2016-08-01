Imports System.Net
Imports System.Security.Cryptography
Imports System.Text

Module BaiduTranslatorModule
    ' 百度翻译API
    ' 参考文档：http://api.fanyi.baidu.com/api/trans/product/apidoc

    Private Const APPID As String = "2015063000000001"
    Private Const KEY As String = "12345678"

    'MD5加密算法
    Public Function GetMD5(ByVal ExpressString As String, ByVal EncodeType As Encoding) As String
        Dim MD5Provider As MD5 = New MD5CryptoServiceProvider()
        Dim ByteArray As Byte() = MD5Provider.ComputeHash(EncodeType.GetBytes(ExpressString))
        Dim TempStringBuilder As StringBuilder = New StringBuilder(32)
        For Index As Integer = 0 To ByteArray.Length - 1
            TempStringBuilder.Append(ByteArray(Index).ToString("x").PadLeft(2, "0"))
        Next
        Return TempStringBuilder.ToString
    End Function

    '翻译函数（原文，来自语言，目标语言）
    Public Function Translate(ByVal OriginalText As String, ByVal FromLanguage As String, ByVal ToLanguage As String) As String
        Try
            Dim MyRandom As Random = New Random(Int32.MaxValue) '生成一个随机数
            Dim RandomNumber As String = MyRandom.Next(1000000, Int32.MaxValue).ToString()
            Dim Sign As String = GetMD5(APPID & OriginalText & RandomNumber & KEY, Encoding.UTF8) 'MD5加密签名
            Dim URL As String = String.Format("http://api.fanyi.baidu.com/api/trans/vip/translate?appid={0}&salt={1}&from={2}&to={3}&sign={4}", APPID, RandomNumber, FromLanguage, ToLanguage, Sign) ' 请求网址
            Dim PostData As String = String.Format("q={0}", System.Web.HttpUtility.UrlEncode(OriginalText, Encoding.UTF8)) ' 以POST方式发送数据(非WEB项目需要添加引用System.Web)
            Dim TempBytes As Byte() = Encoding.UTF8.GetBytes(PostData)
            Dim Client As WebClient = New WebClient()
            Client.Encoding = Encoding.UTF8
            Client.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
            Client.Headers.Add("ContentLength", PostData.Length.ToString())
            Dim ResponseData As Byte() = Client.UploadData(URL, "POST", TempBytes)
            Dim StringResult As String = Encoding.GetEncoding("utf-8").GetString(ResponseData) ' 取得响应结果 
            Dim SplitString As String() = Strings.Split(StringResult, """") '
            Select Case SplitString(1)
                Case "error_code" '服务器返回错误信息
                    Return GetErrorMessage(SplitString(3))
                Case "from" '服务器返回翻译结果
                    Return GetDecodeString(SplitString(SplitString.Count - 2))
            End Select
            Return StringResult '字段未被识别，直接返回服务器发来的数据（百度不更改规则就不会发生）
        Catch ex As Exception
            Return "程序运行异常，请检查网络连接。"
        End Try
    End Function

    '判断错误信息（错误号）
    Private Function GetErrorMessage(ByVal ErrorID As String) As String
        Select Case ErrorID
            Case "52001" : Return "请求超时，请重试"
            Case "52002" : Return "系统错误，请重试"
            Case "52003" : Return "未授权用户，请检查您的appid 是否正确"
            Case "54000" : Return "必填参数为空，请检查是否少传参数"
            Case "58000" : Return "客户端IP非法，请检查个人资料里填写的IP地址是否正确"
            Case "54001" : Return "签名错误，请检查您的签名生成方法"
            Case "54003" : Return "访问频率受限，请降低您的调用频率"
            Case "58001" : Return "译文语言方向不支持，请检查译文语言是否在语言列表里"
            Case "54004" : Return "账户余额不足，请前往管理控制平台为账户充值"
            Case "54005" : Return "长query请求频繁，请降低长query的发送频率，3s后再试"
        End Select
        '未知错误，返回错误号
        Return "未知错误！错误代码：" & ErrorID
    End Function

    '转换翻译结果编码
    Private Function GetDecodeString(ByVal EncodeString As String) As String
        Dim SplitChars() As String = Strings.Split(EncodeString, "\u")
        Dim UnicodeChar As String
        Dim DecodeString As String = SplitChars.First

        For Index As Integer = 1 To UBound(SplitChars)
            If SplitChars(Index).Length < 4 Then '过滤非Unicode编码
                DecodeString &= "\u" & SplitChars(Index)
            ElseIf SplitChars(Index).Length = 4 Then '转换Unicode编码
                UnicodeChar = "&H" & SplitChars(Index)
                DecodeString &= ChrW(UnicodeChar)
            ElseIf SplitChars(Index).Length > 4 Then '过滤非Unicode编码并转换Unicode编码
                UnicodeChar = "&H" & Strings.Left(SplitChars(Index), 4)
                DecodeString &= ChrW(Val(UnicodeChar)) & SplitChars(Index).Substring(4)
            End If
        Next
        Return DecodeString
    End Function

End Module
