namespace WibboEmulator.WebSocketSharp;

#region License
/*
 * Ext.cs
 *
 * Some parts of this code are derived from Mono (http://www.mono-project.com):
 * - GetStatusDescription is derived from HttpListenerResponse.cs (System.Net)
 * - MaybeUri is derived from Uri.cs (System)
 * - isPredefinedScheme is derived from Uri.cs (System)
 *
 * The MIT License
 *
 * Copyright (c) 2001 Garrett Rooney
 * Copyright (c) 2003 Ian MacLean
 * Copyright (c) 2003 Ben Maurer
 * Copyright (c) 2003, 2005, 2009 Novell, Inc. (http://www.novell.com)
 * Copyright (c) 2009 Stephane Delcroix
 * Copyright (c) 2010-2022 sta.blockhead
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

#region Contributors
/*
 * Contributors:
 * - Liryna <liryna.stark@gmail.com>
 * - Nikola Kovacevic <nikolak@outlook.com>
 * - Chris Swiedler
 */
#endregion

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using WebSocketSharp.Net;

/// <summary>
/// Provides a set of static methods for websocket-sharp.
/// </summary>
public static class Ext
{
    #region Private Fields

    private static readonly byte[] Last = [0x00];
    private static readonly int MaxRetry = 5;
    private const string TSPECIALS = "()<>@,;:\\\"/[]?={} \t";

    #endregion

    #region Private Methods

    private static byte[] Compress(this byte[] data)
    {
        if (data.LongLength == 0)
        {
            return data;
        }

        using var input = new MemoryStream(data);
        return input.CompressToArray();
    }

    private static MemoryStream Compress(this Stream stream)
    {
        var ret = new MemoryStream();

        if (stream.Length == 0)
        {
            return ret;
        }

        stream.Position = 0;

        var mode = CompressionMode.Compress;

        using var ds = new DeflateStream(ret, mode, true);
        stream.CopyTo(ds, 1024);
        ds.Close(); // BFINAL set to 1.
        ret.Write(Last, 0, 1);

        ret.Position = 0;

        return ret;
    }

    private static byte[] CompressToArray(this Stream stream)
    {
        using var output = stream.Compress();
        output.Close();

        return output.ToArray();
    }

    private static byte[] Decompress(this byte[] data)
    {
        if (data.LongLength == 0)
        {
            return data;
        }

        using var input = new MemoryStream(data);
        return input.DecompressToArray();
    }

    private static MemoryStream Decompress(this Stream stream)
    {
        var ret = new MemoryStream();

        if (stream.Length == 0)
        {
            return ret;
        }

        stream.Position = 0;

        var mode = CompressionMode.Decompress;

        using var ds = new DeflateStream(stream, mode, true);
        ds.CopyTo(ret, 1024);

        ret.Position = 0;

        return ret;
    }

    private static byte[] DecompressToArray(this Stream stream)
    {
        using var output = stream.Decompress();
        output.Close();

        return output.ToArray();
    }

    private static bool IsPredefinedScheme(this string value)
    {
        var c = value[0];

        if (c == 'h')
        {
            return value is "http" or "https";
        }

        if (c == 'w')
        {
            return value is "ws" or "wss";
        }

        if (c == 'f')
        {
            return value is "file" or "ftp";
        }

        if (c == 'g')
        {
            return value == "gopher";
        }

        if (c == 'm')
        {
            return value == "mailto";
        }

        if (c == 'n')
        {
            c = value[1];

            return c == 'e'
                   ? value is "news" or "net.pipe" or "net.tcp"
                   : value == "nntp";
        }

        return false;
    }

    #endregion

    #region Internal Methods

    internal static byte[] Append(this ushort code, string reason)
    {
        var codeAsBytes = code.ToByteArray(ByteOrder.Big);

        if (reason == null || reason.Length == 0)
        {
            return codeAsBytes;
        }

        var buff = new List<byte>(codeAsBytes);
        var reasonAsBytes = Encoding.UTF8.GetBytes(reason);

        buff.AddRange(reasonAsBytes);

        return [.. buff];
    }

    internal static byte[] Compress(
      this byte[] data, CompressionMethod method
    ) => method == CompressionMethod.Deflate
               ? data.Compress()
               : data;

    internal static Stream Compress(
      this Stream stream, CompressionMethod method
    ) => method == CompressionMethod.Deflate
               ? stream.Compress()
               : stream;

    internal static bool Contains(this string value, params char[] anyOf) => anyOf != null && anyOf.Length > 0 && value.IndexOfAny(anyOf) > -1;

    internal static bool Contains(
      this NameValueCollection collection, string name
    ) => collection[name] != null;

    internal static bool Contains(
      this NameValueCollection collection,
      string name,
      string value,
      StringComparison comparisonTypeForValue
    )
    {
        var val = collection[name];

        if (val == null)
        {
            return false;
        }

        foreach (var elm in val.Split(','))
        {
            if (elm.Trim().Equals(value, comparisonTypeForValue))
            {
                return true;
            }
        }

        return false;
    }

