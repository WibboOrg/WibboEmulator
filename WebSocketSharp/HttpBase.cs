namespace WibboEmulator.WebSocketSharp;

#region License
/*
 * HttpBase.cs
 *
 * The MIT License
 *
 * Copyright (c) 2012-2022 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using WibboEmulator.WebSocketSharp.Net;

internal abstract class HttpBase(Version version, NameValueCollection headers)
{
    #region Private Fields

    private static readonly int MaxMessageHeaderLength;
    private string _messageBody;

    #endregion

    #region Protected Fields

    protected static readonly string CrLf;
    protected static readonly string CrLfHt;
    protected static readonly string CrLfSp;

    #endregion

    #region Static Constructor

    static HttpBase()
    {
        MaxMessageHeaderLength = 8192;

        CrLf = "\r\n";
        CrLfHt = "\r\n\t";
        CrLfSp = "\r\n ";
    }

    #endregion
    #region Protected Constructors

    #endregion

    #region Internal Properties

    internal byte[] MessageBodyData { get; private set; }

    #endregion

    #region Protected Properties

    protected string HeaderSection
    {
        get
        {
            var buff = new StringBuilder(64);

            foreach (var key in this.Headers.AllKeys)
            {
                _ = buff.AppendFormat("{0}: {1}{2}", key, this.Headers[key], CrLf);
            }

            _ = buff.Append(CrLf);

            return buff.ToString();
        }
    }

    #endregion

    #region Public Properties

    public bool HasMessageBody => this.MessageBodyData != null;

    public NameValueCollection Headers { get; } = headers;

    public string MessageBody
    {
        get
        {
            this._messageBody ??= this.GetMessageBody();

            return this._messageBody;
        }
    }

    public abstract string MessageHeader { get; }

    public Version ProtocolVersion { get; } = version;

    #endregion

    #region Private Methods

    private string GetMessageBody()
    {
        if (this.MessageBodyData == null || this.MessageBodyData.LongLength == 0)
        {
            return string.Empty;
        }

        var contentType = this.Headers["Content-Type"];

        var enc = contentType != null && contentType.Length > 0
                  ? HttpUtility.GetEncoding(contentType)
                  : Encoding.UTF8;

        return enc.GetString(this.MessageBodyData);
    }

    private static byte[] ReadMessageBodyFrom(Stream stream, string length)
    {

        if (!long.TryParse(length, out var len))
        {
            var msg = "It cannot be parsed.";

            throw new ArgumentException(msg, nameof(length));
        }

        if (len < 0)
        {
            var msg = "It is less than zero.";

            throw new ArgumentOutOfRangeException(nameof(length), msg);
        }

        return len > 1024
               ? stream.ReadBytes(len, 1024)
               : len > 0
                 ? stream.ReadBytes((int)len)
                 : null;
    }

    private static string[] ReadMessageHeaderFrom(Stream stream)
    {
        var buff = new List<byte>();
        var cnt = 0;
        void add(int i)
        {
            if (i == -1)
            {
                var msg = "The header could not be read from the data stream.";

                throw new EndOfStreamException(msg);
            }

            buff.Add((byte)i);

            cnt++;
        }

        var end = false;

        do
        {
            end = stream.ReadByte().IsEqualTo('\r', add)
                  && stream.ReadByte().IsEqualTo('\n', add)
                  && stream.ReadByte().IsEqualTo('\r', add)
                  && stream.ReadByte().IsEqualTo('\n', add);

            if (cnt > MaxMessageHeaderLength)
            {
                var msg = "The length of the header is greater than the max length.";

                throw new InvalidOperationException(msg);
            }
        }
        while (!end);

        var bytes = buff.ToArray();

        return Encoding.UTF8.GetString(bytes)
               .Replace(CrLfSp, " ")
               .Replace(CrLfHt, " ")
               .Split(new[] { CrLf }, StringSplitOptions.RemoveEmptyEntries);
    }

    #endregion

    #region Internal Methods

    internal void WriteTo(Stream stream)
    {
        var bytes = this.ToByteArray();

        stream.Write(bytes, 0, bytes.Length);
    }

    #endregion

    #region Protected Methods

    protected static T Read<T>(
      Stream stream, Func<string[], T> parser, int millisecondsTimeout
    )
      where T : HttpBase
    {
        T ret = null;

        var timeout = false;
        var timer = new Timer(
                      state =>
                      {
                          timeout = true;
                          stream.Close();
                      },
                      null,
                      millisecondsTimeout,
                      -1
                    );

        Exception exception = null;

        try
        {
            var header = ReadMessageHeaderFrom(stream);
            ret = parser(header);

            var contentLen = ret.Headers["Content-Length"];

            if (contentLen != null && contentLen.Length > 0)
            {
                ret.MessageBodyData = ReadMessageBodyFrom(stream, contentLen);
            }
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        finally
        {
            _ = timer.Change(-1, -1);
            timer.Dispose();
        }

        if (timeout)
        {
            var msg = "A timeout has occurred.";

            throw new WebSocketException(msg);
        }

        if (exception != null)
        {
            var msg = "An exception has occurred.";

            throw new WebSocketException(msg, exception);
        }

        return ret;
    }

    #endregion

    #region Public Methods

    public byte[] ToByteArray()
    {
        var headerData = Encoding.UTF8.GetBytes(this.MessageHeader);

        return this.MessageBodyData != null
               ? [.. headerData, .. this.MessageBodyData]
               : headerData;
    }

    public override string ToString() => this.MessageBodyData != null
               ? this.MessageHeader + this.MessageBody
               : this.MessageHeader;

    #endregion
}
