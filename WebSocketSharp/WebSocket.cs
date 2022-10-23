namespace WibboEmulator.WebSocketSharp;

#region License
/*
 * WebSocket.cs
 *
 * This code is derived from WebSocket.java
 * (http://github.com/adamac/Java-WebSocket-client).
 *
 * The MIT License
 *
 * Copyright (c) 2009 Adam MacBeth
 * Copyright (c) 2010-2016 sta.blockhead
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
 * - Frank Razenberg <frank@zzattack.org>
 * - David Wood <dpwood@gmail.com>
 * - Liryna <liryna.stark@gmail.com>
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using WibboEmulator.WebSocketSharp.Net;
using WibboEmulator.WebSocketSharp.Net.WebSockets;

/// <summary>
/// Implements the WebSocket interface.
/// </summary>
/// <remarks>
///   <para>
///   This class provides a set of methods and properties for two-way
///   communication using the WebSocket protocol.
///   </para>
///   <para>
///   The WebSocket protocol is defined in
///   <see href="http://tools.ietf.org/html/rfc6455">RFC 6455</see>.
///   </para>
/// </remarks>
public class WebSocket : IDisposable
{
    #region Private Fields

    private string _base64Key;
    private readonly bool _client;
    private Action _closeContext;
    private CompressionMethod _compression;
    private WebSocketContext _context;
    private bool _enableRedirection;
    private string _extensions;
    private object _forMessageEventQueue;
    private object _forPing;
    private object _forSend;
    private object _forState;
    private MemoryStream _fragmentsBuffer;
    private bool _fragmentsCompressed;
    private Opcode _fragmentsOpcode;
    private const string GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
    private bool _inContinuation;
    private volatile bool _inMessage;
    private volatile Logger _logger;
    private readonly Action<MessageEventArgs> _message;
    private Queue<MessageEventArgs> _messageEventQueue;
    private string _origin;
    private ManualResetEvent _pongReceived;
    private string _protocol;
    private volatile WebSocketState _readyState;
    private ManualResetEvent _receivingExited;
    private Stream _stream;
    private TcpClient _tcpClient;
    private const string VERSION = "13";
    private TimeSpan _waitTime;

    #endregion

    #region Internal Fields

    /// <summary>
    /// Represents the empty array of <see cref="byte"/> used internally.
    /// </summary>
    internal static readonly byte[] EmptyBytes;

    /// <summary>
    /// Represents the length used to determine whether the data should be fragmented in sending.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   The data will be fragmented if that length is greater than the value of this field.
    ///   </para>
    ///   <para>
    ///   If you would like to change the value, you must set it to a value between <c>125</c> and
    ///   <c>Int32.MaxValue - 14</c> inclusive.
    ///   </para>
    /// </remarks>
    internal static readonly int FragmentLength;

    /// <summary>
    /// Represents the random number generator used internally.
    /// </summary>
    internal static readonly RandomNumberGenerator RandomNumber;

    #endregion

    #region Static Constructor

    static WebSocket()
    {
        EmptyBytes = Array.Empty<byte>();
        FragmentLength = 1016;
        RandomNumber = RandomNumberGenerator.Create();
    }

    #endregion

    #region Internal Constructors

    // As server
    internal WebSocket(HttpListenerWebSocketContext context, string protocol)
    {
        this._context = context;
        this._protocol = protocol;

        this._closeContext = context.Close;
        this._logger = context.Log;
        this._message = this.Messages;
        this.IsSecure = context.IsSecureConnection;
        this._stream = context.Stream;
        this._waitTime = TimeSpan.FromSeconds(1);

        this.Init();
    }

    // As server
    internal WebSocket(TcpListenerWebSocketContext context, string protocol)
    {
        this._context = context;
        this._protocol = protocol;

        this._closeContext = context.Close;
        this._logger = context.Log;
        this._message = this.Messages;
        this.IsSecure = context.IsSecureConnection;
        this._stream = context.Stream;
        this._waitTime = TimeSpan.FromSeconds(1);

        this.Init();
    }

    #endregion

    #region Public Constructors

    #endregion

    #region Internal Properties

    internal CookieCollection CookieCollection { get; private set; }

    // As server
    internal Func<WebSocketContext, string> CustomHandshakeRequestChecker { get; set; }

    internal bool HasMessage
    {
        get
        {
            lock (this._forMessageEventQueue)
            {
                return this._messageEventQueue.Count > 0;
            }
        }
    }

    // As server
    internal bool IgnoreExtensions { get; set; }

    internal bool IsConnected => this._readyState is WebSocketState.Open or WebSocketState.Closing;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the compression method used to compress a message.
    /// </summary>
    /// <remarks>
    /// The set operation does nothing if the connection has already been
    /// established or it is closing.
    /// </remarks>
    /// <value>
    ///   <para>
    ///   One of the <see cref="CompressionMethod"/> enum values.
    ///   </para>
    ///   <para>
    ///   It specifies the compression method used to compress a message.
    ///   </para>
    ///   <para>
    ///   The default value is <see cref="CompressionMethod.None"/>.
    ///   </para>
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// The set operation is not available if this instance is not a client.
    /// </exception>
    public CompressionMethod Compression
    {
        get => this._compression;

        set
        {
            string msg;
            if (!this._client)
            {
                msg = "This instance is not a client.";
                throw new InvalidOperationException(msg);
            }

            if (!this.CanSet(out msg))
            {
                this._logger.Warn(msg);
                return;
            }

            lock (this._forState)
            {
                if (!this.CanSet(out msg))
                {
                    this._logger.Warn(msg);
                    return;
                }

                this._compression = value;
            }
        }
    }

    public IEnumerable<Cookie> Cookies
    {
        get
        {
            lock (this.CookieCollection.SyncRoot)
            {
                foreach (var cookie in this.CookieCollection)
                {
                    yield return cookie;
                }
            }
        }
    }

    /// <summary>
    /// Gets the credentials for the HTTP authentication (Basic/Digest).
    /// </summary>
    /// <value>
    ///   <para>
    ///   A <see cref="NetworkCredential"/> that represents the credentials
    ///   used to authenticate the client.
    ///   </para>
    ///   <para>
    ///   The default value is <see langword="null"/>.
    ///   </para>
    /// </value>
    public NetworkCredential Credentials { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether a <see cref="OnMessage"/> event
    /// is emitted when a ping is received.
    /// </summary>
    /// <value>
    ///   <para>
    ///   <c>true</c> if this instance emits a <see cref="OnMessage"/> event
    ///   when receives a ping; otherwise, <c>false</c>.
    ///   </para>
    ///   <para>
    ///   The default value is <c>false</c>.
    ///   </para>
    /// </value>
    public bool EmitOnPing { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the URL redirection for
    /// the handshake request is allowed.
    /// </summary>
    /// <remarks>
    /// The set operation does nothing if the connection has already been
    /// established or it is closing.
    /// </remarks>
    /// <value>
    ///   <para>
    ///   <c>true</c> if this instance allows the URL redirection for
    ///   the handshake request; otherwise, <c>false</c>.
    ///   </para>
    ///   <para>
    ///   The default value is <c>false</c>.
    ///   </para>
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// The set operation is not available if this instance is not a client.
    /// </exception>
    public bool EnableRedirection
    {
        get => this._enableRedirection;

        set
        {
            string msg;
            if (!this._client)
            {
                msg = "This instance is not a client.";
                throw new InvalidOperationException(msg);
            }

            if (!this.CanSet(out msg))
            {
                this._logger.Warn(msg);
                return;
            }

            lock (this._forState)
            {
                if (!this.CanSet(out msg))
                {
                    this._logger.Warn(msg);
                    return;
                }

                this._enableRedirection = value;
            }
        }
    }

    /// <summary>
    /// Gets the extensions selected by server.
    /// </summary>
    /// <value>
    /// A <see cref="string"/> that will be a list of the extensions
    /// negotiated between client and server, or an empty string if
    /// not specified or selected.
    /// </value>
    public string Extensions => this._extensions ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether the connection is alive.
    /// </summary>
    /// <remarks>
    /// The get operation returns the value by using a ping/pong
    /// if the current state of the connection is Open.
    /// </remarks>
    /// <value>
    /// <c>true</c> if the connection is alive; otherwise, <c>false</c>.
    /// </value>
    public bool IsAlive => this.Ping(EmptyBytes);

    /// <summary>
    /// Gets a value indicating whether a secure connection is used.
    /// </summary>
    /// <value>
    /// <c>true</c> if this instance uses a secure connection; otherwise,
    /// <c>false</c>.
    /// </value>
    public bool IsSecure { get; private set; }

    /// <summary>
    /// Gets the logging function.
    /// </summary>
    /// <remarks>
    /// The default logging level is <see cref="LogLevel.Error"/>.
    /// </remarks>
    /// <value>
    /// A <see cref="Logger"/> that provides the logging function.
    /// </value>
    public Logger Log
    {
        get => this._logger;

        internal set => this._logger = value;
    }

    /// <summary>
    /// Gets or sets the value of the HTTP Origin header to send with
    /// the handshake request.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   The HTTP Origin header is defined in
    ///   <see href="http://tools.ietf.org/html/rfc6454#section-7">
    ///   Section 7 of RFC 6454</see>.
    ///   </para>
    ///   <para>
    ///   This instance sends the Origin header if this property has any.
    ///   </para>
    ///   <para>
    ///   The set operation does nothing if the connection has already been
    ///   established or it is closing.
    ///   </para>
    /// </remarks>
    /// <value>
    ///   <para>
    ///   A <see cref="string"/> that represents the value of the Origin
    ///   header to send.
    ///   </para>
    ///   <para>
    ///   The syntax is &lt;scheme&gt;://&lt;host&gt;[:&lt;port&gt;].
    ///   </para>
    ///   <para>
    ///   The default value is <see langword="null"/>.
    ///   </para>
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// The set operation is not available if this instance is not a client.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   The value specified for a set operation is not an absolute URI string.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The value specified for a set operation includes the path segments.
    ///   </para>
    /// </exception>
    public string Origin
    {
        get => this._origin;

        set
        {
            string msg;
            if (!this._client)
            {
                msg = "This instance is not a client.";
                throw new InvalidOperationException(msg);
            }

            if (!value.IsNullOrEmpty())
            {
                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
                {
                    msg = "Not an absolute URI string.";
                    throw new ArgumentException(msg, nameof(value));
                }

                if (uri.Segments.Length > 1)
                {
                    msg = "It includes the path segments.";
                    throw new ArgumentException(msg, nameof(value));
                }
            }

            if (!this.CanSet(out msg))
            {
                this._logger.Warn(msg);
                return;
            }

            lock (this._forState)
            {
                if (!this.CanSet(out msg))
                {
                    this._logger.Warn(msg);
                    return;
                }

                this._origin = !value.IsNullOrEmpty() ? value.TrimEnd('/') : value;
            }
        }
    }

    /// <summary>
    /// Gets the name of subprotocol selected by the server.
    /// </summary>
    /// <value>
    ///   <para>
    ///   A <see cref="string"/> that will be one of the names of
    ///   subprotocols specified by client.
    ///   </para>
    ///   <para>
    ///   An empty string if not specified or selected.
    ///   </para>
    /// </value>
    public string Protocol
    {
        get => this._protocol ?? string.Empty;

        internal set => this._protocol = value;
    }

    /// <summary>
    /// Gets the current state of the connection.
    /// </summary>
    /// <value>
    ///   <para>
    ///   One of the <see cref="WebSocketState"/> enum values.
    ///   </para>
    ///   <para>
    ///   It indicates the current state of the connection.
    ///   </para>
    ///   <para>
    ///   The default value is <see cref="WebSocketState.Connecting"/>.
    ///   </para>
    /// </value>
    public WebSocketState ReadyState => this._readyState;

    /// <summary>
    /// Gets or sets the time to wait for the response to the ping or close.
    /// </summary>
    /// <remarks>
    /// The set operation does nothing if the connection has already been
    /// established or it is closing.
    /// </remarks>
    /// <value>
    ///   <para>
    ///   A <see cref="TimeSpan"/> to wait for the response.
    ///   </para>
    ///   <para>
    ///   The default value is the same as 5 seconds if this instance is
    ///   a client.
    ///   </para>
    /// </value>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The value specified for a set operation is zero or less.
    /// </exception>
    public TimeSpan WaitTime
    {
        get => this._waitTime;

        set
        {
            if (value <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Zero or less.");
            }

            if (!this.CanSet(out var msg))
            {
                this._logger.Warn(msg);
                return;
            }

            lock (this._forState)
            {
                if (!this.CanSet(out msg))
                {
                    this._logger.Warn(msg);
                    return;
                }

                this._waitTime = value;
            }
        }
    }

    #endregion

    #region Public Events

    /// <summary>
    /// Occurs when the WebSocket connection has been closed.
    /// </summary>
    public event EventHandler<CloseEventArgs> OnClose;

    /// <summary>
    /// Occurs when the <see cref="WebSocket"/> gets an error.
    /// </summary>
    public event EventHandler<ErrorEventArgs> OnError;

    /// <summary>
    /// Occurs when the <see cref="WebSocket"/> receives a message.
    /// </summary>
    public event EventHandler<MessageEventArgs> OnMessage;

    /// <summary>
    /// Occurs when the WebSocket connection has been established.
    /// </summary>
    public event EventHandler OnOpen;

    #endregion

    #region Private Methods

    // As server
    private bool AcceptWS()
    {
        lock (this._forState)
        {
            if (this._readyState == WebSocketState.Open)
            {
                var msg = "The connection has already been established.";

                this._logger.Trace(msg);

                return false;
            }

            if (this._readyState == WebSocketState.Closing)
            {
                var msg = "The connection is closing.";

                this._logger.Error(msg);

                this.Error(msg, null);

                return false;
            }

            if (this._readyState == WebSocketState.Closed)
            {
                var msg = "The connection has been closed.";

                this._logger.Error(msg);

                this.Error(msg, null);

                return false;
            }

            try
            {
                var accepted = this.AcceptHandshake();

                if (!accepted)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this._logger.Fatal(ex.Message);
                this._logger.Debug(ex.ToString());

                var msg = "An exception has occurred while attempting to accept.";
                this.Fatal(msg, ex);

                return false;
            }

            this._readyState = WebSocketState.Open;

            return true;
        }
    }

    // As server
    private bool AcceptHandshake()
    {
        var fmt = "A handshake request from {0}:\n{1}";
        var msg = string.Format(fmt, this._context.UserEndPoint, this._context);

        this._logger.Trace(msg);

        if (!this.CheckHandshakeRequest(this._context, out msg))
        {
            this._logger.Error(msg);

            var reason = "A handshake error has occurred while attempting to accept.";
            this.RefuseHandshake(CloseStatusCode.ProtocolError, reason);

            return false;
        }

        if (!this.CustomCheckHandshakeRequest(this._context, out msg))
        {
            this._logger.Error(msg);

            var reason = "A handshake error has occurred while attempting to accept.";
            this.RefuseHandshake(CloseStatusCode.PolicyViolation, reason);

            return false;
        }

        this._base64Key = this._context.Headers["Sec-WebSocket-Key"];

        if (this._protocol != null)
        {
            var vals = this._context.SecWebSocketProtocols;

            this.ProcessSecWebSocketProtocolClientHeader(vals);
        }

        if (!this.IgnoreExtensions)
        {
            var val = this._context.Headers["Sec-WebSocket-Extensions"];

            this.ProcessSecWebSocketExtensionsClientHeader(val);
        }

        var res = this.CreateHandshakeResponse();

        return this.SendHttpResponse(res);
    }

    private bool CanSet(out string message)
    {
        message = null;

        if (this._readyState == WebSocketState.Open)
        {
            message = "The connection has already been established.";
            return false;
        }

        if (this._readyState == WebSocketState.Closing)
        {
            message = "The connection is closing.";
            return false;
        }

        return true;
    }

    // As server
    private bool CheckHandshakeRequest(
      WebSocketContext context, out string message
    )
    {
        message = null;

        if (!context.IsWebSocketRequest)
        {
            message = "Not a WebSocket handshake request.";

            return false;
        }

        if (context.RequestUri == null)
        {
            message = "The Request-URI is invalid.";

            return false;
        }

        var headers = context.Headers;

        var key = headers["Sec-WebSocket-Key"];

        if (key == null)
        {
            message = "The Sec-WebSocket-Key header is non-existent.";

            return false;
        }

        if (key.Length == 0)
        {
            message = "The Sec-WebSocket-Key header is invalid.";

            return false;
        }

        var ver = headers["Sec-WebSocket-Version"];

        if (ver == null)
        {
            message = "The Sec-WebSocket-Version header is non-existent.";

            return false;
        }

        if (ver != VERSION)
        {
            message = "The Sec-WebSocket-Version header is invalid.";

            return false;
        }

        var subps = headers["Sec-WebSocket-Protocol"];

        if (subps != null && subps.Length == 0)
        {
            message = "The Sec-WebSocket-Protocol header is invalid.";

            return false;
        }

        if (!this.IgnoreExtensions)
        {
            var exts = headers["Sec-WebSocket-Extensions"];

            if (exts != null && exts.Length == 0)
            {
                message = "The Sec-WebSocket-Extensions header is invalid.";

                return false;
            }
        }

        return true;
    }

    private bool CheckReceivedFrame(WebSocketFrame frame, out string message)
    {
        message = null;

        var masked = frame.IsMasked;

        if (this._client && masked)
        {
            message = "A frame from the server is masked.";

            return false;
        }

        if (!this._client && !masked)
        {
            message = "A frame from a client is not masked.";

            return false;
        }

        if (this._inContinuation && frame.IsData)
        {
            message = "A data frame was received while receiving continuation frames.";

            return false;
        }

        if (frame.IsCompressed && this._compression == CompressionMethod.None)
        {
            message = "A compressed frame was received without any agreement for it.";

            return false;
        }

        if (frame.Rsv2 == Rsv.On)
        {
            message = "The RSV2 of a frame is non-zero without any negotiation for it.";

            return false;
        }

        if (frame.Rsv3 == Rsv.On)
        {
            message = "The RSV3 of a frame is non-zero without any negotiation for it.";

            return false;
        }

        return true;
    }

    private void CloseWS(ushort code, string reason)
    {
        if (this._readyState == WebSocketState.Closing)
        {
            this._logger.Trace("The closing is already in progress.");

            return;
        }

        if (this._readyState == WebSocketState.Closed)
        {
            this._logger.Trace("The connection has already been closed.");

            return;
        }

        if (code == 1005)
        {
            this.Close(PayloadData.Empty, true, false);

            return;
        }

        var data = new PayloadData(code, reason);
        var send = !code.IsReserved();

        this.Close(data, send, false);
    }

    private void Close(PayloadData payloadData, bool send, bool received)
    {
        lock (this._forState)
        {
            if (this._readyState == WebSocketState.Closing)
            {
                this._logger.Trace("The closing is already in progress.");

                return;
            }

            if (this._readyState == WebSocketState.Closed)
            {
                this._logger.Trace("The connection has already been closed.");

                return;
            }

            send = send && this._readyState == WebSocketState.Open;

            this._readyState = WebSocketState.Closing;
        }

        this._logger.Trace("Begin closing the connection.");

        var res = this.CloseHandshake(payloadData, send, received);

        this.ReleaseResources();

        this._logger.Trace("End closing the connection.");

        this._readyState = WebSocketState.Closed;

        var e = new CloseEventArgs(payloadData, res);

        try
        {
            OnClose.Emit(this, e);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());
        }
    }

    private void CloseWSAsync(ushort code, string reason)
    {
        if (this._readyState == WebSocketState.Closing)
        {
            this._logger.Trace("The closing is already in progress.");

            return;
        }

        if (this._readyState == WebSocketState.Closed)
        {
            this._logger.Trace("The connection has already been closed.");

            return;
        }

        if (code == 1005)
        {
            this.CloseAsync(PayloadData.Empty, true, false);

            return;
        }

        var data = new PayloadData(code, reason);
        var send = !code.IsReserved();

        this.CloseAsync(data, send, false);
    }

    private void CloseAsync(PayloadData payloadData, bool send, bool received)
    {
        Action<PayloadData, bool, bool> closer = this.Close;

        _ = closer.BeginInvoke(
          payloadData, send, received, ar => closer.EndInvoke(ar), null
        );
    }

    private bool CloseHandshake(
      PayloadData payloadData, bool send, bool received
    )
    {
        var sent = false;

        if (send)
        {
            var frame = WebSocketFrame.CreateCloseFrame(payloadData, this._client);
            var bytes = frame.ToArray();

            sent = this.SendBytes(bytes);

            if (this._client)
            {
                frame.Unmask();
            }
        }

        var wait = !received && sent;

        if (wait && this._receivingExited != null)
        {
            received = this._receivingExited.WaitOne(this._waitTime);
        }

        var ret = sent && received;

        var msg = string.Format(
                    "The closing was clean? {0} (sent: {1} received: {2})",
                    ret,
                    sent,
                    received
                  );

        this._logger.Trace(msg);

        return ret;
    }

    // As server
    private static HttpResponse CreateHandshakeFailureResponse(HttpStatusCode code)
    {
        var ret = HttpResponse.CreateCloseResponse(code);

        ret.Headers["Sec-WebSocket-Version"] = VERSION;

        return ret;
    }

    // As server
    private HttpResponse CreateHandshakeResponse()
    {
        var ret = HttpResponse.CreateWebSocketHandshakeResponse();

        var headers = ret.Headers;

        headers["Sec-WebSocket-Accept"] = CreateResponseKey(this._base64Key);

        if (this._protocol != null)
        {
            headers["Sec-WebSocket-Protocol"] = this._protocol;
        }

        if (this._extensions != null)
        {
            headers["Sec-WebSocket-Extensions"] = this._extensions;
        }

        if (this.CookieCollection.Count > 0)
        {
            ret.SetCookies(this.CookieCollection);
        }

        return ret;
    }

    // As server
    private bool CustomCheckHandshakeRequest(
      WebSocketContext context, out string message
    )
    {
        message = null;

        if (this.CustomHandshakeRequestChecker == null)
        {
            return true;
        }

        message = this.CustomHandshakeRequestChecker(context);

        return message == null;
    }

    private void EnqueueToMessageEventQueue(MessageEventArgs e)
    {
        lock (this._forMessageEventQueue)
        {
            this._messageEventQueue.Enqueue(e);
        }
    }

    private void Error(string message, Exception exception)
    {
        var e = new ErrorEventArgs(message, exception);

        try
        {
            OnError.Emit(this, e);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());
        }
    }

    private void Fatal(string message, Exception exception)
    {
        var code = exception is WebSocketException exception1
                   ? exception1.Code
                   : CloseStatusCode.Abnormal;

        this.Fatal(message, (ushort)code);
    }

    private void Fatal(string message, ushort code)
    {
        var data = new PayloadData(code, message);

        this.Close(data, false, false);
    }

    private void Init()
    {
        this._compression = CompressionMethod.None;
        this.CookieCollection = new CookieCollection();
        this._forPing = new object();
        this._forSend = new object();
        this._forState = new object();
        this._messageEventQueue = new Queue<MessageEventArgs>();
        this._forMessageEventQueue = ((ICollection)this._messageEventQueue).SyncRoot;
        this._readyState = WebSocketState.Connecting;
    }

    private void Message()
    {
        MessageEventArgs e = null;

        lock (this._forMessageEventQueue)
        {
            if (this._inMessage)
            {
                return;
            }

            if (this._messageEventQueue.Count == 0)
            {
                return;
            }

            if (this._readyState != WebSocketState.Open)
            {
                return;
            }

            e = this._messageEventQueue.Dequeue();

            this._inMessage = true;
        }

        this._message(e);
    }

    private void Messages(MessageEventArgs e)
    {
        try
        {
            OnMessage.Emit(this, e);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());

            this.Error("An exception has occurred during an OnMessage event.", ex);
        }

        lock (this._forMessageEventQueue)
        {
            if (this._messageEventQueue.Count == 0)
            {
                this._inMessage = false;

                return;
            }

            if (this._readyState != WebSocketState.Open)
            {
                this._inMessage = false;

                return;
            }

            e = this._messageEventQueue.Dequeue();
        }

        _ = Task.Run(() => this.Messages(e));
    }

    private void OpenWS()
    {
        this._inMessage = true;

        this.StartReceiving();

        try
        {
            OnOpen.Emit(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());

            this.Error("An exception has occurred during the OnOpen event.", ex);
        }

        MessageEventArgs e = null;

        lock (this._forMessageEventQueue)
        {
            if (this._messageEventQueue.Count == 0)
            {
                this._inMessage = false;

                return;
            }

            if (this._readyState != WebSocketState.Open)
            {
                this._inMessage = false;

                return;
            }

            e = this._messageEventQueue.Dequeue();
        }

        if (this._message != null)
        {
            _ = Task.Run(() => this._message(e));
        }
    }

    private bool Ping(byte[] data)
    {
        if (this._readyState != WebSocketState.Open)
        {
            return false;
        }

        var received = this._pongReceived;

        if (received == null)
        {
            return false;
        }

        lock (this._forPing)
        {
            try
            {
                _ = received.Reset();

                var sent = this.Send(Fin.Final, Opcode.Ping, data, false);

                if (!sent)
                {
                    return false;
                }

                return received.WaitOne(this._waitTime);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }

    private bool ProcessCloseFrame(WebSocketFrame frame)
    {
        var data = frame.PayloadData;
        var send = !data.HasReservedCode;

        this.Close(data, send, true);

        return false;
    }

    private bool ProcessDataFrame(WebSocketFrame frame)
    {
        var e = frame.IsCompressed
                ? new MessageEventArgs(
                    frame.Opcode,
                    frame.PayloadData.ApplicationData.Decompress(this._compression)
                  )
                : new MessageEventArgs(frame);

        this.EnqueueToMessageEventQueue(e);

        return true;
    }

    private bool ProcessFragmentFrame(WebSocketFrame frame)
    {
        if (!this._inContinuation)
        {
            if (frame.IsContinuation)
            {
                return true;
            }

            this._fragmentsOpcode = frame.Opcode;
            this._fragmentsCompressed = frame.IsCompressed;
            this._fragmentsBuffer = new MemoryStream();
            this._inContinuation = true;
        }

        this._fragmentsBuffer.WriteBytes(frame.PayloadData.ApplicationData, 1024);

        if (frame.IsFinal)
        {
            using (this._fragmentsBuffer)
            {
                var data = this._fragmentsCompressed
                           ? this._fragmentsBuffer.DecompressToArray(this._compression)
                           : this._fragmentsBuffer.ToArray();

                var e = new MessageEventArgs(this._fragmentsOpcode, data);

                this.EnqueueToMessageEventQueue(e);
            }

            this._fragmentsBuffer = null;
            this._inContinuation = false;
        }

        return true;
    }

    private bool ProcessPingFrame(WebSocketFrame frame)
    {
        this._logger.Trace("A ping was received.");

        var pong = WebSocketFrame.CreatePongFrame(frame.PayloadData, this._client);

        lock (this._forState)
        {
            if (this._readyState != WebSocketState.Open)
            {
                this._logger.Trace("A pong to this ping cannot be sent.");

                return true;
            }

            var bytes = pong.ToArray();
            var sent = this.SendBytes(bytes);

            if (!sent)
            {
                return false;
            }
        }

        this._logger.Trace("A pong to this ping has been sent.");

        if (this.EmitOnPing)
        {
            if (this._client)
            {
                pong.Unmask();
            }

            var e = new MessageEventArgs(frame);

            this.EnqueueToMessageEventQueue(e);
        }

        return true;
    }

    private bool ProcessPongFrame(WebSocketFrame frame)
    {
        if (frame is null)
        {
            throw new ArgumentNullException(nameof(frame));
        }

        this._logger.Trace("A pong was received.");

        try
        {
            _ = this._pongReceived.Set();
        }
        catch (NullReferenceException)
        {
            return false;
        }
        catch (ObjectDisposedException)
        {
            return false;
        }

        this._logger.Trace("It has been signaled.");

        return true;
    }

    private bool ProcessReceivedFrame(WebSocketFrame frame)
    {
        if (!this.CheckReceivedFrame(frame, out var msg))
        {
            throw new WebSocketException(CloseStatusCode.ProtocolError, msg);
        }

        frame.Unmask();

        return frame.IsFragment
               ? this.ProcessFragmentFrame(frame)
               : frame.IsData
                 ? this.ProcessDataFrame(frame)
                 : frame.IsPing
                   ? this.ProcessPingFrame(frame)
                   : frame.IsPong
                     ? this.ProcessPongFrame(frame)
                     : frame.IsClose
                       ? this.ProcessCloseFrame(frame)
                       : this.ProcessUnsupportedFrame(frame);
    }

    // As server
    private void ProcessSecWebSocketExtensionsClientHeader(string value)
    {
        if (value == null)
        {
            return;
        }

        var buff = new StringBuilder(80);

        var comp = false;

        foreach (var elm in value.SplitHeaderValue(','))
        {
            var ext = elm.Trim();

            if (ext.Length == 0)
            {
                continue;
            }

            if (!comp)
            {
                if (ext.IsCompressionExtension(CompressionMethod.Deflate))
                {
                    this._compression = CompressionMethod.Deflate;

                    var str = this._compression.ToExtensionString(
                                "client_no_context_takeover",
                                "server_no_context_takeover"
                              );

                    _ = buff.AppendFormat("{0}, ", str);

                    comp = true;
                }
            }
        }

        var len = buff.Length;

        if (len <= 2)
        {
            return;
        }

        buff.Length = len - 2;

        this._extensions = buff.ToString();
    }

    // As server
    private void ProcessSecWebSocketProtocolClientHeader(
      IEnumerable<string> values
    )
    {
        if (values.Contains(val => val == this._protocol))
        {
            return;
        }

        this._protocol = null;
    }

    private bool ProcessUnsupportedFrame(WebSocketFrame frame)
    {
        this._logger.Fatal("An unsupported frame was received.");
        this._logger.Debug("The frame is" + frame.PrintToString(false));

        this.Fatal("There is no way to handle it.", 1003);

        return false;
    }

    // As server
    private void RefuseHandshake(CloseStatusCode code, string reason)
    {
        this._readyState = WebSocketState.Closing;

        var res = CreateHandshakeFailureResponse(HttpStatusCode.BadRequest);
        _ = this.SendHttpResponse(res);

        this.ReleaseServerResources();

        this._readyState = WebSocketState.Closed;

        var e = new CloseEventArgs((ushort)code, reason, false);

        try
        {
            OnClose.Emit(this, e);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());
        }
    }

    // As client
    private void ReleaseClientResources()
    {
        if (this._stream != null)
        {
            this._stream.Dispose();
            this._stream = null;
        }

        if (this._tcpClient != null)
        {
            this._tcpClient.Close();
            this._tcpClient = null;
        }
    }

    private void ReleaseCommonResources()
    {
        if (this._fragmentsBuffer != null)
        {
            this._fragmentsBuffer.Dispose();
            this._fragmentsBuffer = null;
            this._inContinuation = false;
        }

        if (this._pongReceived != null)
        {
            this._pongReceived.Close();
            this._pongReceived = null;
        }

        if (this._receivingExited != null)
        {
            this._receivingExited.Close();
            this._receivingExited = null;
        }
    }

    private void ReleaseResources()
    {
        if (this._client)
        {
            this.ReleaseClientResources();
        }
        else
        {
            this.ReleaseServerResources();
        }

        this.ReleaseCommonResources();
    }

    // As server
    private void ReleaseServerResources()
    {
        if (this._closeContext == null)
        {
            return;
        }

        this._closeContext();
        this._closeContext = null;
        this._stream = null;
        this._context = null;
    }

    private bool Send(Opcode opcode, Stream stream)
    {
        lock (this._forSend)
        {
            var src = stream;
            var compressed = false;
            var sent = false;

            try
            {
                if (this._compression != CompressionMethod.None)
                {
                    stream = stream.Compress(this._compression);
                    compressed = true;
                }

                sent = this.Send(opcode, stream, compressed);

                if (!sent)
                {
                    this.Error("A send has been interrupted.", null);
                }
            }
            catch (Exception ex)
            {
                this._logger.Error(ex.Message);
                this._logger.Debug(ex.ToString());

                this.Error("An exception has occurred during a send.", ex);
            }
            finally
            {
                if (compressed)
                {
                    stream.Dispose();
                }

                src.Dispose();
            }

            return sent;
        }
    }

    private bool Send(Opcode opcode, Stream stream, bool compressed)
    {
        var len = stream.Length;

        if (len == 0)
        {
            return this.Send(Fin.Final, opcode, EmptyBytes, false);
        }

        var quo = len / FragmentLength;
        var rem = (int)(len % FragmentLength);

        byte[] buff;
        if (quo == 0)
        {
            buff = new byte[rem];

            return stream.Read(buff, 0, rem) == rem
                   && this.Send(Fin.Final, opcode, buff, compressed);
        }

        if (quo == 1 && rem == 0)
        {
            buff = new byte[FragmentLength];

            return stream.Read(buff, 0, FragmentLength) == FragmentLength
                   && this.Send(Fin.Final, opcode, buff, compressed);
        }

        /* Send fragments */

        // Begin

        buff = new byte[FragmentLength];

        var sent = stream.Read(buff, 0, FragmentLength) == FragmentLength
                   && this.Send(Fin.More, opcode, buff, compressed);

        if (!sent)
        {
            return false;
        }

        var n = rem == 0 ? quo - 2 : quo - 1;

        for (long i = 0; i < n; i++)
        {
            sent = stream.Read(buff, 0, FragmentLength) == FragmentLength
                   && this.Send(Fin.More, Opcode.Cont, buff, false);

            if (!sent)
            {
                return false;
            }
        }

        // End

        if (rem == 0)
        {
            rem = FragmentLength;
        }
        else
        {
            buff = new byte[rem];
        }

        return stream.Read(buff, 0, rem) == rem
               && this.Send(Fin.Final, Opcode.Cont, buff, false);
    }

    private bool Send(Fin fin, Opcode opcode, byte[] data, bool compressed)
    {
        lock (this._forState)
        {
            if (this._readyState != WebSocketState.Open)
            {
                this._logger.Trace("The connection is closing.");

                return false;
            }

            var frame = new WebSocketFrame(fin, opcode, data, compressed, this._client);
            var bytes = frame.ToArray();

            return this.SendBytes(bytes);
        }
    }

    private void SendAsync(
      Opcode opcode, Stream stream, Action<bool> completed
    ) => _ = Task.Run(() =>
              {
                  try
                  {
                      var sent = this.Send(opcode, stream);

                      completed?.Invoke(sent);
                  }
                  catch (Exception ex)
                  {
                      this._logger.Error(ex.Message);
                      this._logger.Debug(ex.ToString());

                      this.Error("An exception has occurred during the callback for an async send.", ex);
                  }
              });

    private bool SendBytes(byte[] bytes)
    {
        try
        {
            this._stream.Write(bytes, 0, bytes.Length);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());

            return false;
        }

        return true;
    }

    // As server
    private bool SendHttpResponse(HttpResponse response)
    {
        var fmt = "An HTTP response to {0}:\n{1}";
        var msg = string.Format(fmt, this._context.UserEndPoint, response);

        this._logger.Trace(msg);

        var bytes = response.ToByteArray();

        return this.SendBytes(bytes);
    }

    private void StartReceiving()
    {
        if (this._messageEventQueue.Count > 0)
        {
            this._messageEventQueue.Clear();
        }

        this._pongReceived = new ManualResetEvent(false);
        this._receivingExited = new ManualResetEvent(false);

        if (this._stream == null)
        {
            return;
        }

        void receive() => WebSocketFrame.ReadFrameAsync(
              this._stream,
              false,
              frame =>
              {
                  var cont = this.ProcessReceivedFrame(frame)
                       && this._readyState != WebSocketState.Closed;

                  if (!cont || this._stream == null)
                  {
                      var exited = this._receivingExited;

                      if (exited != null)
                      {
                          _ = exited.Set();
                      }

                      return;
                  }

                  receive();

                  if (this._inMessage)
                  {
                      return;
                  }

                  this.Message();
              },
              ex =>
              {
                  this._logger.Fatal(ex.Message);
                  this._logger.Debug(ex.ToString());

                  this.Fatal("An exception has occurred while receiving.", ex);
              }
            );

        receive();
    }

    #endregion

    #region Internal Methods

    // As server
    internal void Accept()
    {
        var accepted = this.AcceptWS();

        if (!accepted)
        {
            return;
        }

        this.OpenWS();
    }

    // As server
    internal void Close(HttpResponse response)
    {
        this._readyState = WebSocketState.Closing;

        _ = this.SendHttpResponse(response);
        this.ReleaseServerResources();

        this._readyState = WebSocketState.Closed;
    }

    // As server
    internal void Close(HttpStatusCode code) => this.Close(CreateHandshakeFailureResponse(code));

    // As server
    internal void Close(PayloadData payloadData, byte[] frameAsBytes)
    {
        lock (this._forState)
        {
            if (this._readyState == WebSocketState.Closing)
            {
                this._logger.Trace("The closing is already in progress.");

                return;
            }

            if (this._readyState == WebSocketState.Closed)
            {
                this._logger.Trace("The connection has already been closed.");

                return;
            }

            this._readyState = WebSocketState.Closing;
        }

        this._logger.Trace("Begin closing the connection.");

        var sent = frameAsBytes != null && this.SendBytes(frameAsBytes);
        var received = sent && this._receivingExited != null && this._receivingExited.WaitOne(this._waitTime);

        var res = sent && received;

        var msg = string.Format(
                    "The closing was clean? {0} (sent: {1} received: {2})",
                    res,
                    sent,
                    received
                  );

        //this._logger.Debug(msg);

        this.ReleaseServerResources();
        this.ReleaseCommonResources();

        this._logger.Trace("End closing the connection.");

        this._readyState = WebSocketState.Closed;

        var e = new CloseEventArgs(payloadData, res);

        try
        {
            OnClose.Emit(this, e);
        }
        catch (Exception ex)
        {
            this._logger.Error(ex.Message);
            this._logger.Debug(ex.ToString());
        }
    }

    internal static string CreateResponseKey(string base64Key)
    {
        var buff = new StringBuilder(base64Key, 64);
        _ = buff.Append(GUID);
        var sha1 = SHA1.Create();
        var src = sha1.ComputeHash(buff.ToString().GetUTF8EncodedBytes());

        return Convert.ToBase64String(src);
    }

    // As server
    internal void InternalAccept()
    {
        try
        {
            if (!this.AcceptHandshake())
            {
                return;
            }
        }
        catch (Exception ex)
        {
            this._logger.Fatal(ex.Message);
            this._logger.Debug(ex.ToString());

            var msg = "An exception has occurred while attempting to accept.";
            this.Fatal(msg, ex);

            return;
        }

        this._readyState = WebSocketState.Open;

        this.OpenWS();
    }

    // As server
    internal bool Ping(byte[] frameAsBytes, TimeSpan timeout)
    {
        if (this._readyState != WebSocketState.Open)
        {
            return false;
        }

        var received = this._pongReceived;

        if (received == null)
        {
            return false;
        }

        lock (this._forPing)
        {
            try
            {
                _ = received.Reset();

                lock (this._forState)
                {
                    if (this._readyState != WebSocketState.Open)
                    {
                        return false;
                    }

                    var sent = this.SendBytes(frameAsBytes);

                    if (!sent)
                    {
                        return false;
                    }
                }

                return received.WaitOne(timeout);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }

    // As server
    internal void Send(
      Opcode opcode, byte[] data, Dictionary<CompressionMethod, byte[]> cache
    )
    {
        lock (this._forSend)
        {
            lock (this._forState)
            {
                if (this._readyState != WebSocketState.Open)
                {
                    this._logger.Error("The connection is closing.");
                    return;
                }

                if (!cache.TryGetValue(this._compression, out var found))
                {
                    found = new WebSocketFrame(
                              Fin.Final,
                              opcode,
                              data.Compress(this._compression),
                              this._compression != CompressionMethod.None,
                              false
                            )
                            .ToArray();

                    cache.Add(this._compression, found);
                }

                _ = this.SendBytes(found);
            }
        }
    }

    // As server
    internal void Send(
      Opcode opcode, Stream stream, Dictionary<CompressionMethod, Stream> cache
    )
    {
        lock (this._forSend)
        {
            if (!cache.TryGetValue(this._compression, out var found))
            {
                found = stream.Compress(this._compression);
                cache.Add(this._compression, found);
            }
            else
            {
                found.Position = 0;
            }

            _ = this.Send(opcode, found, this._compression != CompressionMethod.None);
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Closes the connection.
    /// </summary>
    /// <remarks>
    /// This method does nothing if the current state of the connection is
    /// Closing or Closed.
    /// </remarks>
    public void Close() => this.Close(1005, string.Empty);

    /// <summary>
    /// Closes the connection with the specified code.
    /// </summary>
    /// <remarks>
    /// This method does nothing if the current state of the connection is
    /// Closing or Closed.
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   A <see cref="ushort"/> that represents the status code indicating
    ///   the reason for the close.
    ///   </para>
    ///   <para>
    ///   The status codes are defined in
    ///   <see href="http://tools.ietf.org/html/rfc6455#section-7.4">
    ///   Section 7.4</see> of RFC 6455.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="code"/> is less than 1000 or greater than 4999.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is 1011 (server error).
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is 1010 (mandatory extension).
    ///   It cannot be used by servers.
    ///   </para>
    /// </exception>
    public void Close(ushort code)
    {
        if (!code.IsCloseStatusCode())
        {
            var msg = "Less than 1000 or greater than 4999.";
            throw new ArgumentOutOfRangeException(nameof(code), msg);
        }

        if (this._client && code == 1011)
        {
            var msg = "1011 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == 1010)
        {
            var msg = "1010 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        this.Close(code, string.Empty);
    }

    /// <summary>
    /// Closes the connection with the specified code.
    /// </summary>
    /// <remarks>
    /// This method does nothing if the current state of the connection is
    /// Closing or Closed.
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   One of the <see cref="CloseStatusCode"/> enum values.
    ///   </para>
    ///   <para>
    ///   It represents the status code indicating the reason for the close.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.ServerError"/>.
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.MandatoryExtension"/>.
    ///   It cannot be used by servers.
    ///   </para>
    /// </exception>
    public void Close(CloseStatusCode code)
    {
        if (this._client && code == CloseStatusCode.ServerError)
        {
            var msg = "ServerError cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == CloseStatusCode.MandatoryExtension)
        {
            var msg = "MandatoryExtension cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        this.Close((ushort)code, string.Empty);
    }

    /// <summary>
    /// Closes the connection with the specified code and reason.
    /// </summary>
    /// <remarks>
    /// This method does nothing if the current state of the connection is
    /// Closing or Closed.
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   A <see cref="ushort"/> that represents the status code indicating
    ///   the reason for the close.
    ///   </para>
    ///   <para>
    ///   The status codes are defined in
    ///   <see href="http://tools.ietf.org/html/rfc6455#section-7.4">
    ///   Section 7.4</see> of RFC 6455.
    ///   </para>
    /// </param>
    /// <param name="reason">
    ///   <para>
    ///   A <see cref="string"/> that represents the reason for the close.
    ///   </para>
    ///   <para>
    ///   The size must be 123 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <para>
    ///   <paramref name="code"/> is less than 1000 or greater than 4999.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The size of <paramref name="reason"/> is greater than 123 bytes.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is 1011 (server error).
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is 1010 (mandatory extension).
    ///   It cannot be used by servers.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is 1005 (no status) and there is reason.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="reason"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    public void Close(ushort code, string reason)
    {
        if (!code.IsCloseStatusCode())
        {
            var msg = "Less than 1000 or greater than 4999.";
            throw new ArgumentOutOfRangeException(nameof(code), msg);
        }

        if (this._client && code == 1011)
        {
            var msg = "1011 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == 1010)
        {
            var msg = "1010 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (reason.IsNullOrEmpty())
        {
            this.CloseWS(code, string.Empty);
            return;
        }

        if (code == 1005)
        {
            var msg = "1005 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!reason.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(reason));
        }

        if (bytes.Length > 123)
        {
            var msg = "Its size is greater than 123 bytes.";
            throw new ArgumentOutOfRangeException(nameof(reason), msg);
        }

        this.CloseWS(code, reason);
    }

    /// <summary>
    /// Closes the connection with the specified code and reason.
    /// </summary>
    /// <remarks>
    /// This method does nothing if the current state of the connection is
    /// Closing or Closed.
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   One of the <see cref="CloseStatusCode"/> enum values.
    ///   </para>
    ///   <para>
    ///   It represents the status code indicating the reason for the close.
    ///   </para>
    /// </param>
    /// <param name="reason">
    ///   <para>
    ///   A <see cref="string"/> that represents the reason for the close.
    ///   </para>
    ///   <para>
    ///   The size must be 123 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.ServerError"/>.
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.MandatoryExtension"/>.
    ///   It cannot be used by servers.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.NoStatus"/> and there is reason.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="reason"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The size of <paramref name="reason"/> is greater than 123 bytes.
    /// </exception>
    public void Close(CloseStatusCode code, string reason)
    {
        if (this._client && code == CloseStatusCode.ServerError)
        {
            var msg = "ServerError cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == CloseStatusCode.MandatoryExtension)
        {
            var msg = "MandatoryExtension cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (reason.IsNullOrEmpty())
        {
            this.CloseWS((ushort)code, string.Empty);
            return;
        }

        if (code == CloseStatusCode.NoStatus)
        {
            var msg = "NoStatus cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!reason.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(reason));
        }

        if (bytes.Length > 123)
        {
            var msg = "Its size is greater than 123 bytes.";
            throw new ArgumentOutOfRangeException(nameof(reason), msg);
        }

        this.CloseWS((ushort)code, reason);
    }

    /// <summary>
    /// Closes the connection asynchronously.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This method does not wait for the close to be complete.
    ///   </para>
    ///   <para>
    ///   This method does nothing if the current state of the connection is
    ///   Closing or Closed.
    ///   </para>
    /// </remarks>
    public void CloseAsync() => this.CloseAsync(1005, string.Empty);

    /// <summary>
    /// Closes the connection asynchronously with the specified code.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This method does not wait for the close to be complete.
    ///   </para>
    ///   <para>
    ///   This method does nothing if the current state of the connection is
    ///   Closing or Closed.
    ///   </para>
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   A <see cref="ushort"/> that represents the status code indicating
    ///   the reason for the close.
    ///   </para>
    ///   <para>
    ///   The status codes are defined in
    ///   <see href="http://tools.ietf.org/html/rfc6455#section-7.4">
    ///   Section 7.4</see> of RFC 6455.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="code"/> is less than 1000 or greater than 4999.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is 1011 (server error).
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is 1010 (mandatory extension).
    ///   It cannot be used by servers.
    ///   </para>
    /// </exception>
    public void CloseAsync(ushort code)
    {
        if (!code.IsCloseStatusCode())
        {
            var msg = "Less than 1000 or greater than 4999.";
            throw new ArgumentOutOfRangeException(nameof(code), msg);
        }

        if (this._client && code == 1011)
        {
            var msg = "1011 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == 1010)
        {
            var msg = "1010 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        this.CloseWSAsync(code, string.Empty);
    }

    /// <summary>
    /// Closes the connection asynchronously with the specified code.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This method does not wait for the close to be complete.
    ///   </para>
    ///   <para>
    ///   This method does nothing if the current state of the connection is
    ///   Closing or Closed.
    ///   </para>
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   One of the <see cref="CloseStatusCode"/> enum values.
    ///   </para>
    ///   <para>
    ///   It represents the status code indicating the reason for the close.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.ServerError"/>.
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.MandatoryExtension"/>.
    ///   It cannot be used by servers.
    ///   </para>
    /// </exception>
    public void CloseAsync(CloseStatusCode code)
    {
        if (this._client && code == CloseStatusCode.ServerError)
        {
            var msg = "ServerError cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == CloseStatusCode.MandatoryExtension)
        {
            var msg = "MandatoryExtension cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        this.CloseWSAsync((ushort)code, string.Empty);
    }

    /// <summary>
    /// Closes the connection asynchronously with the specified code and reason.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This method does not wait for the close to be complete.
    ///   </para>
    ///   <para>
    ///   This method does nothing if the current state of the connection is
    ///   Closing or Closed.
    ///   </para>
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   A <see cref="ushort"/> that represents the status code indicating
    ///   the reason for the close.
    ///   </para>
    ///   <para>
    ///   The status codes are defined in
    ///   <see href="http://tools.ietf.org/html/rfc6455#section-7.4">
    ///   Section 7.4</see> of RFC 6455.
    ///   </para>
    /// </param>
    /// <param name="reason">
    ///   <para>
    ///   A <see cref="string"/> that represents the reason for the close.
    ///   </para>
    ///   <para>
    ///   The size must be 123 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    ///   <para>
    ///   <paramref name="code"/> is less than 1000 or greater than 4999.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The size of <paramref name="reason"/> is greater than 123 bytes.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is 1011 (server error).
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is 1010 (mandatory extension).
    ///   It cannot be used by servers.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is 1005 (no status) and there is reason.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="reason"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    public void CloseAsync(ushort code, string reason)
    {
        if (!code.IsCloseStatusCode())
        {
            var msg = "Less than 1000 or greater than 4999.";
            throw new ArgumentOutOfRangeException(nameof(code), msg);
        }

        if (this._client && code == 1011)
        {
            var msg = "1011 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == 1010)
        {
            var msg = "1010 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (reason.IsNullOrEmpty())
        {
            this.CloseWSAsync(code, string.Empty);
            return;
        }

        if (code == 1005)
        {
            var msg = "1005 cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!reason.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(reason));
        }

        if (bytes.Length > 123)
        {
            var msg = "Its size is greater than 123 bytes.";
            throw new ArgumentOutOfRangeException(nameof(reason), msg);
        }

        this.CloseWSAsync(code, reason);
    }

    /// <summary>
    /// Closes the connection asynchronously with the specified code and reason.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This method does not wait for the close to be complete.
    ///   </para>
    ///   <para>
    ///   This method does nothing if the current state of the connection is
    ///   Closing or Closed.
    ///   </para>
    /// </remarks>
    /// <param name="code">
    ///   <para>
    ///   One of the <see cref="CloseStatusCode"/> enum values.
    ///   </para>
    ///   <para>
    ///   It represents the status code indicating the reason for the close.
    ///   </para>
    /// </param>
    /// <param name="reason">
    ///   <para>
    ///   A <see cref="string"/> that represents the reason for the close.
    ///   </para>
    ///   <para>
    ///   The size must be 123 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.ServerError"/>.
    ///   It cannot be used by clients.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.MandatoryExtension"/>.
    ///   It cannot be used by servers.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="code"/> is
    ///   <see cref="CloseStatusCode.NoStatus"/> and there is reason.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="reason"/> could not be UTF-8-encoded.
    ///   </para>
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The size of <paramref name="reason"/> is greater than 123 bytes.
    /// </exception>
    public void CloseAsync(CloseStatusCode code, string reason)
    {
        if (this._client && code == CloseStatusCode.ServerError)
        {
            var msg = "ServerError cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!this._client && code == CloseStatusCode.MandatoryExtension)
        {
            var msg = "MandatoryExtension cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (reason.IsNullOrEmpty())
        {
            this.CloseWSAsync((ushort)code, string.Empty);
            return;
        }

        if (code == CloseStatusCode.NoStatus)
        {
            var msg = "NoStatus cannot be used.";
            throw new ArgumentException(msg, nameof(code));
        }

        if (!reason.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(reason));
        }

        if (bytes.Length > 123)
        {
            var msg = "Its size is greater than 123 bytes.";
            throw new ArgumentOutOfRangeException(nameof(reason), msg);
        }

        this.CloseWSAsync((ushort)code, reason);
    }

    /// <summary>
    /// Sends a ping using the WebSocket connection.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the send has done with no error and a pong has been
    /// received within a time; otherwise, <c>false</c>.
    /// </returns>
    public bool Ping() => this.Ping(EmptyBytes);

    /// <summary>
    /// Sends a ping with <paramref name="message"/> using the WebSocket
    /// connection.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the send has done with no error and a pong has been
    /// received within a time; otherwise, <c>false</c>.
    /// </returns>
    /// <param name="message">
    ///   <para>
    ///   A <see cref="string"/> that represents the message to send.
    ///   </para>
    ///   <para>
    ///   The size must be 125 bytes or less in UTF-8.
    ///   </para>
    /// </param>
    /// <exception cref="ArgumentException">
    /// <paramref name="message"/> could not be UTF-8-encoded.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The size of <paramref name="message"/> is greater than 125 bytes.
    /// </exception>
    public bool Ping(string message)
    {
        if (message.IsNullOrEmpty())
        {
            return this.Ping(EmptyBytes);
        }

        if (!message.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(message));
        }

        if (bytes.Length > 125)
        {
            var msg = "Its size is greater than 125 bytes.";
            throw new ArgumentOutOfRangeException(nameof(message), msg);
        }

        return this.Ping(bytes);
    }

    /// <summary>
    /// Sends the specified data using the WebSocket connection.
    /// </summary>
    /// <param name="data">
    /// An array of <see cref="byte"/> that represents the binary data to send.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public void Send(byte[] data)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        _ = this.Send(Opcode.Binary, new MemoryStream(data));
    }

    /// <summary>
    /// Sends the specified file using the WebSocket connection.
    /// </summary>
    /// <param name="fileInfo">
    ///   <para>
    ///   A <see cref="FileInfo"/> that specifies the file to send.
    ///   </para>
    ///   <para>
    ///   The file is sent as the binary data.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileInfo"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   The file does not exist.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The file could not be opened.
    ///   </para>
    /// </exception>
    public void Send(FileInfo fileInfo)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (fileInfo == null)
        {
            throw new ArgumentNullException(nameof(fileInfo));
        }

        if (!fileInfo.Exists)
        {
            var msg = "The file does not exist.";
            throw new ArgumentException(msg, nameof(fileInfo));
        }

        if (!fileInfo.TryOpenRead(out var stream))
        {
            var msg = "The file could not be opened.";
            throw new ArgumentException(msg, nameof(fileInfo));
        }

        _ = this.Send(Opcode.Binary, stream);
    }

    /// <summary>
    /// Sends the specified data using the WebSocket connection.
    /// </summary>
    /// <param name="data">
    /// A <see cref="string"/> that represents the text data to send.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="data"/> could not be UTF-8-encoded.
    /// </exception>
    public void Send(string data)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (!data.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(data));
        }

        _ = this.Send(Opcode.Text, new MemoryStream(bytes));
    }

    /// <summary>
    /// Sends the data from the specified stream using the WebSocket connection.
    /// </summary>
    /// <param name="stream">
    ///   <para>
    ///   A <see cref="Stream"/> instance from which to read the data to send.
    ///   </para>
    ///   <para>
    ///   The data is sent as the binary data.
    ///   </para>
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of bytes to send.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="stream"/> cannot be read.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than 1.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   No data could be read from <paramref name="stream"/>.
    ///   </para>
    /// </exception>
    public void Send(Stream stream, int length)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            var msg = "It cannot be read.";
            throw new ArgumentException(msg, nameof(stream));
        }

        if (length < 1)
        {
            var msg = "Less than 1.";
            throw new ArgumentException(msg, nameof(length));
        }

        var bytes = stream.ReadBytes(length);

        var len = bytes.Length;
        if (len == 0)
        {
            var msg = "No data could be read from it.";
            throw new ArgumentException(msg, nameof(stream));
        }

        if (len < length)
        {
            this._logger.Warn(
              string.Format(
                "Only {0} byte(s) of data could be read from the stream.",
                len
              )
            );
        }

        _ = this.Send(Opcode.Binary, new MemoryStream(bytes));
    }

    /// <summary>
    /// Sends the specified data asynchronously using the WebSocket connection.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="data">
    /// An array of <see cref="byte"/> that represents the binary data to send.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    public void SendAsync(byte[] data, Action<bool> completed)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        this.SendAsync(Opcode.Binary, new MemoryStream(data), completed);
    }

    /// <summary>
    /// Sends the specified file asynchronously using the WebSocket connection.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="fileInfo">
    ///   <para>
    ///   A <see cref="FileInfo"/> that specifies the file to send.
    ///   </para>
    ///   <para>
    ///   The file is sent as the binary data.
    ///   </para>
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="fileInfo"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   The file does not exist.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   The file could not be opened.
    ///   </para>
    /// </exception>
    public void SendAsync(FileInfo fileInfo, Action<bool> completed)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (fileInfo == null)
        {
            throw new ArgumentNullException(nameof(fileInfo));
        }

        if (!fileInfo.Exists)
        {
            var msg = "The file does not exist.";
            throw new ArgumentException(msg, nameof(fileInfo));
        }

        if (!fileInfo.TryOpenRead(out var stream))
        {
            var msg = "The file could not be opened.";
            throw new ArgumentException(msg, nameof(fileInfo));
        }

        this.SendAsync(Opcode.Binary, stream, completed);
    }

    /// <summary>
    /// Sends the specified data asynchronously using the WebSocket connection.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="data">
    /// A <see cref="string"/> that represents the text data to send.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="data"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="data"/> could not be UTF-8-encoded.
    /// </exception>
    public void SendAsync(string data, Action<bool> completed)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        if (!data.TryGetUTF8EncodedBytes(out var bytes))
        {
            var msg = "It could not be UTF-8-encoded.";
            throw new ArgumentException(msg, nameof(data));
        }

        this.SendAsync(Opcode.Text, new MemoryStream(bytes), completed);
    }

    /// <summary>
    /// Sends the data from the specified stream asynchronously using
    /// the WebSocket connection.
    /// </summary>
    /// <remarks>
    /// This method does not wait for the send to be complete.
    /// </remarks>
    /// <param name="stream">
    ///   <para>
    ///   A <see cref="Stream"/> instance from which to read the data to send.
    ///   </para>
    ///   <para>
    ///   The data is sent as the binary data.
    ///   </para>
    /// </param>
    /// <param name="length">
    /// An <see cref="int"/> that specifies the number of bytes to send.
    /// </param>
    /// <param name="completed">
    ///   <para>
    ///   An <c>Action&lt;bool&gt;</c> delegate or <see langword="null"/>
    ///   if not needed.
    ///   </para>
    ///   <para>
    ///   The delegate invokes the method called when the send is complete.
    ///   </para>
    ///   <para>
    ///   <c>true</c> is passed to the method if the send has done with
    ///   no error; otherwise, <c>false</c>.
    ///   </para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// The current state of the connection is not Open.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="stream"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///   <para>
    ///   <paramref name="stream"/> cannot be read.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   <paramref name="length"/> is less than 1.
    ///   </para>
    ///   <para>
    ///   -or-
    ///   </para>
    ///   <para>
    ///   No data could be read from <paramref name="stream"/>.
    ///   </para>
    /// </exception>
    public void SendAsync(Stream stream, int length, Action<bool> completed)
    {
        if (this._readyState != WebSocketState.Open)
        {
            var msg = "The current state of the connection is not Open.";
            throw new InvalidOperationException(msg);
        }

        if (stream == null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        if (!stream.CanRead)
        {
            var msg = "It cannot be read.";
            throw new ArgumentException(msg, nameof(stream));
        }

        if (length < 1)
        {
            var msg = "Less than 1.";
            throw new ArgumentException(msg, nameof(length));
        }

        var bytes = stream.ReadBytes(length);

        var len = bytes.Length;
        if (len == 0)
        {
            var msg = "No data could be read from it.";
            throw new ArgumentException(msg, nameof(stream));
        }

        if (len < length)
        {
            this._logger.Warn(
              string.Format(
                "Only {0} byte(s) of data could be read from the stream.",
                len
              )
            );
        }

        this.SendAsync(Opcode.Binary, new MemoryStream(bytes), completed);
    }

    #endregion

    #region Explicit Interface Implementations

    /// <summary>
    /// Closes the connection and releases all associated resources.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///   This method closes the connection with close status 1001 (going away).
    ///   </para>
    ///   <para>
    ///   And this method does nothing if the current state of the connection is
    ///   Closing or Closed.
    ///   </para>
    /// </remarks>
    public void Dispose()
    {
        this.Close(1001, string.Empty);
        GC.SuppressFinalize(this);
    }

    #endregion
}