    internal static bool Contains<T>(
      this IEnumerable<T> source, Func<T, bool> condition
    )
    {
        foreach (var elm in source)
        {
            if (condition(elm))
            {
                return true;
            }
        }

        return false;
    }

    internal static bool ContainsTwice(this string[] values)
    {
        var len = values.Length;
        var end = len - 1;

        bool seek(int idx)
        {
            if (idx == end)
            {
                return false;
            }

            var val = values[idx];

            for (var i = idx + 1; i < len; i++)
            {
                if (values[i] == val)
                {
                    return true;
                }
            }

            return seek(++idx);
        }

        return seek(0);
    }

    internal static T[] Copy<T>(this T[] sourceArray, int length)
    {
        var dest = new T[length];

        Array.Copy(sourceArray, 0, dest, 0, length);

        return dest;
    }

    internal static T[] Copy<T>(this T[] sourceArray, long length)
    {
        var dest = new T[length];

        Array.Copy(sourceArray, 0, dest, 0, length);

        return dest;
    }

    internal static void CopyTo(
      this Stream sourceStream, Stream destinationStream, int bufferLength
    )
    {
        var buff = new byte[bufferLength];

        while (true)
        {
            var nread = sourceStream.Read(buff, 0, bufferLength);

            if (nread <= 0)
            {
                break;
            }

            destinationStream.Write(buff, 0, nread);
        }
    }

    internal static void CopyToAsync(
      this Stream sourceStream,
      Stream destinationStream,
      int bufferLength,
      Action completed,
      Action<Exception> error
    )
    {
        var buff = new byte[bufferLength];

        void callback(IAsyncResult ar)
        {
            try
            {
                var nread = sourceStream.EndRead(ar);

                if (nread <= 0)
                {
                    completed?.Invoke();

                    return;
                }

                destinationStream.Write(buff, 0, nread);

                _ = sourceStream.BeginRead(buff, 0, bufferLength, callback, null);
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
            }
        }

        try
        {
            _ = sourceStream.BeginRead(buff, 0, bufferLength, callback, null);
        }
        catch (Exception ex)
        {
            error?.Invoke(ex);
        }
    }

    internal static byte[] Decompress(
      this byte[] data, CompressionMethod method
    ) => method == CompressionMethod.Deflate
               ? data.Decompress()
               : data;

    internal static Stream Decompress(
      this Stream stream, CompressionMethod method
    ) => method == CompressionMethod.Deflate
               ? stream.Decompress()
               : stream;

    internal static byte[] DecompressToArray(
      this Stream stream, CompressionMethod method
    ) => method == CompressionMethod.Deflate
               ? stream.DecompressToArray()
               : stream.ToByteArray();

    internal static void Emit(
      this EventHandler eventHandler, object sender, EventArgs e
    )
    {
        if (eventHandler == null)
        {
            return;
        }

        eventHandler(sender, e);
    }

    internal static void Emit<TEventArgs>(
      this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs e
    )
      where TEventArgs : EventArgs
    {
        if (eventHandler == null)
        {
            return;
        }

        eventHandler(sender, e);
    }

    internal static string GetAbsolutePath(this Uri uri)
    {
        if (uri.IsAbsoluteUri)
        {
            return uri.AbsolutePath;
        }

        var original = uri.OriginalString;

        if (original[0] != '/')
        {
            return null;
        }

        var idx = original.IndexOfAny(['?', '#']);

        return idx > 0 ? original[..idx] : original;
    }

    internal static CookieCollection GetCookies(
      this NameValueCollection headers, bool response
    )
    {
        var val = headers[response ? "Set-Cookie" : "Cookie"];

        return val != null
               ? CookieCollection.Parse(val, response)
               : [];
    }

    internal static string GetDnsSafeHost(this Uri uri, bool bracketIPv6) => bracketIPv6 && uri.HostNameType == UriHostNameType.IPv6
               ? uri.Host
               : uri.DnsSafeHost;

    internal static string GetMessage(this CloseStatusCode code) => code switch
    {
        CloseStatusCode.ProtocolError => "A protocol error has occurred.",
        CloseStatusCode.UnsupportedData => "Unsupported data has been received.",
        CloseStatusCode.Abnormal => "An abnormal error has occurred.",
        CloseStatusCode.InvalidData => "Invalid data has been received.",
        CloseStatusCode.PolicyViolation => "A policy violation has occurred.",
        CloseStatusCode.TooBig => "A too big message has been received.",
        CloseStatusCode.MandatoryExtension => "The client did not receive expected extension(s).",
        CloseStatusCode.ServerError => "The server got an internal error.",
        CloseStatusCode.TlsHandshakeFailure => "An error has occurred during a TLS handshake.",
        _ => string.Empty,
    };

    internal static string GetName(this string nameAndValue, char separator)
    {
        var idx = nameAndValue.IndexOf(separator);

        return idx > 0 ? nameAndValue[..idx].Trim() : null;
    }

