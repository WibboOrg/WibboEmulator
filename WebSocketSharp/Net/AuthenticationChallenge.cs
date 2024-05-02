namespace WibboEmulator.WebSocketSharp.Net;

#region License
/*
 * AuthenticationChallenge.cs
 *
 * The MIT License
 *
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

using System.Collections.Specialized;
using System.Text;

internal sealed class AuthenticationChallenge : AuthenticationBase
{
    #region Private Constructors

    private AuthenticationChallenge(AuthenticationSchemes scheme, NameValueCollection parameters)
      : base(scheme, parameters)
    {
    }

    #endregion

    #region Internal Constructors

    internal AuthenticationChallenge(AuthenticationSchemes scheme, string realm)
      : base(scheme, [])
    {
        this.Parameters["realm"] = realm;
        if (scheme == AuthenticationSchemes.Digest)
        {
            this.Parameters["nonce"] = CreateNonceValue();
            this.Parameters["algorithm"] = "MD5";
            this.Parameters["qop"] = "auth";
        }
    }

    #endregion

    #region Public Properties

    public string Domain => this.Parameters["domain"];

    public string Stale => this.Parameters["stale"];

    #endregion

    #region Internal Methods

    internal static AuthenticationChallenge CreateBasicChallenge(string realm) => new(AuthenticationSchemes.Basic, realm);

    internal static AuthenticationChallenge CreateDigestChallenge(string realm) => new(AuthenticationSchemes.Digest, realm);

    internal static AuthenticationChallenge Parse(string value)
    {
        var chal = value.Split(new[] { ' ' }, 2);
        if (chal.Length != 2)
        {
            return null;
        }

        var schm = chal[0].ToLower();
        return schm == "basic"
               ? new AuthenticationChallenge(
                   AuthenticationSchemes.Basic, ParseParameters(chal[1]))
               : schm == "digest"
                 ? new AuthenticationChallenge(
                     AuthenticationSchemes.Digest, ParseParameters(chal[1]))
                 : null;
    }

    internal override string ToBasicString() => string.Format("Basic realm=\"{0}\"", this.Parameters["realm"]);

    internal override string ToDigestString()
    {
        var output = new StringBuilder(128);

        var domain = this.Parameters["domain"];
        if (domain != null)
        {
            _ = output.AppendFormat(
              "Digest realm=\"{0}\", domain=\"{1}\", nonce=\"{2}\"",
              this.Parameters["realm"],
              domain,
              this.Parameters["nonce"]);
        }
        else
        {
            _ = output.AppendFormat(
              "Digest realm=\"{0}\", nonce=\"{1}\"", this.Parameters["realm"], this.Parameters["nonce"]);
        }

        var opaque = this.Parameters["opaque"];
        if (opaque != null)
        {
            _ = output.AppendFormat(", opaque=\"{0}\"", opaque);
        }

        var stale = this.Parameters["stale"];
        if (stale != null)
        {
            _ = output.AppendFormat(", stale={0}", stale);
        }

        var algo = this.Parameters["algorithm"];
        if (algo != null)
        {
            _ = output.AppendFormat(", algorithm={0}", algo);
        }

        var qop = this.Parameters["qop"];
        if (qop != null)
        {
            _ = output.AppendFormat(", qop=\"{0}\"", qop);
        }

        return output.ToString();
    }

    #endregion
}
