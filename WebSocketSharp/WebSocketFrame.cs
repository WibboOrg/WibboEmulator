namespace WibboEmulator.WebSocketSharp;

#region License
/*
 * WebSocketFrame.cs
 *
 * The MIT License
 *
 * Copyright (c) 2012-2021 sta.blockhead
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
 * - Chris Swiedler
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

internal class WebSocketFrame : IEnumerable<byte>
{
    #region Private Fields


    #endregion

    #region Private Constructors

    private WebSocketFrame()
    {
    }

    #endregion

    #region Internal Constructors

    internal WebSocketFrame(Opcode opcode, PayloadData payloadData, bool mask)
      : this(Fin.Final, opcode, payloadData, false, mask)
    {
    }

    internal WebSocketFrame(
      Fin fin, Opcode opcode, byte[] data, bool compressed, bool mask
    )
      : this(fin, opcode, new PayloadData(data), compressed, mask)
    {
    }

    internal WebSocketFrame(
      Fin fin,
      Opcode opcode,
      PayloadData payloadData,
      bool compressed,
      bool mask
    )
    {
        this.Fin = fin;
        this.Opcode = opcode;

        this.Rsv1 = opcode.IsData() && compressed ? Rsv.On : Rsv.Off;
        this.Rsv2 = Rsv.Off;
        this.Rsv3 = Rsv.Off;

        var len = payloadData.Length;

        if (len < 126)
        {
            this.PayloadLength = (byte)len;
            this.ExtendedPayloadLength = WebSocket.EmptyBytes;
        }
        else if (len < 0x010000)
        {
            this.PayloadLength = 126;
            this.ExtendedPayloadLength = ((ushort)len).ToByteArray(ByteOrder.Big);
        }
        else
        {
            this.PayloadLength = 127;
            this.ExtendedPayloadLength = len.ToByteArray(ByteOrder.Big);
        }

        if (mask)
        {
            this.Mask = Mask.On;
            this.MaskingKey = CreateMaskingKey();

            payloadData.Mask(this.MaskingKey);
        }
        else
        {
            this.Mask = Mask.Off;
            this.MaskingKey = WebSocket.EmptyBytes;
        }

        this.PayloadData = payloadData;
    }

    #endregion

    #region Internal Properties

    internal ulong ExactPayloadLength => this.PayloadLength < 126
                   ? this.PayloadLength
                   : this.PayloadLength == 126
                     ? this.ExtendedPayloadLength.ToUInt16(ByteOrder.Big)
                     : this.ExtendedPayloadLength.ToUInt64(ByteOrder.Big);

    internal int ExtendedPayloadLengthWidth => this.PayloadLength < 126
                   ? 0
                   : this.PayloadLength == 126
                     ? 2
                     : 8;

    #endregion

    #region Public Properties

    public byte[] ExtendedPayloadLength { get; private set; }

    public Fin Fin { get; private set; }

    public bool IsBinary => this.Opcode == Opcode.Binary;

    public bool IsClose => this.Opcode == Opcode.Close;

    public bool IsCompressed => this.Rsv1 == Rsv.On;

    public bool IsContinuation => this.Opcode == Opcode.Cont;

    public bool IsControl => this.Opcode >= Opcode.Close;

    public bool IsData => this.Opcode is Opcode.Text or Opcode.Binary;

    public bool IsFinal => this.Fin == Fin.Final;

    public bool IsFragment => this.Fin == Fin.More || this.Opcode == Opcode.Cont;

    public bool IsMasked => this.Mask == Mask.On;

    public bool IsPing => this.Opcode == Opcode.Ping;

    public bool IsPong => this.Opcode == Opcode.Pong;

    public bool IsText => this.Opcode == Opcode.Text;

    public ulong Length => 2
                   + (ulong)(this.ExtendedPayloadLength.Length + this.MaskingKey.Length)
                   + this.PayloadData.Length;

    public Mask Mask { get; private set; }

    public byte[] MaskingKey { get; private set; }

    public Opcode Opcode { get; private set; }

    public PayloadData PayloadData { get; private set; }

    public byte PayloadLength { get; private set; }

    public Rsv Rsv1 { get; private set; }

    public Rsv Rsv2 { get; private set; }

    public Rsv Rsv3 { get; private set; }

    #endregion

    #region Private Methods

    private static byte[] CreateMaskingKey()
    {
        var key = new byte[4];

        WebSocket.RandomNumber.GetBytes(key);

        return key;
    }

    private static string Dump(WebSocketFrame frame)
    {
        var len = frame.Length;
        var cnt = (long)(len / 4);
        var rem = (int)(len % 4);

        int cntDigit;
        string cntFmt;

        if (cnt < 10000)
        {
            cntDigit = 4;
            cntFmt = "{0,4}";
        }
        else if (cnt < 0x010000)
        {
            cntDigit = 4;
            cntFmt = "{0,4:X}";
        }
        else if (cnt < 0x0100000000)
        {
            cntDigit = 8;
            cntFmt = "{0,8:X}";
        }
        else
        {
            cntDigit = 16;
            cntFmt = "{0,16:X}";
        }

        var spFmt = string.Format("{{0,{0}}}", cntDigit);

        var headerFmt = string.Format(
                          @"
{0} 01234567 89ABCDEF 01234567 89ABCDEF
{0}+--------+--------+--------+--------+\n",
                          spFmt
                        );

        var lineFmt = string.Format(
                        "{0}|{{1,8}} {{2,8}} {{3,8}} {{4,8}}|\n", cntFmt
                      );

        var footerFmt = string.Format(
                          "{0}+--------+--------+--------+--------+", spFmt
                        );

        var buff = new StringBuilder(64);

        Action<string, string, string, string> linePrinter()
        {
            long lineCnt = 0;

            return (arg1, arg2, arg3, arg4) => _ = buff.AppendFormat(
            lineFmt, ++lineCnt, arg1, arg2, arg3, arg4
          );
        }

        var printLine = linePrinter();
        var bytes = frame.ToArray();

        _ = buff.AppendFormat(headerFmt, string.Empty);

        for (long i = 0; i <= cnt; i++)
        {
            var j = i * 4;

            if (i < cnt)
            {
                printLine(
                  Convert.ToString(bytes[j], 2).PadLeft(8, '0'),
                  Convert.ToString(bytes[j + 1], 2).PadLeft(8, '0'),
                  Convert.ToString(bytes[j + 2], 2).PadLeft(8, '0'),
                  Convert.ToString(bytes[j + 3], 2).PadLeft(8, '0')
                );

                continue;
            }

            if (rem > 0)
            {
                printLine(
                  Convert.ToString(bytes[j], 2).PadLeft(8, '0'),
                  rem >= 2
                  ? Convert.ToString(bytes[j + 1], 2).PadLeft(8, '0')
                  : string.Empty,
                  rem == 3
                  ? Convert.ToString(bytes[j + 2], 2).PadLeft(8, '0')
                  : string.Empty,
                  string.Empty
                );
            }
        }

        _ = buff.AppendFormat(footerFmt, string.Empty);

        return buff.ToString();
    }

    private static string Print(WebSocketFrame frame)
    {
        // Payload Length
        var payloadLen = frame.PayloadLength;

        // Extended Payload Length
        var extPayloadLen = payloadLen > 125
                            ? frame.ExactPayloadLength.ToString()
                            : string.Empty;

        // Masking Key
        var maskingKey = BitConverter.ToString(frame.MaskingKey);

        // Payload Data
        var payload = payloadLen == 0
                      ? string.Empty
                      : payloadLen > 125
                        ? "---"
                        : !frame.IsText
                          || frame.IsFragment
                          || frame.IsMasked
                          || frame.IsCompressed
                          ? frame.PayloadData.ToString()
                          : Utf8Decode(frame.PayloadData.ApplicationData);

        var fmt = @"
                    FIN: {0}
                   RSV1: {1}
                   RSV2: {2}
                   RSV3: {3}
                 Opcode: {4}
                   MASK: {5}
         Payload Length: {6}
Extended Payload Length: {7}
            Masking Key: {8}
           Payload Data: {9}";

        return string.Format(
                 fmt,
                 frame.Fin,
                 frame.Rsv1,
                 frame.Rsv2,
                 frame.Rsv3,
                 frame.Opcode,
                 frame.Mask,
                 payloadLen,
                 extPayloadLen,
                 maskingKey,
                 payload
               );
    }

    private static WebSocketFrame ProcessHeader(byte[] header)
    {
        if (header.Length != 2)
        {
            var msg = "The header part of a frame could not be read.";

            throw new WebSocketException(msg);
        }

        // FIN
        var fin = (header[0] & 0x80) == 0x80 ? Fin.Final : Fin.More;

        // RSV1
        var rsv1 = (header[0] & 0x40) == 0x40 ? Rsv.On : Rsv.Off;

        // RSV2
        var rsv2 = (header[0] & 0x20) == 0x20 ? Rsv.On : Rsv.Off;

        // RSV3
        var rsv3 = (header[0] & 0x10) == 0x10 ? Rsv.On : Rsv.Off;

        // Opcode
        var opcode = (byte)(header[0] & 0x0f);

        // MASK
        var mask = (header[1] & 0x80) == 0x80 ? Mask.On : Mask.Off;

        // Payload Length
        var payloadLen = (byte)(header[1] & 0x7f);

        if (!opcode.IsSupported())
        {
            var msg = "A frame has an unsupported opcode.";

            throw new WebSocketException(CloseStatusCode.ProtocolError, msg);
        }

        if (!opcode.IsData() && rsv1 == Rsv.On)
        {
            var msg = "A non data frame is compressed.";

            throw new WebSocketException(CloseStatusCode.ProtocolError, msg);
        }

        if (opcode.IsControl())
        {
            if (fin == Fin.More)
            {
                var msg = "A control frame is fragmented.";

                throw new WebSocketException(CloseStatusCode.ProtocolError, msg);
            }

            if (payloadLen > 125)
            {
                var msg = "A control frame has too long payload length.";

                throw new WebSocketException(CloseStatusCode.ProtocolError, msg);
            }
        }

        var frame = new WebSocketFrame
        {
            Fin = fin,
            Rsv1 = rsv1,
            Rsv2 = rsv2,
            Rsv3 = rsv3,
            Opcode = (Opcode)opcode,
            Mask = mask,
            PayloadLength = payloadLen
        };

        return frame;
    }

    private static WebSocketFrame ReadExtendedPayloadLength(
      Stream stream, WebSocketFrame frame
    )
    {
        if (stream == null)
        {
            return frame;
        }

        var len = frame.ExtendedPayloadLengthWidth;

        if (len == 0)
        {
            frame.ExtendedPayloadLength = WebSocket.EmptyBytes;

            return frame;
        }

        var bytes = stream.ReadBytes(len);

        if (bytes.Length != len)
        {
            var msg = "The extended payload length of a frame could not be read.";

            throw new WebSocketException(msg);
        }

        frame.ExtendedPayloadLength = bytes;

        return frame;
    }

    private static void ReadExtendedPayloadLengthAsync(
      Stream stream,
      WebSocketFrame frame,
      Action<WebSocketFrame> completed,
      Action<Exception> error
    )
    {
        var len = frame.ExtendedPayloadLengthWidth;

        if (len == 0)
        {
            frame.ExtendedPayloadLength = WebSocket.EmptyBytes;

            completed(frame);

            return;
        }

        stream.ReadBytesAsync(
          len,
          bytes =>
          {
              if (bytes.Length != len)
              {
                  var msg = "The extended payload length of a frame could not be read.";

                  throw new WebSocketException(msg);
              }

              frame.ExtendedPayloadLength = bytes;

              completed(frame);
          },
          error
        );
    }

    private static WebSocketFrame ReadHeader(Stream stream)
    {
        var bytes = stream.ReadBytes(2);

        return ProcessHeader(bytes);
    }

    private static void ReadHeaderAsync(Stream stream, Action<WebSocketFrame> completed, Action<Exception> error) =>
        stream.ReadBytesAsync(2,
          bytes =>
          {
              var frame = ProcessHeader(bytes);

              completed(frame);
          },
          error
        );

    private static WebSocketFrame ReadMaskingKey(
      Stream stream, WebSocketFrame frame
    )
    {
        if (!frame.IsMasked)
        {
            frame.MaskingKey = WebSocket.EmptyBytes;

            return frame;
        }

        var len = 4;
        var bytes = stream.ReadBytes(len);

        if (bytes.Length != len)
        {
            var msg = "The masking key of a frame could not be read.";

            throw new WebSocketException(msg);
        }

        frame.MaskingKey = bytes;

        return frame;
    }

    private static void ReadMaskingKeyAsync(
      Stream stream,
      WebSocketFrame frame,
      Action<WebSocketFrame> completed,
      Action<Exception> error
    )
    {
        if (!frame.IsMasked)
        {
            frame.MaskingKey = WebSocket.EmptyBytes;

            completed(frame);

            return;
        }

        var len = 4;

        stream.ReadBytesAsync(
          len,
          bytes =>
          {
              if (bytes.Length != len)
              {
                  var msg = "The masking key of a frame could not be read.";

                  throw new WebSocketException(msg);
              }

              frame.MaskingKey = bytes;

              completed(frame);
          },
          error
        );
    }

    private static WebSocketFrame ReadPayloadData(
      Stream stream, WebSocketFrame frame
    )
    {
        var exactLen = frame.ExactPayloadLength;

        if (exactLen > PayloadData.MaxLength)
        {
            var msg = "A frame has too long payload length.";

            throw new WebSocketException(CloseStatusCode.TooBig, msg);
        }

        if (exactLen == 0)
        {
            frame.PayloadData = PayloadData.Empty;

            return frame;
        }

        var len = (long)exactLen;
        var bytes = frame.PayloadLength < 127
                    ? stream.ReadBytes((int)exactLen)
                    : stream.ReadBytes(len, 1024);

        if (bytes.LongLength != len)
        {
            var msg = "The payload data of a frame could not be read.";

            throw new WebSocketException(msg);
        }

        frame.PayloadData = new PayloadData(bytes, len);

        return frame;
    }

    private static void ReadPayloadDataAsync(
      Stream stream,
      WebSocketFrame frame,
      Action<WebSocketFrame> completed,
      Action<Exception> error
    )
    {
        var exactLen = frame.ExactPayloadLength;

        if (exactLen > PayloadData.MaxLength)
        {
            var msg = "A frame has too long payload length.";

            throw new WebSocketException(CloseStatusCode.TooBig, msg);
        }

        if (exactLen == 0)
        {
            frame.PayloadData = PayloadData.Empty;

            completed(frame);

            return;
        }

        var len = (long)exactLen;

        void comp(byte[] bytes)
        {
            if (bytes.LongLength != len)
            {
                var msg = "The payload data of a frame could not be read.";

                throw new WebSocketException(msg);
            }

            frame.PayloadData = new PayloadData(bytes, len);

            completed(frame);
        }

        if (frame.PayloadLength < 127)
        {
            stream.ReadBytesAsync((int)exactLen, comp, error);

            return;
        }

        stream.ReadBytesAsync(len, 1024, comp, error);
    }

    private static string Utf8Decode(byte[] bytes)
    {
        try
        {
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return null;
        }
    }

    #endregion

    #region Internal Methods

    internal static WebSocketFrame CreateCloseFrame(
      PayloadData payloadData, bool mask
    ) => new(Fin.Final, Opcode.Close, payloadData, false, mask);

    internal static WebSocketFrame CreatePingFrame(bool mask) => new(Fin.Final, Opcode.Ping, PayloadData.Empty, false, mask);

    internal static WebSocketFrame CreatePingFrame(byte[] data, bool mask) => new(Fin.Final, Opcode.Ping, new PayloadData(data), false, mask);

    internal static WebSocketFrame CreatePongFrame(
      PayloadData payloadData, bool mask
    ) => new(Fin.Final, Opcode.Pong, payloadData, false, mask);

    internal static WebSocketFrame ReadFrame(Stream stream, bool unmask)
    {
        var frame = ReadHeader(stream);

        _ = ReadExtendedPayloadLength(stream, frame);
        _ = ReadMaskingKey(stream, frame);
        _ = ReadPayloadData(stream, frame);

        if (unmask)
        {
            frame.Unmask();
        }

        return frame;
    }

    internal static void ReadFrameAsync(
      Stream stream,
      bool unmask,
      Action<WebSocketFrame> completed,
      Action<Exception> error
    ) => ReadHeaderAsync(
          stream,
          frame =>
            ReadExtendedPayloadLengthAsync(
              stream,
              frame,
              frame1 =>
                ReadMaskingKeyAsync(
                  stream,
                  frame1,
                  frame2 =>
                    ReadPayloadDataAsync(
                      stream,
                      frame2,
                      frame3 =>
                      {
                          if (unmask)
                          {
                              frame3.Unmask();
                          }

                          completed(frame3);
                      },
                      error
                    ),
                  error
                ),
              error
            ),
          error
        );

    internal void Unmask()
    {
        if (this.Mask == Mask.Off)
        {
            return;
        }

        this.PayloadData.Mask(this.MaskingKey);

        this.MaskingKey = WebSocket.EmptyBytes;
        this.Mask = Mask.Off;
    }

    #endregion

    #region Public Methods

    public IEnumerator<byte> GetEnumerator()
    {
        foreach (var b in this.ToArray())
        {
            yield return b;
        }
    }

    public void Print(bool dumped)
    {
        var val = dumped ? Dump(this) : Print(this);

        Console.WriteLine(val);
    }

    public string PrintToString(bool dumped) => dumped ? Dump(this) : Print(this);

    public byte[] ToArray()
    {
        using var buff = new MemoryStream();
        var header = (int)this.Fin;
        header = (header << 1) + (int)this.Rsv1;
        header = (header << 1) + (int)this.Rsv2;
        header = (header << 1) + (int)this.Rsv3;
        header = (header << 4) + (int)this.Opcode;
        header = (header << 1) + (int)this.Mask;
        header = (header << 7) + this.PayloadLength;

        var headerAsUshort = (ushort)header;
        var headerAsBytes = headerAsUshort.ToByteArray(ByteOrder.Big);

        buff.Write(headerAsBytes, 0, 2);

        if (this.PayloadLength > 125)
        {
            var cnt = this.PayloadLength == 126 ? 2 : 8;

            buff.Write(this.ExtendedPayloadLength, 0, cnt);
        }

        if (this.Mask == Mask.On)
        {
            buff.Write(this.MaskingKey, 0, 4);
        }

        if (this.PayloadLength > 0)
        {
            var bytes = this.PayloadData.ToArray();

            if (this.PayloadLength < 127)
            {
                buff.Write(bytes, 0, bytes.Length);
            }
            else
            {
                buff.WriteBytes(bytes, 1024);
            }
        }

        buff.Close();

        return buff.ToArray();
    }

    public override string ToString()
    {
        var val = this.ToArray();

        return BitConverter.ToString(val);
    }

    #endregion

    #region Explicit Interface Implementations

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion
}