    internal static string GetUTF8DecodedString(this byte[] bytes) => Encoding.UTF8.GetString(bytes);

    internal static byte[] GetUTF8EncodedBytes(this string s) => Encoding.UTF8.GetBytes(s);

    internal static string GetValue(this string nameAndValue, char separator) => nameAndValue.GetValue(separator, false);

    internal static string GetValue(
      this string nameAndValue, char separator, bool unquote
    )
    {
        var idx = nameAndValue.IndexOf(separator);

        if (idx < 0 || idx == nameAndValue.Length - 1)
        {
            return null;
        }

        var val = nameAndValue[(idx + 1)..].Trim();

        return unquote ? val.Unquote() : val;
    }

    internal static bool IsCompressionExtension(
      this string value, CompressionMethod method
    )
    {
        var val = method.ToExtensionString();
        var compType = StringComparison.Ordinal;

        return value.StartsWith(val, compType);
    }

    internal static bool IsControl(this byte opcode) => opcode is > 0x7 and < 0x10;

    internal static bool IsControl(this Opcode opcode) => opcode >= Opcode.Close;

    internal static bool IsData(this byte opcode) => opcode is 0x1 or 0x2;

    internal static bool IsData(this Opcode opcode) => opcode is Opcode.Text or Opcode.Binary;

    internal static bool IsEqualTo(
      this int value, char c, Action<int> beforeComparing
    )
    {
        beforeComparing(value);

        return value == c - 0;
    }

    internal static bool IsHttpMethod(this string value) => value is "GET"
               or "HEAD"
               or "POST"
               or "PUT"
               or "DELETE"
               or "CONNECT"
               or "OPTIONS"
               or "TRACE";

    internal static bool IsPortNumber(this int value) => value is > 0 and < 65536;

    internal static bool IsReserved(this ushort code) => code is 1004
               or 1005
               or 1006
               or 1015;

    internal static bool IsReserved(this CloseStatusCode code) => code is CloseStatusCode.Undefined
               or CloseStatusCode.NoStatus
               or CloseStatusCode.Abnormal
               or CloseStatusCode.TlsHandshakeFailure;

    internal static bool IsSupported(this byte opcode) => Enum.IsDefined(typeof(Opcode), opcode);

    internal static bool IsText(this string value)
    {
        var len = value.Length;

        for (var i = 0; i < len; i++)
        {
            var c = value[i];

            if (c < 0x20)
            {
                if (!"\r\n\t".Contains(c))
                {
                    return false;
                }

                if (c == '\n')
                {
                    i++;

                    if (i == len)
                    {
                        break;
                    }

                    c = value[i];

                    if (!" \t".Contains(c))
                    {
                        return false;
                    }
                }

                continue;
            }

            if (c == 0x7f)
            {
                return false;
            }
        }

        return true;
    }

    internal static bool IsToken(this string value)
    {
        foreach (var c in value)
        {
            if (c < 0x20)
            {
                return false;
            }

            if (c > 0x7e)
            {
                return false;
            }

            if (TSPECIALS.IndexOf(c) > -1)
            {
                return false;
            }
        }

        return true;
    }

    internal static bool KeepsAlive(
      this NameValueCollection headers, Version version
    )
    {
        var compType = StringComparison.OrdinalIgnoreCase;

        return version < HttpVersion.Version11
               ? headers.Contains("Connection", "keep-alive", compType)
               : !headers.Contains("Connection", "close", compType);
    }

    internal static bool MaybeUri(this string value)
    {
        var idx = value.IndexOf(':');

        if (idx is < 2 or > 9)
        {
            return false;
        }

        var schm = value[..idx];

        return schm.IsPredefinedScheme();
    }

    internal static string Quote(this string value)
    {
        var fmt = "\"{0}\"";
        var val = value.Replace("\"", "\\\"");

        return string.Format(fmt, val);
    }

    internal static byte[] ReadBytes(this Stream stream, int length)
    {
        var ret = new byte[length];

        var offset = 0;
        var retry = 0;

        while (length > 0)
        {
            var nread = stream.Read(ret, offset, length);

            if (nread <= 0)
            {
                if (retry < MaxRetry)
                {
                    retry++;

                    continue;
                }

                return ret.SubArray(0, offset);
            }

            retry = 0;

            offset += nread;
            length -= nread;
        }

        return ret;
    }

    internal static byte[] ReadBytes(
      this Stream stream, long length, int bufferLength
    )
    {

        if (stream == null)
        {
            return [];
        }

        using var dest = new MemoryStream();
        var buff = new byte[bufferLength];
        var retry = 0;

        while (length > 0)
        {
            if (length < bufferLength)
            {
                bufferLength = (int)length;
            }

            var nread = stream.Read(buff, 0, bufferLength);

            if (nread <= 0)
            {
                if (retry < MaxRetry)
                {
                    retry++;

                    continue;
                }

                break;
            }

            retry = 0;

            dest.Write(buff, 0, nread);

            length -= nread;
        }

        dest.Close();

        return dest.ToArray();
    }

