namespace WibboEmulator.WebSocketSharp.Net;

#region License
/*
 * HttpConnection.cs
 *
 * This code is derived from HttpConnection.cs (System.Net) of Mono
 * (http://www.mono-project.com).
 *
 * The MIT License
 *
 * Copyright (c) 2005 Novell, Inc. (http://www.novell.com)
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

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

#region Contributors
/*
 * Contributors:
 * - Liryna <liryna.stark@gmail.com>
 * - Rohan Singh <rohan-singh@hotmail.com>
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using WebSocketSharp;

internal sealed class HttpConnection : IDisposable
{
    #region Private Fields

    private int _attempts;
    private readonly byte[] _buffer;
    private static readonly int BufferLength;
    private HttpListenerContext _context;
    private StringBuilder _currentLine;
    private readonly EndPointListener _endPointListener;
    private InputState _inputState;
    private RequestStream _inputStream;
    private LineState _lineState;
    private readonly EndPoint _localEndPoint;
    private static readonly int MaxInputLength;
    private ResponseStream _outputStream;
    private int _position;
    private readonly EndPoint _remoteEndPoint;
    private MemoryStream _requestBuffer;
    private Socket _socket;
    private readonly object _sync;
    private int _timeout;
    private readonly Dictionary<int, bool> _timeoutCanceled;
    private Timer _timer;

    #endregion

    #region Static Constructor

    static HttpConnection()
    {
        BufferLength = 8192;
        MaxInputLength = 32768;
    }

    #endregion

    #region Internal Constructors

    internal HttpConnection(Socket socket, EndPointListener listener)
    {
        this._socket = socket;
        this._endPointListener = listener;

        var netStream = new NetworkStream(socket, false);

        if (listener.IsSecure)
        {
            var sslConf = listener.SslConfiguration;
            var sslStream = new SslStream(
                              netStream,
                              false,
                              sslConf.ClientCertificateValidationCallback
                            );

            sslStream.AuthenticateAsServer(
              sslConf.ServerCertificate,
              sslConf.ClientCertificateRequired,
              sslConf.EnabledSslProtocols,
              sslConf.CheckCertificateRevocation
            );

            this.IsSecure = true;
            this.Stream = sslStream;
        }
        else
        {
            this.Stream = netStream;
        }

        this._buffer = new byte[BufferLength];
        this._localEndPoint = socket.LocalEndPoint;
        this._remoteEndPoint = socket.RemoteEndPoint;
        this._sync = new object();
        this._timeoutCanceled = [];
        this._timer = new Timer(OnTimeout, this, Timeout.Infinite, Timeout.Infinite);

        // 90k ms for first request, 15k ms from then on.
        this.Initialize(new MemoryStream(), 90000);
    }

    #endregion

    #region Public Properties

    public bool IsClosed => this._socket == null;

    public bool IsLocal => ((IPEndPoint)this._remoteEndPoint).Address.IsLocal();

    public bool IsSecure { get; }

    public IPEndPoint LocalEndPoint => (IPEndPoint)this._localEndPoint;

    public IPEndPoint RemoteEndPoint => (IPEndPoint)this._remoteEndPoint;

    public int Reuses { get; private set; }

    public Stream Stream { get; private set; }

    #endregion

    #region Private Methods

    private void Close()
    {
        lock (this._sync)
        {
            if (this._socket == null)
            {
                return;
            }

            this.DisposeTimer();
            this.DisposeRequestBuffer();
            this.DisposeStream();
            this.CloseSocket();
        }

        this._context.Unregister();
        this._endPointListener.RemoveConnection(this);

        this.Dispose();
    }

    private void CloseSocket()
    {
        try
        {
            this._socket.Shutdown(SocketShutdown.Both);
        }
        catch
        {
        }

        this._socket.Close();
        this._socket.Dispose();

        this._socket = null;
    }

    private static MemoryStream CreateRequestBuffer(
      RequestStream inputStream
    )
    {
        var ret = new MemoryStream();

        if (inputStream is ChunkedRequestStream crs)
        {
            if (crs.HasRemainingBuffer)
            {
                var buff = crs.RemainingBuffer;

                ret.Write(buff, 0, buff.Length);
            }

            return ret;
        }

        var cnt = inputStream.Count;

        if (cnt > 0)
        {
            ret.Write(inputStream.InitialBuffer, inputStream.Offset, cnt);
        }

        return ret;
    }

    private void DisposeRequestBuffer()
    {
        if (this._requestBuffer == null)
        {
            return;
        }

        this._requestBuffer.Dispose();

        this._requestBuffer = null;
    }

    private void DisposeStream()
    {
        if (this.Stream == null)
        {
            return;
        }

        this.Stream.Dispose();

        this.Stream = null;
    }

    private void DisposeTimer()
    {
        if (this._timer == null)
        {
            return;
        }

        try
        {
            _ = this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        catch
        {
        }

        this._timer.Dispose();

        this._timer = null;
    }

    private void Initialize(MemoryStream requestBuffer, int timeout)
    {
        this._requestBuffer = requestBuffer;
        this._timeout = timeout;

        this._context = new HttpListenerContext(this);
        this._currentLine = new StringBuilder(64);
        this._inputState = InputState.RequestLine;
        this._inputStream = null;
        this._lineState = LineState.None;
        this._outputStream = null;
        this._position = 0;
    }

    private static void OnRead(IAsyncResult asyncResult)
    {
        var conn = (HttpConnection)asyncResult.AsyncState;

        if (conn == null)
        {
            return;
        }

        var current = conn._attempts;

        if (conn._socket == null)
        {
            return;
        }

        lock (conn._sync)
        {
            if (conn._socket == null)
            {
                return;
            }

            _ = conn._timer.Change(Timeout.Infinite, Timeout.Infinite);
            conn._timeoutCanceled[current] = true;

            var nread = 0;

            try
            {
                nread = conn.Stream.EndRead(asyncResult);
            }
            catch (Exception)
            {
                // TODO: Logging.

                conn.Close();

                return;
            }

            if (nread <= 0)
            {
                conn.Close();

                return;
            }

            conn._requestBuffer.Write(conn._buffer, 0, nread);

            if (conn.ProcessRequestBuffer())
            {
                return;
            }

            conn.BeginReadRequest();
        }
    }

    private static void OnTimeout(object state)
    {
        var conn = (HttpConnection)state;
        var current = conn._attempts;

        if (conn._socket == null)
        {
            return;
        }

        lock (conn._sync)
        {
            if (conn._socket == null)
            {
                return;
            }

            if (conn._timeoutCanceled[current])
            {
                return;
            }

            conn._context.SendError(408);
        }
    }

    private bool ProcessInput(byte[] data, int length)
    {
        // This method returns a bool:
        // - true  Done processing
        // - false Need more input

        var req = this._context.Request;

        try
        {
            while (true)
            {
                var line = this.ReadLineFrom(data, this._position, length, out var nread);

                this._position += nread;

                if (line == null)
                {
                    break;
                }

                if (line.Length == 0)
                {
                    if (this._inputState == InputState.RequestLine)
                    {
                        continue;
                    }

                    if (this._position > MaxInputLength)
                    {
                        this._context.ErrorMessage = "Headers too long";
                    }

                    return true;
                }

                if (this._inputState == InputState.RequestLine)
                {
                    req.SetRequestLine(line);

                    this._inputState = InputState.Headers;
                }
                else
                {
                    req.AddHeader(line);
                }

                if (this._context.HasErrorMessage)
                {
                    return true;
                }
            }
        }
        catch (Exception)
        {
            // TODO: Logging.

            this._context.ErrorMessage = "Processing failure";

            return true;
        }

        if (this._position >= MaxInputLength)
        {
            this._context.ErrorMessage = "Headers too long";

            return true;
        }

        return false;
    }

    private bool ProcessRequestBuffer()
    {
        // This method returns a bool:
        // - true  Done processing
        // - false Need more write

        var data = this._requestBuffer.GetBuffer();
        var len = (int)this._requestBuffer.Length;

        if (!this.ProcessInput(data, len))
        {
            return false;
        }

        var req = this._context.Request;

        if (!this._context.HasErrorMessage)
        {
            req.FinishInitialization();
        }

        if (this._context.HasErrorMessage)
        {
            this._context.SendError();

            return true;
        }

        var uri = req.Url;

        if (!this._endPointListener.TrySearchHttpListener(uri, out var httplsnr))
        {
            this._context.SendError(404);

            return true;
        }

        _ = httplsnr.RegisterContext(this._context);

        return true;
    }

    private string ReadLineFrom(
      byte[] buffer, int offset, int length, out int nread
    )
    {
        nread = 0;

        for (var i = offset; i < length; i++)
        {
            nread++;

            var b = buffer[i];

            if (b == 13)
            {
                this._lineState = LineState.Cr;

                continue;
            }

            if (b == 10)
            {
                this._lineState = LineState.Lf;

                break;
            }

            _ = this._currentLine.Append((char)b);
        }

        if (this._lineState != LineState.Lf)
        {
            return null;
        }

        var ret = this._currentLine.ToString();

        this._currentLine.Length = 0;
        this._lineState = LineState.None;

        return ret;
    }

    private MemoryStream TakeOverRequestBuffer()
    {
        if (this._inputStream != null)
        {
            return CreateRequestBuffer(this._inputStream);
        }

        var ret = new MemoryStream();

        var buff = this._requestBuffer.GetBuffer();
        var len = (int)this._requestBuffer.Length;
        var cnt = len - this._position;

        if (cnt > 0)
        {
            ret.Write(buff, this._position, cnt);
        }

        this.DisposeRequestBuffer();

        return ret;
    }

    #endregion

    #region Internal Methods

    internal void BeginReadRequest()
    {
        this._attempts++;

        this._timeoutCanceled.Add(this._attempts, false);
        _ = this._timer.Change(this._timeout, Timeout.Infinite);

        try
        {
            _ = this.Stream.BeginRead(this._buffer, 0, BufferLength, OnRead, this);
        }
        catch (Exception)
        {
            // TODO: Logging.

            this.Close();
        }
    }

    internal void Close(bool force = false)
    {
        if (this._socket == null)
        {
            return;
        }

        lock (this._sync)
        {
            if (this._socket == null)
            {
                return;
            }

            if (force)
            {
                this._outputStream?.Close(true);

                this.Close();

                return;
            }

            this.GetResponseStream().Close(false);

            if (this._context.Response.CloseConnection)
            {
                this.Close();

                return;
            }

            if (!this._context.Request.FlushInput())
            {
                this.Close();

                return;
            }

            this._context.Unregister();

            this.Reuses++;

            var buff = this.TakeOverRequestBuffer();
            var len = buff.Length;

            this.Initialize(buff, 15000);

            if (len > 0)
            {
                if (this.ProcessRequestBuffer())
                {
                    return;
                }
            }

            this.BeginReadRequest();
        }

        this.Dispose();
    }

    #endregion

    #region Public Methods
    public RequestStream GetRequestStream(long contentLength, bool chunked)
    {
        lock (this._sync)
        {
            if (this._socket == null)
            {
                return null;
            }

            if (this._inputStream != null)
            {
                return this._inputStream;
            }

            var buff = this._requestBuffer.GetBuffer();
            var len = (int)this._requestBuffer.Length;
            var cnt = len - this._position;

            this._inputStream = chunked
                           ? new ChunkedRequestStream(
                               this.Stream, buff, this._position, cnt, this._context
                             )
                           : new RequestStream(
                               this.Stream, buff, this._position, cnt, contentLength
                             );

            this.DisposeRequestBuffer();

            return this._inputStream;
        }
    }

    public ResponseStream GetResponseStream()
    {
        lock (this._sync)
        {
            if (this._socket == null)
            {
                return null;
            }

            if (this._outputStream != null)
            {
                return this._outputStream;
            }

            var lsnr = this._context.Listener;
            var ignore = lsnr == null || lsnr.IgnoreWriteExceptions;

            this._outputStream = new ResponseStream(this.Stream, this._context.Response, ignore);

            return this._outputStream;
        }
    }

    public void Dispose() => GC.SuppressFinalize(this);

    #endregion
}
