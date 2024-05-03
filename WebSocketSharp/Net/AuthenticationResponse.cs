namespace WibboEmulator.WebSocketSharp.Net;

#region License
/*
 * AuthenticationResponse.cs
 *
 * ParseBasicCredentials is derived from System.Net.HttpListenerContext.cs of Mono
 * (http://www.mono-project.com).
 *
 * The MIT License
 *
 * Copyright (c) 2005 Novell, Inc. (http://www.novell.com)
 * Copyright (c) 2013-2014 sta.blockhead
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
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using WebSocketSharp;

internal sealed class AuthenticationResponse : AuthenticationBase
{
    #region Private Fields

    private uint _nonceCount;

    #endregion

    #region Private Constructors

    private AuthenticationResponse(AuthenticationSchemes scheme, NameValueCollection parameters)
      : base(scheme, parameters)
    {
    }

    #endregion

    #region Internal Constructors

    internal AuthenticationResponse(NetworkCredential credentials)
      : this(AuthenticationSchemes.Basic, [], credentials, 0)
    {
    }

    internal AuthenticationResponse(
      AuthenticationChallenge challenge, NetworkCredential credentials, uint nonceCount)
      : this(challenge.Scheme, challenge.Parameters, credentials, nonceCount)
    {
    }

    internal AuthenticationResponse(
      AuthenticationSchemes scheme,
      NameValueCollection parameters,
      NetworkCredential credentials,
      uint nonceCount)
      : base(scheme, parameters)
    {
        this.Parameters["username"] = credentials.Username;
        this.Parameters["password"] = credentials.Password;
        this.Parameters["uri"] = credentials.Domain;
        this._nonceCount = nonceCount;
        if (scheme == AuthenticationSchemes.Digest)
        {
            this.InitAsDigest();
        }
    }

    #endregion

    #region Internal Properties

    internal uint NonceCount => this._nonceCount < uint.MaxValue
                   ? this._nonceCount
                   : 0;

    #endregion

    #region Public Properties

    public string Cnonce => this.Parameters["cnonce"];

    public string Nc => this.Parameters["nc"];

    public string Password => this.Parameters["password"];

    public string Response => this.Parameters["response"];

    public string Uri => this.Parameters["uri"];

    public string UserName => this.Parameters["username"];

    #endregion

    #region Private Methods

    private static string CreateA1(string username, string password, string realm) => string.Format("{0}:{1}:{2}", username, realm, password);

    private static string CreateA1(
      string username, string password, string realm, string nonce, string cnonce) => string.Format(
          "{0}:{1}:{2}", Hash(CreateA1(username, password, realm)), nonce, cnonce);

    private static string CreateA2(string method, string uri) => string.Format("{0}:{1}", method, uri);

    private static string CreateA2(string method, string uri, string entity) => string.Format("{0}:{1}:{2}", method, uri, Hash(entity));

    private static string Hash(string value)
    {
        var src = Encoding.UTF8.GetBytes(value);
        var hashed = MD5.HashData(src);

        var res = new StringBuilder(64);
        foreach (var b in hashed)
        {
            _ = res.Append(b.ToString("x2"));
        }

        return res.ToString();
    }

    private void InitAsDigest()
    {
        var qops = this.Parameters["qop"];
        if (qops != null)
        {
            if (qops.Split(',').Contains(qop => qop.Trim().Equals("auth", StringComparison.CurrentCultureIgnoreCase)))
            {
                this.Parameters["qop"] = "auth";
                this.Parameters["cnonce"] = CreateNonceValue();
                this.Parameters["nc"] = string.Format("{0:x8}", ++this._nonceCount);
            }
            else
            {
                this.Parameters["qop"] = null;
            }
        }

        this.Parameters["method"] = "GET";
        this.Parameters["response"] = CreateRequestDigest(this.Parameters);
    }

    #endregion

    #region Internal Methods

    internal static string CreateRequestDigest(NameValueCollection parameters)
    {
        var user = parameters["username"];
        var pass = parameters["password"];
        var realm = parameters["realm"];
        var nonce = parameters["nonce"];
        var uri = parameters["uri"];
        var algo = parameters["algorithm"];
        var qop = parameters["qop"];
        var cnonce = parameters["cnonce"];
        var nc = parameters["nc"];
        var method = parameters["method"];

        var a1 = algo != null && algo.Equals("md5-sess"
, StringComparison.CurrentCultureIgnoreCase)
                 ? CreateA1(user, pass, realm, nonce, cnonce)
                 : CreateA1(user, pass, realm);

        var a2 = qop != null && qop.Equals("auth-int"
, StringComparison.CurrentCultureIgnoreCase)
                 ? CreateA2(method, uri, parameters["entity"])
                 : CreateA2(method, uri);

        var secret = Hash(a1);
        var data = qop != null
                   ? string.Format("{0}:{1}:{2}:{3}:{4}", nonce, nc, cnonce, qop, Hash(a2))
                   : string.Format("{0}:{1}", nonce, Hash(a2));

        return Hash(string.Format("{0}:{1}", secret, data));
    }

    internal static AuthenticationResponse Parse(string value)
    {
        try
        {
            var cred = value.Split([' '], 2);
            if (cred.Length != 2)
            {
                return null;
            }

            var schm = cred[0].ToLower();
            return schm == "basic"
                   ? new AuthenticationResponse(
                       AuthenticationSchemes.Basic, ParseBasicCredentials(cred[1]))
                   : schm == "digest"
                     ? new AuthenticationResponse(
                         AuthenticationSchemes.Digest, ParseParameters(cred[1]))
                     : null;
        }
        catch
        {
        }

        return null;
    }

    internal static NameValueCollection ParseBasicCredentials(string value)
    {
        // Decode the basic-credentials (a Base64 encoded string).
        var userPass = Encoding.Default.GetString(Convert.FromBase64String(value));

        // The format is [<domain>\]<username>:<password>.
        var i = userPass.IndexOf(':');
        var user = userPass[..i];
        var pass = i < userPass.Length - 1 ? userPass[(i + 1)..] : string.Empty;

        // Check if 'domain' exists.
        i = user.IndexOf('\\');
        if (i > -1)
        {
            user = user[(i + 1)..];
        }

        var res = new NameValueCollection
        {
            ["username"] = user,
            ["password"] = pass
        };

        return res;
    }

    internal override string ToBasicString()
    {
        var userPass = string.Format("{0}:{1}", this.Parameters["username"], this.Parameters["password"]);
        var cred = Convert.ToBase64String(Encoding.UTF8.GetBytes(userPass));

        return "Basic " + cred;
    }

    internal override string ToDigestString()
    {
        var output = new StringBuilder(256);
        _ = output.AppendFormat(
          "Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", response=\"{4}\"",
          this.Parameters["username"],
          this.Parameters["realm"],
          this.Parameters["nonce"],
          this.Parameters["uri"],
          this.Parameters["response"]);

        var opaque = this.Parameters["opaque"];
        if (opaque != null)
        {
            _ = output.AppendFormat(", opaque=\"{0}\"", opaque);
        }

        var algo = this.Parameters["algorithm"];
        if (algo != null)
        {
            _ = output.AppendFormat(", algorithm={0}", algo);
        }

        var qop = this.Parameters["qop"];
        if (qop != null)
        {
            _ = output.AppendFormat(
              ", qop={0}, cnonce=\"{1}\", nc={2}", qop, this.Parameters["cnonce"], this.Parameters["nc"]);
        }

        return output.ToString();
    }

    #endregion

    #region Public Methods

    public IIdentity ToIdentity()
    {
        var schm = this.Scheme;
        return schm == AuthenticationSchemes.Basic
               ? new HttpBasicIdentity(this.Parameters["username"], this.Parameters["password"])
               : schm == AuthenticationSchemes.Digest
                 ? new HttpDigestIdentity(this.Parameters)
                 : null;
    }

    #endregion
}