    internal static void ReadBytesAsync(
      this Stream stream,
      int length,
      Action<byte[]> completed,
      Action<Exception> error
    )
    {
        if (stream == null || !stream.CanRead)
        {
            return;
        }

        var ret = new byte[length];

        var offset = 0;
        var retry = 0;

        void callback(IAsyncResult ar)
        {
            try
            {
                if (stream == null || !stream.CanRead)
                {
                    return;
                }

                var nread = stream.EndRead(ar);

                if (nread <= 0)
                {
                    if (retry < MaxRetry)
                    {
                        retry++;

                        _ = stream.BeginRead(ret, offset, length, callback, null);

                        return;
                    }

                    completed?.Invoke(ret.SubArray(0, offset));

                    return;
                }

                if (nread == length)
                {
                    completed?.Invoke(ret);

                    return;
                }

                retry = 0;

                offset += nread;
                length -= nread;

                _ = stream.BeginRead(ret, offset, length, callback, null);
            }
            catch (Exception ex)
            {
                error?.Invoke(ex);
            }
        }

        try
        {
            _ = stream.BeginRead(ret, offset, length, callback, null);
        }
        catch (Exception ex)
        {
            error?.Invoke(ex);
        }
    }

    internal static void ReadBytesAsync(
      this Stream stream,
      long length,
      int bufferLength,
      Action<byte[]> completed,
      Action<Exception> error
    )
    {
        var dest = new MemoryStream();

        var buff = new byte[bufferLength];
        var retry = 0;

        void read(long len)
        {
            if (len < bufferLength)
            {
                bufferLength = (int)len;
            }

            _ = stream.BeginRead(
        buff,
        0,
        bufferLength,
        ar =>
            {
                try
                {
                    if (stream == null)
                    {
                        return;
                    }

                    var nread = stream.EndRead(ar);

                    if (nread <= 0)
                    {
                        if (retry < MaxRetry)
                        {
                            retry++;

                            read(len);

                            return;
                        }

                        if (completed != null)
                        {
                            dest.Close();

                            var ret = dest.ToArray();
                            completed(ret);
                        }

                        dest.Dispose();

                        return;
                    }

                    dest.Write(buff, 0, nread);

                    if (nread == len)
                    {
                        if (completed != null)
                        {
                            dest.Close();

                            var ret = dest.ToArray();
                            completed(ret);
                        }

                        dest.Dispose();

                        return;
                    }

                    retry = 0;

                    read(len - nread);
                }
                catch (Exception ex)
                {
                    dest.Dispose();

                    error?.Invoke(ex);
                }
            },
        null
      );
        }

        try
        {
            read(length);
        }
        catch (Exception ex)
        {
            dest.Dispose();

            error?.Invoke(ex);
        }
    }

    internal static T[] Reverse<T>(this T[] array)
    {
        var len = array.LongLength;
        var ret = new T[len];

        var end = len - 1;

        for (long i = 0; i <= end; i++)
        {
            ret[i] = array[end - i];
        }

        return ret;
    }

    internal static IEnumerable<string> SplitHeaderValue(
      this string value, params char[] separators
    )
    {
        var len = value.Length;
        var end = len - 1;

        var buff = new StringBuilder(32);
        var escaped = false;
        var quoted = false;

        for (var i = 0; i <= end; i++)
        {
            var c = value[i];
            _ = buff.Append(c);

            if (c == '"')
            {
                if (escaped)
                {
                    escaped = false;

                    continue;
                }

                quoted = !quoted;

                continue;
            }

            if (c == '\\')
            {
                if (i == end)
                {
                    break;
                }

                if (value[i + 1] == '"')
                {
                    escaped = true;
                }

                continue;
            }

            if (Array.IndexOf(separators, c) > -1)
            {
                if (quoted)
                {
                    continue;
                }

                buff.Length -= 1;

                yield return buff.ToString();

                buff.Length = 0;

                continue;
            }
        }

        yield return buff.ToString();
    }

    internal static byte[] ToByteArray(this Stream stream)
    {
        stream.Position = 0;

        using var buff = new MemoryStream();
        stream.CopyTo(buff, 1024);
        buff.Close();

        return buff.ToArray();
    }

    internal static byte[] ToByteArray(this ushort value, ByteOrder order)
    {
        var ret = BitConverter.GetBytes(value);

        if (!order.IsHostOrder())
        {
            Array.Reverse(ret);
        }

        return ret;
    }

    internal static byte[] ToByteArray(this ulong value, ByteOrder order)
    {
        var ret = BitConverter.GetBytes(value);

        if (!order.IsHostOrder())
        {
            Array.Reverse(ret);
        }

        return ret;
    }

    internal static CompressionMethod ToCompressionMethod(this string value)
    {
        var methods = Enum.GetValues(typeof(CompressionMethod));

        foreach (CompressionMethod method in methods)
        {
            if (method.ToExtensionString() == value)
            {
                return method;
            }
        }

        return CompressionMethod.None;
    }

    internal static string ToExtensionString(
      this CompressionMethod method, params string[] parameters
    )
    {
        if (method == CompressionMethod.None)
        {
            return string.Empty;
        }

        var name = method.ToString().ToLower();
        var ename = string.Format("permessage-{0}", name);

        if (parameters == null || parameters.Length == 0)
        {
            return ename;
        }

        var eparams = parameters.ToString("; ");

        return string.Format("{0}; {1}", ename, eparams);
    }

    internal static int ToInt32(this string numericString) => int.Parse(numericString);

    internal static System.Net.IPAddress ToIPAddress(this string value)
    {
        if (value == null || value.Length == 0)
        {
            return null;
        }


        if (System.Net.IPAddress.TryParse(value, out var addr))
        {
            return addr;
        }

        try
        {
            var addrs = System.Net.Dns.GetHostAddresses(value);

            return addrs[0];
        }
        catch
        {
            return null;
        }
    }

    internal static List<TSource> ToList<TSource>(
      this IEnumerable<TSource> source
    ) => new(source);

    internal static string ToString(
      this System.Net.IPAddress address, bool bracketIPv6
    ) => bracketIPv6
               && address.AddressFamily == AddressFamily.InterNetworkV6
               ? string.Format("[{0}]", address)
               : address.ToString();

    internal static ushort ToUInt16(this byte[] source, ByteOrder sourceOrder)
    {
        var val = source.ToHostOrder(sourceOrder);

        return BitConverter.ToUInt16(val, 0);
    }

    internal static ulong ToUInt64(this byte[] source, ByteOrder sourceOrder)
    {
        var val = source.ToHostOrder(sourceOrder);

        return BitConverter.ToUInt64(val, 0);
    }

    internal static Version ToVersion(this string versionString) => new(versionString);

    internal static IEnumerable<string> TrimEach(
      this IEnumerable<string> source
    )
    {
        foreach (var elm in source)
        {
            yield return elm.Trim();
        }
    }

    internal static string TrimSlashFromEnd(this string value)
    {
        var ret = value.TrimEnd('/');

        return ret.Length > 0 ? ret : "/";
    }

    internal static string TrimSlashOrBackslashFromEnd(this string value)
    {
        var ret = value.TrimEnd('/', '\\');

        return ret.Length > 0 ? ret : value[0].ToString();
    }

    internal static bool TryCreateVersion(
      this string versionString, out Version result
    )
    {
        result = null;

        try
        {
            result = new Version(versionString);
        }
        catch
        {
            return false;
        }

        return true;
    }

    internal static bool TryCreateWebSocketUri(
      this string uriString, out Uri result, out string message
    )
    {
        result = null;
        message = null;

        var uri = uriString.ToUri();

        if (uri == null)
        {
            message = "An invalid URI string.";

            return false;
        }

        if (!uri.IsAbsoluteUri)
        {
            message = "A relative URI.";

            return false;
        }

        var schm = uri.Scheme;
        var valid = schm is "ws" or "wss";

        if (!valid)
        {
            message = "The scheme part is not 'ws' or 'wss'.";

            return false;
        }

        var port = uri.Port;

        if (port == 0)
        {
            message = "The port part is zero.";

            return false;
        }

        if (uri.Fragment.Length > 0)
        {
            message = "It includes the fragment component.";

            return false;
        }

        if (port == -1)
        {
            port = schm == "ws" ? 80 : 443;
            uriString = string.Format(
                          "{0}://{1}:{2}{3}",
                          schm,
                          uri.Host,
                          port,
                          uri.PathAndQuery
                        );

            result = new Uri(uriString);
        }
        else
        {
            result = uri;
        }

        return true;
    }

    internal static bool TryGetUTF8DecodedString(
      this byte[] bytes, out string s
    )
    {
        s = null;

        try
        {
            s = Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return false;
        }

        return true;
    }

    internal static bool TryGetUTF8EncodedBytes(
      this string s, out byte[] bytes
    )
    {
        bytes = null;

        try
        {
            bytes = Encoding.UTF8.GetBytes(s);
        }
        catch
        {
            return false;
        }

        return true;
    }

    internal static bool TryOpenRead(
      this FileInfo fileInfo, out FileStream fileStream
    )
    {
        fileStream = null;

        try
        {
            fileStream = fileInfo.OpenRead();
        }
        catch
        {
            return false;
        }

        return true;
    }

    internal static string Unquote(this string value)
    {
        var first = value.IndexOf('"');

        if (first == -1)
        {
            return value;
        }

        var last = value.LastIndexOf('"');

        if (last == first)
        {
            return value;
        }

        var len = last - first - 1;

        return len > 0
               ? value.Substring(first + 1, len).Replace("\\\"", "\"")
               : string.Empty;
    }

    internal static bool Upgrades(
      this NameValueCollection headers, string protocol
    )
    {
        var compType = StringComparison.OrdinalIgnoreCase;

        return headers.Contains("Upgrade", protocol, compType)
               && headers.Contains("Connection", "Upgrade", compType);
    }

    internal static string UrlDecode(this string value, Encoding encoding) => value.IndexOfAny(['%', '+']) > -1
               ? HttpUtility.UrlDecode(value, encoding)
               : value;

    internal static string UrlEncode(this string value, Encoding encoding) => HttpUtility.UrlEncode(value, encoding);

    internal static void WriteBytes(
      this Stream stream, byte[] bytes, int bufferLength
    )
    {
        using var src = new MemoryStream(bytes);
        src.CopyTo(stream, bufferLength);
    }

    internal static void WriteBytesAsync(
      this Stream stream,
      byte[] bytes,
      int bufferLength,
      Action completed,
      Action<Exception> error
    )
    {
        var src = new MemoryStream(bytes);

        src.CopyToAsync(
          stream,
          bufferLength,
          () =>
          {
              completed?.Invoke();

              src.Dispose();
          },
          ex =>
          {
              src.Dispose();

              error?.Invoke(ex);
          }
        );
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the description of the specified HTTP status code.
    /// </summary>
    /// <returns>
    /// A <see cref="string"/> that represents the description of
    /// the HTTP status code.
    /// </returns>
    /// <param name="code">
    ///   <para>
    ///   One of the <see cref="HttpStatusCode"/> enum values.
    ///   </para>
    ///   <para>
    ///   It specifies the HTTP status code.
    ///   </para>
    /// </param>
    public static string GetDescription(this HttpStatusCode code) => ((int)code).GetStatusDescription();

    /// <summary>
    /// Gets the description of the specified HTTP status code.
    /// </summary>
    /// <returns>
    ///   <para>
    ///   A <see cref="string"/> that represents the description of
    ///   the HTTP status code.
    ///   </para>
    ///   <para>
    ///   An empty string if the description is not present.
    ///   </para>
    /// </returns>
    /// <param name="code">
    /// An <see cref="int"/> that specifies the HTTP status code.
    /// </param>
    public static string GetStatusDescription(this int code) => code switch
    {
        100 => "Continue",
        101 => "Switching Protocols",
        102 => "Processing",
        200 => "OK",
        201 => "Created",
        202 => "Accepted",
        203 => "Non-Authoritative Information",
        204 => "No Content",
        205 => "Reset Content",
        206 => "Partial Content",
        207 => "Multi-Status",
        300 => "Multiple Choices",
        301 => "Moved Permanently",
        302 => "Found",
        303 => "See Other",
        304 => "Not Modified",
        305 => "Use Proxy",
        307 => "Temporary Redirect",
        400 => "Bad Request",
        401 => "Unauthorized",
        402 => "Payment Required",
        403 => "Forbidden",
        404 => "Not Found",
        405 => "Method Not Allowed",
        406 => "Not Acceptable",
        407 => "Proxy Authentication Required",
        408 => "Request Timeout",
        409 => "Conflict",
        410 => "Gone",
        411 => "Length Required",
        412 => "Precondition Failed",
        413 => "Request Entity Too Large",
        414 => "Request-Uri Too Long",
        415 => "Unsupported Media Type",
        416 => "Requested Range Not Satisfiable",
        417 => "Expectation Failed",
        422 => "Unprocessable Entity",
        423 => "Locked",
        424 => "Failed Dependency",
        500 => "Internal Server Error",
        501 => "Not Implemented",
        502 => "Bad Gateway",
        503 => "Service Unavailable",
        504 => "Gateway Timeout",
        505 => "Http Version Not Supported",
        507 => "Insufficient Storage",
        _ => string.Empty,
    };

    /// <summary>
    /// Determines whether the specified ushort is in the range of
    /// the status code for the WebSocket connection close.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   The ranges are the following:
    ///   </para>
    ///   <list type="bullet">
    ///     <item>
    ///       <term>
    ///       1000-2999: These numbers are reserved for definition by
    ///       the WebSocket protocol.
    ///       </term>
    ///     </item>
    ///     <item>
    ///       <term>
    ///       3000-3999: These numbers are reserved for use by libraries,
    ///       frameworks, and applications.
    ///       </term>
    ///     </item>
    ///     <item>
    ///       <term>
    ///       4000-4999: These numbers are reserved for private use.
    ///       </term>
    ///     </item>
    ///   </list>
    /// </remarks>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is in the range of
    /// the status code for the close; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="value">
    /// A <see cref="ushort"/> to test.
    /// </param>
    public static bool IsCloseStatusCode(this ushort value) => value is > 999 and < 5000;

    /// <summary>
    /// Determines whether the specified string is enclosed in
    /// the specified character.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is enclosed in
    /// <paramref name="c"/>; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="value">
    /// A <see cref="string"/> to test.
    /// </param>
    /// <param name="c">
    /// A <see cref="char"/> to find.
    /// </param>
    public static bool IsEnclosedIn(this string value, char c)
    {
        if (value == null)
        {
            return false;
        }

        var len = value.Length;

        return len > 1 && value[0] == c && value[len - 1] == c;
    }

    /// <summary>
    /// Determines whether the specified byte order is host (this computer
    /// architecture) byte order.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <paramref name="order"/> is host byte order; otherwise,
    /// <c>false</c>.
    /// </returns>
    /// <param name="order">
    /// One of the <see cref="ByteOrder"/> enum values to test.
    /// </param>
    public static bool IsHostOrder(this ByteOrder order) =>
        // true: !(true ^ true) or !(false ^ false)
        // false: !(true ^ false) or !(false ^ true)
        !(BitConverter.IsLittleEndian ^ order == ByteOrder.Little);

    /// <summary>
    /// Determines whether the specified IP address is a local IP address.
    /// </summary>
    /// <remarks>
    /// This local means NOT REMOTE for the current host.
    /// </remarks>
    /// <returns>
    /// <c>true</c> if <paramref name="address"/> is a local IP address;
    /// otherwise, <c>false</c>.
    /// </returns>
    /// <param name="address">
    /// A <see cref="System.Net.IPAddress"/> to test.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="address"/> is <see langword="null"/>.
    /// </exception>
    public static bool IsLocal(this System.Net.IPAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        if (address.Equals(System.Net.IPAddress.Any))
        {
            return true;
        }

        if (address.Equals(System.Net.IPAddress.Loopback))
        {
            return true;
        }

        if (Socket.OSSupportsIPv6)
        {
            if (address.Equals(System.Net.IPAddress.IPv6Any))
            {
                return true;
            }

            if (address.Equals(System.Net.IPAddress.IPv6Loopback))
            {
                return true;
            }
        }

        var name = System.Net.Dns.GetHostName();
        var addrs = System.Net.Dns.GetHostAddresses(name);

        foreach (var addr in addrs)
        {
            if (address.Equals(addr))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines whether the specified string is <see langword="null"/> or
    /// an empty string.
    /// </summary>
    /// <returns>
    /// <c>true</c> if <paramref name="value"/> is <see langword="null"/> or
    /// an empty string; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="value">
    /// A <see cref="string"/> to test.
    /// </param>
    public static bool IsNullOrEmpty(this string value) => value == null || value.Length == 0;

    /// <summary>
    /// Retrieves a sub-array from the specified array. A sub-array starts at
    /// the specified index in the array.
    /// </summary>
    /// <returns>
    /// An array of T that receives a sub-array.
    /// </returns>
    /// <param name="array">
    /// An array of T from which to retrieve a sub-array.
    /// </param>
    /// <param name="startIndex">
    /// An <see cref="int"/> that specifies the zero-based index in the array
    /// at which retrieving starts.
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of elements to retrieve.
    /// </param>
    /// <typeparam name="T">
    /// The type of elements in the array.
    /// </typeparam>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="array"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <para>
    ///   <paramref name="startIndex"/> is less than zero.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="startIndex"/> is greater than the end of the array.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than zero.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is greater than the number of elements from
    ///   <paramref name="startIndex"/> to the end of the array.
    ///   </para>
    /// </exception>
    public static T[] SubArray<T>(this T[] array, int startIndex, int length)
    {
        ArgumentNullException.ThrowIfNull(array);

        var len = array.Length;

        if (len == 0)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(startIndex, 0);

            ArgumentOutOfRangeException.ThrowIfNotEqual(length, 0);

            return array;
        }

        if (startIndex < 0 || startIndex >= len)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (length < 0 || length > len - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (length == 0)
        {
            return [];
        }

        if (length == len)
        {
            return array;
        }

        var ret = new T[length];

        Array.Copy(array, startIndex, ret, 0, length);

        return ret;
    }

    /// <summary>
    /// Retrieves a sub-array from the specified array. A sub-array starts at
    /// the specified index in the array.
    /// </summary>
    /// <returns>
    /// An array of T that receives a sub-array.
    /// </returns>
    /// <param name="array">
    /// An array of T from which to retrieve a sub-array.
    /// </param>
    /// <param name="startIndex">
    /// A <see cref="long"/> that specifies the zero-based index in the array
    /// at which retrieving starts.
    /// </param>
    /// <param name="length">
    /// A <see cref="long"/> that specifies the number of elements to retrieve.
    /// </param>
    /// <typeparam name="T">
    /// The type of elements in the array.
    /// </typeparam>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="array"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <para>
    ///   <paramref name="startIndex"/> is less than zero.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="startIndex"/> is greater than the end of the array.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than zero.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is greater than the number of elements from
    ///   <paramref name="startIndex"/> to the end of the array.
    ///   </para>
    /// </exception>
    public static T[] SubArray<T>(this T[] array, long startIndex, long length)
    {
        ArgumentNullException.ThrowIfNull(array);

        var len = array.LongLength;

        if (len == 0)
        {
            ArgumentOutOfRangeException.ThrowIfNotEqual(startIndex, 0);

            ArgumentOutOfRangeException.ThrowIfNotEqual(length, 0);

            return array;
        }

        if (startIndex < 0 || startIndex >= len)
        {
            throw new ArgumentOutOfRangeException(nameof(startIndex));
        }

        if (length < 0 || length > len - startIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        if (length == 0)
        {
            return [];
        }

        if (length == len)
        {
            return array;
        }

        var ret = new T[length];

        Array.Copy(array, startIndex, ret, 0, length);

        return ret;
    }

    /// <summary>
    /// Executes the specified delegate <paramref name="n"/> times.
    /// </summary>
    /// <param name="n">
    /// An <see cref="int"/> that specifies the number of times to execute.
    /// </param>
    /// <param name="action">
    ///   <para>
    ///   An <c>Action&lt;int&gt;</c> delegate to execute.
    ///   </para>
    ///   <para>
    ///   The <see cref="int"/> parameter is the zero-based count of iteration.
    ///   </para>
    /// </param>
    public static void Times(this int n, Action<int> action)
    {
        if (n <= 0)
        {
            return;
        }

        if (action == null)
        {
            return;
        }

        for (var i = 0; i < n; i++)
        {
            action(i);
        }
    }

    /// <summary>
    /// Executes the specified delegate <paramref name="n"/> times.
    /// </summary>
    /// <param name="n">
    /// A <see cref="long"/> that specifies the number of times to execute.
    /// </param>
    /// <param name="action">
    ///   <para>
    ///   An <c>Action&lt;long&gt;</c> delegate to execute.
    ///   </para>
    ///   <para>
    ///   The <see cref="long"/> parameter is the zero-based count of iteration.
    ///   </para>
    /// </param>
    public static void Times(this long n, Action<long> action)
    {
        if (n <= 0)
        {
            return;
        }

        if (action == null)
        {
            return;
        }

        for (long i = 0; i < n; i++)
        {
            action(i);
        }
    }

    /// <summary>
    /// Converts the order of elements in the specified byte array to
    /// host (this computer architecture) byte order.
    /// </summary>
    /// <returns>
    ///   <para>
    ///   An array of <see cref="byte"/> converted from
    ///   <paramref name="source"/>.
    ///   </para>
    ///   <para>
    ///   <paramref name="source"/> if the number of elements in
    ///   it is less than 2 or <paramref name="sourceOrder"/> is
    ///   same as host byte order.
    ///   </para>
    /// </returns>
    /// <param name="source">
    /// An array of <see cref="byte"/> to convert.
    /// </param>
    /// <param name="sourceOrder">
    ///   <para>
    ///   One of the <see cref="ByteOrder"/> enum values.
    ///   </para>
    ///   <para>
    ///   It specifies the order of elements in <paramref name="source"/>.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="source"/> is <see langword="null"/>.
    /// </exception>
    public static byte[] ToHostOrder(this byte[] source, ByteOrder sourceOrder)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (source.Length < 2)
        {
            return source;
        }

        if (sourceOrder.IsHostOrder())
        {
            return source;
        }

        return source.Reverse();
    }

    /// <summary>
    /// Converts the specified array to a string.
    /// </summary>
    /// <returns>
    ///   <para>
    ///   A <see cref="string"/> converted by concatenating each element of
    ///   <paramref name="array"/> across <paramref name="separator"/>.
    ///   </para>
    ///   <para>
    ///   An empty string if <paramref name="array"/> is an empty array.
    ///   </para>
    /// </returns>
    /// <param name="array">
    /// An array of T to convert.
    /// </param>
    /// <param name="separator">
    /// A <see cref="string"/> used to separate each element of
    /// <paramref name="array"/>.
    /// </param>
    /// <typeparam name="T">
    /// The type of elements in <paramref name="array"/>.
    /// </typeparam>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="array"/> is <see langword="null"/>.
    /// </exception>
    public static string ToString<T>(this T[] array, string separator)
    {
        ArgumentNullException.ThrowIfNull(array);

        var len = array.Length;

        if (len == 0)
        {
            return string.Empty;
        }

        var buff = new StringBuilder(64);
        var end = len - 1;

        for (var i = 0; i < end; i++)
        {
            _ = buff.AppendFormat("{0}{1}", array[i], separator);
        }

        _ = buff.AppendFormat("{0}", array[end]);

        return buff.ToString();
    }

    /// <summary>
    /// Converts the specified string to a <see cref="Uri"/>.
    /// </summary>
    /// <returns>
    ///   <para>
    ///   A <see cref="Uri"/> converted from <paramref name="value"/>.
    ///   </para>
    ///   <para>
    ///   <see langword="null"/> if the conversion has failed.
    ///   </para>
    /// </returns>
    /// <param name="value">
    /// A <see cref="string"/> to convert.
    /// </param>
    public static Uri ToUri(this string value)
    {
        if (value == null || value.Length == 0)
        {
            return null;
        }

        var kind = value.MaybeUri() ? UriKind.Absolute : UriKind.Relative;

        _ = Uri.TryCreate(value, kind, out var ret);

        return ret;
    }

    #endregion
}
